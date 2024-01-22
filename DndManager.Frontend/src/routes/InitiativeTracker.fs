module InitiativeTracker

open Feliz
open Feliz.UseElmish
open Elmish

[<Literal>]
let route = "InitiativeTracker"

type Model = { Count: int }

type Msg =
    | Increment
    | Decrement

let init() = { Count = 0 }, Cmd.none

let update (msg: Msg) (model: Model) =
    match msg with
    | Increment -> { model with Count = model.Count + 1 }, Cmd.none
    | Decrement -> { model with Count = model.Count - 1 }, Cmd.none

let view model dispatch =
    Html.div [
        Html.h1 model.Count
        Html.button [
            prop.text "Increment"
            prop.onClick (fun _ -> dispatch Increment)
        ]

        Html.button [
            prop.text "Decrement"
            prop.onClick (fun _ -> dispatch Decrement)
        ]
    ]