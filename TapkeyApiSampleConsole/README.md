# Tapkey API Console Sample
An exemplary application that demonstrates accessing Tapkey's API via a .NET Core 2.0 console
application. This application will print a list of all locking devices bound to a user's owner
account.

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

## Example output
```
Lock ID: 43301e56-••••-••••-••••-••••••••••••
Title: Sebastian's Lock
Description: This is my first Tapkey lock.
Lock is active: True
Lock model name: Td20
Binding date: 02-Jun-17 8:18:56
```

## Structure
All relevant code for retrieving information about locking devices is located in `Program.cs`. Those
classes inside the `OAuth2` directory are being used for OAuth2.0 authorization only. Even more
specific, `QueryBoundLocks()` contains the steps required for retrieving and displaying data.

### Outline
1. Authorize against Tapkey's OAuth 2.0 service
2. Retrieve owner accounts for the present user, and select the first one (this is a demo)
3. Retrieve bound locks for that owner account
4. Display information

# License
Copyright (c) Tapkey GmbH. All rights reserved.

Licensed under the [Apache License 2.0](https://spdx.org/licenses/Apache-2.0.html)

[1]: https://developers.google.com/api-client-library/dotnet/
[2]: https://docs.microsoft.com/en-us/aspnet/core/api/microsoft.extensions.commandlineutils
