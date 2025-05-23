using System;

namespace RealEstateSolution.RecycleService.Dtos
{
    public class RecycleDto
    {
        public int Id { get; set; }
        public string RecycleNumber { get; set; }
        public int PropertyId { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 