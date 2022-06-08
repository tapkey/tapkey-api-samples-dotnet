# How to: Authorization Code Flow (ASP.NET Core 6, Blazor Server-Side)

## Authorization Code Flow

This console application demonstrates how to consume the Tapkey Management API from an application using the authorization code flow. 

The Authorization Code flow is typically used when the client (the application) is acting as a certain user. This user needs to consent with this application to "act on his behalf" with certain scope.
Scopes help to limit what application is allowed to do with your account.

## What does this application do?

This application will print a list of all owner accounts that are managed by this OAuth client (as a co-admin in the main owner account), their locking products and let the user assign an access grant for one day for defined email address.
This code was developed as part of How To series on Tapkey Youtube chanell. To see the development step by step, have a look on the video.

## Prerequisites

### Register your OAuth client with Tapkey

You need to register your client application in the [Tapkey Integrator Portal](https://portal.tapkey.io) in OAuth Clients section.

For that **make sure you**:

1. Select the `Authorization Code` flow with 'Client secret' authorization type
2. Define 'Logo URL' for your client
3. Define 'Redirect URIs' as 'https://localhost:7259/Auth/LoginCallback'
4. Define 'Allowed CORS origins' as 'https://localhost:7259'
2. In permissions section select "ReadOnly" for the `Owners` permission and "ReadAndWrite" for `Core Entities` and `Grants`
3. Upon saving, you'll receive a client id and a client secret for your Authorization Code client. Please save this in a secure way. Tapkey does not store the secret. If you lose it, you'll have to reset it.

### Technical requirements

- .NET 6

### Usage from Visual Studio 2022:

It's always interesting to run the app in Debug mode to see how things work. To do that, you can set the `clientId` and `clientSecret` in the `appsettings.Development.json`:

```
{
    ...
    "ClientId": "<YOUR_CLIENT_ID>",
    "ClientSecret": "<YOUR_CLIENT_SECRET>"
}
```

### Outline

1. Authorize against Tapkey's OAuth 2.0 service using the `code` flow
2. Retrieve the Owner Accounts which this OAuth client has access to, and list them all
3. Let the user select an Owner Account he wants to create grant for
4. Retrieve Bound Locks for selected Owner Account
5. Let the user select a Bound Lock he wants to create grant for
4. Ask the user for email address of an user to create grant for
5. Create grant for defined email address and selected bound lock for next 24 hours.

## Technology used

* ASP.NET Core 6 Blazor Server-Side

# License

Copyright (c) Tapkey GmbH. All rights reserved.

Licensed under the [Apache License 2.0](https://spdx.org/licenses/Apache-2.0.html)
