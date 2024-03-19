module Main


open Elmish
open Elmish.React
open Elmish.Debug


Program.mkProgram App.init App.update App.view
|> Program.withReactSynchronous "root"
|> Program.withConsoleTrace
|> Program.run
