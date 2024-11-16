using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using ECommerce.WebAPI.Data;
using ECommerce.WebAPI.Models.DTOs;
using ECommerce.WebAPI.Models.Entities;
using ECommerce.WebAPI.Services.Interfaces;
using ECommerce.WebAPI.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using ECommerce.WebAPI.Models.DTOs.Auth;
using FluentValidation;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;

namespace ECommerce.WebAPI.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private const string USER_ORDERS_CACHE_KEY = "user_orders_{0}";

        public AuthService(
            ApplicationDbContext context,
            ITokenService tokenService,
            IHttpContextAccessor httpContextAccessor,
            ICurrentUserService currentUserService,
            IMapper mapper,
            IMemoryCache cache)
        {
            _context = context;
            _tokenService = tokenService;
            _httpContextAccessor = httpContextAccessor;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                throw new ECommerce.WebAPI.Exceptions.ValidationException("Invalid email or password");

            return await GenerateAuthResponseAsync(user);
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
                throw new ECommerce.WebAPI.Exceptions.ValidationException("Email already exists");

            var user = new User
            {
                Email = registerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Role = "User"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return await GenerateAuthResponseAsync(user);
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string token, string refreshToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                throw new UnauthorizedException("Invalid refresh token");

            return await GenerateAuthResponseAsync(user);
        }

        public async Task RevokeTokenAsync(string refreshToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;
                await _context.SaveChangesAsync();
            }
        }

        public async Task LogoutAsync()
        {
            var userId = _currentUserService.GetUserId();
            var user = await _context.Users.FindAsync(userId);
            
            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;
                await _context.SaveChangesAsync();
            }
        }

        public async Task InvalidateOrderCacheAsync(int userId)
        {
            _cache.Remove(string.Format(USER_ORDERS_CACHE_KEY, userId.ToString()));
            await Task.CompletedTask;
        }

        private async Task<AuthResponseDto> GenerateAuthResponseAsync(User user)
        {
            var token = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();
            var expiresAt = DateTime.UtcNow.AddHours(1);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();

            var userDto = new UserDto
            {
                Id = user.Id.ToString(),
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role
            };

            return new AuthResponseDto(token, refreshToken, expiresAt, userDto);
        }
    }
}
