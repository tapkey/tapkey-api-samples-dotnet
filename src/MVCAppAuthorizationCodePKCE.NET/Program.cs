using Microsoft.Extensions.Options;
using MVCAppAuthorizationCodePKCE.NET;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddMvc();
builder.Services.AddControllersWithViews(options => options.EnableEndpointRouting = false)
    .AddRazorRuntimeCompilation();
builder.Services.AddMemoryCache();
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

// Register the settings for our application as a Singleton
builder.Services.Configure<AppConfiguration>(builder.Configuration.GetSection("AppConfiguration"));
builder.Services.AddSingleton(resolver =>
    resolver.GetRequiredService<IOptions<AppConfiguration>>().Value);

var appConfig = builder.Configuration.GetSection(nameof(AppConfiguration)).Get<AppConfiguration>();

// Add our typed Tapkey API HttpClient
builder.Services.AddHttpClient<ITapkeyApiClient, TapkeyApiClient>(client =>
{
    // Set the base address of the Tapkey API from the configuration
    client.BaseAddress = new Uri(appConfig.TapkeyApiBaseUrl);

    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("User-Agent", "MVCAppAuthorizationCodePKCE.NETCore");
});

// Add a HttpClient for the Tapkey Authorization Server
builder.Services.AddHttpClient(AppConstants.TapkeyAuthorizationServerClient, client =>
{
    client.BaseAddress = new Uri(appConfig.TapkeyAuthorizationServerUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("User-Agent", "MVCAppAuthorizationCodePKCE.NETCore");
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
} else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCookiePolicy();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
            
