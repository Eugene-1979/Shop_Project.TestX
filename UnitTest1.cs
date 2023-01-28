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
using Shop_Project.Repository;
using Moq;
using Shop_Project.Db;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shop_Project.Models;
using Microsoft.EntityFrameworkCore;
using Shop_Project.Data;

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



        [Fact]
        public async Task GetRedirectIfNotAuth()
            {
            // Arrange
            var client = _factory.CreateClient(
                new WebApplicationFactoryClientOptions
                    {
                    AllowAutoRedirect = false
                    });
            // Act
            var response = await client.GetAsync("/Products/Edit?id=1");
            // Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Contains("Identity/Account/Login",
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
            var response = await client.GetAsync("/Test/Index");
            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

        [Fact]
        public async Task TestWithMock()
            {
            WebApplicationFactory<Program> webApplicationFactory = _factory.WithWebHostBuilder(builder =>
            builder.ConfigureTestServices(service =>
            {/*Находим и удаляем сервис ITest и подменяем результат на True*/
                var serv = service.FirstOrDefault(q => q.ServiceType == typeof(ITest));
                service.Remove(serv);
                /* Реализуем через Moq , и меняем ReturnFalse() == true*/
                Mock<ITest> mock = new Mock<ITest>();
                mock.Setup(q => q.ReturnFalse()).Returns(() => true);
                /*  Добавляем Mock*/
                service.AddScoped(q => mock.Object);
            }
             )

            );

            HttpClient httpClient = webApplicationFactory.CreateClient();
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync("Test1");

            Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
            }


        [Fact]
        public async Task TestWithMockDb()
            {
            WebApplicationFactory<Program> webApplicationFactory = _factory.WithWebHostBuilder(builder => {
                builder.ConfigureTestServices(servCol =>
                {


                    ServiceDescriptor? serviceDescriptor = servCol.FirstOrDefault(q => q.ServiceType == typeof(DbContextOptions<AppDbContent>));
                    servCol.Remove(serviceDescriptor);

                    servCol.AddDbContext<AppDbContent>(option =>
                    option.UseInMemoryDatabase("_ShopBase11234")
                    );

                    /*  Mock<AppDbContent> mock = new Mock<AppDbContent>();
                      mock.Setup(q=>q.Products).Returns(()=>new DbSet<Product>())*/

                });
                          
            });
            
             AppDbContent? appDbContent = webApplicationFactory.Services.CreateScope().ServiceProvider.GetService<AppDbContent>();
            List<Order> orders = new() { new Order(), new Order() };
           await appDbContent.Orders.AddRangeAsync(orders);
            await appDbContent.SaveChangesAsync();

            HttpClient httpClient = webApplicationFactory.CreateClient();
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync("Test2");
            Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
            string v = await httpResponseMessage.Content.ReadAsStringAsync();
            int temp = int.Parse(v);

            Assert.Equal(orders.Count, temp);

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
            }





        }

        }