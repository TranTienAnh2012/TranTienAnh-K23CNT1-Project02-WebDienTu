using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;  

namespace WebDienTu.Areas.Admin.ViewModels
{
    public class SanPhamThuocTinhViewModel
    {
        [Required]
        public int MaSanPham { get; set; }   // Sản phẩm áp dụng

        public List<ThuocTinhGiaTriItem> ThuocTinhGiaTris { get; set; } = new();
    }

    public class ThuocTinhGiaTriItem
    {
        public int ThuocTinhId { get; set; }   // Thuộc tính (CPU, RAM, ...)
        public string GiaTri { get; set; } = string.Empty;   // Giá trị (Intel i5, 16GB ...)
    }
}
