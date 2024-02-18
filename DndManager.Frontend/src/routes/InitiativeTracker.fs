module InitiativeTracker

open Feliz
open Elmish
open Feliz.Bulma
open Fetch
open Thoth.Fetch
open Thoth.Json

[<Literal>]
let route = "InitiativeTracker"

type CombatantType =
    | Enemy
    | Ally
    | Player

type GameState =
    | CharacterSetup
    | InitiativeRolled
    | Active

type Combatant = {
    initiativeModifier: int
    name: string
    imageUrl: string
    playerType: CombatantType
    locationX: int
    locationY: int
    health: int
    maxHealth: int
}

type Scene = {
    id: int
    name: string
    description: string
    backgroundImage: string
    width: int
    height: int
    squareSize: int
    combatantTurn: int
    round: int
    combatants: Combatant List
    gameState: GameState
}

type InitiativeViewModel = { 
      scene : Scene
    }

type Model = {
    InitiativeViewModel: InitiativeViewModel
    NewCharacter: Combatant option
    ErrorMessage: string
    IsLoggedIn: bool
}

type Msg =
    | NoOp
    | AddCharacterClicked
    | RollInitiativesClicked
    | StartCombatClicked
    | EndTurnClicked
    | ResetClicked
    | NewCharacterNameUpdated of string
    | NewCharacterDexterityUpdated of string
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

let getInitiativeViewModel accessToken maybeSceneId =
    match maybeSceneId |> Option.map (fun (id) -> id.ToString ()) with
        | Some sceneId ->
            match accessToken with
                | Some token ->
                    let 
                        headers = [ HttpRequestHeaders.Authorization ("Bearer " + token) ]
                    Cmd.OfPromise.either
                            (fun () -> Fetch.get ("https://localhost:7068/Home/Get/" + sceneId, headers = headers))
                            ()
                            (fun (response) -> OnGetModelSuccess response)
                            OnGetModelError
                | None ->
                    (Cmd.OfFunc.either 
                        (fun () -> Browser.WebStorage.localStorage.getItem ("scenes"))
                        ()
                        (fun (sceneInfo) -> OnGotModelFromStorageSuccess (sceneInfo, sceneId)))
                        OnGotModelFromStorageError
        | None ->
            Cmd.none


let updateLocalStorage (model: Model, cmd: Cmd<Msg>) =
    let sceneResults = Decode.Auto.fromString<Scene List>(Browser.WebStorage.localStorage.getItem ("scenes"))
    match sceneResults with
        | Ok scenes ->
            let updatedScenes = 
                if List.exists (fun (s) -> s.id = model.InitiativeViewModel.scene.id) scenes then
                    List.map (fun (s) -> if s.id = model.InitiativeViewModel.scene.id then model.InitiativeViewModel.scene else s) scenes
                else
                    model.InitiativeViewModel.scene :: scenes
            model, Cmd.batch [ 
                cmd 
                Cmd.OfFunc.either
                    (fun () -> Browser.WebStorage.localStorage.setItem ("scenes", Encode.Auto.toString (0, updatedScenes) ))
                    ()
                    (fun () -> OnStorageUpdatedSuccess)
                    OnStorageUpdatedError
            ]
        | Error err ->
            { model with ErrorMessage = err }, cmd
    

let init accessToken sceneId = ({
        InitiativeViewModel = { 
            scene = {
                id = sceneId |> Option.defaultWith (fun () -> -1)
                combatants = []
                name = ""
                description = ""
                combatantTurn = 0
                round = 0
                width = 0
                height = 0
                squareSize = 0
                gameState = CharacterSetup
                backgroundImage = "https://rattrapgames.com/cdn/shop/products/RAT-MAT-GF001_1024x1024@2x.jpg?v=1576461000"
            }
        }
        NewCharacter = None
        ErrorMessage = ""
        IsLoggedIn = Option.isSome accessToken
    }
    , getInitiativeViewModel accessToken sceneId)

