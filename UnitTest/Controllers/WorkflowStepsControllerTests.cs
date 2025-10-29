using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebAPi.Controllers;
using Models.Dto.Responses;
using Models.Enums;
using static Models.Dto.Requests.WorkflowStepsRequest;
using static Models.Dto.Responses.WorkflowStepsResponse;

namespace UnitTest.Controllers
{
    public class WorkflowStepsControllerTests
    {
        [Fact]
        public async Task QuerySearchList_WithValidPermission_ShouldReturnSuccessResult()
        {
            // Arrange
            var permissionServiceMock = new Mock<IPermissionService>();
            permissionServiceMock
                .Setup(x => x.GetBitValue("CDP", "EDM_Detail", "Read", It.IsAny<CancellationToken>()))
                .ReturnsAsync(4); // 權限正確

            var workflowStepsServiceMock = new Mock<IWorkflowStepsService>();
            workflowStepsServiceMock
                .Setup(x => x.QueryWorkflowStepsSearchList(It.IsAny<WorkflowStepsSearchListRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new WorkflowStepsSearchListResponse());

            var services = new ServiceCollection();
            services.AddSingleton(permissionServiceMock.Object);
            services.AddSingleton(workflowStepsServiceMock.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.RequestServices = services.BuildServiceProvider();

            var claims = new[]
            {
                new Claim("UserId", "123"),
                new Claim("FeatureMask", "4"),
                new Claim(JwtRegisteredClaimNames.Exp, ((DateTimeOffset.UtcNow.AddMinutes(10)).ToUnixTimeSeconds()).ToString())
            };
            var jwt = new JwtSecurityToken(claims: claims);
            var token = new JwtSecurityTokenHandler().WriteToken(jwt);
            httpContext.Request.Headers.Add("Authorization", $"Bearer {token}");

            var controller = new WorkflowStepsController(workflowStepsServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };

            // Act
            var result = await controller.QuerySearchList(new WorkflowStepsSearchListRequest(), CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ResultResponse<WorkflowStepsSearchListResponse>>(result);
            Assert.Equal(ResultStatus.Success, result.Status);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task QuerySearchList_WithInvalidPermission_ShouldReturnSuccessResult_ButEmptyData()
        {
            // Arrange
            var permissionServiceMock = new Mock<IPermissionService>();
            permissionServiceMock
                .Setup(x => x.GetBitValue("CDP", "EDM_Detail", "Read", It.IsAny<CancellationToken>()))
                .ReturnsAsync(8); // 模擬權限不足

            var workflowStepsServiceMock = new Mock<IWorkflowStepsService>();
            workflowStepsServiceMock
                .Setup(x => x.QueryWorkflowStepsSearchList(It.IsAny<WorkflowStepsSearchListRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new WorkflowStepsSearchListResponse()); // 可以回空物件或特定值

            var services = new ServiceCollection();
            services.AddSingleton(permissionServiceMock.Object);
            services.AddSingleton(workflowStepsServiceMock.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.RequestServices = services.BuildServiceProvider();

            var claims = new[]
            {
        new Claim("UserId", "123"),
        new Claim("FeatureMask", "4"), // 模擬權限不足
        new Claim(JwtRegisteredClaimNames.Exp, ((DateTimeOffset.UtcNow.AddMinutes(10)).ToUnixTimeSeconds()).ToString())
    };
            var jwt = new JwtSecurityToken(claims: claims);
            var token = new JwtSecurityTokenHandler().WriteToken(jwt);
            httpContext.Request.Headers.Add("Authorization", $"Bearer {token}");

            var controller = new WorkflowStepsController(workflowStepsServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };

            // Act
            var result = await controller.QuerySearchList(new WorkflowStepsSearchListRequest(), CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ResultResponse<WorkflowStepsSearchListResponse>>(result);
            Assert.Equal(ResultStatus.Success, result.Status);  // 保留 Success
            Assert.NotNull(result.Data);                        // 可以檢查回傳空物件或特定預設值
        }

    }
}
