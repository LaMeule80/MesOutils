using Outils.Helper.EnumTools;

namespace Outils.ObjectResultData
{
    public enum Level
    {
        [EnumValue("erreur(s)", 1)]
        Erreur,
        [EnumValue("warning(s)", 1)]
        Attention,
        [EnumValue("information(s)", 1)]
        Information
    }
}