using Biblioteka.Data;
using Biblioteka.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Biblioteka.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly LibraryContext _context;

        public AuthorsController(LibraryContext context)
        {
            _context = context;
        }

        // GET: /Authors
        public async Task<IActionResult> Index(string? searchName)
        {
            var authors = _context.Authors.Include(a => a.Books).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchName))
                authors = authors.Where(a =>
                    a.FirstName.Contains(searchName) ||
                    a.LastName.Contains(searchName));

            ViewBag.SearchName = searchName;
            return View(await authors.OrderBy(a => a.LastName).ToListAsync());
        }

        // GET: /Authors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var author = await _context.Authors
                .Include(a => a.Books)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (author == null) return NotFound();

            return View(author);
        }

        // GET: /Authors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Authors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Author author)
        {
            if (ModelState.IsValid)
            {
                _context.Authors.Add(author);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Autor został dodany.";
                return RedirectToAction(nameof(Index));
            }
            return View(author);
        }

        // GET: /Authors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var author = await _context.Authors.FindAsync(id);
            if (author == null) return NotFound();

            return View(author);
        }

        // POST: /Authors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Author author)
        {
            if (id != author.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Authors.Update(author);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Dane autora zostały zaktualizowane.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Authors.Any(a => a.Id == author.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(author);
        }

        // GET: /Authors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var author = await _context.Authors
                .Include(a => a.Books)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (author == null) return NotFound();

            return View(author);
        }

        // POST: /Authors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var author = await _context.Authors
                .Include(a => a.Books)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (author == null) return NotFound();

            if (author.Books.Any())
            {
                TempData["Error"] = "Nie można usunąć autora, który ma przypisane książki.";
                return RedirectToAction(nameof(Index));
            }

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Autor został usunięty.";
            return RedirectToAction(nameof(Index));
        }
    }
}