using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Plastic.Data;
using Plastic.Helper;
using Plastic.Hubs;
using Plastic.IRepository;
using Plastic.Models;
using Plastic.Repository;
using Plastic.Services;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.AddSingleton<RabbitMqService>();
builder.Services.AddSignalR();
builder.Services.AddHostedService<RabbitMqListener>();


//timeout ald���m i�in s�rekli 
builder.WebHost.ConfigureKestrel(t =>
{
    t.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(15);
});

//builder.Services.AddDbContext<PlasticDbContext>(options =>
//    options.UseSqlServer(connectionString, sqlOptions =>
//        sqlOptions.CommandTimeout(60)));

builder.Services.AddTransient<IPhotoService, PhotoService>(); 
builder.Services.AddTransient<IFranchiseRepository, FranchiseRepository>();
builder.Services.AddTransient<IClinicRepository, ClinicRepository>();
builder.Services.AddTransient<IDoctorRepository, DoctorRepository>();
builder.Services.AddTransient<IOperationDoctorRepository, OperationDoctorRepository>();
//cache eklemeyince repository i kullanam�yorum
builder.Services.AddMemoryCache(); //Caching makes a copy of data that can be returned much faster than from the source. The in-memory cache can store any object. The distributed cache interface is limited to byte


//??????????????????????????????????  doctor repository de tempdata kullanaya �al��mak i�in denedim 
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});



//builder.Services.AddDbContext<PlasticDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<PlasticDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") + ";Connection Timeout=60;");
});


//------ Identity Framework Ayarlar� //------
//appdbcontext in alt�nda olmal� identity ayarlar� !!!!!!!!!
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<PlasticDbContext>();

//builder.Services.AddMemoryCache(); //bunu eklemezsek de�i�ik bir hata alabiliriz B�R TANE CACHE OLMASI YETERL� - AddDbContext VE IDENT�TY ROLE EKLEMDEN �NCE KULLANILIR GENELDE 

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

app.UseAuthentication();  //UseRouting ve end points a��a��s�nda olmal�!!
app.UseAuthorization();  

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllerRoute(
//        name: "default",
//        pattern: "{controller=Home}/{action=Index}/{id?}");
//});


//app
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Clinic}/{action=Index}/{id?}");

    endpoints.MapHub<ChatHub>("/chathub");
});

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await Seed.SeedUsersAndRolesAsync(app); // Rolleri ve kullan�c�lar� seed et
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during seeding users and roles.");
    }
}

app.Run();
