using System.ComponentModel.DataAnnotations;

namespace Clinic.DTOs
{
    public class UserRegisterDTO
    {
        [Required] 
        public string Name { get; set; }
        [Required] 
        public string FullName { get; set; }
        [Required] 
        public string Phone { get; set; }
        [Required] 
        public string Password { get; set; }
    }
}
