using Biblioteka.Data;
using Biblioteka.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization; 

namespace Biblioteka.Controllers
{
    [Authorize]
    public class BooksController : Controller
    {
        private readonly LibraryContext _context;

        public BooksController(LibraryContext context)
        {
            _context = context;
        }

        // GET: /Books  (z filtrowaniem)
        [AllowAnonymous]
        public async Task<IActionResult> Index(int? authorId, int? genreId, string? searchTitle)
        {
            var books = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genres)
                .AsQueryable();

            if (authorId.HasValue)
                books = books.Where(b => b.AuthorId == authorId.Value);

            if (genreId.HasValue)
                books = books.Where(b => b.Genres.Any(g => g.Id == genreId.Value));

            if (!string.IsNullOrWhiteSpace(searchTitle))
                books = books.Where(b => b.Title.Contains(searchTitle));

            ViewBag.Authors = new SelectList(
                _context.Authors.OrderBy(a => a.LastName),
                "Id", "FullName", authorId);

            ViewBag.Genres = new SelectList(
                _context.Genres.OrderBy(g => g.Name),
                "Id", "Name", genreId);

            ViewBag.SearchTitle = searchTitle;
            ViewBag.SelectedAuthorId = authorId;
            ViewBag.SelectedGenreId = genreId;

            return View(await books.OrderBy(b => b.Title).ToListAsync());
        }

        // GET: /Books/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genres)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null) return NotFound();

            return View(book);
        }

        // GET: /Books/Create
        public IActionResult Create()
        {
            ViewBag.AuthorList = new SelectList(
                _context.Authors.OrderBy(a => a.LastName),
                "Id", "FullName");

            ViewBag.AllGenres = _context.Genres.OrderBy(g => g.Name).ToList();
            return View();
        }

        // POST: /Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Title,ISBN,PublishedYear,AuthorId")] Book book,
            int[]? selectedGenres)
        {
            if (ModelState.IsValid)
            {
                if (selectedGenres != null && selectedGenres.Length > 0)
                {
                    book.Genres = await _context.Genres
                        .Where(g => selectedGenres.Contains(g.Id))
                        .ToListAsync();
                }

                _context.Books.Add(book);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Książka została dodana.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.AuthorList = new SelectList(
                _context.Authors.OrderBy(a => a.LastName),
                "Id", "FullName", book.AuthorId);

            ViewBag.AllGenres = _context.Genres.OrderBy(g => g.Name).ToList();
            return View(book);
        }

        // GET: /Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books
                .Include(b => b.Genres)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null) return NotFound();

            ViewBag.AuthorList = new SelectList(
                _context.Authors.OrderBy(a => a.LastName),
                "Id", "FullName", book.AuthorId);

            ViewBag.AllGenres = _context.Genres.OrderBy(g => g.Name).ToList();
            ViewBag.SelectedGenreIds = book.Genres.Select(g => g.Id).ToList();
            return View(book);
        }

        // POST: /Books/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Title,ISBN,PublishedYear,AuthorId")] Book book,
            int[]? selectedGenres)
        {
            if (id != book.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var existingBook = await _context.Books
                    .Include(b => b.Genres)
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (existingBook == null) return NotFound();

                existingBook.Title = book.Title;
                existingBook.ISBN = book.ISBN;
                existingBook.PublishedYear = book.PublishedYear;
                existingBook.AuthorId = book.AuthorId;

                existingBook.Genres.Clear();

                if (selectedGenres != null && selectedGenres.Length > 0)
                {
                    existingBook.Genres = await _context.Genres
                        .Where(g => selectedGenres.Contains(g.Id))
                        .ToListAsync();
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "Książka została zaktualizowana.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.AuthorList = new SelectList(
                _context.Authors.OrderBy(a => a.LastName),
                "Id", "FullName", book.AuthorId);

            ViewBag.AllGenres = _context.Genres.OrderBy(g => g.Name).ToList();
            ViewBag.SelectedGenreIds = selectedGenres?.ToList() ?? new List<int>();
            return View(book);
        }

        // GET: /Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genres)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null) return NotFound();

            return View(book);
        }

        // POST: /Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books
                .Include(b => b.Genres)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book != null)
            {
                book.Genres.Clear();
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Książka została usunięta.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}