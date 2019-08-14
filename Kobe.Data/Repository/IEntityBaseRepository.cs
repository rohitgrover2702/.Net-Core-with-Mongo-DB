using System;
using System.Collections.Generic;
using System.Text;

namespace Kobe.Data.Repository
{
   public interface IEntityBaseRepository<T> where T: class
    {
        List<T> GetAllNew();
    }
}
