module Login

open Feliz
open Elmish
open Feliz.Router
open Fetch.Types
open Thoth.Fetch
open Thoth.Json
open Fable.Core

[<Literal>]
let route = "Login"

type Model =
    { Username: string
      Password: string
      ErrorMessage: string }


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
    | OnRegisterSuccess
    | UsernameUpdated of string
    | PasswordUpdated of string
    | OnStorageSet

let init () =
    { Username = ""
      Password = ""
      ErrorMessage = "" },
    Cmd.none

let loginOrRegData model =
        Encode.object [
            "email", Encode.string model.Username
            "password", Encode.string model.Password
        ]

let login model =
    Cmd.OfPromise.either
            (fun () -> Fetch.post ("https://localhost:7068/login", loginOrRegData model))
            ()
            (fun response -> OnLoginSuccess response)
            OnLoginError

let register model =
    Cmd.OfPromise.either
            (fun () -> Fetch.post ("https://localhost:7068/register", loginOrRegData model))
            ()
            (fun () -> OnRegisterSuccess)
            OnRegisterError


let update (msg: Msg) (model: Model) =
    match msg with
    | RegisterClicked ->
        model,
        register model
    | LoginClicked -> 
        model,
        login model
    | OnLoginError error ->
        { model with
            ErrorMessage = error.Message },
        Cmd.none
    | OnLoginSuccess response ->
        model, Cmd.OfFunc.perform 
                (fun () -> Browser.WebStorage.localStorage.setItem ("accessToken", response.accessToken))
                ()
                (fun () -> OnStorageSet)
    | OnRegisterError error ->
        { model with
            ErrorMessage = error.Message },
        Cmd.none
    | OnRegisterSuccess -> model, Cmd.navigatePath (fullPath = "")
    | UsernameUpdated event -> { model with Username = event } , Cmd.none
    | PasswordUpdated event -> { model with Password = event } , Cmd.none
    | OnStorageSet ->
        model, Cmd.navigatePath (fullPath = "")


let view model dispatch =
    Html.div
        [ Html.h1 "Login or Register"
          Html.div [
              Html.label [ Html.text "Email"
                           Html.input [ 
                               prop.type'.text
                               prop.onChange (fun ev -> dispatch (UsernameUpdated ev))
                           ]
              ]
          ]

          Html.div [
              Html.label [ Html.text "Password"
                           Html.input [ 
                               prop.type'.password
                               prop.onChange (fun ev -> dispatch (PasswordUpdated ev))
                           ]
              ]
          ]

          Html.div [
          Html.span [ 
                Html.button [ prop.type' "button"; prop.text "Login"; prop.onClick (fun _ -> dispatch LoginClicked) ]
                Html.button [ prop.type' "button"; prop.text "Register"; prop.onClick (fun _ -> dispatch RegisterClicked) ] 
              ]
          ]
        ]