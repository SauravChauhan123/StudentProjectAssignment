using Infrastructure.DependencyInjection;
using MediatR;
using Application.Mappings; 
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Register AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Register infrastructure dependencies using your custom DependencyInjection class
builder.Services.AddInfrastructure(builder.Configuration);

// Add controllers and configure JSON serialization settings (AddNewtonsoftJson)
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    });

// Configure Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline for development environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable HTTPS redirection
app.UseHttpsRedirection();

// Configure authorization (ensure you have any authentication middlewares if required)
app.UseAuthorization();

// Map the controllers
app.MapControllers();

app.Run();
