using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VisualHFT.Commons.Helpers
{
    public static class AttributesHelper
    {
        public static TValue? GetAttributeValue<TAttr, TValue>(object? target, Func<TAttr, TValue> valueSelector) where TAttr : Attribute
            where TValue : class
        {
            if (target == null)
                return null;

            var attribute = target.GetType().GetCustomAttribute<TAttr>();

            if (attribute == null)
                return null;

            return valueSelector(attribute);
        }

        public static string? GetEnumDescription(Enum? target)
        {
            if (target == null)
                return null;

            var fi = target.GetType().GetField(target.ToString());

            if (fi == null) 
                return null;

            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return target.ToString();
        }
    }
}
