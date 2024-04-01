module InitiativeTracker

open Feliz
open Elmish
open Feliz.Bulma
open Feliz.Router
open Fetch
open Thoth.Fetch
open Thoth.Json
open Fable.Core.Util
open Fable.Core
open Fable.Core.JS

[<Literal>]
let Route = "InitiativeTracker"

type CombatantType =
    | Enemy
    | Ally
    | Player

type GameState =
    | CharacterSetup
    | Active

type Monster =
    { Index: string
      Name: string
      Image: string option
      HitPoints: int
      Dexterity: int }

type MonsterSummary = { Index: string; Name: string }

type MonsterOptionResponse =
    { Count: int
      Results: MonsterSummary List }

type Combatant =
    { InitiativeModifier: int
      Name: string
      ImageUrl: string
      PlayerType: CombatantType
      LocationX: int
      LocationY: int
      Health: int
      MaxHealth: int
      ArmorClass: int
      IsTokenBeingDragged: bool
      PreviousLocationX: int
      PreviousLocationY: int
      RolledInitiative: int option }

type Scene =
    { Id: int
      Name: string
      Description: string
      BackgroundImage: string
      Width: int
      Height: int
      SquareSize: int
      CombatantTurn: int
      Round: int
      Combatants: Combatant List
      GameState: GameState
      ShowGrid: bool }

type InitiativeViewModel = { Scene: Scene }

type Model =
    { InitiativeViewModel: InitiativeViewModel
      NewCharacter: Combatant option
      ErrorMessage: string
      IsLoggedIn: bool
      MonsterOptions: MonsterSummary List
      BackgroundDropdownToggled: bool
      InitiativePopupOpen: bool }

type Msg =
    | NoOp
    | AddCharacterClicked
    | RollInitiativesClicked
    | StartCombatClicked
    | EndTurnClicked
    | ResetClicked
    | NewCharacterNameUpdated of string
    | NewCharacterDexterityUpdated of int
    | NewCharacterImageUpdated of string
    | NewCharacterPlayerTypeUpdated of string
    | NewCharacterCancelClicked
    | NewCharacterCreateClicked
    | BackgroundUpdated of string
    | OnGetModelSuccess of InitiativeViewModel
    | OnGetModelError of exn
    | OnGotModelFromStorageSuccess of string * string
    | OnGotModelFromStorageError of exn
    | OnStorageUpdatedSuccess
    | OnStorageUpdatedError of exn
    | OnGetMonsterOptionsSuccess of MonsterOptionResponse
    | OnGetMonsterOptionsError of exn
    | NewCharacterNameOptionClicked of string
    | OnGetMonsterDetailsSuccess of Monster
    | OnGetMonsterDetailsError of exn
    | OnTokenClicked of int * float * float
    | OnTokenReleased
    | OnTokenMove of float * float
    | BackgroundDropdownToggled
    | CharacterInitiativeSet of int * int option
    | RollInitiativeCancelClicked
    | RollInitiativeRollClicked
    | ShowGridToggled of bool
    | OnTokenDeleteClicked of int


let getInitiativeViewModel accessToken maybeSceneId =
    match maybeSceneId |> Option.map (fun (id) -> id.ToString()) with
    | Some sceneId ->
        match accessToken with
        | Some token ->
            let headers = [ Authorization("Bearer " + token) ]

            Cmd.OfPromise.either
                (fun () -> Fetch.get ("https://localhost:7068/Home/Get/" + sceneId, headers = headers))
                ()
                (fun (response) -> OnGetModelSuccess response)
                OnGetModelError
        | None ->
            (Cmd.OfFunc.either (fun () -> Browser.WebStorage.localStorage.getItem ("scenes")) () (fun (sceneInfo) ->
                OnGotModelFromStorageSuccess(sceneInfo, sceneId)))
                OnGotModelFromStorageError
    | None -> Cmd.none

let getMonsterOptions () =
    Cmd.OfPromise.either
        (fun () -> Fetch.get ("https://www.dnd5eapi.co/api/monsters", caseStrategy = CaseStrategy.CamelCase))
        ()
        (fun (response) -> OnGetMonsterOptionsSuccess response)
        OnGetMonsterOptionsError

