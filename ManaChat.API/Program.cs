using ManaChat.API.Middleware;
using ManaChat.API.Models.Auth;
using ManaChat.Core.Configuration;
using ManaChat.Core.Constants;
using ManaChat.Core.Models.Auth;
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

builder.Services.AddRuneReaderConfig(DatabaseConstants.MessagingDatabaseKey, builder.Configuration.GetSection("ConnectionStrings"));
builder.Services.Configure<ManaChatConfiguration>(builder.Configuration.GetSection("ManaChat"));
builder.Services.AddScoped<IRuneReaderManager, RuneReaderManager>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();

builder.Services.AddScoped<IAuthenticatedUserDetails, AuthenticatedUserDetails>();

builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new ApplyFromBodyConvention());
})
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<ErrorSafetyMiddleware>();
app.UseMiddleware<EnforceConsumerVersionMiddleware>();
app.UseMiddleware<IdentityValidationMiddleware>();

app.Run();
