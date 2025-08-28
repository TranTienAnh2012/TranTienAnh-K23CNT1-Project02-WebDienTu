using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebDienTu.Models;
using System.Linq;

namespace WebDienTu.Controllers
{
    public class DanhGiasController : Controller
    {
        private readonly DienTuStoreContext _context;

        public DanhGiasController(DienTuStoreContext context)
        {
            _context = context;
        }

        // 1. Lấy danh sách đánh giá (PartialView)
        public IActionResult List(int maSanPham)
        {
            var danhGias = _context.DanhGia
                .Include(d => d.MaNguoiDungNavigation) // để hiển thị tên người dùng
                .Where(d => d.MaSanPham == maSanPham)
                .OrderByDescending(d => d.NgayDanhGia)
                .ToList();

            // Tính trung bình sao
            ViewBag.TrungBinhSao = danhGias.Any() ? danhGias.Average(d => d.SoSao ?? 0) : 0;

            return PartialView("_DanhGiaList", danhGias);
        }

        // 2. Thêm đánh giá mới (Ajax, trả về JSON)
        [HttpPost]
        public IActionResult Create(int maSanPham, int soSao, string binhLuan)
        {
            if (!User.Identity.IsAuthenticated)
                return Json(new { success = false, message = "Bạn cần đăng nhập để đánh giá." });

            var userId = int.Parse(User.FindFirst("UserId").Value);

            var danhGia = new DanhGia
            {
                MaSanPham = maSanPham,
                MaNguoiDung = userId,
                SoSao = soSao,
                BinhLuan = binhLuan,
                NgayDanhGia = DateTime.Now
            };

            _context.DanhGia.Add(danhGia);
            _context.SaveChanges();

            // Tính trung bình sao mới
            var trungBinh = _context.DanhGia
                .Where(d => d.MaSanPham == maSanPham)
                .Average(d => (double?)d.SoSao) ?? 0;

            return Json(new { success = true, trungBinh });
        }
    }
}
