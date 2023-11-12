using AutoMapper;
using DndManager.Data;
using DndManager.DataContracts;

namespace DndManager.Helpers;

public class AutoMapperProfile : Profile
{
	public AutoMapperProfile()
	{
        CreateMap<UpdateCharacterDto, Character>()
            .ForMember(c => c.Abilities, opt => opt.Ignore())
            .ForMember(c => c.InventoryItems, opt => opt.Ignore())
            .ForMember(c => c.Spells, opt => opt.Ignore())
            .ForMember(c => c.SpellSlots, opt => opt.Ignore())
            .ForMember(c => c.ProficiencyBonuses, opt => opt.Ignore())
            .ForMember(c => c.CharacterClasses, opt => opt.Ignore())
            .ForMember(c => c.Languages, opt => opt.Ignore());
		CreateMap<UpdateAbilityDto, Ability>()
			.ForMember(a => a.Character, opt => opt.Ignore());
		CreateMap<UpdateInventoryItemDto, InventoryItem>()
			.ForMember(a => a.Character, opt => opt.Ignore());
		CreateMap<UpdateProficiencyBonusDto, ProficiencyBonus>()
			.ForMember(a => a.Character, opt => opt.Ignore());
		CreateMap<UpdateSpellDto, Spell>()
			.ForMember(a => a.Character, opt => opt.Ignore());
		CreateMap<UpdateSpellSlotDto, SpellSlot>()
			.ForMember(a => a.Character, opt => opt.Ignore());
		CreateMap<UpdateCharacterClassDto, CharacterClass>()
			.ForMember(a => a.Character, opt => opt.Ignore());
        CreateMap<UpdateLanguageDto, Language>()
            .ForMember(l => l.Character, opt => opt.Ignore());
	}
}
