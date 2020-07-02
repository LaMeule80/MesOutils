using Outils.Helper.EnumTools;

namespace Outils.SQL
{
    public enum OrdreLogique
    {
        [EnumValue("=")]
        Egal,
        [EnumValue("IS NULL")]
        Null,
        [EnumValue("IN")]
        In,
    }
}