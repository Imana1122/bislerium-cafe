using Infrastructure.DependencyInjection;
using Application.DependencyInjection;
using WebUI.Components;
using Microsoft.AspNetCore.Components.Authorization;
using WebUI.Components.Layout.Identity;
using WebUI.Hubs;
using WebUI.States;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddInfrastructureService(builder.Configuration);
builder.Services.AddApplicationService();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthStateProvider>();
builder.Services.AddScoped<ICustomAuthorizationService, CustomAuthorizationService>();
builder.Services.AddScoped<NetcodeHubConnectionService>();
builder.Services.AddScoped<ChangePasswordState>();


// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.MapSignOutEndpoint();
app.Run();
