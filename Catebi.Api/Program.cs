  AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var services = builder.Services;

// Register CORS policy
services
  .AddCors(options =>
  {
    options.AddPolicy(
        "CorsPolicy",
        builder =>
        {
          builder
              .AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
              .Build();
        }
    );
  });

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddMemoryCache();

services.AddDbContext<CatebiContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Pgsql")));

services.Configure<NotionApiSettings>(builder.Configuration.GetSection("NotionApi"));

var notionAuthToken = builder.Configuration.GetSection("NotionApi:AuthToken").Value;

services.AddNotionClient(options =>
{
  options.AuthToken = notionAuthToken;
});

services.AddScoped<INotionApiService, NotionApiService>();
services.AddScoped<IMapService, MapService>();
services.AddScoped<IDutyScheduleService, DutyScheduleService>();

var app = builder.Build();

app.UseCors("CorsPolicy");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.Run();
