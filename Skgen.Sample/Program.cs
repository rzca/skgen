using Microsoft.SemanticKernel;
namespace Skgen.Sample;

class Program
{
    static async Task Main(string[] args)
    {
        var builder = Kernel.CreateBuilder();
        builder.Services.AddOpenAIChatCompletion(
            modelId: "gpt-4-1106-preview",  // The model ID of your Azure OpenAI service
            apiKey: "<recommend you use an environment variable for this!>"           // The API key of your Azure OpenAI service
        );

        var pluginsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Plugins");
        builder.Plugins.AddFromPromptDirectory("./plugins/Assistant");
        builder.Plugins.AddFromPromptDirectory("./plugins/Summarize");

        var kernel = builder.Build();

        var result = await kernel.AssistantGetResponseAsync(
            prompt: "What do you recommend doing for vacation?", 
            snippet0: "My favorite place in the world is Sardinia and I haven't been in ages.", 
            cancellationToken: CancellationToken.None);

        Console.WriteLine(result.ToString());
    }
}