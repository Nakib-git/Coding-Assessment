using ErpUsers.Application.Interfaces;
using ErpUsers.Application.Services;
using ErpUsers.Infrastructure.Extensions;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Infrastructure (EF Core + ADO.NET + Repository)
builder.Services.AddInfrastructure(builder.Configuration);

// Application services
builder.Services.AddScoped<IUserService, UserService>();

// CORS restricts origins to the Angular dev server
builder.Services.AddCors(options =>
    options.AddPolicy("Angular", policy =>
        policy.WithOrigins(
                builder.Configuration.GetValue<string>("Cors:AllowedOrigin")
                    ?? "http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()));

// API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "ERP Users API", Version = "v1" });
});

var app = builder.Build();

// Global exception handler — returns RFC 7807 problem details 
app.UseExceptionHandler(errorApp =>
    errorApp.Run(async context =>
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode  = StatusCodes.Status500InternalServerError;

        var feature = context.Features.Get<IExceptionHandlerFeature>();
        var logger  = context.RequestServices
            .GetRequiredService<ILogger<Program>>();

        if (feature?.Error is not null)
            logger.LogError(feature.Error, "Unhandled exception");

        await context.Response.WriteAsJsonAsync(new
        {
            status = 500,
            title  = "An unexpected error occurred.",
        });
    }));

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("Angular");
app.UseAuthorization();
app.MapControllers();

app.Run();
