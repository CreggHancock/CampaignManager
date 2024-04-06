module Home

open Feliz
open Elmish
open Feliz.Router
open Feliz.Bulma
open Thoth.Fetch
open Fetch
open Thoth.Json
open Scene
open Browser.Types

[<Literal>]
let Route = "Home"

type Character =
    { Id: int
      Name: string
      Description: string
      Level: int }

type HomeViewModel =
    { UserCharacters: Character List
      UserScenes: Scene List
      TemplateSceneId: int Option }


type Model =
    { HomeViewModel: HomeViewModel
      ErrorMessage: string
      EditingSceneName: (int * string) Option }

type Msg =
    | NoOp
    | NavigateToInitiativeTracker
    | NavigateToInitiativeTrackerWithId of int
    | NavigateToInitiativeCharacter
    | OnGetModelSuccess of HomeViewModel
    | OnGetModelError of exn
    | OnGotScenesFromStorageSuccess of string
    | OnGotScenesFromStorageError of exn
    | OnGotTemplateFromStorageSuccess of string
    | OnGotTemplateFromStorageError of exn
    | OnTemplateStorageUpdatedError of exn
    | OnTemplateSceneToggled of Scene
    | CreateSceneFromTemplate of Scene
    | OnTemplateSceneCreated of int
    | OnTemplateSceneCreateError of exn
    | DeleteScene of Scene
    | OnSceneStorageUpdatedError of exn
    | ExportScene of Scene
    | ImportSceneFileSuccess of string
    | ImportSceneFileError
    | SetEditingSceneName of (int * string) option
    | SaveSceneName



let updateTemplateLocalStorage (model: Model, cmd: Cmd<Msg>) =
    model,
    Cmd.batch
        [ cmd
          Cmd.OfFunc.either
              (fun () ->
                  Browser.WebStorage.localStorage.setItem (
                      "template",
                      Encode.Auto.toString (0, model.HomeViewModel.TemplateSceneId)
                  ))
              ()
              (fun () -> NoOp)
              OnTemplateStorageUpdatedError ]

let updateSceneLocalStorage scenes (model: Model, cmd: Cmd<Msg>) =
    { model with
        HomeViewModel =
            { model.HomeViewModel with
                UserScenes = scenes } },
    Cmd.batch
        [ cmd
          Cmd.OfFunc.either
              (fun () -> Browser.WebStorage.localStorage.setItem ("scenes", Encode.Auto.toString (0, scenes)))
              ()
              (fun () -> NoOp)
              OnSceneStorageUpdatedError ]

let addTemplateSceneToLocalStorage (templateScene: Scene) (model: Model, cmd: Cmd<Msg>) =
    let nextId =
        model.HomeViewModel.UserScenes
        |> List.map (fun (s) -> s.Id)
        |> List.sortDescending
        |> List.tryHead
        |> Option.defaultWith (fun () -> 0)
        |> (fun (n) -> n + 1)

    let updatedModel =
        { model with
            HomeViewModel =
                { model.HomeViewModel with
                    UserScenes =
                        { templateScene with
                            Id = nextId
                            Name = $"{templateScene.Name} (Copy)" }
                        :: model.HomeViewModel.UserScenes } }

    updatedModel,
    Cmd.OfFunc.either
        (fun () ->
            Browser.WebStorage.localStorage.setItem (
                "scenes",
                Encode.Auto.toString (0, updatedModel.HomeViewModel.UserScenes)
            ))
        ()
        (fun () -> OnTemplateSceneCreated nextId)
        OnTemplateSceneCreateError


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
        Cmd.batch
            [ (Cmd.OfFunc.either (fun () -> Browser.WebStorage.localStorage.getItem ("scenes")) () (fun (sceneInfo) ->
                  OnGotScenesFromStorageSuccess sceneInfo))
                  OnGotScenesFromStorageError
              (Cmd.OfFunc.either
                  (fun () -> Browser.WebStorage.localStorage.getItem ("template"))
                  ()
                  (fun (templateInfo) -> OnGotTemplateFromStorageSuccess templateInfo))
                  OnGotTemplateFromStorageError ]

