using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace SimpleUptime.Infrastructure.Services
{
    public class ValueToMessageConverter
    {
        public Message Convert(object value)
        {
            var valueType = value.GetType();

            var bytes = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value,
                Constants.JsonSerializerSettings));

            return new Message(bytes)
            {
                ContentType = "application/json",
                UserProperties =
                {
                    {Constants.Properties.MessageTypeName, valueType.Name},
                    {Constants.Properties.MessageTypeAssemblyQualifiedName, valueType.AssemblyQualifiedName}
                }
            };
        }
    }
}