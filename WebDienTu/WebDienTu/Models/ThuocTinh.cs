using System;
using System.Collections.Generic;

namespace WebDienTu.Models;

public partial class ThuocTinh
{
    public int ThuocTinhId { get; set; }

    public string TenThuocTinh { get; set; } = null!;

    public virtual ICollection<DanhMucThuocTinh> DanhMucThuocTinhs { get; set; } = new List<DanhMucThuocTinh>();

    public virtual ICollection<GiaTriThuocTinh> GiaTriThuocTinhs { get; set; } = new List<GiaTriThuocTinh>();
}
