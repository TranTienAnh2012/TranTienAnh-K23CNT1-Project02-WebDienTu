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


    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();


    public virtual ICollection<NguoiDungGiamGia> NguoiDungGiamGia { get; set; } = new List<NguoiDungGiamGia>();



}
