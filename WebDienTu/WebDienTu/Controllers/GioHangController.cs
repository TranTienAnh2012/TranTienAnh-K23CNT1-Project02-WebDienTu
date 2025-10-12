using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebDienTu.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

[Authorize] // Bắt buộc đăng nhập với tất cả action trong controller
public class GioHangController : Controller
{
    private readonly DienTuStoreContext _context;

    public GioHangController(DienTuStoreContext context)
    {
        _context = context;
    }

    // Xem giỏ hàng
    public async Task<IActionResult> Index()
    {
        var userIdString = User.FindFirstValue("UserId"); // dùng claim "UserId"
        if (!int.TryParse(userIdString, out int userId))
        {
            // Claim không hợp lệ → redirect login
            return RedirectToAction("Login", "Account");
        }

        var gioHang = await _context.GioHangTams
            .Include(g => g.MaSanPhamNavigation)
            .Where(g => g.MaNguoiDung == userId)
            .ToListAsync();
        // Thống kê
        ViewBag.TongSanPham = gioHang.Sum(g => g.SoLuong);
        ViewBag.ChuaThanhToan = gioHang.Where(g => g.TrangThai == "ChuaThanhToan").Sum(g => g.SoLuong);
        ViewBag.DaThanhToan = gioHang.Where(g => g.TrangThai == "DaThanhToan").Sum(g => g.SoLuong);
        return View(gioHang);
    }

    // Thêm sản phẩm vào giỏ hàng
    public async Task<IActionResult> AddToCart(int sanPhamId, int quantity = 1)
    {
        var userIdString = User.FindFirstValue("UserId");
        if (!int.TryParse(userIdString, out int userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var item = await _context.GioHangTams
            .FirstOrDefaultAsync(g => g.MaNguoiDung == userId && g.MaSanPham == sanPhamId);

        if (item != null)
        {
            item.SoLuong += quantity;
        }
        else
        {
            item = new GioHangTam
            {
                MaNguoiDung = userId,
                MaSanPham = sanPhamId,
                SoLuong = quantity
            };
            _context.GioHangTams.Add(item);
        }

        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    // theo dõi số lượng sản phẩm trong giỏ hàng
    [HttpPost]
    public async Task<IActionResult> UpdateQuantity(int sanPhamId, int change)
    {
        var userIdString = User.FindFirstValue("UserId");
        if (!int.TryParse(userIdString, out int userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var item = await _context.GioHangTams
            .FirstOrDefaultAsync(g => g.MaNguoiDung == userId && g.MaSanPham == sanPhamId);

        if (item != null)
        {
            item.SoLuong += change;
            if (item.SoLuong <= 0)
            {
                _context.GioHangTams.Remove(item); // Nếu số lượng <= 0 thì xóa luôn
            }
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> XacNhanThanhToan()
    {
        int userId = int.Parse(User.Claims.First(c => c.Type == "UserId").Value);

        var gioHang = await _context.GioHangTams
            .Include(g => g.MaSanPhamNavigation)
            .Where(g => g.MaNguoiDung == userId)
            .ToListAsync();

        if (!gioHang.Any())
        {
            TempData["Error"] = "Giỏ hàng trống!";
            return RedirectToAction("Index");
        }

        ViewBag.TongTien = gioHang.Sum(g => g.SoLuong * (g.MaSanPhamNavigation.GiaBan ?? g.MaSanPhamNavigation.Gia));
        return View(gioHang); // tìm view: Views/GioHang/XacNhanThanhToan.cshtml
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ThanhToanGioHang()
    {
        if (!User.Identity.IsAuthenticated)
        {
            TempData["Warning"] = "Vui lòng đăng nhập để mua hàng!";
            return RedirectToAction("Login", "Account");
        }

        int userId = int.Parse(User.Claims.First(c => c.Type == "UserId").Value);

        var gioHang = await _context.GioHangTams
            .Include(g => g.MaSanPhamNavigation)
            .Where(g => g.MaNguoiDung == userId)
            .ToListAsync();

        if (!gioHang.Any())
        {
            TempData["Error"] = "Giỏ hàng trống!";
            return RedirectToAction("Index", "GioHang");
        }

        // Tạo đơn hàng
        var donHang = new DonHang
        {
            MaNguoiDung = userId,
            NgayDatHang = DateTime.Now,
            TongTien = gioHang.Sum(g => g.SoLuong * (g.MaSanPhamNavigation.GiaBan ?? g.MaSanPhamNavigation.Gia)),
            TrangThai = false
        };
        _context.DonHangs.Add(donHang);
        await _context.SaveChangesAsync();

        // Thêm chi tiết đơn hàng
        foreach (var item in gioHang)
        {
            var chiTiet = new ChiTietDonHang
            {
                MaDonHang = donHang.MaDonHang,
                MaSanPham = item.MaSanPham,
                SoLuong = item.SoLuong,
                DonGia = item.MaSanPhamNavigation.GiaBan ?? item.MaSanPhamNavigation.Gia
            };
            _context.ChiTietDonHangs.Add(chiTiet);

            // Trừ tồn kho
            item.MaSanPhamNavigation.SoLuongTon -= item.SoLuong;
        }

        // Xóa giỏ hàng tạm
        _context.GioHangTams.RemoveRange(gioHang);

        await _context.SaveChangesAsync();

        TempData["Success"] = "Đặt hàng thành công! Đơn hàng đang chờ admin xác nhận.";
        return RedirectToAction("Index", "GioHang");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ThanhToanSanPham(int gioHangId)
    {
        if (!User.Identity.IsAuthenticated)
        {
            TempData["Warning"] = "Vui lòng đăng nhập để mua hàng!";
            return RedirectToAction("Login", "Account");
        }

        int userId = int.Parse(User.Claims.First(c => c.Type == "UserId").Value);

        // ✅ Lấy sản phẩm cụ thể trong giỏ
        var item = await _context.GioHangTams
            .Include(g => g.MaSanPhamNavigation)
            .FirstOrDefaultAsync(g => g.Id == gioHangId && g.MaNguoiDung == userId);

        if (item == null)
        {
            TempData["Error"] = "Không tìm thấy sản phẩm trong giỏ!";
            return RedirectToAction("Index");
        }

        var gia = item.MaSanPhamNavigation.GiaBan ?? item.MaSanPhamNavigation.Gia;

        // ✅ Tạo đơn hàng mới cho 1 sản phẩm
        var donHang = new DonHang
        {
            MaNguoiDung = userId,
            NgayDatHang = DateTime.Now,
            TongTien = gia * item.SoLuong,
            TrangThai = false
        };
        _context.DonHangs.Add(donHang);
        await _context.SaveChangesAsync();

        // ✅ Thêm chi tiết đơn hàng
        var chiTiet = new ChiTietDonHang
        {
            MaDonHang = donHang.MaDonHang,
            MaSanPham = item.MaSanPham,
            SoLuong = item.SoLuong,
            DonGia = gia
        };
        _context.ChiTietDonHangs.Add(chiTiet);

        // ✅ Trừ tồn kho
        item.MaSanPhamNavigation.SoLuongTon -= item.SoLuong;

        // ✅ Xóa sản phẩm khỏi giỏ
        _context.GioHangTams.Remove(item);

        await _context.SaveChangesAsync();

        TempData["Success"] = "Đặt hàng thành công cho sản phẩm này!";
        return RedirectToAction("Index");
    }


    // Xóa sản phẩm khỏi giỏ hàng
    public async Task<IActionResult> Remove(int id)
    {
        var item = await _context.GioHangTams.FindAsync(id);
        if (item != null)
        {
            _context.GioHangTams.Remove(item);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }
}
