using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing; 
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebApi.Attributes;
using Microsoft.AspNetCore.Mvc.Abstractions;

namespace UnitTest.Attributes
{
    public class PermissionGroupFilterAttributeTests
    {
        private class TestCookiesFeature : IRequestCookiesFeature
        {
            public required IRequestCookieCollection Cookies { get; set; }
        }

        private class TestCookieCollection : IRequestCookieCollection
        {
            private readonly Dictionary<string, string> _cookies;
            public TestCookieCollection(Dictionary<string, string> cookies) => _cookies = cookies;
            public string this[string key] => _cookies[key];
            public int Count => _cookies.Count;
            public ICollection<string> Keys => _cookies.Keys;
            public bool ContainsKey(string key) => _cookies.ContainsKey(key);
            public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _cookies.GetEnumerator();
            public bool TryGetValue(string key, out string value) => _cookies.TryGetValue(key, out value);
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _cookies.GetEnumerator();
        }

        [Fact]
        public async Task OnAuthorizationAsync_ValidPermission_ShouldPass()
        {
            // Arrange
            var permissionString = "Module=Test;Feature=FeatureA;Action=Read";
            var attribute = new PermissionGroupFilterAttribute(permissionString);

            var httpContext = new DefaultHttpContext();

            // 模擬 JWT Token
            var claims = new[]
            {
                new Claim("UserId", "123"),
                new Claim("FeatureMask", "4"),
                new Claim(JwtRegisteredClaimNames.Exp, ((DateTimeOffset.UtcNow.AddMinutes(10)).ToUnixTimeSeconds()).ToString())
            };
            var jwt = new JwtSecurityToken(claims: claims);
            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            // 用自訂 CookieCollection 注入 Cookie
            var cookies = new Dictionary<string, string> { { "accessToken", token } };
            var cookieFeature = new TestCookiesFeature { Cookies = new TestCookieCollection(cookies) };
            httpContext.Features.Set<IRequestCookiesFeature>(cookieFeature);

            // 模擬 IPermissionService
            var permissionServiceMock = new Mock<IPermissionService>();
            permissionServiceMock
                .Setup(x => x.GetBitValue(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(4);

            var services = new ServiceCollection();
            services.AddSingleton(permissionServiceMock.Object);
            httpContext.RequestServices = services.BuildServiceProvider();

            // 初始化 RouteData
            var routeData = new RouteData();

            var context = new AuthorizationFilterContext(
                new ActionContext(httpContext, routeData, new ActionDescriptor()),
                new List<IFilterMetadata>()
            );

            // Act
            await attribute.OnAuthorizationAsync(context);

            // Assert
            Assert.Null(context.Result); // 通過授權
        }
    }
}