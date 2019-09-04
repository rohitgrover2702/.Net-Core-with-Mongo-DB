using System;
using System.Collections.Generic;
using System.Text;

namespace Kobe.Data.DBMapping
{
    public class DBSettings : IDBSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public class AppSettings
    {
        public string Secret { get; set; }
    }
}
