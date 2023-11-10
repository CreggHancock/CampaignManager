module Common.Character exposing (Ability, Character, CharacterClass, InventoryItem, ProficiencyBonus, SkillType, Spell, SpellSlot, StatType, encodeCharacter, characterDecoder, empty, isEmpty)

import Json.Decode as Decode exposing (Decoder)
import Json.Decode.Pipeline exposing (required)
import Json.Encode as Encode


type alias Character =
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
    , race : String
    , alignment : String
    , experiencePoints : Int
    , initiativeBonus : Int
    , proficiencyBonus : Int
    , speed : Int
    , hitDice : String
    , hasInspiration : Bool
    , characterClasses : List CharacterClass
    , abilities : List Ability
    , inventoryItems : List InventoryItem
    , proficiencyBonuses : List ProficiencyBonus
    , spells : List Spell
    , spellSlots : List SpellSlot
    }

type alias CharacterClass =
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


isEmpty : Character -> Bool
isEmpty character =
    character.id == 0


empty : Character
empty =
    Character 0 "" 0 0 0 0 0 0 0 0 0 "" "" "" "" 0 0 0 0 "" False [] [] [] [] [] []


encodeCharacter : Character -> Encode.Value
encodeCharacter character =
    Encode.object
        [ ("id", Encode.int character.id )
        , ("userId", Encode.string character.userId )
        , ( "level", Encode.int character.level )
        , ( "health", Encode.int character.health )
        , ( "maxHealth", Encode.int character.maxHealth )
        , ( "tempHealth", Encode.int character.tempHealth )
        , ( "gold", Encode.int character.gold )
        , ( "silver", Encode.int character.silver )
        , ( "copper", Encode.int character.copper )
        , ( "electrum", Encode.int character.electrum )
        , ( "platinum", Encode.int character.platinum )
        , ( "name", Encode.string character.name )
        , ( "description", Encode.string character.description )
        , ( "race", Encode.string character.race )
        , ( "alignment", Encode.string character.alignment )
        , ( "experiencePoints", Encode.int character.experiencePoints )
        , ( "initiativeBonus", Encode.int character.initiativeBonus )
        , ( "proficiencyBonus", Encode.int character.proficiencyBonus )
        , ( "speed", Encode.int character.speed )
        , ( "hitDice", Encode.string character.hitDice )
        , ( "hasInspiration", Encode.bool character.hasInspiration )
        ]

encoderCharacterClass : CharacterClass -> Encode.Value
encoderCharacterClass characterClass =
    Encode.object
        [ ( "name", Encode.string characterClass.name )
        , ( "level", Encode.int characterClass.level )
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

characterDecoder : Decoder Character
characterDecoder =
    Decode.succeed Character
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
        |> required "race" Decode.string
        |> required "alignment" Decode.string
        |> required "experiencePoints" Decode.int
        |> required "initiativeBonus" Decode.int
        |> required "proficiencyBonus" Decode.int
        |> required "speed" Decode.int
        |> required "hitDice" Decode.string
        |> required "hasInspiration" Decode.bool
        |> required "characterClasses" (Decode.list characterClassDecoder)
        |> required "abilities" (Decode.list abilityDecoder)
        |> required "inventoryItems" (Decode.list inventoryItemDecoder)
        |> required "proficiencyBonuses" (Decode.list proficiencyBonusDecoder)
        |> required "spells" (Decode.list spellDecoder)
        |> required "spellSlots" (Decode.list spellSlotDecoder)


characterClassDecoder : Decoder CharacterClass
characterClassDecoder =
    Decode.succeed CharacterClass
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
