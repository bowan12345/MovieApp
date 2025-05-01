using Microsoft.AspNetCore.Mvc;
using MovieApp.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MovieApp.DataAccess.Data;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace MovieAppWeb.Areas.Admin.Controllers
{
    [Area("Customer")]
    public class MovieController : Controller
        {
            private readonly ApplicationDbContext _context;

            public MovieController(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<IActionResult> Index()
            {
                var movies = await _context.Movies.Include(m => m.Category).ToListAsync();
                return View(movies);
            }

            public async Task<IActionResult> Details(int? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var movie = await _context.Movies
                    .Include(m => m.Category)
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (movie == null)
                {
                    return NotFound();
                }

                return View(movie);
            }

            public IActionResult Create()
            {
                ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
                return View();
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create([Bind("Id,Title,Description,ReleaseYear,Director,ImageUrl,CategoryId")] Movie movie)
            {
                if (ModelState.IsValid)
                {
                    _context.Add(movie);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", movie.CategoryId);
                return View(movie);
            }

            public async Task<IActionResult> Edit(int? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var movie = await _context.Movies.FindAsync(id);
                if (movie == null)
                {
                    return NotFound();
                }
                ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", movie.CategoryId);
                return View(movie);
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,ReleaseYear,Director,ImageUrl,CategoryId")] Movie movie)
            {
                if (id != movie.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(movie);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!MovieExists(movie.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }
                ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", movie.CategoryId);
                return View(movie);
            }
     
            public async Task<IActionResult> Delete(int? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var movie = await _context.Movies
                    .Include(m => m.Category)
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (movie == null)
                {
                    return NotFound();
                }

                return View(movie);
            }

            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> DeleteConfirmed(int id)
            {
                var movie = await _context.Movies.FindAsync(id);
                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            private bool MovieExists(int id)
            {
                return _context.Movies.Any(e => e.Id == id);
            }
        }
}


