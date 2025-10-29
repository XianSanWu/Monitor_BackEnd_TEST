using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Models.Dto.Responses;
using Models.Enums;
using System.Net.Http.Json;
using static Models.Dto.Requests.WorkflowStepsRequest;
using Models.Common;

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
                "accessToken=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySWQiOiJERjM5QjlFNi03NEQ0LTQwQUItOURFMi1FREVFQTYwNzJBQjQiLCJGZWF0dXJlTWFzayI6IjEiLCJleHAiOjE3NTk4MTgzODEsImlzcyI6Ik1vbml0b3JfQmFja0VuZCIsImF1ZCI6Ik1vbml0b3JfQmFja0VuZF9Vc2VycyJ9.KDqA5nGRCDipF5DhXla2jSPIuw7-6exYO5Vq89shnfA");

            var request = new WorkflowStepsSearchListRequest
            {
                Page = new PageBase
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
