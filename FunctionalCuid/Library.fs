module Cuid

open System

(*
  Functional CUID
  Collision-resistant Unique IDentifiers, written in F#

  adapted from https://github.com/ericelliott/cuid
  MIT License
*)

[<AutoOpen>]
module private Support =
  let blockSize = 4
  let baseSize = 36UL
  let discreteValues = pown baseSize blockSize

  let pad size num =
    let s = sprintf "000000000%s" num
    s.Substring(s.Length - size)

  let padToSize = pad blockSize

  let base36Chars = "0123456789abcdefghijklmnopqrstuvwxyz"

  let toBase36 nbr =
    let rec convert nbr current =
      match nbr with
      | 0UL -> sprintf "%s0" current
      | _ when nbr < baseSize -> sprintf "%c%s" base36Chars.[int nbr] current
      | _ -> convert (nbr / baseSize) (sprintf "%c%s" base36Chars.[int (nbr % baseSize)] current)
    convert nbr ""

  let padBase36 = toBase36 >> padToSize

  let rnd = Random ()

  let randomBlock () =
    (uint64 >> padBase36) (rnd.NextDouble () * float discreteValues)
  
  let mutable c = 0UL

  let safeCounter () =
    c <- if c < discreteValues then c else 0UL
    c <- c + 1UL
    c - 1UL

  let epoch = DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)

  let timestampNow () =
    (uint64 >> toBase36) (DateTime.Now - epoch).TotalMilliseconds
  
  let hostname =
    try Environment.MachineName
    with _ -> string (Random().Next ())

  let fingerprint () =
    let padTo2 = uint64 >> toBase36 >> pad 2
    [ Diagnostics.Process.GetCurrentProcess().Id |> padTo2
      hostname |> Seq.fold (fun acc chr -> acc + int chr) (hostname.Length + 36) |> padTo2
      ]
    |> List.reduce (+)

  let rightChars chars (str : string) =
    match chars with
    | _ when chars > str.Length -> str
    | _ -> str.[str.Length - chars..]

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
let cuid () =
  [ "c"
    timestampNow ()
    (safeCounter >> padBase36) ()
    fingerprint ()
    randomBlock ()
    randomBlock ()
    ]
  |> List.reduce (+)  

/// Generate a slug
///
/// The slug is not as collision-resistant as the CUID, and is also not monotonically increasing, which is desirable
/// for indexed database IDs; full CUIDs should be used in this case. A slug is made up of 4 parts:
/// - The two right-most characters of the timestamp
/// - The non-padded counter value (may be 1 to 4 characters)
/// - The first and last characters of the fingerprint
/// - 2 characters of random gibberish
let slug () =
  let print = fingerprint ()
  [ (timestampNow >> rightChars 2) ()
    (safeCounter >> string >> rightChars 4) ()
    print.[0..0]
    rightChars 1 print
    (randomBlock >> rightChars 2) ()
    ]
  |> List.reduce (+)
