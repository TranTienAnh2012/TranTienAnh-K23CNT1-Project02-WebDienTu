using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebDienTu.Models;

public class AccountController : Controller
{
    private readonly DienTuStoreContext _context;

    public AccountController(DienTuStoreContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(string hoTen, string email, string matKhau, string xacNhanMatKhau)
    {
        // Kiểm tra mật khẩu trùng khớp
        if (matKhau != xacNhanMatKhau)
        {
            ViewBag.Error = "Mật khẩu và xác nhận mật khẩu không khớp";
            return View();
        }

        // Kiểm tra email đã tồn tại chưa
        var existUser = _context.QuanTriViens.FirstOrDefault(u => u.Email == email);
        if (existUser != null)
        {
            ViewBag.Error = "Email đã được sử dụng";
            return View();
        }

        // Tạo user mới (default VaiTro = 0 => user thường)
        var newUser = new QuanTriVien
        {
            HoTen = hoTen,
            Email = email,
            MatKhau = matKhau,
            VaiTro = 0
        };

        _context.QuanTriViens.Add(newUser);
        await _context.SaveChangesAsync();

        // Tự động đăng nhập sau khi đăng ký
        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, newUser.HoTen),
        new Claim(ClaimTypes.Role, "User"),
        new Claim("UserId", newUser.MaNguoiDung.ToString())
    };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return RedirectToAction("Index", "Home");
    }


    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string email, string matKhau)
    {
        var user = _context.QuanTriViens
            .FirstOrDefault(u => u.Email == email && u.MatKhau == matKhau);

        if (user != null)
        {
            // Tạo claims
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.HoTen),
            new Claim(ClaimTypes.Role, user.VaiTro == 1 ? "Admin" : "User"), // giữ Role
            new Claim("UserId", user.MaNguoiDung.ToString())
        };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Tất cả user và admin đều vào trang user/Home
            return RedirectToAction("Index", "Home");
        }

        ViewBag.Error = "Sai email hoặc mật khẩu";
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
