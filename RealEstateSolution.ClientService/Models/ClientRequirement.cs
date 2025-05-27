using RealEstateSolution.Database.Models;
using System;

namespace RealEstateSolution.ClientService.Models
{
    /// <summary>
    /// 客户需求
    /// </summary>
    public class ClientRequirement
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 客户ID
        /// </summary>
        public int ClientId { get; set; }

        /// <summary>
        /// 最低价格
        /// </summary>
        public decimal? MinPrice { get; set; }

        /// <summary>
        /// 最高价格
        /// </summary>
        public decimal? MaxPrice { get; set; }

        /// <summary>
        /// 最小面积
        /// </summary>
        public decimal? MinArea { get; set; }

        /// <summary>
        /// 最大面积
        /// </summary>
        public decimal? MaxArea { get; set; }

        /// <summary>
        /// 期望位置
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// 房产类型
        /// </summary>
        public PropertyType? PropertyType { get; set; }

        /// <summary>
        /// 其他需求
        /// </summary>
        public string OtherRequirements { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }
} 