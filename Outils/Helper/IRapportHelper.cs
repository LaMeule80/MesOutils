using System.Collections.Generic;
using Outils.ObjectResultData;

namespace Outils.Helper
{
    public interface IRapportHelper
    {
        bool FichierExiste { get; }
        string NomFichier { get; }
        List<string> Result { get; }

        void Add(string titre, IEnumerable<string> items, ObjectResult objectResult = null);
        void Add(string titre, int? compteur);
        void Add(string titre, ObjectResult<Dictionary<string, List<string>>> items);
        void Add(string titre, ObjectResult<List<string>> items);
        void Create();
        string DonneTitre(string titre);
    }
}