namespace StackExchange.Redis.Extensions.Data
{
    public class Key
    {
        private string _propertyName;
        private string _propertyValue;

        public string PropertyName
        {
            get { return _propertyName; }
            set { _propertyName = value.ToLowerInvariant(); }
        }

        public string PropertyValue
        {
            get { return _propertyValue; }
            set { _propertyValue = value?.ToLowerInvariant(); }
        }

        public Key(string propertyName, string propertyValue)
        {
            PropertyName = propertyName;
            PropertyValue = propertyValue;
        }

        public override string ToString()
        {
            return KeyFormatter.Format(PropertyName, PropertyValue);
        }
    }
}