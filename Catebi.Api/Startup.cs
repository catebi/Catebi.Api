using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;

namespace Catebi.Api;

public class Startup(IConfiguration configuration)
{
    public IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<CatebiContext>(options =>
            options.UseNpgsql(Configuration.GetConnectionString("Pgsql")));

        services.AddDbContext<IdentityContext>(options =>
            options.UseNpgsql(Configuration.GetConnectionString("Pgsql")));

        // services.AddIdentityApiEndpoints<IdentityUser>(options =>
        // {
        //     options.SignIn.RequireConfirmedAccount = true;
        // })
        // .AddEntityFrameworkStores<IdentityContext>();

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

        foreach (var type in assembly.GetTypes())
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
        services.AddScoped<IDutyScheduleService, DutyScheduleService>();

        services.AddTransient<IEmailSender, EmailSender>();

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
            {
                builder.WithOrigins("http://localhost:4200")
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials();
            });
        });

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

        services.AddLogging((loggingBuilder) => loggingBuilder
        .SetMinimumLevel(LogLevel.Debug)
        .AddOpenTelemetry(options =>
            options
                .AddConsoleExporter()
                .SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddService("Catebi.Api"))
            )
        );
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        if (!env.IsDevelopment())
        {
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        // app.UseHttpsRedirection();
        app.UseCors("CorsPolicy");

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        // app.UseEndpoints(endpoints =>
        // {
        //     endpoints.MapControllerRoute(
        //         name: "default",
        //         pattern: "{controller=Home}/{action=Index}/{id?}"
        //     );
        // });
        app.UseEndpoints(endpoints => endpoints.MapControllers());

    }
}
