using RealEstateSolution.Database.Models;

namespace RealEstateSolution.ClientService.Dtos
{
    /// <summary>
    /// 客户需求数据传输对象
    /// </summary>
    public class ClientRequirementDto
    {
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
        /// 位置
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// 房源类型
        /// </summary>
        public PropertyType? PropertyType { get; set; }

        /// <summary>
        /// 其他要求
        /// </summary>
        public string OtherRequirements { get; set; }

        /// <summary>
        /// 价格范围显示（只读）
        /// </summary>
        public string PriceRangeDisplay { get; set; } = string.Empty;

        /// <summary>
        /// 面积范围显示（只读）
        /// </summary>
        public string AreaRangeDisplay { get; set; } = string.Empty;

        /// <summary>
        /// 房源类型显示（只读）
        /// </summary>
        public string PropertyTypeDisplay { get; set; } = string.Empty;
    }
} 