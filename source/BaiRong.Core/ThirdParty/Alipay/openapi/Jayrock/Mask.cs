#region License, Terms and Conditions
//
// Jayrock - JSON and JSON-RPC for Microsoft .NET Framework and Mono
// Written by Atif Aziz (atif.aziz@skybow.com)
// Copyright (c) 2005 Atif Aziz. All rights reserved.
//
// This library is free software; you can redistribute it and/or modify it under
// the terms of the GNU Lesser General Public License as published by the Free
// Software Foundation; either version 2.1 of the License, or (at your option)
// any later version.
//
// This library is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
// details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this library; if not, write to the Free Software Foundation, Inc.,
// 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 
//
#endregion

namespace Jayrock
{
    #region Imports

    using System;

    #endregion

    /// <summary>
    /// Provides masking services where one value masks another given a test.
    /// </summary>

    internal sealed class Mask
    {
        public static string NullString(string actual) 
        {
            return actual == null ? string.Empty : actual;
        }

        public static string NullString(string actual, string mask) 
        {
            return actual == null ? mask : actual;
        }

        public static string EmptyString(string actual, string emptyValue) 
        {
            return Mask.NullString(actual).Length == 0 ? emptyValue : actual;
        }

        private Mask()
        {
            throw new NotSupportedException();
        }
    }
}
