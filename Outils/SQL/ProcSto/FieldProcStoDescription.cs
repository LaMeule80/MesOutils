// ---------------------------------------------------------------------------
//  This source file is the confidential property and copyright of WIUZ.
//  Reproduction or transmission in whole or in part, in any form or 
//  by any means, electronic, mechanical or otherwise, is prohibited
//  without the prior written consent of the copyright owner.
//  <copyright file="FieldProcStoDescription.cs" company="WIUZ">
//     Copyright (C) WIUZ.  All rights reserved. 2016 - 2019
//  </copyright>
//  History:
//   2019/04/10 at 11:08:  aka Jérôme M. 
// --------------------------------------------------------------------------- 

using System.Globalization;

namespace Outils.SQL
{
    public class FieldProcStoDescription
    {
        private readonly ProsStoBase _table;

        public FieldProcStoDescription(string name, ProsStoBase table)
        {
            Name = name;
            _table = table;
            _table.Add(this);
        }

        public string ParameterName => string.Format(CultureInfo.InvariantCulture, "@{0}_{1}_{2}", new object[]
        {
            _table.Schema.Name,
            _table.Name,
            Name
        });

        public string SqlName => string.Format(CultureInfo.InvariantCulture, $"[{_table.Schema.Name}].[{_table.Name}].[{Name}]");

        public string Name { get; }
    }
}