using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Microsoft.Extensions.Logging;
using Serilog;
using ClinicAPI;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) => {
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .WriteTo.Console()
        .WriteTo.File(context.Configuration["Logging:File:Path"]);
});

// Add services to the container.
builder.Services.AddControllers();

// Retrieve the Application Name from appsettings.json depending on the environment
var appName = builder.Configuration["ApplicationName"] ?? "Default API Name";
Console.WriteLine($"Application Name: {builder.Configuration["ApplicationName"]}");

// Configuring Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = appName,  // Use the dynamic application name
        Version = "v1",
        Description = "An API for managing patients", // Optional
        TermsOfService = new Uri("https://example.com/terms"), // Optional
        Contact = new OpenApiContact
        {
            Name = "Support",
            Email = "support@example.com",
            Url = new Uri("https://support.example.com")
        }, // Optional
        License = new OpenApiLicense
        {
            Name = "Use under LICX",
            Url = new Uri("https://example.com/license")
        } // Optional
    });
});

var app = builder.Build();

// Middleware for Error Handling
app.UseMiddleware<ErrorHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("QA") || app.Environment.IsEnvironment("UAT"))
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", appName + " V1");
        c.RoutePrefix = string.Empty; // To serve Swagger UI at application's root
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
