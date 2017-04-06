using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GildedRose.Tests.Integration
{
    /// <summary>
    /// This is used to create a random database connection string for integration testing.
    /// </summary>
    public static class ConnectionStringBuilder
    {
        //private const string _customizedConnectionString = @"data source=localhost\SQLEXPRESS;initial catalog=GildedRose{0};user id=sa;password=!Password1";

        public static string GetConnectionString()
        {
            return string.Format(System.Configuration.ConfigurationManager.ConnectionStrings["Test"].ConnectionString, Guid.NewGuid());
            //return string.Format(_customizedConnectionString, Guid.NewGuid());
        }
    }
}
