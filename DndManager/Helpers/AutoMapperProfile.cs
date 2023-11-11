using AutoMapper;
using Data = DndManager.Data;
using DataContracts = DndManager.DataContracts;

namespace DndManager.Helpers;

public class AutoMapperProfile : Profile
{
	public AutoMapperProfile()
	{
		CreateMap<DataContracts.UpdateCharacterDto, Data.Character>()
			.ForMember(c => c.Abilities, opt => opt.Ignore())
			.ForMember(c => c.InventoryItems, opt => opt.Ignore())
			.ForMember(c => c.Spells, opt => opt.Ignore())
			.ForMember(c => c.SpellSlots, opt => opt.Ignore())
			.ForMember(c => c.ProficiencyBonuses, opt => opt.Ignore())
			.ForMember(c => c.CharacterClasses, opt => opt.Ignore());
		CreateMap<DataContracts.UpdateAbilityDto, Data.Ability>()
			.ForMember(a => a.Character, opt => opt.Ignore());
		CreateMap<DataContracts.UpdateInventoryItemDto, Data.InventoryItem>()
			.ForMember(a => a.Character, opt => opt.Ignore());
		CreateMap<DataContracts.UpdateProficiencyBonusDto, Data.ProficiencyBonus>()
			.ForMember(a => a.Character, opt => opt.Ignore());
		CreateMap<DataContracts.UpdateSpellDto, Data.Spell>()
			.ForMember(a => a.Character, opt => opt.Ignore());
		CreateMap<DataContracts.UpdateSpellSlotDto, Data.SpellSlot>()
			.ForMember(a => a.Character, opt => opt.Ignore());
		CreateMap<DataContracts.UpdateCharacterClassDto, Data.CharacterClass>()
			.ForMember(a => a.Character, opt => opt.Ignore());
	}
}
