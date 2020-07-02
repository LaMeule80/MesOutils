using System;   

namespace Outils.Helper.EnumTools
{
    [AttributeUsage(AttributeTargets.All)]
    public class EnumValueAttribute : Attribute
    {
        public EnumValueAttribute(string value, int id, bool visible) : this(value,id)
        {
            Visible = visible;
        }

        public EnumValueAttribute(string value, int id) : this(value)
        {
            Id = id;
        }

        public EnumValueAttribute(object value)
        {
            Value = value;
        }

        public int Id { get; }

        public object Value { get; }

        public bool Visible { get; }
    }
}