let updateScene model updater =
    { model with InitiativeViewModel = { scene = updater model.InitiativeViewModel.scene } }

let update (msg: Msg) (model: Model) =
    match msg with
    | NoOp -> model, Cmd.none
    | AddCharacterClicked -> { model with NewCharacter = Some { name = ""; initiativeModifier = 0; imageUrl = ""; playerType = CombatantType.Player; locationX = 0; locationY = 0; health = 0; maxHealth = 0 } }, Cmd.none
    | RollInitiativesClicked -> updateScene model (fun scene -> { scene with gameState = GameState.InitiativeRolled }), Cmd.none 
    | StartCombatClicked -> updateScene model (fun scene -> { scene with gameState = GameState.Active; combatantTurn = 0 }), Cmd.none 
    | EndTurnClicked -> updateScene model (fun scene -> { scene with gameState = GameState.Active; combatantTurn = scene.combatantTurn + 1 }), Cmd.none 
    | ResetClicked -> updateScene model  (fun scene -> { scene with gameState = GameState.CharacterSetup; combatantTurn = 0 }), Cmd.none
    | NewCharacterNameUpdated event -> { model with NewCharacter = Option.map (fun c -> { c with name = event } ) model.NewCharacter }, Cmd.none
    | NewCharacterDexterityUpdated event -> { model with NewCharacter = Option.map (fun c -> { c with initiativeModifier = int event } ) model.NewCharacter }, Cmd.none
    | NewCharacterImageUpdated event -> { model with NewCharacter = Option.map (fun c -> { c with imageUrl = event } ) model.NewCharacter }, Cmd.none
    | NewCharacterPlayerTypeUpdated event -> 
        match event with
            | "player" -> { model with NewCharacter = Option.map (fun c -> { c with playerType = CombatantType.Player } ) model.NewCharacter }, Cmd.none
            | "enemy" ->  { model with NewCharacter = Option.map (fun c -> { c with playerType = CombatantType.Enemy } ) model.NewCharacter }, Cmd.none
            | "ally" -> { model with NewCharacter = Option.map (fun c -> { c with playerType = CombatantType.Ally } ) model.NewCharacter }, Cmd.none
            | _ -> { model with NewCharacter = Option.map (fun c -> { c with playerType = CombatantType.Player } ) model.NewCharacter }, Cmd.none
    | NewCharacterCancelClicked -> { model with NewCharacter = None }, Cmd.none
    | NewCharacterCreateClicked ->
        match model.NewCharacter with
            | Some character ->
                let updatedScene = updateScene model (fun scene -> { scene with combatants = character :: scene.combatants})
                { updatedScene with NewCharacter = None }, Cmd.none
            | None ->
                model, Cmd.none
    | BackgroundUpdated background -> updateScene model (fun scene -> { scene with backgroundImage = background }) , Cmd.none
    | OnGetModelSuccess viewModel -> model, Cmd.none
    | OnGetModelError err -> { model with ErrorMessage = err.ToString() }, Cmd.none
    | OnGotModelFromStorageSuccess (scenesString, sceneId) ->
        let sceneResults = Decode.Auto.fromString<Scene List>(scenesString)
        match sceneResults with
            | Ok scenes ->
                updateScene model (fun scene -> (scenes |> List.tryFind (fun (sceneOption) -> sceneOption.id.ToString() = sceneId) |> Option.defaultWith (fun () -> scene))), Cmd.none
            | Error err ->
                { model with ErrorMessage = err }, Cmd.none
    | OnGotModelFromStorageError err -> { model with ErrorMessage = err.ToString() }, Cmd.none
        


