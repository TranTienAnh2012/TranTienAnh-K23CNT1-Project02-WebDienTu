using Microsoft.AspNetCore.Mvc;
using WebDienTu.Models;
using Microsoft.EntityFrameworkCore;

public class SanPhamDaXemController : Controller
{
    private readonly DienTuStoreContext _context;

    public SanPhamDaXemController(DienTuStoreContext context)
    {
        _context = context;
    }

    // 👉 Lấy danh sách sản phẩm đã xem của user
    public IActionResult Index(int maNguoiDung)
    {
        if (maNguoiDung <= 0)
            return PartialView("_SanPhamDaXemPartial", new List<SanPham>());

        var list = _context.SanPhamDaXems
            .Where(x => x.MaNguoiDung == maNguoiDung)
            .OrderByDescending(x => x.ThoiGianXem)
            .Include(x => x.MaSanPhamNavigation) // load navigation
            .Select(x => x.MaSanPhamNavigation)  // lấy sản phẩm
            .Take(5)
            .ToList();

        return PartialView("_SanPhamDaXemPartial", list); // trả về List<SanPham>
    }

    // 👉 Lưu sản phẩm đã xem (tránh trùng lặp)
    [HttpPost]
    public IActionResult LuuDaXem(int maNguoiDung, int maSanPham)
    {
        if (maNguoiDung <= 0 || maSanPham <= 0)
            return BadRequest("Thiếu dữ liệu hợp lệ.");

        try
        {
            var daXem = _context.SanPhamDaXems
                .FirstOrDefault(x => x.MaNguoiDung == maNguoiDung && x.MaSanPham == maSanPham);

            if (daXem == null)
            {
                daXem = new SanPhamDaXem
                {
                    MaNguoiDung = maNguoiDung,
                    MaSanPham = maSanPham,
                    ThoiGianXem = DateTime.Now
                };
                _context.SanPhamDaXems.Add(daXem);
            }
            else
            {
                daXem.ThoiGianXem = DateTime.Now;
                _context.SanPhamDaXems.Update(daXem);
            }

            _context.SaveChanges();
            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }
}
