# Tapkey Management API Console Sample with Client Credentials (C#, .NET Core 2.2) 

## Client credentials flow

This console application demonstrates how to consume the Tapkey Management API from an application using the client credentials flow. 

The Client Credentials flow is typically used when the client (the application) is acting on its own behalf. This means that there's no actual user/person interaction in the process. The client (application) is the owner of the resources.

## What does this application do?

This application will print a list of all locking products that are managed by this OAuth client (as a co-admin in the main owner account)

## Prerequisites

### Register your OAuth client with Tapkey

You need to register your client application in the Tapkey portal here: [Manage your OAuth clients](https://my.tapkey.com/AdminUI/#/oauth-clients). 

For that **make sure you**:

1. Select the `Client Credentials` flow
2. Select "ReadOnly" for both the `CoreEntities` and `Owners` permissions
3. Upon saving, you'll receive the client credentials (id and secret). Please save this in a secure way. Tapkey does not store the secret. If you lose it, you'll have to reset it.
4. Optionally, give access as "co-admin" to your client to an existing owner account. For this, you can use the e-mail address also shown in the client details page

### Technical requirements

- .NET Core SDK 2.2

## Usage

You can either run the app via the command line or with Visual Studio.

### From the command line:

.NET Core applications can be run from the project root directory using `dotnet run`. The following arguments have to be specified:

```
  -i|--id <clientId> Your client ID.
  -s|--secret <clientSecret> Your client secret.
```

Example:
```
dotnet run -i my-client-id -s my-secret
```

### From Visual Studio 2017 (or upcoming 2019):

It's always interesting to run the app in Debug mode to see how things work. To do that, you can set the `clientId` and `clientSecret` in the `launchSettings.json` file under `Properties` in the console project:

```
{
  "profiles": {
    "ConsoleClientCredentials.NETCore": {
      "commandName": "Project",
      "commandLineArgs": "-i my-client-id -s my-secret"
    }
  }
}
```

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

 The most important methods of the application are the `RequestClientCredentialsToken(string clientId, string clientSecret)` which initiates the client credentials flow for the provided `clientId`. After a successfull authorization, the `QueryBoundLocksAsync(string access_token)` is called and it will use the `access_token` to communicate with the Tapkey Management API.

### Outline

1. Authorize against Tapkey's OAuth 2.0 service using the `client_credentials` flow
2. Retrieve the Owner Accounts which this OAuth client has access, and iterate over them
3. Retrieve Bound Locks for each owner account found
4. Displays the information

## Technology used

* .NET Core 2.2
* [IdentityModel][1] (for handling the Client Credentials flow)
* [McMaster.Extensions.CommandLineUtils][2] (for command line argument processing)

# License

Copyright (c) Tapkey GmbH. All rights reserved.

Licensed under the [Apache License 2.0](https://spdx.org/licenses/Apache-2.0.html)

[1]: https://github.com/IdentityModel/IdentityModel2
[2]: https://github.com/natemcmaster/CommandLineUtils
