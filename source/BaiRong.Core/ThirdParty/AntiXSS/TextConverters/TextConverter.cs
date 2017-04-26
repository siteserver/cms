// ***************************************************************
// <copyright file="TextConverter.cs" company="Microsoft">
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
    using Strings = CtsResources.TextConvertersStrings;

    
    
    internal interface IResultsFeedback
    {
        
        
        
        void Set(ConfigParameter parameterId, object val);
    }

    
    
    internal enum ConfigParameter : int
    {
        
        InputEncoding,
        OutputEncoding,
        RtfCompressionMode,
        RtfEncapsulation,
    }


    
    
    
    
    internal abstract class TextConverter : IResultsFeedback
    {
        internal bool testBoundaryConditions = false;
        internal int inputBufferSize = 4096;
        internal int outputBufferSize = 4096;
        internal bool locked;

        

        
        
        
        internal TextConverter()
        {
        }

        internal bool TestBoundaryConditions
        {
            get { return testBoundaryConditions; }
            set { AssertNotLocked(); testBoundaryConditions = value; }
        }

        
        
        
        
        
        
        
        
        
        
        
        
        public int InputStreamBufferSize
        {
            get { return inputBufferSize; }
            set
            {
                AssertNotLocked();

                if (value < 1024 || value > 80 * 1024)
                {
                    throw new ArgumentOutOfRangeException("value", Strings.BufferSizeValueRange);
                }

                inputBufferSize = value;
            }
        }

        
        
        
        
        
        
        
        
        
        
        
        
        public int OutputStreamBufferSize
        {
            get { return outputBufferSize; }
            set
            {
                AssertNotLocked();

                if (value < 1024 || value > 80 * 1024)
                {
                    throw new ArgumentOutOfRangeException("value",Strings.BufferSizeValueRange);
                }

                outputBufferSize = value;
            }
        }

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        public void Convert(Stream sourceStream, Stream destinationStream)
        {
            if (destinationStream == null)
            {
                throw new ArgumentNullException("destinationStream");
            }

            

            Stream converter = new ConverterStream(sourceStream, this, ConverterStreamAccess.Read);

            var buf = new byte[outputBufferSize];

            while (true)
            {
                var cnt = converter.Read(buf, 0, buf.Length);
                if (0 == cnt)
                {
                    break;
                }

                destinationStream.Write(buf, 0, cnt);
            }

            destinationStream.Flush();
        }

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        public void Convert(Stream sourceStream, TextWriter destinationWriter)
        {
            if (destinationWriter == null)
            {
                throw new ArgumentNullException("destinationWriter");
            }
            

            TextReader converter = new ConverterReader(sourceStream, this);

            var buf = new char[4096];

            while (true)
            {
                var cnt = converter.Read(buf, 0, buf.Length);
                if (0 == cnt)
                {
                    break;
                }

                destinationWriter.Write(buf, 0, cnt);
            }

            destinationWriter.Flush();
        }

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        public void Convert(TextReader sourceReader, Stream destinationStream)
        {
            if (destinationStream == null)
            {
                throw new ArgumentNullException("destinationStream");
            }
            

            Stream converter = new ConverterStream(sourceReader, this);

            var buf = new byte[outputBufferSize];

            while (true)
            {
                var cnt = converter.Read(buf, 0, buf.Length);
                if (0 == cnt)
                {
                    break;
                }

                destinationStream.Write(buf, 0, cnt);
            }

            destinationStream.Flush();
        }

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        public void Convert(TextReader sourceReader, TextWriter destinationWriter)
        {
            if (destinationWriter == null)
            {
                throw new ArgumentNullException("destinationWriter");
            }
            

            TextReader converter = new ConverterReader(sourceReader, this);

            var buf = new char[4096];

            while (true)
            {
                var cnt = converter.Read(buf, 0, buf.Length);
                if (0 == cnt)
                {
                    break;
                }

                destinationWriter.Write(buf, 0, cnt);
            }

            destinationWriter.Flush();
        }

        

        internal abstract IProducerConsumer CreatePushChain(ConverterStream converterStream, Stream output);
        internal abstract IProducerConsumer CreatePushChain(ConverterStream converterStream, TextWriter output);
        internal abstract IProducerConsumer CreatePushChain(ConverterWriter converterWriter, Stream output);
        internal abstract IProducerConsumer CreatePushChain(ConverterWriter converterWriter, TextWriter output);

        internal abstract IProducerConsumer CreatePullChain(Stream input, ConverterStream converterStream);
        internal abstract IProducerConsumer CreatePullChain(TextReader input, ConverterStream converterStream);
        internal abstract IProducerConsumer CreatePullChain(Stream input, ConverterReader converterReader);
        internal abstract IProducerConsumer CreatePullChain(TextReader input, ConverterReader converterReader);

        

        internal virtual void SetResult(ConfigParameter parameterId, object val)
        {
        }

        void IResultsFeedback.Set(ConfigParameter parameterId, object val)
        {
            SetResult(parameterId, val);
        }

        internal void AssertNotLocked()
        {
            if (locked)
            {
                throw new InvalidOperationException(Strings.ParametersCannotBeChangedAfterConverterObjectIsUsed);
            }
        }
    }
}

