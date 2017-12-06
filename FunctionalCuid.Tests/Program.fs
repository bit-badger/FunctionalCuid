
open Expecto

// these tests are bad, and I should feel bad...
let tests =
  testList "Smoke Tests" [
    test "Generate a CUID" {
      Cuid.cuid () |> ignore
      }
    test "Generate a slug" {
      Cuid.slug () |> ignore
      }
    ]
  
[<EntryPoint>]
let main argv =
  runTestsWithArgs defaultConfig argv tests
