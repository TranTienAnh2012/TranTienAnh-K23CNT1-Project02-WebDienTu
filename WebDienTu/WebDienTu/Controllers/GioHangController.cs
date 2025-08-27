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
