using RealEstateSolution.Database.Models;
using System.Collections.Generic;

namespace RealEstateSolution.ClientService.Models
{
    /// <summary>
    /// 客户统计数据
    /// </summary>
    public class ClientStats
    {
        /// <summary>
        /// 客户总数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 潜在客户数
        /// </summary>
        public int PotentialCount { get; set; }

        /// <summary>
        /// 活跃客户数
        /// </summary>
        public int ActiveCount { get; set; }

        /// <summary>
        /// 已成交客户数
        /// </summary>
        public int ClosedCount { get; set; }

        /// <summary>
        /// 按类型统计的客户数
        /// </summary>
        public Dictionary<ClientType, int> CountByType { get; set; } = new Dictionary<ClientType, int>();

        /// <summary>
        /// 最近30天新增客户数
        /// </summary>
        public int NewCount30Days { get; set; }

        /// <summary>
        /// 最近30天成交客户数
        /// </summary>
        public int ClosedCount30Days { get; set; }
    }
} 