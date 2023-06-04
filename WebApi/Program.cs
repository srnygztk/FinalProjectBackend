using Infrastracture.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using WebApi.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

var mariaDbConnectionString = builder.Configuration.GetConnectionString("MariaDB")!;

builder.Services.AddDbContext<ApplicationDbContext>(opt=>opt.UseMySql(mariaDbConnectionString,ServerVersion.AutoDetect(mariaDbConnectionString)));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyMethod()
            .AllowCredentials()
            .SetIsOriginAllowed((host) => true)
            .AllowAnyHeader());
});




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();


app.MapHub<SeleniumLogHub>("/Hubs/SeleniumLogHub");
app.MapHub<ProductHub>("/Hubs/ProductHub");
app.MapHub<OrderHub>("/Hubs/OrderHub");


app.Run();