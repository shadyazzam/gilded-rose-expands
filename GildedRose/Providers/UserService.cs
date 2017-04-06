using GildedRose.Logic;
using Microsoft.AspNet.Identity;
using System.Web;
using System.Web.Http;

namespace GildedRose.Providers
{
    public class UserService : ApiController, IUserService
    {
        /// <summary>
        /// Gets the current user's Id.
        /// </summary>
        /// <returns>The Id of the current user.</returns>
        public string GetCurrentUserId()
        {
            return User.Identity.GetUserId();
        }
    }
}
