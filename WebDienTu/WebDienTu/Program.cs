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
        // Các đường dẫn login/logout/denied
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";

        // Session cookie: sẽ mất khi tắt trình duyệt
        options.Cookie.HttpOnly = true;   // bảo vệ cookie khỏi JS
        options.Cookie.IsEssential = true; // bắt buộc cho GDPR/consent
        options.Cookie.MaxAge = null;      // session cookie

        // Thời gian tồn tại cookie nếu trình duyệt mở lâu
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // optional
        options.SlidingExpiration = true; // tự làm mới khi user thao tác

        // Bảo mật
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // HTTPS nếu có
    });

// Kích hoạt session nếu dùng
builder.Services.AddSession();

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
