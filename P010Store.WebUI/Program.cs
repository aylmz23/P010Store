using P010Store.Data;
using P010Store.Data.Abstract;
using P010Store.Data.Concrete;
using P010Store.Service.Abstract;
using P010Store.Service.Concrete;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<DatabaseContext>();

builder.Services.AddTransient(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddTransient(typeof(IService<>), typeof(Service<>));

// Veritaban� i�lemleri yapaca��m�z servisleri ekledik. Burada .net core a e�er sana IService interface i kullanma iste�i gelirse Service s�n�f�ndan bir nesne olu�tur demi� olduk.
// .net core da 3 farkl� y�ntemle servisleri ekleyebiliyoruz:

// builder.Services.AddSingleton(); : AddSingleton kullanarak olu�turdu�umuz nesneden 1 tane �rnek olu�ur ve her seferinde bu �rnek kullan�l�r

// builder.Services.AddTransient() y�nteminde ise �nceden olu�mu� nesne varsa o kullan�l�r yoksa yenisi olu�turulur

// builder.Services.AddScoped() y�nteminde ise yap�lan her istek i�in yeni bir nesne olu�turulur

builder.Services.AddTransient<IProductService, ProductService>();
builder.Services.AddTransient<ICategoryService, CategoryService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(x=> {
    x.LoginPath = "/Admin/Login";
    x.AccessDeniedPath = "/AccessDenied";
    x.LogoutPath = "/Admin/Login/LogOut";
    x.Cookie.Name= "Administrator";
    x.Cookie.MaxAge=TimeSpan.FromMinutes(30);
});
builder.Services.AddAuthorization(x =>
{
    x.AddPolicy("AdminPolicy", policy=>policy.RequireClaim("Role","Admin"));
    x.AddPolicy("UserPolicy", policy=>policy.RequireClaim("Role","User"));
});
var app = builder.Build();

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

app.UseAuthentication(); // Admin Login i�in

app.UseAuthorization(); // Yetkilendirme (Oturum a�an ki�inin admine giri� yetkisi var m�?)

app.MapControllerRoute(
            name: "admin",
            pattern: "{area:exists}/{controller=Main}/{action=Index}/{id?}"
          );
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
