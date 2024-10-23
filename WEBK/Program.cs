using Microsoft.Extensions.Configuration;
using WEBK.Controllers;
using WEBK.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var firebaseConfig = builder.Configuration.GetSection("Firebase");
builder.Services.AddSingleton(new FirebaseService(firebaseConfig["BasePath"], firebaseConfig["Secret"]));

builder.Services.AddControllersWithViews();
builder.Services.AddControllers();
builder.Services.AddHttpClient<FormController>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
