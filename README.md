# Azure Functions F# Template

This repository serves as a template for creating Azure Functions projects using F# with .NET 9 and the [isolated process model](https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-in-process-differences). It provides a basic structure and a simple "ping" function to get you started.

Azure provides [official templates](https://github.com/Azure/azure-functions-templates/tree/dev) however due to a long-standing [bug](https://github.com/Azure/azure-functions-core-tools/issues/3171) in the `func` CLI. The F# templates won't be generated. They templates repo still prodives examples of the various kinds of triggers that can be used by Azure Functions.

## Getting Started

1. **Clone this repository:**

```bash
git clone https://github.com/roboz0r/az-functions-template.git
```

2. **Copy the files into your own project**

Each project has different requirements so this repo isn't configured as a .NET template. Instead, review the code an copy in parts as you see fit.

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download)
- [Azure Functions Core Tools](https://github.com/Azure/azure-functions-core-tools)

## Project Structure

The project has the following structure:

```
.
├── src
│   ├── Functions.fsproj
│   ├── host.json
│   ├── Ping.fs
│   └── Program.fs
├── .editorconfig
├── .gitignore
├── AzFunctionsTemplate.sln
├── Directory.Build.props
├── Directory.Packages.props
├── LICENSE
└── README.md
```

### [Directory.Build.props](./Directory.Build.props)

Project settings that are applied to all projects. Here we just disable the default FSharp.Core reference as it interferes with central package management.

### [Directory.Packages.props](./Directory.Packages.props)

Configures central package management. Placing conditions like `<ItemGroup Condition="'$(IsFunctionsProject)' == 'true'">` in this file allows packages to be scoped to projects with the corresponding property `<IsFunctionsProject>true</IsFunctionsProject>`.

### [.editorconfig](./.editorconfig)

Used to configure [Fantomas](https://fsprojects.github.io/fantomas/docs/index.html).

### [host.json](./src/host.json)

The configuration file for the Azure Functions host. It allows you to configure things like logging and function timeouts.

### [local.settings.json](./src/local.settings.json)

Provides configuration to the Azure Functions runtime. As this file often contains secrets it is ignored by default. It should have the structure as below (without comments).

```jsonc
{
    "IsEncrypted": false,
    "Host": {
        "CORS": "*" // Allows requests from all origins
    },
    "Values": {
        "AzureWebJobsStorage": "",
        "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
        // Values in this section will be available to your functions as environment variables.
        "NOTE": "This file is for local development only. Do not deploy to production.",
    }
}
```

### [Ping.fs](./src/Ping.fs)

A simple HTTP-triggered function that returns "pong". This is a good starting point for creating your own functions.

### [Program.fs](./src/Program.fs)

The entry point for the application. It configures and starts the Azure Functions host.

## Running Locally

Once you have the tools installed, you can run the functions with the following command from the `src` directory:

```pwsh
cd src
# Start the local functions server on port 7022
func start --port 7022
```

This will start a local development server and you will be able to access the "ping" function at `http://localhost:7022/api/ping`.

### Manual Testing

Open <http://localhost:7022/api/ping> to see the function response.

Alternately, use your preferred tool to make requests.

```pwsh
$uri = "http://localhost:7022/api/ping"
$body = @{ message = "Hello, functions" } | ConvertTo-Json
$response = Invoke-WebRequest -Uri $uri -Method Post -Body $body -ContentType "application/json" -UseBasicParsing

Write-Host "Status Code: $($response.StatusCode)"
Write-Host "Content: $($response.Content)"
```

## Further Reading

- [Guide for running C# Azure Functions in the isolated worker model](https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide)

### Deploying to Azure

To deploy the functions to Azure, you will need to create a Function App in Azure. You can do this through the Azure Portal, the Azure CLI, or an IDE like Visual Studio Code.

Once you have created the Function App, you can deploy your code using various methods, such as:

- **Visual Studio Code:** The Azure Functions extension for Visual Studio Code provides a simple way to deploy your functions.
- **Azure CLI:** You can use the `az functionapp deployment source config-zip` command to deploy your code from a zip file.
- **GitHub Actions:** You can set up a GitHub Actions workflow to automatically deploy your code whenever you push to your repository.

For more detailed instructions on how to deploy your functions, please refer to the [Azure Functions documentation](https://docs.microsoft.com/en-us/azure/azure-functions/functions-deployment-technologies).

## Contributing

Contributions are welcome! If you have any suggestions or improvements, please feel free to open an issue or a pull request.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.
