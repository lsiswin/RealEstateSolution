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
        public int TotalClients { get; set; }

        /// <summary>
        /// 活跃客户数
        /// </summary>
        public int ActiveClients { get; set; }

        /// <summary>
        /// 潜在客户数
        /// </summary>
        public int PotentialClients { get; set; }

        /// <summary>
        /// 成交客户数
        /// </summary>
        public int ClosedClients { get; set; }

        /// <summary>
        /// 无效客户数
        /// </summary>
        public int InvalidClients { get; set; }

        /// <summary>
        /// 买家数量
        /// </summary>
        public int BuyerClients { get; set; }

        /// <summary>
        /// 卖家数量
        /// </summary>
        public int SellerClients { get; set; }

        /// <summary>
        /// 租客数量
        /// </summary>
        public int TenantClients { get; set; }

        /// <summary>
        /// 房东数量
        /// </summary>
        public int LandlordClients { get; set; }

        /// <summary>
        /// 最近30天新增客户数
        /// </summary>
        public int NewClientsLast30Days { get; set; }

        /// <summary>
        /// 最近30天成交客户数
        /// </summary>
        public int ClosedClientsLast30Days { get; set; }
    }
} 