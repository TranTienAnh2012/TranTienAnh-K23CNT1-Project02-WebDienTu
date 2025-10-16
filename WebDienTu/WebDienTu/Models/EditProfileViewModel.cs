using System.ComponentModel.DataAnnotations;

namespace WebDienTu.Models
{
    public class EditProfileViewModel
    {
        [Required(ErrorMessage = "Họ tên không được để trống!")]
        [StringLength(100)]
        public string HoTen { get; set; } = null!;

        [Required(ErrorMessage = "Email không được để trống!")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ!")]
        public string Email { get; set; } = null!;

        [DataType(DataType.Password)]
        public string MatKhauHienTai { get; set; } = null!;

        [DataType(DataType.Password)]
        public string MatKhauMoi { get; set; } = null!;

        [DataType(DataType.Password)]
        [Compare("MatKhauMoi", ErrorMessage = "Mật khẩu mới không khớp")]
        public string XacNhanMatKhauMoi { get; set; } = null!;
    }
}
