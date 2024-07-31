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
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));  //CLOUDÝNARY!!!!!!


//timeout aldýðým için sürekli 
builder.WebHost.ConfigureKestrel(t =>
{
    t.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(15);
});


builder.Services.AddScoped<IPhotoService, PhotoService>(); //Transient kullanmadým???????????????    CLOUDÝNARY!!!!!!
builder.Services.AddTransient<IFranchiseRepository, FranchiseRepository>();
builder.Services.AddTransient<IClinicRepository, ClinicRepository>();
builder.Services.AddTransient<IDoctorRepository, DoctorRepository>();
//cache eklemeyince repository i kullanamýyorum
builder.Services.AddMemoryCache(); //Caching makes a copy of data that can be returned much faster than from the source. The in-memory cache can store any object. The distributed cache interface is limited to byte
//AddSingleton , AddScoped, AddTransient


//??????????????????????????????????  doctor repository de tempdata kullanaya çalýþmak için denedim 
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

//builder.Services.AddSession(); //http session kullanab. için 

var app = builder.Build();

app.UseSession(); //http session kullanab. için

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



app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
