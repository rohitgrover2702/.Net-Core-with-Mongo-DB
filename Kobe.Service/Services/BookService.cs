using Kobe.Data.Repository;
using Kobe.Domain.Entities;
using Kobe.Service.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kobe.Service.Services
{
    public class BookService : IBookService
    {
        private readonly IEntityBaseRepository<Book> _bookRepository;
        public BookService(IEntityBaseRepository<Book> bookRepository)
        {
            _bookRepository = bookRepository;
        }
        public List<Book> GetallBooks()
        {
           return _bookRepository.GetAllNew();
        }
    }
}
