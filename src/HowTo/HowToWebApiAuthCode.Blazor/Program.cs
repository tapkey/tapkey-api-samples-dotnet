using HowToWebApiAuthCode.Blazor;
using HowToWebApiAuthCode.Blazor.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddMemoryCache();

var appConfig = new AppConfig()
{
    ClientId = builder.Configuration.GetValue<string>("ClientId"),
    ClientSecret = builder.Configuration.GetValue<string>("ClientSecret")
};

builder.Services.AddSingleton(appConfig);
builder.Services.AddHttpClient(AppConstants.TapkeyAuthorizationServerClient, client =>
{
    client.BaseAddress = new Uri(AppConstants.TapkeyAuthorizationServerUrl);
});

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<TapkeyApi>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
