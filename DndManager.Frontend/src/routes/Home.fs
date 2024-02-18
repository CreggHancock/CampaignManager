module Home

open Feliz
open Feliz.UseElmish
open Elmish
open Feliz.Router
open Fable.Core
open Thoth.Fetch
open Fetch
open Thoth.Json

[<Literal>]
let route = "Home"

type Character = {
        id: int
        name: string
        description: string
        level: int
    }

type Scene = {
        id: int
        name: string
        description: string
    }

type HomeViewModel = { 
      userCharacters : Character List
      userScenes : Scene List
    }


type Model = { homeViewModel: HomeViewModel; ErrorMessage: string }

type Msg =
    | NavigateToInitiativeTracker
    | NavigateToInitiativeTrackerWithId of int
    | NavigateToInitiativeCharacter
    | OnGetModelSuccess of HomeViewModel
    | OnGetModelError of exn
    | OnGotModelFromStorageSuccess of string
    | OnGotModelFromStorageError of exn

let getHomeViewModel accessToken =
    match accessToken with
        | Some token ->
            let 
                headers = [ HttpRequestHeaders.Authorization ("Bearer " + token) ]
            Cmd.OfPromise.either
                    (fun () -> Fetch.get ("https://localhost:7068/Home/Get/", headers = headers))
                    ()
                    (fun (response) -> OnGetModelSuccess response)
                    OnGetModelError
        | None ->
            (Cmd.OfFunc.either 
                (fun () -> Browser.WebStorage.localStorage.getItem ("scenes"))
                ()
                (fun (sceneInfo) -> OnGotModelFromStorageSuccess sceneInfo))
                OnGotModelFromStorageError

let init accessToken = 
           { homeViewModel = { userCharacters = []; userScenes = [] }; ErrorMessage = "" },
           getHomeViewModel accessToken

let update (msg: Msg) (model: Model) =
    match msg with
    | NavigateToInitiativeTracker -> model, Cmd.navigatePath(InitiativeTracker.route)
    | OnGetModelSuccess response -> { model with homeViewModel = response }, Cmd.none
    | OnGetModelError error -> { model with ErrorMessage = error.Message }, Cmd.none
    | NavigateToInitiativeCharacter -> model, Cmd.navigatePath(InitiativeTracker.route)
    | NavigateToInitiativeTrackerWithId id -> model, Cmd.navigatePath(InitiativeTracker.route + "?sceneId=" + id.ToString() )
    | OnGotModelFromStorageSuccess scenesString -> 
        let sceneResults = Decode.Auto.fromString<Scene List>(scenesString)
        match sceneResults with
            | Ok scenes ->
                { model with homeViewModel = { model.homeViewModel with userScenes = scenes }}, Cmd.none
            | Error err ->
                { model with ErrorMessage = err }, Cmd.none
    | OnGotModelFromStorageError error -> { model with ErrorMessage = error.Message }, Cmd.none

let viewCharacters (characters : Character List): Fable.React.ReactElement =
    Html.div (Html.div [ Html.h3 "Characters" ] :: List.map (fun (character : Character) -> Html.div [ Html.text character.name ]) characters)

let viewScenes (scenes : Scene List) dispatch: Fable.React.ReactElement =
    Html.div (Html.div [ Html.h3 "Scenes" ] :: List.map (fun (scene : Scene) -> Html.button [ prop.text (if scene.name = "" then "Scene " + scene.id.ToString() else scene.name); prop.onClick (fun _ -> dispatch <| NavigateToInitiativeTrackerWithId scene.id) ]) scenes)


let view model userName dispatch =
    Html.div [
        match userName with
        | Some name ->
            Html.h2 $"Hello, {name}"
        | None ->
            Html.span ""
        (viewCharacters model.homeViewModel.userCharacters)
        Html.button [
        prop.text "New Character"
        prop.onClick (fun _ -> dispatch NavigateToInitiativeCharacter)
        ]
        (viewScenes model.homeViewModel.userScenes dispatch)
        Html.button [
        prop.text "New Scene"
        prop.onClick (fun _ -> dispatch NavigateToInitiativeTracker)
        ]
    ]