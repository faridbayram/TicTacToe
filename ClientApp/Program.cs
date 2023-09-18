using ClientApp.Middlewares;
using Core.Persistence.Abstract;
using Core.Persistence.Concrete;
using Core.Services.Abstract;
using Core.Services.Concrete;
using Core.Utilities.Security.JWT;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Server.Utilities.Security.Encryption;
using Server.Utilities.Security.JWT;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();

builder.Services.AddSingleton<IUserPersistence, UserPersistence>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<ITokenHelper, JwtHelper>();
builder.Services.AddTransient<IBonusService, BonusService>();

var tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<TokenOptions>();
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidIssuer = tokenOptions?.Issuer,
            ValidateLifetime = true,
            ValidateAudience = true,
            ValidAudience = tokenOptions?.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = SecurityKeyHelper.CreateSecurityKey(tokenOptions?.SecurityKey)
        };

        options.Events = new JwtBearerEvents()
        {
            OnChallenge = context =>
            {
                var request = context.HttpContext.Request;
                string redirectUrl = $"{request.Path}{request.QueryString}";
                
                context.HandleResponse();
                context.Response.Redirect($"/account/login?redirectUrl={redirectUrl}");
                return Task.CompletedTask;
            }
        };
    });

var app = builder.Build();

app.UseSession();
app.UseMiddleware<AuthHeaderMiddleware>();

// // Configure the HTTP request pipeline.
// if (!app.Environment.IsDevelopment())
// {
//     app.UseExceptionHandler("/Home/Error");
//     // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//     app.UseHsts();
// }

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();