using System;
using System.Collections.Generic;

namespace WebDienTu.Models;

public partial class GiaTriThuocTinh
{
    public int GiaTriId { get; set; }

    public int MaSanPham { get; set; }

    public int ThuocTinhId { get; set; }

    public string GiaTri { get; set; } = null!;

    public virtual SanPham MaSanPhamNavigation { get; set; } = null!;

    public virtual ThuocTinh ThuocTinh { get; set; } = null!;
}
