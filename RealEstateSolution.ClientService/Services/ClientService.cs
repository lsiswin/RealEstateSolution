using RealEstateSolution.ClientService.Repository;
using RealEstateSolution.Database.Models;
using RealEstateSolution.Common.Utils;
using RealEstateSolution.ClientService.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using RealEstateSolution.Common.Extensions;
using RealEstateSolution.ClientService.Models;
using RealEstateSolution.Common.Repository;
using RealEstateSolution.ClientService.Data;

namespace RealEstateSolution.ClientService.Services;

/// <summary>
/// 客户服务实现类
/// 实现客户管理相关的所有业务逻辑，包括客户信息的CRUD操作、需求管理和数据统计
/// </summary>
public class ClientService : IClientService
{
    private readonly IUnitOfWork<ClientDbContext> _unitOfWork;
    private readonly IClientRepository _clientRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="unitOfWork">工作单元</param>
    /// <param name="clientRepository">客户数据仓储接口</param>
    /// <param name="mapper">对象映射器，用于DTO和实体之间的转换</param>
    public ClientService(
        IUnitOfWork<ClientDbContext> unitOfWork,
        IClientRepository clientRepository, 
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _clientRepository = clientRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// 获取客户列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页的客户列表</returns>
    public async Task<ApiResponse<PagedList<ClientDto>>> GetClientsAsync(ClientQueryDto query)
    {
        try
        {
            var clients = await _clientRepository.SearchClientsAsync(
                query.Name,
                query.Phone,
                query.Email,
                query.Type,
                query.Status);

            // 应用分页
            var totalCount = clients.Count();
            var pagedClients = clients
                .Skip((query.PageIndex - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            var clientDtos = _mapper.Map<List<ClientDto>>(pagedClients);

            var pagedResult = new PagedList<ClientDto>(
                clientDtos,
                totalCount,
                query.PageIndex,
                query.PageSize);

            return new ApiResponse<PagedList<ClientDto>>
            {
                Success = true,
                Data = pagedResult,
                Message = "获取客户列表成功"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<PagedList<ClientDto>>
            {
                Success = false,
                Message = $"获取客户列表失败: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// 根据ID获取客户详细信息
    /// </summary>
    /// <param name="id">客户ID</param>
    /// <returns>客户详细信息</returns>
    public async Task<ApiResponse<ClientDto>> GetClientByIdAsync(int id)
    {
        try
        {
            var client = await _clientRepository.GetByIdAsync(id);
            if (client == null)
            {
                return new ApiResponse<ClientDto>
                {
                    Success = false,
                    Message = "客户不存在"
                };
            }

            var clientDto = _mapper.Map<ClientDto>(client);
            return new ApiResponse<ClientDto>
            {
                Success = true,
                Data = clientDto,
                Message = "获取客户详情成功"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<ClientDto>
            {
                Success = false,
                Message = $"获取客户详情失败: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// 创建新客户
    /// </summary>
    /// <param name="clientDto">客户信息DTO</param>
    /// <param name="userId">创建人ID</param>
    /// <returns>创建成功的客户信息</returns>
    public async Task<ApiResponse<ClientDto>> CreateClientAsync(ClientDto clientDto, string userId)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            // 检查手机号是否已存在
            if (await _clientRepository.IsPhoneExistsAsync(clientDto.Phone))
            {
                return new ApiResponse<ClientDto>
                {
                    Success = false,
                    Message = "手机号已存在"
                };
            }

            // 检查邮箱是否已存在（如果提供了邮箱）
            if (!string.IsNullOrEmpty(clientDto.Email) && await _clientRepository.IsEmailExistsAsync(clientDto.Email))
            {
                return new ApiResponse<ClientDto>
                {
                    Success = false,
                    Message = "邮箱已存在"
                };
            }

            var client = _mapper.Map<Client>(clientDto);
            client.CreateTime = DateTime.Now;
            client.UpdateTime = DateTime.Now;

            await _clientRepository.AddAsync(client);
            await _unitOfWork.CommitAsync();

            var resultDto = _mapper.Map<ClientDto>(client);
            return new ApiResponse<ClientDto>
            {
                Success = true,
                Data = resultDto,
                Message = "创建客户成功"
            };
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return new ApiResponse<ClientDto>
            {
                Success = false,
                Message = $"创建客户失败: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// 更新客户信息
    /// </summary>
    /// <param name="id">要更新的客户ID</param>
    /// <param name="clientDto">新的客户信息DTO</param>
    /// <param name="userId">更新人ID</param>
    /// <returns>更新后的客户信息</returns>
    public async Task<ApiResponse<ClientDto>> UpdateClientAsync(int id, ClientDto clientDto, string userId)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var existingClient = await _clientRepository.GetByIdAsync(id);
            if (existingClient == null)
            {
                return new ApiResponse<ClientDto>
                {
                    Success = false,
                    Message = "客户不存在"
                };
            }

            // 检查手机号是否已存在（排除当前客户）
            if (await _clientRepository.IsPhoneExistsAsync(clientDto.Phone, id))
            {
                return new ApiResponse<ClientDto>
                {
                    Success = false,
                    Message = "手机号已存在"
                };
            }

            // 检查邮箱是否已存在（排除当前客户）
            if (!string.IsNullOrEmpty(clientDto.Email) && await _clientRepository.IsEmailExistsAsync(clientDto.Email, id))
            {
                return new ApiResponse<ClientDto>
                {
                    Success = false,
                    Message = "邮箱已存在"
                };
            }

            // 更新客户信息
            existingClient.Name = clientDto.Name;
            existingClient.Phone = clientDto.Phone;
            existingClient.Email = clientDto.Email;
            existingClient.Type = clientDto.Type;
            existingClient.Status = clientDto.Status;
            existingClient.Address = clientDto.Address;
            existingClient.UpdateTime = DateTime.Now;

            _clientRepository.Update(existingClient);
            await _unitOfWork.CommitAsync();

            var resultDto = _mapper.Map<ClientDto>(existingClient);
            return new ApiResponse<ClientDto>
            {
                Success = true,
                Data = resultDto,
                Message = "更新客户成功"
            };
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return new ApiResponse<ClientDto>
            {
                Success = false,
                Message = $"更新客户失败: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// 删除客户
    /// </summary>
    /// <param name="id">要删除的客户ID</param>
    /// <param name="userId">删除人ID</param>
    /// <returns>删除操作结果</returns>
    public async Task<ApiResponse> DeleteClientAsync(int id, string userId)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var client = await _clientRepository.GetByIdAsync(id);
            if (client == null)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "客户不存在"
                };
            }

            await _clientRepository.DeleteClientAsync(id);
            await _unitOfWork.CommitAsync();

            return new ApiResponse
            {
                Success = true,
                Message = "删除客户成功"
            };
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return new ApiResponse
            {
                Success = false,
                Message = $"删除客户失败: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// 获取客户需求信息
    /// </summary>
    /// <param name="clientId">客户ID</param>
    /// <returns>客户需求详细信息</returns>
    public async Task<ApiResponse<ClientRequirementDto>> GetClientRequirementsAsync(int clientId)
    {
        try
        {
            // 参数验证
            if (clientId <= 0)
                return ApiResponse<ClientRequirementDto>.Error("客户ID必须大于0");

            // 检查客户是否存在
            var client = await _clientRepository.GetByIdAsync(clientId);
            if (client == null)
            {
                return ApiResponse<ClientRequirementDto>.Error($"未找到ID为{clientId}的客户");
            }

            var requirements = await _clientRepository.GetClientRequirementsAsync(clientId);
            if (requirements == null)
            {
                return ApiResponse<ClientRequirementDto>.Error("该客户尚未设置需求信息");
            }

            var requirementDto = _mapper.Map<ClientRequirementDto>(requirements);
            return ApiResponse<ClientRequirementDto>.Ok(requirementDto, "成功获取客户需求信息");
        }
        catch (Exception ex)
        {
            return ApiResponse<ClientRequirementDto>.Error($"获取客户需求信息失败：{ex.Message}");
        }
    }

    /// <summary>
    /// 更新或创建客户需求信息
    /// </summary>
    /// <param name="clientId">客户ID</param>
    /// <param name="requirementDto">客户需求信息DTO</param>
    /// <param name="userId">操作人ID</param>
    /// <returns>更新后的客户需求信息</returns>
    public async Task<ApiResponse<ClientRequirementDto>> UpdateClientRequirementsAsync(int clientId, ClientRequirementDto requirementDto, string userId)
    {
        try
        {
            // 参数验证
            if (clientId <= 0)
                return ApiResponse<ClientRequirementDto>.Error("客户ID必须大于0");
            
            if (requirementDto == null)
                return ApiResponse<ClientRequirementDto>.Error("需求信息不能为空");

            // 检查客户是否存在
            var client = await _clientRepository.GetByIdAsync(clientId);
            if (client == null)
            {
                return ApiResponse<ClientRequirementDto>.Error($"未找到ID为{clientId}的客户");
            }

            // 业务逻辑验证
            if (requirementDto.MinPrice.HasValue && requirementDto.MaxPrice.HasValue && 
                requirementDto.MinPrice > requirementDto.MaxPrice)
            {
                return ApiResponse<ClientRequirementDto>.Error("最低价格不能大于最高价格");
            }

            if (requirementDto.MinArea.HasValue && requirementDto.MaxArea.HasValue && 
                requirementDto.MinArea > requirementDto.MaxArea)
            {
                return ApiResponse<ClientRequirementDto>.Error("最小面积不能大于最大面积");
            }

            // 转换为实体
            var requirements = _mapper.Map<ClientRequirements>(requirementDto);
            requirements.ClientId = clientId;

            // 检查是否已存在需求信息
            var existingRequirements = await _clientRepository.GetClientRequirementsAsync(clientId);
            if (existingRequirements != null)
            {
                // 更新现有需求
                requirements.Id = existingRequirements.Id;
                requirements.CreateTime = existingRequirements.CreateTime;
                requirements.UpdateTime = DateTime.Now;
                await _clientRepository.UpdateClientRequirementsAsync(requirements);
            }
            else
            {
                // 创建新需求
                requirements.CreateTime = DateTime.Now;
                requirements.UpdateTime = DateTime.Now;
                await _clientRepository.CreateClientRequirementsAsync(requirements);
            }

            var resultDto = _mapper.Map<ClientRequirementDto>(requirements);
            return ApiResponse<ClientRequirementDto>.Ok(resultDto, "客户需求信息更新成功");
        }
        catch (Exception ex)
        {
            return ApiResponse<ClientRequirementDto>.Error($"更新客户需求信息失败：{ex.Message}");
        }
    }

    /// <summary>
    /// 获取客户统计数据
    /// </summary>
    /// <returns>客户相关的统计信息</returns>
    public async Task<ApiResponse<ClientStatsDto>> GetClientStatsAsync()
    {
        try
        {
            var (total, active, inactive, potential) = await _clientRepository.GetClientStatsAsync();

            var stats = new ClientStatsDto
            {
                TotalClients = total,
                ActiveClients = active,
                InvalidClients = inactive,
                PotentialClients = potential
            };

            return new ApiResponse<ClientStatsDto>
            {
                Success = true,
                Data = stats,
                Message = "获取统计信息成功"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<ClientStatsDto>
            {
                Success = false,
                Message = $"获取统计信息失败: {ex.Message}"
            };
        }
    }

    public async Task<ApiResponse> UpdateClientStatusAsync(int id, ClientStatus status, string userId)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var client = await _clientRepository.GetByIdAsync(id);
            if (client == null)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "客户不存在"
                };
            }

            await _clientRepository.UpdateStatusAsync(id, status);
            await _unitOfWork.CommitAsync();

            return new ApiResponse
            {
                Success = true,
                Message = $"客户状态已更新为{status}"
            };
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return new ApiResponse
            {
                Success = false,
                Message = $"更新客户状态失败: {ex.Message}"
            };
        }
    }
} 