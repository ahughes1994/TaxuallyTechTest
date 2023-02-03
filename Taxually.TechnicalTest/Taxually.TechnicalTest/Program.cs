using Taxually.TechnicalTest.Clients;
using Taxually.TechnicalTest.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<ITaxuallyHttpClient, TaxuallyHttpClient>();
builder.Services.AddTransient<ITaxuallyQueueClient, TaxuallyQueueClient>();

builder.Services.AddTransient<IVatRequestProcessingService, VatRequestProcessingService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
