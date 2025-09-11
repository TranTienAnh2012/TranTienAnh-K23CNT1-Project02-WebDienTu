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
using X.PagedList.Mvc.Core;

namespace WebDienTu.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DanhGiaController : Controller
    {
        private readonly DienTuStoreContext _context;

        public DanhGiaController(DienTuStoreContext context)
        {
            _context = context;
        }

        // GET: Admin/DanhGia
        public async Task<IActionResult> Index(int? page)
        {
            int pageSize = 10; // số dòng mỗi trang
            int pageNumber = page ?? 1;

            var query = _context.DanhGia
                .Include(d => d.MaNguoiDungNavigation)
                .Include(d => d.MaSanPhamNavigation)
                .OrderByDescending(d => d.NgayDanhGia);

            var pagedList = query.ToPagedList(pageNumber, pageSize);
            return View(pagedList);
        }


        // GET: Admin/DanhGia/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var danhGium = await _context.DanhGia
                .Include(d => d.MaNguoiDungNavigation)
                .Include(d => d.MaSanPhamNavigation)
                .FirstOrDefaultAsync(m => m.MaDanhGia == id);
            if (danhGium == null)
            {
                return NotFound();
            }

            return View(danhGium);
        }

        // GET: Admin/DanhGia/Create
        public IActionResult Create()
        {
            ViewData["MaNguoiDung"] = new SelectList(_context.QuanTriViens, "MaNguoiDung", "MaNguoiDung");
            ViewData["MaSanPham"] = new SelectList(_context.SanPhams, "MaSanPham", "MaSanPham");
            return View();
        }

        // POST: Admin/DanhGia/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaDanhGia,MaSanPham,MaNguoiDung,SoSao,BinhLuan,NgayDanhGia")] DanhGia danhGium)
        {
            if (ModelState.IsValid)
            {
                _context.Add(danhGium);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaNguoiDung"] = new SelectList(_context.QuanTriViens, "MaNguoiDung", "MaNguoiDung", danhGium.MaNguoiDung);
            ViewData["MaSanPham"] = new SelectList(_context.SanPhams, "MaSanPham", "MaSanPham", danhGium.MaSanPham);
            return View(danhGium);
        }

        // GET: Admin/DanhGia/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var danhGium = await _context.DanhGia.FindAsync(id);
            if (danhGium == null)
            {
                return NotFound();
            }
            ViewData["MaNguoiDung"] = new SelectList(_context.QuanTriViens, "MaNguoiDung", "MaNguoiDung", danhGium.MaNguoiDung);
            ViewData["MaSanPham"] = new SelectList(_context.SanPhams, "MaSanPham", "MaSanPham", danhGium.MaSanPham);
            return View(danhGium);
        }

        // POST: Admin/DanhGia/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaDanhGia,MaSanPham,MaNguoiDung,SoSao,BinhLuan,NgayDanhGia")] DanhGia danhGium)
        {
            if (id != danhGium.MaDanhGia)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(danhGium);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DanhGiumExists(danhGium.MaDanhGia))
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
            ViewData["MaNguoiDung"] = new SelectList(_context.QuanTriViens, "MaNguoiDung", "MaNguoiDung", danhGium.MaNguoiDung);
            ViewData["MaSanPham"] = new SelectList(_context.SanPhams, "MaSanPham", "MaSanPham", danhGium.MaSanPham);
            return View(danhGium);
        }

        // GET: Admin/DanhGia/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var danhGium = await _context.DanhGia
                .Include(d => d.MaNguoiDungNavigation)
                .Include(d => d.MaSanPhamNavigation)
                .FirstOrDefaultAsync(m => m.MaDanhGia == id);
            if (danhGium == null)
            {
                return NotFound();
            }

            return View(danhGium);
        }

        // POST: Admin/DanhGia/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var danhGium = await _context.DanhGia.FindAsync(id);
            if (danhGium != null)
            {
                _context.DanhGia.Remove(danhGium);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DanhGiumExists(int id)
        {
            return _context.DanhGia.Any(e => e.MaDanhGia == id);
        }
    }
}
