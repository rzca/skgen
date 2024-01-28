// See https://aka.ms/new-console-template for more information

using Microsoft.SemanticKernel;
using Skgen;
namespace Skgen.Sample;

partial class Program
{
    static async Task Main(string[] args)
    {
        HelloFrom("Generated Code");
        
        var pluginsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Plugins");
        var kernel = Kernel.CreateBuilder().Build();
        var summarizePlugin = kernel.ImportPluginFromPromptDirectory(Path.Combine(pluginsDirectory, "SummarizePlugin"));
        
        var result = await kernel.SummarizePluginSummarizeAsync("this is input", "this is how many chunks", "total chunks", "fact 1", "fact 2", "fact 2");
    }
    
    static partial void HelloFrom(string name);
}