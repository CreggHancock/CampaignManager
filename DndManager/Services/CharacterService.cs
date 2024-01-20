using AutoMapper;
using DndManager.Data;
using DndManager.Data.Characters;
using DndManager.DataContracts.Characters;
using DndManager.Helpers;

namespace DndManager.Services;

public class CharacterService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IMapper mapper;

    public CharacterService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
    }

    public async Task<Character> GetCharacter(string userId, int? characterId)
    {
        var character = CharacterHelpers.BuildEmptyCharacter(userId);
        if (characterId != null)
        {
            var repository = this.unitOfWork.Repository<Character>();
            var characterExists = await repository.ExistsAsync(characterId);
            character = characterExists ? await repository.GetByIdAsync(characterId) : character;
            if (character?.UserId != userId)
            {
                throw new InvalidOperationException("You don't have permission to view this character");
            }
        }

        return character;
    }

    public async Task<Character> UpdateOrCreateCharacter(string userId, UpdateCharacterDto updateCharacterDto)
    {
        ArgumentNullException.ThrowIfNull(nameof(updateCharacterDto));

        var repository = this.unitOfWork.Repository<Character>();
        var character = this.mapper.Map<Character>(updateCharacterDto);
        if (updateCharacterDto.Id != CharacterHelpers.EmptyId)
        {

            var characterExists = await repository.ExistsAsync(updateCharacterDto.Id);
            var characterForCheck = characterExists ? await repository.GetByIdAsync(updateCharacterDto.Id) : character;

            if (!characterExists)
            {
                throw new InvalidOperationException("Id invalid to update this character.");
            }
            if (characterExists && characterForCheck?.UserId != userId)
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

        return character;
    }

    public async Task DeleteCharacter(string userId, int characterId)
    {
        var repository = this.unitOfWork.Repository<Character>();
        if (characterId != CharacterHelpers.EmptyId)
        {

            var characterExists = await repository.ExistsAsync(characterId);
            if (!characterExists)
            {
                throw new InvalidOperationException("Can't delete character with this id");
            }

            var character = await repository.GetByIdAsync(characterId);
            if (character != null && character?.UserId != userId)
            {
                throw new InvalidOperationException("Can't update a character that isn't owned by the user");
            }

            repository.Delete(character!);
        }

        await this.unitOfWork.SaveChangesAsync();
    }
}
