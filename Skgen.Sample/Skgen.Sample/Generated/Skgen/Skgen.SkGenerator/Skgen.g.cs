﻿// <auto-generated/>
using System;
using Microsoft.SemanticKernel;

namespace Skgen;

public static partial class KernelExtensionMethods
{
    
    public static async Task<FunctionResult> AssistantGetResponseAsync(this Kernel kernel, string prompt = "", string snippet0 = "", string snippet1 = "", string snippet2 = "", CancellationToken cancellationToken = default(CancellationToken))
    {
        var arguments = new KernelArguments();
        arguments["prompt"] = prompt;
		arguments["snippet0"] = snippet0;
		arguments["snippet1"] = snippet1;
		arguments["snippet2"] = snippet2;
    
        var answer = await kernel.InvokeAsync("Assistant", "GetResponse", arguments, cancellationToken);
        return answer;
    }

    public static async Task<FunctionResult> SummarizeSummarizeWithContextAsync(this Kernel kernel, string input = "", string fact1 = "", string fact2 = "", string fact3 = "", CancellationToken cancellationToken = default(CancellationToken))
    {
        var arguments = new KernelArguments();
        arguments["input"] = input;
		arguments["fact1"] = fact1;
		arguments["fact2"] = fact2;
		arguments["fact3"] = fact3;
    
        var answer = await kernel.InvokeAsync("Summarize", "SummarizeWithContext", arguments, cancellationToken);
        return answer;
    }
}
