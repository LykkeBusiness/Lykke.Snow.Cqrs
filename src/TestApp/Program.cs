using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TestApp;

var hostBuilder = Host.CreateDefaultBuilder(args)
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>((context, builder) =>
    {
        builder.RegisterModule(new CqrsModule(context.Configuration));
    })
    .ConfigureAppConfiguration((context, builder) =>
    {
        builder.SetBasePath(Directory.GetCurrentDirectory());
        builder.AddJsonFile("appsettings.json");
        builder.AddUserSecrets<Program>();
    })
    .ConfigureLogging(x => x.AddConsole())
    .ConfigureServices((context, services) => { services.AddSingleton<App>(); });

var host = hostBuilder.Build();

var app = host.Services.GetRequiredService<App>();
app.Execute();