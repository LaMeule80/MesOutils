using Outils.ObjectResultData;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Outils.Helper;

namespace Outils.SQL
{
    public class AccessCommand : DisposableBase
    {
        private readonly SqlCommand _cmd;

        private readonly ObjectResult _result;

        private SqlConnection _connection;

        public ObjectResult ObjectResult { get; }

        public string Requete
        {
            get
            {
                string requete = _cmd.CommandText;
                if (_cmd.Parameters.Count > 0)
                    foreach (SqlParameter item in _cmd.Parameters)
                        requete = requete.Replace(item.ParameterName, $"'{item.Value}'");
                return requete;
            }
        }

        public AccessCommand(ObjectResult result, string requete, CommandType commandType)
        {
            var sqlConnectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = ConfigurationManager.AppSettings["DataSource"],
                InitialCatalog = ConfigurationManager.AppSettings["NomBase"],
                IntegratedSecurity = true
            };

            ObjectResult = new ObjectResult();

            _connection = new SqlConnection(sqlConnectionStringBuilder.ConnectionString);
            _connection.Open();

            _cmd = new SqlCommand
            {
                Connection = _connection,
                CommandText = requete,
                CommandType = commandType
            };
            _result = result;
        }

        public void Add(FieldViewDescription champ, object valeur)
        {
            Add(champ.ParameterName, valeur);
        }
        
        public void Add(string name, object valeur)
        {
            if (valeur == null)
                _cmd.Parameters.AddWithValue(name, DBNull.Value);
            else if (Guid.TryParse(valeur.ToString(), out Guid guidValeur))
                _cmd.Parameters.AddWithValue(name, GuidHelper.IsNullOrEmpty(guidValeur) ? DBNull.Value : valeur);
            else
                _cmd.Parameters.AddWithValue(name, valeur);
        }
        
        public AccessReaderResult ExecuteReader()
        {
            _connection = _cmd.Connection;
            var accessReaderResult = new AccessReaderResult();

            try
            {
                Debug.WriteLine(Requete);
                accessReaderResult.Initialisation(_cmd.ExecuteReader());
            }
            catch (SqlException sqlException)
            {
                _result.AddError(sqlException.Message);
            }

            return accessReaderResult;
        }

        public void ExecuteNonQuery()
        {
            try
            {
                _cmd.ExecuteNonQuery();
            }
            catch (SqlException sqlException)
            {
                _result.AddError(sqlException);
            }
        }

        protected override void DisposeManagedResources()
        {
            _cmd?.Dispose();

            _connection?.Dispose();

            base.DisposeManagedResources();
        }
    }
}