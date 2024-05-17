using System.Reflection;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
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

        services.AddDbContext<CatebiContext>(options =>
            options.UseNpgsql(Configuration.GetConnectionString("Pgsql")));

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
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"));
        }

        if (!env.IsDevelopment())
        {
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseRouting();
        app.UseHttpsRedirection();
        app.UseCors("CorsPolicy");
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            );
        });
    }
}
