using Domain.Request;
using Microsoft.OpenApi.Models;
using PaymentAPI;
using WebAPI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.Antiforgery;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration, "AzureAd");
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
   .RequireAuthenticatedUser()
   .Build();
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = "MyApp.Antiforgery";
    options.HeaderName = "X-CSRF-TOKEN";
});
builder.useApplicationServices();

// Configure the app

var app = builder.Build();
app.UseExceptionHandler(_ => { });

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Savy Pay APIS");

    });
}

app.UseHttpsRedirection();
//app.UseHttpLogging();
app.UseAuthentication();
app.UseAuthorization();

app.Use((context, next) =>
{
    var antiforgery = context.RequestServices.GetRequiredService<IAntiforgery>();
    var tokens = antiforgery.GetAndStoreTokens(context);
    context.Response.Cookies.Append("MyApp.Antiforgery", tokens.RequestToken!, new CookieOptions { HttpOnly = true });
    return next(context);
});
app.usePaymentAPIS();


app.Run();


