﻿using System.Collections.Immutable;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Skgen;

[Generator(LanguageNames.CSharp)]
public class SkGenerator : IIncrementalGenerator
{
    private record Parameter(string name, string description, string defaultValue);

    private record Input(IList<Parameter> parameters);

    private record Config(Input input);

    private record PluginFunction(Config config, string pluginName, string functionName);

    private static string GenerateStaticExtension(PluginFunction pluginFunction)
    {
        return $@"
    public static async Task<FunctionResult> {pluginFunction.pluginName}{pluginFunction.functionName}Async(this Kernel kernel{(pluginFunction.config.input.parameters.Count > 0 ? ", " : "") + string.Join(", ", pluginFunction.config.input.parameters.Select(p => $"string {p.name} = \"{p.defaultValue}\"").ToImmutableList())}, CancellationToken cancellationToken = default(CancellationToken))
    {{
        var arguments = new KernelArguments();
        {string.Join("\n\t\t", pluginFunction.config.input.parameters.Select(p => $"arguments[\"{p.name}\"] = {p.name};"))}
    
        var answer = await kernel.InvokeAsync(""{pluginFunction.pluginName}"", ""{pluginFunction.functionName}"", arguments, cancellationToken);
        return answer;
    }}";
    }

    public void Execute(GeneratorExecutionContext context)
    {
        return;
        // Find the main method
        var mainMethod = context.Compilation.GetEntryPoint(context.CancellationToken);

        var pluginConfigFiles = context.AdditionalFiles
            .Where(s => s.Path.EndsWith("config.json")).ToImmutableList();

        var methods =
            pluginConfigFiles.Select(t => new
                {
                    text = t.GetText().ToString(),
                    PluginName = new DirectoryInfo(Path.GetDirectoryName(Path.GetDirectoryName(t.Path))).Name,
                    FunctionName = new DirectoryInfo(Path.GetDirectoryName(t.Path)).Name
                })
                .Select((text, plu) => new PluginFunction(JsonSerializer.Deserialize<Config>(text.text),
                    text.PluginName, text.FunctionName))
                .Select(i => GenerateStaticExtension(i)).ToImmutableList();

        string source = $@"// <auto-generated/>
using System;
using Microsoft.SemanticKernel;

namespace Skgen;

public static partial class KernelExtensionMethods
{{
    {methods[0]}
}}
";

        // Add the source code to the compilation
        context.AddSource($"SkSourceGeneratorKernelExtensionMethods.g.cs", source);
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        // No initialization required for this one
        Console.WriteLine(context);
    }
    
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // find all additional files that end with config.json
        IncrementalValuesProvider<AdditionalText> textFiles =
            context.AdditionalTextsProvider.Where(static file => file.Path.EndsWith("config.json"));

        // read their contents and save their name
        IncrementalValuesProvider<(string path, string content)> namesAndPaths = textFiles
            .Select((text, cancellationToken) =>
                (path: text.Path, content: text.GetText(cancellationToken)?.ToString() ?? ""));
        // generate a class that contains their values as const strings

        var collected = namesAndPaths.Collect();

        context.RegisterSourceOutput(collected, static (spc, pathAndContent) =>
        {
            spc.AddSource("Skgen.g.cs", $@"// <auto-generated/>
using System;
using Microsoft.SemanticKernel;

namespace Skgen;

public static partial class KernelExtensionMethods
{{
    {string.Join("\n", pathAndContent.Select(pathAndContent => {
        var pluginName = new DirectoryInfo(Path.GetDirectoryName(Path.GetDirectoryName(pathAndContent.path))).Name;
        var functionName = new DirectoryInfo(Path.GetDirectoryName(pathAndContent.path)).Name;
        var config = JsonSerializer.Deserialize<Config>(pathAndContent.content);
        var pluginFunction = new PluginFunction(config, pluginName, functionName);

        return GenerateStaticExtension(pluginFunction);
    }))}
}}
");
        });
    }
}