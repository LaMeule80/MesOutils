using System;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Media.Imaging;

namespace Outils.SQL
{
    public class AccessReaderResult : DisposableBase
    {
        private SqlDataReader _reader;
        
        internal void Initialisation(SqlDataReader reader)
        {
            _reader = reader;
        }

        internal bool Read()
        {
            return _reader.Read();
        }

        public string GetStringValue(FieldViewDescription champ)
        {
            if (_reader[champ.Name] == DBNull.Value)
                return null;
            return (string)_reader[champ.Name];
        }

        public T? GetNullableValue<T>(FieldViewDescription champ) where T : struct
        {
            if (_reader[champ.Name] == DBNull.Value)
                return null;
            return (T)_reader[champ.Name];
        }

        public T GetValue<T>(FieldViewDescription champ)
        {
            return GetValue<T>(champ.Name);
        }

        public T GetValue<T>(string champ)
        {
            if (_reader[champ] == DBNull.Value)
                return default;
            return (T)_reader[champ];
        }

        public BitmapImage GetBitmapImage(FieldViewDescription champ)
        {
            var bitmap = new BitmapImage();
            var tableau = GetValue<byte[]>(champ.Name);
            if (tableau != null && tableau.Length != 0)
                using (var memoryStream = new MemoryStream(tableau))
                {
                    bitmap.BeginInit();
                    bitmap.StreamSource = memoryStream;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                }
            return bitmap;
        }

        protected override void DisposeManagedResources()
        {
            _reader?.Dispose();
        }
    }
}