let getMonsterDetails index =
    Cmd.OfPromise.either
        (fun () -> Fetch.get ("https://www.dnd5eapi.co/api/monsters/" + index, caseStrategy = CaseStrategy.SnakeCase))
        ()
        (fun (response) -> OnGetMonsterDetailsSuccess response)
        OnGetMonsterDetailsError

let updateScene updater model =
    { model with
        InitiativeViewModel = { Scene = updater model.InitiativeViewModel.Scene } }

let updateLocalStorage (model: Model, cmd: Cmd<Msg>) =
    let sceneResults =
        Decode.Auto.fromString<Scene List> (Browser.WebStorage.localStorage.getItem ("scenes"))

    match sceneResults with
    | Ok scenes ->
        let nextId =
            scenes
            |> List.map (fun (s) -> s.Id)
            |> List.sortDescending
            |> List.tryHead
            |> Option.defaultWith (fun () -> 0)
            |> (fun (n) -> n + 1)


        let updatedModel =
            model |> updateScene (fun s -> if s.Id > 0 then s else { s with Id = nextId })

        let updatedScenes =
            if
                model.InitiativeViewModel.Scene.Id > 0
                && List.exists (fun (s) -> s.Id = model.InitiativeViewModel.Scene.Id) scenes
            then
                List.map
                    (fun (s) ->
                        if s.Id = model.InitiativeViewModel.Scene.Id then
                            model.InitiativeViewModel.Scene
                        else
                            s)
                    scenes
            else
                updatedModel.InitiativeViewModel.Scene :: scenes

        updatedModel,
        Cmd.batch
            [ cmd
              Cmd.OfFunc.either
                  (fun () ->
                      Browser.WebStorage.localStorage.setItem ("scenes", Encode.Auto.toString (0, updatedScenes)))
                  ()
                  (fun () -> OnStorageUpdatedSuccess)
                  OnStorageUpdatedError ]
    | Error err ->
        { model with
            ErrorMessage = "No scenes found" }
        |> updateScene (fun s -> if s.Id > 0 then s else { s with Id = 1 }),
        Cmd.OfFunc.either
            (fun () ->
                Browser.WebStorage.localStorage.setItem (
                    "scenes",
                    Encode.Auto.toString (
                        0,
                        List.singleton
                            { model.InitiativeViewModel.Scene with
                                Id = 1 }
                    )
                ))
            ()
            (fun () -> OnStorageUpdatedSuccess)
            OnStorageUpdatedError


[<Import("initialize", "../js/pan.js")>]
let initializePan: unit -> unit = jsNative

[<Import("setDraggable", "../js/pan.js")>]
let setDraggable (canDrag: bool, blockingId: string) : unit = jsNative

[<Import("getPanOffset", "../js/pan.js")>]
let getPanOffset
    : unit
          -> {| xOffset: int
                yOffset: int
                scale: int |} =
    jsNative

let init accessToken sceneId =
    ({ InitiativeViewModel =
        { Scene =
            { Id = sceneId |> Option.defaultWith (fun () -> -1)
              Combatants = []
              Name = ""
              Description = ""
              CombatantTurn = 0
              Round = 0
              Width = 0
              Height = 0
              SquareSize = 0
              GameState = CharacterSetup
              BackgroundImage = "https://rattrapgames.com/cdn/shop/products/RAT-MAT-GF001_1024x1024@2x.jpg?v=1576461000"
              ShowGrid = false } }
       NewCharacter = None
       ErrorMessage = ""
       IsLoggedIn = Option.isSome accessToken
       MonsterOptions = []
       BackgroundDropdownToggled = false
       InitiativePopupOpen = false },
     Cmd.batch
         [ getInitiativeViewModel accessToken sceneId
           getMonsterOptions ()
           Cmd.OfFunc.perform (fun () -> initializePan ()) () (fun () -> NoOp) ]

    )

