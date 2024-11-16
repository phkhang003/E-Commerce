using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using ECommerce.WebAPI.Data;
using ECommerce.WebAPI.Models.DTOs;
using ECommerce.WebAPI.Models.Entities;
using ECommerce.WebAPI.Services.Implementations;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace ECommerce.WebAPI.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _mockTokenService = new Mock<ITokenService>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        
        _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
            
        var context = new ApplicationDbContext(_dbContextOptions);
        _sut = new AuthService(context, _mockTokenService.Object, _mockHttpContextAccessor.Object);
    }

    // Copy phần tests từ AuthService.cs sang đây
}