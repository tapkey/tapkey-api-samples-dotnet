Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text
Imports IdentityModel.Client
Imports McMaster.Extensions.CommandLineUtils
Imports Newtonsoft.Json
Imports Tapkey.Api.Models

Module Program

    ' Tapkey Api configuration
    Private _tapkeyApiClient As HttpClient = New HttpClient()
    Private Const TapkeyApiUrl As String = "https://my.tapkey.com/"
    Private Const TapkeyApiVersion As String = "api/v1"

    ' Tapkey Authorization Server configuration
    Private _tapkeyAuthServer As HttpClient = New HttpClient()
    Private Const TapkeyAuthorizationServer As String = "https://login.tapkey.com"

    Public Function Main(args As String()) As Integer

        Dim app = New CommandLineApplication()

        Dim clientId = app.Option("-i|--id <clientId>", "Your client ID", CommandOptionType.SingleValue)
        Dim clientSecret = app.Option("-s|--secret <clientSecret>", "Your client secret.", CommandOptionType.SingleValue)

        'pre-configure the TapkeyHttpClient
        _tapkeyApiClient.BaseAddress = New Uri(TapkeyApiUrl)
        _tapkeyApiClient.DefaultRequestHeaders.Add(
                "User-Agent",
                ".NET Core Console Native App Authorization Code with PKCE flow")

        app.OnExecute(
            Async Function() As Task(Of Integer)

                If Not clientId.HasValue() OrElse Not clientSecret.HasValue() Then
                    app.ShowHelp()
                    Return 1
                End If

                Try
                    Dim access_token = Await RequestClientCredentialsToken(clientId.Value(), clientSecret.Value())

                    ' Get the bound locks which are managed by this service account user
                    Await QueryBoundLocksAsync(access_token)
                Catch ex As Exception
                    Console.WriteLine($"ERROR: {ex.Message}")
                End Try

                Return 0

            End Function)

        Return app.Execute(args)

    End Function

    ''' <summary>
    ''' Initiates a client credentials flow and returns the access_token
    ''' </summary>
    ''' <param name="clientId">The client identitifer</param>
    ''' <param name="clientSecret">the client secret</param>
    ''' <returns></returns>
    Private Async Function RequestClientCredentialsToken(clientId As String, clientSecret As String) As Task(Of String)
        'uses the RequestClientCredentialsTokenAsync extension method on IdentityModel2 NuGet package
        Dim response = Await _tapkeyAuthServer.RequestClientCredentialsTokenAsync(New ClientCredentialsTokenRequest With
            {
                .Address = $"{TapkeyAuthorizationServer}/connect/token",
                .ClientId = clientId,
                .ClientSecret = clientSecret,
                .Scope = "read:core:entities read:owneraccounts"
            })

        If response.IsError Then
            Throw New Exception(response.Error)
        End If

        Return response.AccessToken

    End Function

    Private Async Function QueryBoundLocksAsync(access_token As String) As Task

        ' Sets the access_token in the request header
        _tapkeyApiClient.DefaultRequestHeaders.Authorization =
                        New AuthenticationHeaderValue("Bearer", access_token)

        'Get the owner accounts that this service account has access (as co-admin)
        Dim ownersJsonResponse = Await _tapkeyApiClient.GetStringAsync($"{TapkeyApiVersion}/owners")
        Dim ownerAccounts = JsonConvert.DeserializeObject(Of List(Of OwnerAccount))(ownersJsonResponse)

        If Not ownerAccounts.Any Then
            Console.WriteLine("This user has no Owner Accounts.")
            Return
        End If

        For Each ownerAccount As OwnerAccount In ownerAccounts

            ' ... And query for bound locks
            Dim boundLocksJson = Await _tapkeyApiClient.GetStringAsync($"{TapkeyApiVersion}/owners/{ownerAccount.Id}/boundlocks")
            Dim boundLocks = JsonConvert.DeserializeObject(Of List(Of BoundLock))(boundLocksJson)

            If Not boundLocks.Any Then
                Console.WriteLine($"No bound locks for owner account {ownerAccount.Name} ({ownerAccount.Id}).")
                Continue For
            End If

            Console.WriteLine($"Displaying Bound Locks for Owner Account {ownerAccount.Name} ({ownerAccount.Id}):")

            For Each boundLock As BoundLock In boundLocks
                Dim lockInfo = New StringBuilder()
                lockInfo.AppendLine("-------------------------")
                lockInfo.AppendLine($"Lock ID: {boundLock.Id}")
                lockInfo.AppendLine($"Title: {boundLock.Title}")
                lockInfo.AppendLine($"Description: {boundLock.Description}")

                If boundLock.LockType IsNot Nothing Then
                    lockInfo.AppendLine($"Lock model name: {boundLock.LockType.ModelName}")
                End If

                lockInfo.AppendLine($"Binding date: {boundLock.BindDate}")
                lockInfo.AppendLine("-------------------------")
                Console.WriteLine(lockInfo)
            Next
        Next
    End Function

End Module
