using System.ComponentModel.DataAnnotations;

namespace Clinic.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(20)]
        public string Name { get; set; }
        [Required]
        [MaxLength(50)]
        public string FullName { get; set; }
        [Required]
        [MaxLength(15)]
        public string Phone { get; set; }
        [Required]
        public string Password { get; set; }
        public string Role { get; set; } = "User";
        public List<Order> Order { get; set; }
    }
}
