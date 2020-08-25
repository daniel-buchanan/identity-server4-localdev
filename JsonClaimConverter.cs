using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class JsonClaimConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return (objectType == typeof(Claim));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        IList<Claim> claims = new List<Claim>();
        JToken jt = JObject.ReadFrom(reader);
        var children = jt.Children<JProperty>();

        string type = children.FirstOrDefault(c => c.Name == "Type")?.Values<string>().FirstOrDefault();
        string value = children.FirstOrDefault(c => c.Name == "Value")?.Values<string>().FirstOrDefault();
        string valueType = children.FirstOrDefault(c => c.Name == "ValueType")?.Values<string>().FirstOrDefault();
        string issuer = children.FirstOrDefault(c => c.Name == "Issuer")?.Values<string>().FirstOrDefault();
        string originalIssuer = children.FirstOrDefault(c => c.Name == "OriginalIssuer")?.Values<string>().FirstOrDefault();
        return new Claim(type, value, valueType, issuer, originalIssuer);
    }

    public override bool CanWrite
    {
        get { return false; }
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {

        var claim = (Claim)value;
        JObject jo = new JObject
        {
            { "Type", claim.Type },
            { "Value", claim.Value },
            { "ValueType", claim.ValueType },
            { "Issuer", claim.Issuer },
            { "OriginalIssuer", claim.OriginalIssuer }
        };
        jo.WriteTo(writer);
    }
}