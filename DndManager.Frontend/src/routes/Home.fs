module Home

open Feliz
open Feliz.UseElmish
open Elmish
open Feliz.Router

[<Literal>]
let route = "Home"

type Model = { Username: string option }

type Msg =
    | NavigateToInitiativeTracker

let init() = { Username = None }, Cmd.none

let update (msg: Msg) (model: Model) =
    match msg with
    | NavigateToInitiativeTracker -> model, Cmd.navigatePath(InitiativeTracker.route)

let view model dispatch =
    Html.div [
        Html.h1 "Home"
        match model.Username with
        | Some userName ->
            Html.h2 $"Hello, {userName}"
            Html.button [
            prop.text "Initiative"
            prop.onClick (fun _ -> dispatch NavigateToInitiativeTracker)
            ]
        | None ->
            Html.span "Log in to get started!"
    ]