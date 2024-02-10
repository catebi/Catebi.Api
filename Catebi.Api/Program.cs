using Microsoft.AspNetCore;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebHost.CreateDefaultBuilder(args)
                     .UseStartup<Startup>()
                     .ConfigureAppConfiguration(
                        (hostingContext, config) => {
                            config.AddUserSecrets<Program>();
                        })
                     .Build();

builder.Run();
