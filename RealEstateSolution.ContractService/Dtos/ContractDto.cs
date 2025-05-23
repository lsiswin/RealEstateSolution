using System;

namespace RealEstateSolution.ContractService.Dtos
{
    public class ContractDto
    {
        public int Id { get; set; }
        public string ContractNumber { get; set; }
        public int PropertyId { get; set; }
        public int ClientId { get; set; }
        public decimal Amount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 