let update (msg: Msg) (model: Model) =
    match msg with
    | NoOp -> model, Cmd.none
    | AddCharacterClicked ->
        { model with
            NewCharacter =
                Some
                    { Name = ""
                      InitiativeModifier = 0
                      ImageUrl = ""
                      PlayerType = Player
                      LocationX = 0
                      LocationY = 0
                      Health = 0
                      MaxHealth = 0
                      ArmorClass = 0
                      IsTokenBeingDragged = false
                      PreviousLocationX = 0
                      PreviousLocationY = 0
                      RolledInitiative = None } },
        Cmd.OfFunc.perform setDraggable (false, "new-character-popup") (fun () -> NoOp)
    | RollInitiativesClicked ->
        ({ model with
            InitiativePopupOpen = true },
         Cmd.OfFunc.perform setDraggable (false, "initiative-popup") (fun () -> NoOp))
        |> updateLocalStorage
    | StartCombatClicked ->
        ({ model with
            InitiativePopupOpen = false }
         |> updateScene (fun scene ->
             { scene with
                 GameState = Active
                 CombatantTurn = 0
                 Combatants =
                     scene.Combatants
                     |> List.sortBy (fun c -> c.RolledInitiative |> Option.defaultValue 0)
                     |> List.rev }),
         Cmd.none)
        |> updateLocalStorage
    | EndTurnClicked ->
        (model
         |> updateScene (fun scene ->
             { scene with
                 GameState = Active
                 CombatantTurn = (scene.CombatantTurn + 1) % (List.length scene.Combatants) }),
         Cmd.none)
        |> updateLocalStorage
    | ResetClicked ->
        (model
         |> updateScene (fun scene ->
             { scene with
                 GameState = CharacterSetup
                 CombatantTurn = 0 }),
         Cmd.none)
        |> updateLocalStorage
    | NewCharacterNameUpdated event ->
        { model with
            NewCharacter = Option.map (fun c -> { c with Name = event }) model.NewCharacter },
        Cmd.none
    | NewCharacterDexterityUpdated dex ->
        { model with
            NewCharacter = Option.map (fun c -> { c with InitiativeModifier = dex }) model.NewCharacter },
        Cmd.none
    | NewCharacterImageUpdated event ->
        { model with
            NewCharacter = Option.map (fun c -> { c with ImageUrl = event }) model.NewCharacter },
        Cmd.none
    | NewCharacterPlayerTypeUpdated event ->
        match event with
        | "player" ->
            { model with
                NewCharacter = Option.map (fun c -> { c with PlayerType = Player }) model.NewCharacter },
            Cmd.none
        | "enemy" ->
            { model with
                NewCharacter = Option.map (fun c -> { c with PlayerType = Enemy }) model.NewCharacter },
            Cmd.none
        | "ally" ->
            { model with
                NewCharacter = Option.map (fun c -> { c with PlayerType = Ally }) model.NewCharacter },
            Cmd.none
        | _ ->
            { model with
                NewCharacter = Option.map (fun c -> { c with PlayerType = Player }) model.NewCharacter },
            Cmd.none
    | NewCharacterCancelClicked ->
        { model with NewCharacter = None },
        Cmd.OfFunc.perform setDraggable (true, "new-character-popup") (fun () -> NoOp)
    | NewCharacterCreateClicked ->
        match model.NewCharacter with
        | Some character ->
            let updatedScene =
                model
                |> updateScene (fun scene ->
                    { scene with
                        Combatants = character :: scene.Combatants })

            ({ updatedScene with
                NewCharacter = None },
             Cmd.OfFunc.perform setDraggable (true, "new-character-popup") (fun () -> NoOp))
            |> updateLocalStorage
        | None -> model, Cmd.none
    | BackgroundUpdated background ->
        (model
         |> updateScene (fun scene ->
             { scene with
                 BackgroundImage = background }),
         Cmd.none)
        |> updateLocalStorage
    | OnGetModelSuccess viewModel -> model, Cmd.none
    | OnGetModelError err ->
        { model with
            ErrorMessage = err.Message },
        Cmd.none
    | OnGotModelFromStorageSuccess(scenesString, sceneId) ->
        let sceneResults = Decode.Auto.fromString<Scene List> (scenesString)

        match sceneResults with
        | Ok scenes ->
            model
            |> updateScene (fun scene ->
                (scenes
                 |> List.tryFind (fun (sceneOption) -> sceneOption.Id.ToString() = sceneId)
                 |> Option.map (fun (scene) ->
                     { scene with
                         Combatants = scene.Combatants |> List.map (fun (c) -> { c with IsTokenBeingDragged = false }) })
                 |> Option.defaultWith (fun () -> scene))),
            Cmd.none
        | Error err -> { model with ErrorMessage = err }, Cmd.none
    | OnGotModelFromStorageError err ->
        { model with
            ErrorMessage = err.Message },
        Cmd.none
    | OnStorageUpdatedSuccess ->
        model,
        match Router.currentUrl () with
        | [ Route ] -> Cmd.navigate (Route + "?sceneId=" + model.InitiativeViewModel.Scene.Id.ToString())
        | _ -> Cmd.none
    | OnStorageUpdatedError err ->
        ({ model with
            ErrorMessage = err.Message },
         Cmd.OfFunc.either
             (fun () -> Browser.WebStorage.localStorage.setItem ("scenes", Encode.Auto.toString (0, [])))
             ()
             (fun () -> OnStorageUpdatedSuccess)
             OnStorageUpdatedError)
    | OnGetMonsterOptionsSuccess monsterOptionsResponse ->
        { model with
            MonsterOptions = monsterOptionsResponse.Results },
        Cmd.none
    | OnGetMonsterOptionsError err ->
        { model with
            ErrorMessage = err.Message },
        Cmd.none
    | NewCharacterNameOptionClicked(monsterIndex: string) ->
        let selectedMonster =
            model.MonsterOptions |> List.tryFind (fun (m) -> m.Index = monsterIndex)

        match selectedMonster with
        | Some monster ->
            ({ model with
                NewCharacter =
                    model.NewCharacter
                    |> Option.map (fun (character) -> { character with Name = monster.Name }) },
             getMonsterDetails monsterIndex)
        | None -> (model, Cmd.none)
    | OnGetMonsterDetailsSuccess response ->
        ({ model with
            NewCharacter =
                model.NewCharacter
                |> Option.map (fun (n) ->
                    { n with
                        Health = response.HitPoints
                        MaxHealth = response.HitPoints
                        ImageUrl =
                            response.Image
                            |> Option.map (fun (img) -> "https://www.dnd5eapi.co" + img)
                            |> Option.defaultWith (fun () -> "")
                        InitiativeModifier = (response.Dexterity - 10) / 2 }) },
         Cmd.none)
    | OnGetMonsterDetailsError err ->
        { model with
            ErrorMessage = err.Message },
        Cmd.none
    | OnTokenClicked(tokenIndex, newX, newY) ->
        (model
         |> updateScene (fun scene ->
             { scene with
                 Combatants =
                     scene.Combatants
                     |> List.mapi (fun ind c ->
                         if ind = tokenIndex then
                             { c with
                                 IsTokenBeingDragged = true
                                 PreviousLocationX = int newX
                                 PreviousLocationY = int newY }
                         else
                             c) }),
         Cmd.OfFunc.perform setDraggable (false, "token") (fun () -> NoOp))
    | OnTokenReleased ->
        (model
         |> updateScene (fun scene ->
             { scene with
                 Combatants = scene.Combatants |> List.map (fun c -> { c with IsTokenBeingDragged = false }) }),
         Cmd.OfFunc.perform setDraggable (true, "token") (fun () -> NoOp))
        |> updateLocalStorage
    | OnTokenMove(newX, newY) ->
        let offsetTransform = (getPanOffset ())

        (model
         |> updateScene (fun scene ->
             { scene with
                 Combatants =
                     scene.Combatants
                     |> List.map (fun c ->
                         if c.IsTokenBeingDragged then
                             { c with
                                 LocationX = ((int newX - c.PreviousLocationX) / offsetTransform.scale) + c.LocationX
                                 LocationY = ((int newY - c.PreviousLocationY) / offsetTransform.scale) + c.LocationY
                                 PreviousLocationX = int newX
                                 PreviousLocationY = int newY }
                         else
                             c) }),
         Cmd.none)
    | BackgroundDropdownToggled ->
        ({ model with
            BackgroundDropdownToggled = not model.BackgroundDropdownToggled },
         Cmd.OfFunc.perform setDraggable (model.BackgroundDropdownToggled, "background-dropdown") (fun () -> NoOp))
    | CharacterInitiativeSet(combatantIndex, initiative) ->
        (model
         |> updateScene (fun s ->
             { s with
                 Combatants =
                     List.mapi
                         (fun ind (combatant: Combatant) ->
                             if ind = combatantIndex then
                                 { combatant with
                                     RolledInitiative = initiative }
                             else
                                 combatant)
                         s.Combatants }),
         Cmd.none)
        |> updateLocalStorage
    | RollInitiativeCancelClicked ->
        ({ model with
            InitiativePopupOpen = false },
         Cmd.OfFunc.perform setDraggable (true, "initiative-popup") (fun () -> NoOp))
    | RollInitiativeRollClicked ->
        (model
         |> updateScene (fun s ->
             { s with
                 Combatants =
                     List.map
                         (fun (combatant: Combatant) ->
                             match combatant.RolledInitiative with
                             | Some _ -> combatant
                             | None ->
                                 { combatant with
                                     RolledInitiative =
                                         Math.random ()
                                         |> (fun i -> i * 20.0)
                                         |> Math.floor
                                         |> int
                                         |> (fun i -> i + 1 + combatant.InitiativeModifier)
                                         |> Some })
                         s.Combatants }),
         Cmd.OfFunc.perform setDraggable (true, "initiative-popup") (fun () -> NoOp))
        |> updateLocalStorage
    | ShowGridToggled toggled -> (model |> updateScene (fun s -> { s with ShowGrid = toggled }), Cmd.none)
    | OnTokenDeleteClicked tokenIndex ->
        (model
         |> updateScene (fun scene ->
             { scene with
                 Combatants = scene.Combatants |> List.removeAt tokenIndex }),
         Cmd.none)



