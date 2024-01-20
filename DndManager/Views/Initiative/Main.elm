module Initiative.Main exposing (Flags, Model, Msg(..), init, initialModel, main, update, view)

import Accessibility as Html exposing (Html)
import Browser
import Browser.Navigation exposing (load)
import Common.Scene as Scene exposing (Scene, sceneDecoder)
import Html as ElmHtml
import Html.Attributes as Attr
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
    , scene = Scene.empty
    }


type alias HttpConfig =
    { baseUrl : String
    , headers : List Http.Header
    }


type alias Model =
    { httpConfig : HttpConfig
    , errorMessage : Maybe String
    , scene : Scene
    }


type alias Flags =
    { httpConfig : HttpConfig
    , scene : Scene
    }


type Msg
    = Init Encode.Value
    | UpdateScene
    | SceneUpdated (Result Http.Error Scene)
    | DeleteScene
    | SceneDeleted (Result Http.Error ())
    | NoOp


update : Msg -> Model -> ( Model, Cmd Msg )
update msg model =
    case msg of
        Init flagsJson ->
            case Decode.decodeValue flagsDecoder flagsJson of
                Ok flags ->
                    ( { model
                        | httpConfig = flags.httpConfig
                        , scene = flags.scene
                      }
                    , Cmd.none
                    )

                Err error ->
                    ( { model | errorMessage = Just (Decode.errorToString error) }, Cmd.none )

        UpdateScene ->
            ( model, updateScene model.scene model.httpConfig )

        SceneUpdated sceneResult ->
            case sceneResult of
                Ok scene ->
                    ( { model | scene = scene }, Cmd.none )

                Err err ->
                    ( { model | errorMessage = Just "Failed to update scene" }, Cmd.none )

        DeleteScene ->
            ( model, deleteScene model.scene.id model.httpConfig )

        SceneDeleted deleteResult ->
            case deleteResult of
                Ok _ ->
                    ( model, load "/" )

                Err err ->
                    ( { model | errorMessage = Just "Failed to delete scene" }, Cmd.none )

        NoOp ->
            ( model, Cmd.none )


view : Model -> Html Msg
view model =
    let
        scene =
            model.scene
    in
    Html.div [ Attr.class "main-wrap" ]
        [ case model.errorMessage of
            Just errorMessage ->
                Html.div [] [ Html.text errorMessage ]

            Nothing ->
                Html.Extra.nothing
        , Html.div [ Attr.class "charsheet", Attr.id "charDetails" ] [ Html.text character.]
        ]


flagsDecoder : Decoder Flags
flagsDecoder =
    Decode.succeed Flags
        |> requiredAt [] httpConfigDecoder
        |> required "scene" sceneDecoder


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


updateScene : Scene -> HttpConfig -> Cmd Msg
updateScene scene config =
    let
        body =
            Scene.encodeScene scene
    in
    Http.request
        { method = "POST"
        , headers = config.headers
        , url = config.baseUrl ++ "/Scene/Update"
        , body = Http.jsonBody body
        , expect = Http.expectJson SceneUpdated sceneDecoder
        , timeout = Nothing
        , tracker = Nothing
        }


deleteScene : Int -> HttpConfig -> Cmd Msg
deleteScene sceneId config =
    let
        body =
            Encode.int sceneId
    in
    Http.request
        { method = "DELETE"
        , headers = config.headers
        , url = config.baseUrl ++ "/Scene/Delete"
        , body = Http.jsonBody body
        , expect = Http.expectWhatever SceneDeleted
        , timeout = Nothing
        , tracker = Nothing
        }
