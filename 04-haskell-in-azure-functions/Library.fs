module functions.SampleHttpTrigger

open Microsoft.Azure.WebJobs
open Microsoft.Azure.WebJobs.Host
open Microsoft.Azure.WebJobs.Extensions.Http
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Mvc

open System
open System.IO
open System.IO.Pipes
open System.Diagnostics

let socketPathFromName (socketName: string): string =
    let tmpdir = Environment.GetEnvironmentVariable("TMPDIR")
    Path.Combine(tmpdir, sprintf "CoreFxPipe_%s" socketName)

[<CLIMutable>]
type Person = {
    name: string
}

let haskellRun (name: string) =
    printfn "Creating a named pipe"
        
    let inputSocketName = "input" + (Guid.NewGuid().ToString())
    let outputSocketName = "output" + (Guid.NewGuid().ToString())
    
    use inputServer = new NamedPipeServerStream(inputSocketName)
    let inputPath = socketPathFromName inputSocketName
    
    use outputServer = new NamedPipeServerStream(outputSocketName)
    let outputPath = socketPathFromName outputSocketName
    
    let cwd = Directory.GetCurrentDirectory()
    printfn "cwd is %s" cwd
    

    let filename = Path.Combine(cwd, "bin/baz")
    
    printfn "Input Socket Path %s" inputPath
    printfn "Output Socket Path %s" outputPath
    printfn "Executable file name %s" filename
    
    let procStartInfo =
        ProcessStartInfo(
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            FileName = filename
        )
        
    procStartInfo.EnvironmentVariables.Add("AZ_INPUT", inputPath)
    procStartInfo.EnvironmentVariables.Add("AZ_OUTPUT", outputPath)
    
    use p = new Process(StartInfo = procStartInfo)
    
    
    let outputHandler f (_sender:obj) (args:DataReceivedEventArgs) = f args.Data
    
    let stdoutHandler =
        fun x ->
            sprintf "STDOUT: %s" x
            |> Console.WriteLine
        |> outputHandler
    
    let stderrHandler =
        fun x ->
            sprintf "STDERR: %s" x
            |> Console.Error.WriteLine
        |> outputHandler    
    
    p.OutputDataReceived.AddHandler(DataReceivedEventHandler stdoutHandler)
    p.ErrorDataReceived.AddHandler(DataReceivedEventHandler stderrHandler)
    
    // The async workflow that will handle the communication for the input    
    let inputWorkflow =
        async {
            printfn "Waiting for a client to connect to input socket"
            do! inputServer.WaitForConnectionAsync() |> Async.AwaitTask
            printfn "Client connected to input socket"
            printfn "Sending data"
            use writer = new StreamWriter(inputServer)
            do! writer.WriteLineAsync(name) |> Async.AwaitTask
            do! writer.FlushAsync() |> Async.AwaitTask
        }
    
    // The async workflow that will handle the communication for the output    
    let outputWorkflow =
        async {
            printfn "Waiting for a client to connect to output socket"
            do! outputServer.WaitForConnectionAsync() |> Async.AwaitTask
            printfn "Client connected to output socket"
            printfn "Receiving data"
            use reader = new StreamReader(outputServer)
            return! reader.ReadToEndAsync() |> Async.AwaitTask
        }
    
    let processWorkflow =
        async {
            printfn "Starting the client"
            // Hack to ensure that the socket will be created....
            do! Async.Sleep(100)
            let started = p.Start()
            if not started then failwith "Failed to start"
            
            p.BeginErrorReadLine()
            p.BeginOutputReadLine()
            
            p.WaitForExit()      
        }
        

    let compositeWorkflow =
        async {
            // Start all the workflows at once
            let! input = Async.StartChild inputWorkflow
            let! output = Async.StartChild outputWorkflow
            let! proc = Async.StartChild processWorkflow
            
            // Wait for input and proc
            do! input
            do! proc
            // Return the result of output
            return! output       
        }

    let result = Async.RunSynchronously compositeWorkflow 
        
    printfn "Received %s" result
    printfn "Closing down"

    result


[<FunctionName("SampleHttpTrigger")>]
let run ([<HttpTrigger(AuthorizationLevel.Anonymous)>]person: Person, log: TraceWriter)=
   haskellRun person.name
