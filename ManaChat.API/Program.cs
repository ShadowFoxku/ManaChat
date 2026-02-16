using ManaChat.Core.Constants;
using ManaChat.Identity.Constants;
using ManaChat.Identity.Repositories;
using ManaChat.Identity.Services;
using ManaFox.Databases.Core.Interfaces;
using ManaFox.Databases.Extensions;
using ManaFox.Databases.TSQL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddRuneReaderDb(DatabaseConstants.MessagingDatabaseString, builder.Configuration.GetSection("ConnectionStrings"));
builder.Services.AddScoped<IRuneReaderManager, RuneReaderManager>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
