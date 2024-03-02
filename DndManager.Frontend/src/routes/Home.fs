module Home

open Feliz
open Elmish
open Feliz.Router
open Thoth.Fetch
open Fetch
open Thoth.Json

[<Literal>]
let route = "Home"

type Character =
    { Id: int
      Name: string
      Description: string
      Level: int }

type Scene =
    { Id: int
      Name: string
      Description: string }

type HomeViewModel =
    { UserCharacters: Character List
      UserScenes: Scene List }


type Model =
    { HomeViewModel: HomeViewModel
      ErrorMessage: string }

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
        let headers = [ Authorization("Bearer " + token) ]

        Cmd.OfPromise.either
            (fun () -> Fetch.get ("https://localhost:7068/Home/Get/", headers = headers))
            ()
            (fun (response) -> OnGetModelSuccess response)
            OnGetModelError
    | None ->
        (Cmd.OfFunc.either (fun () -> Browser.WebStorage.localStorage.getItem ("scenes")) () (fun (sceneInfo) ->
            OnGotModelFromStorageSuccess sceneInfo))
            OnGotModelFromStorageError

let init accessToken =
    { HomeViewModel = { UserCharacters = []; UserScenes = [] }
      ErrorMessage = "" },
    getHomeViewModel accessToken

let update (msg: Msg) (model: Model) =
    match msg with
    | NavigateToInitiativeTracker -> model, Cmd.navigate (InitiativeTracker.route)
    | OnGetModelSuccess response -> { model with HomeViewModel = response }, Cmd.none
    | OnGetModelError error ->
        { model with
            ErrorMessage = error.Message },
        Cmd.none
    | NavigateToInitiativeCharacter -> model, Cmd.navigate (InitiativeTracker.route)
    | NavigateToInitiativeTrackerWithId id ->
        model, Cmd.navigate (InitiativeTracker.route + "?sceneId=" + id.ToString())
    | OnGotModelFromStorageSuccess scenesString ->
        let sceneResults = Decode.Auto.fromString<Scene List> (scenesString)

        match sceneResults with
        | Ok scenes ->
            { model with
                HomeViewModel =
                    { model.HomeViewModel with
                        UserScenes = scenes } },
            Cmd.none
        | Error err -> { model with ErrorMessage = err }, Cmd.none
    | OnGotModelFromStorageError error ->
        { model with
            ErrorMessage = error.Message },
        Cmd.none

let viewCharacters (characters: Character List) : Fable.React.ReactElement =
    Html.div (
        Html.div [ Html.h3 "Characters" ]
        :: List.map (fun (character: Character) -> Html.div [ Html.text character.Name ]) characters
    )

let viewScenes (scenes: Scene List) dispatch : Fable.React.ReactElement =
    Html.div (
        Html.div [ Html.h3 "Scenes" ]
        :: List.map
            (fun (scene: Scene) ->
                Html.button
                    [ prop.text (
                          if scene.Name = "" then
                              "Scene " + scene.Id.ToString()
                          else
                              scene.Name
                      )
                      prop.onClick (fun _ -> dispatch <| NavigateToInitiativeTrackerWithId scene.Id) ])
            scenes
    )


let view model userName dispatch =
    Html.div
        [ match userName with
          | Some name -> Html.h2 $"Hello, {name}"
          | None -> Html.span ""
          (viewCharacters model.HomeViewModel.UserCharacters)
          Html.button
              [ prop.text "New Character"
                prop.onClick (fun _ -> dispatch NavigateToInitiativeCharacter) ]
          (viewScenes model.HomeViewModel.UserScenes dispatch)
          Html.button
              [ prop.text "New Scene"
                prop.onClick (fun _ -> dispatch NavigateToInitiativeTracker) ] ]
