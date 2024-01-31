using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IServiceProvider ConfigureServices(IServiceCollection services)
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

        services.AddDbContext<CatebiContext>(options =>
            options.UseNpgsql(Configuration.GetConnectionString("Pgsql")));

        services.AddDbContext<FreeganContext>(options =>
            options.UseNpgsql(Configuration.GetConnectionString("Pgsql")));

        services.Configure<NotionApiSettings>(Configuration.GetSection("NotionApi"));
        var notionAuthToken = Configuration.GetSection("NotionApi:AuthToken").Value;

        services.AddNotionClient(options =>
        {
            options.AuthToken = notionAuthToken;
        });

        var builder = new ContainerBuilder();

        // Pull the .net core dependencies into the container, like controllers
        builder.Populate(services);

        // Register repositories
        var assembly = Assembly.GetAssembly(typeof(BaseRepository<>));
        builder.RegisterAssemblyTypes(assembly)
               .Where(t => t.Name.EndsWith("Repository"))
               .AsImplementedInterfaces()
               .InstancePerLifetimeScope();

        builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();

        // Register other services
        builder.RegisterType<NotionApiService>().As<INotionApiService>().InstancePerLifetimeScope();
        builder.RegisterType<CatService>().As<ICatService>().InstancePerLifetimeScope();
        builder.RegisterType<FreeganMessageService>().As<IFreeganMessageService>().InstancePerLifetimeScope();
        builder.RegisterType<DutyScheduleService>().As<IDutyScheduleService>().InstancePerLifetimeScope();

        var container = builder.Build();

        // Diagnostic tracer for autofac
        // container.EnableAutofacDiagnosticTracer();

        return new AutofacServiceProvider(container);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"));
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
