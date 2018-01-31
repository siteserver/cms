// ***************************************************************
// <copyright file="HtmlToHtml.cs" company="Microsoft">
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
    using System.Text;
    using Internal.Html;
    using Strings = CtsResources.TextConvertersStrings;

    
    
    
    
    internal class HtmlToHtml : TextConverter
    {
        private Encoding inputEncoding = null;
        private bool detectEncodingFromByteOrderMark = true;
        private bool detectEncodingFromMetaTag = true;

        private Encoding outputEncoding = null;
        private bool outputEncodingSameAsInput = true;

        private bool normalizeInputHtml = false;

        private HeaderFooterFormat injectionFormat = HeaderFooterFormat.Text;
        private string injectHead = null;
        private string injectTail = null;

        

        private bool filterHtml = false;
        private HtmlTagCallback htmlCallback = null;
        
        private bool testTruncateForCallback = true;

        private bool testConvertFragment;
        private bool outputFragment;

        private int testMaxTokenRuns = 512;

        private Stream testTraceStream = null;
        private bool testTraceShowTokenNum = true;
        private int testTraceStopOnTokenNum = 0;

        private Stream testNormalizerTraceStream = null;
        private bool testNormalizerTraceShowTokenNum = true;
        private int testNormalizerTraceStopOnTokenNum = 0;

        private int maxHtmlTagSize = 32768;

        private int testMaxHtmlTagAttributes = 64;
        private int testMaxHtmlRestartOffset = 4096;

        
        private int testMaxHtmlNormalizerNesting = HtmlSupport.HtmlNestingLimit;

        private int smallCssBlockThreshold = -1;        
        private bool preserveDisplayNoneStyle = false;  

        private bool testNoNewLines;

        

        
        
        
        
        
        
        
        public HtmlToHtml()
        {
        }

        
        

        
        
        
        
        
        
        
        
        
        
        public Encoding InputEncoding
        {
            get { return inputEncoding; }
            set { AssertNotLocked(); inputEncoding = value; }
        }

        
        
        
        
        
        
        
        public bool DetectEncodingFromByteOrderMark
        {
            get { return detectEncodingFromByteOrderMark; }
            set { AssertNotLocked(); detectEncodingFromByteOrderMark = value; }
        }

        
        
        
        
        
        
        
        public bool DetectEncodingFromMetaTag
        {
            get { return detectEncodingFromMetaTag; }
            set { AssertNotLocked(); detectEncodingFromMetaTag = value; }
        }

        
        
        
        
        
        
        
        public bool NormalizeHtml
        {
            get { return normalizeInputHtml; }
            set { AssertNotLocked(); normalizeInputHtml = value; }
        }

#if M3STUFF
        
        
        
        
        
        public IHtmlParsingCallback HtmlParsingCallback
        {
            get { return this.parsingCallback; }
            set { this.AssertNotLocked(); this.parsingCallback = value; }
        }
#endif

        
        

        
        
        
        
        
        
        
        
        
        public Encoding OutputEncoding
        {
            get { return outputEncoding; }
            set
            {
                AssertNotLocked(); 
                outputEncoding = value;
                outputEncodingSameAsInput = (value == null);
            }
        }

        
        
        
        
        
        
        
        
        
        
        
        
        public HeaderFooterFormat HeaderFooterFormat
        {
            get { return injectionFormat; }
            set { AssertNotLocked(); injectionFormat = value; }
        }

        
        
        
        
        
        
        public string Header
        {
            get { return injectHead; }
            set { AssertNotLocked(); injectHead = value; }
        }

        
        
        
        
        
        
        public string Footer
        {
            get { return injectTail; }
            set { AssertNotLocked(); injectTail = value; }
        }

        
        
        
        
        
        
        public bool OutputHtmlFragment
        {
            get { return outputFragment; }
            set { AssertNotLocked(); outputFragment = value; }
        }

        
        
        
        
        
        
        public bool FilterHtml
        {
            get { return filterHtml; }
            set { AssertNotLocked(); filterHtml = value; }
        }

        
        
        
        
        
        
        
        public HtmlTagCallback HtmlTagCallback
        {
            get { return htmlCallback; }
            set { AssertNotLocked(); htmlCallback = value; }
        }

        
        
        
        
        
        
        
        
        
        public int MaxCallbackTagLength
        {
            get { return maxHtmlTagSize; }
            set { AssertNotLocked(); maxHtmlTagSize = value; }
        }

#if M3STUFF
        
        
        
        
        
        
        public HtmlFilterTables HtmlFilterTables
        {
            get { return this.filterTables; }
            set { this.AssertNotLocked(); this.filterTables = value; }
        }
#endif

        
        

        internal HtmlToHtml SetInputEncoding(Encoding value)
        {
            InputEncoding = value;
            return this;
        }

        internal HtmlToHtml SetDetectEncodingFromByteOrderMark(bool value)
        {
            DetectEncodingFromByteOrderMark = value;
            return this;
        }

        internal HtmlToHtml SetDetectEncodingFromMetaTag(bool value)
        {
            DetectEncodingFromMetaTag = value;
            return this;
        }

        internal HtmlToHtml SetOutputEncoding(Encoding value)
        {
            OutputEncoding = value;
            return this;
        }

        internal HtmlToHtml SetNormalizeHtml(bool value)
        {
            NormalizeHtml = value;
            return this;
        }

        internal HtmlToHtml SetHeaderFooterFormat(HeaderFooterFormat value)
        {
            HeaderFooterFormat = value;
            return this;
        }

        internal HtmlToHtml SetHeader(string value)
        {
            Header = value;
            return this;
        }

        internal HtmlToHtml SetFooter(string value)
        {
            Footer = value;
            return this;
        }

        internal HtmlToHtml SetFilterHtml(bool value)
        {
            FilterHtml = value;
            return this;
        }

        internal HtmlToHtml SetHtmlTagCallback(HtmlTagCallback value)
        {
            HtmlTagCallback = value;
            return this;
        }

        internal HtmlToHtml SetTestTruncateForCallback(bool value)
        {
            testTruncateForCallback = value;
            return this;
        }

        internal HtmlToHtml SetMaxCallbackTagLength(int value)
        {
            maxHtmlTagSize = value;
            return this;
        }

        internal HtmlToHtml SetInputStreamBufferSize(int value)
        {
            InputStreamBufferSize = value;
            return this;
        }

        internal HtmlToHtml SetOutputHtmlFragment(bool value)
        {
            OutputHtmlFragment = value;
            return this;
        }

        internal HtmlToHtml SetTestConvertHtmlFragment(bool value)
        {
            testConvertFragment = value;
            return this;
        }

        internal HtmlToHtml SetTestBoundaryConditions(bool value)
        {
            testBoundaryConditions = value;
            if (value)
            {
                maxHtmlTagSize = 123;
                testMaxHtmlTagAttributes = 5;
                testMaxHtmlNormalizerNesting = 10;
            }
            return this;
        }

        internal HtmlToHtml SetTestMaxTokenRuns(int value)
        {
            testMaxTokenRuns = value;
            return this;
        }

        internal HtmlToHtml SetTestTraceStream(Stream value)
        {
            testTraceStream = value;
            return this;
        }

        internal HtmlToHtml SetTestTraceShowTokenNum(bool value)
        {
            testTraceShowTokenNum = value;
            return this;
        }

        internal HtmlToHtml SetTestTraceStopOnTokenNum(int value)
        {
            testTraceStopOnTokenNum = value;
            return this;
        }

        internal HtmlToHtml SetTestNormalizerTraceStream(Stream value)
        {
            testNormalizerTraceStream = value;
            return this;
        }

        internal HtmlToHtml SetTestNormalizerTraceShowTokenNum(bool value)
        {
            testNormalizerTraceShowTokenNum = value;
            return this;
        }

        internal HtmlToHtml SetTestNormalizerTraceStopOnTokenNum(int value)
        {
            testNormalizerTraceStopOnTokenNum = value;
            return this;
        }

        internal HtmlToHtml SetTestMaxHtmlTagAttributes(int value)
        {
            testMaxHtmlTagAttributes = value;
            return this;
        }

        internal HtmlToHtml SetTestMaxHtmlRestartOffset(int value)
        {
            testMaxHtmlRestartOffset = value;
            return this;
        }

        internal HtmlToHtml SetTestMaxHtmlNormalizerNesting(int value)
        {
            testMaxHtmlNormalizerNesting = value;
            return this;
        }

        internal HtmlToHtml SetTestNoNewLines(bool value)
        {
            testNoNewLines = value;
            return this;
        }

        internal HtmlToHtml SetSmallCssBlockThreshold(int value)
        {
            smallCssBlockThreshold = value;
            return this;
        }

        internal HtmlToHtml SetPreserveDisplayNoneStyle(bool value)
        {
            preserveDisplayNoneStyle = value;
            return this;
        }

        

        internal override IProducerConsumer CreatePushChain(ConverterStream converterStream, Stream output)
        {
            if (inputEncoding == null)
            {
                throw new InvalidOperationException(Strings.InputEncodingRequired);
            }

            ConverterInput converterIn = new ConverterDecodingInput(
                                    converterStream,
                                    true,
                                    inputEncoding,
                                    detectEncodingFromByteOrderMark,
                                    maxHtmlTagSize,
                                    testMaxHtmlRestartOffset,
                                    InputStreamBufferSize,
                                    testBoundaryConditions,
                                    this as IResultsFeedback,
                                    null);

            ConverterOutput converterOut = new ConverterEncodingOutput(
                                    output,
                                    true,       
                                    true,       
                                    outputEncodingSameAsInput ? inputEncoding : outputEncoding, 
                                    outputEncodingSameAsInput,
                                    testBoundaryConditions,
                                    this as IResultsFeedback);

            return CreateChain(converterIn, converterOut, converterStream as IProgressMonitor);
        }

        internal override IProducerConsumer CreatePushChain(ConverterStream converterStream, TextWriter output)
        {
            if (inputEncoding == null)
            {
                throw new InvalidOperationException(Strings.InputEncodingRequired);
            }

            outputEncoding = Encoding.Unicode;

            ConverterInput converterIn = new ConverterDecodingInput(
                                    converterStream,
                                    true,
                                    inputEncoding,
                                    detectEncodingFromByteOrderMark,
                                    maxHtmlTagSize,
                                    testMaxHtmlRestartOffset,
                                    InputStreamBufferSize,
                                    testBoundaryConditions,
                                    this as IResultsFeedback,
                                    null);

            ConverterOutput converterOut = new ConverterUnicodeOutput(
                                    output,
                                    true,       
                                    true);      

            return CreateChain(converterIn, converterOut, converterStream as IProgressMonitor);
        }

        internal override IProducerConsumer CreatePushChain(ConverterWriter converterWriter, Stream output)
        {
            inputEncoding = Encoding.Unicode;

            ConverterInput converterIn = new ConverterUnicodeInput(
                                    converterWriter,
                                    true,
                                    maxHtmlTagSize,
                                    testBoundaryConditions,
                                    null);

            ConverterOutput converterOut = new ConverterEncodingOutput(
                                    output,
                                    true,       
                                    false,      
                                    outputEncodingSameAsInput ? Encoding.UTF8 : outputEncoding, 
                                    outputEncodingSameAsInput,
                                    testBoundaryConditions,
                                    this as IResultsFeedback);

            return CreateChain(converterIn, converterOut, converterWriter as IProgressMonitor);
        }

        internal override IProducerConsumer CreatePushChain(ConverterWriter converterWriter, TextWriter output)
        {
            inputEncoding = Encoding.Unicode;
            outputEncoding = Encoding.Unicode;

            ConverterInput converterIn = new ConverterUnicodeInput(
                                    converterWriter,
                                    true,
                                    maxHtmlTagSize,
                                    testBoundaryConditions,
                                    null);

            ConverterOutput converterOut = new ConverterUnicodeOutput(
                                    output,
                                    true,       
                                    false);     

            return CreateChain(converterIn, converterOut, converterWriter as IProgressMonitor);
        }

        internal override IProducerConsumer CreatePullChain(Stream input, ConverterStream converterStream)
        {
            if (inputEncoding == null)
            {
                throw new InvalidOperationException(Strings.InputEncodingRequired);
            }

            ConverterInput converterIn = new ConverterDecodingInput(
                                    input,
                                    false,
                                    inputEncoding,
                                    detectEncodingFromByteOrderMark,
                                    maxHtmlTagSize,
                                    testMaxHtmlRestartOffset,
                                    InputStreamBufferSize,
                                    testBoundaryConditions,
                                    this as IResultsFeedback,
                                    converterStream as IProgressMonitor);

            ConverterOutput converterOut = new ConverterEncodingOutput(
                                    converterStream,
                                    false,      
                                    true,       
                                    outputEncodingSameAsInput ? inputEncoding : outputEncoding, 
                                    outputEncodingSameAsInput,
                                    testBoundaryConditions,
                                    this as IResultsFeedback);

            return CreateChain(converterIn, converterOut, converterStream as IProgressMonitor);
        }

        internal override IProducerConsumer CreatePullChain(TextReader input, ConverterStream converterStream)
        {
            inputEncoding = Encoding.Unicode;

            ConverterInput converterIn = new ConverterUnicodeInput(
                                    input,
                                    false,
                                    maxHtmlTagSize,
                                    testBoundaryConditions,
                                    converterStream as IProgressMonitor);

            ConverterOutput converterOut = new ConverterEncodingOutput(
                                    converterStream,
                                    false,      
                                    false,      
                                    outputEncodingSameAsInput ? Encoding.UTF8 : outputEncoding, 
                                    outputEncodingSameAsInput,
                                    testBoundaryConditions,
                                    this as IResultsFeedback);

            return CreateChain(converterIn, converterOut, converterStream as IProgressMonitor);
        }

        internal override IProducerConsumer CreatePullChain(Stream input, ConverterReader converterReader)
        {
            if (inputEncoding == null)
            {
                throw new InvalidOperationException(Strings.InputEncodingRequired);
            }

            outputEncoding = Encoding.Unicode;

            ConverterInput converterIn = new ConverterDecodingInput(
                                    input,
                                    false,
                                    inputEncoding,
                                    detectEncodingFromByteOrderMark,
                                    maxHtmlTagSize,
                                    testMaxHtmlRestartOffset,
                                    InputStreamBufferSize,
                                    testBoundaryConditions,
                                    this as IResultsFeedback,
                                    converterReader as IProgressMonitor);

            ConverterOutput converterOut = new ConverterUnicodeOutput(
                                    converterReader,
                                    false,      
                                    true);      

            return CreateChain(converterIn, converterOut, converterReader as IProgressMonitor);
        }

        internal override IProducerConsumer CreatePullChain(TextReader input, ConverterReader converterReader)
        {
            inputEncoding = Encoding.Unicode;
            outputEncoding = Encoding.Unicode;

            ConverterInput converterIn = new ConverterUnicodeInput(
                                    input,
                                    false,
                                    maxHtmlTagSize,
                                    testBoundaryConditions,
                                    converterReader as IProgressMonitor);

            ConverterOutput converterOut = new ConverterUnicodeOutput(
                                    converterReader,
                                    false,      
                                    false);     

            return CreateChain(converterIn, converterOut, converterReader as IProgressMonitor);
        }

        

        private IProducerConsumer CreateChain(ConverterInput input, ConverterOutput output, IProgressMonitor progressMonitor)
        {
            locked = true;

            HtmlInjection injection = null;

            if (injectHead != null || injectTail != null)
            {
                injection = new HtmlInjection(
                            injectHead,
                            injectTail,
                            injectionFormat,
                            filterHtml,
                            htmlCallback,
                            testBoundaryConditions,
                            null,
                            progressMonitor);

                
                
                normalizeInputHtml = true;
            }

            if (filterHtml || outputFragment || htmlCallback != null)
            {
                
                normalizeInputHtml = true;
            }

            IHtmlParser parser;

            if (normalizeInputHtml)
            {
                
                

                var preParser = new HtmlParser(
                                        input,
                                        detectEncodingFromMetaTag,
                                        false,
                                        testMaxTokenRuns,
                                        testMaxHtmlTagAttributes,
                                        testBoundaryConditions);

                parser = new HtmlNormalizingParser(
                                        preParser,
                                        injection,
                                        htmlCallback != null,      
                                        testMaxHtmlNormalizerNesting,
                                        testBoundaryConditions,
                                        testNormalizerTraceStream,
                                        testNormalizerTraceShowTokenNum,
                                        testNormalizerTraceStopOnTokenNum);
            }
            else
            {
                parser = new HtmlParser(
                                        input,
                                        detectEncodingFromMetaTag,
                                        false,
                                        testMaxTokenRuns,
                                        testMaxHtmlTagAttributes,
                                        testBoundaryConditions);
            }

            var writer = new HtmlWriter(
                                    output,
                                    filterHtml,
                                    normalizeInputHtml && !testNoNewLines);

            return new HtmlToHtmlConverter(
                                    parser,
                                    writer,
                                    testConvertFragment,
                                    outputFragment,
                                    filterHtml,
                                    htmlCallback,
                                    testTruncateForCallback,
                                    injection != null && injection.HaveTail,    
                                    testTraceStream,
                                    testTraceShowTokenNum,
                                    testTraceStopOnTokenNum,
                                    smallCssBlockThreshold,
                                    preserveDisplayNoneStyle,
                                    progressMonitor);
        }
        
        

        internal override void SetResult(ConfigParameter parameterId, object val)
        {
            switch (parameterId)
            {
                case ConfigParameter.InputEncoding:
                        inputEncoding = (Encoding) val;
                        break;

                case ConfigParameter.OutputEncoding:
                        outputEncoding = (Encoding) val;
                        break;
            }

            base.SetResult(parameterId, val);
        }
    }
}

