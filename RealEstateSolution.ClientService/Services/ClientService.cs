using RealEstateSolution.ClientService.Repository;
using RealEstateSolution.Database.Models;
using RealEstateSolution.Common.Utils;
using RealEstateSolution.ClientService.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using RealEstateSolution.Common.Extensions;

namespace RealEstateSolution.ClientService.Services;

/// <summary>
/// 客户服务实现类
/// 实现客户管理相关的所有业务逻辑，包括客户信息的CRUD操作、需求管理和数据统计
/// </summary>
public class ClientService : IClientService
{
    private readonly IClientRepository _clientRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="clientRepository">客户数据仓储接口</param>
    /// <param name="mapper">对象映射器，用于DTO和实体之间的转换</param>
    public ClientService(IClientRepository clientRepository, IMapper mapper)
    {
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
            // 参数验证
            if (query.PageIndex < 1)
                return ApiResponse<PagedList<ClientDto>>.Error("页码必须大于0");
            
            if (query.PageSize < 1 || query.PageSize > 100)
                return ApiResponse<PagedList<ClientDto>>.Error("每页记录数必须在1-100之间");

            // 构建查询条件 - 使用IQueryable提升性能
            var queryable = _clientRepository.Query();

            // 按关键词搜索
            if (!string.IsNullOrWhiteSpace(query.Keyword))
            {
                var keyword = query.Keyword.Trim();
                queryable = queryable.Where(c => 
                    c.Name.Contains(keyword) || 
                    c.Phone.Contains(keyword) || 
                    (c.Email != null && c.Email.Contains(keyword)));
            }

            // 按姓名模糊查询
            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                queryable = queryable.Where(c => c.Name.Contains(query.Name.Trim()));
            }
            
            // 按电话模糊查询
            if (!string.IsNullOrWhiteSpace(query.Phone))
            {
                queryable = queryable.Where(c => c.Phone.Contains(query.Phone.Trim()));
            }
            
            // 按类型精确查询
            if (query.Type.HasValue)
            {
                queryable = queryable.Where(c => c.Type == query.Type.Value);
            }
            
            // 按状态精确查询
            if (query.Status.HasValue)
            {
                queryable = queryable.Where(c => c.Status == query.Status.Value);
            }
            
            // 排序
            queryable = queryable.OrderByDescending(c => c.CreateTime);

            // 使用AutoMapper扩展方法进行分页查询
            var pagedResult = await queryable.ToPagedListAsync<Client, ClientDto>(_mapper, query.PageIndex, query.PageSize);
            
