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
    public class GuestController : Controller
    {
        
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public GuestController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            
            _userManager = userManager;
        }

        // GET: AdminController

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var predmet = _context.Arhiva.Find(id);
            if (predmet == null)
            {
                return View("Error");
            }
            predmet.Sluzbenik = await _userManager.FindByIdAsync(predmet.SluzbenikId);
            return View(predmet);
        }

    }

}


