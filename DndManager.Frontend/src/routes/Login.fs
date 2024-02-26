module Login

open Feliz
open Elmish
open Feliz.Router
open Fetch.Types
open Thoth.Fetch
open Thoth.Json
open Fable.Core
open Feliz.Bulma

[<Literal>]
let route = "Login"

type Model =
    { Username: string
      Password: string
      ErrorMessage: string
      IsRegistering: bool }


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
      ErrorMessage = ""
      IsRegistering = false },
    Cmd.none

let loginOrRegData model =
    Encode.object
        [ "email", Encode.string model.Username
          "password", Encode.string model.Password ]

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
    | RegisterClicked -> model, register model
    | LoginClicked -> model, login model
    | OnLoginError error ->
        { model with
            ErrorMessage = error.Message },
        Cmd.none
    | OnLoginSuccess response ->
        model,
        Cmd.OfFunc.perform
            (fun () -> Browser.WebStorage.localStorage.setItem ("accessToken", response.accessToken))
            ()
            (fun () -> OnStorageSet)
    | OnRegisterError error ->
        { model with
            ErrorMessage = error.Message },
        Cmd.none
    | OnRegisterSuccess -> model, Cmd.navigate (fullPath = "")
    | UsernameUpdated event -> { model with Username = event }, Cmd.none
    | PasswordUpdated event -> { model with Password = event }, Cmd.none
    | OnStorageSet -> model, Cmd.navigate (fullPath = "")


let view model dispatch =
    Html.div
        [ Bulma.panel
              [ Bulma.panelHeading [ prop.text "Login" ]
                Bulma.panelBlock.div
                    [ Html.form
                          [ prop.onSubmit (fun ev -> ev.preventDefault ())
                            prop.children
                                [ Bulma.field.div
                                      [ Bulma.label "Email"
                                        Bulma.control.div
                                            [ Bulma.input.text
                                                  [ prop.required true
                                                    prop.onChange (fun ev -> dispatch (UsernameUpdated ev)) ] ] ]

                                  Bulma.field.div
                                      [ Bulma.label "Password"
                                        Bulma.control.div
                                            [ Bulma.input.password
                                                  [ prop.required true
                                                    prop.onChange (fun ev -> dispatch (PasswordUpdated ev)) ] ] ]

                                  Bulma.field.div
                                      [ Bulma.field.isGrouped
                                        Bulma.field.isGroupedCentered
                                        prop.children
                                            [ if model.IsRegistering then
                                                  Bulma.button.button
                                                      [ Bulma.color.isInfo
                                                        prop.text "Register"
                                                        prop.onClick (fun _ -> dispatch RegisterClicked) ]
                                              else
                                                  Bulma.button.button
                                                      [ Bulma.color.isInfo
                                                        prop.text "Login"
                                                        prop.onClick (fun _ -> dispatch LoginClicked) ]

                                              ] ] ] ] ] ] ]
