using Microsoft.AspNetCore.Mvc;
using WebDienTu.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class SanPhamDaXemController : Controller
{
    private readonly DienTuStoreContext _context;

    public SanPhamDaXemController(DienTuStoreContext context)
    {
        _context = context;
    }

    // 👉 Lấy danh sách sản phẩm đã xem của user (Top 5)
    public async Task<IActionResult> Index(int maNguoiDung)
    {
        if (maNguoiDung <= 0)
            return PartialView("_SanPhamDaXemPartial", new List<SanPham>());

        var list = await _context.SanPhamDaXems
            .AsNoTracking() // Tối ưu truy vấn chỉ đọc
            .Where(x => x.MaNguoiDung == maNguoiDung)
            .OrderByDescending(x => x.ThoiGianXem)
            .Include(x => x.MaSanPhamNavigation)
            .Select(x => x.MaSanPhamNavigation)
            .Take(5)
            .ToListAsync();

        return PartialView("_SanPhamDaXemPartial", list);
    }

    // 👉 Lưu sản phẩm đã xem (tránh trùng lặp + giới hạn 20 sản phẩm gần nhất)
    [HttpPost]
    public async Task<IActionResult> LuuDaXem(int maNguoiDung, int maSanPham)
    {
        if (maNguoiDung <= 0 || maSanPham <= 0)
            return BadRequest("Thiếu dữ liệu hợp lệ.");

        // Kiểm tra sản phẩm tồn tại
        var sanPham = await _context.SanPhams.AnyAsync(s => s.MaSanPham == maSanPham && s.TrangThai == true);
        if (!sanPham)
            return BadRequest("Sản phẩm không tồn tại hoặc không hoạt động.");

        try
        {
            var daXem = await _context.SanPhamDaXems
                .FirstOrDefaultAsync(x => x.MaNguoiDung == maNguoiDung && x.MaSanPham == maSanPham);

            if (daXem == null)
            {
                daXem = new SanPhamDaXem
                {
                    MaNguoiDung = maNguoiDung,
                    MaSanPham = maSanPham,
                    ThoiGianXem = DateTime.UtcNow // Đồng bộ múi giờ
                };
                await _context.SanPhamDaXems.AddAsync(daXem);
            }
            else
            {
                daXem.ThoiGianXem = DateTime.UtcNow; // Đồng bộ múi giờ
                _context.SanPhamDaXems.Update(daXem);
            }

            await _context.SaveChangesAsync();

            // ✅ Giới hạn 20 sản phẩm gần nhất để tránh phình dữ liệu
            var toDelete = await _context.SanPhamDaXems
                .Where(x => x.MaNguoiDung == maNguoiDung)
                .OrderByDescending(x => x.ThoiGianXem)
                .Skip(20)
                .ToListAsync();

            if (toDelete.Any())
            {
                _context.SanPhamDaXems.RemoveRange(toDelete);
                await _context.SaveChangesAsync();
            }

            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }
}