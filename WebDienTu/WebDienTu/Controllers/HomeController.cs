using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using WebDienTu.Models;

namespace WebDienTu.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DienTuStoreContext _context;

        public HomeController(ILogger<HomeController> logger, DienTuStoreContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var sanPhams = await _context.SanPhams
                .Where(s => s.TrangThai == true)
                .Include(s => s.MaDanhMucNavigation)
                .Include(s => s.MaKhuyenMais)
                .ToListAsync();

            foreach (var sp in sanPhams)
            {
                var giamGiaHienHanh = sp.MaKhuyenMais
                    .Where(g => g.TrangThai && g.NgayBatDau <= DateTime.Now && g.NgayKetThuc >= DateTime.Now)
                    .OrderByDescending(g => g.GiaTri)
                    .FirstOrDefault();

                sp.GiaBan = giamGiaHienHanh != null ? sp.Gia * (1 - giamGiaHienHanh.GiaTri / 100m) : sp.Gia;
            }

            return View(sanPhams);
        }

        public async Task<IActionResult> Details(int id)
        {
            var sp = await _context.SanPhams
                .Include(s => s.MaDanhMucNavigation)
                .Include(s => s.MaKhuyenMais)
                .FirstOrDefaultAsync(s => s.MaSanPham == id && s.TrangThai == true);

            if (sp == null) return NotFound();

            var giamGiaHienHanh = sp.MaKhuyenMais
                .Where(g => g.TrangThai && g.NgayBatDau <= DateTime.Now && g.NgayKetThuc >= DateTime.Now)
                .OrderByDescending(g => g.GiaTri)
                .FirstOrDefault();

            sp.GiaBan = giamGiaHienHanh != null ? sp.Gia * (1 - giamGiaHienHanh.GiaTri / 100m) : sp.Gia;

            return View(sp);
        }

        [HttpGet]
        public async Task<IActionResult> XacNhanMua(int sanPhamId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["Warning"] = "Vui lòng đăng nhập hoặc đăng ký để mua hàng!";
                return RedirectToAction("Login", "Account");
            }

            var sp = await _context.SanPhams.FindAsync(sanPhamId);
            if (sp == null || sp.TrangThai == false)
                return RedirectToAction("Index", "Home");

            return View(sp);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
