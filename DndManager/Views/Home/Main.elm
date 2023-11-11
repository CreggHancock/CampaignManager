module Home.Main exposing (Flags, Model, Msg(..), init, initialModel, main, update, view)

import Accessibility as Html exposing (Html)
import Browser
import Common.Character as Character exposing (Character, characterDecoder)
import Html.Attributes as Attr exposing (href)
import Html.Events exposing (onClick)
import Html.Extra
import Http
import Json.Decode as Decode exposing (Decoder)
import Json.Decode.Pipeline exposing (required, requiredAt)
import Json.Encode as Encode


main : Program Encode.Value Model Msg
main =
    Browser.element
        { init = init
        , view = view
        , update = update
        , subscriptions = \_ -> Sub.none
        }


init : Encode.Value -> ( Model, Cmd Msg )
init flags =
    update (Init flags) initialModel


initialModel : Model
initialModel =
    { httpConfig = { baseUrl = "", headers = [] }
    , errorMessage = Nothing
    , userCharacters = []
    , isLoggedIn = False
    }


type alias HttpConfig =
    { baseUrl : String
    , headers : List Http.Header
    }


type alias Model =
    { httpConfig : HttpConfig
    , errorMessage : Maybe String
    , userCharacters : List Character
    , isLoggedIn : Bool
    }


type alias Flags =
    { httpConfig : HttpConfig
    , userCharacters : List Character
    , isLoggedIn : Bool
    }


type Msg
    = Init Encode.Value
    | OnCreateClicked
    | NoOp


update : Msg -> Model -> ( Model, Cmd Msg )
update msg model =
    case msg of
        Init flagsJson ->
            case Decode.decodeValue flagsDecoder flagsJson of
                Ok flags ->
                    ( { model
                        | httpConfig = flags.httpConfig
                        , userCharacters = flags.userCharacters
                        , isLoggedIn = flags.isLoggedIn
                      }
                    , Cmd.none
                    )

                Err error ->
                    ( { model | errorMessage = Just (Decode.errorToString error) }, Cmd.none )

        OnCreateClicked ->
            ( model, Cmd.none )

        NoOp ->
            ( model, Cmd.none )


view : Model -> Html Msg
view model =
    Html.div []
        [ case model.errorMessage of
            Just errorMessage ->
                Html.div [] [ Html.text errorMessage ]

            Nothing ->
                Html.Extra.nothing
        , viewCharacters model
        , viewParties model
        ]


viewCharacters : Model -> Html Msg
viewCharacters model =
    Html.div []
        [ Html.h2 [] [ Html.text "My Characters" ]
        , if not model.isLoggedIn then
            Html.span [] [ Html.text "Please log in to start making characters." ]

          else
            Html.ul []
                (model.userCharacters
                    |> List.map
                        (\c ->
                            if Character.isEmpty c then
                                Html.li [] [ Html.a [ href <| model.httpConfig.baseUrl ++ "/CharacterSheet" ] [ Html.text "Create Character" ] ]

                            else
                                Html.li [] [ Html.a [ href <| model.httpConfig.baseUrl ++ "/CharacterSheet?id=" ++ String.fromInt c.id ] [ Html.text c.name ] ]
                        )
                )
        ]


viewParties : Model -> Html Msg
viewParties model =
    Html.div []
        [ Html.h2 [] [ Html.text "My Parties" ]
        , if not model.isLoggedIn then
            Html.span [] [ Html.text "Please log in to manage your parties." ]

          else
            Html.span []
                [ Html.button [ Attr.type_ "button", onClick NoOp ] [ Html.text "Join A Party" ]
                , Html.button [ Attr.type_ "button", onClick NoOp ] [ Html.text "Create A Party" ]
                ]
        ]


flagsDecoder : Decoder Flags
flagsDecoder =
    Decode.succeed Flags
        |> requiredAt [] httpConfigDecoder
        |> required "userCharacters" (Decode.list characterDecoder)
        |> required "isLoggedIn" Decode.bool


httpConfigDecoder : Decoder HttpConfig
httpConfigDecoder =
    Decode.map2 HttpConfig
        (Decode.field "baseUrl" Decode.string)
        (Decode.field "headers" httpHeaderDecoder)


httpHeaderDecoder : Decode.Decoder (List Http.Header)
httpHeaderDecoder =
    let
        decoder : List ( String, String ) -> List Http.Header
        decoder headers =
            headers
                |> List.map (\( key, value ) -> Http.header key value)
    in
    Decode.keyValuePairs Decode.string
        |> Decode.map decoder
