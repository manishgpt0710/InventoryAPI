using InventoryWebApi.Infrastructure;
using InventoryWebApi.Application.Services;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

// Add controllers instead of minimal APIs.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Prevent "A possible object cycle was detected" when returning EF entities
        // with bidirectional navigation properties (e.g., LookupGroup <-> LookupItem).
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        // Keep default behavior for now; can be customized later.
    });

// Register Infrastructure with SQL Server / EF Core.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                      ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddInfrastructure(connectionString);
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddEndpointsApiExplorer();

// Add Swagger/OpenAPI services
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Inventory Web API",
        Description = "A RESTful API for managing inventory, products, warehouses, and related entities.",
        Contact = new OpenApiContact
        {
            Name = "API Support",
            Email = "support@inventoryapi.com"
        }
    });

    // Include XML comments if available
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    
    // Enable Swagger UI
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Inventory Web API v1");
        options.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
        options.DisplayRequestDuration();
        options.EnableTryItOutByDefault();
    });
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
