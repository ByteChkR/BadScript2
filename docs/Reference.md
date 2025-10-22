


















# BadScript2 API Reference



## BadHtml










### BadHtml.Info




#### Properties

| Name | Value |
| --- | --- |
| AssemblyName |  BadScript2.Interop.Html  |
| Name |  BadHtml  |
| Version |  25.1.2.1  |













### BadHtml.Run

Runs a BadHtml Template

Returns: [string](#string) - The string result of the html transformation


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| file | [string](#string) | True | False | False | The Template File |
| model | [any](#any) | False | True | False | The ModelDefault Value: null |
| skipEmptyTextNodes | [bool](#bool) | True | True | False | If True, empty text nodes are omitted from the output htmlDefault Value: false |












## Compression










### Compression.Base64










#### Compression.Base64.Decode

Decodes a base64 string to an array of bytes

Returns: [Array](#array) - Bytes


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| str | [string](#string) | True | False | False | String to Decode |









#### Compression.Base64.Encode

Encodes the given string to a base64 string

Returns: [string](#string) - Base64 String


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| obj | [Array](#array) | True | False | False | Bytes to Encode |














### Compression.Deflate










#### Compression.Deflate.Compress

Compresses the given string using the Deflate Algorithm

Returns: [Array](#array) - Compressed Array of bytes


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| obj | [string](#string) | True | False | False | String to Compress |









#### Compression.Deflate.Decompress

Inflates the given array of bytes using the Deflate Algorithm

Returns: [string](#string) - Decompressed String


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| obj | [Array](#array) | True | False | False | Bytes to Decompress |














### Compression.GZip










#### Compression.GZip.Compress

Compresses the given string using the GZip Algorithm

Returns: [Array](#array) - Compressed Array of bytes


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| obj | [string](#string) | True | False | False | String to Compress |









#### Compression.GZip.Decompress

Inflates the given array of bytes using the GZip Algorithm

Returns: [any](#any) - Decompressed String


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| obj | [Array](#array) | True | False | False | Bytes to Decompress |














### Compression.Info




#### Properties

| Name | Value |
| --- | --- |
| AssemblyName |  BadScript2.Interop.Compression  |
| Name |  Compression  |
| Version |  25.1.2.1  |













### Compression.ZLib










#### Compression.ZLib.Compress

Compresses the given string using the ZLib Algorithm

Returns: [any](#any) - Compressed Array of bytes


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| obj | [string](#string) | True | False | False | String to Compress |









#### Compression.ZLib.Decompress

Inflates the given array of bytes using the ZLib Algorithm

Returns: [any](#any) - Decompressed String


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| obj | [Array](#array) | True | False | False | Bytes to Decompress |














### Compression.Zip










#### Compression.Zip.FromDirectory

Creates a new Zip Archive from the given directory

Returns: [any](#any)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| inputDir | [string](#string) | True | False | False | Input Directory |
| outputFile | [string](#string) | True | False | False | Output File |









#### Compression.Zip.ToDirectory

Extracts a Zip Archive to the given directory

Returns: [any](#any)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| outputDir | [string](#string) | True | False | False | Output Directory |
| inputFile | [string](#string) | True | False | False | Input File |

















## Concurrent










### Concurrent.Create

Creates a new Task

Returns: [Task](#task) - The Created Task


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| func | [Function](#function) | True | False | False | The Function that will be executed within the task. |









### Concurrent.GetCurrent

Returns the Current Task

Returns: [any](#any) - The Current Task








### Concurrent.Info




#### Properties

| Name | Value |
| --- | --- |
| AssemblyName |  BadScript2.Interop.Common  |
| Name |  Concurrent  |
| Version |  25.1.2.1  |













### Concurrent.Run

Runs a Task

Returns: [any](#any)


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| task | [Task](#task) | True | False | False | The Task that will be executed |












## Console










### Console.Clear

Clears the Console

Returns: [any](#any)








### Console.Info




#### Properties

| Name | Value |
| --- | --- |
| AssemblyName |  BadScript2.Interop.Common  |
| Name |  Console  |
| Version |  25.1.2.1  |













### Console.ReadLine

Reads a line from the Console

Returns: [string](#string) - The Console Input








### Console.ReadLineAsync

Reads a line from the Console asynchronously

Returns: [Task](#task) - The Console Input








### Console.Write

Writes to the Console

Returns: [any](#any)


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| obj | [any](#any) | True | False | False | The Object to Write |









### Console.WriteLine

Writes a line to the Console

Returns: [any](#any)


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| obj | [any](#any) | True | False | False | The Object to Write |












## IO










### IO.Directory










#### IO.Directory.Copy

Copies a specified file to a new location, providing the option to specify a new file name.

Returns: [any](#any)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| source | [string](#string) | True | False | False | The Path of the file to copy |
| destination | [string](#string) | True | False | False | The Destination Path |
| overwrite | [bool](#bool) | True | True | False | If true, allows an existing file to be overwritten; otherwise, false.Default Value: false |









#### IO.Directory.CreateDirectory

Creates all directories and subdirectories in the specified path.

Returns: [any](#any)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| path | [string](#string) | True | False | False | The directory to create. |









#### IO.Directory.Delete

Deletes the specified directory and, if indicated, any subdirectories in the directory.

Returns: [any](#any)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| path | [string](#string) | True | False | False | The Path to delete |
| recursive | [bool](#bool) | True | False | False | If true, the directory will be deleted recursively |









#### IO.Directory.Exists

Tests if the path points to an existing directory.

Returns: [bool](#bool) - True if the directory exists; otherwise, false.


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| path | [string](#string) | True | False | False | The Path to check |









#### IO.Directory.GetCurrentDirectory

Returns the Current Working Directory.

Returns: [string](#string) - The Current Working Directory








#### IO.Directory.GetDirectories

Returns the directories in the specified directory.

Returns: [Array](#array) - An array of directories in the specified directory.


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| path | [string](#string) | True | False | False | The Path to get the directories from. |
| recursive | [bool](#bool) | True | True | False | If true, the search will return all subdirectories recursivelyDefault Value: false |









#### IO.Directory.GetFiles

Returns the files in the specified directory.

Returns: [Array](#array) - An array of files in the specified directory.


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| path | [string](#string) | True | False | False | The Path to get the files from. |
| searchPattern | [string](#string) | True | True | False | The search pattern.Default Value: "" |
| recursive | [bool](#bool) | True | True | False | If true, the search will return all files recursivelyDefault Value: false |









#### IO.Directory.GetStartupDirectory

Returns the startup directory

Returns: [string](#string) - The startup directory








#### IO.Directory.Move

Moves a specified file to a new location, providing the option to specify a new file name.

Returns: [any](#any)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| source | [string](#string) | True | False | False | The Path of the file to move |
| destination | [string](#string) | True | False | False | The Destination Path |
| overwrite | [bool](#bool) | True | True | False | If true, allows an existing file to be overwritten; otherwise, false.Default Value: false |









#### IO.Directory.SetCurrentDirectory

Sets the Current Working Directory.

Returns: [any](#any)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| path | [string](#string) | True | False | False | The Path to set as the Current Working Directory. |














### IO.File










#### IO.File.Copy

Copies a specified file to a new location, providing the option to specify a new file name.

Returns: [any](#any)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| source | [string](#string) | True | False | False | The Path of the file to copy |
| destination | [string](#string) | True | False | False | The Destination Path |
| overwrite | [bool](#bool) | True | True | False | If true, allows an existing file to be overwritten; otherwise, false.Default Value: false |









#### IO.File.Delete

Deletes the specified file.

Returns: [any](#any)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| path | [string](#string) | True | False | False | The Path of the file to delete |









#### IO.File.Exists

Determines whether the specified file exists.

Returns: [bool](#bool) - True if the caller has the required permissions and path contains the name of an existing file; otherwise, false.


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| path | [string](#string) | True | False | False | The Path to check |









#### IO.File.Move

Moves a specified file to a new location, providing the option to specify a new file name.

Returns: [any](#any)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| source | [string](#string) | True | False | False | The Path of the file to move |
| destination | [string](#string) | True | False | False | The Destination Path |
| overwrite | [bool](#bool) | True | True | False | If true, allows an existing file to be overwritten; otherwise, false.Default Value: false |









#### IO.File.ReadAllBytes

Opens a file, reads all bytes of the file, and then closes the file.

Returns: [Array](#array) - The content of the file


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| path | [string](#string) | True | False | False | The Path of the file to read |









#### IO.File.ReadAllLines

Opens a file, reads all lines of the file, and then closes the file.

Returns: [Array](#array) - The content of the file


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| path | [string](#string) | True | False | False | The Path of the file to read |









#### IO.File.ReadAllText

Opens a text file, reads all content of the file, and then closes the file.

Returns: [string](#string) - The content of the file


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| path | [string](#string) | True | False | False | The Path of the file to read |









#### IO.File.WriteAllBytes

Opens a file, writes all bytes to the file, and then closes the file.

Returns: [any](#any)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| path | [string](#string) | True | False | False | The Path of the file to write |
| content | [Array](#array) | True | False | False | The Content |









#### IO.File.WriteAllLines

Opens a file, writes all lines to the file, and then closes the file.

Returns: [any](#any)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| path | [string](#string) | True | False | False | The Path of the file to write |
| content | [Array](#array) | True | False | False | The Content |









#### IO.File.WriteAllText

Writes the specified string to a file, overwriting the file if it already exists.

Returns: [any](#any)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| path | [string](#string) | True | False | False | The Path of the file to write |
| content | [string](#string) | True | False | False | The Content |














### IO.Info




#### Properties

| Name | Value |
| --- | --- |
| AssemblyName |  BadScript2.Interop.IO  |
| Name |  IO  |
| Version |  25.1.2.1  |













### IO.Path










#### IO.Path.ChangeExtension

Changes the extension of a path string.

Returns: [string](#string) - The modified path information.


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| path | [string](#string) | True | False | False | The path information to modify. |
| extension | [string](#string) | True | False | False | The new extension (with or without a leading period). Specify null to remove an existing extension from path |









#### IO.Path.Combine

Combines two or more strings into a path.

Returns: [string](#string) - The combined path.


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| parts | [Array](#array) | True | False | True | The Path Parts |









#### IO.Path.Expand

Expands Patterns using the GlobFile Syntax.

Returns: [Array](#array) - The expanded files.


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| patterns | [Array](#array) | True | False | False | The Patterns |
| workingDirectory | [string](#string) | False | True | False | Default Value: null |









#### IO.Path.GetDirectoryName

Returns the directory information for the specified path string

Returns: [string](#string) - Directory information for path, or null if path denotes a root directory or is null. Returns Empty if path does not contain directory information.


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| path | [string](#string) | True | False | False | The path of a file or directory. |









#### IO.Path.GetExtension

Returns the extension of the specified path string.

Returns: [string](#string) - The extension of the specified path (including the period).


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| path | [string](#string) | True | False | False | The path of a file or directory. |









#### IO.Path.GetFileName

Returns the file name and extension of the specified path string.

Returns: [string](#string) - The characters after the last directory character in path.


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| path | [string](#string) | True | False | False | The Path to get the file name from. |









#### IO.Path.GetFileNameWithoutExtension

Returns the file name of the specified path string without the extension

Returns: [string](#string) - The string returned by GetFileName(string), minus the last period (.) and all characters following it


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| path | [string](#string) | True | False | False | The Path to get the file name from. |









#### IO.Path.GetFullPath

Returns the absolute path for the specified path string.

Returns: [string](#string) - The fully qualified location of path, such as "C:\MyFile.txt" or "/home/user/myFolder".


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| path | [string](#string) | True | False | False | The path of a file or directory. |









#### IO.Path.GetStartupPath

Returns the Startup Directory.

Returns: [string](#string) - The Startup Directory
















## Json




### Properties

| Name | Value |
| --- | --- |
| Settings |  < multi line content >  |








### Json.FromJson

Converts a JSON String to a BadObject

Returns: [any](#any) - The Parsed Object


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| str | [string](#string) | True | False | False | The JSON String |









### Json.FromYaml

Converts a YAML String to a BadObject

Returns: [any](#any) - The Parsed Object


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| str | [string](#string) | True | False | False | The YAML String |









### Json.Info




#### Properties

| Name | Value |
| --- | --- |
| AssemblyName |  BadScript2.Interop.Json  |
| Name |  Json  |
| Version |  25.1.2.1  |













### Json.ToJson

Converts a BadObject to a JSON String

Returns: [string](#string) - The JSON String


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| o | [any](#any) | True | False | False | The Object to be converted. |









### Json.ToYaml

Converts a BadObject to a YAML String

Returns: [string](#string) - The YAML String


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| o | [any](#any) | True | False | False | The Object to be converted. |












## Math




### Properties

| Name | Value |
| --- | --- |
| E |  < multi line content >  |
| PI |  < multi line content >  |
| Tau |  < multi line content >  |








### Math.Abs

Returns the absolute value of a number

Returns: [num](#num) - A decimal number, x, such that 0 ≤ x ≤MaxValue.


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| x | [num](#num) | True | False | False | A number that is greater than or equal to MinValue, but less than or equal to MaxValue. |









### Math.Acos

Returns the angle whose cosine is the specified number.

Returns: [num](#num) - An angle, θ, measured in radians, such that 0 ≤θ≤π -or- NaN if x < -1 or x > 1 or x equals NaN.


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| x | [num](#num) | True | False | False | A number representing a cosine, where x must be greater than or equal to -1, but less than or equal to 1 |









### Math.Asin

Returns the angle whose sine is the specified number.

Returns: [num](#num) - An angle, θ, measured in radians, such that -π/2 ≤θ≤π/2 -or- NaN if x < -1 or x > 1 or x equals NaN.


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| x | [num](#num) | True | False | False | A number representing a sine, where x must be greater than or equal to -1, but less than or equal to 1 |









### Math.Atan

Returns the angle whose tangent is the specified number.

Returns: [num](#num) - An angle, θ, measured in radians, such that -π/2 ≤θ≤π/2. -or- NaN if x equals NaN, -π/2 rounded to double precision (-1.5707963267949) if x equals NegativeInfinity, or π/2 rounded to double precision (1.5707963267949) if x equals PositiveInfinity.


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| x | [num](#num) | True | False | False | A number representing a tangent |









### Math.Atan2

Returns the angle whose tangent is the quotient of two specified numbers.

Returns: [num](#num) - An angle, θ, measured in radians, such that -π≤θ≤π, and tan(θ) = y / x, where (x, y) is a point in the Cartesian plane. Observe the following: For (x, y) in quadrant 1, 0 < θ < π/2. For (x, y) in quadrant 2, π/2 < θ≤π. For (x, y) in quadrant 3, -π < θ < -π/2. For (x, y) in quadrant 4, -π/2 < θ < 0. For points on the boundaries of the quadrants, the return value is the following: If y is 0 and x is not negative, θ = 0. If y is 0 and x is negative, θ = π. If y is positive and x is 0, θ = π/2. If y is negative and x is 0, θ = -π/2. If y is 0 and x is 0, θ = 0. If x or y is NaN, or if x and y are either PositiveInfinity or NegativeInfinity, the method returns NaN


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| y | [num](#num) | True | False | False | The y coordinate of a point. |
| x | [num](#num) | True | False | False | The x coordinate of a point. |









### Math.Ceiling

Returns the smallest integer greater than or equal to the specified number.

Returns: [num](#num) - The smallest integral value that is greater than or equal to x. Note that this method returns a Decimal instead of an integral type


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| x | [num](#num) | True | False | False | A decimal number. |









### Math.Cos

Returns the cosine of the specified angle.

Returns: [num](#num) - The cosine of x. If x is equal to NaN, NegativeInfinity, or PositiveInfinity, this method returns NaN.


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| x | [num](#num) | True | False | False | An angle, measured in radians |









### Math.Cosh

Returns the hyperbolic cosine of the specified angle.

Returns: [num](#num) - The hyperbolic cosine of x. If x is equal to NegativeInfinity or PositiveInfinity, PositiveInfinity is returned. If x is equal to NaN, NaN is returned


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| x | [num](#num) | True | False | False | An angle, measured in radians |









### Math.Exp

Returns the value of e raised to the specified power.

Returns: [num](#num) - The number e raised to the power x. If x equals NaN or PositiveInfinity, that value is returned. If x equals NegativeInfinity, 0 is returned.


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| x | [num](#num) | True | False | False | A number specifying a power |









### Math.Floor

Returns the largest integer less than or equal to the specified number.

Returns: [num](#num) - The largest integral value that is less than or equal to x. Note that this method returns a Decimal instead of an integral type


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| x | [num](#num) | True | False | False | A decimal number. |









### Math.Info




#### Properties

| Name | Value |
| --- | --- |
| AssemblyName |  BadScript2.Interop.Common  |
| Name |  Math  |
| Version |  25.1.2.1  |













### Math.Log

Returns the natural (base e) logarithm of a specified number.

Returns: [num](#num) - The natural logarithm of x; that is, ln x, where e is approximately equal to 2.71828182845904. If x is equal to NaN or NegativeInfinity, that value is returned. If x is equal to PositiveInfinity, PositiveInfinity is returned.


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| x | [num](#num) | True | False | False | A decimal number. |









### Math.Log10

Returns the logarithm of a specified number with base 10.

Returns: [num](#num) - The base 10 logarithm of x; that is, log10 x. If x is equal to NaN or NegativeInfinity, that value is returned. If x is equal to PositiveInfinity, PositiveInfinity is returned.


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| x | [num](#num) | True | False | False | A decimal number. |









### Math.Max

Returns the larger of two numbers.

Returns: [num](#num) - The larger of x or y. If x, or y, or both x and y are equal to NaN, NaN is returned.


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| x | [num](#num) | True | False | False | The first of two decimal numbers to compare. |
| y | [num](#num) | True | False | False | The second of two decimal numbers to compare. |









### Math.Min

Returns the smaller of two numbers.

Returns: [num](#num) - The smaller of x or y. If x, or y, or both x and y are equal to NaN, NaN is returned.


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| x | [num](#num) | True | False | False | The first of two decimal numbers to compare. |
| y | [num](#num) | True | False | False | The second of two decimal numbers to compare. |









### Math.NextRandom

Returns a random number between 0.0 and 1.0.

Returns: [num](#num) - A double-precision floating point number greater than or equal to 0.0, and less than 1.0.








### Math.Pow

Returns a specified number raised to the specified power.

Returns: [num](#num) - The number x raised to the power y.


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| x | [num](#num) | True | False | False | A double-precision floating-point number to be raised to a power |
| y | [num](#num) | True | False | False | A double-precision floating-point number that specifies a power |









### Math.Round

Rounds a value to the nearest integer or to the specified number of decimal places.

Returns: [num](#num) - The number nearest to x that contains a number of fractional digits equal to decimals.


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| x | [num](#num) | True | False | False | A decimal number to be rounded |
| y | [num](#num) | True | False | False | The number nearest to x that contains a number of fractional digits equal to decimals |









### Math.Sign

Returns a value indicating the sign of a number.

Returns: [num](#num) - A number that indicates the sign of x, as shown in the following table. Value Meaning -1 x is less than zero. 0 x is equal to zero. 1 x is greater than zero.


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| x | [num](#num) | True | False | False | A decimal number. |









### Math.Sin

Returns the sine of the specified angle.

Returns: [num](#num) - The sine of x. If x is equal to NaN, NegativeInfinity, or PositiveInfinity, this method returns NaN.


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| x | [num](#num) | True | False | False | An angle, measured in radians |









### Math.Sinh

Returns the hyperbolic sine of the specified angle.

Returns: [num](#num) - The hyperbolic sine of x. If x is equal to NegativeInfinity, NegativeInfinity is returned. If x is equal to PositiveInfinity, PositiveInfinity is returned. If x is equal to NaN, NaN is returned.


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| x | [num](#num) | True | False | False | An angle, measured in radians |









### Math.Sqrt

Returns the square root of a specified number.

Returns: [num](#num) - The positive square root of x. If x is equal to NaN, NegativeInfinity, or PositiveInfinity, that value is returned.


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| x | [num](#num) | True | False | False | A decimal number. |









### Math.Tan

Returns the tangent of the specified angle.

Returns: [num](#num) - The tangent of x. If x is equal to NaN, NegativeInfinity, or PositiveInfinity, this method returns NaN.


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| x | [num](#num) | True | False | False | An angle, measured in radians |









### Math.Tanh

Returns the hyperbolic tangent of the specified angle.

Returns: [num](#num) - The hyperbolic tangent of x. If x is equal to NegativeInfinity, -1 is returned. If x is equal to PositiveInfinity, 1 is returned. If x is equal to NaN, NaN is returned.


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| x | [num](#num) | True | False | False | An angle, measured in radians |









### Math.Truncate

Returns the integral part of a specified decimal number.

Returns: [num](#num) - The integral part of d; that is, the number that remains after any fractional digits have been discarded.


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| x | [num](#num) | True | False | False | A number to truncate |












## Net










### Net.DecodeUriComponent

Decodes a URI Component

Returns: [string](#string) - The decoded URI Component


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| s | [string](#string) | True | False | False | The component to decode |









### Net.EncodeUriComponent

Encodes a URI Component

Returns: [string](#string) - The encoded URI Component


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| s | [string](#string) | True | False | False | The component to encode |









### Net.Get

Performs a GET request to the given url

Returns: [Task](#task) - The Awaitable Task


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| url | [string](#string) | True | False | False | The URL of the GET request |









### Net.Info




#### Properties

| Name | Value |
| --- | --- |
| AssemblyName |  BadScript2.Interop.Net  |
| Name |  Net  |
| Version |  25.1.2.1  |













### Net.Post

Performs a POST request to the given url with the given content

Returns: [Task](#task) - The Awaitable Task


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| url | [string](#string) | True | False | False | The URL of the POST request |
| content | [string](#string) | True | False | False | The String content of the post request |












## NetHost










### NetHost.Create

Creates a new NetHost Instance

Returns: [Table](#table) - The NetHost Instance


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| prefixes | [Array](#array) | True | False | False | Array of string prefixes |









### NetHost.Info




#### Properties

| Name | Value |
| --- | --- |
| AssemblyName |  BadScript2.Interop.NetHost  |
| Name |  NetHost  |
| Version |  25.1.2.1  |
















## OS










### OS.Environment




#### Properties

| Name | Value |
| --- | --- |
| CommandLine |  /work/BadScript2/src/BadScript2.Console/BadScript2.Console/bin/Debug/net8.0/bs.dll run -f ../projects/bsdoc/src/Main.bs -p 16  |
| CurrentDirectory |  /work/BadScript2/docs  |
| CurrentManagedThreadId |  < multi line content >  |
| ExitCode |  < multi line content >  |
| HasShutdownStarted |  < multi line content >  |
| Is64BitOperatingSystem |  < multi line content >  |
| Is64BitProcess |  < multi line content >  |
| MachineName |  tim-laptop  |
| NewLine |  < multi line content >  |
| OSVersion |  Unix 6.14.0.33  |
| ProcessorCount |  < multi line content >  |
| StackTrace |  < multi line content >  |
| SystemDirectory |    |
| SystemPageSize |  < multi line content >  |
| TickCount |  < multi line content >  |
| UserDomainName |  tim-laptop  |
| UserInteractive |  < multi line content >  |
| UserName |  tim  |
| WorkingSet |  < multi line content >  |








#### OS.Environment.Exit

Exits the Application

Returns: [any](#any)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| e | [num](#num) | True | False | False | The exit code to return to the operating system. Use 0 (zero) to indicate that the process completed successfully. |









#### OS.Environment.ExpandEnvironmentVariables

Replaces the name of each environment variable embedded in the specified string with the string equivalent of the value of the variable, then returns the resulting string.

Returns: [string](#string) - A string with each environment variable replaced by its value.


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| s | [string](#string) | True | False | False | A string containing the names of zero or more environment variables. Each environment variable is quoted with the percent sign character (%). |









#### OS.Environment.FailFast

Terminates the process

Returns: [any](#any)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| m | [string](#string) | True | False | False | A message that explains why the process was terminated, or null if no explanation is provided. |









#### OS.Environment.GetCommandLineArguments

Gets the Commandline Arguments

Returns: [Array](#array) - An Array containing all Commandline Arguments








#### OS.Environment.GetEnvironmentVariable

Gets the value of an Environment Variable

Returns: [string](#string) - The value of the Environment Variable


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| s | [string](#string) | True | False | False | The name of the environment variable |









#### OS.Environment.GetEnvironmentVariables

Gets all Environment Variables

Returns: [Table](#table) - A Table containing all Environment Variables








#### OS.Environment.GetLogicalDrives

Gets the Logical Drives

Returns: [Array](#array) - An Array containing all Logical Drives








#### OS.Environment.SetEnvironmentVariable

Sets the value of an Environment Variable

Returns: [any](#any)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| n | [string](#string) | True | False | False | The name of an environment variable |
| v | [string](#string) | True | False | False | A value to assign to variable |














### OS.Info




#### Properties

| Name | Value |
| --- | --- |
| AssemblyName |  BadScript2.Interop.Common  |
| Name |  OS  |
| Version |  25.1.2.1  |













### OS.Run

Runs a Process

Returns: [Table](#table) - Process Table


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| fileName | [string](#string) | True | False | False | The name of the application to start, or the name of a document of a file type that is associated with an application and that has a default open action available to it. The default is an empty string (""). |
| arguments | [string](#string) | True | False | False | A single string containing the arguments to pass to the target application specified in the FileName property. The default is an empty string (""). On Windows Vista and earlier versions of the Windows operating system, the length of the arguments added to the length of the full path to the process must be less than 2080. On Windows 7 and later versions, the length must be less than 32699. Arguments are parsed and interpreted by the target application, so must align with the expectations of that application. For.NET applications as demonstrated in the Examples below, spaces are interpreted as a separator between multiple arguments. A single argument that includes spaces must be surrounded by quotation marks, but those quotation marks are not carried through to the target application. In include quotation marks in the final parsed argument, triple-escape each mark. |
| workingDirectory | [string](#string) | True | False | False | When UseShellExecute is true, the fully qualified name of the directory that contains the process to be started. When the UseShellExecute property is false, the working directory for the process to be started. The default is an empty string ("") |
| createNoWindow | [bool](#bool) | True | False | False | true if the process should be started without creating a new window to contain it; otherwise, false. The default is false. |
| useShellExecute | [bool](#bool) | True | False | False | true if the shell should be used when starting the process; false if the process should be created directly from the executable file. The default is true |












## Runtime










### Runtime.CreateDefaultScope

Creates a default scope based off of the root scope of the caller

Returns: [Scope](#scope) - The created scope








### Runtime.EvaluateAsync

Evaluates a BadScript Source String

Returns: [Task](#task) - An Awaitable Task


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| source | [string](#string) | True | False | False | The Source of the Script |
| file | [string](#string) | False | True | False | An (optional but recommended) file path, it will be used to determine the working directory of the script.Default Value: null |
| optimize | [bool](#bool) | True | True | False | If true, any optimizations that are activated in the settings will be applied.Default Value: true |
| scope | [Scope](#scope) | False | True | False | An (optional) scope that the execution takes place in, if not specified, an Instance of BadRuntime will get searched and a scope will be created from it, if its not found, a scope will be created from the root scope of the caller.Default Value: null |
| setLastAsReturn | [bool](#bool) | True | True | False | If true, the last element that was returned from the enumeration will be the result of the task. Otherwise the result will be the exported objects of the script.Default Value: false |









### Runtime.GetArguments

Gets the Commandline Arguments

Returns: [Array](#array) - An Array containing all Commandline Arguments








### Runtime.GetExtensionNames

Lists all extension names of the given object

Returns: [Array](#array) - An Array containing all Extension Names


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| o | [any](#any) | True | False | False | Object |









### Runtime.GetExtensions

Returns all Extensions of the given object(will create the objects based on the supplied object)

Returns: [Table](#table) - A Table containing all Extensions


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| o | [any](#any) | True | False | False | Object |









### Runtime.GetGlobalExtensionNames

Lists all global extension names

Returns: [any](#any) - An Array containing all Extension Names








### Runtime.GetMembers

Gets the Attributes of the given objects members

Returns: [Array](#array) - A Table containing the Attributes of the given objects members.


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| obj | [any](#any) | True | False | False |  |









### Runtime.GetNativeTypes

Returns all Native Types

Returns: [Array](#array) - An Array containing all Native Types








### Runtime.GetRegisteredApis

Returns all registered apis

Returns: [Array](#array) - An Array containing all registered apis








### Runtime.GetRootScope



Returns: [Scope](#scope)








### Runtime.GetRuntimeAssemblyPath

Returns the Assembly Path of the current Runtime

Returns: [string](#string) - The Assembly Path








### Runtime.GetStackTrace

Returns the Stack Trace of the current Execution Context

Returns: [string](#string) - The Stack Trace








### Runtime.GetTimeNow

Returns the Current Time

Returns: [any](#any) - The Current Time








### Runtime.ImportAsync

Imports a module at runtime.

Returns: [Task](#task)


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| path | [string](#string) | True | False | False |  |









### Runtime.Info




#### Properties

| Name | Value |
| --- | --- |
| AssemblyName |  BadScript2.Interop.Common  |
| Name |  Runtime  |
| Version |  25.1.2.1  |













### Runtime.IsApiRegistered

Returns true if an api with that name is registered

Returns: [any](#any) - True if the api is registered


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| api | [string](#string) | True | False | False | The Api Name |









### Runtime.MakeReference

Creates a new Reference Object

Returns: [any](#any) - The Reference Object


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| refText | [string](#string) | True | False | False | The Text for the Reference |
| get | [Function](#function) | True | False | False | The getter Function |
| set | [Function](#function) | False | True | False | The setter FunctionDefault Value: null |
| delete | [Function](#function) | False | True | False | The delete FunctionDefault Value: null |









### Runtime.Native










#### Runtime.Native.IsArray

Returns true if the given object is an array

Returns: [bool](#bool) - True if the given object is an array


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| arg | [any](#any) | False | False | False | Object to test |









#### Runtime.Native.IsBoolean

Returns true if the given object is a boolean

Returns: [bool](#bool) - True if the given object is a boolean


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| arg | [any](#any) | False | False | False | Object to test |









#### Runtime.Native.IsEnumerable

Returns true if the given object is enumerable

Returns: [bool](#bool) - True if the given object is enumerable


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| arg | [any](#any) | False | False | False | Object to test |









#### Runtime.Native.IsEnumerator

Returns true if the given object is an enumerator

Returns: [bool](#bool) - True if the given object is an enumerator


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| arg | [any](#any) | False | False | False | Object to test |









#### Runtime.Native.IsFunction

Returns true if the given object is a function

Returns: [bool](#bool) - True if the given object is a function


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| arg | [any](#any) | False | False | False | Object to test |









#### Runtime.Native.IsNative

Returns true if the given object is a native object

Returns: [bool](#bool) - True if the given object is a native object


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| arg | [any](#any) | False | False | False | Object to test |









#### Runtime.Native.IsNumber

Returns true if the given object is a number

Returns: [bool](#bool) - True if the given object is a number


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| arg | [any](#any) | False | False | False | Object to test |









#### Runtime.Native.IsPrototype

Returns true if the given object is a class prototype

Returns: [bool](#bool) - True if the given object is a class prototype


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| arg | [any](#any) | False | False | False | Object to test |









#### Runtime.Native.IsPrototypeInstance

Returns true if the given object is an instance of a class prototype

Returns: [bool](#bool) - True if the given object is an instance of a class prototype


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| arg | [any](#any) | False | False | False | Object to test |









#### Runtime.Native.IsString

Returns true if the given object is a string

Returns: [bool](#bool) - True if the given object is a string


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| arg | [any](#any) | False | False | False | Object to test |









#### Runtime.Native.IsTable

Returns true if the given object is a table

Returns: [bool](#bool) - True if the given object is a table


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| arg | [any](#any) | False | False | False | Object to test |














### Runtime.NewGuid

Returns a new Guid

Returns: [string](#string) - A new Guid








### Runtime.ParseDate

Parses a date string

Returns: [any](#any) - Bad Table with the parsed date


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| date | [string](#string) | True | False | False | The date string |









### Runtime.RegisterImportHandler

Registers an Import Handler

Returns: [any](#any)


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| cls | [IImportHandler](#iimporthandler) | True | False | False | An Import handler implementing the IImportHandler Interface |









### Runtime.Validate

Validates a source string

Returns: [Table](#table) - Validation Result


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| source | [string](#string) | True | False | False | The Source to Validate |
| file | [string](#string) | True | False | False | The File Name |












## Xml










### Xml.Info




#### Properties

| Name | Value |
| --- | --- |
| AssemblyName |  BadScript2.Interop.Common  |
| Name |  Xml  |
| Version |  25.1.2.1  |













### Xml.Load

Loads an XML Document from a string

Returns: [any](#any) - XmlDocument


#### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| xml | [string](#string) | True | False | False |  |












## Types

An exhaustive list of default types/interfaces available.



### Array




Base Type: [any](#any)


	



#### Interfaces
- [IArray](#iarray)




#### Properties

| Name | Value |
| --- | --- |
| Length |  < multi line content >  |








#### Array.Add



Returns: [any](#any)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| elem | [any](#any) | False | False | False |  |










#### Array.AddRange



Returns: [any](#any)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| elems | [any](#any) | False | False | False |  |










#### Array.All



Returns: [bool](#bool)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| selector | [Function](#function) | True | False | False |  |










#### Array.Any



Returns: [bool](#bool)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| filter | [any](#any) | False | True | False |  |










#### Array.Append



Returns: [IEnumerable](#ienumerable)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| element | [any](#any) | True | False | False |  |










#### Array.Clear



Returns: [any](#any)









#### Array.Concat



Returns: [IEnumerable](#ienumerable)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| selector | [IEnumerable](#ienumerable) | True | False | False |  |










#### Array.Contains



Returns: [bool](#bool)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| elem | [any](#any) | False | False | False |  |










#### Array.Count



Returns: [num](#num)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| predicate | [any](#any) | False | True | False |  |










#### Array.ElementAt



Returns: [any](#any)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| index | [num](#num) | True | False | False |  |










#### Array.ElementAtOrDefault



Returns: [any](#any)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| index | [num](#num) | True | False | False |  |










#### Array.First



Returns: [any](#any)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| selector | [any](#any) | False | True | False |  |










#### Array.FirstOrDefault



Returns: [any](#any)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| selector | [any](#any) | False | True | False |  |










#### Array.Get



Returns: [any](#any)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| index | [any](#any) | False | False | False |  |










#### Array.GetEnumerator



Returns: [IEnumerator](#ienumerator)









#### Array.GetType



Returns: [Type](#type)









#### Array.GroupBy



Returns: [any](#any)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| selector | [Function](#function) | True | False | False |  |










#### Array.Insert



Returns: [any](#any)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| index | [num](#num) | True | False | False |  |
| elem | [any](#any) | True | False | False |  |










#### Array.InsertRange



Returns: [any](#any)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| index | [any](#any) | False | False | False |  |
| elems | [any](#any) | False | False | False |  |










#### Array.IsInstanceOf



Returns: [bool](#bool)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| prototype | [any](#any) | True | False | False |  |










#### Array.Last



Returns: [any](#any)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| selector | [any](#any) | False | True | False |  |










#### Array.LastOrDefault



Returns: [any](#any)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| selector | [any](#any) | False | True | False |  |










#### Array.OrderBy



Returns: [IEnumerable](#ienumerable)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| selector | [IEnumerable](#ienumerable) | True | False | False |  |










#### Array.Remove



Returns: [bool](#bool)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| elem | [any](#any) | False | False | False |  |










#### Array.RemoveAt



Returns: [any](#any)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| index | [any](#any) | False | False | False |  |










#### Array.Reverse



Returns: [any](#any)









#### Array.Select



Returns: [IEnumerable](#ienumerable)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| selector | [Function](#function) | True | False | False |  |










#### Array.SelectMany



Returns: [any](#any)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| selector | [Function](#function) | True | False | False |  |










#### Array.Set



Returns: [any](#any)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| index | [num](#num) | True | False | False |  |
| elem | [any](#any) | True | False | False |  |










#### Array.Skip



Returns: [IEnumerable](#ienumerable)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| count | [num](#num) | True | False | False |  |










#### Array.SkipLast



Returns: [IEnumerable](#ienumerable)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| count | [num](#num) | True | False | False |  |










#### Array.Sum



Returns: [num](#num)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| selector | [any](#any) | False | True | False |  |










#### Array.Take



Returns: [IEnumerable](#ienumerable)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| count | [num](#num) | True | False | False |  |










#### Array.ThenBy



Returns: [IEnumerable](#ienumerable)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| selector | [IEnumerable](#ienumerable) | True | False | False |  |










#### Array.ThenByDescending



Returns: [IEnumerable](#ienumerable)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| selector | [IEnumerable](#ienumerable) | True | False | False |  |










#### Array.ToArray



Returns: [Array](#array)









#### Array.ToString



Returns: [string](#string)









#### Array.ToTable



Returns: [Table](#table)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| keySelector | [Function](#function) | True | False | False |  |
| valueSelector | [Function](#function) | True | False | False |  |










#### Array.Where



Returns: [IEnumerable](#ienumerable)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| selector | [Function](#function) | True | False | False |  |










#### Array.op_ArrayAccessReverse



Returns: [any](#any)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| index | [any](#any) | False | False | False |  |













### Capture




Base Type: [any](#any)


	








### Date




Base Type: [any](#any)


	






#### Properties

| Name | Value |
| --- | --- |
| Day |  < multi line content >  |
| DayOfWeek |  < multi line content >  |
| DayOfYear |  < multi line content >  |
| Hour |  < multi line content >  |
| Millisecond |  < multi line content >  |
| Minute |  < multi line content >  |
| Month |  < multi line content >  |
| Offset |  < multi line content >  |
| Second |  < multi line content >  |
| TimeOfDay |  < multi line content >  |
| UnixTimeMilliseconds |  < multi line content >  |
| UnixTimeSeconds |  < multi line content >  |
| Year |  < multi line content >  |








#### Date.Add



Returns: [Date](#date)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| time | [Time](#time) | True | False | False |  |










#### Date.AddDays



Returns: [Date](#date)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| days | [num](#num) | True | False | False |  |










#### Date.AddHours



Returns: [Date](#date)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| hours | [num](#num) | True | False | False |  |










#### Date.AddMilliseconds



Returns: [Date](#date)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| ms | [num](#num) | True | False | False |  |










#### Date.AddMinutes



Returns: [Date](#date)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| minutes | [num](#num) | True | False | False |  |










#### Date.AddMonths



Returns: [Date](#date)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| months | [num](#num) | True | False | False |  |










#### Date.AddSeconds



Returns: [Date](#date)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| seconds | [num](#num) | True | False | False |  |










#### Date.AddTicks



Returns: [Date](#date)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| ticks | [num](#num) | True | False | False |  |










#### Date.AddYears



Returns: [Date](#date)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| years | [num](#num) | True | False | False |  |










#### Date.Format



Returns: [string](#string)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| format | [string](#string) | True | False | False |  |
| timeZone | [string](#string) | False | True | False |  |
| culture | [string](#string) | False | True | False |  |










#### Date.GetType



Returns: [Type](#type)









#### Date.IsInstanceOf



Returns: [bool](#bool)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| prototype | [any](#any) | True | False | False |  |










#### Date.Subtract



Returns: [Date](#date)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| time | [Time](#time) | True | False | False |  |










#### Date.ToLocalTime



Returns: [Date](#date)









#### Date.ToLongDateString



Returns: [string](#string)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| timeZone | [string](#string) | False | True | False |  |










#### Date.ToLongTimeString



Returns: [string](#string)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| timeZone | [string](#string) | False | True | False |  |










#### Date.ToOffset



Returns: [Date](#date)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| offset | [Time](#time) | True | False | False |  |










#### Date.ToShortDateString



Returns: [string](#string)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| timeZone | [string](#string) | False | True | False |  |










#### Date.ToShortTimeString



Returns: [string](#string)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| timeZone | [string](#string) | False | True | False |  |










#### Date.ToString



Returns: [string](#string)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| format | [string](#string) | True | True | False |  |
| culture | [string](#string) | True | True | False |  |










#### Date.ToUniversalTime



Returns: [Date](#date)









#### Date.WeekOfYear



Returns: [num](#num)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| culture | [string](#string) | False | True | False |  |










#### Date.op_Add



Returns: [Date](#date)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| time | [Time](#time) | True | False | False |  |










#### Date.op_Equal



Returns: [bool](#bool)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| right | [any](#any) | False | False | False |  |










#### Date.op_Greater



Returns: [bool](#bool)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| right | [Date](#date) | False | False | False |  |










#### Date.op_GreaterOrEqual



Returns: [bool](#bool)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| right | [Date](#date) | False | False | False |  |










#### Date.op_Less



Returns: [bool](#bool)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| right | [Date](#date) | False | False | False |  |










#### Date.op_LessOrEqual



Returns: [bool](#bool)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| right | [Date](#date) | False | False | False |  |










#### Date.op_NotEqual



Returns: [bool](#bool)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| right | [any](#any) | False | False | False |  |










#### Date.op_Subtract



Returns: [Date](#date)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| time | [Time](#time) | True | False | False |  |













### Error




Base Type: [any](#any)


	








### Function




Base Type: [any](#any)


	








### Group




Base Type: [any](#any)


	








### IArray





	



#### Interfaces
- [IEnumerable](#ienumerable)






### IAttribute





	








### IChangeAttribute





	



#### Interfaces
- [IAttribute](#iattribute)






### IChangedAttribute





	



#### Interfaces
- [IAttribute](#iattribute)






### IDisposable





	








### IEnumerable





	








### IEnumerator





	








### IImportHandler





	








### IInitializeAttribute





	



#### Interfaces
- [IAttribute](#iattribute)






### IMemberChangeEventArgs





	








### IMemberChangedEventArgs





	



#### Interfaces
- [IMemberChangeEventArgs](#imemberchangeeventargs)






### IMemberChangingEventArgs





	



#### Interfaces
- [IMemberChangeEventArgs](#imemberchangeeventargs)






### Match




Base Type: [any](#any)


	








### MemberInfo




Base Type: [any](#any)


	








### Regex




Base Type: [any](#any)


	








### Scope




Base Type: [any](#any)


	












#### Scope.GetLocals



Returns: [Table](#table)









#### Scope.GetParent



Returns: [Scope](#scope)









#### Scope.GetType



Returns: [Type](#type)









#### Scope.IsInstanceOf



Returns: [bool](#bool)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| prototype | [any](#any) | True | False | False |  |










#### Scope.ToString



Returns: [string](#string)












### Table




Base Type: [any](#any)


	



#### Interfaces
- [IEnumerable](#ienumerable)













### Task




Base Type: [any](#any)


	








### Time




Base Type: [any](#any)


	






#### Properties

| Name | Value |
| --- | --- |
| Hours |  < multi line content >  |
| Milliseconds |  < multi line content >  |
| Minutes |  < multi line content >  |
| Seconds |  < multi line content >  |
| Ticks |  < multi line content >  |
| TotalHours |  < multi line content >  |
| TotalMilliseconds |  < multi line content >  |
| TotalMinutes |  < multi line content >  |
| TotalSeconds |  < multi line content >  |








#### Time.Add



Returns: [Time](#time)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| time | [Time](#time) | True | False | False |  |










#### Time.GetType



Returns: [Type](#type)









#### Time.IsInstanceOf



Returns: [bool](#bool)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| prototype | [any](#any) | True | False | False |  |










#### Time.Negate



Returns: [Time](#time)









#### Time.Subtract



Returns: [Time](#time)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| time | [Time](#time) | True | False | False |  |










#### Time.ToString



Returns: [string](#string)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| format | [string](#string) | True | True | False |  |
| culture | [string](#string) | True | True | False |  |










#### Time.op_Equal



Returns: [bool](#bool)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| right | [any](#any) | False | False | False |  |










#### Time.op_Greater



Returns: [bool](#bool)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| right | [Time](#time) | False | False | False |  |










#### Time.op_GreaterOrEqual



Returns: [bool](#bool)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| right | [Time](#time) | False | False | False |  |










#### Time.op_Less



Returns: [bool](#bool)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| right | [Time](#time) | False | False | False |  |










#### Time.op_LessOrEqual



Returns: [bool](#bool)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| right | [Time](#time) | False | False | False |  |










#### Time.op_NotEqual



Returns: [bool](#bool)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| right | [any](#any) | False | False | False |  |













### Type




Base Type: [any](#any)


	








### Version




Base Type: [any](#any)


	












#### Version.GetType



Returns: [Type](#type)









#### Version.IsInstanceOf



Returns: [bool](#bool)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| prototype | [any](#any) | True | False | False |  |










#### Version.ToString



Returns: [string](#string)












### any

This class represents the any type. It is the base class for all types and can be used to represent any type.



	








### bool




Base Type: [any](#any)


	












#### bool.GetType



Returns: [Type](#type)









#### bool.IsInstanceOf



Returns: [bool](#bool)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| prototype | [any](#any) | True | False | False |  |










#### bool.ToString



Returns: [string](#string)












### num




Base Type: [any](#any)


	












#### num.GetType



Returns: [Type](#type)









#### num.IsInstanceOf



Returns: [bool](#bool)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| prototype | [any](#any) | True | False | False |  |










#### num.ToString



Returns: [string](#string)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| format | [string](#string) | True | True | False |  |
| culture | [string](#string) | True | True | False |  |













### string




Base Type: [any](#any)


	






#### Properties

| Name | Value |
| --- | --- |
| IsDigits |  < multi line content >  |
| IsLetters |  < multi line content >  |
| IsWhiteSpace |  < multi line content >  |
| Length |  < multi line content >  |








#### string.All



Returns: [bool](#bool)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| selector | [Function](#function) | True | False | False |  |










#### string.Any



Returns: [bool](#bool)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| filter | [any](#any) | False | True | False |  |










#### string.Append



Returns: [IEnumerable](#ienumerable)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| element | [any](#any) | True | False | False |  |










#### string.CharCodeAt



Returns: [num](#num)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| index | [any](#any) | False | False | False |  |










#### string.Concat



Returns: [IEnumerable](#ienumerable)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| selector | [IEnumerable](#ienumerable) | True | False | False |  |










#### string.Contains



Returns: [bool](#bool)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| str | [string](#string) | True | False | False |  |










#### string.Count



Returns: [num](#num)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| predicate | [any](#any) | False | True | False |  |










#### string.ElementAt



Returns: [any](#any)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| index | [num](#num) | True | False | False |  |










#### string.ElementAtOrDefault



Returns: [any](#any)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| index | [num](#num) | True | False | False |  |










#### string.EndsWith



Returns: [bool](#bool)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| str | [string](#string) | True | False | False |  |










#### string.First



Returns: [any](#any)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| selector | [any](#any) | False | True | False |  |










#### string.FirstOrDefault



Returns: [any](#any)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| selector | [any](#any) | False | True | False |  |










#### string.Format



Returns: [string](#string)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| args | [any](#any) | False | False | True |  |










#### string.GetEnumerator



Returns: [IEnumerator](#ienumerator)









#### string.GetType



Returns: [Type](#type)









#### string.GroupBy



Returns: [any](#any)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| selector | [Function](#function) | True | False | False |  |










#### string.IndexOf



Returns: [num](#num)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| str | [string](#string) | True | False | False |  |










#### string.Insert



Returns: [string](#string)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| index | [any](#any) | False | False | False |  |
| str | [any](#any) | False | False | False |  |










#### string.IsInstanceOf



Returns: [bool](#bool)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| prototype | [any](#any) | True | False | False |  |










#### string.Last



Returns: [any](#any)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| selector | [any](#any) | False | True | False |  |










#### string.LastIndexOf



Returns: [num](#num)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| str | [string](#string) | True | False | False |  |










#### string.LastOrDefault



Returns: [any](#any)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| selector | [any](#any) | False | True | False |  |










#### string.OrderBy



Returns: [IEnumerable](#ienumerable)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| selector | [IEnumerable](#ienumerable) | True | False | False |  |










#### string.PadLeft

Returns a new string that right-aligns the characters in this instance by padding them with spaces on the left, for a specified total length.

Returns: [string](#string) - A new string that is equivalent to this instance, but right-aligned and padded on the left with as many spaces as needed to create a length of totalWidth. However, if totalWidth is less than the length of this instance, the method returns a reference to the existing instance. If totalWidth is equal to the length of this instance, the method returns a new string that is identical to this instance.


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| padding | [num](#num) | True | False | False | The number of characters in the resulting string, equal to the number of original characters plus any additional padding characters. |
| character | [string](#string) | False | True | False | A character to use as padding. |










#### string.PadRight

Returns a new string that left-aligns the characters in this string by padding them with spaces on the right, for a specified total length.

Returns: [string](#string) - A new string that is equivalent to this instance, but left-aligned and padded on the right with as many spaces as needed to create a length of totalWidth. However, if totalWidth is less than the length of this instance, the method returns a reference to the existing instance. If totalWidth is equal to the length of this instance, the method returns a new string that is identical to this instance.


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| padding | [num](#num) | True | False | False | The number of characters in the resulting string, equal to the number of original characters plus any additional padding characters. |
| character | [string](#string) | False | True | False |  |










#### string.Remove



Returns: [string](#string)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| start | [any](#any) | False | False | False |  |
| count | [any](#any) | False | False | False |  |










#### string.Replace



Returns: [string](#string)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| oldStr | [string](#string) | True | False | False |  |
| newStr | [string](#string) | True | False | False |  |










#### string.Reverse



Returns: [any](#any)









#### string.Select



Returns: [IEnumerable](#ienumerable)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| selector | [Function](#function) | True | False | False |  |










#### string.SelectMany



Returns: [any](#any)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| selector | [Function](#function) | True | False | False |  |










#### string.Skip



Returns: [IEnumerable](#ienumerable)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| count | [num](#num) | True | False | False |  |










#### string.SkipLast



Returns: [IEnumerable](#ienumerable)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| count | [num](#num) | True | False | False |  |










#### string.Split



Returns: [Array](#array)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| splitStr | [any](#any) | False | False | False |  |
| skipEmpty | [any](#any) | False | True | False |  |










#### string.StartsWith



Returns: [bool](#bool)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| str | [string](#string) | True | False | False |  |










#### string.Substring



Returns: [string](#string)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| start | [any](#any) | False | False | False |  |
| end | [any](#any) | False | False | False |  |










#### string.Sum



Returns: [num](#num)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| selector | [any](#any) | False | True | False |  |










#### string.Take



Returns: [IEnumerable](#ienumerable)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| count | [num](#num) | True | False | False |  |










#### string.ThenBy



Returns: [IEnumerable](#ienumerable)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| selector | [IEnumerable](#ienumerable) | True | False | False |  |










#### string.ThenByDescending



Returns: [IEnumerable](#ienumerable)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| selector | [IEnumerable](#ienumerable) | True | False | False |  |










#### string.ToArray



Returns: [Array](#array)









#### string.ToLower



Returns: [string](#string)









#### string.ToString



Returns: [string](#string)









#### string.ToTable



Returns: [Table](#table)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| keySelector | [Function](#function) | True | False | False |  |
| valueSelector | [Function](#function) | True | False | False |  |










#### string.ToUpper



Returns: [string](#string)









#### string.Trim



Returns: [string](#string)









#### string.TrimEnd



Returns: [string](#string)









#### string.TrimStart



Returns: [string](#string)









#### string.Where



Returns: [IEnumerable](#ienumerable)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| selector | [Function](#function) | True | False | False |  |










#### string.op_ArrayAccessReverse



Returns: [any](#any)


##### Parameters

| Name | Type | Null Checked | Optional | Rest Args | Description |
| --- | --- | --- | --- | --- | --- |
| index | [any](#any) | False | False | False |  |













### void





	








# Table of Contents

- [BadScript2 API Reference](#badscript2-api-reference)
	- [BadHtml](#badhtml)
		- [BadHtml.Info](#badhtmlinfo)
		- [BadHtml.Run](#badhtmlrun)
	- [Compression](#compression)
		- [Compression.Base64](#compressionbase64)
			- [Compression.Base64.Decode](#compressionbase64decode)
			- [Compression.Base64.Encode](#compressionbase64encode)
		- [Compression.Deflate](#compressiondeflate)
			- [Compression.Deflate.Compress](#compressiondeflatecompress)
			- [Compression.Deflate.Decompress](#compressiondeflatedecompress)
		- [Compression.GZip](#compressiongzip)
			- [Compression.GZip.Compress](#compressiongzipcompress)
			- [Compression.GZip.Decompress](#compressiongzipdecompress)
		- [Compression.Info](#compressioninfo)
		- [Compression.ZLib](#compressionzlib)
			- [Compression.ZLib.Compress](#compressionzlibcompress)
			- [Compression.ZLib.Decompress](#compressionzlibdecompress)
		- [Compression.Zip](#compressionzip)
			- [Compression.Zip.FromDirectory](#compressionzipfromdirectory)
			- [Compression.Zip.ToDirectory](#compressionziptodirectory)
	- [Concurrent](#concurrent)
		- [Concurrent.Create](#concurrentcreate)
		- [Concurrent.GetCurrent](#concurrentgetcurrent)
		- [Concurrent.Info](#concurrentinfo)
		- [Concurrent.Run](#concurrentrun)
	- [Console](#console)
		- [Console.Clear](#consoleclear)
		- [Console.Info](#consoleinfo)
		- [Console.ReadLine](#consolereadline)
		- [Console.ReadLineAsync](#consolereadlineasync)
		- [Console.Write](#consolewrite)
		- [Console.WriteLine](#consolewriteline)
	- [IO](#io)
		- [IO.Directory](#iodirectory)
			- [IO.Directory.Copy](#iodirectorycopy)
			- [IO.Directory.CreateDirectory](#iodirectorycreatedirectory)
			- [IO.Directory.Delete](#iodirectorydelete)
			- [IO.Directory.Exists](#iodirectoryexists)
			- [IO.Directory.GetCurrentDirectory](#iodirectorygetcurrentdirectory)
			- [IO.Directory.GetDirectories](#iodirectorygetdirectories)
			- [IO.Directory.GetFiles](#iodirectorygetfiles)
			- [IO.Directory.GetStartupDirectory](#iodirectorygetstartupdirectory)
			- [IO.Directory.Move](#iodirectorymove)
			- [IO.Directory.SetCurrentDirectory](#iodirectorysetcurrentdirectory)
		- [IO.File](#iofile)
			- [IO.File.Copy](#iofilecopy)
			- [IO.File.Delete](#iofiledelete)
			- [IO.File.Exists](#iofileexists)
			- [IO.File.Move](#iofilemove)
			- [IO.File.ReadAllBytes](#iofilereadallbytes)
			- [IO.File.ReadAllLines](#iofilereadalllines)
			- [IO.File.ReadAllText](#iofilereadalltext)
			- [IO.File.WriteAllBytes](#iofilewriteallbytes)
			- [IO.File.WriteAllLines](#iofilewritealllines)
			- [IO.File.WriteAllText](#iofilewritealltext)
		- [IO.Info](#ioinfo)
		- [IO.Path](#iopath)
			- [IO.Path.ChangeExtension](#iopathchangeextension)
			- [IO.Path.Combine](#iopathcombine)
			- [IO.Path.Expand](#iopathexpand)
			- [IO.Path.GetDirectoryName](#iopathgetdirectoryname)
			- [IO.Path.GetExtension](#iopathgetextension)
			- [IO.Path.GetFileName](#iopathgetfilename)
			- [IO.Path.GetFileNameWithoutExtension](#iopathgetfilenamewithoutextension)
			- [IO.Path.GetFullPath](#iopathgetfullpath)
			- [IO.Path.GetStartupPath](#iopathgetstartuppath)
	- [Json](#json)
		- [Json.FromJson](#jsonfromjson)
		- [Json.FromYaml](#jsonfromyaml)
		- [Json.Info](#jsoninfo)
		- [Json.ToJson](#jsontojson)
		- [Json.ToYaml](#jsontoyaml)
	- [Math](#math)
		- [Math.Abs](#mathabs)
		- [Math.Acos](#mathacos)
		- [Math.Asin](#mathasin)
		- [Math.Atan](#mathatan)
		- [Math.Atan2](#mathatan2)
		- [Math.Ceiling](#mathceiling)
		- [Math.Cos](#mathcos)
		- [Math.Cosh](#mathcosh)
		- [Math.Exp](#mathexp)
		- [Math.Floor](#mathfloor)
		- [Math.Info](#mathinfo)
		- [Math.Log](#mathlog)
		- [Math.Log10](#mathlog10)
		- [Math.Max](#mathmax)
		- [Math.Min](#mathmin)
		- [Math.NextRandom](#mathnextrandom)
		- [Math.Pow](#mathpow)
		- [Math.Round](#mathround)
		- [Math.Sign](#mathsign)
		- [Math.Sin](#mathsin)
		- [Math.Sinh](#mathsinh)
		- [Math.Sqrt](#mathsqrt)
		- [Math.Tan](#mathtan)
		- [Math.Tanh](#mathtanh)
		- [Math.Truncate](#mathtruncate)
	- [Net](#net)
		- [Net.DecodeUriComponent](#netdecodeuricomponent)
		- [Net.EncodeUriComponent](#netencodeuricomponent)
		- [Net.Get](#netget)
		- [Net.Info](#netinfo)
		- [Net.Post](#netpost)
	- [NetHost](#nethost)
		- [NetHost.Create](#nethostcreate)
		- [NetHost.Info](#nethostinfo)
	- [OS](#os)
		- [OS.Environment](#osenvironment)
			- [OS.Environment.Exit](#osenvironmentexit)
			- [OS.Environment.ExpandEnvironmentVariables](#osenvironmentexpandenvironmentvariables)
			- [OS.Environment.FailFast](#osenvironmentfailfast)
			- [OS.Environment.GetCommandLineArguments](#osenvironmentgetcommandlinearguments)
			- [OS.Environment.GetEnvironmentVariable](#osenvironmentgetenvironmentvariable)
			- [OS.Environment.GetEnvironmentVariables](#osenvironmentgetenvironmentvariables)
			- [OS.Environment.GetLogicalDrives](#osenvironmentgetlogicaldrives)
			- [OS.Environment.SetEnvironmentVariable](#osenvironmentsetenvironmentvariable)
		- [OS.Info](#osinfo)
		- [OS.Run](#osrun)
	- [Runtime](#runtime)
		- [Runtime.CreateDefaultScope](#runtimecreatedefaultscope)
		- [Runtime.EvaluateAsync](#runtimeevaluateasync)
		- [Runtime.GetArguments](#runtimegetarguments)
		- [Runtime.GetExtensionNames](#runtimegetextensionnames)
		- [Runtime.GetExtensions](#runtimegetextensions)
		- [Runtime.GetGlobalExtensionNames](#runtimegetglobalextensionnames)
		- [Runtime.GetMembers](#runtimegetmembers)
		- [Runtime.GetNativeTypes](#runtimegetnativetypes)
		- [Runtime.GetRegisteredApis](#runtimegetregisteredapis)
		- [Runtime.GetRootScope](#runtimegetrootscope)
		- [Runtime.GetRuntimeAssemblyPath](#runtimegetruntimeassemblypath)
		- [Runtime.GetStackTrace](#runtimegetstacktrace)
		- [Runtime.GetTimeNow](#runtimegettimenow)
		- [Runtime.ImportAsync](#runtimeimportasync)
		- [Runtime.Info](#runtimeinfo)
		- [Runtime.IsApiRegistered](#runtimeisapiregistered)
		- [Runtime.MakeReference](#runtimemakereference)
		- [Runtime.Native](#runtimenative)
			- [Runtime.Native.IsArray](#runtimenativeisarray)
			- [Runtime.Native.IsBoolean](#runtimenativeisboolean)
			- [Runtime.Native.IsEnumerable](#runtimenativeisenumerable)
			- [Runtime.Native.IsEnumerator](#runtimenativeisenumerator)
			- [Runtime.Native.IsFunction](#runtimenativeisfunction)
			- [Runtime.Native.IsNative](#runtimenativeisnative)
			- [Runtime.Native.IsNumber](#runtimenativeisnumber)
			- [Runtime.Native.IsPrototype](#runtimenativeisprototype)
			- [Runtime.Native.IsPrototypeInstance](#runtimenativeisprototypeinstance)
			- [Runtime.Native.IsString](#runtimenativeisstring)
			- [Runtime.Native.IsTable](#runtimenativeistable)
		- [Runtime.NewGuid](#runtimenewguid)
		- [Runtime.ParseDate](#runtimeparsedate)
		- [Runtime.RegisterImportHandler](#runtimeregisterimporthandler)
		- [Runtime.Validate](#runtimevalidate)
	- [Xml](#xml)
		- [Xml.Info](#xmlinfo)
		- [Xml.Load](#xmlload)
	- [Types](#types)
		- [Array](#array)
			- [Array.Add](#arrayadd)
			- [Array.AddRange](#arrayaddrange)
			- [Array.All](#arrayall)
			- [Array.Any](#arrayany)
			- [Array.Append](#arrayappend)
			- [Array.Clear](#arrayclear)
			- [Array.Concat](#arrayconcat)
			- [Array.Contains](#arraycontains)
			- [Array.Count](#arraycount)
			- [Array.ElementAt](#arrayelementat)
			- [Array.ElementAtOrDefault](#arrayelementatordefault)
			- [Array.First](#arrayfirst)
			- [Array.FirstOrDefault](#arrayfirstordefault)
			- [Array.Get](#arrayget)
			- [Array.GetEnumerator](#arraygetenumerator)
			- [Array.GetType](#arraygettype)
			- [Array.GroupBy](#arraygroupby)
			- [Array.Insert](#arrayinsert)
			- [Array.InsertRange](#arrayinsertrange)
			- [Array.IsInstanceOf](#arrayisinstanceof)
			- [Array.Last](#arraylast)
			- [Array.LastOrDefault](#arraylastordefault)
			- [Array.OrderBy](#arrayorderby)
			- [Array.Remove](#arrayremove)
			- [Array.RemoveAt](#arrayremoveat)
			- [Array.Reverse](#arrayreverse)
			- [Array.Select](#arrayselect)
			- [Array.SelectMany](#arrayselectmany)
			- [Array.Set](#arrayset)
			- [Array.Skip](#arrayskip)
			- [Array.SkipLast](#arrayskiplast)
			- [Array.Sum](#arraysum)
			- [Array.Take](#arraytake)
			- [Array.ThenBy](#arraythenby)
			- [Array.ThenByDescending](#arraythenbydescending)
			- [Array.ToArray](#arraytoarray)
			- [Array.ToString](#arraytostring)
			- [Array.ToTable](#arraytotable)
			- [Array.Where](#arraywhere)
			- [Array.op_ArrayAccessReverse](#arrayop_arrayaccessreverse)
		- [Capture](#capture)
		- [Date](#date)
			- [Date.Add](#dateadd)
			- [Date.AddDays](#dateadddays)
			- [Date.AddHours](#dateaddhours)
			- [Date.AddMilliseconds](#dateaddmilliseconds)
			- [Date.AddMinutes](#dateaddminutes)
			- [Date.AddMonths](#dateaddmonths)
			- [Date.AddSeconds](#dateaddseconds)
			- [Date.AddTicks](#dateaddticks)
			- [Date.AddYears](#dateaddyears)
			- [Date.Format](#dateformat)
			- [Date.GetType](#dategettype)
			- [Date.IsInstanceOf](#dateisinstanceof)
			- [Date.Subtract](#datesubtract)
			- [Date.ToLocalTime](#datetolocaltime)
			- [Date.ToLongDateString](#datetolongdatestring)
			- [Date.ToLongTimeString](#datetolongtimestring)
			- [Date.ToOffset](#datetooffset)
			- [Date.ToShortDateString](#datetoshortdatestring)
			- [Date.ToShortTimeString](#datetoshorttimestring)
			- [Date.ToString](#datetostring)
			- [Date.ToUniversalTime](#datetouniversaltime)
			- [Date.WeekOfYear](#dateweekofyear)
			- [Date.op_Add](#dateop_add)
			- [Date.op_Equal](#dateop_equal)
			- [Date.op_Greater](#dateop_greater)
			- [Date.op_GreaterOrEqual](#dateop_greaterorequal)
			- [Date.op_Less](#dateop_less)
			- [Date.op_LessOrEqual](#dateop_lessorequal)
			- [Date.op_NotEqual](#dateop_notequal)
			- [Date.op_Subtract](#dateop_subtract)
		- [Error](#error)
		- [Function](#function)
		- [Group](#group)
		- [IArray](#iarray)
		- [IAttribute](#iattribute)
		- [IChangeAttribute](#ichangeattribute)
		- [IChangedAttribute](#ichangedattribute)
		- [IDisposable](#idisposable)
		- [IEnumerable](#ienumerable)
		- [IEnumerator](#ienumerator)
		- [IImportHandler](#iimporthandler)
		- [IInitializeAttribute](#iinitializeattribute)
		- [IMemberChangeEventArgs](#imemberchangeeventargs)
		- [IMemberChangedEventArgs](#imemberchangedeventargs)
		- [IMemberChangingEventArgs](#imemberchangingeventargs)
		- [Match](#match)
		- [MemberInfo](#memberinfo)
		- [Regex](#regex)
		- [Scope](#scope)
			- [Scope.GetLocals](#scopegetlocals)
			- [Scope.GetParent](#scopegetparent)
			- [Scope.GetType](#scopegettype)
			- [Scope.IsInstanceOf](#scopeisinstanceof)
			- [Scope.ToString](#scopetostring)
		- [Table](#table)
		- [Task](#task)
		- [Time](#time)
			- [Time.Add](#timeadd)
			- [Time.GetType](#timegettype)
			- [Time.IsInstanceOf](#timeisinstanceof)
			- [Time.Negate](#timenegate)
			- [Time.Subtract](#timesubtract)
			- [Time.ToString](#timetostring)
			- [Time.op_Equal](#timeop_equal)
			- [Time.op_Greater](#timeop_greater)
			- [Time.op_GreaterOrEqual](#timeop_greaterorequal)
			- [Time.op_Less](#timeop_less)
			- [Time.op_LessOrEqual](#timeop_lessorequal)
			- [Time.op_NotEqual](#timeop_notequal)
		- [Type](#type)
		- [Version](#version)
			- [Version.GetType](#versiongettype)
			- [Version.IsInstanceOf](#versionisinstanceof)
			- [Version.ToString](#versiontostring)
		- [any](#any)
		- [bool](#bool)
			- [bool.GetType](#boolgettype)
			- [bool.IsInstanceOf](#boolisinstanceof)
			- [bool.ToString](#booltostring)
		- [num](#num)
			- [num.GetType](#numgettype)
			- [num.IsInstanceOf](#numisinstanceof)
			- [num.ToString](#numtostring)
		- [string](#string)
			- [string.All](#stringall)
			- [string.Any](#stringany)
			- [string.Append](#stringappend)
			- [string.CharCodeAt](#stringcharcodeat)
			- [string.Concat](#stringconcat)
			- [string.Contains](#stringcontains)
			- [string.Count](#stringcount)
			- [string.ElementAt](#stringelementat)
			- [string.ElementAtOrDefault](#stringelementatordefault)
			- [string.EndsWith](#stringendswith)
			- [string.First](#stringfirst)
			- [string.FirstOrDefault](#stringfirstordefault)
			- [string.Format](#stringformat)
			- [string.GetEnumerator](#stringgetenumerator)
			- [string.GetType](#stringgettype)
			- [string.GroupBy](#stringgroupby)
			- [string.IndexOf](#stringindexof)
			- [string.Insert](#stringinsert)
			- [string.IsInstanceOf](#stringisinstanceof)
			- [string.Last](#stringlast)
			- [string.LastIndexOf](#stringlastindexof)
			- [string.LastOrDefault](#stringlastordefault)
			- [string.OrderBy](#stringorderby)
			- [string.PadLeft](#stringpadleft)
			- [string.PadRight](#stringpadright)
			- [string.Remove](#stringremove)
			- [string.Replace](#stringreplace)
			- [string.Reverse](#stringreverse)
			- [string.Select](#stringselect)
			- [string.SelectMany](#stringselectmany)
			- [string.Skip](#stringskip)
			- [string.SkipLast](#stringskiplast)
			- [string.Split](#stringsplit)
			- [string.StartsWith](#stringstartswith)
			- [string.Substring](#stringsubstring)
			- [string.Sum](#stringsum)
			- [string.Take](#stringtake)
			- [string.ThenBy](#stringthenby)
			- [string.ThenByDescending](#stringthenbydescending)
			- [string.ToArray](#stringtoarray)
			- [string.ToLower](#stringtolower)
			- [string.ToString](#stringtostring)
			- [string.ToTable](#stringtotable)
			- [string.ToUpper](#stringtoupper)
			- [string.Trim](#stringtrim)
			- [string.TrimEnd](#stringtrimend)
			- [string.TrimStart](#stringtrimstart)
			- [string.Where](#stringwhere)
			- [string.op_ArrayAccessReverse](#stringop_arrayaccessreverse)
		- [void](#void)
- [Table of Contents](#table-of-contents)

