using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using InstagramMVC.DAL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure the connection string and register MediaDbContext
var connectionString = builder.Configuration.GetConnectionString("MediaDbContextConnection")
    ?? throw new InvalidOperationException("Connection string 'MediaDbContextConnection' not found.");

builder.Services.AddDbContext<MediaDbContext>(options =>
{
    options.UseSqlite(connectionString);  // Use SQLite with the provided connection string
});

// Register IBildeRepository with its concrete implementation BildeRepository
builder.Services.AddScoped<IBildeRepository, BildeRepository>();
builder.Services.AddScoped<INotatRepository, NotatRepository>();
builder.Services.AddScoped<IKommentarRepository, KommentarRepository>();



var app = builder.Build();
// Autentisering i program cs 
/*builder.Services.AddDefaultIdentity<IdentityUser>(options => 
    options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<DbContext>();

app.UseAuthentication(); 
app.UseAuthorization(); */ 

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();

// Optional: Use a default controller route if needed
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Front}/{action=Index}/{id?}"); 

// Start the app
app.Run();