let viewPlayerCard player ind currentTurn dispatch =
    let getClassFromPlayer playerType =
        match playerType with
        | Enemy -> "enemy"
        | Ally -> "ally"
        | Player -> "player"


    Html.div
        [ prop.classes
              [ "initiative-card"
                getClassFromPlayer player.PlayerType
                if ind = currentTurn then "active" else "" ]
          prop.children
              [ Html.img [ prop.src player.ImageUrl; prop.title player.Name ]
                Html.div
                    [ prop.className "character-details"
                      prop.children
                          [ Html.div
                                [ prop.className "stats dex"
                                  prop.children
                                      [ Html.span [ prop.className "stat-label"; prop.text "Dex." ]
                                        Html.span
                                            [ prop.className "stat-mod"; prop.text $"{player.InitiativeModifier}" ] ] ] ] ]
                if player.PlayerType <> Player then
                    Html.button
                        [ prop.className "remove-button"
                          prop.onPointerUp (fun ev ->
                              ev.stopPropagation ()
                              ev.preventDefault ()
                              dispatch <| OnTokenDeleteClicked ind)
                          prop.children [ Html.i [ prop.className "fa-solid fa-skull" ] ] ]
                else
                    Html.none ] ]

let viewPlayerToken player playerIndex isActive dispatch =
    let getClassFromPlayer playerType =
        match playerType with
        | Enemy -> "enemy"
        | Ally -> "ally"
        | Player -> "player"


    Html.div
        [ prop.style
              [ style.left (length.px (player.LocationX))
                style.top (length.px (player.LocationY)) ]
          prop.onMouseDown
          <| (fun (ev) ->
              ev.stopPropagation ()
              ev.preventDefault ()
              dispatch <| OnTokenClicked(playerIndex, ev.clientX, ev.clientY))
          prop.className
              $"""map-token initiative-card {getClassFromPlayer player.PlayerType} {if isActive then "active" else ""}"""
          prop.children
              [ Html.img [ prop.src player.ImageUrl; prop.title player.Name ]
                Html.span
                    [ prop.className "character-name"
                      prop.text player.Name

                      ] ] ]

