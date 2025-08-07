using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Chatbot.Resources
{
    static class APICommunication
    {
        static string endpoint = "http://127.0.0.1:11434";
        static HttpClient client = new HttpClient();

        static APICommunication()
        {
            client.BaseAddress = new Uri(endpoint);
        }

        public async static Task<bool> EstablishLLM(LLMCreationTemplate template)
        {
            HttpResponseMessage responseRequest = await client.PostAsync("/api/create", new StringContent(template.ToJson(), Encoding.UTF8, "application/json"));

            Task<string> success = responseRequest.Content.ReadAsStringAsync();

            return success.Result.Contains("Success");
        }

        public async static Task<ChatResponse> ChatWithLLM(string message) 
        {
            Message userMessage = new Message()
            {
                Role = "user",
                Content = message
            };
            LLMMemory.Save(userMessage);

            HttpResponseMessage chatRequest = await client.PostAsync
                (
                "/api/chat", 
                new StringContent(new ChatFormat 
                { 
                    Model = "Hanna", 
                    Messages = LLMMemory.Messages
                }.ToJson(), 
                Encoding.UTF8, 
                "application/json"
                ));

                var responseContent = await chatRequest.Content.ReadAsStringAsync();
                ChatResponse response = JsonSerializer.Deserialize<ChatResponse>(responseContent);

                return response;
        }

        public async static Task<ChatResponse> InitializeConversation()
        {
            HttpResponseMessage chatRequest = await client.PostAsync
            (
                "/api/chat",
                new StringContent(new ChatFormat
                {
                    Model = "Hanna",
                    Messages = [new Message { Role = "system", Content = "Introduce yourself as Hanna and greet the user" }],
                }.ToJson(),
                Encoding.UTF8,
                "application/json"
            ));

            var responseContent = await chatRequest.Content.ReadAsStringAsync();
            ChatResponse response = JsonSerializer.Deserialize<ChatResponse>(responseContent);

            return response;
        }

        public async static Task<ChatResponse> SummarizeConversation() 
        {
            LLMMemory.Save(new Message { Role = "system", Content = "Summarize the conversation as short as possible without losing information you deem important in thrid person" });
            HttpResponseMessage chatRequest = await client.PostAsync
                (
                    "/api/chat",
                    new StringContent(new ChatFormat
                    {
                        Model = "Hanna",
                        Messages = LLMMemory.Messages
                    }.ToJson(),
                    Encoding.UTF8,
                    "application/json"
                ));

            var responseContent = await chatRequest.Content.ReadAsStringAsync();
            ChatResponse response = JsonSerializer.Deserialize<ChatResponse>(responseContent);

            return response;
        }
    }

    class ChatFormat 
    {
        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("messages")]
        public List<Message> Messages { get; set; }

        [JsonPropertyName("stream")]
        public bool Stream { get; set; } = false;

        public string ToJson() 
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
