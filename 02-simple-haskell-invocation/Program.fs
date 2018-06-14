// Learn more about F# at http://fsharp.org

open System
open System.IO
open System.Diagnostics

[<EntryPoint>]
let main argv =
    let cwd = Directory.GetCurrentDirectory()

    printfn "CWD is %s" cwd

    let haskellExecutable = Path.Combine(cwd, "bin/foo")
    
    printfn "Running haskell executable at %s" haskellExecutable
    
    let processInfo =
        ProcessStartInfo (
            RedirectStandardOutput = true,
            UseShellExecute = false,
            FileName = haskellExecutable
        )
        
    use haskell = new Process(StartInfo = processInfo)
    
    haskell.Start() |> ignore
    let output = haskell.StandardOutput.ReadToEnd()
    haskell.WaitForExit()
    
    printf "%s" output
     
    0 // return an integer exit code
