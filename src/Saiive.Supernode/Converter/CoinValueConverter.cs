using System;
using Newtonsoft.Json;

namespace Saiive.SuperNode.Converter
{
    public class CoinValueConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var value = reader.Value;
            
            if(value != null)
            {
                var longValue = Convert.ToUInt64(value);
                if (longValue == 0)
                {
                    return 0.0;
                }
                return (double)longValue;
            }

            return 0;
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }
    }
}
