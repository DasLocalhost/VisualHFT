using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace VisualHFT.Commons.WPF.MarkupExt
{
    public class EnumAsItemSource : MarkupExtension
    {
        private readonly Type? _type;

        public EnumAsItemSource() { }

        public EnumAsItemSource(Type type)
        {
            _type = type;
        }

        public override object? ProvideValue(IServiceProvider serviceProvider)
        {
            // TODO : change to string to description
            // TODO : errors handling

            if (_type == null)
                return null;

            return Enum.GetValues(_type)
                .Cast<object>()
                .Select(_ => new { Value = _, DisplayName = _.ToString() });
        }
    }
}
