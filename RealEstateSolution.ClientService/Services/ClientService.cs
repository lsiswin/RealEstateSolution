using RealEstateSolution.ClientService.Repository;
using RealEstateSolution.Database.Models;

namespace RealEstateSolution.ClientService.Services;

/// <summary>
/// 客户服务实现类
/// </summary>
public class ClientService : IClientService
{
    private readonly IClientRepository _clientRepository;

    public ClientService(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public async Task<Client> CreateClientAsync(Client client)
    {
        client.CreateTime = DateTime.Now;
        client.UpdateTime = DateTime.Now;
        client.Status = ClientStatus.Active;
        await _clientRepository.AddAsync(client);
        return client;
    }

    public async Task<Client> UpdateClientAsync(Client client)
    {
        var existingClient = await _clientRepository.GetByIdAsync(client.Id);
        if (existingClient == null)
        {
            throw new KeyNotFoundException($"未找到ID为{client.Id}的客户");
        }

        existingClient.Name = client.Name;
        existingClient.Phone = client.Phone;
        existingClient.Email = client.Email;
        existingClient.Address = client.Address;
        existingClient.Status = client.Status;
        existingClient.UpdateTime = DateTime.Now;
         _clientRepository.Update(existingClient);
        return existingClient;
    }

    public async Task<Client?> GetClientByIdAsync(int id)
    {
        return await _clientRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Client>> SearchClientsAsync(
        string? keyword,
        ClientType? type,
        ClientStatus? status,
        DateTime? startDate,
        DateTime? endDate)
    {
        return await _clientRepository.SearchClientsAsync(keyword, status, startDate, endDate);
    }

    public async Task DeleteClientAsync(int id)
    {
        var client = await _clientRepository.GetByIdAsync(id);
        if (client == null)
        {
            throw new KeyNotFoundException($"未找到ID为{id}的客户");
        }

        _clientRepository.Delete(client);
    }

    public async Task<ClientRequirements?> GetClientRequirementsAsync(int clientId)
    {
        return await _clientRepository.GetClientRequirementsAsync(clientId);
    }

    public async Task<ClientRequirements> UpdateClientRequirementsAsync(ClientRequirements requirements)
    {
        await _clientRepository.UpdateClientRequirementsAsync(requirements);
        return requirements;
    }

    public async Task UpdateClientStatusAsync(int clientId, ClientStatus status)
    {
        await _clientRepository.UpdateClientStatusAsync(clientId, status);
    }
} 