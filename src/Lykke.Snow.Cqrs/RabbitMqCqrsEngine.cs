using System.Collections.Generic;
using JetBrains.Annotations;
using Lykke.Cqrs;
using Lykke.Cqrs.Configuration;
using Lykke.Messaging;
using Lykke.Messaging.Configuration;
using Lykke.Messaging.Contract;
using Lykke.Messaging.RabbitMq;
using Microsoft.Extensions.Logging;

namespace Lykke.Snow.Cqrs
{
    /// <summary>
    /// Cqrs engine with RabbitMQ transport resolver.
    /// Takes care of messaging engine creation and disposal.
    /// </summary>
    [PublicAPI]
    public class RabbitMqCqrsEngine : CqrsEngine
    {
        public RabbitMqCqrsEngine(ILoggerFactory loggerFactory,
            string endpoint,
            string username,
            string password,
            params IRegistration[] registrations) : base(loggerFactory,
            CreateMessagingEngine(loggerFactory, endpoint, username, password), registrations)
        {
        }

        public RabbitMqCqrsEngine(ILoggerFactory loggerFactory,
            IEndpointProvider endpointProvider,
            string endpoint,
            string username,
            string password,
            params IRegistration[] registrations) : base(loggerFactory,
            CreateMessagingEngine(loggerFactory, endpoint, username, password), endpointProvider,
            registrations)
        {
        }

        public RabbitMqCqrsEngine(ILoggerFactory loggerFactory,
            IDependencyResolver dependencyResolver,
            IEndpointProvider endpointProvider,
            string endpoint,
            string username,
            string password,
            params IRegistration[] registrations) : base(loggerFactory, dependencyResolver,
            CreateMessagingEngine(loggerFactory, endpoint, username, password), endpointProvider,
            registrations)
        {
        }

        public RabbitMqCqrsEngine(ILoggerFactory loggerFactory,
            IDependencyResolver dependencyResolver,
            IEndpointProvider endpointProvider,
            string endpoint,
            string username,
            string password,
            bool createMissingEndpoints,
            params IRegistration[] registrations) : base(loggerFactory, dependencyResolver,
            CreateMessagingEngine(loggerFactory, endpoint, username, password), endpointProvider,
            createMissingEndpoints, registrations)
        {
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                MessagingEngine.Dispose();
            }
        }

        private static IMessagingEngine CreateMessagingEngine(ILoggerFactory loggerFactory,
            string rabbitMqEndpoint,
            string rabbitMqUserName,
            string rabbitMqPassword)
        {
            var transportResolver = CreateTransport(rabbitMqEndpoint, rabbitMqUserName, rabbitMqPassword);

            return new MessagingEngine(loggerFactory, transportResolver, new RabbitMqTransportFactory(loggerFactory));
        }

        private static ITransportResolver CreateTransport(string rabbitMqEndpoint,
            string rabbitMqUserName,
            string rabbitMqPassword)
        {
            return new TransportResolver(new Dictionary<string, TransportInfo>
            {
                {
                    "RabbitMq",
                    new TransportInfo(rabbitMqEndpoint, rabbitMqUserName, rabbitMqPassword, "None", "RabbitMq")
                }
            });
        }
    }
}