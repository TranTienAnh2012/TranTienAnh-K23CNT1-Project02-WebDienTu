
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebDienTu.Models;

public partial class SanPham
{
    public int MaSanPham { get; set; }

    public string TenSanPham { get; set; } = null!;

    public int MaDanhMuc { get; set; }

    public decimal Gia { get; set; }

    public string? MoTa { get; set; }

    public string? HinhAnh { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn phải là số nguyên >= 0")]
    public int SoLuongTon { get; set; }


    public bool? TrangThai { get; set; }

    public DateTime? NgayThem { get; set; }

    public string? Loai { get; set; }

    public decimal? GiaBan { get; set; }

    public string? ThuongHieu { get; set; }

    public string? XuatXu { get; set; }

    public string? BaoHanh { get; set; }

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual ICollection<DanhGia> DanhGia { get; set; } = new List<DanhGia>();

    public virtual ICollection<GiaTriThuocTinh> GiaTriThuocTinhs { get; set; } = new List<GiaTriThuocTinh>();

    public virtual ICollection<GioHangTam> GioHangTams { get; set; } = new List<GioHangTam>();

    public virtual DanhMuc? MaDanhMucNavigation { get; set; }


    public virtual ICollection<SanPhamDaXem> SanPhamDaXems { get; set; } = new List<SanPhamDaXem>();

    public virtual ICollection<GiamGia> MaKhuyenMais { get; set; } = new List<GiamGia>();
}
