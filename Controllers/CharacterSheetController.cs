using DndManager.Data;
using DndManager.Helpers;
using DndManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DndManager.Controllers;

[Authorize]
public class CharacterSheetController : Controller
{
    private readonly IUnitOfWork unitOfWork;

    public CharacterSheetController(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    [HttpGet]
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

    [HttpPost]
    public async Task<IActionResult> Update(UpdateCharacterDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));

        var userIdentifier = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (dto.Character.UserId != userIdentifier)
        {
            throw new InvalidOperationException("Can't update a character that isn't owned by the user");
        }

        var repository = this.unitOfWork.Repository<Character>();
        Character? character = null;
        if (dto.Character.Id != CharacterHelpers.EmptyId)
        {
            
            var characterExists = await repository.ExistsAsync(dto.Character.Id);
            character = characterExists ? await repository.GetByIdAsync(dto.Character.Id) : character;
            if (characterExists && character?.UserId != userIdentifier) 
            {
                throw new InvalidOperationException("Can't update a character that isn't owned by the user");
            }

            repository.Update(dto.Character);
        }
        else
        {
            await repository.AddAsync(dto.Character);
        }

        await this.unitOfWork.SaveChangesAsync();

        return Json(character);
    }
}
