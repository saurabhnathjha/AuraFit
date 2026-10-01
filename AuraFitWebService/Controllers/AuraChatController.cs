using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using AuraFitWebService.Models;
using Microsoft.AspNetCore.Authorization;
using AuraFitWebService.DTOs;

namespace AuraFitWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuraChatController : Controller
    {
        private readonly HttpClient _httpClient;

        public AuraChatController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [Authorize]
        [HttpPost("ask")]
        public async Task<IActionResult> AskChatbot([FromBody] QueryDTO query)
        {
            var chatRequest = new
            {
                messages = new[]
                {
                new { role = "user", content = "Answer if question is related to fitness, exercise, health, nutrition etc." +
                "If unrelated, reply something like I specialize in fitness and nutrition. " +
                "Please ask something related to health, workouts, or diet etc.. In 10 - 50 words." + query.Query }
            },
                model = "gpt-4o-mini"
            };

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://chatgpt-42.p.rapidapi.com/chat"),
                Headers =
            {
                { "x-rapidapi-key", "0e3fab7c02mshd5e25eded8a27bcp1e3417jsnadea582eb52f" },
                { "x-rapidapi-host", "chatgpt-42.p.rapidapi.com" }
            },
                Content = new StringContent(JsonSerializer.Serialize(chatRequest), Encoding.UTF8, "application/json")
            };

            using var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var parsed = JsonSerializer.Deserialize<ChatbotApiResponse>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var reply = parsed?.Choices?.FirstOrDefault()?.Message?.Content ?? "No response";
            return Ok(new { response = reply });
        }
    }
}
