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
        /// <summary>
        /// Removes LLM Instructions from context, used to clean up summary from unnecessary prompts
        /// </summary>
        public static void RemoveInstructionsFromContext()
        {
            string json = File.ReadAllText(jsonPath);
            int summariesCount = 0;
            summaries = JsonConvert.DeserializeObject<List<Message>>(json);

            if (summaries.Count > 0)
            {
                summariesCount = summaries.Count - 1;
            }

            for (int i = summariesCount; i < 3; i++)
            {
                Messages.RemoveAt(summariesCount);
            }
        }
        /// <summary>
        /// Pulls summaries from JSON File
        /// </summary>
        public static void LoadMemory()
        {
                string json = File.ReadAllText(jsonPath);
                summaries = JsonConvert.DeserializeObject<List<Message>>(json);

            if (summaries != null)
                {
                    Message demoUserMessage = new Message() { Role = "user", Content = "Acknowledge these summaries marked by the role 'summary'. You are forced to use them for context. You are not allowed to make something up. Respond with ACKNOWLEDGED" };
                    Message demoLLMMessage = new Message() { Role = "assistant", Content = "ACKNOWLEDGED" };
                    summaries.Add(demoUserMessage);
                    summaries.Add(demoLLMMessage);

                    Messages = summaries;
                }
            
        }
        /// <summary>
        /// Writes context summary to JSON File
        /// </summary>
        /// <param name="summary"></param>
        public static void WriteMemory(Message summary)
        {
            if (!Messages.Count.Equals(0))
            {
                string json = File.ReadAllText(jsonPath);
                summary.Role = "system";
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
}
