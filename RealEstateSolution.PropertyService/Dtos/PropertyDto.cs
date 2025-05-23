using System;
using System.Collections.Generic;
using RealEstateSolution.Database.Models;

namespace RealEstateSolution.PropertyService.Dtos
{
    public class PropertyDto
    {
        public int Id { get; set; }
        public PropertyType Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal Area { get; set; }
        public string Address { get; set; }
        public DecorationType Decoration { get; set; }
        public OrientationType Orientation { get; set; }
        public int Floor { get; set; }
        public int TotalFloors { get; set; }
        public int Rooms { get; set; }
        public int Bathrooms { get; set; }
        public PropertyStatus Status { get; set; }
        public int OwnerId { get; set; }
        public List<string> ImageUrls { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 