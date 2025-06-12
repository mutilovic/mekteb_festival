using Mekteb_Festival.Data;
using Mekteb_Festival.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// DB
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Auth
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Blazor
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<ExcelExportService>();
builder.Services.AddScoped<PdfExportService>();
builder.Services.AddSingleton<DzematService>();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();
builder.Services.AddMemoryCache();
builder.Logging.AddConsole();

var app = builder.Build();
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();  // Optionale Migrationen im Entwicklungsmodus
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
