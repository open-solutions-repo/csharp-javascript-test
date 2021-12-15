using System.Data;

namespace CSharpJs.Test.Api.Infraestructure
{
    /// <summary>
    /// 
    /// </summary>
    public static class DataReaderExtensionMethods
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="collumName"></param>
        /// <returns></returns>
        public static T Get<T>(this IDataReader reader, string collumName)
        {
            if (reader.IsDBNull(reader.GetOrdinal(collumName)))
                return default;

            return (T)reader.GetValue(reader.GetOrdinal(collumName));
        }
    }
}
