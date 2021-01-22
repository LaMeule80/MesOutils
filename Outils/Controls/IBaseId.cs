using System;

namespace Outils.Controls
{
    public interface IBaseId
    {
        Guid Id { get; set; }

        string Nom { get; set; }
    }
}