using System;
using System.Collections.Generic;

namespace WebDienTu.Models;

public partial class GioHangTam
{
    public int Id { get; set; }

    public int MaNguoiDung { get; set; }

    public int MaSanPham { get; set; }

    public int SoLuong { get; set; }

    public string TrangThai { get; set; } = null!;

    public virtual QuanTriVien MaNguoiDungNavigation { get; set; } = null!;

    public virtual SanPham MaSanPhamNavigation { get; set; } = null!;
}
