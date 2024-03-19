using AutoMapper;
using CampaignManager.Data;
using CampaignManager.Data.Initiatives;
using CampaignManager.DataContracts.Initiatives;
using CampaignManager.Helpers;
using Microsoft.EntityFrameworkCore;

namespace CampaignManager.Services;

public class InitiativeService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IMapper mapper;

    public InitiativeService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
    }

    public async Task<IEnumerable<Scene>> GetScenes(string userId)
    {
        var repository = this.unitOfWork.Repository<Scene>();
        var userScenes = await repository.GetAsync(async dbSet =>
        {
            return await dbSet.Where(c => c.UserId == userId).ToListAsync();
        });

        return userScenes ?? Array.Empty<Scene>();
    }

    public async Task<Scene> GetScene(string userId, int? sceneId)
    {
        var scene = SceneHelpers.BuildEmptyScene(userId);
        if (sceneId != null)
        {
            var repository = this.unitOfWork.Repository<Scene>();
            var sceneExists = await repository.ExistsAsync(sceneId);
            scene = sceneExists ? await repository.GetByIdAsync(sceneId) : scene;
            if (scene?.UserId != userId)
            {
                throw new InvalidOperationException("You don't have permission to view this character");
            }
        }

        return scene;
    }

    public async Task<Scene> UpdateOrCreateScene(string userId, UpdateSceneDto updateSceneDto)
    {
        ArgumentNullException.ThrowIfNull(nameof(updateSceneDto));

        var repository = this.unitOfWork.Repository<Scene>();
        var scene = this.mapper.Map<Scene>(updateSceneDto);
        if (updateSceneDto.Id != SceneHelpers.EmptyId)
        {

            var sceneExists = await repository.ExistsAsync(updateSceneDto.Id);
            var sceneForCheck = sceneExists ? await repository.GetByIdAsync(updateSceneDto.Id) : scene;

            if (!sceneExists)
            {
                throw new InvalidOperationException("Id invalid to update this scene.");
            }
            if (sceneExists && sceneForCheck?.UserId != userId)
            {
                throw new InvalidOperationException("Can't update a scene that isn't owned by the user");
            }

            repository.Update(scene);
        }
        else
        {
            await repository.AddAsync(scene);
        }

        await this.unitOfWork.SaveChangesAsync();

        return scene;
    }

    public async Task DeleteScene(string userId, int sceneId)
    {
        var repository = this.unitOfWork.Repository<Scene>();
        if (sceneId != SceneHelpers.EmptyId)
        {

            var sceneExists = await repository.ExistsAsync(sceneId);
            if (!sceneExists)
            {
                throw new InvalidOperationException("Can't delete scene with this id");
            }

            var scene = await repository.GetByIdAsync(sceneId);
            if (scene != null && scene?.UserId != userId)
            {
                throw new InvalidOperationException("Can't update a scene that isn't owned by the user");
            }

            repository.Delete(scene!);
        }

        await this.unitOfWork.SaveChangesAsync();
    }
}
