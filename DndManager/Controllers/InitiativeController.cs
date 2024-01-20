using DndManager.DataContracts.Initiatives;
using DndManager.Models;
using DndManager.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DndManager.Controllers
{
    public class InitiativeController : Controller
    {
        private readonly InitiativeService initiativeService;

        public InitiativeController(InitiativeService initiativeService)
        {
            this.initiativeService = initiativeService;
        }

        public async Task<IActionResult> Index(int? id)
        {
            var userIdentifier = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdentifier == null)
            {
                throw new InvalidOperationException("User must be logged in to access this content");
            }

            var scene = await initiativeService.GetScene(userIdentifier, id);
            var model = new InitiativeViewModel()
            {
                Scene = scene!,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update([FromBody] UpdateSceneDto dto)
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

            var scene = await this.initiativeService.UpdateOrCreateScene(userIdentifier, dto);

            return Json(scene);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromBody] int sceneId)
        {
            var userIdentifier = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdentifier == null)
            {
                throw new InvalidOperationException("User must be logged in to access this content");
            }

            await this.initiativeService.DeleteScene(userIdentifier, sceneId);

            return Ok();
        }
    }
}
