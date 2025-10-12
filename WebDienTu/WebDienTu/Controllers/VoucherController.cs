using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebDienTu.Models;
using Microsoft.EntityFrameworkCore;

[Authorize] // Chỉ user đăng nhập mới được nhận voucher
public class VoucherController : Controller
{
    private readonly DienTuStoreContext _context;

    public VoucherController(DienTuStoreContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        int userId = int.Parse(User.Claims.First(c => c.Type == "UserId").Value);

        // Lấy tất cả voucher còn hiệu lực
        var vouchers = await _context.GiamGia
            .Where(v => v.TrangThai == true && v.NgayBatDau <= DateTime.Now && v.NgayKetThuc >= DateTime.Now)
            .OrderByDescending(v => v.NgayBatDau)
            .ToListAsync();

        // Lấy danh sách voucher user đã nhận
        var voucherDaNhan = await _context.NguoiDungGiamGia
            .Where(nd => nd.MaNguoiDung == userId)
            .Select(nd => nd.MaKhuyenMai)
            .ToListAsync();

        // Gắn trạng thái đã nhận cho view
        ViewBag.VoucherDaNhan = voucherDaNhan;

        return View(vouchers);
    }

    [HttpPost]
    public async Task<IActionResult> NhanVoucher(int id)
    {
        int userId = int.Parse(User.Claims.First(c => c.Type == "UserId").Value);

        var voucher = await _context.GiamGia.FindAsync(id);
        if (voucher == null || voucher.TrangThai == false)
        {
            TempData["Error"] = "Voucher không hợp lệ!";
            return RedirectToAction("Index");
        }

        // Kiểm tra user đã nhận voucher chưa
        bool daNhan = await _context.NguoiDungGiamGia
            .AnyAsync(x => x.MaNguoiDung == userId && x.MaKhuyenMai == id);
        if (daNhan)
        {
            TempData["Warning"] = "Bạn đã nhận voucher này rồi!";
            return RedirectToAction("Index");
        }

        // Thêm bản ghi user nhận voucher
        _context.NguoiDungGiamGia.Add(new NguoiDungGiamGia
        {
            MaNguoiDung = userId,
            MaKhuyenMai = id,
            DaSuDung = false, // mặc định chưa dùng
            NgayNhan = DateTime.Now
        });

        await _context.SaveChangesAsync();

        TempData["Success"] = "Nhận voucher thành công!";
        return RedirectToAction("Index");
    }
}
