using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lesson52;
using Lesson52.Models;
using Lesson52.ViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Xml.Linq;

namespace Lesson52.Controllers
{

    public enum PhoneSortState
    {
        NameAsc,
        NameDesc,
        PriceAsc,
        PriceDesc,
        CompanyAsc,
        CompanyDesc,
        ReviewAsc,
        ReviewDesc
    }
    public class PhonesController : Controller
    {
        private readonly PhoneShopContext _context;

        public PhonesController(PhoneShopContext context)
        {
            _context = context;
        }

        // GET: Phones
        public async Task<IActionResult> Index(int? company, string name, int? priceFrom, int? priceTo, PhoneSortState sortOrder = PhoneSortState.NameAsc, int page = 1)
        {
            IQueryable<Phone> phones = _context.Phones.Include(u => u.Company)
                                                         .Include(x => x.Review);

            if (company != null && company != 0)
            {
                phones = phones.Where(p => p.CompanyId == company);
            }
            if(priceFrom != null && priceFrom != 0)
            {
                phones = phones.Where(p =>p.Price >= priceFrom);
            }
            if (priceTo != null && priceTo != 0)
            {
                phones = phones.Where(p => p.Price <= priceTo);
            }
            if (!String.IsNullOrEmpty(name))
            {
                phones = phones.Where(p => p.Name.Contains(name));
            }


            ViewBag.NameSort = sortOrder == PhoneSortState.NameAsc ? PhoneSortState.NameDesc : PhoneSortState.NameAsc;
            ViewBag.PriceSort = sortOrder == PhoneSortState.PriceAsc ? PhoneSortState.PriceDesc : PhoneSortState.PriceAsc;
            ViewBag.CompanySort = sortOrder == PhoneSortState.CompanyAsc ? PhoneSortState.CompanyDesc : PhoneSortState.CompanyAsc;
            ViewBag.ReviewSort = sortOrder == PhoneSortState.ReviewAsc ? PhoneSortState.ReviewDesc : PhoneSortState.ReviewAsc;

            switch (sortOrder)
            {
                case PhoneSortState.NameDesc:
                    phones = phones.OrderByDescending(s => s.Name);
                    break;
                case PhoneSortState.PriceAsc:
                    phones = phones.OrderBy(s => s.Price);
                    break;
                case PhoneSortState.PriceDesc:
                    phones = phones.OrderByDescending(s => s.Price);
                    break;
                case PhoneSortState.ReviewAsc:
                    phones = phones.OrderBy(s => s.Review.Grade);
                    break;
                case PhoneSortState.ReviewDesc:
                    phones = phones.OrderByDescending(s => s.Review.Grade);
                    break;
                case PhoneSortState.CompanyAsc:
                    phones = phones.OrderBy(s => s.Company);
                    break;
                case PhoneSortState.CompanyDesc:
                    phones = phones.OrderByDescending(s => s.Company);
                    break;
                default:
                    phones = phones.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = 10;
            var count = await phones.CountAsync();
            PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);


            List<Company> companies = _context.Companies.ToList();
            companies.Insert(0, new Company { Name = "All", Id = 0 });

            PhoneListViewModel viewModel = new PhoneListViewModel
            {
                Phones = await phones.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(),
                Companies = new SelectList(companies, "Id", "Name"),
                PageViewModel = pageViewModel,
                Name = name,
                PriceFrom = priceFrom,
                PriceTo = priceTo
            };

            return View(viewModel);
        }

        // GET: Phones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phone = await _context.Phones
                .Include(p => p.Company)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (phone == null)
            {
                return NotFound();
            }

            return View(phone);
        }

        // GET: Phones/Create
        public IActionResult Create()
        {
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name");
            return View();
        }

        // POST: Phones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,CompanyId")] Phone phone)
        {
            if (ModelState.IsValid)
            {
                _context.Add(phone);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", phone.CompanyId);
            return View(phone);
        }

        // GET: Phones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phone = await _context.Phones.FindAsync(id);
            if (phone == null)
            {
                return NotFound();
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", phone.CompanyId);
            return View(phone);
        }

        // POST: Phones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,CompanyId")] Phone phone)
        {
            if (id != phone.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(phone);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhoneExists(phone.Id))
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
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", phone.CompanyId);
            return View(phone);
        }

        // GET: Phones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phone = await _context.Phones
                .Include(p => p.Company)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (phone == null)
            {
                return NotFound();
            }

            return View(phone);
        }

        // POST: Phones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var phone = await _context.Phones.FindAsync(id);
            _context.Phones.Remove(phone);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PhoneExists(int id)
        {
            return _context.Phones.Any(e => e.Id == id);
        }
    }
}
