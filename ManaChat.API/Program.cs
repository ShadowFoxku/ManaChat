using ManaChat.API.Middleware;
using ManaChat.API.Models.Auth;
using ManaChat.Core.Configuration;
using ManaChat.Core.Constants;
using ManaChat.Core.Helpers;
using ManaChat.Core.Models.Auth;
using ManaChat.Identity.Repositories;
using ManaChat.Identity.Services;
using ManaFox.Databases.Core.Interfaces;
using ManaFox.Databases.Extensions;
using ManaFox.Databases.TSQL;

public partial class Program
{
    private static void Main(string[] args)
    {
        ManaLoader.ShowLaunching();
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        builder.Services.AddRuneReaderConfig(DatabaseConstants.MessagingDatabaseKey, builder.Configuration.GetSection("ConnectionStrings"));
        builder.Services.Configure<ManaChatConfiguration>(builder.Configuration.GetSection("ManaChat"));
        builder.Services.AddScoped<IRuneReaderManager, RuneReaderManager>();

        var settings = builder.Configuration.GetSection("ManaChat").Get<ManaChatConfiguration>();
        if (!string.IsNullOrWhiteSpace(settings?.InstanceName))
            ManaLoader.ShowInstanceNamed(settings.InstanceName);

        // identity
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IRelationshipService, RelationshipService>();
        builder.Services.AddScoped<IIdentityService, IdentityService>();
        builder.Services.AddScoped<IUsersRepository, UsersRepository>();
        builder.Services.AddScoped<IRelationshipRepository, RelationshipRepository>();
        builder.Services.AddScoped<IIdentityRepository, IdentityRepository>();

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

        app.Lifetime.ApplicationStarted.Register(() => ManaLoader.ShowReady(settings?.InstanceName ?? string.Empty));

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
    }
}