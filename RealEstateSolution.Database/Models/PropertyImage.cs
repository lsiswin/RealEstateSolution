using System.ComponentModel.DataAnnotations;

namespace RealEstateSolution.Database.Models;

/// <summary>
/// 房源图片
/// </summary>
public class PropertyImage
{
    /// <summary>
    /// 主键
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// 房源ID
    /// </summary>
    [Required]
    public int PropertyId { get; set; }

    /// <summary>
    /// 图片URL
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string ImageUrl { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [Required]
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 关联的房源
    /// </summary>
    public virtual Property Property { get; set; }
} 