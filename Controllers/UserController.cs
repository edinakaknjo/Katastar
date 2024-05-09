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
    [Authorize(Roles = "Sluzbenik,Admin")]
    [Route("User")]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(ApplicationDbContext context, ILogger<UserController> logger, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

		// GET: AdminController

		[HttpGet]
		public IActionResult Index()
		{
			var sluzbenikId = _userManager.GetUserId(User);
			var predmeti = GetTasksForSluzbenik(sluzbenikId);
			return View(predmeti);
		}

		public List<Predmet> GetTasksForSluzbenik(string sluzbenikId)
        {
            return _context.Predmeti.Where(t => t.SluzbenikId == sluzbenikId && !t.Zavrsen).ToList();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> Details(int id)
        {
            var predmet = _context.Predmeti.Find(id);
            predmet.Sluzbenik = await _userManager.FindByIdAsync(predmet.SluzbenikId);
            if (predmet == null)
            {
                return NotFound();
            }
            return View(predmet);
        }
		[HttpGet("[action]")]
		public IActionResult Edit(int id)
        {
            var predmet = _context.Predmeti.Find(id);
            if (predmet == null)
            {
                return NotFound();
            }
            return View(predmet);
        }

        [HttpPost("[action]")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Predmet predmetModel)
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

                    //predmet.Ime = predmetModel.Ime;
                    predmet.Vlasnik = predmetModel.Vlasnik;
                    predmet.Lokacija = predmetModel.Lokacija;
                    predmet.Dimenzija = predmetModel.Dimenzija;
                    predmet.Zavrsen = predmetModel.Zavrsen;
                    predmet.Deskripcija = predmetModel.Deskripcija;

                    _context.SaveChanges();

                    return RedirectToAction(nameof(Index));
                }
				foreach (var key in ModelState.Keys)
				{
					var modelStateEntry = ModelState[key];
					foreach (var error in modelStateEntry.Errors)
					{
						Debug.WriteLine($"ModelState Error for key '{key}': {error.ErrorMessage}");
					}
				}

				return View(predmetModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greska tokom uredjivanja predmeta");
                throw; 
            }
        }
    }

}


