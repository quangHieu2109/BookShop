using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

public class AuthorizeRoleFilter : IAuthorizationFilter
{
    private readonly string[] _allowedRoles;

    public AuthorizeRoleFilter(string[] allowedRoles)
    {
        _allowedRoles = allowedRoles;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Lấy thông tin về vai trò từ JWT
        var role = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "role")?.Value;

        // Kiểm tra nếu vai trò của người dùng được phép truy cập
        if (role == null || !_allowedRoles.Contains(role))
        {
            context.Result = new ForbidResult();
        }
    }
}