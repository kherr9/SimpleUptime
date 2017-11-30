namespace SimpleUptime.Infrastructure.Services
{
#if NET461
    using System;
    using System.IO;
    using Newtonsoft.Json;
    using Microsoft.ServiceBus.Messaging;
    public class BrokeredMessageToValueConverter
    {
        public object Convert(BrokeredMessage message)
        {
            var messageType = GetMessageType(message);

            using (var stream = message.GetBody<Stream>())
            using (var reader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(reader))
            {
                var jsonSerializer = JsonSerializer.Create(Constants.JsonSerializerSettings);

                return jsonSerializer.Deserialize(jsonReader, messageType);

            }
        }

        private Type GetMessageType(BrokeredMessage message)
        {
            var typeName = (string)message.Properties[Constants.Properties.MessageTypeAssemblyQualifiedName];

            return Type.GetType(typeName, true);
        }
    }
#endif
}