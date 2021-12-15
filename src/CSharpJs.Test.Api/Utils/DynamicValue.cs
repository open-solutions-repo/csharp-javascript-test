namespace CSharpJs.Test.Api.Utils
{
    public static class DynamicValue
    {
        public static string GetDynamicValue(string value, dynamic obj)
        {
            var propertyInfo = obj.GetType().GetProperty(value);
            return propertyInfo.GetValue(obj, null);
        }
    }
}
