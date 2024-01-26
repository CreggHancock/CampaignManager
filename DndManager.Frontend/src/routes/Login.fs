module Login

open Feliz
open Elmish
open Feliz.Router

[<Literal>]
let route = "Login"

type Model =
    { Username: string
      Password: string
      ErrorMessage: string }


type LoginResponse =
    { TokenType: string
      AccessToken: string
      ExpiresIn: int
      RefreshToken: string }

type Msg =
    | RegisterClicked
    | LoginClicked
    | OnLoginError of exn
    | OnLoginSuccess of LoginResponse
    | OnRegisterError of exn
    | OnRegisterSuccess of LoginResponse
    | UsernameUpdated of string
    | PasswordUpdated of string

let init () =
    { Username = ""
      Password = ""
      ErrorMessage = "" },
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
            ErrorMessage = error.Message },
        Cmd.none
    | OnLoginSuccess value -> model, Cmd.navigatePath (fullPath = "")
    | OnRegisterError error ->
        { model with
            ErrorMessage = error.Message },
        Cmd.none
    | OnRegisterSuccess value -> model, Cmd.navigatePath (fullPath = "")
    | UsernameUpdated event -> { model with Username = event } , Cmd.none
    | PasswordUpdated event -> { model with Password = event } , Cmd.none


let view model dispatch =
    Html.div
        [ Html.h1 "Login"
          Html.input [ prop.text "Username"
                       prop.type'.text
                       prop.onChange (fun ev -> dispatch (UsernameUpdated ev))
                     ]

          Html.input [ prop.text "Password"
                       prop.type'.password
                       prop.onChange (fun ev -> dispatch (PasswordUpdated ev))]

          Html.span
              [ Html.button [ prop.text "Login"; prop.onClick (fun _ -> dispatch LoginClicked) ]
                Html.button [ prop.text "Register"; prop.onClick (fun _ -> dispatch RegisterClicked) ] ] ]
