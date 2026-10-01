// Services/NutritionixService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using AuraFitDataAccessLayer.Models;
using AuraFitWebService.DTOs;
using AuraFitWebService.NutririonixModels;

namespace AuraFitWebService.Services
{
    public class NutritionixService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly string _appId;
        private readonly string _apiKey;

        public NutritionixService(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;

            _appId = _config["Nutritionix:AppId"];
            _apiKey = _config["Nutritionix:ApiKey"];
        }

        public async Task<List<ParsedFoodItem>> ParseMealQueryAsync(string query)
        {
            var client = _httpClientFactory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Post, "https://trackapi.nutritionix.com/v2/natural/nutrients");
            request.Headers.Add("x-app-id", _appId);
            request.Headers.Add("x-app-key", _apiKey);
            request.Content = new StringContent(
                JsonSerializer.Serialize(new { query }),
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Nutritionix API Error: " + response.StatusCode);
                return new List<ParsedFoodItem>();
            }

            using var stream = await response.Content.ReadAsStreamAsync();
            var nutritionixResponse = await JsonSerializer.DeserializeAsync<NutritionixResponse>(stream);

            if (nutritionixResponse?.Foods == null)
                return new List<ParsedFoodItem>();

            return nutritionixResponse.Foods.Select(food => new ParsedFoodItem
            {
                Name = food.FoodName,
                Quantity = $"{food.ServingQty} {food.ServingUnit}",
                Calories = (int)Math.Round(food.Calories)
            }).ToList();
        }



    }
}
