using EcommerceWebApp.Handlers;

var builder = WebApplication.CreateBuilder(args);

// Add DBHandler to Dependency Injection
builder.Services.AddSingleton<DBHandler>();

var app = builder.Build();

// Test database connection
var dbHandler = app.Services.GetRequiredService<DBHandler>();
if (dbHandler.TestConnection())
{
    Console.WriteLine("Database connection successful!");
}
else
{
    Console.WriteLine("Failed to connect to the database.");
}

// Define API routes
app.MapGet("/", () => "Hello World!");

app.Run();
