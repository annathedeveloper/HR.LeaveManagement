using HR.LeaveManagement.MVC.Contracts;
using HR.LeaveManagement.MVC.Services;
using HR.LeaveManagement.MVC.Services.Base;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;
services.AddHttpContextAccessor();
services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.None;
});
services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();

services.AddTransient<IAuthenticationService, AuthenticationService>();
services.AddScoped<JwtSecurityTokenHandler>();//Added to fix error
services.AddHttpClient<IClient, Client>(cl => cl.BaseAddress = new Uri("https://localhost:7241"));
services.AddAutoMapper(Assembly.GetExecutingAssembly());

services.AddScoped<ILeaveTypeService, LeaveTypeService>();
services.AddScoped<ILeaveAllocationService, LeaveAllocationService>();
services.AddScoped<ILeaveRequestService, LeaveRequestService>();

services.AddSingleton<ILocalStorageService, LocalStorageService>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseCookiePolicy();
app.UseAuthentication();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
