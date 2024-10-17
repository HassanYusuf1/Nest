using Microsoft.EntityFrameworkCore;
using InstagramMVC.DAL;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
var connectionString = builder.Configuration.GetConnectionString("MediaDbContextConnection") ?? throw new 
    InvalidOperationException("Connection string 'MediaDbContextConnection' not found.");


builder.Services.AddDbContext<MediaDbContext>(options =>
{
    options.UseSqlite(
        builder.Configuration["ConnectionStrings:MediaDbContextConnection"]);
});




var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();

app.MapDefaultControllerRoute();

// app.MapControllerRoute(
//     name: "default",
//     pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();