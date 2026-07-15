using ELMS.Commons.Constants;
using ELMS.Commons.Enums;
using ELMS.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Session;
using Microsoft.IdentityModel.Tokens;

namespace ELMS.Web.Filters
{
    public class SessionAuthorizeAttribute : AuthorizeFilter, IAuthorizationFilter
    {
        private readonly Roles _roles;
        public SessionAuthorizeAttribute(Roles roles)
        {
            _roles = roles;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                long UserId = SessionManager.Get<Int64>(context.HttpContext, Sessioncnt.UserId);
                if (UserId <= 0) context.Result = new RedirectToActionResult("Login", "Login", null);

                if (_roles > 0)
                {
                    string? role = SessionManager.Get<string>(
                        context.HttpContext,
                        Sessioncnt.Role);

                    if (!_roles.Equals(role))
                    {
                        context.Result = new RedirectToActionResult(
                            "AccessDenied",
                            "Login",
                            null);
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
