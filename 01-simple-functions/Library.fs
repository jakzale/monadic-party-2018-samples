module functions.SampleHttpTrigger

open Microsoft.Azure.WebJobs
open Microsoft.Azure.WebJobs.Host
open Microsoft.Azure.WebJobs.Extensions.Http
open Microsoft.AspNetCore.Mvc

open System.Diagnostics
open Microsoft.WindowsAzure.Storage.Queue
open System.Net.Http
open System.Diagnostics.Tracing

[<CLIMutable>]
type Person = {
    name: string
    age: int
}

[<FunctionName("SampleHttpTrigger")>]
let run ([<HttpTrigger(AuthorizationLevel.Anonymous)>]person: Person, log: TraceWriter) =
    log.Info("Processing a HTTP request")

    
    sprintf "Hello, %s aged %d!" (person.name) (person.age)
    |> OkObjectResult

[<FunctionName("SampleQueueProducer")>]
let foo ([<HttpTrigger(AuthorizationLevel.Anonymous)>] person: Person,
         log: TraceWriter)
        : [<return: Queue("sample-queue")>] _ =
    
    let message = sprintf "Received a message about %s" person.name

    log.Info(message)
    
    person

[<FunctionName("SampleQueueConsumer")>]
let bar ([<QueueTrigger("sample-queue")>] person: Person, log: TraceWriter) : unit =
    let message = sprintf "Received a message about %s" person.name
    log.Info(message)
    
    

