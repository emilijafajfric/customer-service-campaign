using CustomerServiceCampaign.Api.Data;
using CustomerServiceCampaign.Api.Services.Customers;
using CustomerServiceCampaign.Api.Services.Rewards;
using Microsoft.EntityFrameworkCore;
using CustomerServiceCampaign.Api.Data.Seed;
using CustomerServiceCampaign.Api.Services.Purchases;
using CustomerServiceCampaign.Api.Services.Campaigns;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ICustomerService, MockCustomerService>();
builder.Services.AddScoped<IRewardService, RewardService>();
builder.Services.AddScoped<IPurchaseImportService, PurchaseImportService>();
builder.Services.AddScoped<ICampaignService, CampaignService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();

    var dbContext = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();

    await dbContext.Database.MigrateAsync();
    await DataSeeder.SeedAsync(dbContext);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
