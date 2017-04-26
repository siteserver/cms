// ***************************************************************
// <copyright file="Injection.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters
{
    using System;
    using System.IO;
    using Internal.Text;


    internal abstract class Injection : IDisposable
    {
        protected HeaderFooterFormat injectionFormat;

        protected string injectHead;
        protected string injectTail;

        protected bool headInjected;
        protected bool tailInjected;

        protected bool testBoundaryConditions;
        protected Stream traceStream;

        public HeaderFooterFormat HeaderFooterFormat => injectionFormat;

        public bool HaveHead => injectHead != null;
        public bool HaveTail => injectTail != null;

        public bool HeadDone => headInjected;
        public bool TailDone => tailInjected;

        public abstract void Inject(bool head, TextOutput output);

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        public virtual void Reset()
        {
            headInjected = false;
            tailInjected = false;
        }

        
        
        public abstract void InjectRtfFonts(int firstAvailableFontHandle);
        public abstract void InjectRtfColors(int nextColorIndex);
    }
}

