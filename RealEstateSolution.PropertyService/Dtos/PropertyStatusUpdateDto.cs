using RealEstateSolution.Database.Models;
using System.ComponentModel.DataAnnotations;

namespace RealEstateSolution.PropertyService.Dtos
{
    /// <summary>
    /// 房源状态更新数据传输对象
    /// </summary>
    public class PropertyStatusUpdateDto
    {
        /// <summary>
        /// 房源状态
        /// </summary>
        [Required(ErrorMessage = "房源状态不能为空")]
        public PropertyStatus Status { get; set; }
    }
} 