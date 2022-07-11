using System;
using System.Collections.Generic;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Cqrs;
using Lykke.Cqrs.Configuration;
using Lykke.Messaging;
using Lykke.Messaging.Configuration;
using Lykke.Messaging.Contract;
using Lykke.Messaging.RabbitMq;

namespace Lykke.Snow.Cqrs
{
    /// <summary>
    /// Cqrs engine with RabbitMQ transport resolver.
    /// Takes care of messaging engine creation and disposal.
    /// </summary>
    [PublicAPI]
    public class RabbitMqCqrsEngine : CqrsEngine
    {
        public RabbitMqCqrsEngine(ILog log,
            string endpoint,
            string username,
            string password,
            params IRegistration[] registrations) : base(log,
            CreateMessagingEngine(log, endpoint, username, password), registrations)
        {
        }

        public RabbitMqCqrsEngine(ILog log,
            IEndpointProvider endpointProvider,
            string endpoint,
            string username,
            string password,
            params IRegistration[] registrations) : base(log,
            CreateMessagingEngine(log, endpoint, username, password), endpointProvider, registrations)
        {
        }

        public RabbitMqCqrsEngine(ILog log,
            IDependencyResolver dependencyResolver,
            IEndpointProvider endpointProvider,
            string endpoint,
            string username,
            string password,
            params IRegistration[] registrations) : base(log, dependencyResolver,
            CreateMessagingEngine(log, endpoint, username, password), endpointProvider, registrations)
        {
        }

        public RabbitMqCqrsEngine(ILog log,
            IDependencyResolver dependencyResolver,
            IEndpointProvider endpointProvider,
            string endpoint,
            string username,
            string password,
            bool createMissingEndpoints,
            params IRegistration[] registrations) : base(log, dependencyResolver,
            CreateMessagingEngine(log, endpoint, username, password), endpointProvider,
            createMissingEndpoints, registrations)
        {
        }

        public RabbitMqCqrsEngine(ILogFactory logFactory,
            string endpoint,
            string username,
            string password,
            params IRegistration[] registrations) : base(logFactory,
            CreateMessagingEngine(logFactory, endpoint, username, password), registrations)
        {
        }

        public RabbitMqCqrsEngine(ILogFactory logFactory,
            IEndpointProvider endpointProvider,
            string endpoint,
            string username,
            string password,
            params IRegistration[] registrations) : base(logFactory,
            CreateMessagingEngine(logFactory, endpoint, username, password), endpointProvider,
            registrations)
        {
        }

        public RabbitMqCqrsEngine(ILogFactory logFactory,
            IDependencyResolver dependencyResolver,
            IEndpointProvider endpointProvider,
            string endpoint,
            string username,
            string password,
            params IRegistration[] registrations) : base(logFactory, dependencyResolver,
            CreateMessagingEngine(logFactory, endpoint, username, password), endpointProvider,
            registrations)
        {
        }

        public RabbitMqCqrsEngine(ILogFactory logFactory,
            IDependencyResolver dependencyResolver,
            IEndpointProvider endpointProvider,
            string endpoint,
            string username,
            string password,
            bool createMissingEndpoints,
            params IRegistration[] registrations) : base(logFactory, dependencyResolver,
            CreateMessagingEngine(logFactory, endpoint, username, password), endpointProvider,
            createMissingEndpoints, registrations)
        {
        }

        [Obsolete("Please, take care of messaging engine disposal")]
        public RabbitMqCqrsEngine(ILogFactory logFactory,
            IMessagingEngine messagingEngine,
            params IRegistration[] registrations) : base(logFactory, messagingEngine, registrations)
        {
        }

        [Obsolete("Please, take care of messaging engine disposal")]
        public RabbitMqCqrsEngine(ILogFactory logFactory,
            IMessagingEngine messagingEngine,
            IEndpointProvider endpointProvider,
            params IRegistration[] registrations) : base(logFactory, messagingEngine, endpointProvider, registrations)
        {
        }

        [Obsolete("Please, take care of messaging engine disposal")]
        public RabbitMqCqrsEngine(ILogFactory logFactory,
            IDependencyResolver dependencyResolver,
            IMessagingEngine messagingEngine,
            IEndpointProvider endpointProvider,
            params IRegistration[] registrations) : base(logFactory, dependencyResolver, messagingEngine,
            endpointProvider, registrations)
        {
        }

        [Obsolete("Please, take care of messaging engine disposal")]
        public RabbitMqCqrsEngine(ILogFactory logFactory,
            IDependencyResolver dependencyResolver,
            IMessagingEngine messagingEngine,
            IEndpointProvider endpointProvider,
            bool createMissingEndpoints,
            params IRegistration[] registrations) : base(logFactory, dependencyResolver, messagingEngine,
            endpointProvider, createMissingEndpoints, registrations)
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

        private static IMessagingEngine CreateMessagingEngine(ILogFactory logFactory,
            string rabbitMqEndpoint,
            string rabbitMqUserName,
            string rabbitMqPassword)
        {
            var transportResolver = CreateTransport(rabbitMqEndpoint, rabbitMqUserName, rabbitMqPassword);

            return new MessagingEngine(logFactory, transportResolver, new RabbitMqTransportFactory());
        }

        private static IMessagingEngine CreateMessagingEngine(ILog log,
            string rabbitMqEndpoint,
            string rabbitMqUserName,
            string rabbitMqPassword)
        {
            var transportResolver = CreateTransport(rabbitMqEndpoint, rabbitMqUserName, rabbitMqPassword);

            return new MessagingEngine(log, transportResolver, new RabbitMqTransportFactory());
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