using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Models.Dto.Responses;
using Models.Enums;
using System.Net.Http.Json;
using static Models.Dto.Requests.WorkflowStepsRequest;
using Models.Dto.Common;

namespace UnitTest.AttributesAndControllers
{
    public class WorkflowStepsIntegrationTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory = factory;

        [Fact]
        public async Task QuerySearchList_WithValidPermission_ShouldReturnSuccess()
        {
            var client = _factory.CreateClient();

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // 用 Cookie 傳 token
            client.DefaultRequestHeaders.Add("Cookie",
                "accessToken=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySWQiOiI2M0MyMTgyRC1BRDFELTRFQ0QtQkVENy1EQjZGQzNERjlFRUMiLCJGZWF0dXJlTWFzayI6IjI2MjE0MyIsImV4cCI6MTc1ODYyNDk1NiwiaXNzIjoiTW9uaXRvcl9CYWNrRW5kIiwiYXVkIjoiTW9uaXRvcl9CYWNrRW5kX1VzZXJzIn0.qpNhjNTDPpFAUUbdc4pSTuLwGOkJYOuC-uCohX_A5hA");

            var request = new WorkflowStepsSearchListRequest
            {
                Page = new Models.Dto.Common.PageBase
                {
                    PageSize = 10,
                    PageIndex = 1,
                    TotalCount = 0
                },
                SortModel = new Option
                {
                    Key = "",
                    Value = ""
                },
                FilterModel = new(), // 如果允許空清單
                FieldModel = new WorkflowStepsSearchListFieldModelRequest
                {
                    Channel = "EDM"
                }
            };

            var response = await client.PostAsJsonAsync("api/WorkflowSteps/SearchList", request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var jsonString = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ResultResponse<object>>(jsonString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(result);
            Assert.Equal(ResultStatus.Success, result.Status);
        }
    }
}
