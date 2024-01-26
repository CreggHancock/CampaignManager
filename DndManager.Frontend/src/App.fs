module App

open Elmish
open Feliz
open Feliz.Router

type Page =
    | Home of Home.Model
    | InitiativeTracker of InitiativeTracker.Model
    | NotFound
    | Login of Login.Model

type Model = { CurrentUrl : string list
               ActivePage : Page
               Username : string option }

type Msg =
    | UrlChanged of string list
    | HomeMsg of Home.Msg
    | InitiativeTrackerMsg of InitiativeTracker.Msg
    | LoginMsg of Login.Msg
    | LoginClicked
    | LogoutClicked

let init() = { CurrentUrl = Router.currentPath()
               ActivePage = NotFound
               Username = None }, Cmd.navigatePath(fullPath = Home.route)


let forceLoginRedirect username ifLoggedIn =
    match username with
        | Some _ ->
            ifLoggedIn
        | None ->
            let (loginModel, loginCmd) = Login.init ()
            (Page.Login loginModel, loginCmd)

let update msg model =
    match model.ActivePage, msg with
    | Page.Home homeModel, HomeMsg homeMsg ->
        let (homeModel, homeCmd) = Home.update homeMsg homeModel
        { model with ActivePage = Page.Home homeModel }, Cmd.map HomeMsg homeCmd
    | Page.InitiativeTracker initiativeTrackerModel, InitiativeTrackerMsg initiativeTrackerMsg ->
        let (initiativeTrackerModel: InitiativeTracker.Model, initiativeCmd) = InitiativeTracker.update initiativeTrackerMsg initiativeTrackerModel
        { model with ActivePage = Page.InitiativeTracker initiativeTrackerModel }, Cmd.map InitiativeTrackerMsg initiativeCmd
    | Page.Login loginModel, LoginMsg loginMsg ->
        let (loginModel, loginCmd) = Login.update loginMsg loginModel
        { model with ActivePage = Page.Login loginModel }, Cmd.map LoginMsg loginCmd
    | _, msg ->
        match msg with
        | UrlChanged segments -> 
            let pageFromUrl = 
                match segments with
                    | [ Home.route ] ->
                        let (homeModel, homeCmd) = Home.init ()
                        (Page.Home { homeModel with Username = model.Username }, homeCmd)
                    | [ InitiativeTracker.route ] ->
                        let (initiativeTrackerModel, initiativeTrackerCmd) = InitiativeTracker.init ()
                        (Page.InitiativeTracker initiativeTrackerModel, initiativeTrackerCmd) 
                            |> forceLoginRedirect model.Username
                    | [ Login.route ] ->
                        let (loginModel, loginCmd) = Login.init ()
                        (Page.Login loginModel, loginCmd)
                    | _ ->
                        (Page.NotFound, Cmd.navigatePath(fullPath = Home.route) )
            { model with CurrentUrl = segments
                         ActivePage = fst pageFromUrl }, snd pageFromUrl
        | LoginClicked ->
            (model, Cmd.navigatePath(fullPath = Login.route))
        | LogoutClicked ->
        | _ ->
            model, Cmd.none

[<ReactComponent>]
let Navbar (username : string option) dispatch =
    Html.nav [
        match username with
        | Some name ->
            Html.span [
                Html.text name
                Html.button [
                    prop.text "Logout"
                    prop.onClick (fun _ -> dispatch LoginClicked)
                ]
            ]
        | None ->
            Html.button [
            prop.text "Login"
            prop.onClick (fun _ -> dispatch LoginClicked)
        ]
    ]

let view model dispatch =
    let currentPage =
        match model.ActivePage with
        | Page.Home homeModel ->
            Home.view homeModel (HomeMsg >> dispatch)
        | Page.InitiativeTracker initiativeTrackerModel ->
            InitiativeTracker.view initiativeTrackerModel (InitiativeTrackerMsg >> dispatch)
        | Page.NotFound ->
            Html.span []
        | Page.Login loginModel ->
            Login.view loginModel (LoginMsg >> dispatch)

    
    React.router [
        router.pathMode
        router.onUrlChanged (UrlChanged >> dispatch)

        router.children [
            Html.div [
                prop.style [ style.padding 20 ]
                prop.children [
                    Html.div [
                        Navbar model.Username dispatch
                        currentPage
                    ]
                ]
            ]
        ]
    ]