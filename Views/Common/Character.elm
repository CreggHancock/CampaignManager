module Common.Character exposing (Ability, Character, CharacterClass, InventoryItem, ProficiencyBonus, SkillType, Spell, SpellSlot, StatType, characterDecoder, empty, isEmpty)

import Json.Decode as Decode exposing (Decoder)
import Json.Decode.Pipeline exposing (required)


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
    character.id == -1


empty : Character
empty =
    Character -1 "" 0 0 0 0 0 0 0 0 0 "" "" "" "" 0 0 0 "" False [] [] [] [] [] []


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
