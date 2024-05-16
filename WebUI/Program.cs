using Infrastructure.DependencyInjection;
using Application.DependencyInjection;
using WebUI.Components;
using Microsoft.AspNetCore.Components.Authorization;
using WebUI.Components.Layout.Identity;
using WebUI.Hubs;
using WebUI.States;
using Syncfusion.Blazor;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Infrastructure.Repository.Products.Handlers.Blogs;
using Infrastructure.Repository.Blogs.Handlers;
using WebUI.Components.Pages;
using ServiceStack.Text;
using Application.Extensions.Email;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddInfrastructureService(builder.Configuration);
builder.Services.AddApplicationService();

builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthStateProvider>();
builder.Services.AddScoped<ICustomAuthorizationService, CustomAuthorizationService>();
builder.Services.AddScoped<NetcodeHubConnectionService>();
builder.Services.AddScoped<NotificationCountState>();
var configuration = builder.Configuration;



builder.Services.AddSyncfusionBlazor();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateBlogHandler).Assembly));
builder.Services.AddSignalR();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddBlazorBootstrap();
builder.Services.AddScoped<IToastService, ToastService>();
builder.Services.AddServerSideBlazor().AddCircuitOptions(o =>
{
    
        o.DetailedErrors = true;
    
});


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
app.MapHub<CommunicationHub>("/communicationhub");

app.MapSignOutEndpoint();
app.Run();
