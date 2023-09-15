using Catebi.Map.WebApi.Models;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var services = builder.Services;

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.Configure<NotionApiSettings>(builder.Configuration.GetSection("NotionApi"));

var notionAuthToken = builder.Configuration.GetSection("NotionApi:AuthToken").Value;

services.AddNotionClient(options => {
  options.AuthToken = notionAuthToken;
});

var app = builder.Build();

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
