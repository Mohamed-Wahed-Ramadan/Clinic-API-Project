using Clinic.DTOs;
using Clinic.Interfaces;
using Clinic.Interfaces.Services;
using Clinic.Models;
using Microsoft.AspNetCore.Identity;

namespace Clinic.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;

        public AuthService(IUnitOfWork unitOfWork, IJwtService jwtService)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
        }

        public async Task<object> RegisterAsync(UserRegisterDTO dto)
        {
            var exists = await _unitOfWork.Users.NameExistsAsync(dto.Name);
            if (exists)
                throw new Exception("This name is already taken");

            var user = new User
            {
                Name = dto.Name,
                FullName = dto.FullName,
                Phone = dto.Phone,
                Role = "User"
            };

            var hasher = new PasswordHasher<User>();
            user.Password = hasher.HashPassword(user, dto.Password);

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.CompleteAsync();

            var token = _jwtService.GenerateToken(user);

            return new
            {
                message = "User registered successfully",
                token,
                user = new
                {
                    user.Id,
                    user.Name,
                    user.FullName,
                    user.Phone,
                    user.Role
                }
            };
        }

        public async Task<object> LoginAsync(UserLoginDTO dto)
        {
            var user = await _unitOfWork.Users.GetByNameAsync(dto.Name);
            if (user == null)
                throw new Exception("Username or password is incorrect");

            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.Password, dto.Password);

            if (result != PasswordVerificationResult.Success)
                throw new Exception("Invalid credentials");

            var token = _jwtService.GenerateToken(user);

            return new
            {
                message = "Login successful",
                token,
                user = new
                {
                    user.Id,
                    user.Name,
                    user.FullName,
                    user.Phone,
                    user.Role
                }
            };
        }
    }
}