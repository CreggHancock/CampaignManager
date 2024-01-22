module Home

open Feliz
open Feliz.UseElmish
open Elmish
open Feliz.Router

[<Literal>]
let route = "Home"

type Model = { userName: string }

type Msg =
    | NavigateToInitiativeTracker

let init() = { userName = "Cregg" }, Cmd.none

let update (msg: Msg) (model: Model) =
    match msg with
    | NavigateToInitiativeTracker -> model, Cmd.navigatePath(InitiativeTracker.route)

let view model dispatch =
    Html.div [
        Html.h1 "Home"
        Html.h2 $"Hello, {model.userName}"
        Html.button [
            prop.text "Initiative"
            prop.onClick (fun _ -> dispatch NavigateToInitiativeTracker)
        ]
    ]