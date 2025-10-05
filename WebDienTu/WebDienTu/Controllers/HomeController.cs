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
        //trang chủ
        public async Task<IActionResult> Index()
        {
            // 🔑 Lấy hot keywords
            var hotKeywords = (await _context.SanPhams
                    .Where(sp => sp.TrangThai == true)
                    .Select(sp => new { sp.TenSanPham, sp.ThuongHieu, sp.Loai })
                    .ToListAsync())
                    .SelectMany(sp => new[] { sp.TenSanPham, sp.ThuongHieu, sp.Loai })
                    .Where(k => !string.IsNullOrEmpty(k))
                    .Distinct()
                    .Take(20 )
                    .ToList();
            ViewBag.HotKeywords = hotKeywords;

            // Lấy danh mục (chỉ những cái có sản phẩm hoạt động)
            var danhMucs = await _context.DanhMucs
                .Where(dm => dm.SanPhams.Any(sp => sp.TrangThai == true))
                .ToListAsync();

            ViewBag.DanhMucs = danhMucs;

            // 🔑 Lấy tất cả sản phẩm
            var sanPhams = await _context.SanPhams
                .Where(s => s.TrangThai == true)
                .Include(s => s.MaDanhMucNavigation)
                .Include(s => s.MaKhuyenMais)
                .Include(s => s.DanhGia)
                .ToListAsync();

            foreach (var sp in sanPhams)
            {
                var giamGiaHienHanh = sp.MaKhuyenMais
                    .Where(g => g.TrangThai && g.NgayBatDau <= DateTime.Now && g.NgayKetThuc >= DateTime.Now)
                    .OrderByDescending(g => g.GiaTri)
                    .FirstOrDefault();

                sp.GiaBan = giamGiaHienHanh != null ? sp.Gia * (1 - giamGiaHienHanh.GiaTri / 100m) : sp.Gia;
            }

            // 👉 Lấy userId từ Claims
            int? currentUserId = null;
            if (User.Identity.IsAuthenticated)
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(userIdStr) && int.TryParse(userIdStr, out int userId))
                {
                    currentUserId = userId;
                }
            }

            // 👉 Lấy sản phẩm đã xem của user hiện tại (phần này đang lỗi)
            List<SanPham> daXem = new();
            if (currentUserId.HasValue)
            {
                daXem = await _context.SanPhamDaXems
                    .Where(x => x.MaNguoiDung == currentUserId.Value)
                    .OrderByDescending(x => x.ThoiGianXem)
                    .Include(x => x.MaSanPhamNavigation)
                    .Select(x => x.MaSanPhamNavigation)
                    .Take(5)
                    .ToListAsync();
            }

            ViewBag.DaXem = daXem;

            return View(sanPhams);
        }


        //Chi tiết sản phẩm
        public async Task<IActionResult> Details(int id)
        {
            int? currentUserId = null;

            // 👉 Lấy user id từ Claim
            if (User.Identity.IsAuthenticated)
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(userIdStr) && int.TryParse(userIdStr, out int userId))
                {
                    currentUserId = userId;
                }
            }

            // 👉 Lấy sản phẩm theo id và include luôn các bảng phụ
            var sp = await _context.SanPhams
                .Include(s => s.MaDanhMucNavigation)
                .Include(s => s.MaKhuyenMais)
                .Include(s => s.DanhGia)
                    .ThenInclude(d => d.MaNguoiDungNavigation)
                .Include(s => s.GiaTriThuocTinhs)         // 🔹 Include thông số sản phẩm
                    .ThenInclude(gt => gt.ThuocTinh)      // 🔹 Include tên thuộc tính
                .AsSplitQuery()
                .FirstOrDefaultAsync(s => s.MaSanPham == id && s.TrangThai == true);

            if (sp == null) return NotFound();

            // ✅ Tính giá bán hiện tại
            var giamGiaHienHanh = sp.MaKhuyenMais?
                .Where(g => g.TrangThai && g.NgayBatDau <= DateTime.UtcNow && g.NgayKetThuc >= DateTime.UtcNow)
                .OrderByDescending(g => g.GiaTri)
                .FirstOrDefault();

            sp.GiaBan = sp.Gia * (1 - (giamGiaHienHanh?.GiaTri ?? 0) / 100m);

            // ✅ Tính trung bình sao
            var danhGias = sp.DanhGia?.Where(d => d.SoSao.HasValue).ToList() ?? new List<DanhGia>();
            ViewBag.TrungBinhSao = danhGias.Any()
                ? Math.Round(danhGias.Average(d => d.SoSao.Value), 1)
                : 0;

            // 👉 Lưu vào bảng SanPhamDaXem nếu có user id hợp lệ (nếu muốn)
            if (currentUserId.HasValue)
            {
                var daXem = await _context.SanPhamDaXems
                    .FirstOrDefaultAsync(x => x.MaNguoiDung == currentUserId.Value && x.MaSanPham == id);

                if (daXem == null)
                {
                    _context.SanPhamDaXems.Add(new SanPhamDaXem
                    {
                        MaNguoiDung = currentUserId.Value,
                        MaSanPham = id,
                        ThoiGianXem = DateTime.UtcNow
                    });
                }
                else
                {
                    daXem.ThoiGianXem = DateTime.UtcNow;
                }

                // ✅ Giới hạn 20 sản phẩm gần nhất
                var toDelete = await _context.SanPhamDaXems
                    .Where(x => x.MaNguoiDung == currentUserId.Value)
                    .OrderByDescending(x => x.ThoiGianXem)
                    .Skip(20)
                    .ToListAsync();

                if (toDelete.Any())
                    _context.SanPhamDaXems.RemoveRange(toDelete);

                await _context.SaveChangesAsync();
            }

            // 🔹 Note: Khi view Details, bạn có thể dùng sp.GiaTriThuocTinhs để hiển thị
            // Ví dụ trong view:
            // @foreach(var gt in Model.GiaTriThuocTinhs) {
            //     <tr><td>@gt.ThuocTinh?.TenThuocTinh</td><td>@gt.GiaTri</td></tr>
            // }

            return View(sp);
        }

        // 🔄 Lấy sản phẩm liên quan (theo danh mục)
        // 🔄 Lấy sản phẩm liên quan (theo danh mục)
        public IActionResult SanPhamLienQuan(int maSanPham, int maDanhMuc)
        {
            var spLienQuan = _context.SanPhams
              .Where(s => s.MaDanhMuc == maDanhMuc
                       && s.MaSanPham != maSanPham
                       && s.TrangThai == true)
              .OrderByDescending(s => s.NgayThem)
              .ToList();


            return PartialView("_SanPhamLienQuan", spLienQuan);
        }




        public async Task<IActionResult> LocTheoLoai(int? madm)
        {
            var sanPhams = _context.SanPhams
                .Where(s => s.TrangThai == true)
                .Include(s => s.MaDanhMucNavigation)
                .Include(s => s.MaKhuyenMais) // Collection
                .Include(s => s.DanhGia)      // Collection
                .AsQueryable();               // 👈 ép về IQueryable

            if (madm.HasValue && madm.Value > 0)
            {
                sanPhams = sanPhams.Where(s => s.MaDanhMuc == madm.Value);
            }

            var list = await sanPhams.ToListAsync();

            foreach (var sp in list)
            {
                var giamGiaHienHanh = sp.MaKhuyenMais
                    .Where(g => g.TrangThai && g.NgayBatDau <= DateTime.Now && g.NgayKetThuc >= DateTime.Now)
                    .OrderByDescending(g => g.GiaTri)
                    .FirstOrDefault();

                sp.GiaBan = giamGiaHienHanh != null ? sp.Gia * (1 - giamGiaHienHanh.GiaTri / 100m) : sp.Gia;
            }

            return PartialView("_SanPhamList", list);
        }



        //🔍 Tìm kiếm sản phẩm (AJAX)
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
        // 🔑 Lấy danh sách keyword gợi ý
        [HttpGet]
        public async Task<IActionResult> GetKeywords(string keyword)
        {
            var keywords = await _context.SanPhams
                .Where(s => s.TrangThai == true &&
                            (string.IsNullOrEmpty(keyword) || s.TenSanPham.Contains(keyword)))
                .SelectMany(s => new[] { s.TenSanPham, s.ThuongHieu, s.Loai })
                .Where(k => !string.IsNullOrEmpty(k))
                .Distinct()
                .Take(10)
                .ToListAsync();

            return Json(keywords); // Trả về JSON để JS hiển thị gợi ý
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
       
        // Trang giới thiệu
        public IActionResult GioiThieu()
        {
            return View();
        }
    }
}
