public class TokenServiceTests
{
    private readonly TokenService _tokenService;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ApplicationDbContext _context;

    public TokenServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _configurationMock = new Mock<IConfiguration>();
        _mapperMock = new Mock<IMapper>();

        _configurationMock.Setup(x => x["Jwt:Key"]).Returns("your-secret-key-with-minimum-16-characters");
        _configurationMock.Setup(x => x["Jwt:Issuer"]).Returns("your-issuer");
        _configurationMock.Setup(x => x["Jwt:Audience"]).Returns("your-audience");

        _tokenService = new TokenService(_configurationMock.Object, _context, _mapperMock.Object);
    }

    [Fact]
    public void GenerateAccessToken_ShouldReturnValidToken()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Email = "test@example.com",
            Role = "User"
        };

        // Act
        var token = _tokenService.GenerateAccessToken(user);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }

    [Fact]
    public void GenerateRefreshToken_ShouldReturnUniqueTokens()
    {
        // Act
        var token1 = _tokenService.GenerateRefreshToken();
        var token2 = _tokenService.GenerateRefreshToken();

        // Assert
        Assert.NotEqual(token1, token2);
    }
}