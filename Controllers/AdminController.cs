using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Katastar.Data;
using Katastar.Models;

namespace Katastar.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context, ILogger<AdminController> logger, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        // GET: AdminController
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        // GET: AdminController/Create
        public IActionResult Create()
        {
            ViewData["Id"] = new SelectList(_userManager.Users, "Id", "Id");
            return View();
        }

        // POST: Admin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id, FirstName,LastName,Email,PhoneNumber")] ApplicationUser scaffold)
        {
            var sluzbenici = await _userManager.GetUsersInRoleAsync("Sluzbenik");

            ViewBag.Sluzbenici = new SelectList(sluzbenici, nameof(ApplicationUser.Id), nameof(ApplicationUser.UserName));

            Console.WriteLine($"Number of sluzbenici retrieved: {sluzbenici.Count}");
            Debug.WriteLine($"Number of sluzbenici retrieved: {sluzbenici.Count}");
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    FirstName = scaffold.FirstName,
                    LastName = scaffold.LastName,
                    Email = scaffold.Email,
                    PhoneNumber = scaffold.PhoneNumber,
                    UserName = scaffold.Email,
                    
                };
                string password = "Sluzbenik123_";

                await _userManager.CreateAsync(user, password);
                await _userManager.AddToRoleAsync(user, "Sluzbenik");
            }
            //}


            ViewData["Id"] = new SelectList(_userManager.Users, "Id", "Id", scaffold.Id);
            return RedirectToAction(nameof(Index));
            //return View();
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id, FirstName, LastName, Email, PhoneNumber")] ApplicationUser updatedUser)
        {
            if (id != updatedUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(id);

                if (user == null)
                {
                    return NotFound();
                }

                user.FirstName = updatedUser.FirstName;
                user.LastName = updatedUser.LastName;
                user.Email = updatedUser.Email;
                user.PhoneNumber = updatedUser.PhoneNumber;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(updatedUser);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(user);
        }
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }


        [HttpGet]
        public async Task<IActionResult> CreatePredmet()
        {
            var sluzbenici = await _userManager.GetUsersInRoleAsync("Sluzbenik");
            ViewBag.Sluzbenici = new SelectList(sluzbenici, "Id", "Email");

            //ViewBag.Sluzbenici = new SelectList(sluzbenici, nameof(ApplicationUser.Id), nameof(ApplicationUser.UserName));
            ViewBag.SelectedSluzbenik = "";
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Predmet()
        {
            var predmeti = await _context.Predmeti.ToListAsync();

            return View(predmeti);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePredmet(Predmet predmet)
        {
            if (ModelState.IsValid)
            {
                var sluzbenici = await _userManager.GetUsersInRoleAsync("Sluzbenik");
                ViewBag.Sluzbenici = new SelectList(sluzbenici, "Id", "Email");
                
                //var selectedSluzbenik = await _userManager.FindByIdAsync(predmet.SluzbenikId);
                //ViewBag.SelectedSluzbenik = selectedSluzbenik != null ? $"{selectedSluzbenik.FirstName} {selectedSluzbenik.LastName}" : "";
             
                var newPredmet = new Predmet
                {
                    Id = predmet.Id,
                    Ime = predmet.Ime,
                    Vlasnik = predmet.Vlasnik,
                    Lokacija = predmet.Lokacija,
                    Dimenzija = predmet.Dimenzija,
                    Zavrsen = predmet.Zavrsen,
                    Deskripcija = predmet.Deskripcija,
                    SluzbenikId = predmet.SluzbenikId, 
                };

                _context.Predmeti.Add(newPredmet);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Predmet));
            }

            return View(predmet);
        }


        [HttpGet]
        public async Task<IActionResult> EditPredmet(int id)
        {
            var sluzbenici = await _userManager.GetUsersInRoleAsync("Sluzbenik");
            ViewBag.Sluzbenici = new SelectList(sluzbenici, "Id", "Email");

            //ViewBag.Sluzbenici = new SelectList(sluzbenici, nameof(ApplicationUser.Id), nameof(ApplicationUser.UserName));
            ViewBag.SelectedSluzbenik = "";
            var predmet = _context.Predmeti.Find(id);
            if (predmet == null)
            {
                return NotFound();
            }
            return View(predmet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditPredmet(int id, Predmet predmetModel)
        {
            try
            {
                if (id != predmetModel.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    var predmet = _context.Predmeti.Find(id);

                    if (predmet == null)
                    {
                        return NotFound();
                    }

                    predmet.Ime = predmetModel.Ime;
                    predmet.Vlasnik = predmetModel.Vlasnik;
                    predmet.Lokacija = predmetModel.Lokacija;
                    predmet.Dimenzija = predmetModel.Dimenzija;
                    predmet.Zavrsen = predmetModel.Zavrsen;
                    predmet.Deskripcija = predmetModel.Deskripcija;
                    predmet.SluzbenikId = predmetModel.SluzbenikId;


                    _context.SaveChanges();

                    return RedirectToAction(nameof(Predmet));
                }

                return View(predmetModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while editing the predmet.");
                throw; // Rethrow the exception for further analysis
            }
        }

        [HttpGet]
        public IActionResult DeletePredmet(int id)
        {
            var predmet = _context.Predmeti.Find(id);
            if (predmet == null)
            {
                return NotFound();
            }
            return View(predmet);
        }

        [HttpPost, ActionName("DeletePredmet")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmedPredmet(int id)
        {
            var predmet = _context.Predmeti.Find(id);
            if (predmet == null)
            {
                return NotFound();
            }
            _context.Predmeti.Remove(predmet);
            _context.SaveChanges();
            return RedirectToAction(nameof(Predmet));
        }

        [HttpGet]
        public async Task<IActionResult> DetailsPredmet(int id)
        {
            var predmet = _context.Predmeti.Find(id);
            predmet.Sluzbenik = await _userManager.FindByIdAsync(predmet.SluzbenikId);
            if (predmet == null)
            {
                return NotFound();
            }
            return View(predmet);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MoveToArhiva(int id)
        {
            try
            {
                var task = _context.Predmeti.FirstOrDefault(p => p.Id == id && p.Zavrsen);

                if (task == null)
                {
                    return NotFound();
                }
                
                    var arhivaEntry = new Arhiva
                    {
                        Id = task.Id,
                        SluzbenikId = task.SluzbenikId,
                        DatumDodavanja = DateTime.Now,
                        Ime = task.Ime,
                        Vlasnik = task.Vlasnik,
                        Lokacija = task.Lokacija,
                        Dimenzija = task.Dimenzija,
                        Deskripcija = task.Deskripcija
                    };

                    _context.Arhiva.Add(arhivaEntry);
                    _context.Predmeti.Remove(task);
                
                foreach (var key in ModelState.Keys)
                {
                    var modelStateEntry = ModelState[key];
                    foreach (var error in modelStateEntry.Errors)
                    {
                        Debug.WriteLine($"ModelState Error for key '{key}': {error.ErrorMessage}");
                    }
                }

                _context.SaveChanges();

                return RedirectToAction(nameof(Arhiva));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while moving completed tasks to archive.");
                // Handle the error appropriately
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Arhiva()
        {
            var arhivirani_predmeti = await _context.Arhiva.ToListAsync();

            return View(arhivirani_predmeti);
        }

        [HttpGet]
        public async Task<IActionResult> DetailsArhiva(int id)
        {
            var predmet = _context.Arhiva.Find(id);
            predmet.Sluzbenik = await _userManager.FindByIdAsync(predmet.SluzbenikId);
            if (predmet == null)
            {
                return NotFound();
            }
            return View(predmet);
        }



    }


}


