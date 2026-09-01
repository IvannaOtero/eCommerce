using System.Linq;
using Microsoft.EntityFrameworkCore;
using eCommerce.Data;
using eCommerce.Models;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.Controllers;

public class MemberController : Controller
{
    private readonly ProductDbContext _context;
    private readonly Microsoft.Extensions.Logging.ILogger<MemberController> _logger;

    public MemberController(ProductDbContext context, Microsoft.Extensions.Logging.ILogger<MemberController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public IActionResult Register()
    {
        return View(); 
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegistrationViewModel reg)
    {
        if (ModelState.IsValid)
        {
            // First check for duplicates; catch DB connectivity errors separately from SaveChanges unique constraint failures.
            try
            {
                bool usernameTaken = await _context.Members
                                     .AnyAsync(m => m.Username == reg.Username);

                if (usernameTaken)
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
            }
            catch (System.Exception ex)
            {
                _logger?.LogWarning(ex, "Database access failed when checking duplicates during registration.");
                ModelState.AddModelError(string.Empty, "Unable to access the database. Please try again later.");
                return View(reg);
            }

            var newMember = new Member
            {
                Username = reg.Username,
                Email = reg.Email,
                Password = reg.Password,
                DateOfBirth = reg.DateOfBirth,
            };

            _context.Members.Add(newMember);
            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                // Handle unique constraint violation specifically so user sees a helpful message.
                var msg = dbEx.InnerException?.Message ?? dbEx.Message;
                if (msg != null && (msg.Contains("UNIQUE constraint failed") || msg.Contains("Violation of UNIQUE KEY") || msg.Contains("Cannot insert duplicate key")))
                {
                    // Try to infer which field caused the duplicate error.
                    if (msg.Contains("Username") || msg.Contains("Members.Username"))
                        ModelState.AddModelError(nameof(Member.Username), "Username already taken");
                    else if (msg.Contains("Email") || msg.Contains("Members.Email"))
                        ModelState.AddModelError(nameof(Member.Email), "Email already taken");
                    else
                        ModelState.AddModelError(string.Empty, "A record with the same key already exists.");

                    return View(reg);
                }

                _logger?.LogError(dbEx, "Failed to save new member to the database.");
                ModelState.AddModelError(string.Empty, "Unable to save your account. Please try again later.");
                return View(reg);
            }
            catch (System.Exception ex)
            {
                _logger?.LogError(ex, "Unexpected error while creating a new member.");
                ModelState.AddModelError(string.Empty, "Unable to save your account. Please try again later.");
                return View(reg);
            }
        }

        return View(reg);
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View("Login");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel login)
    {
        if (ModelState.IsValid)
        {
            try
            {
                // Check if UsernameOrEmail and Password matches in the database
                var loggedInMember = await _context.Members
                                        .Where(m => (m.Username == login.UsernameOrEmail || m.Email == login.UsernameOrEmail)
                                        && m.Password == login.Password)
                                        .Select(m => new { m.Username, m.MemberId })
                                        .SingleOrDefaultAsync();

                if (loggedInMember == null)
                {
                    ModelState.AddModelError(string.Empty, "The username/email or password is incorrect.");
                    return View("Login", login);
                }

                // Log the user in
                HttpContext.Session.SetString("Username", loggedInMember.Username);
                HttpContext.Session.SetInt32("Id", loggedInMember.MemberId);

                return RedirectToAction("Index", "Home");
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                _logger?.LogWarning(dbEx, "Database update/connection error during login.");
                ModelState.AddModelError(string.Empty, "Unable to access the database. Please try again later.");
                return View("Login", login);
            }
            catch (System.Exception ex)
            {
                _logger?.LogWarning(ex, "Database access failed during login; returning form with error.");
                ModelState.AddModelError(string.Empty, "Unable to access the database. Please try again later.");
                return View(login);
            }
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
