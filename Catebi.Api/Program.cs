using Microsoft.AspNetCore;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebHost.CreateDefaultBuilder(args)
                     .UseStartup<Startup>()
                     .ConfigureAppConfiguration(
                        (hostingContext, config) => {
                            config.AddUserSecrets<Program>();
                            // var builtConfig = config.Build();
                            // config.AddJsonFile(builtConfig["VaultSettingsFilePath"], true, false);
                        })
                     .Build();

builder.Run();
