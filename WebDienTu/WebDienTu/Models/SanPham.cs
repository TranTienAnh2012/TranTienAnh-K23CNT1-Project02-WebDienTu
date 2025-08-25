using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema; // Cần cho [NotMapped]

namespace WebDienTu.Models
{
    public partial class SanPham
    {
        public int MaSanPham { get; set; }

        public string TenSanPham { get; set; } = null!;

        public int MaDanhMuc { get; set; }

        public decimal Gia { get; set; }

        public string? MoTa { get; set; }

        public string? HinhAnh { get; set; }

        public int? SoLuongTon { get; set; }

        public bool? TrangThai { get; set; }

        public DateTime? NgayThem { get; set; }

        public string? Loai { get; set; }

        public decimal? GiaBan { get; set; }

        public string? ThuongHieu { get; set; }

        public string? XuatXu { get; set; }

        public string? BaoHanh { get; set; }

        // Property tạm để upload ảnh (không tạo cột trong DB)
        [NotMapped]
        public IFormFile? HinhAnhFile { get; set; }

        [NotMapped]
        public decimal? PhanTramGiam
        {
            get
            {
                if (GiaBan.HasValue && GiaBan.Value < Gia && Gia > 0)
                {
                    return Math.Round((Gia - GiaBan.Value) / Gia * 100, 2);
                }
                return null;
            }
        }

        // Navigation property collections
        public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

        public virtual ICollection<DanhGia> DanhGia { get; set; } = new List<DanhGia>();

        public virtual ICollection<GioHangTam> GioHangTams { get; set; } = new List<GioHangTam>();

        public virtual DanhMuc? MaDanhMucNavigation { get; set; }

        public virtual ICollection<SanPhamDaXem> SanPhamDaXems { get; set; } = new List<SanPhamDaXem>();

        public virtual ICollection<GiamGia> MaKhuyenMais { get; set; } = new List<GiamGia>();

    }
}
