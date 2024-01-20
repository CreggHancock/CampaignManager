module Common.Scene exposing (Ability, Scene, SceneClass, InventoryItem, ProficiencyBonus, SkillType, Spell, SpellSlot, StatType, encodeScene, sceneDecoder, empty, isEmpty)

import Json.Decode as Decode exposing (Decoder)
import Json.Decode.Pipeline exposing (required)
import Json.Encode as Encode


type alias Scene =
    { id : Int
    , userId : String
    , level : Int
    , health : Int
    , maxHealth : Int
    , tempHealth : Int
    , gold : Int
    , silver : Int
    , copper : Int
    , electrum : Int
    , platinum : Int
    , name : String
    , description : String
    , background : String
    , race : String
    , alignment : String
    , experiencePoints : Int
    , initiativeBonus : Int
    , proficiencyBonus : Int
    , speed : Int
    , hitDice : String
    , hasInspiration : Bool
    , strength : Int
    , dexterity : Int
    , constitution : Int
    , intelligence : Int
    , wisdom : Int
    , charisma : Int
    , sceneClasses : List SceneClass
    , abilities : List Ability
    , inventoryItems : List InventoryItem
    , proficiencyBonuses : List ProficiencyBonus
    , spells : List Spell
    , spellSlots : List SpellSlot
    , languages : List Language
    }

type alias SceneClass =
    { name : String
    , level : Int
    }


type alias Ability =
    { maxUses : Int
    , remainingUses : Int
    , name : String
    , description : String
    }


type alias InventoryItem =
    { name : String
    }


type alias ProficiencyBonus =
    { skillType : SkillType
    , statType : StatType
    }


type alias Spell =
    { level : Int
    , name : String
    }


type alias SpellSlot =
    { level : Int
    , remainingUses : Int
    , maxUses : Int
    }


type alias Language =
    { name : String
    }


type StatType
    = Strength
    | Dexterity
    | Constitution
    | Intelligence
    | Wisdom
    | Charisma


type SkillType
    = Acrobatics
    | AnimalHandling
    | Arcana
    | Athletics
    | Deception
    | History
    | Insight
    | Intimidation
    | Investigation
    | Medicine
    | Nature
    | Perception
    | Performance
    | Persuasion
    | Religion
    | SleightOfHand
    | Stealth
    | Survival


isEmpty : Scene -> Bool
isEmpty scene =
    scene.id == 0


empty : Scene
empty =
    Scene 0 "" 0 0 0 0 0 0 0 0 0 "" "" "" "" "" 0 0 0 0 "" False 0 0 0 0 0 0 [] [] [] [] [] [] []


encodeScene : Scene -> Encode.Value
encodeScene scene =
    Encode.object
        [ ("id", Encode.int scene.id )
        , ("userId", Encode.string scene.userId )
        , ( "level", Encode.int scene.level )
        , ( "health", Encode.int scene.health )
        , ( "maxHealth", Encode.int scene.maxHealth )
        , ( "tempHealth", Encode.int scene.tempHealth )
        , ( "gold", Encode.int scene.gold )
        , ( "silver", Encode.int scene.silver )
        , ( "copper", Encode.int scene.copper )
        , ( "electrum", Encode.int scene.electrum )
        , ( "platinum", Encode.int scene.platinum )
        , ( "name", Encode.string scene.name )
        , ( "description", Encode.string scene.description )
        , ( "race", Encode.string scene.race )
        , ( "alignment", Encode.string scene.alignment )
        , ( "experiencePoints", Encode.int scene.experiencePoints )
        , ( "initiativeBonus", Encode.int scene.initiativeBonus )
        , ( "proficiencyBonus", Encode.int scene.proficiencyBonus )
        , ( "speed", Encode.int scene.speed )
        , ( "hitDice", Encode.string scene.hitDice )
        , ( "hasInspiration", Encode.bool scene.hasInspiration )
        ]

encoderSceneClass : SceneClass -> Encode.Value
encoderSceneClass sceneClass =
    Encode.object
        [ ( "name", Encode.string sceneClass.name )
        , ( "level", Encode.int sceneClass.level )
        ]

encoderAbility : Ability -> Encode.Value
encoderAbility ability =
    Encode.object
        [ ( "maxUses", Encode.int ability.maxUses )
        , ( "remainingUses", Encode.int ability.remainingUses )
        , ( "name", Encode.string ability.name )
        , ( "description", Encode.string ability.description )
        ]

encoderInventoryItem : InventoryItem -> Encode.Value
encoderInventoryItem inventoryItem =
    Encode.object
        [ ( "name", Encode.string inventoryItem.name )
        ]

encodeProficiencyBonus : ProficiencyBonus -> Encode.Value
encodeProficiencyBonus proficiencyBonus =
    Encode.object
        [ ( "skillType", encodeSkillType proficiencyBonus.skillType )
        , ( "statType", encodeStatType proficiencyBonus.statType )
        ]

encodeSpell : Spell -> Encode.Value
encodeSpell spell =
    Encode.object
        [ ("level", Encode.int spell.level)
        , ( "name", Encode.string spell.name)
        ]

encodeSpellSlot : SpellSlot -> Encode.Value
encodeSpellSlot spellSlot =
    Encode.object
        [ ( "level", Encode.int spellSlot.level )
        , ( "remainingUses", Encode.int spellSlot.remainingUses )
        , ( "maxUses", Encode.int spellSlot.maxUses )
        ]

encodeLanguage : Language -> Encode.Value
encodeLanguage language =
    Encode.object
        [ ("name", Encode.string language.name )
        ]

