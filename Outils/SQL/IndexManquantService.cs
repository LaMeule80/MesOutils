using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using NLog;

namespace Outils.SQL
{
    public static class IndexManquantService
    {
        private static readonly string _requete = @"SELECT TOP 25
                                    'USE '+QUOTENAME(DB_name(dm_mid.database_id))+' CREATE INDEX [IX_TEMP_' + OBJECT_NAME(dm_mid.OBJECT_ID,dm_mid.database_id) + '_'
                                    + REPLACE(REPLACE(REPLACE(ISNULL(dm_mid.equality_columns,''),', ','_'),'[',''),']','') 
                                    + CASE
                                    WHEN dm_mid.equality_columns IS NOT NULL
                                    AND dm_mid.inequality_columns IS NOT NULL THEN '_'
                                    ELSE ''
                                    END
                                    + REPLACE(REPLACE(REPLACE(ISNULL(dm_mid.inequality_columns,''),', ','_'),'[',''),']','')
                                    + ']'
                                    + ' ON ' + dm_mid.statement
                                    + ' (' + ISNULL (dm_mid.equality_columns,'')
                                    + CASE WHEN dm_mid.equality_columns IS NOT NULL AND dm_mid.inequality_columns 
                                    IS NOT NULL THEN ',' ELSE
                                    '' END
                                    + ISNULL (dm_mid.inequality_columns, '')
                                    + ')'
                                    + ISNULL (' INCLUDE (' + dm_mid.included_columns + ')', '') AS Create_Statement,
                                    dm_migs.avg_user_impact*(dm_migs.user_seeks+dm_migs.user_scans) Avg_Estimated_Impact,
                                    dm_migs.last_user_seek AS Last_User_Seek,
                                    OBJECT_NAME(dm_mid.OBJECT_ID,dm_mid.database_id) AS [TableName]
                                    FROM sys.dm_db_missing_index_groups dm_mig
                                    INNER JOIN sys.dm_db_missing_index_group_stats dm_migs
                                    ON dm_migs.group_handle = dm_mig.index_group_handle
                                    INNER JOIN sys.dm_db_missing_index_details dm_mid
                                    ON dm_mig.index_handle = dm_mid.index_handle
                                    WHERE dm_mid.database_ID = DB_ID()
                                    ORDER BY Avg_Estimated_Impact DESC
                                    ";

        public static void Execute()
        {
            using (var worker = new BackgroundWorker())
            {
                worker.DoWork += worker_DoWork;
                worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
                worker.RunWorkerAsync();
            }
        }

        private static void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var result = e.Result as string;
            if (!string.IsNullOrEmpty(result))
                LogManager.GetCurrentClassLogger().Info(result);
        }

        private static void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var sqlConnectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = ConfigurationManager.AppSettings["DataSource"],
                InitialCatalog = ConfigurationManager.AppSettings["NomBase"],
                IntegratedSecurity = true
            };

            List<string> result = new List<string>();

            using (var connection = new SqlConnection(sqlConnectionStringBuilder.ConnectionString))
            {
                connection.Open();
                using (var cmd = new SqlCommand(_requete, connection))
                {
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var item = (string)reader["Create_Statement"];
                        result.Add(item);
                    }
                }
            }

            if (result.Count > 0)
            {
                foreach (var cmdText in result)
                {
                    try
                    {
                        using (var connection = new SqlConnection(sqlConnectionStringBuilder.ConnectionString))
                        {
                            connection.Open();
                            using (var cmd = new SqlCommand(cmdText, connection))
                            {
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        result.Add(exception.Message);
                    }
                }
            }

            e.Result = string.Join(Environment.NewLine, result);
        }
    }
}
