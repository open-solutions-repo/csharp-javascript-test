using System.Collections.Generic;

namespace CSharpJs.Test.Api.Model
{
    public class ModelReport
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string AliasName { get; set; }
        public string PrintFormat { get; set; }
        public List<Parameter> Parameters { get; set; }
    }
    public class Parameter
    {
        public string FieldName { get; set; }
        public string FieldText { get; set; }
        public string FieldType { get; set; }
        public bool FieldOptional { get; set; }
        public string FieldValue { get; set; }
        public string FieldDiscreteOrRange { get; set; }
        public string FieldReportName { get; set; }
        public List<DefaultValue> DefaultValueList { get; set; }
    }

    public class DefaultValue
    {
        public string Description { get; set; }
        public string Value { get; set; }
    }
}