let init accessToken =
    { HomeViewModel =
        { UserCharacters = []
          UserScenes = []
          TemplateSceneId = None }
      ErrorMessage = ""
      EditingSceneName = None },
    getHomeViewModel accessToken

let update (msg: Msg) (model: Model) =
    match msg with
    | NoOp -> model, Cmd.none
    | NavigateToInitiativeTracker -> model, Cmd.navigate (InitiativeTracker.Route)
    | OnGetModelSuccess response -> { model with HomeViewModel = response }, Cmd.none
    | OnGetModelError error ->
        { model with
            ErrorMessage = error.Message },
        Cmd.none
    | NavigateToInitiativeCharacter -> model, Cmd.navigate (InitiativeTracker.Route)
    | NavigateToInitiativeTrackerWithId id ->
        model, Cmd.navigate (InitiativeTracker.Route + "?sceneId=" + id.ToString())
    | OnGotScenesFromStorageSuccess scenesString ->
        let sceneResults = Decode.Auto.fromString<Scene List> (scenesString)

        match sceneResults with
        | Ok scenes ->
            { model with
                HomeViewModel =
                    { model.HomeViewModel with
                        UserScenes = scenes } },
            Cmd.none
        | Error err -> { model with ErrorMessage = err }, Cmd.none
    | OnGotScenesFromStorageError error ->
        { model with
            ErrorMessage = error.Message },
        Cmd.none
    | OnGotTemplateFromStorageSuccess templateString ->
        let sceneResults = Decode.Auto.fromString<int Option> (templateString)

        match sceneResults with
        | Ok scene ->
            { model with
                HomeViewModel =
                    { model.HomeViewModel with
                        TemplateSceneId = scene } },
            Cmd.none
        | Error err -> { model with ErrorMessage = err }, Cmd.none
    | OnGotTemplateFromStorageError error ->
        { model with
            ErrorMessage = error.Message },
        Cmd.none
    | OnTemplateStorageUpdatedError error ->
        { model with
            ErrorMessage = error.Message },
        Cmd.none
    | OnTemplateSceneToggled scene ->
        if model.HomeViewModel.TemplateSceneId = Some scene.Id then
            { model with
                HomeViewModel =
                    { model.HomeViewModel with
                        TemplateSceneId = None } },
            Cmd.none
        else
            { model with
                HomeViewModel =
                    { model.HomeViewModel with
                        TemplateSceneId = Some scene.Id } },
            Cmd.none
        |> updateTemplateLocalStorage
    | CreateSceneFromTemplate scene -> (model, Cmd.none) |> addTemplateSceneToLocalStorage scene
    | OnTemplateSceneCreated id -> model, Cmd.navigate (InitiativeTracker.Route + "?sceneId=" + id.ToString())
    | OnTemplateSceneCreateError error ->
        { model with
            ErrorMessage = error.Message },
        Cmd.none
    | DeleteScene scene ->
        (model, Cmd.none)
        |> updateSceneLocalStorage (model.HomeViewModel.UserScenes |> List.filter (fun s -> s.Id <> scene.Id))
    | OnSceneStorageUpdatedError error ->
        { model with
            ErrorMessage = error.Message },
        Cmd.none
    | ImportSceneFileSuccess fileString ->
        let sceneResult = Decode.Auto.fromString<Scene> (fileString)

        match sceneResult with
        | Ok scene -> (model, Cmd.none) |> addTemplateSceneToLocalStorage scene
        | Error err -> { model with ErrorMessage = err }, Cmd.none
    | SetEditingSceneName maybeScene ->
        { model with
            EditingSceneName = maybeScene },
        Cmd.none
    | SaveSceneName ->
        match model.EditingSceneName with
        | Some(sceneId, newName) ->
            ({ model with EditingSceneName = None }, Cmd.none)
            |> updateSceneLocalStorage (
                model.HomeViewModel.UserScenes
                |> List.map (fun s -> if s.Id = sceneId then { s with Name = newName } else s)
            )

        | None -> (model, Cmd.none)

