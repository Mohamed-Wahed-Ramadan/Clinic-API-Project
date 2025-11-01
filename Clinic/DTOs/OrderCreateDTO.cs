using System.ComponentModel.DataAnnotations;

namespace Clinic.DTOs
{
    public class OrderCreateDTO
    {
        [Required]
        [MaxLength(150)]
        public string Description { get; set; }
        public decimal? Price { get; set; }
        public DateTime? NextDate { get; set; }
        public string Status { get; set; } = "Waiting";
        public string? StatusType { get; set; }
    }

    public class OrderUpdateDTO
    {
        [MaxLength(150)]
        public string Description { get; set; }
        public decimal? Price { get; set; }
        public DateTime? NextDate { get; set; }
        public string Status { get; set; }
        public string? StatusType { get; set; }
    }
}