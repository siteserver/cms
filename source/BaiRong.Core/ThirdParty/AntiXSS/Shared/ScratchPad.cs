// ***************************************************************
// <copyright file="ScratchPad.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.Internal
{
    using System;
    using System.Threading;
    using System.Text;
    using System.Diagnostics;

    internal static class ScratchPad
    {
        private static LocalDataStoreSlot scratchPadTlsSlot = Thread.AllocateDataSlot();

        public static void Begin()
        {
            var pad = (ScratchPadContainer)Thread.GetData(scratchPadTlsSlot);
            if (pad == null)
            {
                pad = new ScratchPadContainer();

                Thread.SetData(scratchPadTlsSlot, pad);
            }
            else
            {
                pad.AddRef();
            }
        }

        public static void End()
        {
            var pad = (ScratchPadContainer)Thread.GetData(scratchPadTlsSlot);

            InternalDebug.Assert(pad != null);

            if (pad != null)
            {
                if (pad.Release())
                {
                    
                    Thread.SetData(scratchPadTlsSlot, null);
                }
            }
        }

        public static byte[] GetByteBuffer(int size)
        {
            var pad = (ScratchPadContainer)Thread.GetData(scratchPadTlsSlot);

            if (pad == null)
            {
                return new byte[size];
            }
               
            return pad.GetByteBuffer(size);
        }

        [Conditional("DEBUG")] 
        public static void ReleaseByteBuffer()
        {
            var pad = (ScratchPadContainer)Thread.GetData(scratchPadTlsSlot);

            if (pad != null)
            {
                pad.ReleaseByteBuffer();
            }
        }

        public static char[] GetCharBuffer(int size)
        {
            var pad = (ScratchPadContainer)Thread.GetData(scratchPadTlsSlot);

            if (pad == null)
            {
                return new char[size];
            }
               
            return pad.GetCharBuffer(size);
        }

        [Conditional("DEBUG")] 
        public static void ReleaseCharBuffer()
        {
            var pad = (ScratchPadContainer)Thread.GetData(scratchPadTlsSlot);

            if (pad != null)
            {
                pad.ReleaseCharBuffer();
            }
        }

        public static StringBuilder GetStringBuilder()
        {
            
            return GetStringBuilder(16);
        }

        public static StringBuilder GetStringBuilder(int initialCapacity)
        {
            var pad = (ScratchPadContainer)Thread.GetData(scratchPadTlsSlot);

            if (pad == null)
            {
                return new StringBuilder(initialCapacity);
            }
               
            return pad.GetStringBuilder(initialCapacity);
        }

        
        public static void ReleaseStringBuilder()
        {
            var pad = (ScratchPadContainer)Thread.GetData(scratchPadTlsSlot);

            if (pad != null)
            {
                pad.ReleaseStringBuilder();
            }
        }

        private class ScratchPadContainer
        {
            public const int ScratchStringBuilderCapacity = 512;

            private int refCount;

            private byte[] byteBuffer;
            private char[] charBuffer;
            private StringBuilder stringBuilder;
#if DEBUG
            private bool byteBufferUsed;
            private bool charBufferUsed;
            private bool stringBuilderUsed;
#endif
            public ScratchPadContainer()
            {
                refCount = 1;
            }

            public void AddRef()
            {
                InternalDebug.Assert(refCount > 0);
                refCount ++;
            }

            public bool Release()
            {
                InternalDebug.Assert(refCount > 0);

                refCount --;
                return refCount == 0;
            }

            public byte[] GetByteBuffer(int size)
            {
#if DEBUG
                InternalDebug.Assert(!byteBufferUsed);
                byteBufferUsed = true;
#endif
                if (byteBuffer == null || byteBuffer.Length < size)
                {
                    byteBuffer = new byte[size];
                }

                return byteBuffer;
            }

            public void ReleaseByteBuffer()
            {
#if DEBUG
                InternalDebug.Assert(byteBufferUsed);
                byteBufferUsed = false;
#endif
            }

            public char[] GetCharBuffer(int size)
            {
#if DEBUG
                InternalDebug.Assert(!charBufferUsed);
                charBufferUsed = true;
#endif
                if (charBuffer == null || charBuffer.Length < size)
                {
                    charBuffer = new char[size];
                }

                return charBuffer;
            }

            public void ReleaseCharBuffer()
            {
#if DEBUG
                InternalDebug.Assert(charBufferUsed);
                charBufferUsed = false;
#endif
            }

            public StringBuilder GetStringBuilder(int initialCapacity)
            {
#if DEBUG
                InternalDebug.Assert(!stringBuilderUsed);
                stringBuilderUsed = true;
#endif
                if (initialCapacity <= ScratchStringBuilderCapacity)
                {
                    if (stringBuilder == null)
                    {
                        stringBuilder = new StringBuilder(ScratchStringBuilderCapacity);
                    }
                    else
                    {
                        InternalDebug.Assert(stringBuilder.Capacity == ScratchStringBuilderCapacity);
                        stringBuilder.Length = 0; 
                    }

                    return stringBuilder;
                }

                return new StringBuilder(initialCapacity);
            }

            public void ReleaseStringBuilder()
            {
#if DEBUG
                InternalDebug.Assert(stringBuilderUsed);
                stringBuilderUsed = false;
#endif
                if (stringBuilder != null &&
                    (stringBuilder.Capacity > ScratchStringBuilderCapacity ||
                    stringBuilder.Length * 2 >= stringBuilder.Capacity + 1))
                {
                    
                    
                    
                    stringBuilder = null;
                }
            }
        }
    }
}

