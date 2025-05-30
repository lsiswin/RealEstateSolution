using AutoMapper;
using RealEstateSolution.Common.Utils;
using RealEstateSolution.ContractService.Models;
using RealEstateSolution.ContractService.Repository;
using RealEstateSolution.Database.Models;
using RealEstateSolution.Common.Repository;
using RealEstateSolution.ContractService.Data;

namespace RealEstateSolution.ContractService.Services;

/// <summary>
/// 合同服务实现类
/// </summary>
public class ContractService : IContractService
{
    private readonly IContractRepository _contractRepository;
    private readonly IContractTemplateService _templateService;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork<ContractDbContext> _unitOfWork;

    public ContractService(
        IContractRepository contractRepository, 
        IContractTemplateService templateService, 
        IMapper mapper,
        IUnitOfWork<ContractDbContext> unitOfWork)
    {
        _contractRepository = contractRepository;
        _templateService = templateService;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PagedList<ContractDto>>> GetContractsAsync(ContractQueryDto query)
    {
        try
        {
            var contracts = await _contractRepository.SearchContractsAsync(
                query.Keyword, 
                query.Type, 
                query.Status, 
                query.StartDate, 
                query.EndDate);

            // 应用分页
            var totalCount = contracts.Count();
            var pagedContracts = contracts
                .Skip((query.PageIndex - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            var contractDtos = _mapper.Map<List<ContractDto>>(pagedContracts);
            
            var pagedResult = new PagedList<ContractDto>(
                contractDtos,
                totalCount,
                query.PageIndex,
                query.PageSize);

            return new ApiResponse<PagedList<ContractDto>>
            {
                Success = true,
                Data = pagedResult,
                Message = "获取合同列表成功"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<PagedList<ContractDto>>
            {
                Success = false,
                Message = $"获取合同列表失败: {ex.Message}"
            };
        }
    }

    public async Task<ApiResponse<ContractDto>> GetContractByIdAsync(int id)
    {
        try
        {
            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null)
            {
                return new ApiResponse<ContractDto>
                {
                    Success = false,
                    Message = "合同不存在"
                };
            }

            var contractDto = _mapper.Map<ContractDto>(contract);
            return new ApiResponse<ContractDto>
            {
                Success = true,
                Data = contractDto,
                Message = "获取合同详情成功"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<ContractDto>
            {
                Success = false,
                Message = $"获取合同详情失败: {ex.Message}"
            };
        }
    }

    public async Task<ApiResponse<ContractDto>> CreateContractAsync(ContractDto contractDto, string userId)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var contract = _mapper.Map<Contract>(contractDto);
            contract.ContractNumber = await GenerateContractNumberAsync(contract.Type);
            contract.CreateTime = DateTime.Now;
            contract.UpdateTime = DateTime.Now;
            contract.Status = ContractStatus.Draft;

            await _contractRepository.AddAsync(contract);
            await _unitOfWork.CommitAsync();
            
            var resultDto = _mapper.Map<ContractDto>(contract);
            return new ApiResponse<ContractDto>
            {
                Success = true,
                Data = resultDto,
                Message = "创建合同成功"
            };
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return new ApiResponse<ContractDto>
            {
                Success = false,
                Message = $"创建合同失败: {ex.Message}"
            };
        }
    }

    public async Task<ApiResponse<ContractDto>> UpdateContractAsync(int id, ContractDto contractDto, string userId)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var existingContract = await _contractRepository.GetByIdAsync(id);
            if (existingContract == null)
            {
                return new ApiResponse<ContractDto>
                {
                    Success = false,
                    Message = "合同不存在"
                };
            }

            // 更新允许修改的字段
            existingContract.Amount = contractDto.Amount;
            existingContract.StartDate = contractDto.StartDate ?? DateTime.Now;
            existingContract.EndDate = contractDto.EndDate;
            existingContract.Terms = contractDto.Terms ?? string.Empty;
            existingContract.Remark = contractDto.Notes;
            existingContract.UpdateTime = DateTime.Now;

            _contractRepository.Update(existingContract);
            await _unitOfWork.CommitAsync();
            
            var resultDto = _mapper.Map<ContractDto>(existingContract);
            return new ApiResponse<ContractDto>
            {
                Success = true,
                Data = resultDto,
                Message = "更新合同成功"
            };
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return new ApiResponse<ContractDto>
            {
                Success = false,
                Message = $"更新合同失败: {ex.Message}"
            };
        }
    }

