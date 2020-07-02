using System;
using System.Text;
using Outils.Helper.EnumTools;

namespace Outils.SQL
{
    public struct Parametre

    {
        public Parametre(FieldViewDescription champ, object value, OrdreLogique ordre = OrdreLogique.Egal)
        {
            Champ = champ;
            Value = value;
            Ordre = ordre;
        }

        public FieldViewDescription Champ { get; }
        public object Value { get; }
        public OrdreLogique Ordre { get; }

        public string Clause
        {
            get
            {
                switch (Ordre)
                {
                    case OrdreLogique.In:
                        return new StringBuilder()
                            .Append(Champ.SqlName)
                            .Append(EnumHelper.GetValue<string>(Ordre))
                            .Append($"({Value})")
                            .ToString();
                    case OrdreLogique.Null:
                        return new StringBuilder()
                            .Append(Champ.SqlName).Append(" IS NULL ")
                            .ToString();
                    case OrdreLogique.Egal:
                        return new StringBuilder()
                            .Append(Champ.Name)
                            .Append(EnumHelper.GetValue<string>(Ordre))
                            .Append(Champ.ParameterName)
                            .ToString();
                    default:
                        throw new InvalidOperationException();
                }
            }
        }
    }
}