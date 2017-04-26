// ***************************************************************
// <copyright file="OutboundCodepageDetector.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.Globalization
{
    using System;
    using System.IO;
    using GlobalizationStrings = CtsResources.GlobalizationStrings;

    
    
    
    internal enum FallbackExceptions
    {

        None,
        
        Common,
        
        All,
    }

    
    
    
    internal class OutboundCodePageDetector
    {
        private CodePageDetect detector;

        
        public OutboundCodePageDetector()
        {
            detector.Initialize();
        }

        internal static bool IsCodePageDetectable(int cpid, bool onlyValid)
        {
            return CodePageDetect.IsCodePageDetectable(cpid, onlyValid);
        }

        
        internal static int[] GetDefaultCodePagePriorityList()
        {
            return CodePageDetect.GetDefaultPriorityList();
        }

        internal static char[] GetCommonExceptionCharacters()
        {
            return CodePageDetect.GetCommonExceptionCharacters();
        }

        public void Reset()
        {
            detector.Reset();
        }

        public void AddText(char ch)
        {
            detector.AddData(ch);

        }

        
        public void AddText(char[] buffer, int index, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (index < 0 || index > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("index", GlobalizationStrings.IndexOutOfRange);
            }

            if (count < 0 || count > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("count", GlobalizationStrings.CountOutOfRange);
            }

            if (buffer.Length - index < count)
            {
                throw new ArgumentOutOfRangeException("count", GlobalizationStrings.CountTooLarge);
            }

            detector.AddData(buffer, index, count);
        }

        public void AddText(char[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            detector.AddData(buffer, 0, buffer.Length);
        }

        
        public void AddText(string value, int index, int count)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (index < 0 || index > value.Length)
            {
                throw new ArgumentOutOfRangeException("index", GlobalizationStrings.IndexOutOfRange);
            }

            if (count < 0 || count > value.Length)
            {
                throw new ArgumentOutOfRangeException("count", GlobalizationStrings.CountOutOfRange);
            }

            if (value.Length - index < count)
            {
                throw new ArgumentOutOfRangeException("count", GlobalizationStrings.CountTooLarge);
            }

            detector.AddData(value, index, count);
        }

        
        
        public void AddText(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            detector.AddData(value, 0, value.Length);
        }

        
        
        public void AddText(TextReader reader, int maxCharacters)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            if (maxCharacters < 0)
            {
                throw new ArgumentOutOfRangeException("maxCharacters", GlobalizationStrings.MaxCharactersCannotBeNegative);
            }

            detector.AddData(reader, maxCharacters);
        }

        public void AddText(TextReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            detector.AddData(reader, Int32.MaxValue);
        }

        public int GetCodePage()
        {
            return detector.GetCodePage(Culture.Default.CodepageDetectionPriorityOrder, false/*allowCommonFallbackExceptions*/, false/*allowAnyFallbackExceptions*/, true/*onlyValidCodePages*/);
        }

        
        public int GetCodePage(Culture culture, bool allowCommonFallbackExceptions)
        {
            if (culture == null)
            {
                culture = Culture.Default;
            }

            return detector.GetCodePage(
                            culture.CodepageDetectionPriorityOrder,
                            allowCommonFallbackExceptions,
                            false/*allowAnyFallbackExceptions*/,
                            true);
        }
 
        public int GetCodePage(Charset preferredCharset, bool allowCommonFallbackExceptions)
        {
            var priorityOrder = Culture.Default.CodepageDetectionPriorityOrder;

            if (preferredCharset != null)
            {
                priorityOrder = CultureCharsetDatabase.GetAdjustedCodepageDetectionPriorityOrder(preferredCharset, priorityOrder);
            }

            return detector.GetCodePage(
                            priorityOrder,
                            allowCommonFallbackExceptions,
                            false/*allowAnyFallbackExceptions*/,
                            true);
        }

        internal int GetCodePage(int[] codePagePriorityList, FallbackExceptions fallbackExceptions, bool onlyValidCodePages)
        {
            if (codePagePriorityList != null)
            {
                for (var i = 0; i < codePagePriorityList.Length; i++)
                {
                    if (!CodePageDetect.IsCodePageDetectable(codePagePriorityList[i], false))
                    {
                        throw new ArgumentException(GlobalizationStrings.PriorityListIncludesNonDetectableCodePage, "codePagePriorityList");
                    }
                }
            }

            return detector.GetCodePage(
                            codePagePriorityList,
                            fallbackExceptions > FallbackExceptions.None/*allowCommonFallbackExceptions*/,
                            fallbackExceptions > FallbackExceptions.Common/*allowAnyFallbackExceptions*/,
                            onlyValidCodePages);
        }

               
        public int[] GetCodePages()
        {
            return detector.GetCodePages(Culture.Default.CodepageDetectionPriorityOrder, false/*allowCommonFallbackExceptions*/, false/*allowAnyFallbackExceptions*/, true/*onlyValidCodePages*/);
        }

        public int[] GetCodePages(Culture culture, bool allowCommonFallbackExceptions)
        {
            if (culture == null)
            {
                culture = Culture.Default;
            }

            return detector.GetCodePages(
                            culture.CodepageDetectionPriorityOrder,
                            allowCommonFallbackExceptions,
                            false/*allowAnyFallbackExceptions*/,
                            true);
        }

        
        public int[] GetCodePages(Charset preferredCharset, bool allowCommonFallbackExceptions)
        {
            var priorityOrder = Culture.Default.CodepageDetectionPriorityOrder;

            if (preferredCharset != null)
            {
                priorityOrder = CultureCharsetDatabase.GetAdjustedCodepageDetectionPriorityOrder(preferredCharset, priorityOrder);
            }

            return detector.GetCodePages(
                            priorityOrder,
                            allowCommonFallbackExceptions,
                            false/*allowAnyFallbackExceptions*/,
                            true);
        }

        internal int[] GetCodePages(int[] codePagePriorityList, FallbackExceptions fallbackExceptions, bool onlyValidCodePages)
        {
            if (codePagePriorityList != null)
            {
                for (var i = 0; i < codePagePriorityList.Length; i++)
                {
                    if (!CodePageDetect.IsCodePageDetectable(codePagePriorityList[i], false))
                    {
                        throw new ArgumentException(GlobalizationStrings.PriorityListIncludesNonDetectableCodePage, "codePagePriorityList");
                    }
                }
            }

            return detector.GetCodePages(
                            codePagePriorityList,
                            fallbackExceptions > FallbackExceptions.None/*allowCommonFallbackExceptions*/,
                            fallbackExceptions > FallbackExceptions.Common/*allowAnyFallbackExceptions*/,
                            onlyValidCodePages);
        }

        public int GetCodePageCoverage(int codePage)
        {
            var charset = Charset.GetCharset(codePage);
            if (charset.UnicodeCoverage == CodePageUnicodeCoverage.Complete)
            {
                return 100;
            }

            if (!charset.IsDetectable)
            {
                if (charset.DetectableCodePageWithEquivalentCoverage == 0)
                {
                    throw new ArgumentException("codePage is not detectable");
                }

                codePage = charset.DetectableCodePageWithEquivalentCoverage;
            }

            return detector.GetCodePageCoverage(codePage);
        }

        
        public int GetBestWindowsCodePage()
        {
            return detector.GetBestWindowsCodePage(false/*allowCommonFallbackExceptions*/, false/*allowAnyFallbackExceptions*/);
        }

        
        internal int GetBestWindowsCodePage(int preferredCodePage)
        {
            return detector.GetBestWindowsCodePage(false/*allowCommonFallbackExceptions*/, false/*allowAnyFallbackExceptions*/, preferredCodePage);
        }
    }
}

