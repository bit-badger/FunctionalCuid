
open Cuid
open Expecto

module CuidTests =
  
  [<Tests>]
  let generateTests =
    testList "Cuid.generate" [
      test "succeeds" {
        let (Cuid cuid) = Cuid.generate ()
        Expect.stringHasLength cuid 25 "A CUID should be 25 characters"
        Expect.stringStarts cuid "c" """A CUID should start with a "c" character"""
        }
      ]
  
  [<Tests>]
  let ofStringTests =
    testList "Cuid.ofString" [
      test "fails when string is null" {
        let x = Cuid.ofString null
        Expect.isError x "Parsing should have returned an error"
        let msg = match x with Error y -> y | _ -> ""
        Expect.equal msg "string was null" "Expected error message not returned"
        }
      test "fails when string is not 25 characters" {
        let x = Cuid.ofString "c12345566677893508"
        Expect.isError x "Parsing should have returned an error"
        let msg = match x with Error y -> y | _ -> ""
        Expect.equal msg "string was not 25 characters (length 18)" "Expected error message not returned"
        }
      test "fails when string does not start with c" {
        let x = Cuid.ofString "djld2cyuq0000t3rmniod1foy"
        Expect.isError x "Parsing should have returned an error"
        let msg = match x with Error y -> y | _ -> ""
        Expect.equal msg """string did not start with "c" ("djld2cyuq0000t3rmniod1foy")"""
          "Expected error message not returned"
        }
      test "fails when string is not valid base-36" {
        let x = Cuid.ofString "cjld2*yuq0/00t3r#niod1foy"
        Expect.isError x "Parsing should have returned an error"
        let msg = match x with Error y -> y | _ -> ""
        Expect.equal msg """string was not in base-36 format ("cjld2*yuq0/00t3r#niod1foy")"""
          "Expected error message not returned"
        }
      test "succeeds" {
        let x = Cuid.ofString "cjld2cyuq0000t3rmniod1foy"
        Expect.isOk x "Parsing should have returned Ok"
        let cuid = match x with Ok y -> y | _ -> Cuid ""
        Expect.equal cuid (Cuid "cjld2cyuq0000t3rmniod1foy") "CUID value not correct"
        }
      ]

  [<Tests>]
  let toStringTests =
    testList "Cuid.toString" [
      test "succeeds" {
        let cuidString = (Cuid >> Cuid.toString) "abc123"
        Expect.equal cuidString "abc123" "The CUID string should have been the string value of the CUID"
        }
      ]
  
  [<Tests>]
  let isValidTests =
    testList "Cuid.isValid" [
      test "succeeds when the string is a valid CUID" {
        Expect.isTrue ((Cuid.generateString >> Cuid.isValid) ()) "A valid CUID should have returned true"
        }
      test "succeeds when the string is not a valid CUID" {
        Expect.isFalse (Cuid.isValid "abc") "An invalid CUID should have returned false"
        }
      ]
  
  [<Tests>]
  let validationMessageTests =
    testList "Cuid.validationMessage" [
      test "succeeds when the string is a valid CUID" {
        Expect.equal ((Cuid.generateString >> Cuid.validationMessage) ()) ""
          "A valid CUID should have returned an empty validation message"
        }
      test "succeeds when the string is an invalid CUID" {
        Expect.equal (Cuid.validationMessage null) "string was null"
          "An invalid CUID should have returned its validation error message"
        }
      ]
  
  [<Tests>]
  let generateStringTests =
    testList "Cuid.generateString" [
      test "succeeds" {
        let cuidString = Cuid.generateString ()
        Expect.stringHasLength cuidString 25 "A CUID string should be 25 characters"
        Expect.stringStarts cuidString "c" """A CUID string should start with a "c" character"""
        }
      ]

module SlugTests =

  let isProperLength (x : string) = x.Length >= 7 && x.Length <= 10

  [<Tests>]
  let generateTests =
    testList "Slug.generate" [
      test "succeeds" {
        let (Slug slug) = Slug.generate ()
        Expect.isTrue (isProperLength slug) "A Slug should be between 7 to 10 characters"
        }
      ]
  
  [<Tests>]
  let ofStringTests =
    testList "Slug.ofString" [
      test "fails when string is null" {
        let x = Slug.ofString null
        Expect.isError x "Parsing should have returned an error"
        let msg = match x with Error y -> y | _ -> ""
        Expect.equal msg "string was null" "Expected error message not returned"
        }
      test "fails when string is less than 7 characters" {
        let x = Slug.ofString "12345"
        Expect.isError x "Parsing should have returned an error"
        let msg = match x with Error y -> y | _ -> ""
        Expect.equal msg "string must be at least 7 characters (length 5)" "Expected error message not returned"
        }
      test "fails when string is more than 10 characters" {
        let x = Slug.ofString "abcdefghijklmnop"
        Expect.isError x "Parsing should have returned an error"
        let msg = match x with Error y -> y | _ -> ""
        Expect.equal msg "string must not exceed 10 characters (length 16)" "Expected error message not returned"
        }
      test "fails when string is not valid base-36" {
        let x = Slug.ofString "cj*q0/0#d"
        Expect.isError x "Parsing should have returned an error"
        let msg = match x with Error y -> y | _ -> ""
        Expect.equal msg """string was not in base-36 format ("cj*q0/0#d")""" "Expected error message not returned"
        }
      test "succeeds" {
        let x = Slug.ofString "cyuq0000t"
        Expect.isOk x "Parsing should have returned Ok"
        let slug = match x with Ok y -> y | _ -> Slug ""
        Expect.equal slug (Slug "cyuq0000t") "Slug value not correct"
        }
      ]

  [<Tests>]
  let toStringTests =
    testList "Slug.toString" [
      test "Create a string from a Slug" {
        let slugString = (Slug >> Slug.toString) "5551234"
        Expect.equal slugString "5551234" "The Slug string should have been the string value of the Slug"
        }
      ]
  
  [<Tests>]
  let isValidTests =
    testList "Slug.isValid" [
      test "succeeds when the string is a valid Slug" {
        Expect.isTrue ((Slug.generateString >> Slug.isValid) ()) "A valid Slug should have returned true"
        }
      test "succeeds when the string is not a valid Slug" {
        Expect.isFalse (Slug.isValid "12345") "An invalid Slug should have returned false"
        }
      ]
  
  [<Tests>]
  let validationMessageTests =
    testList "Slug.validationMessage" [
      test "succeeds when the string is a valid Slug" {
        Expect.equal ((Slug.generateString >> Slug.validationMessage) ()) ""
          "A valid Slug should have returned an empty validation message"
        }
      test "succeeds when the string is an invalid Slug" {
        Expect.equal (Slug.validationMessage null) "string was null"
          "An invalid Slug should have returned its validation error message"
        }
      ]
  
  [<Tests>]
  let generateStringTests =
    testList "Slug.generateString" [
      test "succeeds" {
        let slugString = Slug.generateString ()
        Expect.isTrue (isProperLength slugString) "A Slug string should be between 7 to 10 characaters"
        }
      ]

[<EntryPoint>]
let main argv =
  let writeResults = TestResults.writeNUnitSummary ("TestResults.xml", "Expecto.Tests")
  let config = defaultConfig.appendSummaryHandler writeResults
  runTestsInAssembly config argv
