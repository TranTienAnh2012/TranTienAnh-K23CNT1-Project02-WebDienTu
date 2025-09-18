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
        if (matKhau != xacNhanMatKhau)
        {
            ViewBag.Error = "Mật khẩu và xác nhận mật khẩu không khớp";
            return View();
        }

        var existUser = _context.QuanTriViens.FirstOrDefault(u => u.Email == email);
        if (existUser != null)
        {
            ViewBag.Error = "Email đã được sử dụng";
            return View();
        }

        var newUser = new QuanTriVien
        {
            HoTen = hoTen,
            Email = email,
            MatKhau = matKhau,
            VaiTro = 0
        };

        _context.QuanTriViens.Add(newUser);
        await _context.SaveChangesAsync();

        // Tự động đăng nhập
        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, newUser.HoTen),
        new Claim(ClaimTypes.Role, "User"),
        new Claim("UserId", newUser.MaNguoiDung.ToString())
    };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        // Thêm thông báo thành công
        TempData["SuccessMessage"] = "Đăng ký thành công! Chào mừng " + newUser.HoTen;

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
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.HoTen),
            new Claim(ClaimTypes.Role, user.VaiTro == 1 ? "Admin" : "User"),
            new Claim("UserId", user.MaNguoiDung.ToString())
        };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            TempData["SuccessMessage"] = "Đăng nhập thành công! Chào mừng " + user.HoTen;
            return RedirectToAction("Index", "Home");
        }

        ViewBag.Error = "Sai email hoặc mật khẩu";
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        TempData["SuccessMessage"] = "Bạn đã đăng xuất thành công!";
        return RedirectToAction("Index", "Home");
    }
    [HttpGet]
    public IActionResult Profile()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Account");
        }

        int userId = int.Parse(User.Claims.First(c => c.Type == "UserId").Value);

        var user = _context.QuanTriViens.FirstOrDefault(u => u.MaNguoiDung == userId);
        if (user == null)
        {
            TempData["Error"] = "Không tìm thấy thông tin tài khoản!";
            return RedirectToAction("Index", "Home");
        }

        return View(user); // Views/Account/Profile.cshtml
    }

    [HttpGet]
    public IActionResult EditProfile()
    {
        int userId = int.Parse(User.Claims.First(c => c.Type == "UserId").Value);
        var user = _context.QuanTriViens.FirstOrDefault(u => u.MaNguoiDung == userId);
        if (user == null) return NotFound();

        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> EditProfile(QuanTriVien model)
    {
        int userId = int.Parse(User.Claims.First(c => c.Type == "UserId").Value);
        var user = _context.QuanTriViens.FirstOrDefault(u => u.MaNguoiDung == userId);

        if (user == null) return NotFound();

        // Cập nhật thông tin
        user.HoTen = model.HoTen;
        user.Email = model.Email;
        // Nếu muốn cho phép đổi mật khẩu thì thêm check riêng

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
        return RedirectToAction("Profile");
    }


    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
