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
    public class DonHangsController : Controller
    {
        private readonly DienTuStoreContext _context;

        public DonHangsController(DienTuStoreContext context)
        {
            _context = context;
        }

        // GET: Admin/DonHangs
        public async Task<IActionResult> Index()
        {
            var dienTuStoreContext = _context.DonHangs.Include(d => d.MaNguoiDungNavigation);
            return View(await dienTuStoreContext.ToListAsync());
        }

        // GET: Admin/DonHangs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var donHang = await _context.DonHangs
                .Include(d => d.MaNguoiDungNavigation)
                .FirstOrDefaultAsync(m => m.MaDonHang == id);
            if (donHang == null)
            {
                return NotFound();
            }

            return View(donHang);
        }

        // GET: Admin/DonHangs/Create
        public IActionResult Create()
        {
            ViewData["MaNguoiDung"] = new SelectList(_context.QuanTriViens, "MaNguoiDung", "MaNguoiDung");
            return View();
        }

        // POST: Admin/DonHangs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaDonHang,MaNguoiDung,NgayDatHang,TongTien,TrangThai")] DonHang donHang)
        {
            if (ModelState.IsValid)
            {
                _context.Add(donHang);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaNguoiDung"] = new SelectList(_context.QuanTriViens, "MaNguoiDung", "MaNguoiDung", donHang.MaNguoiDung);
            return View(donHang);
        }
        // GET: Admin/DonHangs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var donHang = await _context.DonHangs.FindAsync(id);
            if (donHang == null) return NotFound();

            // Dropdown trạng thái: bind bool
            ViewBag.TrangThaiList = new SelectList(new[]
            {
        new { Value = "true", Text = "Đã xác nhận" },
        new { Value = "false", Text = "Chưa xác nhận" }
    }, "Value", "Text", donHang.TrangThai?.ToString().ToLower());

            // Dropdown người đặt
            ViewData["MaNguoiDung"] = new SelectList(
                _context.QuanTriViens,
                "MaNguoiDung",
                "MaNguoiDung",
                donHang.MaNguoiDung
            );

            return View(donHang);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaDonHang,MaNguoiDung,NgayDatHang,TongTien,TrangThai")] DonHang donHang)
        {
            if (id != donHang.MaDonHang) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.DonHangs
                        .Include(d => d.MaNguoiDungNavigation)
                        .FirstOrDefaultAsync(d => d.MaDonHang == id);

                    if (existing == null) return NotFound();

                    // Update từng field
                    existing.MaNguoiDung = donHang.MaNguoiDung;
                    existing.NgayDatHang = donHang.NgayDatHang;
                    existing.TongTien = donHang.TongTien;
                    existing.TrangThai = donHang.TrangThai;

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.DonHangs.Any(e => e.MaDonHang == id)) return NotFound();
                    throw;
                }
            }

            // Reload dropdown nếu lỗi
            ViewBag.TrangThaiList = new SelectList(new[]
            {
        new { Value = "true", Text = "Đã xác nhận" },
        new { Value = "false", Text = "Chưa xác nhận" }
    }, "Value", "Text", donHang.TrangThai?.ToString().ToLower());

            ViewData["MaNguoiDung"] = new SelectList(
                _context.QuanTriViens,
                "MaNguoiDung",
                "MaNguoiDung",
                donHang.MaNguoiDung
            );

            return View(donHang);
        }


        // GET: Admin/DonHangs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var donHang = await _context.DonHangs
                .Include(d => d.MaNguoiDungNavigation)
                .FirstOrDefaultAsync(m => m.MaDonHang == id);
            if (donHang == null)
            {
                return NotFound();
            }

            return View(donHang);
        }

        // POST: Admin/DonHangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var donHang = await _context.DonHangs.FindAsync(id);
            if (donHang != null)
            {
                _context.DonHangs.Remove(donHang);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DonHangExists(int id)
        {
            return _context.DonHangs.Any(e => e.MaDonHang == id);
        }
    }
}
