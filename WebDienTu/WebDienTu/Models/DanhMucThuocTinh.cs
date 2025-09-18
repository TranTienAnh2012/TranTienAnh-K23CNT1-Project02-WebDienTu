using System;
using System.Collections.Generic;

namespace WebDienTu.Models;

public partial class DanhMucThuocTinh
{
    public int Id { get; set; }

    public int MaDanhMuc { get; set; }

    public int ThuocTinhId { get; set; }

    public virtual DanhMuc MaDanhMucNavigation { get; set; } = null!;

    public virtual ThuocTinh ThuocTinh { get; set; } = null!;
}
