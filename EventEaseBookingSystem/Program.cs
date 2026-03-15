using Microsoft.EntityFrameworkCore;
using EventEaseBookingSystem.Data;

var builder = WebApplication.CreateBuilder(args);

// ADD THIS LINE
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<EventEaseContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("EventEaseConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()
    ));
   

builder.Services.AddAuthorization();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();