            return ApiResponse<PagedList<ClientDto>>.Ok(pagedResult, $"成功获取第{query.PageIndex}页客户列表，共{pagedResult.TotalCount}条记录");
        }
        catch (Exception ex)
        {
            return ApiResponse<PagedList<ClientDto>>.Error($"获取客户列表失败：{ex.Message}");
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
            // 参数验证
            if (id <= 0)
                return ApiResponse<ClientDto>.Error("客户ID必须大于0");

            // 使用投影查询优化性能
            var queryable = _clientRepository.Query().Where(c => c.Id == id);
            var clientDto = await queryable.ProjectToFirstOrDefaultAsync<Client, ClientDto>(_mapper);
            
            if (clientDto == null)
            {
                return ApiResponse<ClientDto>.Error($"未找到ID为{id}的客户");
            }

            // 获取客户需求信息
            var requirements = await _clientRepository.GetClientRequirementsAsync(id);
            if (requirements != null)
            {
                clientDto.Requirements = _mapper.SafeMap<ClientRequirements, ClientRequirementDto>(requirements);
            }

            return ApiResponse<ClientDto>.Ok(clientDto, "成功获取客户详细信息");
        }
        catch (Exception ex)
        {
            return ApiResponse<ClientDto>.Error($"获取客户详细信息失败：{ex.Message}");
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
            // 参数验证
            if (clientDto == null)
                return ApiResponse<ClientDto>.Error("客户信息不能为空");

            // 检查电话号码是否已存在
            var phoneExists = await _clientRepository.AnyAsync(c => c.Phone == clientDto.Phone.Trim());
            if (phoneExists)
            {
                return ApiResponse<ClientDto>.Error($"电话号码{clientDto.Phone}已被其他客户使用");
            }

            // 转换为实体并验证
            var client = _mapper.MapWithValidation<ClientDto, Client>(clientDto, entity => 
            {
                if (string.IsNullOrWhiteSpace(entity.Name))
                    return "客户姓名不能为空";
                if (string.IsNullOrWhiteSpace(entity.Phone))
                    return "客户电话不能为空";
                return null;
            });
            
            // 设置系统字段
            client.CreateTime = DateTime.Now;
            client.UpdateTime = DateTime.Now;
            client.Source = ClientSource.Website; // 默认来源
            
            await _clientRepository.AddAsync(client);
            
            var resultDto = _mapper.Map<ClientDto>(client);
            return ApiResponse<ClientDto>.Ok(resultDto, $"客户{client.Name}创建成功");
        }
        catch (ArgumentException ex)
        {
            return ApiResponse<ClientDto>.Error($"数据验证失败：{ex.Message}");
        }
        catch (Exception ex)
        {
            return ApiResponse<ClientDto>.Error($"创建客户失败：{ex.Message}");
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
            // 参数验证
            if (id <= 0)
                return ApiResponse<ClientDto>.Error("客户ID必须大于0");
            
            if (clientDto == null)
                return ApiResponse<ClientDto>.Error("客户信息不能为空");

            var existingClient = await _clientRepository.GetByIdAsync(id);
            if (existingClient == null)
            {
                return ApiResponse<ClientDto>.Error($"未找到ID为{id}的客户");
            }

            // 检查电话号码是否与其他客户冲突
            var phoneConflict = await _clientRepository.AnyAsync(c => c.Id != id && c.Phone == clientDto.Phone.Trim());
            if (phoneConflict)
            {
                return ApiResponse<ClientDto>.Error($"电话号码{clientDto.Phone}已被其他客户使用");
            }

            // 使用AutoMapper的条件映射，只更新非空值
            _mapper.MapNonNullValues(clientDto, existingClient);
            existingClient.UpdateTime = DateTime.Now;

            _clientRepository.Update(existingClient);
            
            var resultDto = _mapper.Map<ClientDto>(existingClient);
            return ApiResponse<ClientDto>.Ok(resultDto, $"客户{existingClient.Name}信息更新成功");
        }
        catch (Exception ex)
        {
            return ApiResponse<ClientDto>.Error($"更新客户信息失败：{ex.Message}");
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
            // 参数验证
            if (id <= 0)
                return ApiResponse.Error("客户ID必须大于0");

            var client = await _clientRepository.GetByIdAsync(id);
            if (client == null)
            {
                return ApiResponse.Error($"未找到ID为{id}的客户");
            }

            // 先删除客户需求信息（如果存在）
            await _clientRepository.DeleteClientRequirementsAsync(id);
            
            // 删除客户
            _clientRepository.Delete(client);
            
            return ApiResponse.Ok($"客户{client.Name}删除成功");
        }
        catch (Exception ex)
        {
            return ApiResponse.Error($"删除客户失败：{ex.Message}");
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
            var allClients = await _clientRepository.GetAllAsync();
            var now = DateTime.Now;
            var thirtyDaysAgo = now.AddDays(-30);

            var stats = new ClientStatsDto
            {
                TotalClients = allClients.Count(),
                ActiveClients = allClients.Count(c => c.Status == ClientStatus.Active),
                PotentialClients = allClients.Count(c => c.Status == ClientStatus.Potential),
                ClosedClients = allClients.Count(c => c.Status == ClientStatus.Closed),
                InvalidClients = allClients.Count(c => c.Status == ClientStatus.Invalid),
                BuyerClients = allClients.Count(c => c.Type == ClientType.Buyer),
                SellerClients = allClients.Count(c => c.Type == ClientType.Seller),
                TenantClients = allClients.Count(c => c.Type == ClientType.Tenant),
                LandlordClients = allClients.Count(c => c.Type == ClientType.Landlord),
                NewClientsLast30Days = allClients.Count(c => c.CreateTime >= thirtyDaysAgo),
                ClosedClientsLast30Days = allClients.Count(c => c.Status == ClientStatus.Closed && c.UpdateTime >= thirtyDaysAgo)
            };

            return ApiResponse<ClientStatsDto>.Ok(stats, "成功获取客户统计数据");
        }
        catch (Exception ex)
        {
            return ApiResponse<ClientStatsDto>.Error($"获取客户统计数据失败：{ex.Message}");
        }
    }
} 