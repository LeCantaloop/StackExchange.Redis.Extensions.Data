namespace StackExchange.Redis.Extensions.Data
{
    public class Key
    {
        public string PropertyName { get; set; }

        public string PropertyValue { get; set; }

        public Key(string propertyName, string propertyValue)
        {
            PropertyName = propertyName.ToLowerInvariant();
            PropertyValue = propertyValue == null ? null : propertyValue.ToLowerInvariant();
        }

        public override string ToString()
        {
            return KeyFormatter.Format( PropertyName, PropertyValue);
        }
    }
}