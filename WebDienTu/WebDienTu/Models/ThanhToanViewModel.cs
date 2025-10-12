using System.Collections.Generic;

namespace WebDienTu.Models
{
    public class ThanhToanViewModel
    {
        public SanPham SanPham { get; set; } = null!;
        public List<NguoiDungGiamGia> UserVouchers { get; set; } = new();
    }
}
