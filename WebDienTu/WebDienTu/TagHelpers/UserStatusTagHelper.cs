using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Security.Claims;

namespace WebDienTu.TagHelpers
{
    [HtmlTargetElement("user-status")]
    public class UserStatusTagHelper : TagHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserStatusTagHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var user = _httpContextAccessor.HttpContext.User;

            output.TagName = "div"; // tag wrapper
            output.Attributes.SetAttribute("class", "navbar-nav");

            if (user.Identity.IsAuthenticated)
            {
                var name = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "Người dùng";
                var role = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "User";

                string links = $"<li class='nav-item'><span class='nav-link text-dark'>Xin chào, {name}</span></li>";

                // Nếu admin, thêm link quản trị
                if (role == "Admin")
                {
                    links += "<li class='nav-item'><a class='nav-link text-primary' href='/Admin/AdminHome/Index'>Quản trị</a></li>";
                }

                links += "<li class='nav-item'><a class='nav-link text-danger' href='/Account/Logout'>Đăng xuất</a></li>";

                output.Content.SetHtmlContent(links);
            }
            else
            {
                output.Content.SetHtmlContent("<li class='nav-item'><a class='nav-link' href='/Account/Login'>Đăng nhập</a></li>");
            }
        }
    }
}
