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

namespace Jayrock.Diagnostics
{
    #region Imports

    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Text;

    #endregion

    /// <summary>
    /// This type supports the Jayrock infrastructure and is not intended to 
    /// be used directly from your code.
    /// </summary>

    public sealed class DebugString
    {
        public static readonly string Ellipsis = "\x2026";
        public static readonly char ControlReplacement = '?';
        
        public static string Format(string s)
        {
            return Format(s, 50);
        }

        public static string Format(string s, int width)
        {
            Debug.Assert(width > Ellipsis.Length);
            
            if (s == null)
                return string.Empty;
            
            StringBuilder sb = new StringBuilder(width);

            for (int i = 0; i < Math.Min(width, s.Length); i++)
            {
                sb.Append(!Char.IsControl(s, i) ? s[i] : ControlReplacement);
            }
            
            if (s.Length > width)
            {
                sb.Remove(width - Ellipsis.Length, Ellipsis.Length);
                sb.Append(Ellipsis);
            }
            
            return sb.ToString();
        }

        private DebugString()
        {
            throw new NotSupportedException();
        }
    }
}
