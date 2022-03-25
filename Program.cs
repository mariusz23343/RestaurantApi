using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NLog.Web;
using RestaurantApi;
using RestaurantApi.Authorization;
using RestaurantApi.Entities;
using RestaurantApi.Middleware;
using RestaurantApi.Models;
using RestaurantApi.Models.Validators;
using RestaurantApi.Services;
using RestaurantAPI.Models.Validators;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder();

//NLog: Setup for DI
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
builder.Host.UseNLog();

// configure service


var authenticationSetting = new AuthenticationSettings();

builder.Configuration.GetSection("Authentication").Bind(authenticationSetting); //ladnie zbinduje, jakby zmapuje do obiektu

builder.Services.AddSingleton(authenticationSetting);

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = "Bearer";
    option.DefaultScheme = "Bearer";
    option.DefaultChallengeScheme = "Bearer";
}).AddJwtBearer(conf => {
    conf.RequireHttpsMetadata = false;
    conf.SaveToken = true;
    conf.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = authenticationSetting.JwtIssuer,
        ValidAudience = authenticationSetting.JwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSetting.JwtKey))
    };
});

builder.Services.AddAuthorization(option =>
{
    option.AddPolicy("HasNationality", builder => builder.RequireClaim("Nationality"));
    option.AddPolicy("Atleast20", builder => builder.AddRequirements(new MinimumAgeRequirement(20)));
    option.AddPolicy("CreatedAtleast2Restaurants", builder => builder.AddRequirements(new CreateMultipleRestaurantRequirement(2)));
});

builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementHandler>();
builder.Services.AddScoped<IAuthorizationHandler, MinimumAgeRequirementHandler>();
builder.Services.AddScoped<IAuthorizationHandler, CreatedMultipleRestaurantRequirementHandler>();
builder.Services.AddControllers().AddFluentValidation();
builder.Services.AddDbContext<RestaurantDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("RestaurantDbConnection")));
builder.Services.AddScoped<RestaurantSeeder>();
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddScoped<IRestaurantService, RestaurantService>();
builder.Services.AddScoped<IDishService, DishService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ErrorHandlingMiddleware>();
builder.Services.AddScoped<RequestTimeMiddleware>();
builder.Services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
builder.Services.AddScoped<IValidator<RestaurantQuery>, RestaurantQueryValidator>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontEndClient", policyBuilder =>
    {
        policyBuilder.AllowAnyMethod().AllowAnyHeader().WithOrigins(builder.Configuration["AllowOrigins"]);
    });
});

// configure

var app = builder.Build();

//wydobywanie typu z kontenera DI
var scope = app.Services.CreateScope();
var seeder = scope.ServiceProvider.GetRequiredService<RestaurantSeeder>();

app.UseResponseCaching();
app.UseStaticFiles();
app.UseCors("FrontEndClient");

seeder.Seed();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestTimeMiddleware>();

app.UseAuthentication();

app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant API");
});

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();