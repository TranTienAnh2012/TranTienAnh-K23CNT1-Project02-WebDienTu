using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebDienTu.Models;

public partial class QuanTriVien
{
    public int MaNguoiDung { get; set; }

    [Required(ErrorMessage = "Họ tên không được để trống!")]
    [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự!")]
    public string HoTen { get; set; } = null!;

    [Required(ErrorMessage = "Email không được để trống!")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ!")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Mật khẩu không được để trống!")]
    [StringLength(50, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự!")]
    public string MatKhau { get; set; } = null!;

    public int? VaiTro { get; set; }

    public DateTime? NgayDangKy { get; set; }

    public virtual ICollection<DanhGia> DanhGia { get; set; } = new List<DanhGia>();

    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();

    public virtual ICollection<GioHangTam> GioHangTams { get; set; } = new List<GioHangTam>();

    public virtual ICollection<NguoiDungGiamGia> NguoiDungGiamGia { get; set; } = new List<NguoiDungGiamGia>();

    public virtual ICollection<SanPhamDaXem> SanPhamDaXems { get; set; } = new List<SanPhamDaXem>();
}
