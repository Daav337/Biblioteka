using Biblioteka.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Biblioteka.Controllers
{
    public class HomeController : Controller
    {
        private readonly LibraryContext _context;

        public HomeController(LibraryContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.BookCount = await _context.Books.CountAsync();
            ViewBag.AuthorCount = await _context.Authors.CountAsync();
            ViewBag.GenreCount = await _context.Genres.CountAsync();

            var recentBooks = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genres)
                .OrderByDescending(b => b.Id)
                .Take(5)
                .ToListAsync();

            return View(recentBooks);
        }
    }
}