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
                .Include(s => s.DanhGia) // thêm include đánh giá
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
            // Lấy sản phẩm kèm danh mục, khuyến mãi và đánh giá + user đánh giá
            var sp = await _context.SanPhams
                .Include(s => s.MaDanhMucNavigation)
                .Include(s => s.MaKhuyenMais)
                .Include(s => s.DanhGia)
                    .ThenInclude(d => d.MaNguoiDungNavigation)
                .FirstOrDefaultAsync(s => s.MaSanPham == id && s.TrangThai == true);

            if (sp == null) return NotFound();

            // Tính giá bán hiện tại dựa trên khuyến mãi
            var giamGiaHienHanh = sp.MaKhuyenMais
                .Where(g => g.TrangThai && g.NgayBatDau <= DateTime.Now && g.NgayKetThuc >= DateTime.Now)
                .OrderByDescending(g => g.GiaTri)
                .FirstOrDefault();

            sp.GiaBan = giamGiaHienHanh != null ? sp.Gia * (1 - giamGiaHienHanh.GiaTri / 100m) : sp.Gia;

            // Tính trung bình sao
            var danhGias = sp.DanhGia.Where(d => d.SoSao.HasValue).ToList();
            ViewBag.TrungBinhSao = danhGias.Any()
                ? Math.Round(danhGias.Average(d => d.SoSao.Value), 1)
                : 0;

            return View(sp);
        }

        public async Task<IActionResult> LocTheoLoai(string loai)
        {
            var sanPhams = await _context.SanPhams
                .Where(s => s.TrangThai == true)
                .Include(s => s.MaDanhMucNavigation)
                .Include(s => s.MaKhuyenMais)
                .Include(s => s.DanhGia)
                .ToListAsync();

            // Lọc theo loại
            if (!string.IsNullOrEmpty(loai))
            {
                sanPhams = sanPhams.Where(s => s.Loai != null && s.Loai.Equals(loai, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Tính giá bán hiện tại
            foreach (var sp in sanPhams)
            {
                var giamGiaHienHanh = sp.MaKhuyenMais
                    .Where(g => g.TrangThai && g.NgayBatDau <= DateTime.Now && g.NgayKetThuc >= DateTime.Now)
                    .OrderByDescending(g => g.GiaTri)
                    .FirstOrDefault();

                sp.GiaBan = giamGiaHienHanh != null ? sp.Gia * (1 - giamGiaHienHanh.GiaTri / 100m) : sp.Gia;
            }

            return PartialView("_SanPhamList", sanPhams);
        }

        public async Task<IActionResult> SearchAjax(string keyword)
        {
            var query = _context.SanPhams
                .Include(s => s.MaDanhMucNavigation)
                .Include(s => s.MaKhuyenMais)
                .Include(s => s.DanhGia)
                .Where(s => s.TrangThai == true);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(s =>
                    s.TenSanPham.Contains(keyword) ||
                    (s.MoTa != null && s.MoTa.Contains(keyword)) ||
                    (s.ThuongHieu != null && s.ThuongHieu.Contains(keyword)) ||
                    (s.XuatXu != null && s.XuatXu.Contains(keyword)) ||
                    (s.Loai != null && s.Loai.Contains(keyword))
                );
            }

            var sanPhams = await query.ToListAsync();

            // Tính giá bán
            foreach (var sp in sanPhams)
            {
                var giamGiaHienHanh = sp.MaKhuyenMais
                    .Where(g => g.TrangThai && g.NgayBatDau <= DateTime.Now && g.NgayKetThuc >= DateTime.Now)
                    .OrderByDescending(g => g.GiaTri)
                    .FirstOrDefault();

                sp.GiaBan = giamGiaHienHanh != null
                    ? sp.Gia * (1 - giamGiaHienHanh.GiaTri / 100m)
                    : sp.Gia;
            }

            return PartialView("_SanPhamList", sanPhams);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
