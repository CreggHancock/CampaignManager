using DndManager.Data;
using DndManager.Helpers;
using DndManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AutoMapper;

using UpdateCharacterDto = DndManager.DataContracts.Characters.UpdateCharacterDto;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using DndManager.Services;
using DndManager.Data.Characters;

namespace DndManager.Controllers;

/// <summary>The controller used for character management.</summary>
/// <param name="characterService">The character service.</param>
[Authorize]
[Route("[Controller]/[Action]")]
public class CharacterSheetController(CharacterService characterService) : Controller
{
    private readonly CharacterService characterService = characterService;

    /// <summary>Gets a character.</summary>
    /// <param name="id">The character ID.</param>
    /// <returns>The character.</returns>
    [HttpGet]
    public async Task<CharacterSheetViewModel> Get(int? id)
    {
        var userIdentifier = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdentifier == null) 
        {
            throw new InvalidOperationException("User must be logged in to access this content");
        }

        var character = await characterService.GetCharacter(userIdentifier, id);
        return new CharacterSheetViewModel()
        {
            Character = character!,
        };
    }

    /// <summary>Updates a character.</summary>
    /// <param name="dto">The <see cref="UpdateCharacterDto"/> dto.</param>
    /// <returns>The updated character.</returns>
    [HttpPost]
    public async Task<Character> Update([FromBody]UpdateCharacterDto dto)
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

        return await this.characterService.UpdateOrCreateCharacter(userIdentifier, dto);
    }

    /// <summary>Deletes a character.</summary>
    /// <param name="characterId">The character ID.</param>
    /// <returns>A Task.</returns>
    [HttpDelete]
    public async Task Delete([FromBody] int characterId)
    {
        var userIdentifier = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdentifier == null)
        {
            throw new InvalidOperationException("User must be logged in to access this content");
        }

        await this.characterService.DeleteCharacter(userIdentifier, characterId);
    }
}
