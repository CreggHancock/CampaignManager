using DndManager.Data.Initiatives;
using DndManager.DataContracts.Initiatives;
using DndManager.Models;
using DndManager.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DndManager.Controllers
{
    /// <summary>The controller used to manage initiative tracking.</summary>
    /// <param name="initiativeService">The <see cref="InitiativeService"/></param>
    [ApiController]
    [Route("[Controller]/[Action]")]
    public class InitiativeController(InitiativeService initiativeService) : Controller
    {
        /// <summary>Gets a model containing the scene used for initiative tracking.</summary>
        /// <param name="id">The scene ID.</param>
        /// <returns>The model containing the scene.</returns>
        [HttpGet]
        public async Task<InitiativeViewModel> Get(int? id)
        {
            var userIdentifier = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdentifier == null)
            {
                throw new InvalidOperationException("User must be logged in to access this content");
            }

            var scene = await initiativeService.GetScene(userIdentifier, id);
            return new InitiativeViewModel()
            {
                Scene = scene!,
            };
        }

        /// <summary>Updates an initiative scene.</summary>
        /// <param name="dto">The <see cref="UpdateSceneDto"/></param>
        /// <returns>The updated scene.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<Scene> Update([FromBody] UpdateSceneDto dto)
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

           return await initiativeService.UpdateOrCreateScene(userIdentifier, dto);
        }

        /// <summary>Deletes an initiative scene.</summary>
        /// <param name="id">The scene ID.</param>
        /// <returns>A Task.</returns>
        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task Delete([FromBody] int id)
        {
            var userIdentifier = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdentifier == null)
            {
                throw new InvalidOperationException("User must be logged in to access this content");
            }

            await initiativeService.DeleteScene(userIdentifier, id);
        }
    }
}
