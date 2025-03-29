using EF_Core;
using EF_Core.Models;
using EShop.Manegers;
using EShop.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NewWithApi.Helpers.filter;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// services to the container.


builder.Services.AddControllers(i =>
{
    i.Filters.Add<GemeralExceptionFilter>();
});

builder.Services.AddDbContext<EShopContext>
    (i => i.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("MyDB")));

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<EShopContext>();
builder.Services.AddScoped(typeof(ProductManager));
builder.Services.AddScoped(typeof(CategoryManager));
builder.Services.AddScoped(typeof(RoleManager));
//builder.Services.AddScoped(typeof(AccountManager));
//builder.Services.AddScoped(typeof(AccountServices));
//builder.Services.AddScoped(typeof(VendorManager));
//builder.Services.AddScoped(typeof(ClientManager));
//builder.Services.AddScoped(typeof(CartItemManager));


builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>
{
    //on One Statless Request
    option.SaveToken = true;
    option.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateAudience = false,
        ValidateIssuer = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JWT:PrivateKey"]))
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>{ policy.AllowAnyOrigin()
                                                   .AllowAnyMethod()
                                                   .AllowAnyHeader();});});


var app = builder.Build();

// Configure the HTTP request pipeline.

//app.UseAuthorization();

app.UseStaticFiles();
app.UseRouting();
app.UseStaticFiles();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=index}");


app.Run();