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

    internal sealed class BooleanObject
    {
        //
        // The following two statics are only used as an optimization so that we
        // don't create a boxed Boolean each time an Object is expecting somewhere.
        // This should help put a little less pressure on the GC where possible.
        //

        public readonly static object True = true;
        public readonly static object False = false;
        
        public static object Box(bool value)
        {
            return value ? True : False;
        }
        
        private BooleanObject()
        {
            throw new NotSupportedException();
        }
    }
}
