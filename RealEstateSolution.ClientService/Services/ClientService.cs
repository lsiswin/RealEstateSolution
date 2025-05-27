using RealEstateSolution.ClientService.Repository;
using RealEstateSolution.Database.Models;
using RealEstateSolution.Common.Utils;
using RealEstateSolution.ClientService.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;

namespace RealEstateSolution.ClientService.Services;

/// <summary>
/// 客户服务实现类
/// </summary>
public class ClientService : IClientService
{
    private readonly IClientRepository _clientRepository;
    private readonly IMapper _mapper;

    public ClientService(IClientRepository clientRepository, IMapper mapper)
    {
        _clientRepository = clientRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// 获取客户列表
    /// </summary>
    public async Task<ApiResponse<PagedList<Client>>> GetClientsAsync(
        int userId,
        string name = null,
        string phone = null,
        ClientType? type = null,
        int pageIndex = 1,
        int pageSize = 10)
    {
        try
        {
            // 构建查询条件
            Expression<Func<Client, bool>> predicate = c => true;
            
            if (!string.IsNullOrEmpty(name))
            {
                predicate = c => c.Name.Contains(name);
            }
            
            if (!string.IsNullOrEmpty(phone))
            {
                var phonePredicate = (Expression<Func<Client, bool>>)(c => c.Phone.Contains(phone));
                predicate = PredicateBuilder.And(predicate, phonePredicate);
            }
            
            // 查询数据
            var clients = await _clientRepository.FindAsync(predicate);
            
            // 应用分页
            var pagedClients = clients
                .OrderByDescending(c => c.CreateTime)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();
                
            // 获取总数
            var totalCount = await _clientRepository.CountAsync(predicate);
            
            // 构建分页结果
            var pagedList = new PagedList<Client>(pagedClients, totalCount, pageIndex, pageSize);
            return ApiResponse<PagedList<Client>>.Ok(pagedList);
        }
        catch (Exception ex)
        {
            return ApiResponse<PagedList<Client>>.Error(ex.Message);
        }
    }

    /// <summary>
    /// 获取客户详情
    /// </summary>
    public async Task<ApiResponse<Client>> GetClientByIdAsync(int id, int userId)
    {
        try
        {
            var client = await _clientRepository.GetByIdAsync(id);
            if (client == null)
            {
                return ApiResponse<Client>.Error($"未找到ID为{id}的客户");
            }

            return ApiResponse<Client>.Ok(client);
        }
        catch (Exception ex)
        {
            return ApiResponse<Client>.Error(ex.Message);
        }
    }

    /// <summary>
    /// 创建客户
    /// </summary>
    public async Task<ApiResponse<Client>> CreateClientAsync(Client client, int userId)
    {
        try
        {
            client.CreateTime = DateTime.Now;
            client.UpdateTime = DateTime.Now;
            client.Status = ClientStatus.Active;
            
            await _clientRepository.AddAsync(client);
            
            return ApiResponse<Client>.Ok(client, "客户创建成功");
        }
        catch (Exception ex)
        {
            return ApiResponse<Client>.Error(ex.Message);
        }
    }

    /// <summary>
    /// 更新客户信息
    /// </summary>
    public async Task<ApiResponse<Client>> UpdateClientAsync(int id, Client client, int userId)
    {
        try
        {
            var existingClient = await _clientRepository.GetByIdAsync(id);
            if (existingClient == null)
            {
                return ApiResponse<Client>.Error($"未找到ID为{id}的客户");
            }

            existingClient.Name = client.Name;
            existingClient.Phone = client.Phone;
            existingClient.Email = client.Email;
            existingClient.Address = client.Address;
            existingClient.Status = client.Status;
            existingClient.UpdateTime = DateTime.Now;
            
            _clientRepository.Update(existingClient);
            
            return ApiResponse<Client>.Ok(existingClient, "客户信息更新成功");
        }
        catch (Exception ex)
        {
            return ApiResponse<Client>.Error(ex.Message);
        }
    }

    /// <summary>
    /// 删除客户
    /// </summary>
    public async Task<ApiResponse> DeleteClientAsync(int id, int userId)
    {
        try
        {
            var client = await _clientRepository.GetByIdAsync(id);
            if (client == null)
            {
                return ApiResponse.Error($"未找到ID为{id}的客户");
            }

            _clientRepository.Delete(client);
            return ApiResponse.Ok("客户删除成功");
        }
        catch (Exception ex)
        {
            return ApiResponse.Error(ex.Message);
        }
    }

    /// <summary>
    /// 获取客户需求
    /// </summary>
    public async Task<ApiResponse<ClientRequirement>> GetClientRequirementsAsync(int clientId, int userId)
    {
        try
        {
            var requirements = await _clientRepository.GetClientRequirementsAsync(clientId);
            if (requirements == null)
            {
                return ApiResponse<ClientRequirement>.Error($"未找到客户{clientId}的需求信息");
            }

            // 使用AutoMapper将数据库模型转换为DTO
            var clientRequirement = _mapper.Map<ClientRequirement>(requirements);
            
            return ApiResponse<ClientRequirement>.Ok(clientRequirement);
        }
        catch (Exception ex)
        {
            return ApiResponse<ClientRequirement>.Error(ex.Message);
        }
    }

    /// <summary>
    /// 更新客户需求
    /// </summary>
    public async Task<ApiResponse<ClientRequirement>> UpdateClientRequirementsAsync(int clientId, ClientRequirement requirements, int userId)
    {
        try
        {
            // 使用AutoMapper将DTO转换为数据库模型
            var dbRequirements = _mapper.Map<ClientRequirements>(requirements);
            dbRequirements.ClientId = clientId;
            dbRequirements.UpdateTime = DateTime.Now;

            await _clientRepository.UpdateClientRequirementsAsync(dbRequirements);
            
            // 使用AutoMapper将更新后的数据库模型转换回DTO
            var updatedRequirements = _mapper.Map<ClientRequirement>(dbRequirements);
            
            return ApiResponse<ClientRequirement>.Ok(updatedRequirements, "客户需求更新成功");
        }
        catch (Exception ex)
        {
            return ApiResponse<ClientRequirement>.Error(ex.Message);
        }
    }

    /// <summary>
    /// 获取客户统计数据
    /// </summary>
    public async Task<ApiResponse<ClientStats>> GetClientStatsAsync(int userId)
    {
        try
        {
            // 客户总数
            var totalCount = await _clientRepository.CountAsync(c => true);
            
            // 按状态统计
            var potentialCount = await _clientRepository.CountAsync(c => c.Status == ClientStatus.Potential);
            var activeCount = await _clientRepository.CountAsync(c => c.Status == ClientStatus.Active);
            var closedCount = await _clientRepository.CountAsync(c => c.Status == ClientStatus.Closed);
            
            // 最近30天的数据
            var thirtyDaysAgo = DateTime.Now.AddDays(-30);
            var newCount30Days = await _clientRepository.CountAsync(c => c.CreateTime >= thirtyDaysAgo);
            var closedCount30Days = await _clientRepository.CountAsync(c => c.Status == ClientStatus.Closed && c.UpdateTime >= thirtyDaysAgo);
            
            // 由于Client类没有Type属性，我们不能直接按类型统计
            // 这里我们创建一个空的字典，实际项目中需要根据数据库表结构调整
            var countByType = new Dictionary<ClientType, int>
            {
                { ClientType.Buyer, 0 },
                { ClientType.Seller, 0 },
                { ClientType.Tenant, 0 },
                { ClientType.Landlord, 0 }
            };
            
            var stats = new ClientStats
            {
                TotalCount = totalCount,
                PotentialCount = potentialCount,
                ActiveCount = activeCount,
                ClosedCount = closedCount,
                CountByType = countByType,
                NewCount30Days = newCount30Days,
                ClosedCount30Days = closedCount30Days
            };
            
            return ApiResponse<ClientStats>.Ok(stats);
        }
        catch (Exception ex)
        {
            return ApiResponse<ClientStats>.Error(ex.Message);
        }
    }
} 