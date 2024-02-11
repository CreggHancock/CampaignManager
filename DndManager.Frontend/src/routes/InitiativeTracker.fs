module InitiativeTracker

open Feliz
open Elmish
open Feliz.Bulma
open Fetch
open Thoth.Fetch

[<Literal>]
let route = "InitiativeTracker"

type PlayerType =
    | Enemy
    | Ally
    | Player

type GameState =
    | CharacterSetup
    | InitiativeRolled
    | Active

type Player = {
    initiativeModifier: int
    name: string
    imageUrl: string
    playerType: PlayerType
}

type Model = { 
    Count: int
    Players: Player List
    PlayerTurn: int
    NewCharacter: Player option
    GameState: GameState
    BackgroundUrl: string
}

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
    | OnGetModelSuccess of HomeViewModel
    | OnGetModelError of exn

let getInitiativeViewModel accessToken =
    let 
        headers =
            match accessToken with
                | Some token ->
                    [ HttpRequestHeaders.Authorization ("Bearer " + token) ]
                | None ->
                    []
    Cmd.OfPromise.either
            (fun () -> Fetch.get ("https://localhost:7068/Home/Get", headers = headers))
            ()
            (fun response -> OnGetModelSuccess response)
            OnGetModelError

let init accessToken = ({
        Count = 0 
        Players = []
        PlayerTurn = 0
        NewCharacter = None
        GameState = CharacterSetup
        BackgroundUrl = "https://rattrapgames.com/cdn/shop/products/RAT-MAT-GF001_1024x1024@2x.jpg?v=1576461000"
    }
    , getInitiativeViewModel accessToken)

let update (msg: Msg) (model: Model) =
    match msg with
    | NoOp -> model, Cmd.none
    | AddCharacterClicked -> { model with NewCharacter = Some { name = ""; initiativeModifier = 0; imageUrl = ""; playerType = PlayerType.Player } }, Cmd.none
    | RollInitiativesClicked -> { model with GameState = GameState.InitiativeRolled }, Cmd.none 
    | StartCombatClicked -> { model with GameState = GameState.Active; PlayerTurn = 0 }, Cmd.none 
    | EndTurnClicked -> { model with GameState = GameState.Active; PlayerTurn = model.PlayerTurn + 1 }, Cmd.none 
    | ResetClicked -> { model with GameState = GameState.CharacterSetup; PlayerTurn = 0 }, Cmd.none
    | NewCharacterNameUpdated event -> { model with NewCharacter = Option.map (fun c -> { c with name = event } ) model.NewCharacter }, Cmd.none
    | NewCharacterDexterityUpdated event -> { model with NewCharacter = Option.map (fun c -> { c with initiativeModifier = int event } ) model.NewCharacter }, Cmd.none
    | NewCharacterImageUpdated event -> { model with NewCharacter = Option.map (fun c -> { c with imageUrl = event } ) model.NewCharacter }, Cmd.none
    | NewCharacterPlayerTypeUpdated event -> 
        match event with
            | "player" -> { model with NewCharacter = Option.map (fun c -> { c with playerType = PlayerType.Player } ) model.NewCharacter }, Cmd.none
            | "enemy" ->  { model with NewCharacter = Option.map (fun c -> { c with playerType = PlayerType.Enemy } ) model.NewCharacter }, Cmd.none
            | "ally" -> { model with NewCharacter = Option.map (fun c -> { c with playerType = PlayerType.Ally } ) model.NewCharacter }, Cmd.none
            | _ -> { model with NewCharacter = Option.map (fun c -> { c with playerType = PlayerType.Player } ) model.NewCharacter }, Cmd.none
    | NewCharacterCancelClicked -> { model with NewCharacter = None }, Cmd.none
    | NewCharacterCreateClicked ->
        match model.NewCharacter with
            | Some character ->
                { model with Players = character :: model.Players; NewCharacter = None }, Cmd.none
            | None ->
                model, Cmd.none
    | BackgroundUpdated background -> { model with BackgroundUrl = background }, Cmd.none
    | OnGetModelSuccess viewModel -> model, Cmd.none
    | OnGetModelError err -> model, Cmd.none


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
                    prop.src model.BackgroundUrl
                ]) :: List.mapi (fun ind player -> viewPlayerToken player (model.PlayerTurn = ind) dispatch) model.Players
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
                                List.map (fun player -> viewPlayerCard player dispatch) model.Players |> prop.children
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
                    viewStateButton GameState.CharacterSetup model.GameState "Add" "addCharacter" AddCharacterClicked dispatch
                    viewStateButton GameState.CharacterSetup model.GameState "Roll Initiatives" "rollInitiative" RollInitiativesClicked dispatch
                    viewStateButton GameState.InitiativeRolled model.GameState "Start Combat" "startCombat" StartCombatClicked dispatch
                    viewStateButton GameState.Active model.GameState "End Turn" "endTurn" EndTurnClicked dispatch
                    viewStateButton GameState.Active model.GameState "Reset" "reset" ResetClicked dispatch
                ]
            ]
        ]
    ]