let viewCharacters (characters: Character List) : Fable.React.ReactElement =
    Html.div (
        Html.div [ Html.h3 "Characters" ]
        :: List.map (fun (character: Character) -> Html.div [ Html.text character.Name ]) characters
    )

let viewScenes
    (scenes: Scene List)
    (templateSceneId: int Option)
    (editingScene: (int * string) Option)
    dispatch
    : Fable.React.ReactElement =
    Html.div
        [ prop.className "scene-list columns is-4 is-variable is-multiline mt-2"
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
                                                              [ match editingScene with
                                                                | Some(sceneId, name) ->
                                                                    if sceneId = scene.Id then
                                                                        Bulma.field.div
                                                                            [ prop.className "scene-name"
                                                                              prop.children
                                                                                  [ Bulma.control.div
                                                                                        [ Bulma.input.text
                                                                                              [ prop.value name
                                                                                                prop.onChange
                                                                                                    (fun ev ->
                                                                                                        dispatch (
                                                                                                            SetEditingSceneName
                                                                                                            <| Some(
                                                                                                                sceneId,
                                                                                                                ev
                                                                                                            )
                                                                                                        )) ] ]
                                                                                    Bulma.button.button
                                                                                        [ button.isSmall
                                                                                          color.isInfo
                                                                                          prop.title "Save"
                                                                                          prop.onClick (fun _ ->
                                                                                              dispatch SaveSceneName)
                                                                                          prop.children
                                                                                              [ Bulma.icon
                                                                                                    [ Html.i
                                                                                                          [ prop.classes
                                                                                                                [ "fa-floppy-disk"
                                                                                                                  "fa-solid" ] ] ]


                                                                                                ]

                                                                                          ]
                                                                                    Bulma.button.button
                                                                                        [ button.isSmall
                                                                                          color.isDanger
                                                                                          prop.title "Cancel"
                                                                                          prop.onClick (fun _ ->
                                                                                              dispatch
                                                                                              <| SetEditingSceneName
                                                                                                  None)
                                                                                          prop.children
                                                                                              [ Bulma.icon
                                                                                                    [ Html.i
                                                                                                          [ prop.classes
                                                                                                                [ "fa-xmark"
                                                                                                                  "fa-solid" ] ] ]


                                                                                                ]

                                                                                          ] ] ]
                                                                    else
                                                                        Bulma.title.p
                                                                            [ Bulma.title.is4
                                                                              prop.text (
                                                                                  if scene.Name = "" then
                                                                                      "Scene " + scene.Id.ToString()
                                                                                  else
                                                                                      scene.Name
                                                                              ) ]
                                                                | None ->
                                                                    Html.span
                                                                        [ prop.className "scene-name"
                                                                          prop.children
                                                                              [ Bulma.title.p
                                                                                    [ Bulma.title.is4
                                                                                      prop.text (
                                                                                          if scene.Name = "" then
                                                                                              "Scene "
                                                                                              + scene.Id.ToString()
                                                                                          else
                                                                                              scene.Name
                                                                                      ) ]

                                                                                Bulma.button.button
                                                                                    [ button.isSmall
                                                                                      button.isInverted
                                                                                      color.isWhite
                                                                                      prop.title "Edit Name"
                                                                                      prop.onClick (fun _ ->
                                                                                          dispatch
                                                                                          <| SetEditingSceneName(
                                                                                              Some(
                                                                                                  scene.Id,
                                                                                                  scene.Name
                                                                                              )
                                                                                          ))
                                                                                      prop.children
                                                                                          [ Bulma.icon
                                                                                                [ Html.i
                                                                                                      [ prop.classes
                                                                                                            [ "fa-pen"
                                                                                                              "fa-solid" ] ] ]


                                                                                            ]

                                                                                      ] ] ] ]

                                                          Bulma.content scene.Description ] ]
                                              Bulma.cardFooter
                                                  [ Bulma.cardFooterItem.div
                                                        [ Bulma.button.button
                                                              [ button.isMedium
                                                                color.isWhite
                                                                prop.title "Save As Template"
                                                                prop.onClick (fun _ ->
                                                                    dispatch <| OnTemplateSceneToggled scene)
                                                                prop.children
                                                                    [ Bulma.icon
                                                                          [ Html.i
                                                                                [ prop.classes
                                                                                      [ "fa-bookmark"
                                                                                        if
                                                                                            (templateSceneId = Some
                                                                                                scene.Id)
                                                                                        then
                                                                                            "fa-solid"
                                                                                        else
                                                                                            "fa-regular" ] ] ]


                                                                      ]

                                                                ] ]
                                                    Bulma.cardFooterItem.a
                                                        [ button.isMedium
                                                          color.isWhite
                                                          let sceneName =
                                                              (if scene.Name = "" then
                                                                   "Scene " + scene.Id.ToString()
                                                               else
                                                                   scene.Name)

                                                          prop.href
                                                              $"data:text/json;charset=utf-8,{Encode.Auto.toString (0, scene)}"

                                                          prop.type' "text/json"
                                                          prop.download $"{sceneName}.json"
                                                          prop.title "Export Scene"

                                                          prop.children
                                                              [ Bulma.icon
                                                                    [ Html.i
                                                                          [ prop.classes
                                                                                [ "fa-file-arrow-down"; "fa-solid" ] ] ]


                                                                ] ]
                                                    Bulma.cardFooterItem.a
                                                        [ button.isSmall
                                                          color.isDark
                                                          prop.text "Continue"
                                                          prop.onClick (fun _ ->
                                                              dispatch <| NavigateToInitiativeTrackerWithId scene.Id) ]

                                                    Bulma.cardFooterItem.div
                                                        [ Bulma.button.button
                                                              [ button.isMedium
                                                                prop.title "Delete"
                                                                color.isDanger
                                                                button.isInverted
                                                                prop.onClick (fun _ -> dispatch <| DeleteScene scene)
                                                                prop.children
                                                                    [ Bulma.icon
                                                                          [ Html.i
                                                                                [ prop.classes
                                                                                      [ "fa-trash"; "fa-solid" ] ] ]


                                                                      ]

                                                                ] ] ] ] ] ] ])
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
                Html.span
                    [ prop.className "file file-import"

                      prop.children
                          [ Html.label
                                [ prop.className "file-label"
                                  prop.children
                                      [ Html.input
                                            [ prop.className "file-input"
                                              prop.type' "file"
                                              prop.name "resume"
                                              prop.onChange (fun (file: File) ->
                                                  let reader = Browser.Dom.FileReader.Create()

                                                  reader.onload <-
                                                      fun _ -> dispatch (ImportSceneFileSuccess(string reader.result))

                                                  reader.onerror <- fun _ -> dispatch ImportSceneFileError

                                                  reader.readAsText (file)) ]
                                        Html.span
                                            [ prop.className "file-cta"
                                              prop.children
                                                  [ Html.span
                                                        [ prop.className "file-icon"
                                                          prop.children
                                                              [ Html.i [ prop.className "fa-solid fa-file-arrow-up" ] ] ]
                                                    Html.span [ prop.className "file-label"; prop.text "Import" ] ] ] ] ] ] ]
                (viewScenes
                    model.HomeViewModel.UserScenes
                    model.HomeViewModel.TemplateSceneId
                    model.EditingSceneName
                    dispatch)
                Bulma.button.button
                    [ button.isMedium
                      Bulma.spacing.mr1
                      color.isInfo
                      prop.text "New Scene"
                      prop.onClick (fun _ -> dispatch NavigateToInitiativeTracker) ]
                match model.HomeViewModel.TemplateSceneId with
                | Some sceneId ->
                    match model.HomeViewModel.UserScenes |> List.tryFind (fun s -> s.Id = sceneId) with
                    | Some scene ->
                        Bulma.button.button
                            [ button.isMedium
                              Bulma.spacing.ml1
                              color.isBlack
                              prop.text "Create From Template"
                              prop.onClick (fun _ -> dispatch <| CreateSceneFromTemplate scene) ]
                    | None -> Html.none
                | None -> Html.none ] ]
