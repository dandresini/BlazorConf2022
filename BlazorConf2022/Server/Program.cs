using Azure.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureB2C"));


builder.Services.Configure<JwtBearerOptions>(
JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters.NameClaimType = "name";
});


builder.Services.AddSingleton(implementationFactory =>
{
TokenCredentialOptions options = new TokenCredentialOptions
{
AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
};

var Configuration = builder.Configuration.GetSection("AzureB2C");

var clientSecretCredential = new ClientSecretCredential(
Configuration["TenantId"],
Configuration["ClientId"],
Configuration["ClientSecret"],
options);

return new Microsoft.Graph.GraphServiceClient(clientSecretCredential, new[] { "https://graph.microsoft.com/.default" });
});


builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