encodeStatType : StatType -> Encode.Value
encodeStatType statType =
    Encode.string
        <| 
            (case statType of
                Strength ->
                    "Strength"

                Dexterity ->
                    "Dexterity"

                Constitution ->
                    "Constitution"

                Intelligence ->
                    "Intelligence"

                Wisdom ->
                    "Wisdom"

                Charisma ->
                    "Charisma"
            )

encodeSkillType : SkillType -> Encode.Value
encodeSkillType skillType =
    Encode.string
        <|
            (
                case skillType of
                    Acrobatics ->
                        "Acrobatics"

                    AnimalHandling ->
                        "AnimalHandling"

                    Arcana ->
                        "Arcana"

                    Athletics ->
                        "Athletics"

                    Deception ->
                        "Deception"

                    History ->
                        "History"

                    Insight ->
                        "Insight"

                    Intimidation ->
                        "Intimidation"

                    Investigation ->
                        "Investigation"

                    Medicine ->
                        "Medicine"

                    Nature ->
                        "Nature"

                    Perception ->
                        "Perception"

                    Performance ->
                        "Performance"

                    Persuasion ->
                        "Persuasion"

                    Religion ->
                        "Religion"

                    SleightOfHand ->
                        "SleightOfHand"

                    Stealth ->
                        "Stealth"

                    Survival ->
                        "Survival"
            )

sceneDecoder : Decoder Scene
sceneDecoder =
    Decode.succeed Scene
        |> required "id" Decode.int
        |> required "userId" Decode.string
        |> required "level" Decode.int
        |> required "health" Decode.int
        |> required "maxHealth" Decode.int
        |> required "tempHealth" Decode.int
        |> required "gold" Decode.int
        |> required "silver" Decode.int
        |> required "copper" Decode.int
        |> required "electrum" Decode.int
        |> required "platinum" Decode.int
        |> required "name" Decode.string
        |> required "description" Decode.string
        |> required "background" Decode.string
        |> required "race" Decode.string
        |> required "alignment" Decode.string
        |> required "experiencePoints" Decode.int
        |> required "initiativeBonus" Decode.int
        |> required "proficiencyBonus" Decode.int
        |> required "speed" Decode.int
        |> required "hitDice" Decode.string
        |> required "hasInspiration" Decode.bool
        |> required "strength" Decode.int
        |> required "dexterity" Decode.int
        |> required "constitution" Decode.int
        |> required "intelligence" Decode.int
        |> required "wisdom" Decode.int
        |> required "charisma" Decode.int
        |> required "sceneClasses" (Decode.list sceneClassDecoder)
        |> required "abilities" (Decode.list abilityDecoder)
        |> required "inventoryItems" (Decode.list inventoryItemDecoder)
        |> required "proficiencyBonuses" (Decode.list proficiencyBonusDecoder)
        |> required "spells" (Decode.list spellDecoder)
        |> required "spellSlots" (Decode.list spellSlotDecoder)
        |> required "languages" (Decode.list lanugageDecoder)


sceneClassDecoder : Decoder SceneClass
sceneClassDecoder =
    Decode.succeed SceneClass
        |> required "name" Decode.string
        |> required "level" Decode.int


abilityDecoder : Decoder Ability
abilityDecoder =
    Decode.succeed Ability
        |> required "maxUses" Decode.int
        |> required "remainingUses" Decode.int
        |> required "name" Decode.string
        |> required "description" Decode.string


inventoryItemDecoder : Decoder InventoryItem
inventoryItemDecoder =
    Decode.succeed InventoryItem
        |> required "name" Decode.string


proficiencyBonusDecoder : Decoder ProficiencyBonus
proficiencyBonusDecoder =
    Decode.succeed ProficiencyBonus
        |> required "skillType" skillTypeDecoder
        |> required "statType" statTypeDecoder


spellSlotDecoder : Decoder SpellSlot
spellSlotDecoder =
    Decode.succeed SpellSlot
        |> required "level" Decode.int
        |> required "remainingUses" Decode.int
        |> required "maxUses" Decode.int


spellDecoder : Decoder Spell
spellDecoder =
    Decode.succeed Spell
        |> required "level" Decode.int
        |> required "name" Decode.string


lanugageDecoder : Decoder Language
lanugageDecoder =
    Decode.succeed Language
        |> required "name" Decode.string


statTypeDecoder : Decoder StatType
statTypeDecoder =
    Decode.string
        |> Decode.map
            (\statType ->
                case statType of
                    "Strength" ->
                        Strength

                    "Dexterity" ->
                        Dexterity

                    "Constitution" ->
                        Constitution

                    "Intelligence" ->
                        Intelligence

                    "Wisdom" ->
                        Wisdom

                    "Charisma" ->
                        Charisma

                    _ ->
                        Strength
            )


skillTypeDecoder : Decoder SkillType
skillTypeDecoder =
    Decode.string
        |> Decode.map
            (\skillType ->
                case skillType of
                    "Acrobatics" ->
                        Acrobatics

                    "AnimalHandling" ->
                        AnimalHandling

                    "Arcana" ->
                        Arcana

                    "Athletics" ->
                        Athletics

                    "Deception" ->
                        Deception

                    "History" ->
                        History

                    "Insight" ->
                        Insight

                    "Intimidation" ->
                        Intimidation

                    "Investigation" ->
                        Investigation

                    "Medicine" ->
                        Medicine

                    "Nature" ->
                        Nature

                    "Perception" ->
                        Perception

                    "Performance" ->
                        Performance

                    "Persuasion" ->
                        Persuasion

                    "Religion" ->
                        Religion

                    "SleightOfHand" ->
                        SleightOfHand

                    "Stealth" ->
                        Stealth

                    "Survival" ->
                        Survival

                    _ ->
                        Acrobatics
            )