    public async Task<ApiResponse> DeleteContractAsync(int id, string userId)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "合同不存在"
                };
            }

            // 使用仓储的删除方法
            _contractRepository.Delete(contract);
            await _unitOfWork.CommitAsync();

            return new ApiResponse
            {
                Success = true,
                Message = "删除合同成功"
            };
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return new ApiResponse
            {
                Success = false,
                Message = $"删除合同失败: {ex.Message}"
            };
        }
    }

    public async Task<ApiResponse<ContractDto>> UpdateContractStatusAsync(int id, ContractStatus status, string userId)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null)
            {
                return new ApiResponse<ContractDto>
                {
                    Success = false,
                    Message = "合同不存在"
                };
            }

            contract.Status = status;
            contract.UpdateTime = DateTime.Now;
            
            // 如果状态变为已签署，设置签署日期
            if (status == ContractStatus.Signed && !contract.SignTime.HasValue)
            {
                contract.SignTime = DateTime.Now;
            }

            _contractRepository.Update(contract);
            await _unitOfWork.CommitAsync();
            
            var resultDto = _mapper.Map<ContractDto>(contract);
            return new ApiResponse<ContractDto>
            {
                Success = true,
                Data = resultDto,
                Message = "更新合同状态成功"
            };
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return new ApiResponse<ContractDto>
            {
                Success = false,
                Message = $"更新合同状态失败: {ex.Message}"
            };
        }
    }

    public async Task<ApiResponse<List<ContractTemplateDto>>> GetContractTemplatesAsync(ContractType? type = null)
    {
        try
        {
            var result = await _templateService.GetActiveTemplatesByTypeAsync(type);
            return result;
        }
        catch (Exception ex)
        {
            return new ApiResponse<List<ContractTemplateDto>>
            {
                Success = false,
                Message = $"获取合同模板失败: {ex.Message}"
            };
        }
    }

    public async Task<ApiResponse<ContractDto>> CreateContractFromTemplateAsync(int templateId, ContractDto contractDto, string userId)
    {
        try
        {
            // 获取模板详情
            var templateResult = await _templateService.GetTemplateByIdAsync(templateId);
            if (!templateResult.Success || templateResult.Data == null)
            {
                return new ApiResponse<ContractDto>
                {
                    Success = false,
                    Message = "模板不存在或已被禁用"
                };
            }

            var template = templateResult.Data;
            
            // 使用模板内容填充合同
            contractDto.Terms = template.Content;
            contractDto.Type = template.Type;
            
            // 创建合同
            return await CreateContractAsync(contractDto, userId);
        }
        catch (Exception ex)
        {
            return new ApiResponse<ContractDto>
            {
                Success = false,
                Message = $"从模板创建合同失败: {ex.Message}"
            };
        }
    }

    public async Task<string> GenerateContractNumberAsync(ContractType type)
    {
        var prefix = type switch
        {
            ContractType.Sale => "HT-S",
            ContractType.Rent => "HT-R", 
            ContractType.Commission => "HT-C",
            _ => "HT"
        };
        
        var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        var random = new Random().Next(100, 999);
        
        return $"{prefix}-{timestamp}-{random}";
    }

    public async Task<ApiResponse<ContractStatsDto>> GetContractStatsAsync()
    {
        try
        {
            var allContracts = await _contractRepository.SearchContractsAsync(null, null, null, null, null);
            
            var stats = new ContractStatsDto
            {
                TotalContracts = allContracts.Count(),
                DraftContracts = allContracts.Count(c => c.Status == ContractStatus.Draft),
                PendingContracts = allContracts.Count(c => c.Status == ContractStatus.Pending),
                SignedContracts = allContracts.Count(c => c.Status == ContractStatus.Signed),
                CompletedContracts = allContracts.Count(c => c.Status == ContractStatus.Completed),
                CancelledContracts = allContracts.Count(c => c.Status == ContractStatus.Cancelled),
                TotalAmount = allContracts.Sum(c => c.Amount),
                MonthlyAmount = allContracts
                    .Where(c => c.CreateTime >= DateTime.Now.AddMonths(-1))
                    .Sum(c => c.Amount)
            };

            return new ApiResponse<ContractStatsDto>
            {
                Success = true,
                Data = stats,
                Message = "获取合同统计成功"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<ContractStatsDto>
            {
                Success = false,
                Message = $"获取合同统计失败: {ex.Message}"
            };
        }
    }
} 