using System;
using System.Collections.Generic;

namespace WebDienTu.Models;

public partial class NguoiDungGiamGia
{
    public int Id { get; set; }

    public int MaNguoiDung { get; set; }

    public int MaKhuyenMai { get; set; }

    public bool DaSuDung { get; set; }  // ← map trực tiếp từ DB

    public DateTime NgayNhan { get; set; }

    public virtual GiamGia MaKhuyenMaiNavigation { get; set; } = null!;

    public virtual QuanTriVien MaNguoiDungNavigation { get; set; } = null!;
}
