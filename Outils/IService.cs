using System;
using Outils.ObjectResultData;

namespace Outils
{
    public interface IService
    {
        void Erreur(string message);

        void Warning(string message);

        void Information(string message);

        bool AskQuestion(string message);

        void ShowException(Exception exception);

        void OuvrirLienInterne(string lien);
        
        void OuvrirLienExterne(string lien);

        ObjectResult ObjectResult { get; }
    }
}
