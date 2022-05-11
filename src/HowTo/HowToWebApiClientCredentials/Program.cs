using CommandLine;
using HowToWebApiClientCredentials;
using HowToWebApiClientCredentials.Models;

var options = (Parser.Default.ParseArguments<Options>(Environment.GetCommandLineArgs()) as Parsed<Options>)?.Value;

if (options == null)
    throw new ArgumentException("Unexpected input parameters");

var tokenResponse = await Authentication.RetrieveAccessToken(options.ClientId, options.ClientSecret);

var tapkeyApiClient = new TapkeyApi(tokenResponse.access_token);
var ownerAccounts = await tapkeyApiClient.GetAllOwnerAccounts();

Console.WriteLine("Please select an owner from the list below by typing its number:");
for (int i = 0; i < ownerAccounts.Length; i++)
{
    Console.WriteLine($"{i+1}: {ownerAccounts[i].name}");
}

var ownerAccountIndexStr = Console.ReadLine();
var ownerAccountIndex = int.Parse(ownerAccountIndexStr);
var ownerAccountId = ownerAccounts[ownerAccountIndex-1].id;

var boundLocks = await tapkeyApiClient.GetAllBoundLocks(ownerAccountId);

Console.WriteLine("Please select a bound lock from the list below by typing its number:");
for (int i = 0; i < boundLocks.Length; i++)
{
    Console.WriteLine($"{i + 1}: {boundLocks[i].title}");
}

var boundLockIndexStr = Console.ReadLine();
var boundLockIndex = int.Parse(boundLockIndexStr);
var boundLockId = boundLocks[boundLockIndex-1].id;

Console.WriteLine();
Console.WriteLine("Define an email of the contact to assign grant to: ");
var contactEmail = Console.ReadLine();

var grant = new Grant
{
    boundLockId = boundLockId,
    contact = new Contact() { identifier = contactEmail },
    validFrom = DateTime.Now,
    validBefore = DateTime.Now.AddDays(1)
};

var result = await tapkeyApiClient.CreateGrant(ownerAccountId, grant);

Console.WriteLine(result ? $"Grant for bound lock {boundLockId} and contact {contactEmail} has been created!" : "There was an error creating the grant");

