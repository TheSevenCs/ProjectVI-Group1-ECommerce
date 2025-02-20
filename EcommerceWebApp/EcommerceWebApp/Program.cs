using EcommerceWebApp.Controllers;
using EcommerceWebApp.Handlers;

var builder = WebApplication.CreateBuilder(args);

// Add MVC Controllers and Views
builder.Services.AddControllersWithViews();

// Add DBHandler to Dependency Injection
builder.Services.AddSingleton<DBHandler>();

builder.Services.AddScoped<ShoppingCartController>();
builder.Services.AddScoped<ItemController>();

// Explicitly configure Kestrel to listen on 0.0.0.0:8080
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080); // Ensures it works inside Docker
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Correct method to serve static assets
app.UseRouting();
app.UseAuthorization();

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

// Define MVC routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
