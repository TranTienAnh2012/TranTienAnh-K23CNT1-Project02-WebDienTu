using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebDienTu.Models;

namespace WebDienTu.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class GiamGiasController : Controller
    {
        private readonly DienTuStoreContext _context;

        public GiamGiasController(DienTuStoreContext context)
        {
            _context = context;
        }

        // GET: Admin/GiamGias
        public async Task<IActionResult> Index()
        {
            var danhSach = await _context.GiamGia
                .OrderByDescending(g => g.NgayBatDau)
                .ToListAsync();
            return View(danhSach);
        }

        // GET: Admin/GiamGias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var giamGia = await _context.GiamGia
                .FirstOrDefaultAsync(m => m.MaKhuyenMai == id);

            if (giamGia == null) return NotFound();

            return View(giamGia);
        }

        // GET: Admin/GiamGias/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/GiamGias/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaKhuyenMai,TenChuongTrinh,Loai,GiaTri,NgayBatDau,NgayKetThuc,TrangThai")] GiamGia giamGia)
        {
            // Kiểm tra ngày
            if (giamGia.NgayBatDau >= giamGia.NgayKetThuc)
            {
                ModelState.AddModelError("", "⛔ Ngày bắt đầu phải nhỏ hơn ngày kết thúc.");
                return View(giamGia);
            }

            // Kiểm tra giá trị giảm
            if (giamGia.GiaTri <= 0)
            {
                ModelState.AddModelError("", "⛔ Giá trị giảm phải lớn hơn 0.");
                return View(giamGia);
            }

            if (ModelState.IsValid)
            {
                giamGia.TrangThai = true; // Mặc định voucher mới là đang hoạt động
                _context.Add(giamGia);
                await _context.SaveChangesAsync();
                TempData["Success"] = "✅ Tạo chương trình giảm giá thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(giamGia);
        }

        // GET: Admin/GiamGias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var giamGia = await _context.GiamGia.FindAsync(id);
            if (giamGia == null) return NotFound();

            return View(giamGia);
        }

        // POST: Admin/GiamGias/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaKhuyenMai,TenChuongTrinh,Loai,GiaTri,NgayBatDau,NgayKetThuc,TrangThai")] GiamGia giamGia)
        {
            if (id != giamGia.MaKhuyenMai) return NotFound();

            if (giamGia.NgayBatDau >= giamGia.NgayKetThuc)
            {
                ModelState.AddModelError("", "⛔ Ngày bắt đầu phải nhỏ hơn ngày kết thúc.");
                return View(giamGia);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(giamGia);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "✅ Cập nhật chương trình giảm giá thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GiamGiaExists(giamGia.MaKhuyenMai))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(giamGia);
        }

        // GET: Admin/GiamGias/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var giamGia = await _context.GiamGia
                .FirstOrDefaultAsync(m => m.MaKhuyenMai == id);

            if (giamGia == null) return NotFound();

            return View(giamGia);
        }

        // POST: Admin/GiamGias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var giamGia = await _context.GiamGia.FindAsync(id);
            if (giamGia != null)
            {
                // ❗ Xóa mềm: chỉ tắt trạng thái thay vì Remove
                giamGia.TrangThai = false;
                _context.Update(giamGia);
                TempData["Warning"] = "⚠ Voucher đã được vô hiệu hóa.";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GiamGiaExists(int id)
        {
            return _context.GiamGia.Any(e => e.MaKhuyenMai == id);
        }
    }
}
