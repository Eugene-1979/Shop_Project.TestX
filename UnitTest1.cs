using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.Extensions.DependencyInjection;

namespace Shop_Project.TestX
    {
    public class UnitTest1 : IClassFixture<WebApplicationFactory<Program>>
        {
        private readonly WebApplicationFactory<Program> _factory;

        public UnitTest1(WebApplicationFactory<Program> factory)
            {
            _factory = factory;
            }

        [Theory]
        [InlineData("/")]
        [InlineData("Home/Index")]
      /*  [InlineData("Home/ViewUser?id=7")]
        [InlineData("Home/Login")]*/
        public async Task GetPages(string url)
            {
            // Arrange
            var client = _factory.CreateClient();
            // Act
            var response = await client.GetAsync(url);
            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType?.ToString());
            }



/*        [Fact]
        public async Task GetRedirectIfNotAuth()
            {
            // Arrange
            var client = _factory.CreateClient(
                new WebApplicationFactoryClientOptions
                    {
                    AllowAutoRedirect = false
                    });
            // Act
            var response = await client.GetAsync("/Home/EditUser?id=7");
            // Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Contains("/Home/Login",
                response.Headers.Location?.OriginalString);
            }

        [Fact]
        public async Task Get_SecurePageIsReturnedForAnAuthenticatedUser()
            {
            // Arrange
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                    services.AddAuthentication(defaultScheme: "TestScheme")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                        "TestScheme", _ => { }));
            })
                .CreateClient(new WebApplicationFactoryClientOptions
                    {
                    AllowAutoRedirect = false,
                    });

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(scheme: "TestScheme");
            //Act
            var response = await client.GetAsync("/Home/EditUser?id=7");
            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
        {
        public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
            {
            }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
            {
            var claims = new[]
            {
            new Claim(ClaimTypes.Name, "Test admin"),
            new(ClaimsIdentity.DefaultRoleClaimType, "Admin")
        };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "TestScheme");
            var result = AuthenticateResult.Success(ticket);
            return Task.FromResult(result);
            }*/





        }

        }