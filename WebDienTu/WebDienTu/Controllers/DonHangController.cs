using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebDienTu.Models;

namespace WebDienTu.Controllers
{
    public class DonHangController : Controller
    {
        private readonly DienTuStoreContext _context;

        public DonHangController(DienTuStoreContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return RedirectToAction("LichSu");
        }

        public async Task<IActionResult> LichSu()
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["Warning"] = "Vui lòng đăng nhập để xem lịch sử đơn hàng!";
                return RedirectToAction("Login", "Account");
            }

            // Lấy claim UserId an toàn
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                TempData["Warning"] = "Thông tin người dùng không hợp lệ. Vui lòng đăng nhập lại!";
                return RedirectToAction("Login", "Account");
            }

            var donHangs = await _context.DonHangs
                .Where(d => d.MaNguoiDung == userId)
                .Include(d => d.ChiTietDonHangs)
                .ThenInclude(ct => ct.MaSanPhamNavigation)
                .OrderByDescending(d => d.NgayDatHang)
                .ToListAsync();

            return View(donHangs);
        }

        [HttpPost]
        public async Task<IActionResult> MuaHang(int sanPhamId, int soLuong = 1)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["Warning"] = "Vui lòng đăng nhập hoặc đăng ký để mua hàng!";
                return RedirectToAction("Login", "Account");
            }

            int userId = int.Parse(User.Claims.First(c => c.Type == "UserId").Value);

            var sp = await _context.SanPhams.FindAsync(sanPhamId);
            if (sp == null || sp.TrangThai == false || sp.SoLuongTon < soLuong)
            {
                TempData["Error"] = "Sản phẩm không hợp lệ hoặc hết hàng.";
                return RedirectToAction("Index", "Home");
            }

            var donHang = new DonHang
            {
                MaNguoiDung = userId,
                NgayDatHang = DateTime.Now,
                TongTien = soLuong * (sp.GiaBan ?? sp.Gia),
                TrangThai = false // chờ admin xác nhận
            };
            _context.DonHangs.Add(donHang);
            await _context.SaveChangesAsync();

            var chiTiet = new ChiTietDonHang
            {
                MaDonHang = donHang.MaDonHang,
                MaSanPham = sp.MaSanPham,
                SoLuong = soLuong,
                DonGia = sp.GiaBan ?? sp.Gia
            };
            _context.ChiTietDonHangs.Add(chiTiet);

            sp.SoLuongTon -= soLuong; // trừ tồn kho

            await _context.SaveChangesAsync();

            TempData["Success"] = "Đơn hàng đã được tạo và đang chờ admin xác nhận!";
            return RedirectToAction("LichSu");
        }
    }
}
