using RealEstateSolution.Database.Models;

namespace RealEstateSolution.PropertyService.Models
{
    /// <summary>
    /// 房源状态更新数据传输对象
    /// </summary>
    public class PropertyStatusUpdateDto
    {
        /// <summary>
        /// 房源状态
        /// </summary>
        public PropertyStatus Status { get; set; }
    }
} 