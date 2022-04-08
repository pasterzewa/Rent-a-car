using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rent_a_Car.Data;

namespace Rent_a_Car.Models
{
    public class CarDetalisController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarDetalisController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CarDetails/0
        public async Task<IActionResult> Index(string sortOrder)
        {
            var applicationDbContext = _context.CarDetails.Include(c => c.Car).Include(c => c.Company);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: CarDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var CarDetails = await _context.CarDetails
                .Include(c => c.Car)
                .Include(c => c.Company)
                .FirstOrDefaultAsync(m => m.CarDetailsID == id);
            if (CarDetails == null)
            {
                return NotFound();
            }

            return View(CarDetails);
        }


        // GET: CarDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var CarDetails = await _context.CarDetails.FindAsync(id);
            if (CarDetails == null)
            {
                return NotFound();
            }
            ViewData["CarID"] = new SelectList(_context.Car, "CarID", "CarID", CarDetails.CarID);
            ViewData["CompanyID"] = new SelectList(_context.Company, "CompanyID", "CompanyID", CarDetails.CompanyID);
            return View(CarDetails);
        }

        // POST: CarDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CarDetailsID,CarID,CompanyID,Price,YearOfProduction,Description,IsAvailable")] CarDetails CarDetails)
        {
            if (id != CarDetails.CarDetailsID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(CarDetails);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarDetalisExists(CarDetails.CarDetailsID))
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
            ViewData["CarID"] = new SelectList(_context.Car, "CarID", "CarID", CarDetails.CarID);
            ViewData["CompanyID"] = new SelectList(_context.Company, "CompanyID", "CompanyID", CarDetails.CompanyID);
            return View(CarDetails);
        }

        private bool CarDetalisExists(int id)
        {
            return _context.CarDetails.Any(e => e.CarDetailsID == id);
        }

        // GET: CarDetails/ShowAllCars
        //[HttpPost]
        public async Task<IActionResult> ShowAllCars(int? id)
        {
            var dbContext = _context.CarDetails.Include(c => c.Car).Include(c => c.Company);

            if (id == null)
            {
                return NotFound();
            }
            return View("Index",
                await dbContext.Where(x => x.CarID.Equals(id)).ToListAsync());
        }
    }
}
