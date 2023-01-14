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

// Veritabaný iþlemleri yapacaðýmýz servisleri ekledik. Burada .net core a eðer sana IService interface i kullanma isteði gelirse Service sýnýfýndan bir nesne oluþtur demiþ olduk.
// .net core da 3 farklý yöntemle servisleri ekleyebiliyoruz:

// builder.Services.AddSingleton(); : AddSingleton kullanarak oluþturduðumuz nesneden 1 tane örnek oluþur ve her seferinde bu örnek kullanýlýr

// builder.Services.AddTransient() yönteminde ise önceden oluþmuþ nesne varsa o kullanýlýr yoksa yenisi oluþturulur

// builder.Services.AddScoped() yönteminde ise yapýlan her istek için yeni bir nesne oluþturulur

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

app.UseAuthentication(); // Admin Login için

app.UseAuthorization(); // Yetkilendirme (Oturum açan kiþinin admine giriþ yetkisi var mý?)

app.MapControllerRoute(
            name: "admin",
            pattern: "{area:exists}/{controller=Main}/{action=Index}/{id?}"
          );
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
