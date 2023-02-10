using System;
using System.Collections.Generic;
using Lykke.Cqrs;
using Lykke.Cqrs.Configuration;
using Lykke.Messaging;
using Lykke.Messaging.Configuration;
using Lykke.Messaging.Contract;
using Lykke.Messaging.RabbitMq;
using Lykke.Messaging.RabbitMq.Retry;
using Lykke.Snow.Cqrs.Logging;
using Microsoft.Extensions.Logging;

namespace Lykke.Snow.Cqrs
{
    /// <summary>
    /// Cqrs engine with RabbitMQ transport resolver.
    /// Takes care of messaging engine creation and disposal.
    /// </summary>
    public class RabbitMqCqrsEngine : CqrsEngine
    {
        public RabbitMqCqrsEngine(ILoggerFactory loggerFactory,
            string endpoint,
            string username,
            string password,
            TimeSpan automaticRecoveryInterval,
            IRetryPolicyProvider retryPolicyProvider,
            params IRegistration[] registrations) : base(loggerFactory,
            CreateMessagingEngine(loggerFactory,
                endpoint,
                username,
                password,
                automaticRecoveryInterval,
                retryPolicyProvider),
            registrations)
        {
        }

        public RabbitMqCqrsEngine(ILoggerFactory loggerFactory,
            IEndpointProvider endpointProvider,
            string endpoint,
            string username,
            string password,
            TimeSpan automaticRecoveryInterval,
            IRetryPolicyProvider retryPolicyProvider,
            params IRegistration[] registrations) : base(loggerFactory,
            CreateMessagingEngine(loggerFactory,
                endpoint,
                username,
                password,
                automaticRecoveryInterval,
                retryPolicyProvider), 
            endpointProvider,
            registrations)
        {
        }

        public RabbitMqCqrsEngine(ILoggerFactory loggerFactory,
            IDependencyResolver dependencyResolver,
            IEndpointProvider endpointProvider,
            string endpoint,
            string username,
            string password,
            TimeSpan automaticRecoveryInterval,
            IRetryPolicyProvider retryPolicyProvider,
            params IRegistration[] registrations) : base(loggerFactory,
            dependencyResolver,
            CreateMessagingEngine(loggerFactory,
                endpoint,
                username,
                password,
                automaticRecoveryInterval,
                retryPolicyProvider),
            endpointProvider,
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
            TimeSpan automaticRecoveryInterval,
            IRetryPolicyProvider retryPolicyProvider,
            params IRegistration[] registrations) : base(loggerFactory,
            dependencyResolver,
            CreateMessagingEngine(loggerFactory,
                endpoint,
                username,
                password,
                automaticRecoveryInterval,
                retryPolicyProvider),
            endpointProvider,
            createMissingEndpoints,
            registrations)
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
            string rabbitMqPassword,
            TimeSpan automaticRecoveryInterval,
            IRetryPolicyProvider retryPolicyProvider)
        {
            var transportResolver = CreateTransport(rabbitMqEndpoint, rabbitMqUserName, rabbitMqPassword);
            var engine = new MessagingEngine(loggerFactory, transportResolver,
                new RabbitMqTransportFactory(loggerFactory, automaticRecoveryInterval, retryPolicyProvider));

            return new LoggingMessagingEngineDecorator(engine, loggerFactory);
        }

        private static ITransportInfoResolver CreateTransport(string rabbitMqEndpoint,
            string rabbitMqUserName,
            string rabbitMqPassword)
        {
            return new TransportInfoResolver(new Dictionary<string, TransportInfo>
            {
                {
                    "RabbitMq",
                    new TransportInfo(rabbitMqEndpoint, rabbitMqUserName, rabbitMqPassword, "None", "RabbitMq")
                }
            });
        }
    }
}