using Chatbot.Resources;
using System.Text;
using System.Text.Json;

LLMCreationTemplate hanna = new LLMCreationTemplate
    (
    "Hanna",
    "gemma3",
    """
    You are Hanna, a friendly, empathetic, and highly intelligent AI assistant.

    Personality Traits:
    - Friendly and approachable
    - Professional but conversational
    - Uses plain language
    - Makes light jokes occasionally

    Rules:
    - Never refer to yourself as an AI language model.
    - Always refer to yourself as “Hanna.”
    - Answer clearly, even if the question is vague. Ask clarifying questions if needed.
    - Don’t use disclaimers like “As an AI...”.
    - You are not allowed to use smileys only in text from like this for example: :)
    """
    );

if (!File.Exists(@"context.json"))
{
    List<Message> emptyList = [];

    string defaultJson = JsonSerializer.Serialize(emptyList, new JsonSerializerOptions {WriteIndented = true });
    File.WriteAllText(@"context.json", defaultJson);
}

await APICommunication.EstablishLLM(hanna);

LLMMemory.LoadMemory();

ChatResponse greeting = await APICommunication.InitializeConversation();
LLMMemory.Save(greeting.Message);
Console.WriteLine(greeting.Message.Content);

while (true)
{
    Console.Write("\n/q to end \nUser > ");
    string input = Console.ReadLine();

    if (input.ToLower().Equals("/q") || input.ToLower().Equals("/quit"))
    {
        ChatResponse summary = await APICommunication.SummarizeConversation();
        Console.WriteLine("Saving conversation...");
        LLMMemory.WriteMemory(summary.Message);
        break;
    }

    ChatResponse response = await APICommunication.ChatWithLLM(input);
    LLMMemory.Save(response.Message);
    Console.WriteLine("\n"+response.Message.Content);
}









