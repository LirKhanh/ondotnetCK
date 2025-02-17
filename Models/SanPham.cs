using System;
using System.Collections.Generic;

namespace ontaptx2.Models;

public partial class SanPham
{
    public string MaSp { get; set; } = null!;

    public string? TenSp { get; set; }

    public string? MaLoai { get; set; }

    public int? DonGia { get; set; }

    public int? SoLuong { get; set; }

    public virtual LoaiSanPham? MaLoaiNavigation { get; set; }
}
