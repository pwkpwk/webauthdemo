namespace webauthdemo.fscontrollers

open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc

[<Route "api/fun">]
type FunController() =
    inherit Controller()
    
    let answer cancellation =
        task {
            do! Task.Delay(50, cancellation)
            return JsonResult({| Answer = 42 |}) :> IActionResult
        }

    [<HttpGet "Answer">]    
    member this.AnswerAsync cancellation = answer cancellation