let viewStateButton neededGameState currentGameState (buttonText: string) (buttonId: string) onClickMsg dispatch =
    Bulma.button.button
        [ prop.text buttonText
          color.isDark
          prop.classes
              [ "button"
                if currentGameState = neededGameState then
                    ""
                else
                    "button-hidden" ]
          prop.type' "button"
          prop.id buttonId
          prop.onPointerUp (fun _ ->
              if currentGameState = neededGameState then
                  dispatch onClickMsg
              else
                  dispatch NoOp) ]

let viewCharacterInitiativeEntry (combatant: Combatant) (combatantIndex: int) dispatch =
    [ Html.div
          [ prop.classes [ "flex-75"; "combatant" ]
            prop.children
                [ Html.span
                      [ Html.text combatant.Name
                        Html.text " "
                        Html.text (
                            if combatant.InitiativeModifier < 0 then
                                (combatant.InitiativeModifier.ToString())
                            else
                                ("+" + (combatant.InitiativeModifier.ToString()))
                        ) ]
                  Bulma.label
                      [ Bulma.input.checkbox
                            [ prop.onChange (fun (ev: bool) ->
                                  dispatch (CharacterInitiativeSet(combatantIndex, (if ev then None else Some 0))))
                              Option.isNone combatant.RolledInitiative |> prop.isChecked ]
                        Bulma.text.span " "
                        Bulma.text.span "Roll automatically" ] ] ]
      match combatant.RolledInitiative with
      | Some initiative ->
          Html.div
              [ prop.className "flex-25"
                prop.children
                    [ Bulma.input.number
                          [ Bulma.input.isSmall
                            prop.placeholder "Initiative"
                            prop.name "initiative"
                            prop.onChange (fun ev -> dispatch (CharacterInitiativeSet(combatantIndex, Some ev)))
                            initiative.ToString() |> prop.value ] ] ]
      | None -> Html.div [ prop.className "flex-25" ]

      ]


