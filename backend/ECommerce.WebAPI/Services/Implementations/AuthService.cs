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
            var storedToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken && !rt.IsRevoked);

            if (storedToken == null || storedToken.ExpiresAt < DateTime.UtcNow)
                throw new UnauthorizedException("Invalid refresh token");

            var newAccessToken = _tokenService.GenerateAccessToken(storedToken.User);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            var expiresAt = DateTime.UtcNow.AddDays(7);

            storedToken.IsRevoked = true;
            await _context.RefreshTokens.AddAsync(new RefreshToken
            {
                Token = newRefreshToken,
                UserId = storedToken.UserId,
                ExpiresAt = expiresAt,
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            var userDto = _mapper.Map<UserDto>(storedToken.User);
            return new AuthResponseDto(newAccessToken, newRefreshToken, expiresAt, userDto);
        }

        public async Task RevokeTokenAsync(string refreshToken)
        {
            var storedToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (storedToken != null)
            {
                storedToken.IsRevoked = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task LogoutAsync()
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("User not authenticated");
            
            var activeTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ToListAsync();
            
            foreach (var token in activeTokens)
            {
                token.IsRevoked = true;
            }
            
            await _context.SaveChangesAsync();
        }

        public async Task InvalidateOrderCacheAsync(int userId)
        {
            _cache.Remove(string.Format(USER_ORDERS_CACHE_KEY, userId.ToString()));
            await Task.CompletedTask;
        }

        public async Task<UserDto> GetCurrentUserAsync()
        {
            var userId = _currentUserService.UserId 
                ?? throw new UnauthorizedException("User not authenticated");
            
            var user = await _context.Users.FindAsync(userId)
                ?? throw new NotFoundException("User not found");
                
            return _mapper.Map<UserDto>(user);
        }

        private async Task<AuthResponseDto> GenerateAuthResponseAsync(User user)
        {
            var token = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();
            var expiresAt = DateTime.UtcNow.AddDays(7);

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiresAt = expiresAt,
                CreatedAt = DateTime.UtcNow
            };

            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync();

            var userDto = _mapper.Map<UserDto>(user);
            return new AuthResponseDto(token, refreshToken, expiresAt, userDto);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email)
                ?? throw new NotFoundException("User not found");

            var resetToken = _tokenService.GeneratePasswordResetToken();
            user.PasswordResetToken = resetToken;
            user.ResetTokenExpiresAt = DateTime.UtcNow.AddHours(1);
            
            await _context.SaveChangesAsync();
            return resetToken;
        }

        public async Task ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            if (resetPasswordDto.NewPassword != resetPasswordDto.ConfirmPassword)
                throw new ECommerce.WebAPI.Exceptions.ValidationException("Passwords do not match");

            var user = await _context.Users.FirstOrDefaultAsync(u => 
                u.Email == resetPasswordDto.Email && 
                u.PasswordResetToken == resetPasswordDto.Token)
                ?? throw new NotFoundException("Invalid reset token");

            if (user.ResetTokenExpiresAt < DateTime.UtcNow)
                throw new ECommerce.WebAPI.Exceptions.ValidationException("Reset token has expired");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(resetPasswordDto.NewPassword);
            user.PasswordResetToken = null;
            user.ResetTokenExpiresAt = null;

            await _context.SaveChangesAsync();
        }
    }
}
