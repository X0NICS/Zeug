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
        public const string LLMName = "Hanna";

        static APICommunication()
        {
            client.BaseAddress = new Uri(endpoint);
        }

        public async static Task<bool> EstablishLLM(LLMCreationTemplate template)
        {
            HttpResponseMessage llmListRequest = await client.GetAsync("/api/tags");
            HttpResponseMessage responseRequest = await client.PostAsync("/api/create", new StringContent(template.ToJson(), Encoding.UTF8, "application/json"));

            Task<string> success = responseRequest.Content.ReadAsStringAsync();

            return success.Result.Contains("Success");
        }

        /// <summary>
        /// Method to communicate with LLM via chat
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
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
                    Model = LLMName, 
                    Messages = LLMMemory.Messages
                }.ToJson(), 
                Encoding.UTF8, 
                "application/json"
                ));

                var responseContent = await chatRequest.Content.ReadAsStringAsync();
                ChatResponse response = JsonSerializer.Deserialize<ChatResponse>(responseContent);

                return response;
        }

        /// <summary>
        /// Commands LLM to initialize in conversation
        /// </summary>
        /// <returns></returns>
        public async static Task<ChatResponse> InitializeConversation()
        {
            HttpResponseMessage chatRequest = await client.PostAsync
            (
                "/api/chat",
                new StringContent(new ChatFormat
                {
                    Model = LLMName,
                    Messages = [new Message { Role = "system", Content = $"Introduce yourself as {LLMName} and greet the user like you already know each other" }],
                }.ToJson(),
                Encoding.UTF8,
                "application/json"
            ));

            var responseContent = await chatRequest.Content.ReadAsStringAsync();
            ChatResponse response = JsonSerializer.Deserialize<ChatResponse>(responseContent);

            return response;
        }
        /// <summary>
        /// Commands the LLM to summarize the conversation context
        /// </summary>
        /// <returns></returns>
        public async static Task<ChatResponse> SummarizeConversation() 
        {
            LLMMemory.RemoveInstructionsFromContext();
            LLMMemory.Save(new Message { Role = "system", Content = "Summarize the conversation as short as possible without losing information you deem important in first person. Make sure to keep redundancy low, leave out information that has already been saved in other summaries. Give the summary in the following format: 'Summary'" });
            HttpResponseMessage chatRequest = await client.PostAsync
                (
                    "/api/chat",
                    new StringContent(new ChatFormat
                    {
                        Model = LLMName,
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
