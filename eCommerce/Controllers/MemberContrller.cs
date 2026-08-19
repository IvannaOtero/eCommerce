using Microsoft.AspNetCore.Mvc;

namespace eCommerce.Controllers;

public class MemberContrller : Controller
{
    public IActionResult Register()
    {
        return View(); 
    }
}
