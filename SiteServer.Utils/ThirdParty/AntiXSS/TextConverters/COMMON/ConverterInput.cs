// ***************************************************************
// <copyright file="ConverterInput.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters
{
    using System;


    internal abstract class ConverterInput : IDisposable
    {
        protected bool endOfFile;
        protected int maxTokenSize;

        protected IProgressMonitor progressMonitor;

        
        
        public bool EndOfFile => endOfFile;


        public int MaxTokenSize => maxTokenSize;

        protected ConverterInput(IProgressMonitor progressMonitor)
        {
            this.progressMonitor = progressMonitor;
        }

        
        
        public virtual void SetRestartConsumer(IRestartable restartConsumer)
        {
        }

        
        
        
        
        
        
        public abstract bool ReadMore(ref char[] buffer, ref int start, ref int current, ref int end);

        
        
        public abstract void ReportProcessed(int processedSize);

        
        
        
        
        public abstract int RemoveGap(int gapBegin, int gapEnd);

        
        void IDisposable.Dispose()
        {
            Dispose();
            GC.SuppressFinalize(this);
        }

        
        protected virtual void Dispose()
        {
        }
    }
}
