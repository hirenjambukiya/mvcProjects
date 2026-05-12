using DynamicExcel.Core.Interfaces;
using DynamicExcel.Infrastructure.Repositories;
using DynamicExcel.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Setup DI
builder.Services.AddSingleton<IDatabaseConnectionRepository, JsonDatabaseConnectionRepository>();
builder.Services.AddSingleton<IImportHistoryRepository, JsonImportHistoryRepository>();
builder.Services.AddScoped<IDatabaseService, DatabaseService>();
builder.Services.AddScoped<IExcelService, ExcelService>();
builder.Services.AddScoped<IQueryService, QueryService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
