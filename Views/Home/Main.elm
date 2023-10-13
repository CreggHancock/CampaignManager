module Home.Main exposing (Flags, Model, Msg(..), init, initialModel, main, update, view)

import Accessibility as Html exposing (Html)
import Browser
import Html.Extra
import Http
import Json.Decode as Decode exposing (Decoder)
import Json.Decode.Pipeline exposing (requiredAt)
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
    }


type alias HttpConfig =
    { baseUrl : String
    , headers : List Http.Header
    }


type alias Model =
    { httpConfig : HttpConfig
    , errorMessage : Maybe String
    }


type alias Flags =
    { httpConfig : HttpConfig
    }


type Msg
    = Init Encode.Value
    | NoOp


update : Msg -> Model -> ( Model, Cmd Msg )
update msg model =
    case msg of
        Init flagsJson ->
            case Decode.decodeValue flagsDecoder flagsJson of
                Ok flags ->
                    ( { model
                        | httpConfig = flags.httpConfig
                      }
                    , Cmd.none
                    )

                Err error ->
                    ( { model | errorMessage = Just (Decode.errorToString error) }, Cmd.none )

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
        , Html.text "Hello World!"
        ]


flagsDecoder : Decoder Flags
flagsDecoder =
    Decode.succeed Flags
        |> requiredAt [] httpConfigDecoder


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
