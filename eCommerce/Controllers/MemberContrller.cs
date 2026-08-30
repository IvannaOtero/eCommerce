using System.Linq;
using Microsoft.EntityFrameworkCore;
using eCommerce.Data;
using eCommerce.Models;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.Controllers;

public class MemberContrller : Controller
{
    private readonly ProductDbContext _context;

    public MemberContrller(ProductDbContext context)
    {
        _context = context;
    }

    public IActionResult Register()
    {
        return View(); 
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegistrationViewModel reg)
    {
        if (ModelState.IsValid)
        {
            // Check if username or email is already taken
            bool usernamteTaken = await _context.Members
                                 .AnyAsync(m => m.Username == reg.Username); 

            if (usernamteTaken)
            {
                ModelState.AddModelError(nameof(Member.Username), "Username already taken");
                return View(reg); 
            }

            bool emailTaken = await _context.Members
                              .AnyAsync(m => m.Email == reg.Email);

            if (emailTaken)
            {
                ModelState.AddModelError(nameof(Member.Email), "Email already taken");
                return View(reg);
            }
            Member newMember = new()
            {
                Username = reg.Username,
                Email = reg.Email,
                Password = reg.Password,
                DateOfBirth = reg.DateOfBirth,
            };

            _context.Members.Add(newMember); 
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");

        }

        return View(reg);
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel login)
    {
        if (ModelState.IsValid)
        {
            // Check if UsernameOrEmail and Password matches in the database
            var loggedInMember = await _context.Members
                                    .Where(m => (m.Username == login.UsernameOrEmail || m.Email == login.UsernameOrEmail)
                                    && m.Password == login.Password)
                                    .Select(m => new {m.Username, m.MemberId})
                                    .SingleOrDefaultAsync();

            if (loggedInMember == null)
            {
                ModelState.AddModelError(string.Empty, "Your provided credential do not match any records in our database");
                return View(login); 
            }

            // Log the user in???
            HttpContext.Session.SetString("Username", loggedInMember.Username);
            HttpContext.Session.SetInt32("Id", loggedInMember.MemberId); 

            return RedirectToAction("Index", "Home");
        }

        return View(login); 
    }

    public IActionResult Logout()
    {
        // Destroy current session
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}
