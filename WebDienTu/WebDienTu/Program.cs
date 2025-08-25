using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies; // 👈 Thêm cái này
using WebDienTu.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// 👉 Cấu hình DbContext
var connectionString = builder.Configuration.GetConnectionString("DienTuStoreConnection");
builder.Services.AddDbContext<DienTuStoreContext>(x => x.UseSqlServer(connectionString));

// 👉 Cấu hình Authentication với Cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";

        options.ExpireTimeSpan = TimeSpan.FromMinutes(1); // optional
        options.SlidingExpiration = true;

        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;

        options.Cookie.MaxAge = null; // 👈 đây: cookie sẽ là session cookie → tắt trình duyệt sẽ mất
    });

// 👉 Thêm Session nếu muốn dùng
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();           // 👈 Kích hoạt session
app.UseAuthentication();    // 👈 Kích hoạt Authentication trước Authorization
app.UseAuthorization();

// 👉 Cấu hình cho Area trước
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// 👉 Cấu hình mặc định
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
