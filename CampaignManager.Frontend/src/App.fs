module App

open Elmish
open Feliz
open Feliz.Router
open Thoth.Fetch
open Fetch.Types
open Feliz.Bulma

type UserInfo =
    { email: string
      isEmailConfirmed: bool }

type Page =
    | Home of Home.Model
    | InitiativeTracker of InitiativeTracker.Model
    | NotFound
    | Login of Login.Model

type Model =
    { CurrentUrl: string list
      ActivePage: Page
      UserInfo: UserInfo option
      ErrorMessage: string }


type Msg =
    | UrlChanged of string list
    | HomeMsg of Home.Msg
    | InitiativeTrackerMsg of InitiativeTracker.Msg
    | LoginMsg of Login.Msg
    | LoginClicked
    | LogoutClicked
    | OnLogoutSuccess
    | OnLogoutError of exn
    | OnGotUserSuccess of UserInfo
    | OnGotUserError of exn
    | OnStorageCleared

let accessToken =
    if Browser.WebStorage.localStorage.length > 0 then
        Some(Browser.WebStorage.localStorage.getItem "accessToken")
    else
        None

let getUserInfo accessToken =
    match accessToken with
    | Some token ->
        Cmd.OfPromise.either
            (fun () -> Fetch.get ("https://localhost:7068/manage/info", headers = [ Authorization("Bearer " + token) ]))
            ()
            (fun (userInfo) -> OnGotUserSuccess userInfo)
            OnGotUserError
    | None -> Cmd.none

let getUserName model =
    (Option.map (fun (info) -> info.email) model.UserInfo)

let init () =
    { CurrentUrl = Router.currentPath ()
      ActivePage = NotFound
      UserInfo = None
      ErrorMessage = "" },
    getUserInfo accessToken


let update msg model =
    match model.ActivePage, msg with
    | Home homeModel, HomeMsg homeMsg ->
        let (homeModel, homeCmd) = Home.update homeMsg homeModel

        { model with
            ActivePage = Home homeModel },
        Cmd.map HomeMsg homeCmd
    | InitiativeTracker initiativeTrackerModel, InitiativeTrackerMsg initiativeTrackerMsg ->
        let (initiativeTrackerModel: InitiativeTracker.Model, initiativeCmd) =
            InitiativeTracker.update initiativeTrackerMsg initiativeTrackerModel

        { model with
            ActivePage = InitiativeTracker initiativeTrackerModel },
        Cmd.map InitiativeTrackerMsg initiativeCmd
    | Login loginModel, LoginMsg loginMsg ->
        let (loginModel, loginCmd) = Login.update loginMsg loginModel

        { model with
            ActivePage = Login loginModel },
        Cmd.map LoginMsg loginCmd
    | _, msg ->
        match msg with
        | UrlChanged segments ->
            let preNavigateCmds =
                match model.ActivePage with
                | Login loginModel -> getUserInfo accessToken
                | _ -> Cmd.none

            let pageFromUrl =
                match segments with
                | [ Home.route ] ->
                    let (homeModel, homeCmd) = Home.init accessToken
                    (Home homeModel, Cmd.map (fun (cmd) -> (HomeMsg) cmd) homeCmd)
                | [ InitiativeTracker.route ] ->
                    let (initiativeTrackerModel, initiativeTrackerCmd) =
                        InitiativeTracker.init accessToken None

                    (InitiativeTracker initiativeTrackerModel,
                     Cmd.map (fun (cmd) -> (InitiativeTrackerMsg) cmd) initiativeTrackerCmd)
                | [ InitiativeTracker.route; Route.Query [ "sceneId", Route.Int sceneId ] ] ->
                    let (initiativeTrackerModel, initiativeTrackerCmd) =
                        InitiativeTracker.init accessToken (Option.Some sceneId)

                    (InitiativeTracker initiativeTrackerModel,
                     Cmd.map (fun (cmd) -> (InitiativeTrackerMsg) cmd) initiativeTrackerCmd)
                | [ Login.route ] ->
                    let (loginModel, loginCmd) = Login.init ()
                    (Login loginModel, loginCmd)
                | _ -> (NotFound, Cmd.navigate (fullPath = Home.route))

            { model with
                CurrentUrl = segments
                ActivePage = fst pageFromUrl },
            Cmd.batch [ preNavigateCmds; snd pageFromUrl ]
        | LoginClicked -> (model, Cmd.navigate (fullPath = Login.route))
        | LogoutClicked ->
            ({ model with UserInfo = None },
             Cmd.OfFunc.perform (fun () -> Browser.WebStorage.localStorage.removeItem ("accessToken")) () (fun () ->
                 OnLogoutSuccess))
        | OnLogoutSuccess -> (model, Cmd.navigate (fullPath = Login.route))
        | OnLogoutError error ->
            { model with
                ErrorMessage = error.Message },
            Cmd.none
        | OnGotUserSuccess userInfo -> { model with UserInfo = Some userInfo }, Cmd.none
        | OnGotUserError error ->
            { model with
                ErrorMessage = error.Message },
            Cmd.none
        | _ -> model, Cmd.none

[<ReactComponent>]
let Navbar (username: string option) dispatch =
    Bulma.navbar
        [ color.isBlack
          prop.children
              [ Bulma.navbarBrand.div [ Bulma.navbarItem.a [ Html.i [ prop.className "fa-solid fa-dice-d20" ] ] ]
                Bulma.navbarMenu
                    [ Bulma.navbarStart.div [ Bulma.navbarItem.a [ prop.text "Home"; prop.href "/#/Home" ] ]
                      Bulma.navbarEnd.div
                          [ Bulma.navbarItem.div
                                [ match username with
                                  | Some name ->
                                      Html.span
                                          [ Html.text name
                                            Bulma.button.a
                                                [ prop.style [ style.visibility.hidden ]
                                                  prop.text "Logout"
                                                  prop.onClick (fun _ -> dispatch LogoutClicked) ] ]
                                  | None ->
                                      Bulma.button.a
                                          [ prop.style [ style.visibility.hidden ]
                                            prop.text "Login"
                                            prop.onClick (fun _ -> dispatch LoginClicked) ] ] ] ]

                ] ]

let view model dispatch =
    let currentPage =
        match model.ActivePage with
        | Home homeModel -> Home.view homeModel (getUserName model) (HomeMsg >> dispatch)
        | InitiativeTracker initiativeTrackerModel ->
            InitiativeTracker.view initiativeTrackerModel (InitiativeTrackerMsg >> dispatch)
        | NotFound -> Html.span []
        | Login loginModel -> Login.view loginModel (LoginMsg >> dispatch)


    React.router
        [ router.onUrlChanged (UrlChanged >> dispatch)

          router.children
              [ Html.div
                    [ prop.style [ style.padding 20 ]
                      prop.children [ Html.div [ Navbar (getUserName model) dispatch; currentPage ] ] ] ] ]
