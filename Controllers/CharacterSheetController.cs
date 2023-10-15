using DndManager.Data;
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
        var character = this.BuildEmptyCharacter();
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

    private Character BuildEmptyCharacter()
    {
        var userIdentifier = this.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (userIdentifier == null)
        {
            throw new InvalidOperationException("Can't use an invalid user ID");
        }

        return new Character()
        {
            UserId = userIdentifier,
            Name = string.Empty,
            Description = string.Empty,
            Alignment = string.Empty,
            Race = string.Empty,
            HitDice = string.Empty,
            CharacterClasses = Array.Empty<CharacterClass>(),
            InventoryItems = Array.Empty<InventoryItem>(),
            ProficiencyBonuses = Array.Empty<ProficiencyBonus>(),
            Spells = Array.Empty<Spell>(),
            SpellSlots = Array.Empty<SpellSlot>(),
            Abilities = Array.Empty<Ability>(),
        };
    }
}
