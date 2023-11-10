using DndManager.Data;
using DndManager.Helpers;
using DndManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AutoMapper;

using UpdateCharacterDto = DndManager.DataContracts.UpdateCharacterDto;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DndManager.Controllers;

[Authorize]
public class CharacterSheetController : Controller
{
    private readonly IUnitOfWork unitOfWork;
	private readonly IMapper mapper;

    public CharacterSheetController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        this.unitOfWork = unitOfWork;
		this.mapper = mapper;
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
    public async Task<IActionResult> Update([FromBody]UpdateCharacterDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));

        var userIdentifier = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (dto.UserId != userIdentifier)
        {
            throw new InvalidOperationException("Can't update a character that isn't owned by the user");
        }

        var repository = this.unitOfWork.Repository<Data.Character>();
        var character = this.mapper.Map<Character>(dto);
        if (dto.Id != CharacterHelpers.EmptyId)
        {
            
            var characterExists = await repository.ExistsAsync(dto.Id);
            var characterForCheck = characterExists ? await repository.GetByIdAsync(dto.Id) : character;
            if (characterExists && characterForCheck?.UserId != userIdentifier) 
            {
                throw new InvalidOperationException("Can't update a character that isn't owned by the user");
            }

            repository.Update(character);
        }
        else
        {
            await repository.AddAsync(character);
        }

        await this.unitOfWork.SaveChangesAsync();

        return Json(character);
    }
}
