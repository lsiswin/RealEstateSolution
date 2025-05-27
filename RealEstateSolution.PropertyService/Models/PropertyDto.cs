using RealEstateSolution.Database.Models;

namespace RealEstateSolution.PropertyService.Models
{
    /// <summary>
    /// 房源数据传输对象
    /// </summary>
    public class PropertyDto
    {
        /// <summary>
        /// 房产ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 房产类型
        /// </summary>
        public PropertyType Type { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 面积
        /// </summary>
        public decimal Area { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 装修类型
        /// </summary>
        public DecorationType Decoration { get; set; }

        /// <summary>
        /// 朝向
        /// </summary>
        public OrientationType Orientation { get; set; }

        /// <summary>
        /// 楼层
        /// </summary>
        public int Floor { get; set; }

        /// <summary>
        /// 总楼层
        /// </summary>
        public int TotalFloors { get; set; }

        /// <summary>
        /// 房间数
        /// </summary>
        public int Rooms { get; set; }

        /// <summary>
        /// 卫生间数
        /// </summary>
        public int Bathrooms { get; set; }

        /// <summary>
        /// 房产状态
        /// </summary>
        public PropertyStatus Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 房源图片URL列表
        /// </summary>
        public List<string> ImageUrls { get; set; }
    }
} 