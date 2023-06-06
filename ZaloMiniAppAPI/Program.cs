using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ZaloMiniAppAPI;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddDataProtection();
builder.Services.AddOptions<JwtSettings>()
        .Bind(builder.Configuration.GetSection("JwtSettings"))
        .ValidateDataAnnotations();

builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.WithOrigins("http://localhost:2999").AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddDbContext<ProductStore>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ProductStore") + ";TrustServerCertificate=true"));

/*builder.Services.AddIdentityCore<AccoutDetail>(options =>
{
    // Cấu hình các tùy chọn của Identity tại đây (nếu cần)
})
    .AddUserStore<UserStore<AccoutDetail, IdentityRole, ProductStore, string>>()
    .AddDefaultTokenProviders();*/
builder.Services.AddScoped<IJwtService, JwtService>();





var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers()
     .RequireCors(builder => builder.WithOrigins("http://localhost:2999")
                                       .AllowAnyHeader()
                                       .AllowAnyMethod());
});


app.Run();
