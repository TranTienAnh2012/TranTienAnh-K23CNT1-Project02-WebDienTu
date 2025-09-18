using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebDienTu.Areas.Admin.ViewModels;
using WebDienTu.Models;

namespace WebDienTu.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SanPhamThuocTinhsController : Controller
    {
        private readonly DienTuStoreContext _context;

        public SanPhamThuocTinhsController(DienTuStoreContext context)
        {
            _context = context;
        }

        // GET: Admin/SanPhamThuocTinhs/Create
        public IActionResult Create()
        {
            LoadDropDowns();
            return View(new SanPhamThuocTinhViewModel());
        }

        // POST: Admin/SanPhamThuocTinhs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SanPhamThuocTinhViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.ThuocTinhGiaTris != null && model.ThuocTinhGiaTris.Any())
                {
                    foreach (var item in model.ThuocTinhGiaTris)
                    {
                        if (!string.IsNullOrWhiteSpace(item.GiaTri))
                        {
                            var giaTri = new GiaTriThuocTinh
                            {
                                MaSanPham = model.MaSanPham,
                                ThuocTinhId = item.ThuocTinhId,
                                GiaTri = item.GiaTri
                            };
                            _context.GiaTriThuocTinhs.Add(giaTri);
                        }
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "GiaTriThuocTinhs");
                }
            }

            // load lại dropdown khi có lỗi
            LoadDropDowns(model.MaSanPham);
            return View(model);
        }

        // AJAX: Lấy thuộc tính theo sản phẩm
        [HttpGet]
        public IActionResult GetThuocTinhsBySanPham(int maSanPham)
        {
            // Tìm sản phẩm
            var sanPham = _context.SanPhams
                .AsNoTracking()
                .FirstOrDefault(sp => sp.MaSanPham == maSanPham);

            if (sanPham == null)
                return Json(new { success = false, message = "Không tìm thấy sản phẩm" });

            var thuocTinhs = _context.DanhMucThuocTinhs
                .AsNoTracking()
                .Where(dmtt => dmtt.MaDanhMuc == sanPham.MaDanhMuc)
                .Include(dmtt => dmtt.ThuocTinh)
                .Select(dmtt => new
                {
                    thuocTinhId = dmtt.ThuocTinhId,
                    tenThuocTinh = dmtt.ThuocTinh.TenThuocTinh
                })
                .ToList();

            if (!thuocTinhs.Any())
                return Json(new { success = false, message = "Danh mục chưa có thuộc tính nào" });

            return Json(new { success = true, data = thuocTinhs });
        }

        // Load dropdown sản phẩm
        private void LoadDropDowns(int? selectedSanPham = null)
        {
            ViewBag.MaSanPham = new SelectList(
                _context.SanPhams.AsNoTracking(),
                "MaSanPham",
                "TenSanPham",
                selectedSanPham
            );
        }
    }
}
