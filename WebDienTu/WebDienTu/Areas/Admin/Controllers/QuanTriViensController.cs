using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebDienTu.Models;
using X.PagedList;
using X.PagedList.Extensions;

namespace WebDienTu.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class QuanTriViensController : Controller
    {
        private readonly DienTuStoreContext _context;

        public QuanTriViensController(DienTuStoreContext context)
        {
            _context = context;
        }

        // GET: Admin/QuanTriViens
        public IActionResult Index(int? page)
        {
            int pageSize = 5; // số bản ghi trên 1 trang
            int pageNumber = page ?? 1;

            var list = _context.QuanTriViens
                               .OrderBy(q => q.VaiTro != 1)
                               .ToPagedList(pageNumber, pageSize);

            return View(list);
        }



        // GET: Admin/QuanTriViens/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quanTriVien = await _context.QuanTriViens
                .FirstOrDefaultAsync(m => m.MaNguoiDung == id);
            if (quanTriVien == null)
            {
                return NotFound();
            }

            return View(quanTriVien);
        }

        // GET: Admin/QuanTriViens/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/QuanTriViens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaNguoiDung,HoTen,Email,MatKhau,VaiTro,NgayDangKy")] QuanTriVien quanTriVien)
        {
            if (ModelState.IsValid)
            {
                _context.Add(quanTriVien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(quanTriVien);
        }

        // GET: Admin/QuanTriViens/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quanTriVien = await _context.QuanTriViens.FindAsync(id);
            if (quanTriVien == null)
            {
                return NotFound();
            }
            return View(quanTriVien);
        }

        // POST: Admin/QuanTriViens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaNguoiDung,HoTen,Email,MatKhau,VaiTro,NgayDangKy")] QuanTriVien quanTriVien)
        {
            if (id != quanTriVien.MaNguoiDung)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(quanTriVien);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuanTriVienExists(quanTriVien.MaNguoiDung))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(quanTriVien);
        }

        // GET: Admin/QuanTriViens/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quanTriVien = await _context.QuanTriViens
                .FirstOrDefaultAsync(m => m.MaNguoiDung == id);
            if (quanTriVien == null)
            {
                return NotFound();
            }

            return View(quanTriVien);
        }

        // POST: Admin/QuanTriViens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var quanTriVien = await _context.QuanTriViens.FindAsync(id);
            if (quanTriVien != null)
            {
                _context.QuanTriViens.Remove(quanTriVien);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuanTriVienExists(int id)
        {
            return _context.QuanTriViens.Any(e => e.MaNguoiDung == id);
        }
    }
}
