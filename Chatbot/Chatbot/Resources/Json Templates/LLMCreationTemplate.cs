using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Chatbot.Resources
{
    class LLMCreationTemplate
    {
        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("from")]
        public string From { get; set; }

        [JsonPropertyName("system")]
        public string System { get; set; }


    public LLMCreationTemplate(string name, string template, string system = "You are a helpful ai assitant")
        {
            Model = name;
            From = template;
            System = system;
        }

        /// <summary>
        /// Turns the object into JSON format. Needed for the http requests
        /// </summary>
        /// <returns>Object in JSON format</returns>
        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
