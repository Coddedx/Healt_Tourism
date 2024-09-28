using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Plastic.Helper;
using Plastic.IRepository;
using Plastic.Models;
using Plastic.Repository;
using Plastic.Services;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));  //CLOUD�NARY!!!!!!


//timeout ald���m i�in s�rekli 
builder.WebHost.ConfigureKestrel(t =>
{
    t.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(15);
});


builder.Services.AddTransient<IPhotoService, PhotoService>(); //Transient kullanmad�m???????????????    CLOUD�NARY!!!!!! AddScoped
builder.Services.AddTransient<IFranchiseRepository, FranchiseRepository>();
builder.Services.AddTransient<IClinicRepository, ClinicRepository>();
builder.Services.AddTransient<IDoctorRepository, DoctorRepository>();
builder.Services.AddTransient<IOperationDoctorRepository, OperationDoctorRepository>();
//cache eklemeyince repository i kullanam�yorum
builder.Services.AddMemoryCache(); //Caching makes a copy of data that can be returned much faster than from the source. The in-memory cache can store any object. The distributed cache interface is limited to byte
//AddSingleton , AddScoped, AddTransient


//??????????????????????????????????  doctor repository de tempdata kullanaya �al��mak i�in denedim 
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});



//builder.Services.AddDbContext<PlasticDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//setting our app  -----------sonradan ekledim ve json dakinini de!!!!!!!!!!!!
builder.Services.AddDbContext<PlasticDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


//------ Identity Framework Ayarlar� //------
//appdbcontext in alt�nda olmal� identity ayarlar� !!!!!!!!!
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<PlasticDbContext>();
builder.Services.AddMemoryCache(); //bunu eklemezsek de�i�ik bir hata alabiliriz
builder.Services.AddSession(); //cookie autentication (use it if possible rather than jwt Authorization)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();                   //cookie autentication(simplier than jwt Authorization)
//------ Identity Framework Ayarlar� //------
//sonras�nda proje sa� t�k- open in terminal -dotnet run seeddata yaz  


//builder.Services.AddSession(); //http session kullanab. i�in 

var app = builder.Build();

app.UseSession(); //http session kullanab. i�in

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllerRoute(
//        name: "default",
//        pattern: "{controller=Home}/{action=Index}/{id?}");
//});


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Clinic}/{action=Index}/{id?}");

app.Run();
