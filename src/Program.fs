open Microsoft.Azure.Functions.Worker
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging

module Program =
    open System
    open System.Text.Json
    open System.Text.Json.Serialization

    let serializerOptions =
        // How to use FSharp.SystemTextJson
        // https://github.com/Tarmil/FSharp.SystemTextJson/blob/master/docs/Using.md
        let o = JsonFSharpOptions.Default().ToJsonSerializerOptions()

        o.PropertyNameCaseInsensitive <- true
        o.PropertyNamingPolicy <- JsonNamingPolicy.CamelCase

        o


    [<EntryPoint>]
    let main argv =
        // Register the custom JSON serializer
        // This is necessary to handle F# records and discriminated unions correctly
        // in methods like `req.ReadFromJsonAsync<Ping>()`.
        let jsonSerializer = Azure.Core.Serialization.JsonObjectSerializer(serializerOptions)


        let host =
            HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices(fun services ->
                    // Register the custom JSON serializer and other services
                    // These services will be available for dependency injection in the function class.
                    services
                        .AddSingleton<TimeProvider>(TimeProvider.System)
                        .AddSingleton<JsonSerializerOptions>(serializerOptions)
                        .AddSingleton<Azure.Core.Serialization.ObjectSerializer>(jsonSerializer)
                        .AddSingleton<ILoggerFactory>(
                            LoggerFactory.Create(fun builder ->
                                builder.AddConsole() |> ignore
                            )
                        )
                    |> ignore
                )
                .Build()

        host.Run()
        0
