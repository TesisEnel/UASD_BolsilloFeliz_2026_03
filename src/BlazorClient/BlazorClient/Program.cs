using BlazorClient.Client.Pages;
using BlazorClient.Components;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using BlazorClient.Client.BlazorDTOS;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/pagina_autenticacion";
        o.ExpireTimeSpan = TimeSpan.FromHours(8);
    });
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization();   // para que las páginas WASM conozcan al usuario



builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.Configuration["ApiUrl"] ?? "https://finanzaspersonales-gffmfchmgvdhewe7.chilecentral-01.azurewebsites.net")
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorClient.Client._Imports).Assembly);


app.MapPost("/login", async (HttpContext ctx, HttpClient http,
    [Microsoft.AspNetCore.Mvc.FromForm] string email,
    [Microsoft.AspNetCore.Mvc.FromForm] string password) =>
{
    var r = await http.PostAsJsonAsync("api/usuarios/login", new { Email = email, Password = password });
    if (!r.IsSuccessStatusCode)
        return Results.Redirect("/pagina_autenticacion?error=1");

    var u = await r.Content.ReadFromJsonAsync<UsuarioDto>();
    var identity = new ClaimsIdentity(new[]
    {
        new Claim(ClaimTypes.NameIdentifier, u!.IdUsuario.ToString()),
        new Claim(ClaimTypes.Name, $"{u.Nombres} {u.Apellidos}")
    }, CookieAuthenticationDefaults.AuthenticationScheme);

    await ctx.SignInAsync(new ClaimsPrincipal(identity));
    return Results.Redirect("/");
});

app.MapPost("/logout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync();
    return Results.Redirect("/pagina_autenticacion");
});

app.Run();
