using System.ComponentModel.DataAnnotations;

namespace Clinic.DTOs
{
    public class UserUpdateDTO
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; } = string.Empty;
        public string Role { get; set; } = "User";

    }
}
