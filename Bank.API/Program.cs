using Bank.API.MIddleware;
using Bank.BLL.Services;
using Bank.Core.Interfaces;
using Bank.Core.Interfaces.Repositories;
using Bank.Core.Interfaces.Services;
using Bank.DAL;
using Bank.DAL.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Database connection config
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString(nameof(ApplicationDbContext)),
    npgsql =>
    {
        npgsql.MigrationsHistoryTable("__ef_migrations_history", "public");
    })
    .UseSnakeCaseNamingConvention();
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IAccountsRepository, AccountsRepository>();
builder.Services.AddScoped<ITransactionsRepository, TransactionsRepository>();

builder.Services.AddScoped<IAccountsService, AccountsService>();
builder.Services.AddScoped<ITransactionsService, TransactionsService>();

builder.Services.AddEndpointsApiExplorer();

// Swagger configuration
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
});

builder.Services.AddControllers();

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

// Database migrations setup
using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (dbContext.Database.CanConnect())
        {
            logger.LogInformation("✅ PostgreSQL: connection successful.");
            dbContext.Database.Migrate();
        }
        else
            logger.LogError("❌ PostgreSQL: connection failed.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ PostgreSQL connection error.");
    }
}

app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.MapControllers();
app.Run();