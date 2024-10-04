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

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));  //CLOUDÝNARY!!!!!!
builder.Services.AddSingleton<RabbitMqService>();
builder.Services.AddSignalR();
builder.Services.AddHostedService<RabbitMqListener>();


//timeout aldýðým için sürekli 
builder.WebHost.ConfigureKestrel(t =>
{
    t.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(15);
});


builder.Services.AddTransient<IPhotoService, PhotoService>(); //Transient kullanmadým???????????????    CLOUDÝNARY!!!!!! AddScoped
builder.Services.AddTransient<IFranchiseRepository, FranchiseRepository>();
builder.Services.AddTransient<IClinicRepository, ClinicRepository>();
builder.Services.AddTransient<IDoctorRepository, DoctorRepository>();
builder.Services.AddTransient<IOperationDoctorRepository, OperationDoctorRepository>();
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


//------ Identity Framework Ayarlarý //------
//appdbcontext in altýnda olmalý identity ayarlarý !!!!!!!!!
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<PlasticDbContext>();
builder.Services.AddMemoryCache(); //bunu eklemezsek deðiþik bir hata alabiliriz
builder.Services.AddSession(); //cookie autentication (use it if possible rather than jwt Authorization)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();                   //cookie autentication(simplier than jwt Authorization)
//------ Identity Framework Ayarlarý //------
//sonrasýnda proje sað týk- open in terminal -dotnet run seeddata yaz  


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


app.UseAuthentication();  //UseRouting ve end points aasýnda olmalý!!
app.UseAuthorization();  //UseRouting ve end points aasýnda olmalý!!

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
        await Seed.SeedUsersAndRolesAsync(app); // Rolleri ve kullanýcýlarý seed et
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during seeding users and roles.");
    }
}

app.Run();
