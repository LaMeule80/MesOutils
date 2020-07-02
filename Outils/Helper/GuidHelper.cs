using System;

namespace Outils.Helper
{
    public class GuidHelper
    {
        public static bool IsNullOrEmpty(Guid? guid)
        {
            return !guid.HasValue || guid.Equals(Guid.Empty);
        }

        public static bool IsEqual(Guid? guid1, Guid? guid2)
        {
            return (!guid1.HasValue || guid1.Equals(Guid.Empty)) && (!guid2.HasValue || guid2.Equals(Guid.Empty)) ||
                   guid1.HasValue && !guid1.Equals(Guid.Empty) && guid2.HasValue && !guid2.Equals(Guid.Empty) &&
                   guid1.Equals(guid2);
        }
    }
}