// ***************************************************************
// <copyright file="TextToken.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters.Internal.Text
{
    internal enum TextTokenId : byte
    {
        None = 0,
        EndOfFile = TokenId.EndOfFile,
        Text = TokenId.Text,
        EncodingChange = TokenId.EncodingChange,
    }


    internal enum TextRunKind : uint
    {
        Invalid = 0,

        Text = RunKind.Text,
        QuotingLevel = (10u << 24),
    }


    internal class TextToken : Token
    {

        public TextToken()
        {
            // this.Reset();
        }


        public new TextTokenId TokenId => (TextTokenId)tokenId;
    }
}

