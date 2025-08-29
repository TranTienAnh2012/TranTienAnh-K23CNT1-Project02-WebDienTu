using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using WebDienTu.Conventions;
using WebDienTu.Models;

var builder = WebApplication.CreateBuilder(args);

// ------------------- DỊCH VỤ -------------------

// Add MVC (Controllers + Views)
builder.Services.AddControllersWithViews(options =>
{
    // 👉 Gắn convention: tất cả controller trong Area "Admin" sẽ require [Authorize(Roles="Admin")]
    options.Conventions.Add(new AdminAreaAuthorization("Admin", "Admin"));
});

// 👉 Cấu hình DbContext
var connectionString = builder.Configuration.GetConnectionString("DienTuStoreConnection");
builder.Services.AddDbContext<DienTuStoreContext>(x => x.UseSqlServer(connectionString));

// 👉 Cấu hình Authentication với Cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Các đường dẫn login/logout/denied
        options.LoginPath = "/Account/Login";         // khi chưa login → redirect                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 
        options.LogoutPath = "/Account/Logout";       // khi logout → redirect
        options.AccessDeniedPath = "/Account/AccessDenied"; // khi sai quyền

        // Cookie settings
        options.Cookie.HttpOnly = true;   // bảo vệ cookie khỏi JS
        options.Cookie.IsEssential = true; // cookie bắt buộc
        options.Cookie.MaxAge = null;      // session cookie (mất khi tắt trình duyệt)

        // Expire settings
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // hết hạn sau 30 phút
        options.SlidingExpiration = true; // tự làm mới nếu còn hoạt động

        // Bảo mật
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // HTTPS nếu có
    });

// 👉 Session
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// ------------------- MIDDLEWARE -------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();           // 👉 Bật Session
app.UseAuthentication();    // 👉 Phải đặt trước Authorization
app.UseAuthorization();

// 👉 Route cho Area (ví dụ: /Admin/AdminHome/Index)
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// 👉 Route mặc định (user site)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


