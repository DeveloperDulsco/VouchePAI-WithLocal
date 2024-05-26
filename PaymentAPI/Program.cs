
using PaymentAPI;
using WebAPI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.HttpLogging;
using Middlewares.Exceptions;



var builder = WebApplication.CreateBuilder(args);
builder.Services.AddWindowsService();
builder.Services.AddExceptionHandler<GlobalExceptions>();
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration, "AzureAd");
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
   .RequireAuthenticatedUser()
   .Build();
});

builder.Services.AddAntiforgery();

builder.Services.AddHttpLogging(logging =>
{
     logging.LoggingFields = HttpLoggingFields.All;
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
    
   


});
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.useApplicationServices();



// Configure the app

var app = builder.Build();

app.UseHttpsRedirection();
app.UseExceptionHandler(_ => { });
app.UseHttpLogging();
app.UseAntiforgery();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Savvy Pay APIS");

    });
}



app.UseAuthentication();
app.UseAuthorization();
//app.LogRequestResponse();
app.usePaymentAPIS();
app.Logger.LogInformation("*******************Starting the app*************************");

app.Run();


