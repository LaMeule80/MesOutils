using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Outils.ObjectResultData
{
    public class ObjectResult<T> : ObjectResult
    {
        public T Value { get; set; }

        public ObjectResult<List<T>> Creer()
        {
            return new ObjectResult<List<T>>
            {
                Value = new List<T>()
            };
        }
    }

    public class ObjectResult : List<ObjectResultItem>
    {
        public bool IsSuccess
        {
            get
            {
                if (!this.Any())
                    return true;
                return this.Count(x => x.Niveau == Level.Erreur) <= 0;
            }
        }

        public string Message
        {
            get
            {
                if (this.Count == 0)
                    return string.Empty;
                
                var stringBuilder = new StringBuilder();
                var erreurs = this.Where(x => x.Niveau == Level.Erreur);
                if (erreurs.Any())
                {
                    stringBuilder.Append("Une ou plusieurs erreurs ont été levées : ").Append(Environment.NewLine);
                    foreach (var item in erreurs)
                        stringBuilder.Append(item.Message).Append(Environment.NewLine);
                }

                var warning = this.Where(x => x.Niveau == Level.Attention);
                if (warning.Any())
                {
                    stringBuilder.Append("Un ou plusieurs warnings ont été levées : ").Append(Environment.NewLine);
                    foreach (var item in warning)
                        stringBuilder.Append(item).Append(Environment.NewLine);
                }

                var infos = this.Where(x => x.Niveau == Level.Information);
                if (infos.Any())
                {
                    stringBuilder.Append("Un ou plusieurs informations² ont été levées : ").Append(Environment.NewLine);
                    foreach (var item in infos)
                        stringBuilder.Append(item).Append(Environment.NewLine);
                }

                return stringBuilder.ToString();
            }
        }

        public void AddError(string message)
        {
            Add(new ObjectResultItem() { Message = message, Niveau = Level.Erreur });
        }

        public void AddWarning(string message)
        {
            Add(new ObjectResultItem() { Message = message, Niveau = Level.Attention });
        }

        public void AddInformation(string message)
        {
            Add(new ObjectResultItem() { Message = message, Niveau = Level.Information });
        }

        public void AddError(Exception exception)
        {
            var stackTrace = new StackTrace();
            var method = stackTrace.GetFrame(2).GetMethod();
            var stringBuilder = new StringBuilder(string.Format(CultureInfo.CurrentCulture,
                "{0}.{1} : Une erreur SQL a été levée.", new object[]
                {
                    method.ReflectedType?.Name,
                    method.Name
                }));
            stringBuilder.Append(Environment.NewLine);
            stringBuilder.Append("Informations complémentaires :");
            stringBuilder.Append(Environment.NewLine);
            stringBuilder.Append(exception.Message);
            AddError(stringBuilder.ToString());
        }

        public void Merge(ObjectResult objectResult)
        {
            AddRange(objectResult);
        }
    }

    public class ObjectResultItem
    {
        public ObjectResultItem()
        {
            
        }

        public ObjectResultItem(string message, Level niveau)
        {
            Message = message;
            Niveau = niveau;
        }

        public string Message { get; set; }

        public Level Niveau { get; set; }
    }

    public enum Level
    {
        Erreur,
        Attention,
        Information
    }
}