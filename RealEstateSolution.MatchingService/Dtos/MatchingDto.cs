using System;

namespace RealEstateSolution.MatchingService.Dtos
{
    public class MatchingDto
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public int ClientId { get; set; }
        public decimal MatchScore { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 