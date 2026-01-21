using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using HomeLib.Data;
using HomeLib.Models;

namespace HomeLib.Controllers
{
    public class BooksController : Controller
    {
        static readonly string[] Languages =
        [
            "Русский", "Английский", "Французский", "Немецкий",
            "Испанский", "Итальянский", "Китайский", "Японский"
        ];

        static readonly string[] Genres =
        [
            "Роман", "Детектив", "Фантастика", "Фэнтези", 
            "Научная литература", "Биография", "Поэзия", "Драма",
            "Приключения", "Исторический", "Ужасы", "Комедия",
            "Триллер", "Мистика", "Научпоп", "Справочник"
        ];

        private readonly IBookRepository _bookRepository;

        public BooksController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            var books = await _bookRepository.GetAllBooksAsync();
            return View(books);
        }

        // GET: Books/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            var book = await _bookRepository.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            ViewBag.Languages = new SelectList(Languages);

            ViewBag.Genres = new SelectList(Genres);

            ViewBag.CurrentYear = DateTime.Now.Year;

            return View();
        }

        // POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book)
        {
            if (book.PublishYear > DateTime.Now.Year)
            {
                ModelState.AddModelError("PublishYear", "Год издания не может быть больше текущего года");
            }

            if (ModelState.IsValid)
            {
                var newBookId = await _bookRepository.CreateBookAsync(book);
                if (newBookId > 0)
                {
                    return RedirectToAction(nameof(Details), new { id = newBookId });
                }
                else
                {
                    ModelState.AddModelError("", "Не удалось создать книгу. Попробуйте еще раз.");
                }
            }

            ViewBag.Languages = new SelectList(Languages, book.Language);

            ViewBag.Genres = new SelectList(Genres, book.Genre);

            ViewBag.CurrentYear = DateTime.Now.Year;

            return View(book);
        }

        // GET: Books/Edit/{id}
        public async Task<IActionResult> Edit(int id)
        {
            var book = await _bookRepository.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            ViewBag.Languages = new SelectList(Languages, book.Language);

            ViewBag.Genres = new SelectList(Genres, book.Genre);

            ViewBag.CurrentYear = DateTime.Now.Year;

            return View(book);
        }

        // POST: Books/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Book book)
        {
            if (id != book.BookID)
            {
                return NotFound();
            }

            if (book.PublishYear > DateTime.Now.Year)
            {
                ModelState.AddModelError("PublishYear", "Год издания не может быть больше текущего года");
            }

            if (ModelState.IsValid)
            {
                var result = await _bookRepository.UpdateBookAsync(book);
                if (result)
                {
                    return RedirectToAction(nameof(Details), new { id = book.BookID });
                }
                else
                {
                    ModelState.AddModelError("", "Не удалось обновить книгу.");
                }
            }

            ViewBag.Languages = new SelectList(Languages, book.Language);

            ViewBag.Genres = new SelectList(Genres, book.Genre);

            ViewBag.CurrentYear = DateTime.Now.Year;

            return View(book);
        }

        // GET: Books/Delete/{id}}
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _bookRepository.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _bookRepository.DeleteBookAsync(id);
            if (result)
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Delete), new { id, error = true });
        }


        public async Task<IActionResult> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return RedirectToAction(nameof(Index));
            }

            var allBooks = await _bookRepository.GetAllBooksAsync();
            var filteredBooks = allBooks.Where(b =>
                (b.Title?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (b.Author?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (b.Genre?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (b.Publisher?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
            ).ToList();

            ViewBag.SearchTerm = searchTerm;
            return View("Index", filteredBooks);
        }
    }
}