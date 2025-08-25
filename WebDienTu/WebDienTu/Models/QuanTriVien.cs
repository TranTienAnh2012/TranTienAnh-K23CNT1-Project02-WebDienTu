using System;
using System.Collections.Generic;

namespace WebDienTu.Models;

public partial class QuanTriVien
{
    public int MaNguoiDung { get; set; }

    public string HoTen { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public int? VaiTro { get; set; }

    public DateTime? NgayDangKy { get; set; }

    public virtual ICollection<DanhGia> DanhGia { get; set; } = new List<DanhGia>();

    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();

    public virtual ICollection<GioHangTam> GioHangTams { get; set; } = new List<GioHangTam>();

    public virtual ICollection<SanPhamDaXem> SanPhamDaXems { get; set; } = new List<SanPhamDaXem>();
}
