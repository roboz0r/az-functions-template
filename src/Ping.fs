namespace Functions
// An enclosing `namespace` is required for the Azure Functions runtime to discover the function class.
// If it is a `module`, the functions will not be discovered.

open System
open System.Threading.Tasks
open System.Net

open Microsoft.Azure.Functions.Worker
open Microsoft.Azure.Functions.Worker.Http
open Microsoft.Extensions.Logging

type Ping = { Message: string }

type Pong =
    {
        Message: string option
        Timestamp: DateTimeOffset
    }

// Constructor parameters are injected by the Azure Functions runtime.
type PingFunctions(logger: ILogger<PingFunctions>, timeProvider: TimeProvider) =


    [<Function("ping")>]
    member _.Ping
        (
            [<HttpTrigger(AuthorizationLevel.Anonymous,
                          "get",
                          "post",
                          Route = "ping")>] req: HttpRequestData,
            ctx: FunctionContext
        ) : Task<HttpResponseData> =
        task {
            let! message =
                task {
                    try
                        match req.Method with
                        | "GET" -> return None
                        | _ ->
                            let! message = req.ReadFromJsonAsync<Ping>()
                            return Option.ofObj message
                    with ex ->
                        logger.LogError(ex, "Error deserializing ping message")
                        return None
                }

            logger.LogInformation("Received ping message: {Message}", message)

            match message with
            | None ->
                let resp = req.CreateResponse HttpStatusCode.OK

                do!
                    resp.WriteAsJsonAsync
                        {
                            Message = None
                            Timestamp = timeProvider.GetUtcNow()
                        }

                return resp

            | Some message ->
                let resp = req.CreateResponse HttpStatusCode.OK

                do!
                    resp.WriteAsJsonAsync
                        {
                            Message = Some message.Message
                            Timestamp = timeProvider.GetUtcNow()
                        }

                return resp
        }
