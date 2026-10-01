using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AuraFitWebService.Controllers;
using AuraFitWebService.DTOs;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.Protected;
using Xunit;
using Microsoft.Extensions.DependencyInjection;

namespace AuraFitAPITestin
{
    public class AuraChatControllerTest
    {
        [Fact]
        public async Task AskChatbot_ReturnsOk_WithExpectedResponse()
        {
            // Arrange
            var expectedResponse = "Mocked response from AI.";
            var responseJson = JsonSerializer.Serialize(new
            {
                choices = new[]
                {
                    new {
                        message = new {
                            content = expectedResponse
                        }
                    }
                }
            });

            // Mock HttpMessageHandler to simulate external API response
            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
                });

            var httpClient = new HttpClient(mockHandler.Object);

            // Mock IHttpClientFactory to return the mocked HttpClient
            var mockFactory = new Mock<IHttpClientFactory>();
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var controller = new AuraChatController(mockFactory.Object);

            var queryDto = new QueryDTO
            {
                Query = "Tell me something about exercise."
            };

            // Act
            var result = await controller.AskChatbot(queryDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var json = JsonSerializer.Serialize(okResult.Value);
            using var doc = JsonDocument.Parse(json);
            var actualResponse = doc.RootElement.GetProperty("response").GetString();

            Assert.Equal(expectedResponse, actualResponse);
        }
    }
}
