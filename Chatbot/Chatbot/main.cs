using Chatbot.Resources;
using System.Text;
using System.Text.Json;

LLMCreationTemplate hanna = new LLMCreationTemplate
    (
    "Hanna",
    "gemma3:latest",
    $"""
    You are {APICommunication.LLMName}, a friendly, empathetic, and highly intelligent female assistant.

    Personality Traits:
    - Friendly and approachable
    - Professional but conversational
    - Uses plain language
    - Makes light jokes occasionally

    Rules:
    - Never refer to yourself as an AI language model.
    - Always refer to yourself as “{APICommunication.LLMName}.”
    - Answer clearly, even if the question is vague. Ask clarifying questions if needed.
    - Don’t use disclaimers like “As an AI...”.
    - You must not make up informations, use the context provided to you.
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
Console.OutputEncoding = Encoding.UTF8;
Console.Title = "hanna";
Console.WriteLine(greeting.Message.Content);

while (true)
{
    Console.Write("\n/q to end \nUser > ");
    string input = Console.ReadLine();

    if (input.ToLower().Equals("/q") || input.ToLower().Equals("/quit"))
    {
        Console.WriteLine("Saving conversation...");
        ChatResponse summary = await APICommunication.SummarizeConversation();
        LLMMemory.WriteMemory(summary.Message);
        Console.WriteLine("\nConversation saved successfully.\n");
        break;
    }

    if (input.Equals("/list"))
    {
        foreach (Message m in LLMMemory.Messages)
        {
            Console.WriteLine($"{m.Role} - {m.Content}");
        }
    }
    else
    {
        ChatResponse response = await APICommunication.ChatWithLLM(input);
        LLMMemory.Save(response.Message);
        Console.WriteLine("\n" + response.Message.Content);
    }
}









