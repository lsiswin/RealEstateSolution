using RealEstateSolution.Database.Models;

namespace RealEstateSolution.PropertyService.Models
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
        public string ImageUrl { get; set; }

        /// <summary>
        /// 图片描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 是否为主图
        /// </summary>
        public bool IsMain { get; set; }

        /// <summary>
        /// 排序序号
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// 上传时间
        /// </summary>
        public DateTime UploadTime { get; set; }
    }
} 