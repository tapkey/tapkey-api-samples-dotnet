Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text
Imports Google.Apis.Auth.OAuth2
Imports Microsoft.Extensions.CommandLineUtils
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports OAuthHelpers

Module Program

    Private credential As UserCredential
    Private TapkeyApiUri As Uri = New Uri("https://my.tapkey.com/api/")
    Private TapkeyApiVersion As String = "v1"

    Sub Main(args As String())
        Dim commandLineApplication = New CommandLineApplication(throwOnUnexpectedArg:=False)

        Dim clientId = commandLineApplication.Option(
                "-i|--id <clientId>",
                "Your client ID.",
                CommandOptionType.SingleValue)

        Dim clientSecret = commandLineApplication.Option(
                "-s|--secret <clientSecret>",
                "Your client secret.",
                CommandOptionType.SingleValue)

        commandLineApplication.OnExecute(Function()
                                             If clientId.HasValue() AndAlso clientSecret.HasValue() Then
                                                 MainAsync(clientId.Value(), clientSecret.Value()).Wait()
                                             Else
                                                 commandLineApplication.ShowHelp()
                                             End If
                                             Return 0
                                         End Function)
        commandLineApplication.Execute(args)
    End Sub

    Async Function MainAsync(clientId As String, clientSecret As String) As Task
        Try
            credential = Await AuthorizeTapkey.Run(clientId, clientSecret)
            Await QueryBoundLocks()
        Catch ex As AggregateException
            For Each e In ex.InnerExceptions
                Console.WriteLine("ERROR: " + e.Message)
            Next
        End Try
    End Function

    Async Function QueryBoundLocks() As Task

        Dim client = New HttpClient()

        client.DefaultRequestHeaders.Add(
            "User-Agent",
            "Tapkey API .NET Core Console Sample Application")

        client.DefaultRequestHeaders.Authorization =
                New AuthenticationHeaderValue("Bearer", credential.Token.AccessToken)

        Dim ownerJson = Await client.GetStringAsync(
            $"{New Uri(TapkeyApiUri, TapkeyApiVersion).AbsoluteUri}/owners")
        Dim ownerAccounts = JsonConvert.DeserializeObject(Of List(Of Dictionary(Of String, Object)))(ownerJson)

        ' Check if the current user has at least one owner account
        If ownerAccounts.Count > 0 Then

            ' Loop over user's Owner Accounts...
            For Each ownerAccount In ownerAccounts

                ' ...and query for bound locks
                Dim boundLockJson = Await client.GetStringAsync(
                    $"{New Uri(TapkeyApiUri, TapkeyApiVersion).AbsoluteUri}/owners/{ownerAccount.Item("id")}/boundlocks")
                Dim boundLocks = JsonConvert.DeserializeObject(Of List(Of Dictionary(Of String, Object)))(boundLockJson)

                ' Make sure the selected owner account has bound locks
                If boundLocks.Count > 0 Then

                    Console.WriteLine($"Displaying Bound Locks for Owner Account {ownerAccount.Item("name")} ({ownerAccount.Item("id")}):")

                    ' Print a list of all bound locks
                    For Each boundLock In boundLocks
                        Dim lockInfo = New StringBuilder()
                        lockInfo.AppendLine("-------------------------")
                        lockInfo.AppendLine($"Lock ID: {boundLock.Item("id")}")
                        lockInfo.AppendLine($"Title: {boundLock.Item("title")}")
                        lockInfo.AppendLine($"Description: {boundLock.Item("description")}")
                        lockInfo.AppendLine($"Lock model name: {DirectCast(boundLock.Item("lockType"), JObject)?.GetValue("modelName")}")
                        lockInfo.AppendLine($"Binding date: {boundLock.Item("bindDate")}")
                        lockInfo.AppendLine("-------------------------")
                        Console.WriteLine(lockInfo)
                    Next
                Else
                    Console.WriteLine($"No bound locks for owner account {ownerAccount.Item("name")} ({ownerAccount.Item("id")}).")
                End If
            Next
        Else
            Console.WriteLine("This user has no Owner Accounts.")
        End If

    End Function

End Module
