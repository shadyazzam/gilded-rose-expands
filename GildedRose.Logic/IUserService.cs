using GildedRose.Entities;
using System;
using System.Collections.Generic;

namespace GildedRose.Logic
{
    public interface IUserService
    {
        /// <summary>
        /// Gets the identifier of the current user.
        /// </summary>
        /// <returns></returns>
        string GetCurrentUserId();
    }
}
