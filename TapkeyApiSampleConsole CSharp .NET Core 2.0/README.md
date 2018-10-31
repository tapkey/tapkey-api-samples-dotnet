# Tapkey API Console Sample in C#
An exemplary console application written in C# using .NET Core 2.0, that demonstrates accessing
Tapkey's API. This application will print a list of all locking products bound to the user's Owner
Accounts.

## Prerequisites
You need to create a Tapkey Client Application prior to running this example in order to be able to
authenticate using Tapkey's OAuth 2.0 service.

## Technology used
* .NET Core 2.0
* [Google API .NET Client Library][1] (for handling OAuth 2.0 flow)
* [Microsoft.Extensions.CommandLineUtils][2] (for command line argument processing)

## Usage
Build, and run with the following arguments:
```
Usage:  [options]

Options:
  -i|--id <clientId>          Your client ID.
  -s|--secret <clientSecret>  Your client secret.
```

.NET Core applications can be run from the project root directory using `dotnet run`.

## Example output
```
Displaying Bound Locks for Owner Account Tapkey (6426d8f8-••••-••••-••••-••••••••••••):

Lock ID: 43301e56-••••-••••-••••-••••••••••••
Title: Sebastian's Lock
Description: This is my first Tapkey lock.
Lock model name: Td20
Binding date: 02-Jun-17 8:18:56

```

## Structure
All relevant code for retrieving information about locking devices is located in `Program.cs`. The
classes inside the `OAuthHelpers` project are being used for OAuth2.0 authorization only. More
specific, `QueryBoundLocks()` contains the steps required for retrieving and displaying data.

### Outline
1. Authorize against Tapkey's OAuth 2.0 service
2. Retrieve Owner Accounts for the present user, and iterate over them
3. Retrieve Bound Locks for the current Owner Account
4. Display information

## Token storage
This application will cache access tokens issued by Tapkey. To reset the cache, delete the
`TapkeyApi` folder in your `AppData`.

# License
Copyright (c) Tapkey GmbH. All rights reserved.

Licensed under the [Apache License 2.0](https://spdx.org/licenses/Apache-2.0.html)

[1]: https://developers.google.com/api-client-library/dotnet/
[2]: https://docs.microsoft.com/en-us/aspnet/core/api/microsoft.extensions.commandlineutils
