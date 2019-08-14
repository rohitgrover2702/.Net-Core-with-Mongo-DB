using System;
using System.Collections.Generic;
using System.Text;

namespace Kobe.Data.DBMapping
{
    public interface IBookstoreDatabaseSettings
    {
        string BooksCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
