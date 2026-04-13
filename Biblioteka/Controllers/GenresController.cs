using Biblioteka.Data;
using Biblioteka.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Biblioteka.Controllers
{
    public class GenresController : Controller
    {
        private readonly LibraryContext _context;

        public GenresController(LibraryContext context)
        {
            _context = context;
        }

        // GET: /Genres
        public async Task<IActionResult> Index()
        {
            var genres = await _context.Genres
                .Include(g => g.Books)
                .OrderBy(g => g.Name)
                .ToListAsync();
            return View(genres);
        }

        // GET: /Genres/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Genres/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Genre genre)
        {
            if (ModelState.IsValid)
            {
                _context.Genres.Add(genre);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Gatunek został dodany.";
                return RedirectToAction(nameof(Index));
            }
            return View(genre);
        }

        // GET: /Genres/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var genre = await _context.Genres.FindAsync(id);
            if (genre == null) return NotFound();

            return View(genre);
        }

        // POST: /Genres/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Genre genre)
        {
            if (id != genre.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Genres.Update(genre);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Gatunek został zaktualizowany.";
                return RedirectToAction(nameof(Index));
            }
            return View(genre);
        }

        // GET: /Genres/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var genre = await _context.Genres
                .Include(g => g.Books)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (genre == null) return NotFound();

            return View(genre);
        }

        // POST: /Genres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var genre = await _context.Genres
                .Include(g => g.Books)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (genre == null) return NotFound();

            if (genre.Books.Any())
            {
                TempData["Error"] = "Nie można usunąć gatunku przypisanego do książek.";
                return RedirectToAction(nameof(Index));
            }

            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Gatunek został usunięty.";
            return RedirectToAction(nameof(Index));
        }
    }
}