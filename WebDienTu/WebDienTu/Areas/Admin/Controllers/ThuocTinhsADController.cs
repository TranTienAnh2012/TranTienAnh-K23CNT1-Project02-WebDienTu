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
    public class ThuocTinhsADController : Controller
    {
        private readonly DienTuStoreContext _context;

        public ThuocTinhsADController(DienTuStoreContext context)
        {
            _context = context;
        }

        // GET: Admin/ThuocTinhsAD
        public async Task<IActionResult> Index()
        {
            return View(await _context.ThuocTinhs.ToListAsync());
        }

        // GET: Admin/ThuocTinhsAD/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thuocTinh = await _context.ThuocTinhs
                .FirstOrDefaultAsync(m => m.ThuocTinhId == id);
            if (thuocTinh == null)
            {
                return NotFound();
            }

            return View(thuocTinh);
        }

        // GET: Admin/ThuocTinhsAD/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/ThuocTinhsAD/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ThuocTinhId,TenThuocTinh")] ThuocTinh thuocTinh)
        {
            if (ModelState.IsValid)
            {
                _context.Add(thuocTinh);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(thuocTinh);
        }

        // GET: Admin/ThuocTinhsAD/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thuocTinh = await _context.ThuocTinhs.FindAsync(id);
            if (thuocTinh == null)
            {
                return NotFound();
            }
            return View(thuocTinh);
        }

        // POST: Admin/ThuocTinhsAD/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ThuocTinhId,TenThuocTinh")] ThuocTinh thuocTinh)
        {
            if (id != thuocTinh.ThuocTinhId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(thuocTinh);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ThuocTinhExists(thuocTinh.ThuocTinhId))
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
            return View(thuocTinh);
        }

        // GET: Admin/ThuocTinhsAD/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thuocTinh = await _context.ThuocTinhs
                .FirstOrDefaultAsync(m => m.ThuocTinhId == id);
            if (thuocTinh == null)
            {
                return NotFound();
            }

            return View(thuocTinh);
        }

        // POST: Admin/ThuocTinhsAD/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var thuocTinh = await _context.ThuocTinhs.FindAsync(id);
            if (thuocTinh != null)
            {
                _context.ThuocTinhs.Remove(thuocTinh);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ThuocTinhExists(int id)
        {
            return _context.ThuocTinhs.Any(e => e.ThuocTinhId == id);
        }
    }
}
