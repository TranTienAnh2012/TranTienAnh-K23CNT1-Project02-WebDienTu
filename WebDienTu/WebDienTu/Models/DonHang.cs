using System;
using System.Collections.Generic;

namespace WebDienTu.Models;

public partial class DonHang
{
    public int MaDonHang { get; set; }

    public int MaNguoiDung { get; set; }

    public DateTime? NgayDatHang { get; set; }

    public decimal TongTien { get; set; }

    public bool? TrangThai { get; set; }

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    // Navigation property nullable để EF không bắt buộc phải có giá trị khi insert/update
    public virtual QuanTriVien? MaNguoiDungNavigation { get; set; }
}