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

namespace DndManager.Controllers;

/// <summary>The controller for the home page.</summary>
/// <param name="logger"></param>
/// <param name="unitOfWork"></param>
[ApiController]
[Route("[Controller]/[Action]")]
public class HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork) : ControllerBase
{
    private readonly ILogger<HomeController> _logger = logger;
    private readonly IUnitOfWork unitOfWork = unitOfWork;

    /// <summary>Gets the data needed for the home page</summary>
    /// <returns>A <see cref="HomeViewModel"/> containing data needed for the home page.</returns>
    [HttpGet]
    public async Task<HomeViewModel> Get()
    {
        var userAuthenticated = User.Identity?.IsAuthenticated ?? false;
        IEnumerable<Character> characters = Array.Empty<Character>();
        string? userId = null;
        if (userAuthenticated) 
        {
            userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var characterRepository = unitOfWork.Repository<Character>();
            characters = await characterRepository.GetAsync(async dbSet =>
            {
                return await dbSet.Where(c => c.UserId == userId).ToListAsync();
            });

            var emptyCharacter = CharacterHelpers.BuildEmptyCharacter(userId);
            characters = characters.Append(emptyCharacter);
        }

        return new HomeViewModel
        {
            UserCharacters = characters,
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
