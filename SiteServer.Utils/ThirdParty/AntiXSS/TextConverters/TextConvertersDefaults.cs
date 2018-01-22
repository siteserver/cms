// ***************************************************************
// <copyright file="TextConvertersDefaults.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters
{
    using Internal.Html;

    internal static class TextConvertersDefaults
    {
        

        private const int normalMinDecodeBytes = 64;

        private const int normalInitialTokenRuns = 64;
        private const int normalMaxTokenRuns = 512;

        private const int normalInitialTokenBufferSize = 1024;
        private const int normalMaxTokenSize = 4096;

        private const int normalInitialHtmlAttributes = 8;
        private const int normalMaxHtmlAttributes = 128;

        private const int normalMaxHtmlNormalizerNesting = HtmlSupport.HtmlNestingLimit;
        private const int normalMaxHtmlMetaRestartOffset = 4096;

        

        private const int boundaryMinDecodeBytes = 1;

        private const int boundaryInitialTokenRuns = 7;
        private const int boundaryMaxTokenRuns = 16;

        private const int boundaryInitialTokenBufferSize = 32;
        private const int boundaryMaxTokenSize = 128;

        private const int boundaryInitialHtmlAttributes = 1;
        private const int boundaryMaxHtmlAttributes = 5;

        private const int boundaryMaxHtmlNormalizerNesting = 10;
        private const int boundaryMaxHtmlMetaRestartOffset = 4096;

        

        public static int MinDecodeBytes(bool boundaryTest)
        {
            return boundaryTest ? boundaryMinDecodeBytes : normalMinDecodeBytes;
        }

        public static int InitialTokenRuns(bool boundaryTest)
        {
            return boundaryTest ? boundaryInitialTokenRuns : normalInitialTokenRuns;
        }

        public static int MaxTokenRuns(bool boundaryTest)
        {
            return boundaryTest ? boundaryMaxTokenRuns : normalMaxTokenRuns;
        }

        public static int InitialTokenBufferSize(bool boundaryTest)
        {
            return boundaryTest ? boundaryInitialTokenBufferSize : normalInitialTokenBufferSize;
        }

        public static int MaxTokenSize(bool boundaryTest)
        {
            return boundaryTest ? boundaryMaxTokenSize : normalMaxTokenSize;
        }

        public static int InitialHtmlAttributes(bool boundaryTest)
        {
            return boundaryTest ? boundaryInitialHtmlAttributes : normalInitialHtmlAttributes;
        }

        public static int MaxHtmlAttributes(bool boundaryTest)
        {
            return boundaryTest ? boundaryMaxHtmlAttributes : normalMaxHtmlAttributes;
        }

        public static int MaxHtmlNormalizerNesting(bool boundaryTest)
        {
            return boundaryTest ? boundaryMaxHtmlNormalizerNesting : normalMaxHtmlNormalizerNesting;
        }

        public static int MaxHtmlMetaRestartOffset(bool boundaryTest)
        {
            return boundaryTest ? boundaryMaxHtmlMetaRestartOffset : normalMaxHtmlMetaRestartOffset;
        }

    }
}
