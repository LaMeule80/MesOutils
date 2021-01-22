using System;

namespace Outils.SQL
{
    [Flags]
    public enum TypeField
    {
        Ajout = 1,
        Modification = 2,
        Suppression = 4
    }

    public class FieldProcStoDescription
    {
        [Obsolete]
        public FieldProcStoDescription(string name, IProcStoBase proSto)
        {
            Name = name;
            proSto.Add(this);
        }

        public FieldProcStoDescription(IProcStoBase proSto, string name, string field)
        {
            proSto.Add(this);
            Name = name;
            Field = field;
        }

        public FieldProcStoDescription(IProcStoBase proSto, string name, string field, TypeField typeField) :
            this(proSto, name, field)
        {
            TypeField = typeField;
        }

        public string Name { get; }

        public object Value { get; set; }
            
        public string Field { get; }

        public TypeField TypeField { get; }
    }
}