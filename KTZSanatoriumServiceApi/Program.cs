using KTZSanatoriumServiceApi.Data.Repository;
using KTZSanatoriumServiceApi.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://127.0.0.1:7087");
// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Policy", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.Configure<DbSetting>(
    builder.Configuration.GetSection("Database"));
builder.Services.AddSingleton<IDbSetting>(provider =>
    provider.GetRequiredService<IOptions<DbSetting>>().Value);

builder.Services.AddScoped(typeof(IDbRepository<>), typeof(DataRepository<>));

builder.Services.Configure<UserSetting>(
    builder.Configuration.GetSection("UserSettings"));
builder.Services.AddSingleton<IUserSetting>(provider =>
    provider.GetRequiredService<IOptions<UserSetting>>().Value);


builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddResponseCaching();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(builder.Configuration["UserSettings:SecretKey"]))
        };
    });


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
app.UseHttpsRedirection();
app.UseResponseCaching();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});
app.UseCors("Policy");
app.Run();
