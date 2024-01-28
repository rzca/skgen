// See https://aka.ms/new-console-template for more information

using Microsoft.SemanticKernel;

namespace Skgen.Sample;

partial class Program
{
    static async Task Main(string[] args)
    {
        HelloFrom("Generated Code");
        
        var kernel = Kernel.CreateBuilder().Build();

        var result = await kernel.SummarizePlugin("this is input", "this is how many chunks", "total chunks", "fact 1", "fact 2", "fact 2");
    }
    
    static partial void HelloFrom(string name);
}