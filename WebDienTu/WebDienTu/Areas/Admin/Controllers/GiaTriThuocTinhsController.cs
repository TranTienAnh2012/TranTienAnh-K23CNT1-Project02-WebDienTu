using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebDienTu.Areas.Admin.ViewModels;
using WebDienTu.Models;

namespace WebDienTu.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class GiaTriThuocTinhsController : Controller
    {
        private readonly DienTuStoreContext _context;

        public GiaTriThuocTinhsController(DienTuStoreContext context)
        {
            _context = context;
        }

        // GET: Admin/GiaTriThuocTinhs
        public async Task<IActionResult> Index()
        {
            var dienTuStoreContext = _context.GiaTriThuocTinhs
                .Include(g => g.MaSanPhamNavigation)
                .Include(g => g.ThuocTinh);
            return View(await dienTuStoreContext.ToListAsync());
        }

        //// GET: Admin/GiaTriThuocTinhs/Create
        //public IActionResult Create()
        //{
        //    // danh sách sản phẩm
        //    ViewBag.MaSanPham = new SelectList(_context.SanPhams, "MaSanPham", "TenSanPham");

        //    // danh sách thuộc tính
        //    ViewBag.ThuocTinhs = _context.ThuocTinhs.ToList();

        //    return View(new SanPhamThuocTinhViewModel());
        //}

        //// POST: Admin/GiaTriThuocTinhs/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(SanPhamThuocTinhViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        foreach (var item in model.ThuocTinhGiaTris)
        //        {
        //            if (!string.IsNullOrWhiteSpace(item.GiaTri))
        //            {
        //                var giaTri = new GiaTriThuocTinh
        //                {
        //                    MaSanPham = model.MaSanPham,
        //                    ThuocTinhId = item.ThuocTinhId,
        //                    GiaTri = item.GiaTri
        //                };

        //                _context.GiaTriThuocTinhs.Add(giaTri);
        //            }
        //        }

        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }

        //    // load lại dropdown nếu có lỗi
        //    ViewBag.MaSanPham = new SelectList(_context.SanPhams, "MaSanPham", "TenSanPham", model.MaSanPham);
        //    ViewBag.ThuocTinhs = _context.ThuocTinhs.ToList();

        //    return View(model);
        //}

        // các action Edit, Details, Delete giữ nguyên
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var giaTriThuocTinh = await _context.GiaTriThuocTinhs
                .Include(g => g.MaSanPhamNavigation)
                .Include(g => g.ThuocTinh)
                .FirstOrDefaultAsync(m => m.GiaTriId == id);

            if (giaTriThuocTinh == null) return NotFound();

            return View(giaTriThuocTinh);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var giaTriThuocTinh = await _context.GiaTriThuocTinhs.FindAsync(id);
            if (giaTriThuocTinh == null) return NotFound();

            ViewData["MaSanPham"] = new SelectList(_context.SanPhams, "MaSanPham", "TenSanPham", giaTriThuocTinh.MaSanPham);
            ViewData["ThuocTinhId"] = new SelectList(_context.ThuocTinhs, "ThuocTinhId", "TenThuocTinh", giaTriThuocTinh.ThuocTinhId);

            return View(giaTriThuocTinh);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, GiaTriThuocTinh giaTriThuocTinh)
        {
            if (id != giaTriThuocTinh.GiaTriId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(giaTriThuocTinh);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.GiaTriThuocTinhs.Any(e => e.GiaTriId == giaTriThuocTinh.GiaTriId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["MaSanPham"] = new SelectList(_context.SanPhams, "MaSanPham", "TenSanPham", giaTriThuocTinh.MaSanPham);
            ViewData["ThuocTinhId"] = new SelectList(_context.ThuocTinhs, "ThuocTinhId", "TenThuocTinh", giaTriThuocTinh.ThuocTinhId);

            return View(giaTriThuocTinh);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var giaTriThuocTinh = await _context.GiaTriThuocTinhs
                .Include(g => g.MaSanPhamNavigation)
                .Include(g => g.ThuocTinh)
                .FirstOrDefaultAsync(m => m.GiaTriId == id);

            if (giaTriThuocTinh == null) return NotFound();

            return View(giaTriThuocTinh);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var giaTriThuocTinh = await _context.GiaTriThuocTinhs.FindAsync(id);
            if (giaTriThuocTinh != null)
            {
                _context.GiaTriThuocTinhs.Remove(giaTriThuocTinh);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
