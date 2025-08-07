using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chatbot.Resources
{
    static class LLMMemory
    {
        public static List<Message> Messages { get; private set; } = [];
        static List<Message> summaries = new List<Message>();
        static string jsonPath = @"context.json";

        public static void Save(Message message)
        {
            Messages.Add(message);
        }

        public static void LoadMemory()
        {
                string json = File.ReadAllText(jsonPath);
                summaries = JsonConvert.DeserializeObject<List<Message>>(json);

                if (summaries != null)
                {
                    Message demoUserMessage = new Message() { Role = "user", Content = "Acknowledge these summaries. You are forced to use them as context. You are not allowed to make something up. Respond with ACKNOWLEDGED" };
                    Message demoLLMMessage = new Message() { Role = "assistant", Content = "ACKNOWLEDGED" };
                    summaries.Add(demoUserMessage);
                    summaries.Add(demoLLMMessage);

                    Messages = summaries;
                }
            
        }

        public static void WriteMemory(Message summary)
        {
            string json = File.ReadAllText(jsonPath);
            summaries = JsonConvert.DeserializeObject<List<Message>>(json);
            if (summaries != null)
            {
                summaries.Add(summary);
            }
            else
            {
                summaries = [summary];
            }

            string formattedSummaries = JsonConvert.SerializeObject(summaries);

            File.WriteAllText(jsonPath, formattedSummaries);
        }
    }
}
