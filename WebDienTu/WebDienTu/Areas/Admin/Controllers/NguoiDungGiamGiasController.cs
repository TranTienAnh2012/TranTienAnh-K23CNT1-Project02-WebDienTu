using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebDienTu.Models;

namespace WebDienTu.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class NguoiDungGiamGiasController : Controller
    {
        private readonly DienTuStoreContext _context;

        public NguoiDungGiamGiasController(DienTuStoreContext context)
        {
            _context = context;
        }

        // GET: Admin/NguoiDungGiamGias
        public async Task<IActionResult> Index()
        {
            var dienTuStoreContext = _context.NguoiDungGiamGia.Include(n => n.MaKhuyenMaiNavigation).Include(n => n.MaNguoiDungNavigation);
            return View(await dienTuStoreContext.ToListAsync());
        }

        // GET: Admin/NguoiDungGiamGias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nguoiDungGiamGia = await _context.NguoiDungGiamGia
                .Include(n => n.MaKhuyenMaiNavigation)
                .Include(n => n.MaNguoiDungNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (nguoiDungGiamGia == null)
            {
                return NotFound();
            }

            return View(nguoiDungGiamGia);
        }

        // GET: Admin/NguoiDungGiamGias/Create
        public IActionResult Create()
        {
            ViewData["MaKhuyenMai"] = new SelectList(_context.GiamGia, "MaKhuyenMai", "TenChuongTrinh");
            ViewData["MaNguoiDung"] = new SelectList(_context.QuanTriViens, "MaNguoiDung", "Email");
            return View();
        }

        // POST: Admin/NguoiDungGiamGias/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MaNguoiDung,MaKhuyenMai,DaSuDung,NgayNhan")] NguoiDungGiamGia nguoiDungGiamGia)
        {
            if (ModelState.IsValid)
            {
                // Nếu muốn mặc định chưa sử dụng
                nguoiDungGiamGia.DaSuDung = nguoiDungGiamGia.DaSuDung; // hoặc false nếu muốn
                _context.Add(nguoiDungGiamGia);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaKhuyenMai"] = new SelectList(_context.GiamGia, "MaKhuyenMai", "TenChuongTrinh", nguoiDungGiamGia.MaKhuyenMai);
            ViewData["MaNguoiDung"] = new SelectList(_context.QuanTriViens, "MaNguoiDung", "Email", nguoiDungGiamGia.MaNguoiDung);
            return View(nguoiDungGiamGia);
        }

        // GET: Admin/NguoiDungGiamGias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var nguoiDungGiamGia = await _context.NguoiDungGiamGia.FindAsync(id);
            if (nguoiDungGiamGia == null)
                return NotFound();

            ViewData["MaKhuyenMai"] = new SelectList(_context.GiamGia, "MaKhuyenMai", "TenChuongTrinh", nguoiDungGiamGia.MaKhuyenMai);
            ViewData["MaNguoiDung"] = new SelectList(_context.QuanTriViens, "MaNguoiDung", "Email", nguoiDungGiamGia.MaNguoiDung);
            return View(nguoiDungGiamGia);
        }

        // POST: Admin/NguoiDungGiamGias/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MaNguoiDung,MaKhuyenMai,DaSuDung,NgayNhan")] NguoiDungGiamGia nguoiDungGiamGia)
        {
            if (id != nguoiDungGiamGia.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nguoiDungGiamGia);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NguoiDungGiamGiaExists(nguoiDungGiamGia.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaKhuyenMai"] = new SelectList(_context.GiamGia, "MaKhuyenMai", "TenChuongTrinh", nguoiDungGiamGia.MaKhuyenMai);
            ViewData["MaNguoiDung"] = new SelectList(_context.QuanTriViens, "MaNguoiDung", "Email", nguoiDungGiamGia.MaNguoiDung);
            return View(nguoiDungGiamGia);
        }

        // GET: Admin/NguoiDungGiamGias/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nguoiDungGiamGia = await _context.NguoiDungGiamGia
                .Include(n => n.MaKhuyenMaiNavigation)
                .Include(n => n.MaNguoiDungNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (nguoiDungGiamGia == null)
            {
                return NotFound();
            }

            return View(nguoiDungGiamGia);
        }

        // POST: Admin/NguoiDungGiamGias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nguoiDungGiamGia = await _context.NguoiDungGiamGia.FindAsync(id);
            if (nguoiDungGiamGia != null)
            {
                _context.NguoiDungGiamGia.Remove(nguoiDungGiamGia);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NguoiDungGiamGiaExists(int id)
        {
            return _context.NguoiDungGiamGia.Any(e => e.Id == id);
        }
    }
}