let view model dispatch =
    Html.div
        [ prop.className "game-board"
          prop.onMouseUp
          <| (fun (ev) ->
              ev.stopPropagation ()
              ev.preventDefault ()
              dispatch <| OnTokenReleased)
          prop.onMouseMove
          <| (fun (ev) ->
              ev.stopPropagation ()
              ev.preventDefault ()
              dispatch <| OnTokenMove(ev.clientX, ev.clientY))
          prop.children
              [ Bulma.dropdown
                    [ prop.id "map-select"
                      prop.className "map-select"
                      if model.BackgroundDropdownToggled then
                          dropdown.isActive
                      else
                          prop.text ""
                      prop.children
                          [ Bulma.dropdownTrigger
                                [ Bulma.button.button
                                      [ color.isDark
                                        prop.text "Map"
                                        prop.onPointerUp (fun ev ->
                                            ev.stopPropagation ()
                                            ev.preventDefault ()
                                            dispatch <| BackgroundDropdownToggled) ]

                                  ]
                            Bulma.dropdownMenu
                                [ Bulma.dropdownContent
                                      [ Bulma.color.isBlack
                                        prop.children
                                            [ Bulma.dropdownItem.div
                                                  [ Bulma.input.text
                                                        [ prop.onChange (fun ev -> dispatch (BackgroundUpdated ev)) ] ]
                                              Bulma.dropdownItem.div
                                                  [ Bulma.label
                                                        [ Bulma.input.checkbox
                                                              [ prop.onChange (fun (ev: bool) ->
                                                                    dispatch (ShowGridToggled ev))
                                                                prop.isChecked model.InitiativeViewModel.Scene.ShowGrid ]
                                                          Bulma.text.span " "
                                                          Bulma.text.span "Display Grid" ] ] ] ]

                                  ]

                            ] ]
                Html.div
                    [ prop.className "game-map"
                      prop.id "battleMap"
                      ([ Html.img
                             [ prop.id "draggable-image"
                               prop.className "map-image"
                               prop.src model.InitiativeViewModel.Scene.BackgroundImage ]
                         if model.InitiativeViewModel.Scene.ShowGrid then
                             Html.div [ prop.className "grid" ]
                         else
                             Html.none ])
                      @ List.mapi
                          (fun ind player ->
                              viewPlayerToken player ind (model.InitiativeViewModel.Scene.CombatantTurn = ind) dispatch)
                          model.InitiativeViewModel.Scene.Combatants
                      |> prop.children ]
                Html.div
                    [ prop.className "hud-wrapper"
                      prop.children
                          [ Html.div
                                [ prop.className "float-screen"
                                  prop.children
                                      [ Html.div
                                            [ prop.className "initiative"
                                              prop.id "initiativeRotation"
                                              List.mapi
                                                  (fun ind player ->
                                                      viewPlayerCard
                                                          player
                                                          ind
                                                          model.InitiativeViewModel.Scene.CombatantTurn
                                                          dispatch)
                                                  model.InitiativeViewModel.Scene.Combatants
                                              |> prop.children ] ] ] ] ]
                match model.NewCharacter with
                | Some character ->
                    Html.div
                        [ prop.classes [ "create-character" ]
                          prop.id "createCharacterForm"
                          prop.children
                              [ Html.h1 [ prop.className "container-title"; prop.text "Add Character" ]
                                Html.div
                                    [ prop.classes [ "flex-50"; "create-character-name" ]
                                      prop.children (
                                          let filteredOptions =
                                              if character.PlayerType <> Player then
                                                  model.MonsterOptions
                                                  |> List.filter (fun (m) ->
                                                      character
                                                      |> (fun (c) ->
                                                          c.Name.Length > 0
                                                          && c.Name <> m.Name
                                                          && m.Name.StartsWith(
                                                              c.Name,
                                                              System.StringComparison.InvariantCultureIgnoreCase
                                                          )))
                                              else
                                                  []

                                          [ Bulma.label "Name"
                                            Bulma.input.text
                                                [ prop.placeholder "Name"
                                                  prop.name "characterName"
                                                  prop.type' "text"
                                                  prop.id "characterName"
                                                  prop.required true
                                                  prop.className (
                                                      if List.length filteredOptions > 0 then
                                                          "input-with-filters"
                                                      else
                                                          "input-no-filters"
                                                  )
                                                  prop.onChange (fun ev -> dispatch (NewCharacterNameUpdated ev))
                                                  prop.value character.Name ]
                                            if List.isEmpty filteredOptions |> not then
                                                Html.div
                                                    [ prop.className "filter-box"
                                                      prop.children (
                                                          filteredOptions
                                                          |> List.sortBy (fun (m) -> m.Name)
                                                          |> List.take (
                                                              if List.length filteredOptions > 4 then
                                                                  4
                                                              else
                                                                  List.length filteredOptions
                                                          )
                                                          |> List.map (fun (m) ->
                                                              Html.div
                                                                  [ prop.className "filter-option"
                                                                    prop.children
                                                                        [ Bulma.button.button
                                                                              [ button.isInverted
                                                                                color.isInfo
                                                                                prop.text m.Name
                                                                                prop.onPointerUp (fun _ ->
                                                                                    dispatch
                                                                                    <| NewCharacterNameOptionClicked
                                                                                        m.Index) ] ] ])
                                                      ) ]
                                            else
                                                Html.none

                                            ]
                                      ) ]
                                Html.div
                                    [ prop.className "flex-50"
                                      prop.children
                                          [ Bulma.label "Dexterity"
                                            Bulma.input.number
                                                [ prop.placeholder "Dexterity"
                                                  prop.name "characterDex"
                                                  prop.id "characterDex"
                                                  prop.required true
                                                  prop.onChange (fun ev -> dispatch (NewCharacterDexterityUpdated ev))
                                                  character.InitiativeModifier.ToString() |> prop.value ] ] ]
                                Html.div
                                    [ prop.className "flex-50"
                                      prop.children
                                          [ Bulma.label "Image URL"
                                            Bulma.input.text
                                                [ prop.placeholder "Image URL"
                                                  prop.name "characterImage"
                                                  prop.type' "text"
                                                  prop.id "characterImage"
                                                  prop.required true
                                                  prop.onChange (fun ev -> dispatch (NewCharacterImageUpdated ev))
                                                  prop.value character.ImageUrl ] ] ]
                                Html.div
                                    [ prop.className "flex-50"
                                      prop.children
                                          [ Bulma.label "Type"
                                            Bulma.select
                                                [ Bulma.select.isNormal
                                                  prop.onChange (fun ev -> dispatch (NewCharacterPlayerTypeUpdated ev))
                                                  prop.children
                                                      [ Html.option [ prop.value "player"; prop.text "Player" ]
                                                        Html.option [ prop.value "ally"; prop.text "Ally" ]
                                                        Html.option [ prop.value "enemy"; prop.text "Enemy" ] ] ] ] ]
                                Html.div
                                    [ prop.className "create-character-buttons"
                                      prop.children
                                          [ Bulma.button.button
                                                [ color.isDanger
                                                  prop.className "button--cancel"
                                                  prop.type' "button"
                                                  prop.text "Cancel"
                                                  prop.onPointerUp (fun _ -> dispatch NewCharacterCancelClicked) ]
                                            Bulma.button.button
                                                [ color.isInfo
                                                  prop.className "button--submit"
                                                  prop.type' "button"
                                                  prop.text "Create"
                                                  prop.onPointerUp (fun _ -> dispatch NewCharacterCreateClicked) ] ] ] ] ]
                | None -> Html.none
                if model.InitiativePopupOpen then
                    Html.div
                        [ prop.classes [ "roll-initiative" ]
                          prop.children (
                              [ Html.h1 [ prop.className "container-title"; prop.text "Roll Initiative" ] ]
                              @ (model.InitiativeViewModel.Scene.Combatants
                                 |> List.mapi (fun i c -> viewCharacterInitiativeEntry c i dispatch)
                                 |> List.concat)
                              @ [ Html.div
                                      [ prop.className "roll-initiative-buttons"
                                        prop.children
                                            [ Bulma.button.button
                                                  [ color.isDanger
                                                    prop.className "button--cancel"
                                                    prop.type' "button"
                                                    prop.text "Cancel"
                                                    prop.onPointerUp (fun _ -> dispatch RollInitiativeCancelClicked) ]
                                              if
                                                  model.InitiativeViewModel.Scene.Combatants
                                                  |> List.exists (fun c -> Option.isNone c.RolledInitiative)
                                              then
                                                  Bulma.button.button
                                                      [ color.isInfo
                                                        prop.className "button--submit"
                                                        prop.type' "button"
                                                        prop.text "Roll"
                                                        prop.onPointerUp (fun _ -> dispatch RollInitiativeRollClicked) ]
                                              else
                                                  Bulma.button.button
                                                      [ color.isInfo
                                                        prop.className "button--submit"
                                                        prop.type' "button"
                                                        prop.text "Begin Combat"
                                                        prop.onPointerUp (fun _ -> dispatch StartCombatClicked) ] ] ] ]
                          ) ]
                else
                    Html.none
                Html.div
                    [ prop.className "game-buttons"
                      prop.id "gameButtons"
                      prop.children
                          [ viewStateButton
                                CharacterSetup
                                model.InitiativeViewModel.Scene.GameState
                                "Add"
                                "addCharacter"
                                AddCharacterClicked
                                dispatch
                            if List.isEmpty model.InitiativeViewModel.Scene.Combatants |> not then
                                viewStateButton
                                    CharacterSetup
                                    model.InitiativeViewModel.Scene.GameState
                                    "Roll Initiatives"
                                    "rollInitiative"
                                    RollInitiativesClicked
                                    dispatch
                            else
                                Html.none
                            viewStateButton
                                Active
                                model.InitiativeViewModel.Scene.GameState
                                "End Turn"
                                "endTurn"
                                EndTurnClicked
                                dispatch
                            viewStateButton
                                Active
                                model.InitiativeViewModel.Scene.GameState
                                "Reset"
                                "reset"
                                ResetClicked
                                dispatch ] ] ] ]
