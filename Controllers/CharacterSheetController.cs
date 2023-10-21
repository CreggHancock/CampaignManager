using DndManager.Data;
using DndManager.Helpers;
using DndManager.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DndManager.Controllers;

public class CharacterSheetController : Controller
{
    private readonly IUnitOfWork unitOfWork;

    public CharacterSheetController(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index(int? id)
    {
        var userIdentifier = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var character = CharacterHelpers.BuildEmptyCharacter(userIdentifier);
        if (id != null)
        {
            var repository = this.unitOfWork.Repository<Character>();
            var characterExists = await repository.ExistsAsync(id);
            character = characterExists ? await repository.GetByIdAsync(id) : character;
        }
        
        var model = new CharacterSheetViewModel()
        {
            Character = character,
        };

        return View(model);
    }
}
