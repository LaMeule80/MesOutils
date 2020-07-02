using System;
using Outils.ObjectResultData;

namespace OutilsBlazor
{
    public class Service : Outils.IService
    {
        public static Service Instance => new Service();

        public Service()
        {
            ObjectResult = new ObjectResult();
        }

        public void Erreur(string message)
        {
            throw new NotImplementedException();
        }

        public void Warning(string message)
        {
            throw new NotImplementedException();
        }

        public void Information(string message)
        {
            throw new NotImplementedException();
        }

        public bool AskQuestion(string message)
        {
            throw new NotImplementedException();
        }

        public void ShowException(Exception exception)
        {
            throw new NotImplementedException();
        }

        public void OuvrirLienInterne(string lien)
        {
            AfficherHelper.OuvrirLienInterne(lien);
        }

        public void OuvrirLienExterne(string lien)
        {
            AfficherHelper.OuvrirLienExterne(lien);
        }

        public ObjectResult ObjectResult { get; }
    }
}