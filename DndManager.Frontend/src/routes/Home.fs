module Home

open Feliz
open Elmish
open Feliz.Router
open Feliz.Bulma
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
      BackgroundImage: string
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
    Html.div
        [ prop.className "columns is-4 is-variable is-multiline"
          prop.children (
              List.map
                  (fun (scene: Scene) ->
                      Html.div
                          [ prop.classes [ "column"; "is-one-quarter" ]
                            prop.children
                                [ Bulma.card
                                      [ prop.children
                                            [ Bulma.cardImage
                                                  [ Bulma.image
                                                        [ image.is4by3
                                                          prop.children
                                                              [ Html.img
                                                                    [ prop.alt "Placeholder image"
                                                                      prop.src scene.BackgroundImage ] ] ] ]
                                              Bulma.cardContent
                                                  [ Bulma.media
                                                        [ Bulma.mediaLeft
                                                              [ Bulma.cardImage
                                                                    [ Bulma.image
                                                                          [ Bulma.image.is48x48
                                                                            prop.children
                                                                                [ Html.img
                                                                                      [ prop.alt "Placeholder image"
                                                                                        prop.src scene.BackgroundImage ] ] ] ] ]
                                                          Bulma.mediaContent
                                                              [ Bulma.title.p
                                                                    [ Bulma.title.is4
                                                                      prop.text (
                                                                          if scene.Name = "" then
                                                                              "Scene " + scene.Id.ToString()
                                                                          else
                                                                              scene.Name
                                                                      ) ] ]
                                                          Bulma.content scene.Description ] ]
                                              Bulma.cardFooter
                                                  [ Bulma.cardFooterItem.a
                                                        [ button.isSmall
                                                          color.isDark
                                                          prop.text "Continue"
                                                          prop.onClick (fun _ ->
                                                              dispatch <| NavigateToInitiativeTrackerWithId scene.Id) ] ] ] ] ] ])
                  scenes
          ) ]


let view model userName dispatch =
    Html.div
        [ match userName with
          | Some name -> Html.h2 $"Hello, {name}"
          | None -> Html.span ""
          Bulma.section
              [ (if List.isEmpty model.HomeViewModel.UserScenes then
                     Html.text ""
                 else
                     Html.h1 [ prop.text "Scenes"; Bulma.color.hasTextWhite ])
                (viewScenes model.HomeViewModel.UserScenes dispatch)
                Bulma.button.button
                    [ button.isMedium
                      color.isInfo
                      prop.text "New Scene"
                      prop.onClick (fun _ -> dispatch NavigateToInitiativeTracker) ] ] ]
