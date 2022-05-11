# How to: Client Credentials (.NET 6, Console application)

## Client credentials flow

This console application demonstrates how to consume the Tapkey Management API from an application using the client credentials flow. 

The Client Credentials flow is typically used when the client (the application) is acting on its own behalf. This means that there's no actual user/person interaction in the process. The client (application) is the owner of the resources.

## What does this application do?

This application will print a list of all owner accounts that are managed by this OAuth client (as a co-admin in the main owner account), their locking products and let the user assign an access grant for one day for defined email address.
This code was developed as part of How To series on Tapkey Youtube chanell. To see the development step by step, have a look on the video.

## Prerequisites

### Register your OAuth client with Tapkey

You need to register your client application in the [Tapkey Integrator Portal](https://portal.tapkey.io) in OAuth Clients section.

For that **make sure you**:

1. Select the `Client Credentials` flow
2. Select "ReadOnly" for the `Owners` permission and "ReadAndWrite" for `Core Entities` and `Grants`
3. Make sure to select "Administrator of this locking system" to give the OAuth client co-admin permissions.
3. Upon saving, you'll receive the client credentials (id and secret). Please save this in a secure way. Tapkey does not store the secret. If you lose it, you'll have to reset it.
4. Optionally, give access as "co-admin" to your client to another existing owner account. For this, you can use the e-mail address also shown in the client details page

### Technical requirements

- .NET 6

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

### From Visual Studio 2022:

It's always interesting to run the app in Debug mode to see how things work. To do that, you can set the `clientId` and `clientSecret` in the `launchSettings.json` file under `Properties` in the console project:

```
{
  "profiles": {
    "HowToWebApiClientCredentials": {
      "commandName": "Project",
      "commandLineArgs": "-i my-client-id -s my-secret"
    }
  }
}
```

### Outline

1. Authorize against Tapkey's OAuth 2.0 service using the `client_credentials` flow
2. Retrieve the Owner Accounts which this OAuth client has access, and list them all
3. Let the user select an Owner Account he wants to create grant for
4. Retrieve Bound Locks for selected Owner Account
5. Let the user select a Bound Lock he wants to create grant for
4. Ask the user for email address of an user to create grant for
5. Create grant for defined email address and selected bound lock for next 24 hours.

## Technology used

* .NET Core 6
* [CommandLineParser][1] (for command line argument processing)

# License

Copyright (c) Tapkey GmbH. All rights reserved.

Licensed under the [Apache License 2.0](https://spdx.org/licenses/Apache-2.0.html)

[1]: https://github.com/commandlineparser/commandline
