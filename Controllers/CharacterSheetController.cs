using Microsoft.AspNetCore.Mvc;

namespace DndManager.Controllers;

public class CharacterSheetController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
