using Microsoft.AspNetCore.Mvc;
using WebDienTu.Models;
using Microsoft.EntityFrameworkCore;

namespace WebDienTu.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminHomeController : Controller
    {
        private readonly DienTuStoreContext _context;

        public AdminHomeController(DienTuStoreContext context)
        {
            _context = context;
        }

        // GET: Admin/AdminHome
        public async Task<IActionResult> Index()
        {
            // Tổng số quản trị viên
            var totalAdmins = await _context.QuanTriViens.CountAsync(u => u.VaiTro == 1);

            // Tổng số users
            var totalUsers = await _context.QuanTriViens.CountAsync(u => u.VaiTro == 0);

            // Tổng số sản phẩm
            var totalProducts = await _context.SanPhams.CountAsync();

            // Tổng số đơn hàng
            var totalOrders = await _context.DonHangs.CountAsync();

            // Tổng số chi tiết đơn hàng
            var totalOrderDetails = await _context.ChiTietDonHangs.CountAsync();

            // Tổng số đánh giá
            var totalReviews = await _context.DanhGia.CountAsync();

            // Tổng số danh mục
            var totalCategories = await _context.DanhMucs.CountAsync();

            // Truyền dữ liệu sang View bằng ViewBag
            ViewBag.TotalAdmins = totalAdmins;
            ViewBag.TotalUsers = totalUsers;
            ViewBag.TotalProducts = totalProducts;
            ViewBag.TotalOrders = totalOrders;
            ViewBag.TotalOrderDetails = totalOrderDetails;
            ViewBag.TotalReviews = totalReviews;
            ViewBag.TotalCategories = totalCategories;

            return View();
        }
    }
}
