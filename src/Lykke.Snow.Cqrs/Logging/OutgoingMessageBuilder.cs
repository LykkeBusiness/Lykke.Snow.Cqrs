using System;
using System.Collections.Generic;
using System.Linq;
using Lykke.Messaging.Contract;

namespace Lykke.Snow.Cqrs.Logging
{
    public class OutgoingMessageBuilder
    {
        public OutgoingMessage Create<TMessage>(byte[] serializedMessage, Endpoint endpoint,
            IDictionary<string, string> headers)
        {
            var type = typeof(TMessage);

            var message = new OutgoingMessage
            {
                MessageTypeName = type.Name,
                MessageTypeFullName = type.FullName,
                Exchange = endpoint.Destination.Publish,
                RoutingKey = type.Name,
                Format = ToLoggingFormat(endpoint.SerializationFormat),
                Headers = headers == null
                    ? new Dictionary<string, object>()
                    : new Dictionary<string, object>(
                        headers
                            .Select(kvp => new KeyValuePair<string, object>(kvp.Key, kvp.Value))
                            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value)),
                Timestamp = DateTime.UtcNow,
                Message = Convert.ToBase64String(serializedMessage),
            };

            return message;
        }

        public OutgoingMessage Create(Type type,
            byte[] serializedMessage,
            Endpoint endpoint,
            Dictionary<string, string> headers)
        {
            var message = new OutgoingMessage
            {
                MessageTypeName = type.Name,
                MessageTypeFullName = type.FullName,
                Exchange = endpoint.Destination.Publish,
                RoutingKey = type.Name,
                Format = ToLoggingFormat(endpoint.SerializationFormat),
                Headers = headers == null
                    ? new Dictionary<string, object>()
                    : new Dictionary<string, object>(
                        headers
                            .Select(kvp => new KeyValuePair<string, object>(kvp.Key, kvp.Value))
                            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value)),
                Timestamp = DateTime.UtcNow,
                Message = Convert.ToBase64String(serializedMessage),
            };

            return message;
        }

        private SerializationFormat ToLoggingFormat(
            Messaging.Serialization.SerializationFormat endpointSerializationFormat)
        {
            switch (endpointSerializationFormat)
            {
                case Messaging.Serialization.SerializationFormat.Json:
                    return SerializationFormat.Json;
                case Messaging.Serialization.SerializationFormat.MessagePack:
                    return SerializationFormat.Messagepack;
                case Messaging.Serialization.SerializationFormat.ProtoBuf:
                    return SerializationFormat.Protobuf;
                default:
                    return SerializationFormat.Unknown;
            }
        }
    }
}