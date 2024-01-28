# What this package does
This is a source generator which finds [Semantic Kernel](https://github.com/microsoft/semantic-kernel) plugins and generates helper static extension methods for calling them with all the right inputs.

### Semantic Kernel plugins
Semantic Kernel plugins are stringly-typed, and it's easy to forget a variable or two when calling them:

A typical Semantic Kernel project defines its prompt-based plugins in a folder called Plugins:
```
.
├── Plugins/
│   ├── Summarize/
│   │   └── SummarizeText/
│   │       ├── config.json
│   │       └── skprompt.txt
│   └── Assistant/
│       └── GetResponse/
│           ├── config.json
│           └── skprompt.txt
└── Program.cs
```

A typical config.json file looks like this:
```json
{
  "schema": 1,
  "type": "completion",
  "description": "Gives helpful responses using snippets of relevant information",
  "completion": {
    "max_tokens": 500,
    "temperature": 0.5,
    "top_p": 0.0,
    "presence_penalty": 0.0,
    "frequency_penalty": 0.0
  },
  "input": {
    "parameters": [
      {
        "name": "prompt",
        "description": "The user's prompt",
        "defaultValue": ""
      },
      {
        "name": "snippet0",
        "description": "The most relevant snippet found from documents",
        "defaultValue": ""
      },
      {
        "name": "snippet1",
        "description": "The second most relevant snippet found from documents",
        "defaultValue": ""
      },
      {
        "name": "snippet2",
        "description": "The third most relevant snippet found from documents",
        "defaultValue": ""
      }
    ]
  }
}
```

This source generator goes through and finds every file called `config.json` and generates static extension methods for Microsoft.SemanticKernel.Kernel that look like this:
```csharp
// <auto-generated/>
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

```

# Getting started
### Install the package 
`dotnet add package Skgen`

### Adjust your package's csproj
By default, the source generated code is not persisted to disk. In order to persist it to a file called Generated, you can add this to your csproj:

```xml
<PropertyGroup>
        <EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>
        <CompilerGeneratedFilesOutputPath>Generated</CompilerGeneratedFilesOutputPath>
</PropertyGroup>

<ItemGroup>
    <Compile Remove="$(CompilerGeneratedFilesOutputPath)/**/*.cs" />
</ItemGroup>
```
Thank you to Andrew Lock for [describing this solution!](https://andrewlock.net/creating-a-source-generator-part-6-saving-source-generator-output-in-source-control/)

### Reasoning
The point of <Compile Remove="$(CompilerGeneratedFilesOutputPath)/**/*.cs" /> is to avoid compiling the source generated files after they have been spit out to disk and hence double compiling the same thing and hitting errors like this `Error CS0111: Type 'KernelExtensionMethods' already defines a member called '' with the same parameters.

![Error CS0111: Type 'KernelExtensionMethods' already defines a member called '' with the same parameters](./readme/image.png)

## Drawbacks
Despite my best effort, I haven't gotten intellisense in Rider, Visual Studio or Visual Studio code to respect these new extension methods without removing <Compile Remove="$(CompilerGeneratedFilesOutputPath)/**/*.cs" /> and hence breaking compilation when you build the second time😔. If you figure this out, please let me know!

![Intellisense shows red squiggles on the new methods even though they work](./readme/brokenintellisense.pngimage.png)


