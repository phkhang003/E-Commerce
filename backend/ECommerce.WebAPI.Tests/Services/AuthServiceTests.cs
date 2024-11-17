using Microsoft.Extensions.Caching.Memory;
using AutoMapper;
using Moq;
using Xunit;
using ECommerce.WebAPI.Services.Interfaces;
using ECommerce.WebAPI.Services.Implementations;
using ECommerce.WebAPI.Models.DTOs;
using ECommerce.WebAPI.Models.Entities;
using ECommerce.WebAPI.Repositories.Interfaces;
using FluentAssertions;
using ECommerce.WebAPI.Exceptions;
using Microsoft.AspNetCore.Http;
using ECommerce.WebAPI.Data;
using ECommerce.WebAPI.Services.Interfaces;

namespace ECommerce.WebAPI.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IMemoryCache> _mockCache;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _mockUserRepo = new Mock<IUserRepository>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _mockMapper = new Mock<IMapper>();
        _mockCache = new Mock<IMemoryCache>();
        var mockDbContext = new Mock<ApplicationDbContext>();
        var mockTokenService = new Mock<ITokenService>();
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

        _sut = new AuthService(
            mockDbContext.Object,
            mockTokenService.Object,
            mockHttpContextAccessor.Object,
            _mockCurrentUserService.Object,
            _mockMapper.Object,
            _mockCache.Object
        );
    }

    // Add your test methods here
}