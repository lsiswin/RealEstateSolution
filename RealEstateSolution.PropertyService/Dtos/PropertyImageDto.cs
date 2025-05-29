using RealEstateSolution.Database.Models;
using System.ComponentModel.DataAnnotations;

namespace RealEstateSolution.PropertyService.Dtos
{
    /// <summary>
    /// 房源图片数据传输对象
    /// </summary>
    public class PropertyImageDto
    {
        /// <summary>
        /// 图片ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 房源ID
        /// </summary>
        public int PropertyId { get; set; }

        /// <summary>
        /// 图片URL
        /// </summary>
        [Required(ErrorMessage = "图片URL不能为空")]
        [StringLength(500, ErrorMessage = "图片URL长度不能超过500个字符")]
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// 图片描述
        /// </summary>
        [StringLength(200, ErrorMessage = "图片描述长度不能超过200个字符")]
        public string? Description { get; set; }

        /// <summary>
        /// 是否为主图
        /// </summary>
        public bool IsMain { get; set; }

        /// <summary>
        /// 排序序号
        /// </summary>
        [Range(0, 999, ErrorMessage = "排序序号必须在0-999之间")]
        public int SortOrder { get; set; }

        /// <summary>
        /// 上传时间
        /// </summary>
        public DateTime UploadTime { get; set; }

        /// <summary>
        /// 文件大小（格式化显示）
        /// </summary>
        public string FileSize { get; set; } = string.Empty;

        /// <summary>
        /// 缩略图URL
        /// </summary>
        public string ThumbnailUrl { get; set; } = string.Empty;
    }
} 