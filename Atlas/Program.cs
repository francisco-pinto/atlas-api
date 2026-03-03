using Atlas.Features.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddEndpoints();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{ 
    //TODO: Uncomment this before production. FIx certificate issues 
    app.UseHttpsRedirection();
}

app.MapControllers();

app.Run();

