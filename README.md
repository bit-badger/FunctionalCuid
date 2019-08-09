# FunctionalCuid [![Build status](https://ci.appveyor.com/api/projects/status/ewxy14onio0grhs1?svg=true)](https://ci.appveyor.com/project/danieljsummers/functionalcuid)

`FunctionalCuid` is a CUID generator written in and designed for F#, which can be used from any .NET language.

## What Is a CUID?

The best answer to that question can be found on [the README from the author](https://github.com/ericelliott/cuid). The short version is that it's a Collision-resistant Unique IDentifier (CUID) that can be generated wherever it needs to be (similar to GUIDs), but also follows a format that make them ever-increasing, which means that they can be indexed by even the most rudimentary of database systems.

## Installing

[![Nuget](https://img.shields.io/nuget/v/FunctionalCuid?logo=nuget&style=flat)](https://www.nuget.org/packages/FunctionalCuid/ "View on NuGet")

`dotnet add package FunctionalCuid` will add the package to a project. If you use Paket, add `nuget FunctionalCuid` to `paket.dependencies` and add `FunctionalCuid` to `paket.references` in the project(s) where it is needed.

## Usage

For F#, `FunctionalCuid` provides `Cuid` and `Slug` types as single-case discriminated unions, and modules to generate these types and convert them to their string representations. For string-based purposes, both `Cuid` and `Slug` also have a `generateString` function that returns the same value, just as a string.

In F#...

```fsharp
module Examples

open Cuid

/// A CUID generated as the CUID type
let generatedCuid = Cuid.generate ()

/// Creating a CUID from a string you already know. This string must be 25
/// characters long, start with "c", and be valid base-36 (0-9 and a-z).
let cuidOfString =
  match Cuid.ofString "cjz362bgd00005cus6t21gba0" with
  | Error msg -> invalidOp msg
  | Ok cuid -> cuid

/// Establish a valid CUID string using isValid and validationMessage instead.
let validatedCuidString =
  let str = "abcdefg"
  match Cuid.isValid str with
  | true -> str
  | false -> (Cuid.validationMessage >> invalidOp) str

/// Get the validation error for a CUID (empty string if CUID is valid).
let cuidErrorMsg = Cuid.validationMessage "howdy"

/// The string representation of a CUID
let cuidString = Cuid.generateString ()
```

For the `Slug` type, just replace `Cuid` with `Slug`; the validation rules for `Slug.ofString` are that the string has to be between 7 and 10 base-36 characters long.

For C# and VB.NET, the syntax is a bit different. Instead of `Cuid` as it reads above, it will appear as `CuidModule`; and, as `generateString`, `isValid`, and `validationMessage` are the most likely functions (methods) called from those languages, their compiled names use Pascal case. The same holds true for the `Slug` modules as well. A C# example...

```csharp
using Cuid;
// ...
var cuid = CuidModule.GenerateString();

// example from an MVC controller
public IActionResult Get(string cuid)
{
    if (CuidModule.IsValid(cuid))
    {
        // do something with it
    }
    else
    {
        return NotFound($"Could not find ID {cuid}; {CuidModule.ValidationMessage(cuid)}");
    }
}
```
