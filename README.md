# FunctionalCuid

`FunctionalCuid` is a CUID generator written in and designed for F#, which can be used from any .NET language.

## What Is a CUID?

The best answer to that question can be found on [the README from the author](https://github.com/ericelliott/cuid). The short version is that it's a Collision-resistant Unique IDentifier (CUID) that can be generated wherever it needs to be (similar to GUIDs), but also follows a format that make them ever-increasing, which means that they can be indexed by even the most rudimentary of database systems.

## Installing

(nuget link here)

## Usage

For F#, `FunctionalCuid` provides `Cuid` and `Slug` types as single-case discriminated unions, and modules to generate these types and convert them to their string representations. For string-based purposes, both `Cuid` and `Slug` also have a `generateString` function that returns the same value, just as a string.

In F#...

```fsharp
module Examples

open Cuid

/// A CUID generated as the CUID type
let generatedCuid = Cuid.generate ()

/// Creating a CUID from a string you already know. This string must be 25
/// characters long and start with "c".
let cuidFromString =
  match Cuid.fromString "cjz362bgd00005cus6t21gba0" with
  | Error msg -> invalidOp msg
  | Ok cuid -> cuid

/// The string representation of a CUID
let cuidString = Cuid.generateString ()
```

For the `Slug` type, just replace `Cuid` with `Slug`; the validation rules for `Slug.fromString` are simply that the string has to be between 7 and 10 characters long.

For C# and VB.NET, the syntax is a bit different. Instead of `Cuid` as it reads above, it will appear as `CuidModule`; and, as `generateString` is the most likely function (method) called from those languages, its compiled name uses Pascal case. The same holds true for the `Slug` modules as well. A C# example...

```csharp
using Cuid;
// ...
var cuid = CuidModule.GenerateString();
var slug = SlugModule.GenerateString();
```
