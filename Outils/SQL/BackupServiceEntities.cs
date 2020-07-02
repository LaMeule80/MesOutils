using System.Configuration;
using System.IO;

namespace Outils.SQL
{
    public class BackupServiceEntities
    {
        public string Dossier { get; set; }
        
        public string Nom { get; set; }

        public string Requete => GetRequete();

        public string GetRequete()
        {
            var path = Path.Combine(Dossier, Nom);
            var requete = "BACKUP DATABASE " + ConfigurationManager.AppSettings["NomBase"]
                                             + @" TO DISK = N'" + path + ".bak' "
                                             + " WITH NOFORMAT, INIT, NAME = N'BaseActrice-Full Database Backup', SKIP, NOREWIND, NOUNLOAD,  STATS = 10 ";
            return requete;
        }
    }
}