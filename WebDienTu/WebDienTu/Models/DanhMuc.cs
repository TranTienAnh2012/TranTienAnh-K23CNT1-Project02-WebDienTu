using System;
using System.Collections.Generic;

namespace WebDienTu.Models;

public partial class DanhMuc
{
    public int MaDanhMuc { get; set; }

    public string TenDanhMuc { get; set; } = null!;

    public string? MoTa { get; set; }

    public virtual ICollection<DanhMucThuocTinh> DanhMucThuocTinhs { get; set; } = new List<DanhMucThuocTinh>();

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
