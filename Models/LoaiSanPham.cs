using System;
using System.Collections.Generic;

namespace ontaptx2.Models;

public partial class LoaiSanPham
{
    public string MaLoai { get; set; } = null!;

    public string? TenLoai { get; set; }

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
