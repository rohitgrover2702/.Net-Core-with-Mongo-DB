using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kobe.Domain.Entities;
using Kobe.Service.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kobe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;
        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }
        [HttpGet]
        public ActionResult<List<Book>> Get()
        {
            return _bookService.GetallBooks();
        }


    }
}