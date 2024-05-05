module Scene

type CombatantType =
    | Enemy
    | Ally
    | Player

type GameState =
    | CharacterSetup
    | Active

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
      ShowGrid: bool
      GridSize: int }
