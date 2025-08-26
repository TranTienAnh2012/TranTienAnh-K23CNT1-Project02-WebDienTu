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
    public class SanPhamsController : Controller
    {
        private readonly DienTuStoreContext _context;

        public SanPhamsController(DienTuStoreContext context)
        {
            _context = context;
        }

        // GET: Admin/SanPhams
        public async Task<IActionResult> Index()
        {
            var dienTuStoreContext = _context.SanPhams.Include(s => s.MaDanhMucNavigation);
            return View(await dienTuStoreContext.ToListAsync());
        }

        // GET: Admin/SanPhams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams
                .Include(s => s.MaDanhMucNavigation)
                .FirstOrDefaultAsync(m => m.MaSanPham == id);
            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }

        // GET: Admin/SanPhams/Create
        public IActionResult Create()
        {

            ViewData["MaDanhMuc"] = new SelectList(_context.DanhMucs, "MaDanhMuc", "MaDanhMuc");
            // Đường dẫn gốc trong wwwroot
            var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

            // Lấy tất cả ảnh trong các thư mục con
            var imageFiles = Directory.GetFiles(rootPath, "*.*", SearchOption.AllDirectories)
                                      .Where(f => f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                                               || f.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                                      .Select(f => "/images/" + Path.GetRelativePath(rootPath, f).Replace("\\", "/"))
                                      .ToList();

            ViewBag.ImageList = new SelectList(imageFiles);
            return View();
        }

        // POST: Admin/SanPhams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaSanPham,TenSanPham,MaDanhMuc,Gia,MoTa,HinhAnh,SoLuongTon,TrangThai,NgayThem,Loai,GiaBan,ThuongHieu,XuatXu,BaoHanh")] SanPham sanPham)
        {
            if (ModelState.IsValid)
            {
              

                _context.Add(sanPham);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            ViewData["MaDanhMuc"] = new SelectList(_context.DanhMucs, "MaDanhMuc", "MaDanhMuc", sanPham.MaDanhMuc);
            return View(sanPham);
        }

        // GET: Admin/SanPhams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham == null) return NotFound();

            // Danh mục
            ViewData["MaDanhMuc"] = new SelectList(_context.DanhMucs, "MaDanhMuc", "TenDanhMuc", sanPham.MaDanhMuc);

            // Trạng thái
            ViewBag.TrangThaiList = new List<SelectListItem>
            {
                new SelectListItem { Text = "Hiển thị", Value = "True", Selected = sanPham.TrangThai == true },
                new SelectListItem { Text = "Ẩn", Value = "False", Selected = sanPham.TrangThai == false }
            };


            // Hình ảnh
            var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
            var imageFiles = Directory.GetFiles(rootPath, "*.*", SearchOption.AllDirectories)
                                      .Where(f => f.EndsWith(".jpg") || f.EndsWith(".png"))
                                      .Select(f => "/images/" + Path.GetRelativePath(rootPath, f).Replace("\\", "/"))
                                      .ToList();
            ViewBag.ImageList = new SelectList(imageFiles);

            return View(sanPham);
        }


        // POST: Admin/SanPhams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaSanPham,TenSanPham,MaDanhMuc,Gia,MoTa,HinhAnh,SoLuongTon,TrangThai,NgayThem,Loai,GiaBan,ThuongHieu,XuatXu,BaoHanh")] SanPham sanPham)
        {
            if (id != sanPham.MaSanPham)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sanPham);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SanPhamExists(sanPham.MaSanPham))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine("ModelState Errors: " + string.Join("; ", errors));
            }

            // Reload dropdowns
            ViewData["MaDanhMuc"] = new SelectList(_context.DanhMucs, "MaDanhMuc", "TenDanhMuc", sanPham.MaDanhMuc);

            ViewBag.TrangThaiList = new List<SelectListItem>
    {
        new SelectListItem { Text = "Hiển thị", Value = "True", Selected = sanPham.TrangThai == true },
        new SelectListItem { Text = "Ẩn", Value = "False", Selected = sanPham.TrangThai == false }
    };

            var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
            var imageFiles = Directory.GetFiles(rootPath, "*.*", SearchOption.AllDirectories)
                                      .Where(f => f.EndsWith(".jpg") || f.EndsWith(".png"))
                                      .Select(f => "/images/" + Path.GetRelativePath(rootPath, f).Replace("\\", "/"))
                                      .ToList();
            ViewBag.ImageList = new SelectList(imageFiles, sanPham.HinhAnh);

            return View(sanPham);
        }



        // GET: Admin/SanPhams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams
                .Include(s => s.MaDanhMucNavigation)
                .FirstOrDefaultAsync(m => m.MaSanPham == id);
            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }

        // POST: Admin/SanPhams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham != null)
            {
                _context.SanPhams.Remove(sanPham);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SanPhamExists(int id)
        {
            return _context.SanPhams.Any(e => e.MaSanPham == id);
        }
    }
}
