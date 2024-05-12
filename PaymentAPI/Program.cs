using PaymentAPI;
using WebAPI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.useApplicationServices();

var app = builder.Build();
app.UseExceptionHandler(_=>{});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI(c=>{
        c.SwaggerEndpoint("/swagger/v1/swagger.json","Savy Pay APIS");

    });
}

app.UseHttpsRedirection();
//app.UseHttpLogging();
app.usePaymentAPIS();


app.Run();


