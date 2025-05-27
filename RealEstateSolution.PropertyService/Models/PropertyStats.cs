using RealEstateSolution.Database.Models;
using System.Collections.Generic;

namespace RealEstateSolution.PropertyService.Models
{
    /// <summary>
    /// 房源统计数据
    /// </summary>
    public class PropertyStats
    {
        /// <summary>
        /// 总房源数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 待售房源数
        /// </summary>
        public int ForSale { get; set; }

        /// <summary>
        /// 已售房源数
        /// </summary>
        public int Sold { get; set; }

        /// <summary>
        /// 待租房源数
        /// </summary>
        public int ForRent { get; set; }

        /// <summary>
        /// 已租房源数
        /// </summary>
        public int Rented { get; set; }

        /// <summary>
        /// 下架房源数
        /// </summary>
        public int OfflineCount { get; set; }

        /// <summary>
        /// 按类型统计的房源数
        /// </summary>
        public Dictionary<string, int> TypeDistribution { get; set; } = new Dictionary<string, int>();

        /// <summary>
        /// 最近30天新增房源数
        /// </summary>
        public int NewCount30Days { get; set; }
    }
} 