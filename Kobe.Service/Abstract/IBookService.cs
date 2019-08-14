using Kobe.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kobe.Service.Abstract
{
    public interface IBookService
    {
        List<Book> GetallBooks();
    }
}
