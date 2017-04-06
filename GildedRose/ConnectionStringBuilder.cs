using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace GildedRose
{
    public class ConnectionStringBuilder
    {
        private string _connectionString;
        public string ConnectionString
        {
            get { return _connectionString ?? (_connectionString = GetConnectionStringFromWebConfig()); }
            set { _connectionString = value; }
        }

        private string GetConnectionStringFromWebConfig()
        {
            return ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }
    }
}