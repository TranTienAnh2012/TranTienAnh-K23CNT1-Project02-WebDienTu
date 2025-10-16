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

    // ========== ĐĂNG KÝ ==========
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(QuanTriVien model, string xacNhanMatKhau)
    {
        // ✅ Kiểm tra dữ liệu model theo DataAnnotation
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // ✅ Kiểm tra xác nhận mật khẩu
        if (model.MatKhau != xacNhanMatKhau)
        {
            ViewBag.Error = "Mật khẩu và xác nhận mật khẩu không khớp!";
            return View(model);
        }

        // ✅ Kiểm tra trùng email
        var existUser = _context.QuanTriViens.FirstOrDefault(u => u.Email == model.Email);
        if (existUser != null)
        {
            ViewBag.Error = "Email đã được sử dụng!";
            return View(model);
        }

        // ✅ Tạo tài khoản mới
        model.VaiTro = 0;
        model.NgayDangKy = DateTime.Now;

        _context.QuanTriViens.Add(model);
        await _context.SaveChangesAsync();

        // ✅ Tự động đăng nhập
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, model.HoTen),
            new Claim(ClaimTypes.Role, "User"),
            new Claim("UserId", model.MaNguoiDung.ToString())
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        TempData["SuccessMessage"] = $"Đăng ký thành công! Chào mừng {model.HoTen} 👋";
        return RedirectToAction("Index", "Home");
    }

    // ========== ĐĂNG NHẬP ==========
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string email, string matKhau)
    {
        // ✅ Kiểm tra nhập đủ
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(matKhau))
        {
            ViewBag.Error = "Vui lòng nhập đầy đủ Email và Mật khẩu!";
            return View();
        }

        var user = _context.QuanTriViens
            .FirstOrDefault(u => u.Email == email && u.MatKhau == matKhau);

        if (user == null)
        {
            ViewBag.Error = "Sai email hoặc mật khẩu!";
            return View();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.HoTen),
            new Claim(ClaimTypes.Role, user.VaiTro == 1 ? "Admin" : "User"),
            new Claim("UserId", user.MaNguoiDung.ToString())
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        TempData["SuccessMessage"] = $"Đăng nhập thành công! Chào mừng {user.HoTen} 👋";
        return RedirectToAction("Index", "Home");
    }

    // ========== ĐĂNG XUẤT ==========
    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        TempData["SuccessMessage"] = "Bạn đã đăng xuất thành công!";
        return RedirectToAction("Index", "Home");
    }

    // ========== THÔNG TIN CÁ NHÂN ==========
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

        return View(user);
    }

    [HttpGet]
    public IActionResult EditProfile()
    {
        int userId = int.Parse(User.Claims.First(c => c.Type == "UserId").Value);
        var user = _context.QuanTriViens.FirstOrDefault(u => u.MaNguoiDung == userId);
        if (user == null) return NotFound();

        var model = new EditProfileViewModel
        {
            HoTen = user.HoTen,
            Email = user.Email
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProfile(EditProfileViewModel model)
    {
        int userId = int.Parse(User.Claims.First(c => c.Type == "UserId").Value);
        var user = _context.QuanTriViens.FirstOrDefault(u => u.MaNguoiDung == userId);
        if (user == null) return NotFound();

        if (!ModelState.IsValid) return View(model);

        // Cập nhật thông tin cơ bản
        user.HoTen = model.HoTen;
        user.Email = model.Email;

        // Đổi mật khẩu nếu người dùng nhập
        if (!string.IsNullOrWhiteSpace(model.MatKhauMoi))
        {
            if (user.MatKhau != model.MatKhauHienTai)
            {
                ViewBag.Error = "Mật khẩu hiện tại không đúng!";
                return View(model);
            }
            user.MatKhau = model.MatKhauMoi;
        }

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
        return RedirectToAction("Profile");
    }


    // ========== TRANG CẤM TRUY CẬP ==========
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
