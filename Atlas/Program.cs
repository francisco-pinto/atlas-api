using Atlas.Features.Shared;
using Atlas.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddEndpoints();

builder.Services.AddSingleton<IGeminiApi, GeminiApi>(); 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{ 
    //TODO: Uncomment this before production. FIx certificate issues 
    app.UseHttpsRedirection();
}

app.MapControllers();

app.Run();

