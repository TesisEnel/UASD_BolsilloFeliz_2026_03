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


var apiUrl = builder.Configuration["ApiUrl"]
    ?? throw new InvalidOperationException(
        "No se encontró la configuración ApiUrl.");

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(apiUrl)
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
//app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorClient.Client._Imports).Assembly);




app.MapPost("/login", async (
    HttpContext ctx,
    HttpClient http,
    [Microsoft.AspNetCore.Mvc.FromForm] string email,
    [Microsoft.AspNetCore.Mvc.FromForm] string password) =>
{
    if (string.IsNullOrWhiteSpace(email) ||
        string.IsNullOrWhiteSpace(password))
    {
        return Results.Redirect("/pagina_autenticacion?error=credenciales");
    }

    try
    {
        var respuesta = await http.PostAsJsonAsync(
            "api/usuarios/login",
            new
            {
                Email = email.Trim(),
                Password = password
            });

        if (respuesta.StatusCode ==
            System.Net.HttpStatusCode.Unauthorized)
        {
            return Results.Redirect(
                "/pagina_autenticacion?error=credenciales");
        }

        if (!respuesta.IsSuccessStatusCode)
        {
            return Results.Redirect(
                "/pagina_autenticacion?error=api");
        }

        var usuario = await respuesta.Content
            .ReadFromJsonAsync<UsuarioDto>();

        if (usuario is null || usuario.IdUsuario <= 0)
        {
            return Results.Redirect(
                "/pagina_autenticacion?error=respuesta");
        }

        var claims = new List<Claim>
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                usuario.IdUsuario.ToString()),

            new Claim(
                ClaimTypes.Name,
                $"{usuario.Nombres} {usuario.Apellidos}"),

            new Claim(
                ClaimTypes.Email,
                usuario.Email),

            new Claim(
                ClaimTypes.Role,
                usuario.IdRol.ToString())
        };

        var identidad = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identidad);

        await ctx.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal);

        return Results.Redirect("/");
    }
    catch (HttpRequestException)
    {
        return Results.Redirect(
            "/pagina_autenticacion?error=api");
    }
    catch (System.Text.Json.JsonException)
    {
        return Results.Redirect(
            "/pagina_autenticacion?error=respuesta");
    }
    catch (TaskCanceledException)
    {
        return Results.Redirect(
            "/pagina_autenticacion?error=api");
    }
});

app.MapPost("/logout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync(
        CookieAuthenticationDefaults.AuthenticationScheme);

    return Results.Redirect("/pagina_autenticacion");
});


app.Run();
