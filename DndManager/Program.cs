using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DndManager.Data;
using DndManager.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Reflection;
using DndManager.Helpers;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.CookiePolicy;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

ConfigureServices(builder.Services, builder.Configuration, builder.Environment);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapSwagger().RequireAuthorization();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowAppCommunication");

app.UseAuthorization();

app.MapControllers();
app.MapIdentityApi<IdentityUser>();

app.Run();

static void ConfigureServices(
    IServiceCollection services,
    ConfigurationManager configuration,
    IWebHostEnvironment environment)
{
    services.AddTransient<IEmailSender, EmailSender>();
    services.AddTransient<IUnitOfWork, UnitOfWork>();
    services.AddTransient<CharacterService>();
    services.AddTransient<InitiativeService>();

    services.AddIdentityApiEndpoints<IdentityUser>()
        .AddEntityFrameworkStores<ApplicationDbContext>();

    services.AddAutoMapper(Assembly.GetExecutingAssembly());

    services.AddControllers(static mvcOptions => mvcOptions.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>());
    services.AddAutoMapper(mapperConfig => mapperConfig.AddProfile<AutoMapperProfile>());

    services.AddCors(options =>
    {
        options.AddPolicy(
            "AllowAppCommunication",
            builder => builder.WithOrigins("http://localhost:5173")
                              .AllowAnyMethod()
                              .AllowCredentials()
                              .AllowAnyHeader());
    });

    if (environment.IsDevelopment())
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(
            options =>
            {
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
            });
    }
}
