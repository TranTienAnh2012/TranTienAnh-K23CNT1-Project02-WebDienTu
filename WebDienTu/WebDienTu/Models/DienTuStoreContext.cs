using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebDienTu.Models;

public partial class DienTuStoreContext : DbContext
{
    public DienTuStoreContext()
    {
    }

    public DienTuStoreContext(DbContextOptions<DienTuStoreContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }

    public virtual DbSet<DanhGia> DanhGia { get; set; }

    public virtual DbSet<DanhMuc> DanhMucs { get; set; }

    public virtual DbSet<DonHang> DonHangs { get; set; }

    public virtual DbSet<GiamGia> GiamGia { get; set; }

    public virtual DbSet<GioHangTam> GioHangTams { get; set; }

    public virtual DbSet<QuanTriVien> QuanTriViens { get; set; }

    public virtual DbSet<SanPham> SanPhams { get; set; }

    public virtual DbSet<SanPhamDaXem> SanPhamDaXems { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=LAPTOP-F48PQH12\\TIENANH1;Database=DienTuStore;User Id=TtaAdmin;Password=tienanh2005;MultipleActiveResultSets=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChiTietDonHang>(entity =>
        {
            entity.HasKey(e => e.MaChiTiet).HasName("PK__ChiTietD__CDF0A11478DCEF27");

            entity.ToTable("ChiTietDonHang");

            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.MaDonHangNavigation).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.MaDonHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietDo__MaDon__48CFD27E");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.MaSanPham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietDo__MaSan__49C3F6B7");
        });

        modelBuilder.Entity<DanhGia>(entity =>
        {
            entity.HasKey(e => e.MaDanhGia).HasName("PK__DanhGia__AA9515BF58A59C8E");

            entity.Property(e => e.BinhLuan).HasMaxLength(500);
            entity.Property(e => e.NgayDanhGia)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.DanhGia)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DanhGia__MaNguoi__4F7CD00D");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.DanhGia)
                .HasForeignKey(d => d.MaSanPham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DanhGia__MaSanPh__4E88ABD4");
        });

        modelBuilder.Entity<DanhMuc>(entity =>
        {
            entity.HasKey(e => e.MaDanhMuc).HasName("PK__DanhMuc__B375088743C75394");

            entity.ToTable("DanhMuc");

            entity.Property(e => e.MoTa).HasMaxLength(250);
            entity.Property(e => e.TenDanhMuc).HasMaxLength(100);
        });

        modelBuilder.Entity<DonHang>(entity =>
        {
            entity.HasKey(e => e.MaDonHang).HasName("PK__DonHang__129584AD09655621");

            entity.ToTable("DonHang");

            entity.Property(e => e.NgayDatHang)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TongTien).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TrangThai).HasDefaultValue(false);

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DonHang__MaNguoi__45F365D3");
        });

        modelBuilder.Entity<GiamGia>(entity =>
        {
            entity.HasKey(e => e.MaKhuyenMai).HasName("PK__GiamGia__6F56B3BD2E46E3B9");

            entity.Property(e => e.GiaTri).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Loai).HasMaxLength(50);
            entity.Property(e => e.NgayBatDau).HasColumnType("datetime");
            entity.Property(e => e.NgayKetThuc).HasColumnType("datetime");
            entity.Property(e => e.TenChuongTrinh).HasMaxLength(200);
            entity.Property(e => e.TrangThai).HasDefaultValue(true);
        });

        modelBuilder.Entity<GioHangTam>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__GioHangT__3214EC07722C29E0");

            entity.ToTable("GioHangTam");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.GioHangTams)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GioHangTa__MaNgu__5DCAEF64");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.GioHangTams)
                .HasForeignKey(d => d.MaSanPham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GioHangTa__MaSan__5EBF139D");
        });

        modelBuilder.Entity<QuanTriVien>(entity =>
        {
            entity.HasKey(e => e.MaNguoiDung).HasName("PK__NguoiDun__C539D7626A37A462");

            entity.ToTable("QuanTriVien");

            entity.HasIndex(e => e.Email, "UQ__NguoiDun__A9D10534DBE729C6").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.MatKhau).HasMaxLength(100);
            entity.Property(e => e.NgayDangKy)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.VaiTro).HasDefaultValue(0);
        });

        modelBuilder.Entity<SanPham>(entity =>
        {
            entity.HasKey(e => e.MaSanPham).HasName("PK__SanPham__FAC7442D4AC22266");

            entity.ToTable("SanPham");

            entity.Property(e => e.BaoHanh).HasMaxLength(50);
            entity.Property(e => e.Gia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.GiaBan).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.HinhAnh).HasMaxLength(200);
            entity.Property(e => e.Loai).HasMaxLength(50);
            entity.Property(e => e.NgayThem)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SoLuongTon).HasDefaultValue(0);
            entity.Property(e => e.TenSanPham).HasMaxLength(150);
            entity.Property(e => e.ThuongHieu).HasMaxLength(100);
            entity.Property(e => e.TrangThai).HasDefaultValue(true);
            entity.Property(e => e.XuatXu).HasMaxLength(100);

            entity.HasOne(d => d.MaDanhMucNavigation).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.MaDanhMuc)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SanPham__MaDanhM__412EB0B6");

            entity.HasMany(d => d.MaKhuyenMais).WithMany(p => p.MaSanPhams)
                .UsingEntity<Dictionary<string, object>>(
                    "SanPhamKhuyenMai",
                    r => r.HasOne<GiamGia>().WithMany()
                        .HasForeignKey("MaKhuyenMai")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__SanPham_K__MaKhu__73BA3083"),
                    l => l.HasOne<SanPham>().WithMany()
                        .HasForeignKey("MaSanPham")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__SanPham_K__MaSan__72C60C4A"),
                    j =>
                    {
                        j.HasKey("MaSanPham", "MaKhuyenMai").HasName("PK__SanPham___3C322F164FF0DFAB");
                        j.ToTable("SanPham_KhuyenMai");
                    });
        });

        modelBuilder.Entity<SanPhamDaXem>(entity =>
        {
            entity.HasKey(e => e.MaDaXem).HasName("PK__SanPhamD__72FF565A9C18DEA6");

            entity.ToTable("SanPhamDaXem");

            entity.Property(e => e.ThoiGianXem)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.SanPhamDaXems)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SanPhamDaXem_NguoiDung");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.SanPhamDaXems)
                .HasForeignKey(d => d.MaSanPham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SanPhamDaXem_SanPham");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
