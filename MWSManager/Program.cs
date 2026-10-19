
using Application;
using BlazorToastify;
using Infrastracture;
using Infrastructure.RabbitSenderRepository;
using Microsoft.AspNetCore.Authentication.Cookies;
using MWSMan;
using MWSManager.Components;
using Persistence;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Options;
using MWSManager.Services;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddServerSideBlazor(options =>
{
    options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromSeconds(30); // کاهش به 30 ثانیه
    options.MaxBufferedUnacknowledgedRenderBatches = 10; // کاهش بافر پیام‌ها
});


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
    });

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddHttpContextAccessor();



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(p =>
{
    p.AddPolicy("AllowBlazore", policy =>
    {
        //policy.AllowAnyOrigin();
        policy.WithOrigins("http://localhost:5129");
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
        policy.AllowCredentials();

    });
});
builder.Services.AddAntiforgery();



// Extenssions For Projects
builder.Services.AddScoped<IRabbitSenderRepository, RabbitSenderRepository>();
builder.Services.Infrastructure_DIRegister(builder.Configuration);
builder.Services.Persistence_DIRegister(builder.Configuration);
builder.Services.MWSMan_DIRegister();
builder.Services.Application_DIRegister();
builder.Services.AddToasts();

// RabbitMQ Management HTTP API. Supply credentials through environment variables or user-secrets,
// never commit them to appsettings.json.
// Example environment variables: RabbitMqMonitoring__UserName and RabbitMqMonitoring__Password.
builder.Services.Configure<RabbitMqMonitoringOptions>(
    builder.Configuration.GetSection("RabbitMqMonitoring"));
builder.Services.AddHttpClient<IRabbitMqMonitoringService, RabbitMqMonitoringService>((services, client) =>
{
    var options = services.GetRequiredService<IOptions<RabbitMqMonitoringOptions>>().Value;
    var managementUrl = builder.Configuration["RabbitMqMonitoring:ManagementUrl"];
    client.BaseAddress = new Uri(string.IsNullOrWhiteSpace(managementUrl)
        ? "http://Rabbit-srv:15672/"
        : managementUrl);
    client.Timeout = TimeSpan.FromSeconds(8);

    if (!string.IsNullOrWhiteSpace(options.UserName))
    {
        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{options.UserName}:{options.Password}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
    }
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
//Custom Middlware Performance

//app.Use(async (context, next) =>
//{
//    Stopwatch st = new Stopwatch();
//    st.Start();
//    await next.Invoke();
//    st.Stop();
//    if (st.ElapsedMilliseconds > 10000)
//    {
        
//    }
//});
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowBlazore");
app.UseAntiforgery();



app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
//app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


app.Run();
