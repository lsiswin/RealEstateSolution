using RealEstateSolution.ContractService.Repository;
using RealEstateSolution.Database.Models;

namespace RealEstateSolution.ContractService.Services;

/// <summary>
/// 合同服务实现类
/// </summary>
public class ContractService : IContractService
{
    private readonly IContractRepository _contractRepository;

    public ContractService(IContractRepository contractRepository)
    {
        _contractRepository = contractRepository;
    }

    public async Task<Contract> CreateContractAsync(Contract contract)
    {
        contract.ContractNumber = await _contractRepository.GenerateContractNumberAsync();
        contract.CreateTime = DateTime.Now;
        contract.UpdateTime = DateTime.Now;
        contract.Status = ContractStatus.Draft;

        await _contractRepository.AddAsync(contract);
        return contract;
    }

    public async Task<Contract> UpdateContractAsync(Contract contract)
    {
        var existingContract = await _contractRepository.GetByIdAsync(contract.Id);
        if (existingContract == null)
        {
            throw new KeyNotFoundException($"合同ID {contract.Id} 不存在");
        }

        contract.UpdateTime = DateTime.Now;
         _contractRepository.Update(contract);
        return contract;
    }

    public async Task<Contract?> GetContractByIdAsync(int id)
    {
        return await _contractRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Contract>> SearchContractsAsync(
        string? keyword,
        ContractType? type,
        ContractStatus? status,
        DateTime? startDate,
        DateTime? endDate)
    {
        return await _contractRepository.SearchContractsAsync(keyword, type, status, startDate, endDate);
    }

    public async Task UpdateContractStatusAsync(int contractId, ContractStatus status)
    {
        await _contractRepository.UpdateStatusAsync(contractId, status);
    }

    public async Task<IEnumerable<Contract>> GetClientContractsAsync(int clientId)
    {
        return await _contractRepository.GetClientContractsAsync(clientId);
    }

    public async Task<IEnumerable<Contract>> GetPropertyContractsAsync(int propertyId)
    {
        return await _contractRepository.GetPropertyContractsAsync(propertyId);
    }

    public async Task DeleteContractAsync(int id)
    {
        var contract = await _contractRepository.GetByIdAsync(id);
        if (contract != null)
        {
             _contractRepository.Delete(contract);
        }
    }
} 