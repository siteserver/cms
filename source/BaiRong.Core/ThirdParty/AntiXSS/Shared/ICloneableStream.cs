// ***************************************************************
// <copyright file="ICloneableStream.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.Internal
{
    using System.IO;

    internal interface ICloneableStream
    {
        Stream Clone();
    }
}
