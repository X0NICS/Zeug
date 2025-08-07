using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Chatbot.Resources
{
    class ChatResponse
    {
        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }

        [JsonPropertyName("message")]
        public Message Message {  get; set; }

        [JsonPropertyName("done_reason")]
        public string DoneReason { get; set; }

        [JsonPropertyName("done")]
        public bool Done { get; set; }

        [JsonPropertyName("total_duration")]
        public float TotalDuration { get; set; }

        [JsonPropertyName("load_duration")]
        public float LoadDuration { get; set; }

        [JsonPropertyName("prompt_eval_count")]
        public float PromptEvalCount { get; set; }

        [JsonPropertyName("prompt_eval_duration")]
        public float PromptEvalDuration { get; set; }

        [JsonPropertyName("eval_count")]
        public float EvalCount { get; set; }

        [JsonPropertyName("eval_duration")]
        public float EvalDuration { get; set; }

    }
}
