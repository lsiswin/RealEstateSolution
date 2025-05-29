using Microsoft.EntityFrameworkCore;
using RealEstateSolution.Common.Repository;
using RealEstateSolution.Database.Models;
using RealEstateSolution.MatchingService.Data;
using RealEstateSolution.MatchingService.Repository;

namespace RealEstateSolution.MatchingService.Services;

/// <summary>
/// 匹配服务实现类
/// </summary>
public class MatchingService : IMatchingService
{
    private readonly IMatchingRepository _matchingRepository;
    private readonly MatchingDbContext _context;

    public MatchingService(IMatchingRepository matchingRepository, MatchingDbContext context)
    {
        _matchingRepository = matchingRepository;
        _context = context;
    }

    public async Task<Matching> CreateMatchingAsync(Matching matching)
    {
        matching.CreateTime = DateTime.Now;
        matching.UpdateTime = DateTime.Now;
        matching.Status = MatchingStatus.Pending;
        await _matchingRepository.AddAsync(matching);
        return matching;
    }

    public async Task UpdateMatchingStatusAsync(int matchingId, MatchingStatus status)
    {
        var matching = await _matchingRepository.GetByIdAsync(matchingId);
        if (matching == null)
        {
            throw new KeyNotFoundException($"匹配ID {matchingId} 不存在");
        }

        matching.Status = status;
        matching.UpdateTime = DateTime.Now;
        _matchingRepository.Update(matching);
       
    }

    public async Task<Matching?> GetMatchingByIdAsync(int id)
    {
        return await _matchingRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Matching>> SearchMatchingsAsync(
        string? keyword,
        MatchingType? type,
        MatchingStatus? status,
        DateTime? startDate,
        DateTime? endDate)
    {
        return await _matchingRepository.SearchMatchingsAsync(
            keyword,
            type,
            status,
            startDate,
            endDate);
    }

    public async Task<IEnumerable<Matching>> GetClientMatchingsAsync(int clientId)
    {
        return await _matchingRepository.GetClientMatchingsAsync(clientId);
    }

    public async Task<IEnumerable<Matching>> GetPropertyMatchingsAsync(int propertyId)
    {
        return await _matchingRepository.GetPropertyMatchingsAsync(propertyId);
    }

    public async Task DeleteMatchingAsync(int id)
    {
        var matching = await _matchingRepository.GetByIdAsync(id);
        if (matching == null)
        {
            throw new KeyNotFoundException($"匹配ID {id} 不存在");
        }

        _matchingRepository.Delete(matching);
    }

    public async Task<IEnumerable<Matching>> AutoMatchAsync(int clientId)
    {
        //var client = await _context.Clients
        //    .Include(c => c.Requirements)
        //    .FirstOrDefaultAsync(c => c.Id == clientId);

        //if (client == null)
        //{
        //    throw new KeyNotFoundException($"客户ID {clientId} 不存在");
        //}

        //var properties = await _context.Properties
        //    .Include(p => p.Images)
        //    .Where(p => p.Status == PropertyStatus.ForSale || p.Status == PropertyStatus.ForRent)
        //    .ToListAsync();

        //var matchings = new List<Matching>();
        //foreach (var property in properties)
        //{
        //    var score = await CalculateMatchingScoreAsync(client, property);
        //    if (score >= 0.7) // 匹配度阈值
        //    {
        //        var matching = new Matching
        //        {
        //            ClientId = clientId,
        //            PropertyId = property.Id,
        //            Type = MatchingType.Auto,
        //            Status = MatchingStatus.Pending,
        //            Score = score,
        //            Reason = GenerateMatchingReason(client, property, score),
        //            CreateTime = DateTime.Now,
        //            UpdateTime = DateTime.Now
        //        };

        //        matchings.Add(await CreateMatchingAsync(matching));
        //    }
        //}

        //return matchings;
        return null;
    }

    public async Task<Matching> ManualMatchAsync(int clientId, int propertyId)
    {
        //var client = await _context.Clients.FindAsync(clientId);
        //if (client == null)
        //{
        //    throw new KeyNotFoundException($"客户ID {clientId} 不存在");
        //}

        //var property = await _context.Properties.FindAsync(propertyId);
        //if (property == null)
        //{
        //    throw new KeyNotFoundException($"房源ID {propertyId} 不存在");
        //}

        //var score = await CalculateMatchingScoreAsync(client, property);
        //var matching = new Matching
        //{
        //    ClientId = clientId,
        //    PropertyId = propertyId,
        //    Type = MatchingType.Manual,
        //    Status = MatchingStatus.Pending,
        //    Score = score,
        //    Reason = GenerateMatchingReason(client, property, score),
        //    CreateTime = DateTime.Now,
        //    UpdateTime = DateTime.Now
        //};

        //return await CreateMatchingAsync(matching);
        return null;
    }

    public async Task<double> CalculateMatchingScoreAsync(Client client, Property property)
    {
        var score = 0.0;
        var totalWeight = 0.0;

        // 价格匹配度
        if (client.Requirements.MinPrice.HasValue && client.Requirements.MaxPrice.HasValue)
        {
            var priceWeight = 0.3;
            totalWeight += priceWeight;
            if (property.Price >= client.Requirements.MinPrice.Value &&
                property.Price <= client.Requirements.MaxPrice.Value)
            {
                score += priceWeight;
            }
        }

        // 面积匹配度
        if (client.Requirements.MinArea.HasValue && client.Requirements.MaxArea.HasValue)
        {
            var areaWeight = 0.2;
            totalWeight += areaWeight;
            if (property.Area >= client.Requirements.MinArea.Value &&
                property.Area <= client.Requirements.MaxArea.Value)
            {
                score += areaWeight;
            }
        }

        // 位置匹配度
        if (!string.IsNullOrEmpty(client.Requirements.Location))
        {
            var locationWeight = 0.3;
            totalWeight += locationWeight;
            if (property.Address.Contains(client.Requirements.Location))
            {
                score += locationWeight;
            }
        }

        // 类型匹配度
        if (client.Requirements.PropertyType.HasValue)
        {
            var typeWeight = 0.2;
            totalWeight += typeWeight;
            if (property.Type == client.Requirements.PropertyType.Value)
            {
                score += typeWeight;
            }
        }

        return totalWeight > 0 ? score / totalWeight : 0;
    }

    private string GenerateMatchingReason(Client client, Property property, double score)
    {
        var reasons = new List<string>();

        if (score >= 0.9)
        {
            reasons.Add("完美匹配");
        }
        else if (score >= 0.7)
        {
            reasons.Add("高度匹配");
        }
        else
        {
            reasons.Add("基本匹配");
        }

        if (client.Requirements.MinPrice.HasValue && client.Requirements.MaxPrice.HasValue &&
            property.Price >= client.Requirements.MinPrice.Value &&
            property.Price <= client.Requirements.MaxPrice.Value)
        {
            reasons.Add("价格符合要求");
        }

        if (client.Requirements.MinArea.HasValue && client.Requirements.MaxArea.HasValue &&
            property.Area >= client.Requirements.MinArea.Value &&
            property.Area <= client.Requirements.MaxArea.Value)
        {
            reasons.Add("面积符合要求");
        }

        if (!string.IsNullOrEmpty(client.Requirements.Location) &&
            property.Address.Contains(client.Requirements.Location))
        {
            reasons.Add("位置符合要求");
        }

        if (client.Requirements.PropertyType.HasValue &&
            property.Type == client.Requirements.PropertyType.Value)
        {
            reasons.Add("类型符合要求");
        }

        return string.Join("，", reasons);
    }
} 