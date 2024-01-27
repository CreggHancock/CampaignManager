using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DndManager.Models;
using DndManager.Data;
using DndManager.DataContracts;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using DndManager.Helpers;
using DndManager.Data.Characters;
using Microsoft.AspNetCore.Authorization;
using DndManager.Services;
using DndManager.Data.Initiatives;

namespace DndManager.Controllers;

/// <summary>The controller for the home page.</summary>
/// <param name="logger"></param>
/// <param name="unitOfWork"></param>
[ApiController]
[Route("[Controller]/[Action]")]
public class HomeController(ILogger<HomeController> logger, InitiativeService initiativeService, IUnitOfWork unitOfWork) : ControllerBase
{

    /// <summary>Gets the data needed for the home page</summary>
    /// <returns>A <see cref="HomeViewModel"/> containing data needed for the home page.</returns>
    [HttpGet]
    public async Task<HomeViewModel> Get()
    {
        var userAuthenticated = User.Identity?.IsAuthenticated ?? false;
        IEnumerable<Character> characters = Array.Empty<Character>();
        IEnumerable<Scene> initiativeScenes = Array.Empty<Scene>();
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userAuthenticated && userId != null) 
        {
            var characterRepository = unitOfWork.Repository<Character>();
            characters = await characterRepository.GetAsync(async dbSet =>
            {
                return await dbSet.Where(c => c.UserId == userId).ToListAsync();
            });

            var emptyCharacter = CharacterHelpers.BuildEmptyCharacter(userId);
            characters = characters.Append(emptyCharacter);

            initiativeScenes = await initiativeService.GetScenes(userId);
        }

        return new HomeViewModel
        {
            UserCharacters = characters,
            UserScenes = initiativeScenes
        };
    }

    /// <summary>Gets the data needed for the home page</summary>
    /// <returns>A <see "/> containing data needed for the home page.</returns>
    [Authorize]
    [HttpGet]
    public string? Username()
    {
        return User.Identity?.Name;
    }
}
