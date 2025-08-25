using System;
using System.Collections.Generic;

namespace WebDienTu.Models;

public partial class DanhGia
{
    public int MaDanhGia { get; set; }

    public int MaSanPham { get; set; }

    public int MaNguoiDung { get; set; }

    public int? SoSao { get; set; }

    public string? BinhLuan { get; set; }

    public DateTime? NgayDanhGia { get; set; }

    public virtual QuanTriVien MaNguoiDungNavigation { get; set; } = null!;

    public virtual SanPham MaSanPhamNavigation { get; set; } = null!;
}
