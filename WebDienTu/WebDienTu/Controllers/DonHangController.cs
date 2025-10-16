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

            // Tạo đơn hàng mới
            var donHang = new DonHang
            {
                MaNguoiDung = userId,
                NgayDatHang = DateTime.Now,
                TongTien = soLuong * (sp.GiaBan ?? sp.Gia),
                TrangThai = false // chờ admin xác nhận
            };
            _context.DonHangs.Add(donHang);
            await _context.SaveChangesAsync();

            // Thêm chi tiết đơn hàng
            var chiTiet = new ChiTietDonHang
            {
                MaDonHang = donHang.MaDonHang,
                MaSanPham = sp.MaSanPham,
                SoLuong = soLuong,
                DonGia = sp.GiaBan ?? sp.Gia
            };
            _context.ChiTietDonHangs.Add(chiTiet);

            // Trừ tồn kho
            sp.SoLuongTon -= soLuong;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đơn hàng đã được tạo và đang chờ admin xác nhận!";
            return RedirectToAction("LichSu");
        }
        [HttpPost]
        public async Task<IActionResult> HuyDon(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["Warning"] = "Vui lòng đăng nhập!";
                return RedirectToAction("Login", "Account");
            }

            int userId = int.Parse(User.Claims.First(c => c.Type == "UserId").Value);

            var donHang = await _context.DonHangs
                .Include(d => d.ChiTietDonHangs)
                .FirstOrDefaultAsync(d => d.MaDonHang == id && d.MaNguoiDung == userId);

            if (donHang == null)
            {
                TempData["Error"] = "Không tìm thấy đơn hàng!";
                return RedirectToAction("LichSu");
            }

            if (donHang.TrangThai == true)
            {
                TempData["Error"] = "Đơn hàng đã được xác nhận, không thể hủy!";
                return RedirectToAction("LichSu");
            }

            // Hoàn lại số lượng tồn
            foreach (var ct in donHang.ChiTietDonHangs)
            {
                var sp = await _context.SanPhams.FindAsync(ct.MaSanPham);
                if (sp != null)
                {
                    sp.SoLuongTon += ct.SoLuong;
                }
            }

            _context.DonHangs.Remove(donHang);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Đơn hàng #{id} đã được hủy!";
            return RedirectToAction("LichSu");
        }
        // GET: Thanh toán
        [HttpGet]
        public async Task<IActionResult> ThanhToan(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["Warning"] = "Vui lòng đăng nhập để mua hàng!";
                return RedirectToAction("Login", "Account");
            }

            var sp = await _context.SanPhams.FindAsync(id);
            if (sp == null || sp.TrangThai == false || sp.SoLuongTon <= 0)
            {
                TempData["Error"] = "Sản phẩm không hợp lệ hoặc đã hết hàng!";
                return RedirectToAction("Index", "Home");
            }

            int userId = int.Parse(User.Claims.First(c => c.Type == "UserId").Value);
            //Lấy danh sách voucher của user đang còn hiệu lực gửi ViewModel này xuống View, để hiển thị sản phẩm và voucher
            var userVouchers = await _context.NguoiDungGiamGia
                .Include(nd => nd.MaKhuyenMaiNavigation)
                .Where(nd => nd.MaNguoiDung == userId && !nd.DaSuDung &&
                             nd.MaKhuyenMaiNavigation.TrangThai &&
                             nd.MaKhuyenMaiNavigation.NgayBatDau <= DateTime.Now &&
                             nd.MaKhuyenMaiNavigation.NgayKetThuc >= DateTime.Now)
                .ToListAsync();

            var vm = new ThanhToanViewModel
            {
                SanPham = sp,
                UserVouchers = userVouchers
            };

            return View(vm);
        }

        // POST: Thanh toán
        [HttpPost]
        public async Task<IActionResult> ThanhToan(int sanPhamId, int soLuong, int? voucherId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["Warning"] = "Vui lòng đăng nhập để mua hàng!";
                return RedirectToAction("Login", "Account");
            }

            int userId = int.Parse(User.Claims.First(c => c.Type == "UserId").Value);

            var sp = await _context.SanPhams.FindAsync(sanPhamId);
            if (sp == null || sp.TrangThai == false || sp.SoLuongTon < soLuong)
            {
                TempData["Error"] = "Sản phẩm không hợp lệ hoặc hết hàng.";
                return RedirectToAction("Index", "Home");
            }

            decimal donGia = sp.GiaBan ?? sp.Gia;
            decimal tongTien = donGia * soLuong;

            // Áp voucher nếu có
            if (voucherId.HasValue)
            {
                var voucher = await _context.GiamGia.FindAsync(voucherId.Value);
                var userVoucher = await _context.NguoiDungGiamGia
                    .FirstOrDefaultAsync(nd => nd.MaNguoiDung == userId && nd.MaKhuyenMai == voucherId.Value);

                if (voucher == null || !voucher.TrangThai ||
                    voucher.NgayBatDau > DateTime.Now || voucher.NgayKetThuc < DateTime.Now ||
                    userVoucher == null || userVoucher.DaSuDung)
                {
                    TempData["Warning"] = "Voucher không hợp lệ hoặc đã sử dụng!";
                    return RedirectToAction("ThanhToan", new { id = sanPhamId });
                }

                string loai = (voucher.Loai ?? "").Trim().ToLower();
                if (loai == "tiền mặt" || loai == "vnđ" || loai == "tien")
                {
                    tongTien -= voucher.GiaTri;
                }
                else if (loai == "phantram" || loai == "r")
                {
                    tongTien -= tongTien * (voucher.GiaTri / 100m);
                }

                if (tongTien < 0) tongTien = 0;

                // Đánh dấu voucher đã dùng
                userVoucher.DaSuDung = true;
                _context.NguoiDungGiamGia.Update(userVoucher);
                await _context.SaveChangesAsync();
            }

            // Tạo đơn hàng
            var donHang = new DonHang
            {
                MaNguoiDung = userId,
                NgayDatHang = DateTime.Now,
                TongTien = tongTien,
                TrangThai = false
            };
            _context.DonHangs.Add(donHang);
            await _context.SaveChangesAsync();

            // Thêm chi tiết đơn hàng
            var chiTiet = new ChiTietDonHang
            {
                MaDonHang = donHang.MaDonHang,
                MaSanPham = sp.MaSanPham,
                SoLuong = soLuong,
                DonGia = tongTien / soLuong
            };
            _context.ChiTietDonHangs.Add(chiTiet);

            // Trừ tồn kho
            sp.SoLuongTon -= soLuong;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Đơn hàng đã được tạo và đang chờ admin xác nhận!";
            return RedirectToAction("LichSu");
        }
    }

    }

