using System;
using System.Collections.Generic;

namespace WebDienTu.Models;

public partial class GiamGia 
{
    public int MaKhuyenMai { get; set; }

    public string TenChuongTrinh { get; set; } = null!;

    public string Loai { get; set; } = null!;

    public decimal GiaTri { get; set; }

    public DateTime NgayBatDau { get; set; }

    public DateTime NgayKetThuc { get; set; }

    public bool TrangThai { get; set; }

    public virtual ICollection<SanPham> MaSanPhams { get; set; } = new List<SanPham>();

}
