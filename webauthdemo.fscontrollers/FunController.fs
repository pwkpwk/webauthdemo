namespace webauthdemo.fscontrollers

open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open webauthdemo.unggoy

[<Route "api/fun">]
[<UnggoyAuthorize>]
type FunController() =
    inherit Controller()
    
    let answer (delay : int) (value : int) cancellation =
        task {
            do! Task.Delay(delay, cancellation)
            return JsonResult({| Answer = value |}) :> IActionResult
        }

    [<HttpGet "Answer">]
    [<UnggoyAction "Answer">]
    member this.AnswerAsync cancellation = answer 50 42 cancellation
    
    [<HttpGet "BetterAnswer">]
    [<UnggoyAction "BetterAnswer">] // The name is derived from the HTTP endpoint set by ASP.NET, and is 'BetterAnswer' in this case.
    member this.BetterAnswerAsync cancellation = answer 150 1 cancellation
    