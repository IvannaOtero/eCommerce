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
}
