using System;
using System.Collections.Generic;
using Lykke.Messaging;
using Lykke.Messaging.Contract;
using Microsoft.Extensions.Logging;

namespace Lykke.Snow.Cqrs.Logging
{
    public class LoggingMessagingEngineDecorator : IMessagingEngine
    {
        private readonly IMessagingEngine _messagingEngine;
        private readonly OutgoingMessageBuilder _outgoingMessageBuilder = new OutgoingMessageBuilder();
        private readonly OutgoingMessageLogger _outgoingMessageLogger;

        public LoggingMessagingEngineDecorator(IMessagingEngine messagingEngine,
            ILoggerFactory loggerFactory)
        {
            _messagingEngine = messagingEngine;
            _outgoingMessageLogger = new OutgoingMessageLogger(loggerFactory.CreateLogger<OutgoingMessageLogger>());
        }

        public void Send<TMessage>(TMessage message, Endpoint endpoint, string processingGroup = null,
            Dictionary<string, string> headers = null)
        {
            var serialized = SerializationManager.SerializeObject(endpoint.SerializationFormat, message);
            var outgoingMessage = _outgoingMessageBuilder.Create<TMessage>(serialized, endpoint, headers);
            _outgoingMessageLogger.Log(outgoingMessage);
            
            _messagingEngine.Send(message, endpoint, processingGroup, headers);
        }

        public void Send<TMessage>(TMessage message, Endpoint endpoint, int ttl, string processingGroup = null,
            Dictionary<string, string> headers = null)
        {
            var serialized = SerializationManager.SerializeObject(endpoint.SerializationFormat, message);
            var outgoingMessage = _outgoingMessageBuilder.Create<TMessage>(serialized, endpoint, headers);
            _outgoingMessageLogger.Log(outgoingMessage);

            _messagingEngine.Send(message, endpoint, ttl, processingGroup, headers);
        }

        public void Send(object message, Endpoint endpoint, string processingGroup = null,
            Dictionary<string, string> headers = null)
        {
            var serialized = SerializationManager.SerializeObject(endpoint.SerializationFormat, message);
            var outgoingMessage = _outgoingMessageBuilder.Create(message.GetType(), serialized, endpoint, headers);
            _outgoingMessageLogger.Log(outgoingMessage);

            _messagingEngine.Send(message, endpoint, processingGroup, headers);
        }

        #region OtherMethods

        public void Dispose()
        {
        }

        public void AddProcessingGroup(string name, ProcessingGroupInfo info)
        {
            _messagingEngine.AddProcessingGroup(name, info);
        }

        public bool GetProcessingGroupInfo(string name, out ProcessingGroupInfo groupInfo)
        {
            return _messagingEngine.GetProcessingGroupInfo(name, out groupInfo);
        }

        public IDisposable SubscribeOnTransportEvents(TransportEventHandler handler)
        {
            return _messagingEngine.SubscribeOnTransportEvents(handler);
        }


        public Destination CreateTemporaryDestination(string transportId, string processingGroup)
        {
            return _messagingEngine.CreateTemporaryDestination(transportId, processingGroup);
        }

        public IDisposable Subscribe<TMessage>(Endpoint endpoint, Action<TMessage> callback)
        {
            return _messagingEngine.Subscribe(endpoint, callback);
        }

        public IDisposable Subscribe<TMessage>(Endpoint endpoint, CallbackDelegate<TMessage> callback,
            string processingGroup = null,
            int priority = 0)
        {
            return _messagingEngine.Subscribe(endpoint, callback, processingGroup, priority);
        }

        public IDisposable Subscribe(Endpoint endpoint, Action<object> callback, Action<string> unknownTypeCallback,
            params Type[] knownTypes)
        {
            return _messagingEngine.Subscribe(endpoint, callback, unknownTypeCallback, knownTypes);
        }

        public IDisposable Subscribe(Endpoint endpoint, Action<object> callback, Action<string> unknownTypeCallback,
            string processingGroup,
            int priority = 0, params Type[] knownTypes)
        {
            return _messagingEngine.Subscribe(endpoint, callback, unknownTypeCallback, processingGroup, priority,
                knownTypes);
        }

        public IDisposable Subscribe(Endpoint endpoint, CallbackDelegate<object> callback,
            Action<string, AcknowledgeDelegate> unknownTypeCallback,
            params Type[] knownTypes)
        {
            return _messagingEngine.Subscribe(endpoint, callback, unknownTypeCallback, knownTypes);
        }

        public IDisposable Subscribe(Endpoint endpoint, CallbackDelegate<object> callback,
            Action<string, AcknowledgeDelegate> unknownTypeCallback, string processingGroup,
            int priority = 0, params Type[] knownTypes)
        {
            return _messagingEngine.Subscribe(endpoint, callback, unknownTypeCallback, processingGroup, priority,
                knownTypes);
        }

        public TResponse SendRequest<TRequest, TResponse>(TRequest request, Endpoint endpoint, long timeout = 30000)
        {
            return _messagingEngine.SendRequest<TRequest, TResponse>(request, endpoint, timeout);
        }

        public IDisposable SendRequestAsync<TRequest, TResponse>(TRequest request, Endpoint endpoint,
            Action<TResponse> callback,
            Action<Exception> onFailure, long timeout = 30000, string processingGroup = null)
        {
            return _messagingEngine.SendRequestAsync(request, endpoint, callback, onFailure, timeout, processingGroup);
        }

        public IDisposable RegisterHandler<TRequest, TResponse>(Func<TRequest, TResponse> handler, Endpoint endpoint)
            where TResponse : class
        {
            return _messagingEngine.RegisterHandler(handler, endpoint);
        }

        public bool VerifyEndpoint(Endpoint endpoint, EndpointUsage usage, bool configureIfRequired, out string error)
        {
            return _messagingEngine.VerifyEndpoint(endpoint, usage, configureIfRequired, out error);
        }

        public Dictionary<Endpoint, string> VerifyEndpoints(EndpointUsage usage, IEnumerable<Endpoint> endpoints,
            bool configureIfRequired)
        {
            return _messagingEngine.VerifyEndpoints(usage, endpoints, configureIfRequired);
        }

        public string GetStatistics()
        {
            return _messagingEngine.GetStatistics();
        }

        public ISerializationManager SerializationManager => _messagingEngine.SerializationManager;

        #endregion
    }
}