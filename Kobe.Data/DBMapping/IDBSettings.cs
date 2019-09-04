using System;
using System.Collections.Generic;
using System.Text;

namespace Kobe.Data.DBMapping
{
    public interface IDBSettings
    {
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
