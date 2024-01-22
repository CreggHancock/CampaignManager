module Login

open Feliz
open Elmish
open Feliz.Router

[<Literal>]
let route = "Login"

type Model =
    { userName: string
      password: string
      errorMessage: string }


type LoginResponse =
    { tokenType: string
      accessToken: string
      expiresIn: int
      refreshToken: string }

type Msg =
    | RegisterClicked
    | LoginClicked
    | OnLoginError of exn
    | OnLoginSuccess of LoginResponse
    | OnRegisterError of exn
    | OnRegisterSuccess of LoginResponse

let init () =
    { userName = ""
      password = ""
      errorMessage = "" },
    Cmd.none

let update (msg: Msg) (model: Model) =
    match msg with
    | RegisterClicked ->
        model,
        Cmd.OfPromise.either
            (fun () -> Request.fetchAs<LoginResponse> "https://localhost:7068/register" [])
            ()
            OnRegisterSuccess
            OnRegisterError
    | LoginClicked -> 
        model,
        Cmd.OfPromise.either
            (fun () -> Request.fetchAs<LoginResponse> "https://localhost:7068/login" [])
            ()
            OnLoginSuccess
            OnLoginError
    | OnLoginError error ->
        { model with
            errorMessage = error.Message },
        Cmd.none
    | OnLoginSuccess value -> model, Cmd.navigatePath ("")
    | OnRegisterError error ->
        { model with
            errorMessage = error.Message },
        Cmd.none
    | OnRegisterSuccess value -> model, Cmd.navigatePath ("")


let view model dispatch =
    Html.div
        [ Html.h1 "Login"

          Html.span
              [ Html.button [ prop.text "Login"; prop.onClick (fun _ -> dispatch LoginClicked) ]
                Html.button [ prop.text "Register"; prop.onClick (fun _ -> dispatch RegisterClicked) ] ] ]
