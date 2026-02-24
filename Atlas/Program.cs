var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// This endpoint is for predefined filters
app.MapGet("/countries/{country-code}/filters", (string countryCode, string filters) =>
    {
    })
    .WithName("");

// This endpoint is for custom filters input by the user
app.MapGet("/countries/{country-code}/custom-filter", (string countryCode, string customFilters) =>
    {
    })
    .WithName("");

app.Run();