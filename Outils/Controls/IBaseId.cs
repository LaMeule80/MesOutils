using System;

namespace ApplicationActrice
{
    public interface IBaseId
    {
        Guid Id { get; set; }

        string Nom { get; set; }
    }
}