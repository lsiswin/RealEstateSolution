using RealEstateSolution.Database.Models;

namespace RealEstateSolution.ClientService.Dtos
{
    /// <summary>
    /// 客户统计数据传输对象
    /// </summary>
    public class ClientStatsDto
    {
        /// <summary>
        /// 总客户数
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
        /// 成交客户数
        /// </summary>
        public int ClosedCount { get; set; }

        /// <summary>
        /// 买家数量
        /// </summary>
        public int BuyerCount { get; set; }

        /// <summary>
        /// 卖家数量
        /// </summary>
        public int SellerCount { get; set; }

        /// <summary>
        /// 租客数量
        /// </summary>
        public int TenantCount { get; set; }

        /// <summary>
        /// 房东数量
        /// </summary>
        public int LandlordCount { get; set; }

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