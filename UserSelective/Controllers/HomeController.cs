using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UserSelective.Models;

namespace AIChatbot.Controllers
{
    public class HomeController : Controller
    {
        const string API_KEY = "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";
        private readonly ILogger<HomeController> _logger;
        static readonly HttpClient client = new HttpClient();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View(new ChatbotOptionsModel());
        }

        public async Task<IActionResult> Get(ChatbotOptionsModel model)
        {
            string context = GetOptionMessage(model.SelectedOption);
            string addon = GetAddonMessage(model.SelectedOption);

            var options = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new
                    {
                        role = "system",
                        content = context
                    },
                    new
                    {
                        role = "user",
                        content = $"{model.InputPrompt}\nnote: {addon}"
                    }
                },
                max_tokens = 3500,
                temperature = 0.2
            };

            var json = JsonConvert.SerializeObject(options);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", API_KEY);

            try
            {
                var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();

                dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);
                string result = jsonResponse.choices[0].message.content;

                return PartialView("_ChatLog", new ChatLogModel
                {
                    UserInput = $"{GetOptionMessage(model.SelectedOption)}: {model.InputPrompt}",
                    ChatbotResponse = $"Chatbot: {result}"
                });
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        private string GetOptionMessage(string context)
        {
            switch (context)
            {
                case "1":
                    return "Need Answer";
                case "2":
                    return "Need Code";
                case "3":
                    return "Need Suggestion";
                default:
                    return "User";
            }
        }

        private string GetAddonMessage(string context)
        {
            switch (context)
            {
                case "1":
                    return "give an answer";
                case "2":
                    return "give code";
                case "3":
                    return "give suggestion";
                default:
                    return "";
            }
        }
    }
}