let viewPlayerCard player dispatch =
    let getClassFromPlayer playerType = 
        match playerType with
        | Enemy -> "enemy"
        | Ally -> "ally"
        | Player -> "player"

            
    Html.div [
        prop.id "default1"
        prop.className $"initiative-card {getClassFromPlayer player.playerType}"
        prop.children [
            Html.img [
                prop.src player.imageUrl
                prop.title player.name
            ]
            Html.div [
                prop.className "character-details"
                prop.children [
                    Html.div [
                        prop.className "stats dex"
                        prop.children [
                            Html.span [
                                prop.className "stat-label"
                                prop.text "Dex."
                            ]
                            Html.span [
                                prop.className "stat-mod"
                                prop.text $"{player.initiativeModifier}" 
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]

let viewPlayerToken player isActive dispatch =
    let getClassFromPlayer playerType = 
        match playerType with
        | Enemy -> "enemy"
        | Ally -> "ally"
        | Player -> "player"

            
    Html.div [
        prop.id "default1"
        prop.className $"""map-token initiative-card {getClassFromPlayer player.playerType} {if isActive then "active" else ""}"""
        prop.children [
            Html.img [
                prop.src player.imageUrl
                prop.title player.name
            ]
            Html.span [
                prop.className "character-name"
                prop.text player.name

            ]
        ]
    ]

let viewStateButton neededGameState currentGameState (buttonText: string) (buttonId: string) onClickMsg dispatch =
    Bulma.button.button [
        prop.text buttonText
        prop.classes [ "button"; if currentGameState = neededGameState then "" else "button-hidden" ]
        prop.type' "button"
        prop.id buttonId
        prop.onClick (fun _ -> if currentGameState = neededGameState then dispatch onClickMsg else dispatch NoOp)
    ]
                       

let view model dispatch =
    Html.div [
        prop.className "game-board"
        prop.children [
            Html.select [
                prop.id "map-select"
                prop.className "map-select"
                prop.onChange (fun ev -> dispatch (BackgroundUpdated ev))
                prop.children [
                    Html.option [
                        prop.value "https://rattrapgames.com/cdn/shop/products/RAT-MAT-GF001_1024x1024@2x.jpg?v=1576461000"
                        prop.text "Field"
                    ]
                    Html.option [
                        prop.value "https://images-wixmp-ed30a86b8c4ca887773594c2.wixmp.com/f/5ae20baf-98de-45dd-b5fb-fb5fe71856ff/dfpohu9-30e97ff5-b72c-46bb-9f32-3f37646858cd.jpg?token=eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJzdWIiOiJ1cm46YXBwOjdlMGQxODg5ODIyNjQzNzNhNWYwZDQxNWVhMGQyNmUwIiwiaXNzIjoidXJuOmFwcDo3ZTBkMTg4OTgyMjY0MzczYTVmMGQ0MTVlYTBkMjZlMCIsIm9iaiI6W1t7InBhdGgiOiJcL2ZcLzVhZTIwYmFmLTk4ZGUtNDVkZC1iNWZiLWZiNWZlNzE4NTZmZlwvZGZwb2h1OS0zMGU5N2ZmNS1iNzJjLTQ2YmItOWYzMi0zZjM3NjQ2ODU4Y2QuanBnIn1dXSwiYXVkIjpbInVybjpzZXJ2aWNlOmZpbGUuZG93bmxvYWQiXX0.ESuTEe8KcpxBfKkGCZrOymAwEDFRDJm2y4QGjxj2eHE"
                        prop.text "House"
                    ]
                    Html.option [
                        prop.value "https://i.pinimg.com/originals/e6/4b/b2/e64bb20325420e33f58b4f7458ae2e66.jpg"
                        prop.text "Camp"
                    ]
                    Html.option [
                        prop.value "https://i.redd.it/rx669zmdv8q61.jpg"
                        prop.text "Vallaki"
                    ]
                ]
            ]
            Html.div [
                prop.className "game-map"
                prop.id "battleMap"
                (Html.img [
                    prop.id "draggable-image"
                    prop.className "map-image"
                    prop.src model.InitiativeViewModel.scene.backgroundImage
                ]) :: List.mapi (fun ind player -> viewPlayerToken player (model.InitiativeViewModel.scene.combatantTurn = ind) dispatch) model.InitiativeViewModel.scene.combatants
                    |> prop.children
            ]
            Html.div [
                prop.className "hud-wrapper"
                prop.children [
                    Html.div [
                        prop.className "float-screen"
                        prop.children [
                            Html.div [
                                prop.className "initiative"
                                prop.id "initiativeRotation"
                                List.map (fun player -> viewPlayerCard player dispatch) model.InitiativeViewModel.scene.combatants |> prop.children
                            ]
                        ]
                    ]
                ]
            ]
            Html.div [
                prop.classes [ "create-character"; if Option.isSome model.NewCharacter then "" else "element-hidden"]
                prop.id "createCharacterForm"
                prop.children [
                    Html.h1 [
                        prop.className "container-title"
                        prop.text "Add Character"
                    ]
                    Html.div [
                        prop.className "flex-50"
                        prop.children [
                            Bulma.input.text [
                                prop.placeholder "Name"
                                prop.name "characterName"
                                prop.type' "text"
                                prop.id "characterName"
                                prop.required true
                                prop.onChange (fun ev -> dispatch (NewCharacterNameUpdated ev))
                            ]
                        ]
                    ]
                    Html.div [
                        prop.className "flex-50"
                        prop.children [
                            Bulma.input.text [
                                prop.placeholder "Dexterity"
                                prop.name "characterDex"
                                prop.type' "text"
                                prop.id "characterDex"
                                prop.required true
                                prop.onChange (fun ev -> dispatch (NewCharacterDexterityUpdated ev))
                            ]
                        ]
                    ]
                    Html.div [
                        prop.className "flex-50"
                        prop.children [
                            Bulma.input.text [
                                prop.placeholder "Image URL"
                                prop.name "characterImage"
                                prop.type' "text"
                                prop.id "characterImage"
                                prop.required true
                                prop.onChange (fun ev -> dispatch (NewCharacterImageUpdated ev))
                            ]
                        ]
                    ]
                    Html.div [
                        prop.className "flex-50"
                        prop.children [
                            Html.select [
                                prop.onChange (fun ev -> dispatch (NewCharacterPlayerTypeUpdated ev))
                                prop.children [
                                    Html.option [
                                        prop.value "player"
                                        prop.text "Player"
                                    ]
                                    Html.option [
                                        prop.value "ally"
                                        prop.text "Ally"
                                    ]
                                    Html.option [
                                        prop.value "enemy"
                                        prop.text "Enemy"
                                    ]
                                ]
                            ]
                        ]
                    ]
                    Html.div [
                        prop.className "create-character-buttons"
                        prop.children [
                            Bulma.button.button [
                                prop.className "button--cancel"
                                prop.type' "button"
                                prop.text "Cancel"
                                prop.onClick (fun _ -> dispatch NewCharacterCancelClicked)
                            ]
                            Bulma.button.button [
                                prop.className "button--submit"
                                prop.type' "button"
                                prop.text "Create"
                                prop.onClick (fun _ -> dispatch NewCharacterCreateClicked)
                            ]
                        ]
                    ]
                ]
            ]
            Html.div [
                prop.className "game-buttons"
                prop.id "gameButtons"
                prop.children [
                    viewStateButton GameState.CharacterSetup model.InitiativeViewModel.scene.gameState "Add" "addCharacter" AddCharacterClicked dispatch
                    viewStateButton GameState.CharacterSetup model.InitiativeViewModel.scene.gameState "Roll Initiatives" "rollInitiative" RollInitiativesClicked dispatch
                    viewStateButton GameState.InitiativeRolled model.InitiativeViewModel.scene.gameState "Start Combat" "startCombat" StartCombatClicked dispatch
                    viewStateButton GameState.Active model.InitiativeViewModel.scene.gameState "End Turn" "endTurn" EndTurnClicked dispatch
                    viewStateButton GameState.Active model.InitiativeViewModel.scene.gameState "Reset" "reset" ResetClicked dispatch
                ]
            ]
        ]
    ]