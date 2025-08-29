using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace WebDienTu.Conventions
{
    /// <summary>
    /// Quy định: mọi Controller trong Area = "Admin" đều yêu cầu Role = "Admin"
    /// </summary>
    public class AdminAreaAuthorization : IControllerModelConvention
    {
        private readonly string _area;
        private readonly string _role;

        public AdminAreaAuthorization(string area, string role)
        {
            _area = area;
            _role = role;
        }

        public void Apply(ControllerModel controller)
        {
            var hasArea = controller.Attributes
                .OfType<AreaAttribute>()
                .Any(a => a.RouteValue.Equals(_area, StringComparison.OrdinalIgnoreCase));

            if (hasArea)
            {
                controller.Filters.Add(new AuthorizeFilter(new AuthorizationPolicyBuilder()
                    .RequireRole(_role)
                    .Build()));
            }
        }
    }
}
