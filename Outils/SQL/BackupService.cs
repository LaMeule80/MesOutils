using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;

namespace Outils.SQL
{
    public static class BackupService
    {
        public static void Execute(List<BackupServiceEntities> backupServiceEntitieses = null)
        {
            using (var worker = new BackgroundWorker())
            {
                worker.DoWork += worker_DoWork;
                worker.RunWorkerAsync(backupServiceEntitieses);
            }
        }

        private static void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var backupServiceEntitieses = e.Argument as List<BackupServiceEntities>;

            var sqlConnectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = ConfigurationManager.AppSettings["DataSource"],
                InitialCatalog = ConfigurationManager.AppSettings["NomBase"],
                IntegratedSecurity = true
            };

            foreach (var item in backupServiceEntitieses)
            {
                using (var connection = new SqlConnection(sqlConnectionStringBuilder.ConnectionString))
                {
                    connection.Open();
                    using (var cmd = new SqlCommand(item.Requete, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}