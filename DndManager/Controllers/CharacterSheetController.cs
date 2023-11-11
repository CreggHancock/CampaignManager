using DndManager.Data;
using DndManager.Helpers;
using DndManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AutoMapper;

using UpdateCharacterDto = DndManager.DataContracts.UpdateCharacterDto;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using DndManager.Services;

namespace DndManager.Controllers;

[Authorize]
public class CharacterSheetController : Controller
{
    private readonly CharacterService characterService;

    public CharacterSheetController(CharacterService characterService)
    {
        this.characterService = characterService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int? id)
    {
        var userIdentifier = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdentifier == null) 
        {
            throw new InvalidOperationException("User must be logged in to access this content");
        }

        var character = await characterService.GetCharacter(userIdentifier, id);
        var model = new CharacterSheetViewModel()
        {
            Character = character!,
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Update([FromBody]UpdateCharacterDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));

        var userIdentifier = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userIdentifier == null)
        {
            throw new InvalidOperationException("User must be logged in to access this content");
        }
        
        if (dto.UserId != userIdentifier)
        {
            throw new InvalidOperationException("Can't update a character that isn't owned by the user");
        }

        var character = await this.characterService.UpdateOrCreateCharacter(userIdentifier, dto);

        return Json(character);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] int characterId)
    {
        var userIdentifier = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdentifier == null)
        {
            throw new InvalidOperationException("User must be logged in to access this content");
        }

        await this.characterService.DeleteCharacter(userIdentifier, characterId);

        return Ok();
    }
}
