using Dapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace CSharpJs.Test.Api.Infraestructure
{
    /// <summary>
    /// Métodos de extensão do Open
    /// </summary>
    public static class ExtensionMethods
    {
        private static string _serverType = ConfigurationManager.AppSettings["ServerType"];

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T current in source)
            {
                action(current);
            }
        }
    }
}
