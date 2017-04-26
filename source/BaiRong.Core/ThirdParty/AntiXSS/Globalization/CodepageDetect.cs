// ***************************************************************
// <copyright file="CodepageDetect.cs" company="Microsoft">
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
    using Internal;

    internal struct CodePageDetect
    {

        internal static uint validCodePagesMask = InitializeValidCodePagesMask();

        private int[] maskMap;

        private int[] codePageList;


        public void Initialize()
        {
            maskMap = new int[CodePageDetectData.codePageMask.Length];
        }



        public static bool IsCodePageDetectable(int cpid, bool validOnly)
        {
            var codepageIndex = CodePageDetectData.codePageIndex[cpid % CodePageDetectData.codePageIndex.Length];

            if (codepageIndex != 255)
            {
                if ((CodePageDetectData.codePages[codepageIndex].cpid == cpid) ||
                    (CodePageDetectData.codePages[codepageIndex].cpid == 38598 && cpid == 28598) ||
                    (CodePageDetectData.codePages[codepageIndex].cpid == 936 && cpid == 54936))
                {
                    return validOnly ? 0 != (validCodePagesMask & CodePageDetectData.codePages[codepageIndex].mask) : true;
                }
            }

            return false;
        }








        public static char[] GetCommonExceptionCharacters()
        {
            return CodePageDetectData.commonExceptions.Clone() as char[];
        }





        public static int[] GetDefaultPriorityList()
        {
            var list = new int[CodePageDetectData.codePages.Length];
            for (var i = 0; i < CodePageDetectData.codePages.Length; i++)
            {
                list[i] = CodePageDetectData.codePages[i].cpid;
            }
            return list;
        }


        public void Reset()
        {

            for (var i = 0; i < maskMap.Length; i++)
            {
                maskMap[i] = 0;
            }
        }


        public void AddData(char ch)
        {
#if !DETECTION_DOUBLE_INDEXING
            maskMap[CodePageDetectData.index[ch]]++;
#else
            ushort index0 = CodePageDetectData.index0[ch >> 4];
            this.maskMap[(index0 & 0x8000) != 0 ? (index0 & 0x00FF) : CodePageDetectData.index1[index0 + (ch & 0x000F)]] ++;
#endif
        }


        public void AddData(char[] buffer, int offset, int count)
        {
            count++;
            while (--count != 0)
            {
#if !DETECTION_DOUBLE_INDEXING
                maskMap[CodePageDetectData.index[buffer[offset]]]++;
#else
                char ch = buffer[offset];
                ushort index0 = CodePageDetectData.index0[ch >> 4];
                this.maskMap[(index0 & 0x8000) != 0 ? (index0 & 0x00FF) : CodePageDetectData.index1[index0 + (ch & 0x000F)]] ++;
#endif
                offset++;
            }
        }


        public void AddData(string buffer, int offset, int count)
        {
            count++;
            while (--count != 0)
            {
#if !DETECTION_DOUBLE_INDEXING
                maskMap[CodePageDetectData.index[buffer[offset]]]++;
#else
                char ch = buffer[offset];
                ushort index0 = CodePageDetectData.index0[ch >> 4];
                this.maskMap[(index0 & 0x8000) != 0 ? (index0 & 0x00FF) : CodePageDetectData.index1[index0 + (ch & 0x000F)]] ++;
#endif
                offset++;
            }
        }


        public void AddData(TextReader reader, int maxCharacters)
        {
            var charBuffer = new char[1024];

            while (maxCharacters != 0)
            {
                var countToRead = Math.Min(charBuffer.Length, maxCharacters);

                InternalDebug.Assert(countToRead != 0);

                var count = reader.Read(charBuffer, 0, countToRead);
                if (0 == count)
                {
                    break;
                }

                var offset = 0;

                maxCharacters -= count;

                count++;
                while (--count != 0)
                {
#if !DETECTION_DOUBLE_INDEXING
                    maskMap[CodePageDetectData.index[charBuffer[offset]]]++;
#else
                    char ch = charBuffer[offset];
                    ushort index0 = CodePageDetectData.index0[ch >> 4];
                    this.maskMap[(index0 & 0x8000) != 0 ? (index0 & 0x00FF) : CodePageDetectData.index1[index0 + (ch & 0x000F)]] ++;
#endif
                    offset++;
                }
            }
        }


        public int GetCodePage(int[] codePagePriorityList, bool allowCommonFallbackExceptions, bool allowAnyFallbackExceptions, bool onlyValidCodePages)
        {
            var cumulativeMask = 0xFFFFFFFF;

            for (var i = 0; i < maskMap.Length; i++)
            {
                if (maskMap[i] != 0)
                {

                    var mask = CodePageDetectData.codePageMask[i];

                    if (allowAnyFallbackExceptions ||
                        (allowCommonFallbackExceptions &&
                        (CodePageDetectData.fallbackMask[i] & CodePageDetectData.commonFallbackMask) != 0))
                    {

                        mask |= CodePageDetectData.fallbackMask[i] & ~CodePageDetectData.commonFallbackMask;
                    }


                    cumulativeMask &= mask;

                    if (cumulativeMask == 0)
                    {

                        break;
                    }
                }
            }

            if (onlyValidCodePages)
            {
                cumulativeMask &= validCodePagesMask;
            }

            return GetCodePage(ref cumulativeMask, codePagePriorityList);
        }


        internal static int GetCodePage(ref uint cumulativeMask, int[] codePagePriorityList)
        {
            if (cumulativeMask != 0)
            {


                if (codePagePriorityList != null)
                {
                    for (var i = 0; i < codePagePriorityList.Length; i++)
                    {
                        var codepageIndex = CodePageDetectData.codePageIndex[codePagePriorityList[i] % CodePageDetectData.codePageIndex.Length];

                        if (codepageIndex != 255)
                        {
                            if ((cumulativeMask & CodePageDetectData.codePages[codepageIndex].mask) != 0 &&
                                ((CodePageDetectData.codePages[codepageIndex].cpid == codePagePriorityList[i]) ||
                                (CodePageDetectData.codePages[codepageIndex].cpid == 38598 && codePagePriorityList[i] == 28598) ||
                                (CodePageDetectData.codePages[codepageIndex].cpid == 936 && codePagePriorityList[i] == 54936)))
                            {

                                cumulativeMask &= ~CodePageDetectData.codePages[codepageIndex].mask;
                                return codePagePriorityList[i];
                            }
                        }
                    }
                }



                for (var j = 0; j < CodePageDetectData.codePages.Length; j++)
                {
                    if ((cumulativeMask & CodePageDetectData.codePages[j].mask) != 0)
                    {

                        cumulativeMask &= ~CodePageDetectData.codePages[j].mask;
                        return CodePageDetectData.codePages[j].cpid;
                    }
                }


                InternalDebug.Assert(false, "not supposed to come here");
            }


            return 65001;
        }


        internal static int GetCodePageCount(uint cumulativeMask)
        {
            var count = 1;

            while (cumulativeMask != 0)
            {
                if (0 != (cumulativeMask & 1))
                {
                    count++;
                }

                cumulativeMask >>= 1;
            }

            return count;
        }


        public int[] GetCodePages(int[] codePagePriorityList, bool allowCommonFallbackExceptions, bool allowAnyFallbackExceptions, bool onlyValidCodePages)
        {
            var cumulativeMask = 0xFFFFFFFF;

            for (var i = 0; i < maskMap.Length; i++)
            {
                if (maskMap[i] != 0)
                {


                    var mask = CodePageDetectData.codePageMask[i];

                    if (allowAnyFallbackExceptions ||
                        (allowCommonFallbackExceptions &&
                        (CodePageDetectData.fallbackMask[i] & CodePageDetectData.commonFallbackMask) != 0))
                    {

                        mask |= CodePageDetectData.fallbackMask[i] & ~CodePageDetectData.commonFallbackMask;
                    }



                    cumulativeMask &= mask;

                    if (cumulativeMask == 0)
                    {

                        break;
                    }
                }
            }

            if (onlyValidCodePages)
            {
                cumulativeMask &= validCodePagesMask;
            }

            var numCodePages = GetCodePageCount(cumulativeMask);

            InternalDebug.Assert(numCodePages > 0);

            var result = new int[numCodePages];
            var index = 0;

            while (cumulativeMask != 0)
            {
                result[index++] = GetCodePage(ref cumulativeMask, codePagePriorityList);
            }

            result[index] = 65001;

            return result;
        }



        public int GetCodePageCoverage(int codePage)
        {
            uint codePageMask = 0;
            var codePageCount = 0;
            var totalCount = 0;

            var codepageIndex = CodePageDetectData.codePageIndex[codePage % CodePageDetectData.codePageIndex.Length];

            if (codepageIndex != 255)
            {
                if ((CodePageDetectData.codePages[codepageIndex].cpid == codePage) ||
                    (CodePageDetectData.codePages[codepageIndex].cpid == 38598 && codePage == 28598) ||
                    (CodePageDetectData.codePages[codepageIndex].cpid == 936 && codePage == 54936))
                {
                    codePageMask = CodePageDetectData.codePages[codepageIndex].mask;
                }
            }

            if (codePageMask != 0)
            {
                for (var i = 0; i < maskMap.Length; i++)
                {
                    if (codePageMask == (CodePageDetectData.codePageMask[i] & codePageMask))
                    {
                        codePageCount += maskMap[i];
                    }

                    totalCount += maskMap[i];
                }
            }

            return totalCount == 0 ? 0 : (int)((long)codePageCount * 100 / totalCount);
        }


        public int GetBestWindowsCodePage(bool allowCommonFallbackExceptions, bool allowAnyFallbackExceptions)
        {
            return GetBestWindowsCodePage(allowCommonFallbackExceptions, allowAnyFallbackExceptions, 0);
        }

        public int GetBestWindowsCodePage(bool allowCommonFallbackExceptions, bool allowAnyFallbackExceptions, int preferredCodePage)
        {
            uint mask;
            var totalCount = 0;

            if (codePageList == null)
            {
                codePageList = new int[CodePageDetectData.codePages.Length + 1];
            }
            else
            {
                for (var j = 0; j < codePageList.Length; j++)
                {
                    codePageList[j] = 0;
                }
            }

            for (var i = 0; i < maskMap.Length; i++)
            {
                if (maskMap[i] != 0)
                {


                    mask = CodePageDetectData.codePageMask[i];

                    if (allowAnyFallbackExceptions ||
                        (allowCommonFallbackExceptions &&
                        (CodePageDetectData.fallbackMask[i] & CodePageDetectData.commonFallbackMask) != 0))
                    {

                        mask |= CodePageDetectData.fallbackMask[i] & ~CodePageDetectData.commonFallbackMask;
                    }

                    mask &= CodePageDetectData.windowsCodePagesMask;
                    if (0 != mask)
                    {


                        for (var j = 0; mask != 0; j++, mask >>= 1)
                        {
                            if (0 != (mask & 1u))
                            {
                                codePageList[j] += maskMap[i];
                            }
                        }
                    }

                    totalCount += maskMap[i];
                }
            }

            var maximumCount = 0;
            var maximumCodePageIndex = 0;

            mask = CodePageDetectData.windowsCodePagesMask;

            for (var j = 0; mask != 0; j++)
            {
                if (CodePageDetectData.codePages[j].windowsCodePage && codePageList[j] > maximumCount)
                {
                    maximumCount = codePageList[j];
                    maximumCodePageIndex = j;
                }

                else if (codePageList[j] == maximumCount &&
                    ((CodePageDetectData.codePages[j].cpid == 1252) || (CodePageDetectData.codePages[j].cpid == preferredCodePage)))
                {
                    maximumCodePageIndex = j;
                }

                mask &= ~CodePageDetectData.codePages[j].mask;
            }

            InternalDebug.Assert(maximumCodePageIndex > 0 && maximumCodePageIndex < CodePageDetectData.codePages.Length);

            return CodePageDetectData.codePages[maximumCodePageIndex].cpid;
        }


        private static uint InitializeValidCodePagesMask()
        {
            uint mask = 0;

            for (var i = 0; i < CodePageDetectData.codePages.Length; i++)
            {


                Charset charset;
                if (Charset.TryGetCharset(CodePageDetectData.codePages[i].cpid, out charset))
                {
                    if (charset.IsAvailable)
                    {
                        mask |= CodePageDetectData.codePages[i].mask;
                    }
                }
            }

            return mask;
        }
    }




    internal struct SimpleCodepageDetector
    {



        private uint mask;


        public void AddData(char ch)
        {
#if !DETECTION_DOUBLE_INDEXING
            mask |= ~CodePageDetectData.codePageMask[CodePageDetectData.index[ch]];
#else
            ushort index0 = CodePageDetectData.index0[ch >> 4];
            mask |= ~CodePageDetectData.codePageMask[(index0 & 0x8000) != 0 ? (index0 & 0x00FF) : CodePageDetectData.index1[index0 + (ch & 0x000F)]];
#endif
        }


        public void AddData(char[] buffer, int offset, int count)
        {
            count++;
            while (--count != 0)
            {
#if !DETECTION_DOUBLE_INDEXING
                mask |= ~CodePageDetectData.codePageMask[CodePageDetectData.index[buffer[offset]]];
#else
                char ch = buffer[offset];
                ushort index0 = CodePageDetectData.index0[ch >> 4];
                mask |= ~CodePageDetectData.codePageMask[(index0 & 0x8000) != 0 ? (index0 & 0x00FF) : CodePageDetectData.index1[index0 + (ch & 0x000F)]];
#endif
                offset++;
            }
        }


        public void AddData(string buffer, int offset, int count)
        {
            count++;
            while (--count != 0)
            {
#if !DETECTION_DOUBLE_INDEXING
                mask |= ~CodePageDetectData.codePageMask[CodePageDetectData.index[buffer[offset]]];
#else
                char ch = buffer[offset];
                ushort index0 = CodePageDetectData.index0[ch >> 4];
                mask |= ~CodePageDetectData.codePageMask[(index0 & 0x8000) != 0 ? (index0 & 0x00FF) : CodePageDetectData.index1[index0 + (ch & 0x000F)]];
#endif
                offset++;
            }
        }


        public int GetCodePage(int[] codePagePriorityList, bool onlyValidCodePages)
        {

            var cumulativeMask = ~mask;

            if (onlyValidCodePages)
            {
                cumulativeMask &= CodePageDetect.validCodePagesMask;
            }

            return CodePageDetect.GetCodePage(ref cumulativeMask, codePagePriorityList);
        }


        public int GetWindowsCodePage(int[] codePagePriorityList, bool onlyValidCodePages)
        {

            var cumulativeMask = ~mask;


            cumulativeMask &= CodePageDetectData.windowsCodePagesMask;

            if (onlyValidCodePages)
            {
                cumulativeMask &= CodePageDetect.validCodePagesMask;
            }

            if (cumulativeMask == 0)
            {

                return 1252;
            }

            return CodePageDetect.GetCodePage(ref cumulativeMask, codePagePriorityList);
        }
    }
}

