using EcommerceWebApp.Handlers;

var builder = WebApplication.CreateBuilder(args);

// Add MVC Controllers and Views
builder.Services.AddControllersWithViews();

// Add DBHandler to Dependency Injection
builder.Services.AddSingleton<DBHandler>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
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

// Map static assets
app.MapStaticAssets();

// Define MVC routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
