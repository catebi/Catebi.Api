using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using AirtableApiClient;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using Telegram.Bot;

using Catebi.Api.HealthChecks;
using Catebi.Api.Domain.Features.AdoptionBot;
using Catebi.Api.ExceptionHandlers;

namespace Catebi.Api;

public class Startup(IConfiguration configuration)
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true
    };

    public IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<CatebiContext>(options =>
            options.UseNpgsql(Configuration.GetConnectionString("Pgsql")));

        services.AddDbContext<IdentityContext>(options =>
            options.UseNpgsql(Configuration.GetConnectionString("Pgsql")));

        services.AddIdentity<User, IdentityRole>(options =>
          {
              options.SignIn.RequireConfirmedAccount = true;
          })
          .AddEntityFrameworkStores<IdentityContext>()
          .AddDefaultTokenProviders();

        services.AddAuthentication();
        services.AddAuthorization();

        services.Configure<NotionApiSettings>(Configuration.GetSection("NotionApi"));
        var notionAuthToken = Configuration.GetSection("NotionApi:AuthToken").Value;

        services.AddNotionClient(options =>
        {
            options.AuthToken = notionAuthToken;
        });

        var assembly = Assembly.GetAssembly(typeof(BaseRepository<>));

        foreach (var type in assembly!.GetTypes())
        {
            if (type.Name.EndsWith("Repository") && !type.IsAbstract)
            {
                foreach (var interfaceType in type.GetInterfaces())
                {
                    services.AddScoped(interfaceType, type);
                }
            }
        }

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<INotionApiService, NotionApiService>();
        services.AddScoped<ICatService, CatService>();
        services.AddScoped<IFreeganService, FreeganService>();
        services.AddScoped<IWorkTaskService, WorkTaskService>();
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<IAirtableRepository, AirtableRepository>();
        services.AddScoped<IAdoptionBotUserService, AdoptionBotUserService>();
        services.AddScoped<IAdoptionBotEventService, AdoptionBotEventService>();
        services.AddScoped<IAdoptionBotCatService, AdoptionBotCatService>();
        services.AddScoped<IAdoptionBotAdminService, AdoptionBotAdminService>();
        services.AddScoped<ISettingsService, SettingsService>();

        services.AddTransient<IEmailSender, EmailSender>();

        // airtable initialization for AdoptionBot
        services.AddScoped(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var apiKey = configuration["AdoptionBot:Airtable:ApiKey"];
            var baseId = configuration["AdoptionBot:Airtable:BaseId"];

            return new AirtableBase(apiKey, baseId);
        });

        // telegram bot initialization for AdoptionBot
        services.AddSingleton(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var botToken = configuration["AdoptionBot:Telegram:Token"]!;
            return new TelegramBotClient(botToken);
        });

        // Common telegram bot for work chat notifications
        services.AddSingleton<CommonTelegramBotClient>(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var botToken = configuration["AdoptionBot:Telegram:CommonBotToken"]!;
            return new CommonTelegramBotClient(new TelegramBotClient(botToken));
        });

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
            {
                builder.WithOrigins("http://localhost:4200", "https://catebi.ge", "https://api.catebi.ge", "https://catebi-adoption-miniapp.catebi.ge")
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials();
            });
        });

        services.AddHealthChecks()
                .AddDbContextCheck<CatebiContext>("Catebi Database Health Check")
                .AddCheck<VersionInfoCheck>("VersionInfo");

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            c.AddSecurityDefinition("cookieAuth", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Cookie,
                Name = ".AspNetCore.Identity.Application",
                Description = "Identity cookie authentication"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "cookieAuth"
                                }
                            },
                            Array.Empty<string>()
                        }
                    });
        });
        services.AddMemoryCache();

        services.AddLogging(loggingBuilder => loggingBuilder
            .SetMinimumLevel(LogLevel.Debug)
            .AddConsole(options =>
            {
                options.FormatterName = "simple";
            })
            .AddSimpleConsole(options =>
            {
                options.IncludeScopes = false;
                options.SingleLine = true;
                options.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
                options.UseUtcTimestamp = false;
            })
        );

        // services.AddLogging((loggingBuilder) => loggingBuilder
        // .SetMinimumLevel(LogLevel.Debug)
        // .AddOpenTelemetry(options =>
        //     options
        //         .AddConsoleExporter()
        //         .SetResourceBuilder(
        //             ResourceBuilder.CreateDefault()
        //                 .AddService("Catebi.Api"))
        //     )
        // );

        // Register localization service
        services.AddScoped<ILocalizationService, LocalizationService>();

        // Register exception handlers
        services.AddExceptionHandler<ValidationExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(ui =>
            {
                ui.SwaggerEndpoint("/swagger/v1/swagger.json", "Adoption Bot API v1");
                // ui.RoutePrefix = "docs";    // uncomment to serve UI at /docs
            });
        }

        if (!env.IsDevelopment())
        {
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        // Add exception handling middleware
        app.UseExceptionHandler();

        app.UseCors("CorsPolicy");

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints
                .MapHealthChecks("/hc", new HealthCheckOptions
                {
                    ResponseWriter = async (context, report) =>
                    {
                        context.Response.ContentType = "application/json";

                        var result = new
                        {
                            status = report.Status.ToString(),
                            hcRequestDuration = report.TotalDuration.ToString(),
                            uptime = TimeSpan.FromMilliseconds(Environment.TickCount64).ToString(),
                            checks = report.Entries.ToDictionary(
                                entry => entry.Key,
                                entry => new
                                {
                                    status = entry.Value.Status.ToString(),
                                    description = entry.Value.Description,
                                    data = entry.Value.Data
                                })
                        };

                        await context.Response.WriteAsync(JsonSerializer.Serialize(result, _jsonOptions));
                    }
                });
            endpoints.MapControllers();
        });
    }
}
