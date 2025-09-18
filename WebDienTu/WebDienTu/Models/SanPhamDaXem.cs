using System;
using System.Collections.Generic;

namespace WebDienTu.Models;

public partial class SanPhamDaXem
{
    public int MaDaXem { get; set; }

    public int MaNguoiDung { get; set; }

    public int MaSanPham { get; set; }

    public DateTime? ThoiGianXem { get; set; }

    public virtual QuanTriVien MaNguoiDungNavigation { get; set; } = null!;

    public virtual SanPham MaSanPhamNavigation { get; set; } = null!;
}
