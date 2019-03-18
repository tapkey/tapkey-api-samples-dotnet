# Tapkey API Console Sample with Client Credentials (VB, .NET Framework 4.7.2) 

## Client credentials flow
This console application demonstrates how to consume the Tapkey API from an application using the client credentials flow. 

The Client Credentials flow is tipically used when the client (the application) is acting on its own behalf. This means that there's no actual user/person interaction in the process. The client (application) is the owner of the resources.

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

- .NET Framework 4.7.2 installed
- At least Visual Studio 2017 (Community version also works)

## Usage

You can either run the app via the command line or with Visual Studio.

### From the command line:

You can run this console app by building the project and executing the `.exe` from the command line passing the following arguments:

```
  -i|--id <clientId> Your client ID.
  -s|--secret <clientSecret> Your client secret.
```

Example:
```
$ cd tapkey-api-samples-dotnet\src\ConsoleClientCredentials.VB.NetFramework\bin\Debug\net472

$ ConsoleClientCredentials.VB.NetFramework.exe -i my-client-id -s my-secret
```

### From Visual Studio 2017 (or upcoming 2019):

To debug this app in Visual Studio, you'll need to set the arguments before hand:

1. Right click in the project > Properties
2. Choose the tab `Debug` on the left menu
3. In "Application arguments", enter the arguments as above. `-i my-clientid -s my-secret`
4. Save, and you can run hitting `F5`


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
 The most important methods of the application are the `RequestClientCredentialsToken(string clientId, string clientSecret)` which initiates the client credentials flow for the provided `clientId`. After a successfull authorization, the `QueryBoundLocksAsync(string access_token)` is called and it will use the `access_token` to communicate with the Tapkey API.

### Outline
1. Authorize against Tapkey's OAuth 2.0 service using the `client_credentials` flow
2. Retrieve the Owner Accounts which this OAuth client has access, and iterate over them
3. Retrieve Bound Locks for each owner account found
4. Displays the information

## Technology used
* .NET Framework 4.7.2
* [IdentityModel][1] (for handling the Client Credentials flow)
* [McMaster.Extensions.CommandLineUtils][2] (for command line argument processing)

# License
Copyright (c) Tapkey GmbH. All rights reserved.

Licensed under the [Apache License 2.0](https://spdx.org/licenses/Apache-2.0.html)

[1]: https://github.com/IdentityModel/IdentityModel2
[2]: https://github.com/natemcmaster/CommandLineUtils
[3]: https://natemcmaster.com/blog/2017/03/09/vs2015-to-vs2017-upgrade/
