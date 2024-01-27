using DndManager.Data.Characters;
using DndManager.Data.Initiatives;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DndManager.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Character> Characters => this.Set<Character>();

    public DbSet<CharacterClass> CharacterClasses => this.Set<CharacterClass>();

    public DbSet<InventoryItem> InventoryItems => this.Set<InventoryItem>();

    public DbSet<ProficiencyBonus> ProfiencyBonuses => this.Set<ProficiencyBonus>();

    public DbSet<Spell> Spells => this.Set<Spell>();

    public DbSet<SpellSlot> SpellSlots => this.Set<SpellSlot>();

    public DbSet<CharacterAbility> Abilities => this.Set<CharacterAbility>();

    public DbSet<Language> Languages => this.Set<Language>();

    public DbSet<Scene> Scenes => this.Set<Scene>();

    public DbSet<Combatant> Combatants => this.Set<Combatant>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        if (modelBuilder == null)
        {
            throw new ArgumentNullException(nameof(modelBuilder));
        }
    }
}
