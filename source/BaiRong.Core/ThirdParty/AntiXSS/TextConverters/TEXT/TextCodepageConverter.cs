// ***************************************************************
// <copyright file="TextCodePageConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters.Internal.Text
{
    using System;
    using System.Text;


    internal class TextCodePageConverter : IProducerConsumer, IDisposable
    {
        ////////////////////////////////////////////////////////////

        protected ConverterInput input;
        protected bool endOfFile;
        protected bool gotAnyText;

        protected ConverterOutput output;


        public TextCodePageConverter(ConverterInput input, ConverterOutput output)
        {
            this.input = input;
            this.output = output;
        }



        public void Run()
        {
            if (endOfFile)
            {
                return;
            }

            char[] buffer = null;
            var start = 0;
            var current = 0;
            var end = 0;

            if (!input.ReadMore(ref buffer, ref start, ref current, ref end))
            {
                // cannot decode more data until next input chunk is available

                return;
            }

            if (input.EndOfFile)
            {
                endOfFile = true;
            }

            if (end - start != 0)
            {
                if (!gotAnyText)
                {
                    if (output is ConverterEncodingOutput)
                    {
                        var encodingOutput = output as ConverterEncodingOutput;

                        if (encodingOutput.CodePageSameAsInput)
                        {
 
                            if (input is ConverterDecodingInput)
                            {
                                encodingOutput.Encoding = (input as ConverterDecodingInput).Encoding;
                            }
                            else
                            {
                                encodingOutput.Encoding = Encoding.UTF8;
                            }

                        }

                    }

                    gotAnyText = true;

                }

                output.Write(buffer, start, end - start);

                input.ReportProcessed(end - start);

            }

            if (endOfFile)
            {
                output.Flush();
            }

        }



        public bool Flush()
        {
            if (!endOfFile)
            {
                Run();
            }

            return endOfFile;
        }


        void IDisposable.Dispose()
        {
            if (input != null /*&& this.input is IDisposable*/)
            {
                ((IDisposable)input).Dispose();
            }

            if (output != null /*&& this.output is IDisposable*/)
            {
                ((IDisposable)output).Dispose();
            }

            input = null;
            output = null;

            GC.SuppressFinalize(this);
        }

    }

}
