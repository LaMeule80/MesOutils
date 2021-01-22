using System;
using Outils.ObjectResultData;

namespace Outils
{
    public interface IService
    {
        void Erreur(string message);

        void Warning(string message);

        void Information(string message);

        void OuvrirLien(string lien);

        bool AskQuestion(string nom);

        ObjectResult ObjectResult { get; }

        void Show(ObjectResult objectResult);
    }
}
