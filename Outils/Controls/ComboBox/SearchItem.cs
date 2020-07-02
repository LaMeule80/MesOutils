using System;

namespace Outils.Controls.ComboBox
{
    public class SearchItem
    {
        public Guid Id { get; set; }

        public string Libelle { get; set; }

        public bool EstFavori { get; set; }

        public int NbreElts { get; set; }

        public string LibelleAffichage
        {
            get
            {
                if (NbreElts == 0)
                    return Libelle;
                return Libelle + "  (" + NbreElts + ")";
            }
        }
        
        public SearchItem(Guid id, string libelle, bool estFavori, int nbreElts)
        {
            Id = id;
            Libelle = libelle;
            EstFavori = estFavori;
            NbreElts = nbreElts;
        }
    }
}
