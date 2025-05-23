using Microsoft.EntityFrameworkCore;
using RealEstateSolution.Common.Repository;
using RealEstateSolution.Database.Models;
using RealEstateSolution.PropertyService.Data;

namespace RealEstateSolution.PropertyService.Repository;

/// <summary>
/// 房源仓储实现类
/// </summary>
public class PropertyRepository : GenericRepository<Property>, IPropertyRepository
{
    public PropertyRepository(PropertyDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Property>> SearchPropertiesAsync(
        string? keyword,
        PropertyType? type,
        PropertyStatus? status,
        decimal? minPrice,
        decimal? maxPrice,
        string? location)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(p => p.Title.Contains(keyword) || 
                                   p.Description.Contains(keyword) || 
                                   p.Address.Contains(keyword));
        }

        if (type.HasValue)
        {
            query = query.Where(p => p.Type == type.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(p => p.Status == status.Value);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(p => p.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= maxPrice.Value);
        }

        if (!string.IsNullOrWhiteSpace(location))
        {
            query = query.Where(p => p.Address.Contains(location));
        }

        return await query.OrderByDescending(p => p.CreateTime).ToListAsync();
    }

    public async Task<IEnumerable<Property>> GetOwnerPropertiesAsync(int ownerId)
    {
        return await _dbSet.Where(p => p.OwnerId == ownerId)
            .OrderByDescending(p => p.CreateTime)
            .ToListAsync();
    }

    public async Task UpdateStatusAsync(int id, PropertyStatus status)
    {
        var property = await _dbSet.FindAsync(id);
        if (property != null)
        {
            property.Status = status;
            property.UpdateTime = DateTime.Now;
            await _context.SaveChangesAsync();
        }
    }

    public async Task AddPropertyImageAsync(int propertyId, string imageUrl)
    {
        var property = await _dbSet.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == propertyId);
        if (property != null)
        {
            var image = new PropertyImage
            {
                PropertyId = propertyId,
                ImageUrl = imageUrl,
                CreateTime = DateTime.Now
            };

            if (property.Images == null)
            {
                property.Images = new List<PropertyImage>();
            }

            property.Images.Add(image);
            property.UpdateTime = DateTime.Now;
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeletePropertyImageAsync(int propertyId, string imageUrl)
    {
        var property = await _dbSet.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == propertyId);
        if (property?.Images != null)
        {
            var image = property.Images.FirstOrDefault(i => i.ImageUrl == imageUrl);
            if (image != null)
            {
                property.Images.Remove(image);
                property.UpdateTime = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }
    }
} 