using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RecursosHumanos.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Cadena de Coneccion string (DefaultConnection) no se encuentra");
var connectionStringFederal = builder.Configuration.GetConnectionString("Federal") ?? throw new InvalidOperationException("Cadena de Coneccion string (Federal) no se encuentra");
var connectionStringHomologados = builder.Configuration.GetConnectionString("Homologados") ?? throw new InvalidOperationException("Cadena de Coneccion string (Homologados) no se encuentra");
var connectionStringCrsp = builder.Configuration.GetConnectionString("Crsp") ?? throw new InvalidOperationException("Cadena de Coneccion string (Crsp) no se encuentra");
var connectionStringAcreditados = builder.Configuration.GetConnectionString("Acreditados") ?? throw new InvalidOperationException("Cadena de Coneccion string (Acreditados) no se encuentra");
var connectionStringRegularizados = builder.Configuration.GetConnectionString("Regularizados") ?? throw new InvalidOperationException("Cadena de Coneccion string (Regularizados) no se encuentra");
var connectionStringProgramasPrioritarios = builder.Configuration.GetConnectionString("ProgramasPrioritarios") ?? throw new InvalidOperationException("Cadena de Coneccion string (ProgramasPrioritarios) no se encuentra");
var connectionStringFormalizados = builder.Configuration.GetConnectionString("Formalizados") ?? throw new InvalidOperationException("Cadena de Coneccion string (Formalizados) no se encuentra");
var connectionStringConveniosFederales = builder.Configuration.GetConnectionString("ConveniosFederales") ?? throw new InvalidOperationException("Cadena de Coneccion string (ConveniosFederales) no se encuentra");
var connectionStringContratos = builder.Configuration.GetConnectionString("Contratos") ?? throw new InvalidOperationException("Cadena de Coneccion string (Contratos) no se encuentra");
var connectionStringContratosIB = builder.Configuration.GetConnectionString("ContratosIB") ?? throw new InvalidOperationException("Cadena de Coneccion string (ContratosIB) no se encuentra");
var connectionStringFormalizadosIB = builder.Configuration.GetConnectionString("FormalizadosIB") ?? throw new InvalidOperationException("Cadena de Coneccion string (FormalizadosIB) no se encuentra");
var connectionStringRegularizadosIB = builder.Configuration.GetConnectionString("RegularizadosIB") ?? throw new InvalidOperationException("Cadena de Coneccion string (RegularizadosIB) no se encuentra");
var connectionStringFederalIB = builder.Configuration.GetConnectionString("FederalIB") ?? throw new InvalidOperationException("Cadena de Coneccion string (FederalIB) no se encuentra");
var connectionStringHomoIB = builder.Configuration.GetConnectionString("HomoIB") ?? throw new InvalidOperationException("Cadena de Coneccion string (HomoIB) no se encuentra");


builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
