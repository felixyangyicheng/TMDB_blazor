using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
using TMDB_blazor.Contracts;
using TMDB_blazor.Services;
using TMDbLib.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();
//On ajoute le client TMDb comme un service, il peut �tre r�cup�rer par injection.
builder.Services.AddTransient((sp) => new TMDbClient("53a27e817e504cd4cae995309e05aecc"));
builder.Services.AddScoped<IJsonFileRepository, JsonFileRepository>();
builder.Services.AddScoped<ITmdbExtension, TmdbExtension>();

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

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
