using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic.Models
{
    public class Order
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Description { get; set; }

        public int? Number { get; set; }
        public decimal? Price { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? NextDate { get; set; }

        public string Status { get; set; } = "Waiting";
        public string? StatusType { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public User? User { get; set; }
    }
}