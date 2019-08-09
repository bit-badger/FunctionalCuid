namespace Cuid

(*
  Functional CUID
  Collision-resistant Unique IDentifiers, written in F#

  adapted from https://github.com/ericelliott/cuid
  MIT License
*)

/// A Collision-resistant Unique IDentifier
type Cuid = Cuid of string


/// A shortened version of the Collision-resistant Unique IDentifier
type Slug = Slug of string


/// Functions to support CUID / slug generation
[<AutoOpen>]
module private Support =
  
  open System

  /// The default block size used for the portion of a CUID
  let blockSize = 4

  /// The character set of the base representation
  let baseSize = 36UL

  /// The number of discrete values that can occur in a block
  let discreteValues = pown baseSize blockSize

  /// Left-pad a value with zeroes to make it a given size
  let pad size num =
    let s = sprintf "000000000%s" num
    s.Substring(s.Length - size)

  /// Left-pad a value to the default block size
  let padToSize = pad blockSize

  /// The character set available for a CUID
  let base36Chars = "0123456789abcdefghijklmnopqrstuvwxyz"

  /// Convert a number to its base-36 representation
  let toBase36 nbr =
    let rec convert nbr current =
      match nbr with
      | 0UL -> sprintf "%s0" current
      | _ when nbr < baseSize -> sprintf "%c%s" base36Chars.[int nbr] current
      | _ -> convert (nbr / baseSize) (sprintf "%c%s" base36Chars.[int (nbr % baseSize)] current)
    convert nbr ""

  /// Is a string in a base-36 representation?
  let isBase36 (x : string) =
    let rec check idx =
      match idx with
      | _ when idx = x.Length -> true
      | _ ->
          match (string >> base36Chars.Contains) x.[idx] with
          | true -> check (idx + 1)
          | false -> false
    check 0

  /// Left-pad a base-36 number to the default block size
  let padBase36 = toBase36 >> padToSize

  /// A pseudo-random number generator instance
  let rnd = Random ()

  /// Create a block of random base-36
  let randomBlock () = rnd.NextDouble () * float discreteValues |> (uint64 >> padBase36)
  
  /// Counter for the monotonically-increasing counter portion of the CUID
  let mutable c = 0UL

  /// Increment the counter, handling roll-over, and return the previous value
  let safeCounter () =
    c <- if c < discreteValues then c else 0UL
    c <- c + 1UL
    c - 1UL

  /// The Unix epoch value
  let epoch = DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)

  /// The timestamp portion of the CUID
  let timestampNow () = (DateTime.Now - epoch).TotalMilliseconds |> (uint64 >> toBase36)
  
  /// The host name, or a random string if it cannot be obtained
  let hostname = try Environment.MachineName with _ -> string (Random().Next ())

  /// The fingerprint (a block made up of 2 characters each from the process ID and hostname)
  let fingerprint () =
    let padTo2 = uint64 >> toBase36 >> pad 2
    [ Diagnostics.Process.GetCurrentProcess().Id |> padTo2
      hostname |> Seq.fold (fun acc chr -> acc + int chr) (hostname.Length + 36) |> padTo2
      ]
    |> List.reduce (+)

  /// Obtain the given number of characters from the right of a string
  let rightChars chars (str : string) =
    match chars with
    | _ when chars > str.Length -> str
    | _ -> str.[str.Length - chars..]


/// Public functions for the CUID type
module Cuid =

  /// Generate a CUID
  ///
  /// The CUID is made up of 5 parts:
  /// - The letter "c" (is for both cookies and CUIDs; lowercase letter makes it HTML element ID friendly)
  /// - A timestamp (in milliseconds send the Unix epoch)
  /// - A sequential counter, used to prevent same-machine collisions
  /// - A fingerprint, generated from the hostname and process ID
  /// - 8 characters of random gibberish
  ///
  /// The timestamp, fingerprint, and randomness are all encoded in base 36, using 0-9 and a-z.
  let generate () =
    [ "c"
      timestampNow ()
      (safeCounter >> padBase36) ()
      fingerprint ()
      randomBlock ()
      randomBlock ()
      ]
    |> List.reduce (+)
    |> Cuid

  /// Create a CUID from a string
  ///
  /// The string must:
  /// - not be null
  /// - be 25 characters long
  /// - start with "c"
  /// - be base-36 format ([0-9|a-z])
  // TODO: extract these validations out so we can provide a "validate" function for C#/VB.NET
  let fromString (x : string) =
    match x with
    | null -> Error "string was null"
    | _ when x.Length <> 25 -> (sprintf "string was not 25 characters (length %i)" >> Error) x.Length
    | _ when not (x.StartsWith "c") -> (sprintf """string did not start with "c" ("%s")""" >> Error) x
    | _ when (not << isBase36) x -> (sprintf """string was not in base-36 format ("%s")""" >> Error) x
    | _ -> (Cuid >> Ok) x

  /// Get the string representation of a CUID
  let toString x = match x with Cuid y -> y

  /// Generate a CUID as a string
  [<CompiledName "GenerateString">]
  let generateString = generate >> toString


/// Public functions for the Slug type
module Slug =

  /// Generate a slug
  ///
  /// The slug is not as collision-resistant as the CUID, and is also not monotonically increasing, which is desirable
  /// for indexed database IDs; full CUIDs should be used in this case. A slug is made up of 4 parts:
  /// - The two right-most characters of the timestamp
  /// - The non-padded counter value (may be 1 to 4 characters)
  /// - The first and last characters of the fingerprint
  /// - 2 characters of random gibberish
  let generate () =
    let print = fingerprint ()
    [ (timestampNow >> rightChars 2) ()
      (safeCounter >> string >> rightChars 4) ()
      print.[0..0]
      rightChars 1 print
      (randomBlock >> rightChars 2) ()
      ]
    |> List.reduce (+)
    |> Slug
  
  /// Create a Slug from a string
  ///
  /// The string must be between 7 and 10 characters long and base-36 format ([0-9|a-z])
  // TODO: extract these validations out so we can provide a "validate" function for C#/VB.NET
  let fromString (x : string) =
    match x with
    | null -> Error "string was null"
    | _ when x.Length < 7 -> (sprintf "string must be at least 7 characters (length %i)" >> Error) x.Length
    | _ when x.Length > 10 -> (sprintf "string must not exceed 10 characters (length %i)" >> Error) x.Length
    | _ when (not << isBase36) x -> (sprintf """string was not in base-36 format ("%s")""" >> Error) x
    | _ -> (Slug >> Ok) x

  /// Get the string representation of a Slug
  let toString x = match x with Slug y -> y

  /// Generate a Slug as a string
  [<CompiledName "GenerateString">]
  let generateString = generate >> toString
