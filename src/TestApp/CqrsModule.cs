using Autofac;
using Lykke.Cqrs;
using Lykke.Cqrs.Configuration;
using Lykke.Cqrs.Configuration.BoundedContext;
using Lykke.Cqrs.Configuration.Routing;
using Lykke.Messaging.RabbitMq.Retry;
using Lykke.Messaging.Serialization;
using Lykke.Snow.Cqrs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TestApp;
using AutofacDependencyResolver = TestApp.AutofacDependencyResolver;

public class CqrsModule : Module
{
    private readonly IConfiguration _configuration;
    private CqrsContextNamesSettings _contextNames = new CqrsContextNamesSettings();
    private const string DefaultRoute = "self";
    private const string DefaultPipeline = "commands";
    private const string DefaultEventPipeline = "events";

    public CqrsModule(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void Load(ContainerBuilder builder)
    {
        builder.Register(context => new AutofacDependencyResolver(context))
            .As<IDependencyResolver>()
            .SingleInstance();

        builder.RegisterInstance(_contextNames).AsSelf().SingleInstance();

        builder.Register(CreateEngine)
            .As<ICqrsEngine>()
            .SingleInstance()
            .AutoActivate();
    }

    private CqrsEngine CreateEngine(IComponentContext ctx)
    {
        var rabbitMqConventionEndpointResolver = new RabbitMqConventionEndpointResolver(
            "RabbitMq",
            SerializationFormat.MessagePack,
            environment: "dev");

        var rabbitMqSettings = new RabbitMQ.Client.ConnectionFactory
        {
            Uri = new Uri(_configuration.GetConnectionString("rabbitmq"), UriKind.Absolute)
        };

        var engine = new RabbitMqCqrsEngine(
            ctx.Resolve<ILoggerFactory>(),
            ctx.Resolve<IDependencyResolver>(),
            new DefaultEndpointProvider(),
            rabbitMqSettings.Endpoint.ToString(),
            rabbitMqSettings.UserName,
            rabbitMqSettings.Password,
            true,
            TimeSpan.FromSeconds(15),
            ctx.Resolve<IRetryPolicyProvider>(),
            Register.DefaultEndpointResolver(rabbitMqConventionEndpointResolver),
            RegisterContext(),
            RegisterDefaultRouting()
        );


        engine.StartPublishers();

        return engine;
    }

    private IRegistration RegisterContext()
    {
        var contextRegistration = Register
            .BoundedContext(_contextNames.MyService)
            .ProcessingOptions(DefaultRoute)
            .MultiThreaded(8)
            .QueueCapacity(1024);
        RegisterEventPublishing(contextRegistration);

        return contextRegistration;
    }
    
    
    private static void RegisterEventPublishing(
        ProcessingOptionsDescriptor<IBoundedContextRegistration> contextRegistration)
    {
        contextRegistration.PublishingEvents(typeof(MyEvent)
            )
            .With(DefaultEventPipeline);
    }

    private PublishingCommandsDescriptor<IDefaultRoutingRegistration> RegisterDefaultRouting()
    {
        return Register.DefaultRouting
            .PublishingCommands(typeof(MyEvent)
            )
            .To(_contextNames.MyService)
            .With(DefaultPipeline);
    }
}