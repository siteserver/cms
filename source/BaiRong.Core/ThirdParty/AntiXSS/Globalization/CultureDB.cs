// ***************************************************************
// <copyright file="CultureDB.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.Globalization
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Globalization;
    using Internal;


    [Serializable]
    internal class Charset
    {
        private int codePage;
        private string name;
        private Culture culture;
        
        
        private short mapIndex;
        private bool windows;
        private bool available;
        private Encoding encoding;
        private string description;

        
        internal Charset(int codePage, string name)
        {
            this.codePage = codePage;
            this.name = name;
            culture = null;
            
            
            available = true;
            mapIndex = -1;
        }

        
        public int CodePage => codePage;


        public string Name => name;


        public Culture Culture => culture;


        public bool IsDetectable => mapIndex >= 0 && 0 != (CodePageMapData.codePages[mapIndex].flags & CodePageFlags.Detectable);


        public bool IsAvailable => !available ? false : encoding != null ? true : CheckAvailable();


        public bool IsWindowsCharset => windows;


        public string Description => description;


        public static Charset DefaultMimeCharset => Culture.Default.MimeCharset;


        public static Charset DefaultWebCharset => Culture.Default.WebCharset;


        public static Charset DefaultWindowsCharset => Culture.Default.WindowsCharset;


        public static Charset ASCII => CultureCharsetDatabase.data.asciiCharset;


        public static Charset UTF8 => CultureCharsetDatabase.data.utf8Charset;


        public static Charset Unicode => CultureCharsetDatabase.data.unicodeCharset;

        internal void SetCulture(Culture culture)
        {
            this.culture = culture;
        }

        internal void SetDescription(string description)
        {
            this.description = description;
        }

        internal void SetDefaultName(string name)
        {
            this.name = name;
        }

        internal void SetWindows()
        {
            windows = true;
        }

        internal void SetMapIndex(int index)
        {
            mapIndex = (short)index;
        }

        internal bool CheckAvailable()
        {
            Encoding encoding;
            return TryGetEncoding(out encoding);
        }

        internal int MapIndex => mapIndex;

        internal CodePageKind Kind => mapIndex < 0 ? CodePageKind.Unknown : CodePageMapData.codePages[mapIndex].kind;

        internal CodePageAsciiSupport AsciiSupport => mapIndex < 0 ? CodePageAsciiSupport.Unknown : CodePageMapData.codePages[mapIndex].asciiSupport;

        internal CodePageUnicodeCoverage UnicodeCoverage => mapIndex < 0 ? CodePageUnicodeCoverage.Unknown : CodePageMapData.codePages[mapIndex].unicodeCoverage;

        internal bool IsSevenBit => mapIndex >= 0 && 0 != (CodePageMapData.codePages[mapIndex].flags & CodePageFlags.SevenBit);

        internal int DetectableCodePageWithEquivalentCoverage => mapIndex < 0 ?
            0 :
            0 != (CodePageMapData.codePages[mapIndex].flags & CodePageFlags.Detectable) ?
                codePage :
                CodePageMapData.codePages[mapIndex].detectCpid;

        internal static int MaxCharsetNameLength => CultureCharsetDatabase.data.maxCharsetNameLength;


        public bool TryGetEncoding(out Encoding encoding)
        {
            if (this.encoding == null && available)
            {
                try
                {
                    if (codePage == 20127)
                    {
                        
                        
                        
                        this.encoding = Encoding.GetEncoding(codePage, new AsciiEncoderFallback(), DecoderFallback.ReplacementFallback);
                    }
                    else if (codePage == 28591 || codePage == 28599)
                    {
                        

                        
                        
                        
                        
                        
                        
                        
                        
                        
                        

                        this.encoding = new RemapEncoding(codePage);
                    }
                    else
                    {
                        this.encoding = Encoding.GetEncoding(codePage);
                    }
                }
                catch (ArgumentException)
                {
                    
                    this.encoding = null;
                }
                catch (NotSupportedException)
                {
                    
                    this.encoding = null;
                }

                if (this.encoding == null)
                {
                    available = false;
                }
            }

            encoding = this.encoding;
            return encoding != null;
        }

        
        
        
        
        
        
        
        
        public Encoding GetEncoding()
        {
            Encoding encoding;

            if (!TryGetEncoding(out encoding))
            {
                
                throw new CharsetNotInstalledException(codePage, name);
            }

            return encoding;
        }

        
        
        
        
        
        
        
        
        
        
        
        
        public static Charset GetCharset(string name)
        {
            Charset cs;

            if (!TryGetCharset(name, out cs))
            {
                throw new InvalidCharsetException(name);
            }

            return cs;
        }

        
        
        
        
        
        
        
        
        public static bool TryGetCharset(string name, out Charset charset)
        {
            if (name == null)
            {
                charset = null;
                return false;
            }

            if (CultureCharsetDatabase.data.nameToCharset.TryGetValue(name, out charset))
            {
                return true;
            }

            if (name.StartsWith("cp", StringComparison.OrdinalIgnoreCase) ||
                name.StartsWith("ms", StringComparison.OrdinalIgnoreCase))
            {
                var cpid = 0;

                for (var i = 2; i < name.Length; i++)
                {
                    if (name[i] < '0' || name[i] > '9')
                    {
                        return false;
                    }

                    cpid = cpid * 10 + (name[i] - '0');

                    if (cpid >= 65536)
                    {
                        return false;
                    }
                }

                if (cpid == 0)
                {
                    return false;
                }

                return TryGetCharset(cpid, out charset);
            }

            return false;
        }

        
        
        
        
        
        
        
        
        
        
        
        
        public static Charset GetCharset(int codePage)
        {
            Charset cs;

            if (!TryGetCharset(codePage, out cs))
            {
                throw new InvalidCharsetException(codePage);
            }

            return cs;
        }

        
        
        
        
        
        
        
        
        public static bool TryGetCharset(int codePage, out Charset charset)
        {
            return CultureCharsetDatabase.data.codePageToCharset.TryGetValue(codePage, out charset);
        }

        
        
        
        
        
        
        
        public static bool TryGetEncoding(int codePage, out Encoding encoding)
        {
            Charset charset;

            if (!TryGetCharset(codePage, out charset))
            {
                encoding = null;
                return false;
            }

            return charset.TryGetEncoding(out encoding);
        }

        
        
        
        
        
        
        
        public static bool TryGetEncoding(string name, out Encoding encoding)
        {
            Charset charset;
            if (!TryGetCharset(name, out charset))
            {
                encoding = null;
                return false;
            }

            return charset.TryGetEncoding(out encoding);
        }

        
        
        
        
        
        
        
        
        
        
        public static Encoding GetEncoding(int codePage)
        {
            var charset = GetCharset(codePage);
            return charset.GetEncoding();
        }

        
        
        
        
        
        
        
        
        
        
        public static Encoding GetEncoding(string name)
        {
            var charset = GetCharset(name);
            return charset.GetEncoding();
        }
    }

    
    
    
    
    
    
    
    
    
	[Serializable]
    internal class Culture
    {
        private int lcid;
        private string name;
        private Charset windowsCharset;
        private Charset mimeCharset;
        private Charset webCharset;
        private string description;
        private string nativeDescription;
        private Culture parentCulture;
        private int[] codepageDetectionPriorityOrder;
        private CultureInfo cultureInfo;

        internal Culture(int lcid, string name)
        {
            this.lcid = lcid;
            this.name = name;
        }

        
        
        
        
        public int LCID => lcid;


        public string Name => name;


        public Charset WindowsCharset => windowsCharset;


        public Charset MimeCharset => mimeCharset;


        public Charset WebCharset => webCharset;


        public string Description => description;


        public string NativeDescription => nativeDescription;


        public Culture ParentCulture => parentCulture;


        internal int[] CodepageDetectionPriorityOrder => GetCodepageDetectionPriorityOrder(CultureCharsetDatabase.data);


        public static Culture Default => CultureCharsetDatabase.data.defaultCulture;


        public static Culture Invariant => CultureCharsetDatabase.data.invariantCulture;

        internal void SetWindowsCharset(Charset windowsCharset)
        {
            this.windowsCharset = windowsCharset;
        }

        internal void SetMimeCharset(Charset mimeCharset)
        {
            this.mimeCharset = mimeCharset;
        }

        internal void SetWebCharset(Charset webCharset)
        {
            this.webCharset = webCharset;
        }

        internal void SetDescription(string description)
        {
            this.description = description;
        }

        internal void SetNativeDescription(string description)
        {
            nativeDescription = description;
        }

        internal void SetParentCulture(Culture parentCulture)
        {
            this.parentCulture = parentCulture;
        }

        internal void SetCultureInfo(CultureInfo cultureInfo)
        {
            this.cultureInfo = cultureInfo;
        }

        internal int[] GetCodepageDetectionPriorityOrder(CultureCharsetDatabase.GlobalizationData data)
        {
            

            if (codepageDetectionPriorityOrder == null)
            {
                codepageDetectionPriorityOrder =
                    CultureCharsetDatabase.GetCultureSpecificCodepageDetectionPriorityOrder(
                            this,
                            parentCulture == null || parentCulture == this ?
                                data.defaultDetectionPriorityOrder :
                                parentCulture.GetCodepageDetectionPriorityOrder(data));
            }

            return codepageDetectionPriorityOrder;
        }

        internal void SetCodepageDetectionPriorityOrder(int[] codepageDetectionPriorityOrder)
        {
            this.codepageDetectionPriorityOrder = codepageDetectionPriorityOrder;
        }

        
        
        
        
        
        
        
        
        
        
        
        
        public CultureInfo GetCultureInfo()
        {
            if (cultureInfo == null)
            {
                return CultureInfo.InvariantCulture;
            }

            return cultureInfo;
        }

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        internal CultureInfo GetSpecificCultureInfo()
        {
            if (cultureInfo == null)
            {
                return CultureInfo.InvariantCulture;
            }

            try
            {
                return CultureInfo.CreateSpecificCulture(cultureInfo.Name);
            }
            catch (ArgumentException)
            {
                return CultureInfo.InvariantCulture;
            }
        }

        
        
        
        
        
        
        
        
        
        public static Culture GetCulture(string name)
        {
            Culture culture;
            if (!TryGetCulture(name, out culture))
            {
                throw new UnknownCultureException(name);
            }
            return culture;
        }

        
        
        
        
        
        
        
        
        public static bool TryGetCulture(string name, out Culture culture)
        {
            if (name == null)
            {
                culture = null;
                return false;
            }

            return CultureCharsetDatabase.data.nameToCulture.TryGetValue(name, out culture);
        }

        
        
        
        
        
        
        
        
        
        public static Culture GetCulture(int lcid)
        {
            Culture culture;
            if (!TryGetCulture(lcid, out culture))
            {
                throw new UnknownCultureException(lcid);
            }
            return culture;
        }

        
        
        
        
        
        
        
        
        public static bool TryGetCulture(int lcid, out Culture culture)
        {
            return CultureCharsetDatabase.data.lcidToCulture.TryGetValue(lcid, out culture);
        }
    }

    
    internal static class CultureCharsetDatabase
    {
        private static readonly IntComparer IntComparerInstance = new IntComparer();
        internal static GlobalizationData data = LoadGlobalizationData(null);

        internal class GlobalizationData
        {
            internal Dictionary<string, Charset> nameToCharset = new Dictionary<string, Charset>(StringComparer.OrdinalIgnoreCase);
            internal Dictionary<int, Charset> codePageToCharset = new Dictionary<int, Charset>(IntComparerInstance);

            internal Dictionary<string, Culture> nameToCulture = new Dictionary<string, Culture>(StringComparer.OrdinalIgnoreCase);
            internal Dictionary<int, Culture> lcidToCulture = new Dictionary<int, Culture>(IntComparerInstance);

            internal Culture defaultCulture;
            internal Culture invariantCulture;

            internal int[] defaultDetectionPriorityOrder;

            internal int maxCharsetNameLength;

            internal Charset utf8Charset;
            internal Charset asciiCharset;
            internal Charset unicodeCharset;
        }

        
        internal static void RefreshConfiguration(string defaultCultureName)
        {
            
            data = LoadGlobalizationData(defaultCultureName);
        }

        private struct WindowsCodePage
        {
            private int codePage;
            private string name;
            private int lcid;
            private string cultureName;
            private int mimeCodePage;
            private int webCodePage;
            private string genericCultureDescription;

            public WindowsCodePage(int codePage, string name, int lcid, string cultureName, int mimeCodePage, int webCodePage, string genericCultureDescription)
            {
                this.codePage = codePage;
                this.name = name;
                this.lcid = lcid;
                this.cultureName = cultureName;
                this.mimeCodePage = mimeCodePage;
                this.webCodePage = webCodePage;
                this.genericCultureDescription = genericCultureDescription;
            }

            public int CodePage => codePage;
            public string Name => name;
            public int LCID => lcid;
            public string CultureName => cultureName;
            public int MimeCodePage => mimeCodePage;
            public int WebCodePage => webCodePage;
            public string GenericCultureDescription => genericCultureDescription;
        }

        private struct CodePageCultureOverride
        {
            private int codePage;
            private string cultureName;

            public CodePageCultureOverride(int codePage, string cultureName)
            {
                this.codePage = codePage;
                this.cultureName = cultureName;
            }

            public int CodePage => codePage;
            public string CultureName => cultureName;
        }

        private struct CultureCodePageOverride
        {
            private string cultureName;
            private int mimeCodePage;
            private int webCodePage;

            public CultureCodePageOverride(string cultureName, int mimeCodePage, int webCodePage)
            {
                this.cultureName = cultureName;
                this.mimeCodePage = mimeCodePage;
                this.webCodePage = webCodePage;
            }

            public string CultureName => cultureName;
            public int MimeCodePage => mimeCodePage;
            public int WebCodePage => webCodePage;
        }

        private struct CharsetName
        {
            private string name;
            private int codePage;

            public CharsetName(string name, int codePage)
            {
                this.name = name;
                this.codePage = codePage;
            }

            public string Name => name;
            public int CodePage => codePage;
        }

        private struct CultureData
        {
            private int lcid;
            private string name;
            private int windowsCodePage;
            private int mimeCodePage;
            private int webCodePage;
            private string parentCultureName;
            private string description;

            public CultureData(int lcid, string name, int windowsCodePage, int mimeCodePage, int webCodePage, string parentCultureName, string description)
            {
                this.lcid = lcid;
                this.name = name;
                this.windowsCodePage = windowsCodePage;
                this.mimeCodePage = mimeCodePage;
                this.webCodePage = webCodePage;
                this.parentCultureName = parentCultureName;
                this.description = description;
            }

            public int LCID => lcid;
            public string Name => name;
            public int WindowsCodePage => windowsCodePage;
            public int MimeCodePage => mimeCodePage;
            public int WebCodePage => webCodePage;
            public string ParentCultureName => parentCultureName;
            public string Description => description;
        }

        private static GlobalizationData LoadGlobalizationData(string defaultCultureName)
        {
            

            
            WindowsCodePage[] windowsCodePages =
            {
                
                new WindowsCodePage(1200, "unicode", 0, null, 65001, 65001, "Unicode generic culture"),                     
                new WindowsCodePage(1250, "windows-1250", 0, null, 28592, 1250, "Central European generic culture"),        
                new WindowsCodePage(1251, "windows-1251", 0, null, 20866, 1251, "Cyrillic generic culture"),                
                new WindowsCodePage(1252, "windows-1252", 0, null, 28591, 1252, "Western European generic culture"),        
                new WindowsCodePage(1253, "windows-1253", 0x0008, "el", 28597, 1253, null),                                 
                new WindowsCodePage(1254, "windows-1254", 0, null, 28599, 1254, "Turkish / Azeri generic culture"),             
                new WindowsCodePage(1255, "windows-1255", 0x000D, "he", 1255, 1255, null),                                  
                new WindowsCodePage(1256, "windows-1256", 0, null, 1256, 1256, "Arabic generic culture"),                   
                new WindowsCodePage(1257, "windows-1257", 0, null, 1257, 1257, "Baltic generic culture"),                   
                new WindowsCodePage(1258, "windows-1258", 0x002A, "vi", 1258, 1258, null),                                  
                new WindowsCodePage(874, "windows-874", 0x001E, "th", 874, 874, null),                                      
                new WindowsCodePage(932, "windows-932", 0x0011, "ja", 50220, 932, null),                                    
                new WindowsCodePage(936, "windows-936", 0x0004, "zh-CHS", 936, 936, null),                                  
                new WindowsCodePage(949, "windows-949", 0x0012, "ko", 949, 949, null),                                      
                new WindowsCodePage(950, "windows-950", 0x7C04, "zh-CHT", 950, 950, null),                                  
            };

            CodePageCultureOverride[] codePageCultureOverrides =
            {
                new CodePageCultureOverride(37, "en"),
                new CodePageCultureOverride(437, "en"),
                new CodePageCultureOverride(860, "pt"),
                new CodePageCultureOverride(861, "is"),
                new CodePageCultureOverride(863, "fr-CA"),
                new CodePageCultureOverride(1141, "de"),
                new CodePageCultureOverride(1144, "it"),
                new CodePageCultureOverride(1145, "es"),
                new CodePageCultureOverride(1146, "en-GB"),
                new CodePageCultureOverride(1147, "fr"),
                new CodePageCultureOverride(1149, "is"),
                new CodePageCultureOverride(10010, "ro"),
                new CodePageCultureOverride(10017, "uk"),
                new CodePageCultureOverride(10079, "is"),
                new CodePageCultureOverride(10082, "hr"),
                new CodePageCultureOverride(20106, "de"),
                new CodePageCultureOverride(20107, "sv"),
                new CodePageCultureOverride(20108, "no"),
                new CodePageCultureOverride(20127, "en"),
                new CodePageCultureOverride(20273, "de"),
                new CodePageCultureOverride(20280, "it"),
                new CodePageCultureOverride(20284, "es"),
                new CodePageCultureOverride(20285, "en-GB"),
                new CodePageCultureOverride(20297, "fr"),
                new CodePageCultureOverride(20866, "ru"),
                new CodePageCultureOverride(20871, "is"),
                new CodePageCultureOverride(20880, "ru"),
                new CodePageCultureOverride(21866, "uk"),
                
                new CodePageCultureOverride(57003, "bn-IN"),
                new CodePageCultureOverride(57004, "ta"),
                new CodePageCultureOverride(57005, "te"),
                
                
                new CodePageCultureOverride(57008, "kn"),
                new CodePageCultureOverride(57009, "ml-IN"),
                new CodePageCultureOverride(57010, "gu"),
                new CodePageCultureOverride(57011, "pa"),
            };

            CultureCodePageOverride[] cultureCodePageOverrides =
            {
                
                new CultureCodePageOverride("et", 28605, 28605),            
                new CultureCodePageOverride("lt", 28603, 28603),            
                new CultureCodePageOverride("lv", 28603, 28603),            

                new CultureCodePageOverride("uk", 21866, 1251),             

                new CultureCodePageOverride("az-AZ-Cyrl", 1251, 1251),      
                new CultureCodePageOverride("be", 1251, 1251),              
                new CultureCodePageOverride("bg", 1251, 1251),              
                new CultureCodePageOverride("mk", 1251, 1251),              
                new CultureCodePageOverride("sr", 1251, 1251),              
                new CultureCodePageOverride("sr-BA-Cyrl", 1251, 1251),      
                new CultureCodePageOverride("sr-Cyrl-CS", 1251, 1251),      
                new CultureCodePageOverride("ky", 1251, 1251),              
                new CultureCodePageOverride("kk", 1251, 1251),              
                new CultureCodePageOverride("tt", 1251, 1251),              
                new CultureCodePageOverride("uz-UZ-Cyrl", 1251, 1251),      

                new CultureCodePageOverride("mn", 65001, 65001),            
            };

            
            
            CharsetName[] charsetNames =
            {
                new CharsetName("_autodetect", 50932),    
                new CharsetName("_autodetect_all", 50001),    
                new CharsetName("_autodetect_kr", 50949),    
                new CharsetName("_iso-2022-jp$ESC", 50221),    
                new CharsetName("_iso-2022-jp$SIO", 50222),    
                new CharsetName("437", 437),    
                new CharsetName("ANSI_X3.4-1968", 20127),    
                new CharsetName("ANSI_X3.4-1986", 20127),    
                new CharsetName("arabic", 28596),    
                new CharsetName("ascii", 20127),    
                new CharsetName("ASMO-708", 708),    
                new CharsetName("Big5-HKSCS", 950),    
                new CharsetName("Big5", 950),    
                new CharsetName("CCSID00858", 858),    
                new CharsetName("CCSID00924", 20924),    
                new CharsetName("CCSID01140", 1140),    
                new CharsetName("CCSID01141", 1141),    
                new CharsetName("CCSID01142", 1142),    
                new CharsetName("CCSID01143", 1143),    
                new CharsetName("CCSID01144", 1144),    
                new CharsetName("CCSID01145", 1145),    
                new CharsetName("CCSID01146", 1146),    
                new CharsetName("CCSID01147", 1147),    
                new CharsetName("CCSID01148", 1148),    
                new CharsetName("CCSID01149", 1149),    
                new CharsetName("chinese", 936),    
                new CharsetName("cn-big5", 950),    
                new CharsetName("CN-GB", 936),    
                new CharsetName("CP00858", 858),    
                new CharsetName("CP00924", 20924),    
                new CharsetName("CP01140", 1140),    
                new CharsetName("CP01141", 1141),    
                new CharsetName("CP01142", 1142),    
                new CharsetName("CP01143", 1143),    
                new CharsetName("CP01144", 1144),    
                new CharsetName("CP01145", 1145),    
                new CharsetName("CP01146", 1146),    
                new CharsetName("CP01147", 1147),    
                new CharsetName("CP01148", 1148),    
                new CharsetName("CP01149", 1149),    
                new CharsetName("cp037", 37),    
                new CharsetName("cp1025", 21025),    
                new CharsetName("CP1026", 1026),    
                new CharsetName("cp1256", 1256),    
                new CharsetName("CP273", 20273),    
                new CharsetName("CP278", 20278),    
                new CharsetName("CP280", 20280),    
                new CharsetName("CP284", 20284),    
                new CharsetName("CP285", 20285),    
                new CharsetName("cp290", 20290),    
                new CharsetName("cp297", 20297),    
                new CharsetName("cp367", 20127),    
                new CharsetName("cp420", 20420),    
                new CharsetName("cp423", 20423),    
                new CharsetName("cp424", 20424),    
                new CharsetName("cp437", 437),    
                new CharsetName("CP500", 500),    
                new CharsetName("cp50227", 50227), 
                new CharsetName("cp50229", 50229), 
                new CharsetName("cp819", 28591),    
                new CharsetName("cp850", 850),    
                new CharsetName("cp852", 852),    
                new CharsetName("cp855", 855),    
                new CharsetName("cp857", 857),    
                new CharsetName("cp858", 858),    
                new CharsetName("cp860", 860),    
                new CharsetName("cp861", 861),    
                new CharsetName("cp862", 862),    
                new CharsetName("cp863", 863),    
                new CharsetName("cp864", 864),    
                new CharsetName("cp865", 865),    
                new CharsetName("cp866", 866),    
                new CharsetName("cp869", 869),    
                new CharsetName("CP870", 870),    
                new CharsetName("CP871", 20871),    
                new CharsetName("cp875", 875),    
                new CharsetName("cp880", 20880),    
                new CharsetName("CP905", 20905),    
                new CharsetName("cp930", 50930),    
                new CharsetName("cp933", 50933),    
                new CharsetName("cp935", 50935),    
                new CharsetName("cp937", 50937),    
                new CharsetName("cp939", 50939),    
                new CharsetName("csASCII", 20127),    
                new CharsetName("csbig5", 950),    
                new CharsetName("csEUCKR", 51949),    
                new CharsetName("csEUCPkdFmtJapanese", 51932),    
                new CharsetName("csGB2312", 936),    
                new CharsetName("csGB231280", 936),    
                new CharsetName("csIBM037", 37),    
                new CharsetName("csIBM1026", 1026),    
                new CharsetName("csIBM273", 20273),    
                new CharsetName("csIBM277", 20277),    
                new CharsetName("csIBM278", 20278),    
                new CharsetName("csIBM280", 20280),    
                new CharsetName("csIBM284", 20284),    
                new CharsetName("csIBM285", 20285),    
                new CharsetName("csIBM290", 20290),    
                new CharsetName("csIBM297", 20297),    
                new CharsetName("csIBM420", 20420),    
                new CharsetName("csIBM423", 20423),    
                new CharsetName("csIBM424", 20424),    
                new CharsetName("csIBM500", 500),    
                new CharsetName("csIBM870", 870),    
                new CharsetName("csIBM871", 20871),    
                new CharsetName("csIBM880", 20880),    
                new CharsetName("csIBM905", 20905),    
                new CharsetName("csIBMThai", 20838),    
                new CharsetName("csISO2022JP", 50221),    
                new CharsetName("csISO2022KR", 50225),    
                new CharsetName("csISO58GB231280", 936),    
                new CharsetName("csISOLatin1", 28591),    
                new CharsetName("csISOLatin2", 28592),    
                new CharsetName("csISOLatin3", 28593),    
                new CharsetName("csISOLatin4", 28594),    
                new CharsetName("csISOLatin5", 28599),    
                new CharsetName("csISOLatin9", 28605),    
                new CharsetName("csISOLatinArabic", 28596),    
                new CharsetName("csISOLatinCyrillic", 28595),    
                new CharsetName("csISOLatinGreek", 28597),    
                new CharsetName("csISOLatinHebrew", 38598),    
                new CharsetName("csKOI8R", 20866),    
                new CharsetName("csKSC56011987", 949),    
                new CharsetName("csPC8CodePage437", 437),    
                new CharsetName("csShiftJIS", 932),    
                new CharsetName("csUnicode11UTF7", 65000),    
                new CharsetName("csWindows31J", 932),    
                new CharsetName("Windows-31J", 932),    
                new CharsetName("cyrillic", 28595),    
                new CharsetName("DIN_66003", 20106),    
                new CharsetName("DOS-720", 720),    
                new CharsetName("DOS-862", 862),    
                new CharsetName("DOS-874", 874),    
                new CharsetName("ebcdic-cp-ar1", 20420),    
                new CharsetName("ebcdic-cp-be", 500),    
                new CharsetName("ebcdic-cp-ca", 37),    
                new CharsetName("ebcdic-cp-ch", 500),    
                new CharsetName("EBCDIC-CP-DK", 20277),    
                new CharsetName("ebcdic-cp-es", 20284),    
                new CharsetName("ebcdic-cp-fi", 20278),    
                new CharsetName("ebcdic-cp-fr", 20297),    
                new CharsetName("ebcdic-cp-gb", 20285),    
                new CharsetName("ebcdic-cp-gr", 20423),    
                new CharsetName("ebcdic-cp-he", 20424),    
                new CharsetName("ebcdic-cp-is", 20871),    
                new CharsetName("ebcdic-cp-it", 20280),    
                new CharsetName("ebcdic-cp-nl", 37),    
                new CharsetName("EBCDIC-CP-NO", 20277),    
                new CharsetName("ebcdic-cp-roece", 870),    
                new CharsetName("ebcdic-cp-se", 20278),    
                new CharsetName("ebcdic-cp-tr", 20905),    
                new CharsetName("ebcdic-cp-us", 37),    
                new CharsetName("ebcdic-cp-wt", 37),    
                new CharsetName("ebcdic-cp-yu", 870),    
                new CharsetName("EBCDIC-Cyrillic", 20880),    
                new CharsetName("ebcdic-de-273+euro", 1141),    
                new CharsetName("ebcdic-dk-277+euro", 1142),    
                new CharsetName("ebcdic-es-284+euro", 1145),    
                new CharsetName("ebcdic-fi-278+euro", 1143),    
                new CharsetName("ebcdic-fr-297+euro", 1147),    
                new CharsetName("ebcdic-gb-285+euro", 1146),    
                new CharsetName("ebcdic-international-500+euro", 1148),    
                new CharsetName("ebcdic-is-871+euro", 1149),    
                new CharsetName("ebcdic-it-280+euro", 1144),    
                new CharsetName("EBCDIC-JP-kana", 20290),    
                new CharsetName("ebcdic-Latin9--euro", 20924),    
                new CharsetName("ebcdic-no-277+euro", 1142),    
                new CharsetName("ebcdic-se-278+euro", 1143),    
                new CharsetName("ebcdic-us-37+euro", 1140),    
                new CharsetName("ECMA-114", 28596),    
                new CharsetName("ECMA-118", 28597),    
                new CharsetName("ELOT_928", 28597),    
                new CharsetName("euc-cn", 51936),    
                new CharsetName("euc-jp", 51932),    
                new CharsetName("euc-kr", 51949),    
                new CharsetName("Extended_UNIX_Code_Packed_Format_for_Japanese", 51932),    
                new CharsetName("GB_2312-80", 936),    
                new CharsetName("GB18030", 54936),    
                new CharsetName("GB2312-80", 936),    
                new CharsetName("GB2312", 936),    
                new CharsetName("GB231280", 936),    
                new CharsetName("GBK", 936),    
                new CharsetName("German", 20106),    
                new CharsetName("greek", 28597),    
                new CharsetName("greek8", 28597),    
                new CharsetName("hebrew", 38598),    
                new CharsetName("hz-gb-2312", 52936),    
                new CharsetName("IBM-Thai", 20838),    
                new CharsetName("IBM00858", 858),    
                new CharsetName("IBM00924", 20924),    
                new CharsetName("IBM01047", 1047),    
                new CharsetName("IBM01140", 1140),    
                new CharsetName("IBM01141", 1141),    
                new CharsetName("IBM01142", 1142),    
                new CharsetName("IBM01143", 1143),    
                new CharsetName("IBM01144", 1144),    
                new CharsetName("IBM01145", 1145),    
                new CharsetName("IBM01146", 1146),    
                new CharsetName("IBM01147", 1147),    
                new CharsetName("IBM01148", 1148),    
                new CharsetName("IBM01149", 1149),    
                new CharsetName("IBM037", 37),    
                new CharsetName("IBM1026", 1026),    
                new CharsetName("IBM273", 20273),    
                new CharsetName("IBM277", 20277),    
                new CharsetName("IBM278", 20278),    
                new CharsetName("IBM280", 20280),    
                new CharsetName("IBM284", 20284),    
                new CharsetName("IBM285", 20285),    
                new CharsetName("IBM290", 20290),    
                new CharsetName("IBM297", 20297),    
                new CharsetName("IBM367", 20127),    
                new CharsetName("IBM420", 20420),    
                new CharsetName("IBM423", 20423),    
                new CharsetName("IBM424", 20424),    
                new CharsetName("IBM437", 437),    
                new CharsetName("IBM500", 500),    
                new CharsetName("ibm737", 737),    
                new CharsetName("ibm775", 775),    
                new CharsetName("ibm819", 28591),    
                new CharsetName("IBM850", 850),    
                new CharsetName("IBM852", 852),    
                new CharsetName("IBM855", 855),    
                new CharsetName("IBM857", 857),    
                new CharsetName("IBM860", 860),    
                new CharsetName("IBM861", 861),    
                new CharsetName("IBM862", 862),    
                new CharsetName("IBM863", 863),    
                new CharsetName("IBM864", 864),    
                new CharsetName("IBM865", 865),    
                new CharsetName("IBM866", 866),    
                new CharsetName("IBM869", 869),    
                new CharsetName("IBM870", 870),    
                new CharsetName("IBM871", 20871),    
                new CharsetName("IBM880", 20880),    
                new CharsetName("IBM905", 20905),    
                new CharsetName("irv", 20105),    
                new CharsetName("ISO-10646-UCS-2", 1200), 
                new CharsetName("iso-2022-jp", 50220),    
                new CharsetName("iso-2022-jpdbcs", 50220),    
                new CharsetName("iso-2022-jpesc", 50221),    
                new CharsetName("iso-2022-jpsio", 50222),    
                new CharsetName("iso-2022-jpeuc", 51932),    
                new CharsetName("iso-2022-kr-7", 50225),    
                new CharsetName("iso-2022-kr-7bit", 50225),    
                new CharsetName("iso-2022-kr-8", 51949),    
                new CharsetName("iso-2022-kr-8bit", 51949),    
                new CharsetName("iso-2022-kr", 50225),    
                new CharsetName("iso-8859-1", 28591),    
                new CharsetName("iso-8859-11", 874),    
                new CharsetName("iso-8859-13", 28603),    
                new CharsetName("iso-8859-15", 28605),    
                new CharsetName("iso-8859-2", 28592),    
                new CharsetName("iso-8859-3", 28593),    
                new CharsetName("iso-8859-4", 28594),    
                new CharsetName("iso-8859-5", 28595),    
                new CharsetName("iso-8859-6", 28596),    
                new CharsetName("iso-8859-7", 28597),    
                new CharsetName("iso-8859-8-i", 38598),    
                new CharsetName("ISO-8859-8 Visual", 28598),    
                new CharsetName("iso-8859-8", 28598),    
                new CharsetName("iso-8859-9", 28599),    
                new CharsetName("iso-ir-100", 28591),    
                new CharsetName("iso-ir-101", 28592),    
                new CharsetName("iso-ir-109", 28593),    
                new CharsetName("iso-ir-110", 28594),    
                new CharsetName("iso-ir-126", 28597),    
                new CharsetName("iso-ir-127", 28596),    
                new CharsetName("iso-ir-138", 38598),    
                new CharsetName("iso-ir-144", 28595),    
                new CharsetName("iso-ir-148", 28599),    
                new CharsetName("iso-ir-149", 949),    
                new CharsetName("iso-ir-58", 936),    
                new CharsetName("iso-ir-6", 20127),    
                new CharsetName("ISO_646.irv:1991", 20127),    
                new CharsetName("iso_8859-1", 28591),    
                new CharsetName("iso_8859-1:1987", 28591),    
                new CharsetName("ISO_8859-15", 28605),    
                new CharsetName("iso_8859-2", 28592),    
                new CharsetName("iso_8859-2:1987", 28592),    
                new CharsetName("ISO_8859-3", 28593),    
                new CharsetName("ISO_8859-3:1988", 28593),    
                new CharsetName("ISO_8859-4", 28594),    
                new CharsetName("ISO_8859-4:1988", 28594),    
                new CharsetName("ISO_8859-5", 28595),    
                new CharsetName("ISO_8859-5:1988", 28595),    
                new CharsetName("ISO_8859-6", 28596),    
                new CharsetName("ISO_8859-6:1987", 28596),    
                new CharsetName("ISO_8859-7", 28597),    
                new CharsetName("ISO_8859-7:1987", 28597),    
                new CharsetName("ISO_8859-8", 28598),    
                new CharsetName("ISO_8859-8:1988", 28598),    
                new CharsetName("ISO_8859-9", 28599),    
                new CharsetName("ISO_8859-9:1989", 28599),    
                new CharsetName("ISO646-US", 20127),    
                new CharsetName("646", 20127),    
                new CharsetName("iso8859-1", 28591),    
                new CharsetName("iso8859-2", 28592),    
                new CharsetName("Johab", 1361),    
                new CharsetName("koi", 20866),    
                new CharsetName("koi8-r", 20866),    
                new CharsetName("koi8-ru", 21866),    
                new CharsetName("koi8-u", 21866),    
                new CharsetName("koi8", 20866),    
                new CharsetName("koi8r", 20866),    
                new CharsetName("korean", 949),    
                new CharsetName("ks-c-5601", 949),    
                new CharsetName("ks-c5601", 949),    
                new CharsetName("ks_c_5601-1987", 949),    
                new CharsetName("ks_c_5601-1989", 949),    
                new CharsetName("ks_c_5601", 949),    
                new CharsetName("ks_c_5601_1987", 949),    
                new CharsetName("KSC_5601", 949),    
                new CharsetName("KSC5601", 949),    
                new CharsetName("l1", 28591),    
                new CharsetName("l2", 28592),    
                new CharsetName("l3", 28593),    
                new CharsetName("l4", 28594),    
                new CharsetName("l5", 28599),    
                new CharsetName("l9", 28605),    
                new CharsetName("latin1", 28591),    
                new CharsetName("latin2", 28592),    
                new CharsetName("latin3", 28593),    
                new CharsetName("latin4", 28594),    
                new CharsetName("latin5", 28599),    
                new CharsetName("latin9", 28605),    
                new CharsetName("logical", 38598),    
                new CharsetName("macintosh", 10000),    
                new CharsetName("MacRoman", 10000),    
                new CharsetName("ms_Kanji", 932),    
                new CharsetName("Norwegian", 20108),    
                new CharsetName("NS_4551-1", 20108),    
                new CharsetName("PC-Multilingual-850+euro", 858),    
                new CharsetName("SEN_850200_B", 20107),    
                new CharsetName("shift-jis", 932),    
                new CharsetName("shift_jis", 932),    
                new CharsetName("sjis", 932),    
                new CharsetName("Swedish", 20107),    
                new CharsetName("TIS-620", 874),    
                new CharsetName("ucs-2", 1200), 
                new CharsetName("unicode-1-1-utf-7", 65000),    
                new CharsetName("unicode-1-1-utf-8", 65001),    
                new CharsetName("unicode-2-0-utf-7", 65000),    
                new CharsetName("unicode-2-0-utf-8", 65001),    
                new CharsetName("unicode", 1200),    
                new CharsetName("unicodeFFFE", 1201),    
                new CharsetName("us-ascii", 20127),    
                new CharsetName("us", 20127),    
                new CharsetName("utf-16", 1200),    
                new CharsetName("UTF-16BE", 1201),    
                new CharsetName("UTF-16LE", 1200),    
                new CharsetName("utf-32", 12000),
                new CharsetName("UTF-32BE", 12001),
                new CharsetName("UTF-32LE", 12000),
                new CharsetName("utf-7", 65000),    
                new CharsetName("utf-8", 65001),    
                new CharsetName("utf7", 65000),    
                new CharsetName("utf8", 65001),    
                new CharsetName("visual", 28598),    
                new CharsetName("windows-1250", 1250),    
                new CharsetName("windows-1251", 1251),    
                new CharsetName("windows-1252", 1252),    
                new CharsetName("windows-1253", 1253),    
                new CharsetName("Windows-1254", 1254),    
                new CharsetName("windows-1255", 1255),    
                new CharsetName("windows-1256", 1256),    
                new CharsetName("windows-1257", 1257),    
                new CharsetName("windows-1258", 1258),    
                new CharsetName("windows-874", 874),    
                new CharsetName("x-ansi", 1252),    
                new CharsetName("x-Chinese-CNS", 20000),    
                new CharsetName("x-Chinese-Eten", 20002),    
                new CharsetName("x-cp1250", 1250),    
                new CharsetName("x-cp1251", 1251),    
                new CharsetName("x-cp20001", 20001),    
                new CharsetName("x-cp20003", 20003),    
                new CharsetName("x-cp20004", 20004),    
                new CharsetName("x-cp20005", 20005),    
                new CharsetName("x-cp20261", 20261),    
                new CharsetName("x-cp20269", 20269),    
                new CharsetName("x-cp20936", 20936),    
                new CharsetName("x-cp20949", 20949),    
                new CharsetName("x-cp21027", 21027),    
                new CharsetName("x-cp50227", 50227),    
                new CharsetName("x-cp50229", 50229),    
                new CharsetName("X-EBCDIC-JapaneseAndUSCanada", 50931),    
                new CharsetName("X-EBCDIC-KoreanExtended", 20833),    
                new CharsetName("x-euc-cn", 51936),    
                new CharsetName("x-euc-jp", 51932),    
                new CharsetName("x-euc", 51932),    
                new CharsetName("x-Europa", 29001),    
                new CharsetName("x-IA5-German", 20106),    
                new CharsetName("x-IA5-Norwegian", 20108),    
                new CharsetName("x-IA5-Swedish", 20107),    
                new CharsetName("x-IA5", 20105),    
                new CharsetName("x-iscii-as", 57006),    
                new CharsetName("x-iscii-be", 57003),    
                new CharsetName("x-iscii-de", 57002),    
                new CharsetName("x-iscii-gu", 57010),    
                new CharsetName("x-iscii-ka", 57008),    
                new CharsetName("x-iscii-ma", 57009),    
                new CharsetName("x-iscii-or", 57007),    
                new CharsetName("x-iscii-pa", 57011),    
                new CharsetName("x-iscii-ta", 57004),    
                new CharsetName("x-iscii-te", 57005),    
                new CharsetName("x-mac-arabic", 10004),    
                new CharsetName("x-mac-ce", 10029),    
                new CharsetName("x-mac-chinesesimp", 10008),    
                new CharsetName("x-mac-chinesetrad", 10002),    
                new CharsetName("x-mac-croatian", 10082),    
                new CharsetName("x-mac-cyrillic", 10007),    
                new CharsetName("x-mac-greek", 10006),    
                new CharsetName("x-mac-hebrew", 10005),    
                new CharsetName("x-mac-icelandic", 10079),    
                new CharsetName("x-mac-japanese", 10001),    
                new CharsetName("x-mac-korean", 10003),    
                new CharsetName("x-mac-romanian", 10010),    
                new CharsetName("x-mac-thai", 10021),    
                new CharsetName("x-mac-turkish", 10081),    
                new CharsetName("x-mac-ukrainian", 10017),    
                new CharsetName("x-ms-cp932", 932),    
                new CharsetName("x-sjis", 932),    
                new CharsetName("x-unicode-1-1-utf-7", 65000),    
                new CharsetName("x-unicode-1-1-utf-8", 65001),    
                new CharsetName("x-unicode-2-0-utf-7", 65000),    
                new CharsetName("x-unicode-2-0-utf-8", 65001),    
                new CharsetName("x-user-defined", 1252),    
                new CharsetName("x-x-big5", 950),    
            };

            
            CultureData[] hardcodedCultures =
            {
                new CultureData(0x401, "ar-SA", 1256, 1256, 1256, "ar", "Arabic (Saudi Arabia)"),
                new CultureData(0x402, "bg-BG", 1251, 1251, 1251, "bg", "Bulgarian (Bulgaria)"),
                new CultureData(0x403, "ca-ES", 1252, 28591, 1252, "ca", "Catalan (Catalan)"),
                new CultureData(0x404, "zh-TW", 950, 950, 950, "zh-CHT", "Chinese (Taiwan)"),
                new CultureData(0x405, "cs-CZ", 1250, 28592, 1250, "cs", "Czech (Czech Republic)"),
                new CultureData(0x406, "da-DK", 1252, 28591, 1252, "da", "Danish (Denmark)"),
                new CultureData(0x407, "de-DE", 1252, 28591, 1252, "de", "German (Germany)"),
                new CultureData(0x408, "el-GR", 1253, 28597, 1253, "el", "Greek (Greece)"),
                new CultureData(0x409, "en-US", 1252, 28591, 1252, "en", "English (United States)"),
                new CultureData(0x40B, "fi-FI", 1252, 28591, 1252, "fi", "Finnish (Finland)"),
                new CultureData(0x40C, "fr-FR", 1252, 28591, 1252, "fr", "French (France)"),
                new CultureData(0x40D, "he-IL", 1255, 1255, 1255, "he", "Hebrew (Israel)"),
                new CultureData(0x40E, "hu-HU", 1250, 28592, 1250, "hu", "Hungarian (Hungary)"),
                new CultureData(0x40F, "is-IS", 1252, 28591, 1252, "is", "Icelandic (Iceland)"),
                new CultureData(0x410, "it-IT", 1252, 28591, 1252, "it", "Italian (Italy)"),
                new CultureData(0x411, "ja-JP", 932, 50220, 932, "ja", "Japanese (Japan)"),
                new CultureData(0x412, "ko-KR", 949, 949, 949, "ko", "Korean (Korea)"),
                new CultureData(0x413, "nl-NL", 1252, 28591, 1252, "nl", "Dutch (Netherlands)"),
                new CultureData(0x414, "nb-NO", 1252, 28591, 1252, "no", "Norwegian - Bokm鍞?(Norway)"),
                new CultureData(0x415, "pl-PL", 1250, 28592, 1250, "pl", "Polish (Poland)"),
                new CultureData(0x416, "pt-BR", 1252, 28591, 1252, "pt", "Portuguese (Brazil)"),
                new CultureData(0x418, "ro-RO", 1250, 28592, 1250, "ro", "Romanian (Romania)"),
                new CultureData(0x419, "ru-RU", 1251, 20866, 1251, "ru", "Russian (Russia)"),
                new CultureData(0x41A, "hr-HR", 1250, 28592, 1250, "hr", "Croatian (Croatia)"),
                new CultureData(0x41B, "sk-SK", 1250, 28592, 1250, "sk", "Slovak (Slovakia)"),
                new CultureData(0x41D, "sv-SE", 1252, 28591, 1252, "sv", "Swedish (Sweden)"),
                new CultureData(0x41E, "th-TH", 874, 874, 874, "th", "Thai (Thailand)"),
                new CultureData(0x41F, "tr-TR", 1254, 28599, 1254, "tr", "Turkish (Turkey)"),
                new CultureData(0x420, "ur-PK", 1256, 1256, 1256, "ur", "Urdu (Islamic Republic of Pakistan)"),
                new CultureData(0x421, "id-ID", 1252, 28591, 1252, "id", "Indonesian (Indonesia)"),
                new CultureData(0x422, "uk-UA", 1251, 21866, 1251, "uk", "Ukrainian (Ukraine)"),
                new CultureData(0x424, "sl-SI", 1250, 28592, 1250, "sl", "Slovenian (Slovenia)"),
                new CultureData(0x425, "et-EE", 1257, 28605, 28605, "et", "Estonian (Estonia)"),
                new CultureData(0x426, "lv-LV", 1257, 28603, 28603, "lv", "Latvian (Latvia)"),
                new CultureData(0x427, "lt-LT", 1257, 28603, 28603, "lt", "Lithuanian (Lithuania)"),
                new CultureData(0x429, "fa-IR", 1256, 1256, 1256, "fa", "Persian (Iran)"),
                new CultureData(0x42A, "vi-VN", 1258, 1258, 1258, "vi", "Vietnamese (Vietnam)"),
                new CultureData(0x42D, "eu-ES", 1252, 28591, 1252, "eu", "Basque (Basque)"),
                new CultureData(0x439, "hi-IN", 1200, 65001, 65001, "hi", "Hindi (India)"),
                new CultureData(0x43E, "ms-MY", 1252, 28591, 1252, "ms", "Malay (Malaysia)"),
                new CultureData(0x43F, "kk-KZ", 1251, 1251, 1251, "kk", "Kazakh (Kazakhstan)"),
                new CultureData(0x456, "gl-ES", 1252, 28591, 1252, "gl", "Galician (Galician)"),
                new CultureData(0x464, "fil-PH", 1252, 28591, 1252, "fil", "Filipino (Philippines)"),
                new CultureData(0x804, "zh-CN", 936, 936, 936, "zh-CHS", "Chinese (People's Republic of China)"),
                new CultureData(0x816, "pt-PT", 1252, 28591, 1252, "pt", "Portuguese (Portugal)"),
                new CultureData(0x81A, "sr-Latn-CS", 1250, 28592, 1250, "sr", "Serbian (Latin - Serbia and Montenegro)"),
                new CultureData(0xC04, "zh-HK", 950, 950, 950, "zh-CHT", "Chinese (Hong Kong S.A.R.)"),
                new CultureData(0xC0A, "es-ES", 1252, 28591, 1252, "es", "Spanish (Spain)"),
                new CultureData(0xC1A, "sr-Cyrl-CS", 1251, 1251, 1251, "sr", "Serbian (Cyrillic)"),
                new CultureData(0x8411, "ja-JP-Yomi", 932, 50220, 932, "ja", "Japanese (Phonetic)"),

                new CultureData(0x1, "ar", 1256, 1256, 1256, null, "Arabic"),
                new CultureData(0x2, "bg", 1251, 1251, 1251, null, "Bulgarian"),
                new CultureData(0x3, "ca", 1252, 28591, 1252, null, "Catalan"),
                new CultureData(0x4, "zh-CHS", 936, 936, 936, null, "Chinese (Simplified)"),
                new CultureData(0x5, "cs", 1250, 28592, 1250, null, "Czech"),
                new CultureData(0x6, "da", 1252, 28591, 1252, null, "Danish"),
                new CultureData(0x7, "de", 1252, 28591, 1252, null, "German"),
                new CultureData(0x8, "el", 1253, 28597, 1253, null, "Greek"),
                new CultureData(0x9, "en", 1252, 28591, 1252, null, "English"),
                new CultureData(0xA, "es", 1252, 28591, 1252, null, "Spanish"),
                new CultureData(0xB, "fi", 1252, 28591, 1252, null, "Finnish"),
                new CultureData(0xC, "fr", 1252, 28591, 1252, null, "French"),
                new CultureData(0xD, "he", 1255, 1255, 1255, null, "Hebrew"),
                new CultureData(0xE, "hu", 1250, 28592, 1250, null, "Hungarian"),
                new CultureData(0xF, "is", 1252, 28591, 1252, null, "Icelandic"),
                new CultureData(0x10, "it", 1252, 28591, 1252, null, "Italian"),
                new CultureData(0x11, "ja", 932, 50220, 932, null, "Japanese"),
                new CultureData(0x12, "ko", 949, 949, 949, null, "Korean"),
                new CultureData(0x13, "nl", 1252, 28591, 1252, null, "Dutch"),
                new CultureData(0x14, "no", 1252, 28591, 1252, null, "Norwegian"),
                new CultureData(0x15, "pl", 1250, 28592, 1250, null, "Polish"),
                new CultureData(0x16, "pt", 1252, 28591, 1252, null, "Portuguese"),
                new CultureData(0x18, "ro", 1250, 28592, 1250, null, "Romanian"),
                new CultureData(0x19, "ru", 1251, 20866, 1251, null, "Russian"),
                new CultureData(0x1A, "hr", 1250, 28592, 1250, null, "Croatian"),
                new CultureData(0x1B, "sk", 1250, 28592, 1250, null, "Slovak"),
                new CultureData(0x1D, "sv", 1252, 28591, 1252, null, "Swedish"),
                new CultureData(0x1E, "th", 874, 874, 874, null, "Thai"),
                new CultureData(0x1F, "tr", 1254, 28599, 1254, null, "Turkish"),
                new CultureData(0x20, "ur", 1256, 1256, 1256, null, "Urdu"),
                new CultureData(0x21, "id", 1252, 28591, 1252, null, "Indonesian"),
                new CultureData(0x22, "uk", 1251, 21866, 1251, null, "Ukrainian"),
                new CultureData(0x24, "sl", 1250, 28592, 1250, null, "Slovenian"),
                new CultureData(0x25, "et", 1257, 28605, 28605, null, "Estonian"),
                new CultureData(0x26, "lv", 1257, 28603, 28603, null, "Latvian"),
                new CultureData(0x27, "lt", 1257, 28603, 28603, null, "Lithuanian"),
                new CultureData(0x29, "fa", 1256, 1256, 1256, null, "Persian"),
                new CultureData(0x2A, "vi", 1258, 1258, 1258, null, "Vietnamese"),
                new CultureData(0x2D, "eu", 1252, 28591, 1252, null, "Basque"),
                new CultureData(0x39, "hi", 1200, 65001, 65001, null, "Hindi"),
                new CultureData(0x3E, "ms", 1252, 28591, 1252, null, "Malay"),
                new CultureData(0x3F, "kk", 1251, 1251, 1251, null, "Kazakh"),
                new CultureData(0x56, "gl", 1252, 28591, 1252, null, "Galician"),
                new CultureData(0x64, "fil", 1252, 28591, 1252, null, "Filipino"),
                new CultureData(0x7C04, "zh-CHT", 950, 950, 950, null, "Chinese (Traditional)"),
                new CultureData(0x7C1A, "sr", 1251, 1251, 1251, null, "Serbian"),
            };

            var newData = new GlobalizationData();

            Culture invariantCulture = null;
            Culture culture;
            Charset charset;

            

            var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);

            foreach (var ci in cultures)
            {
                
                InternalDebug.Assert(!newData.lcidToCulture.ContainsKey(ci.LCID),
                    $"Culture LCID {ci.LCID:X} has more than one entry in CLR (\"{ci.Name}\")");

                if (!newData.lcidToCulture.TryGetValue(ci.LCID, out culture))
                {
                    culture = new Culture(ci.LCID, ci.Name);

                    newData.lcidToCulture.Add(ci.LCID, culture);
                    culture.SetDescription(ci.EnglishName);
                    culture.SetNativeDescription(ci.NativeName);
                    culture.SetCultureInfo(ci);

                    if (ci.LCID == 127)
                    {
                        
                        invariantCulture = culture;
                    }
                }
                

                InternalDebug.Assert(!newData.nameToCulture.ContainsKey(ci.Name),
                    $"Culture name \"{ci.Name}\" is already reserved in CLR for another LCID ({ci.LCID:X})");

                if (!newData.nameToCulture.ContainsKey(ci.Name))
                {
                    
                    newData.nameToCulture.Add(ci.Name, culture);
                }
                
            }

            
            
            

            foreach (var cd in hardcodedCultures)
            {
                if (!newData.lcidToCulture.TryGetValue(cd.LCID, out culture))
                {
                    culture = new Culture(cd.LCID, cd.Name);

                    newData.lcidToCulture.Add(cd.LCID, culture);
                    culture.SetDescription(cd.Description);
                    culture.SetNativeDescription(cd.Description);
                }

                if (!newData.nameToCulture.ContainsKey(cd.Name))
                {
                    
                    newData.nameToCulture.Add(cd.Name, culture);
                }
            }

            InternalDebug.Assert(invariantCulture != null);

            newData.invariantCulture = invariantCulture;

            

            foreach (var ci in cultures)
            {
                
                culture = newData.lcidToCulture[ci.LCID];

                if (ci.Parent != null)
                {
                    Culture parentCulture;
                    if (newData.lcidToCulture.TryGetValue(ci.Parent.LCID, out parentCulture))
                    {
                        culture.SetParentCulture(parentCulture);
                    }
                    else
                    {
                        
                        
                        
                        
                        

                        
                        InternalDebug.Assert(false);
                        culture.SetParentCulture(culture);
                    }
                }
                else
                {
                    culture.SetParentCulture(culture);
                }
            }

            foreach (var cd in hardcodedCultures)
            {
                culture = newData.lcidToCulture[cd.LCID];

                if (culture.ParentCulture == null)
                {
                    if (cd.ParentCultureName != null)
                    {
                        Culture parentCulture;
                        if (newData.nameToCulture.TryGetValue(cd.ParentCultureName, out parentCulture))
                        {
                            culture.SetParentCulture(parentCulture);
                        }
                        else
                        {
                            
                            
                            InternalDebug.Assert(false);
                            culture.SetParentCulture(culture);
                        }
                    }
                    else
                    {
                        culture.SetParentCulture(culture);
                    }
                }
            }

            

            var encodings = Encoding.GetEncodings();

            foreach (var ei in encodings)
            {
                InternalDebug.Assert(!newData.codePageToCharset.ContainsKey(ei.CodePage),
                    $"CodePage {ei.CodePage} has more than one entry in CLR (\"{ei.Name}\")");

                if (!newData.codePageToCharset.TryGetValue(ei.CodePage, out charset))
                {
                    charset = new Charset(ei.CodePage, ei.Name);

                    charset.SetDescription(ei.DisplayName);

                    newData.codePageToCharset.Add(ei.CodePage, charset);
                }

                if (!newData.nameToCharset.ContainsKey(ei.Name))
                {
                    newData.nameToCharset.Add(ei.Name, charset);
                    if (ei.Name.Length > newData.maxCharsetNameLength)
                    {
                        newData.maxCharsetNameLength = ei.Name.Length;
                    }
                }
            }

            

            foreach (var wcp in windowsCodePages)
            {
                if (wcp.LCID == 0)
                {
                    InternalDebug.Assert(wcp.CultureName == null);

                    if (wcp.CodePage == 1200 && invariantCulture != null)
                    {
                        culture = invariantCulture;
                    }
                    else
                    {
                        
                        culture = new Culture(0, null);

                        culture.SetParentCulture(invariantCulture);
                    }

                    culture.SetDescription(wcp.GenericCultureDescription);

                    
                    
                }
                else
                {
                    InternalDebug.Assert(wcp.CultureName != null);

                    if (!newData.lcidToCulture.TryGetValue(wcp.LCID, out culture))
                    {
                        
                        

                        InternalDebug.Assert(false);

                        if (!newData.nameToCulture.TryGetValue(wcp.CultureName, out culture))
                        {
                            culture = new Culture(wcp.LCID, wcp.CultureName);

                            newData.lcidToCulture.Add(wcp.LCID, culture);
                            newData.nameToCulture.Add(wcp.CultureName, culture);
                        }
                    }
                }

                if (!newData.codePageToCharset.TryGetValue(wcp.CodePage, out charset))
                {
                    
                    InternalDebug.Assert(false);

                    charset = new Charset(wcp.CodePage, wcp.Name);

                    newData.nameToCharset.Add(charset.Name, charset);
                    newData.codePageToCharset.Add(charset.CodePage, charset);

                    if (charset.Name.Length > newData.maxCharsetNameLength)
                    {
                        newData.maxCharsetNameLength = charset.Name.Length;
                    }
                }

                charset.SetWindows();

                
                culture.SetWindowsCharset(charset);
                charset.SetCulture(culture);

                if (!newData.codePageToCharset.TryGetValue(wcp.MimeCodePage, out charset))
                {
                    

                    
                    
                    charset = newData.codePageToCharset[wcp.CodePage];
                }

                culture.SetMimeCharset(charset);

                if (!newData.codePageToCharset.TryGetValue(wcp.WebCodePage, out charset))
                {
                    

                    
                    
                    charset = newData.codePageToCharset[wcp.CodePage];
                }

                culture.SetWebCharset(charset);
            }

            
            
            

            foreach (var csn in charsetNames)
            {
                if (!newData.nameToCharset.TryGetValue(csn.Name, out charset))
                {
                    if (newData.codePageToCharset.TryGetValue(csn.CodePage, out charset))
                    {
                        newData.nameToCharset.Add(csn.Name, charset);
                        if (csn.Name.Length > newData.maxCharsetNameLength)
                        {
                            newData.maxCharsetNameLength = csn.Name.Length;
                        }
                    }
                }
                else
                {
                    if (charset.CodePage != csn.CodePage)
                    {
                        
                        
                        
                        
                        

                        
                        
                        
                        

                        

                        if (newData.codePageToCharset.TryGetValue(csn.CodePage, out charset))
                        {
                            newData.nameToCharset[csn.Name] = charset;
                        }
                    }
                }

                InternalDebug.Assert(charset == null || charset.CodePage == csn.CodePage);
            }

            
            
            

            for (var i = 0; i < CodePageMapData.codePages.Length; i++)
            {
                if (newData.codePageToCharset.TryGetValue(CodePageMapData.codePages[i].cpid, out charset))
                {
                    charset.SetMapIndex(i);
                }

                
                if (charset.Culture == null)
                {
                    
                    
                    var windowsCharset = newData.codePageToCharset[CodePageMapData.codePages[i].windowsCpid];

                    charset.SetCulture(windowsCharset.Culture);
                }
            }


            

            foreach (var over in cultureCodePageOverrides)
            {
                if (newData.nameToCulture.TryGetValue(over.CultureName, out culture))
                {
                    if (newData.codePageToCharset.TryGetValue(over.MimeCodePage, out charset))
                    {
                        

                        culture.SetMimeCharset(charset);
                    }

                    if (newData.codePageToCharset.TryGetValue(over.WebCodePage, out charset))
                    {
                        

                        culture.SetWebCharset(charset);
                    }
                }
            }

            

            foreach (var ci in cultures)
            {
                InternalDebug.Assert(newData.lcidToCulture.ContainsKey(ci.LCID));

                
                culture = newData.lcidToCulture[ci.LCID];

                if (culture.WindowsCharset == null)
                {
                    var windowsCodePage = ci.TextInfo.ANSICodePage;

                    if (windowsCodePage <= 500)
                    {
                        windowsCodePage = 1200;     
                    }
                    else
                    {
                        
                        
                        var validWindowsCodepage = false;

                        foreach (var wcp in windowsCodePages)
                        {
                            if (windowsCodePage == wcp.CodePage)
                            {
                                validWindowsCodepage = true;
                                break;
                            }
                        }

                        if (!validWindowsCodepage)
                        {
                            
                            windowsCodePage = 1200;
                        }
                    }

                    
                    
                    charset = newData.codePageToCharset[windowsCodePage];

                    
                    culture.SetWindowsCharset(charset);

                    if (culture != culture.ParentCulture)
                    {
                        if (culture.WindowsCharset == culture.ParentCulture.WindowsCharset)
                        {
                            culture.SetMimeCharset(culture.ParentCulture.MimeCharset);
                            culture.SetWebCharset(culture.ParentCulture.WebCharset);
                        }
                    }

                    if (culture.MimeCharset == null)
                    {
                        culture.SetMimeCharset(charset.Culture.MimeCharset);
                    }

                    if (culture.WebCharset == null)
                    {
                        culture.SetWebCharset(charset.Culture.WebCharset);
                    }
                }
            }

            

            foreach (var cd in hardcodedCultures)
            {
                InternalDebug.Assert(newData.lcidToCulture.ContainsKey(cd.LCID));

                
                culture = newData.lcidToCulture[cd.LCID];

                if (culture.WindowsCharset == null)
                {
                    var windowsCodePage = cd.WindowsCodePage;

                    InternalDebug.Assert(windowsCodePage > 500);

                    
                    charset = newData.codePageToCharset[windowsCodePage];

                    
                    culture.SetWindowsCharset(charset);

                    Charset otherCharset;

                    if (newData.codePageToCharset.TryGetValue(cd.MimeCodePage, out otherCharset))
                    {
                        culture.SetMimeCharset(otherCharset);
                    }

                    if (newData.codePageToCharset.TryGetValue(cd.WebCodePage, out otherCharset))
                    {
                        culture.SetWebCharset(otherCharset);
                    }

                    if (culture != culture.ParentCulture)
                    {
                        if (culture.WindowsCharset == culture.ParentCulture.WindowsCharset)
                        {
                            if (culture.MimeCharset == null)
                            {
                                culture.SetMimeCharset(culture.ParentCulture.MimeCharset);
                            }

                            if (culture.WebCharset == null)
                            {
                                culture.SetWebCharset(culture.ParentCulture.WebCharset);
                            }
                        }
                    }

                    if (culture.MimeCharset == null)
                    {
                        culture.SetMimeCharset(charset.Culture.MimeCharset);
                    }

                    if (culture.WebCharset == null)
                    {
                        culture.SetWebCharset(charset.Culture.WebCharset);
                    }
                }
            }

            

            foreach (var ei in encodings)
            {
                
                
                charset = newData.codePageToCharset[ei.CodePage];

                if (charset.Culture == null)
                {
                    
                    
                    
                    
                    
                    

                    Encoding encoding;
                    Charset windowsCharset;
                    var windowsCodePage = 1200;  

                    if (charset.TryGetEncoding(out encoding))
                    {
                        windowsCodePage = encoding.WindowsCodePage;
                        var validWindowsCodepage = false;

                        foreach (var wcp in windowsCodePages)
                        {
                            if (windowsCodePage == wcp.CodePage)
                            {
                                validWindowsCodepage = true;
                                break;
                            }
                        }

                        if (!validWindowsCodepage)
                        {
                            
                            
                            
                            windowsCodePage = 1200;
                        }
                    }

                    
                    
                    windowsCharset = newData.codePageToCharset[windowsCodePage];

                    charset.SetCulture(windowsCharset.Culture);
                }
            }

            

            foreach (var over in codePageCultureOverrides)
            {
                if (newData.codePageToCharset.TryGetValue(over.CodePage, out charset))
                {
                    if (newData.nameToCulture.TryGetValue(over.CultureName, out culture))
                    {
                        charset.SetCulture(culture);
                    }
                }
            }

            
            if (!newData.lcidToCulture.TryGetValue(CultureInfo.CurrentUICulture.LCID, out newData.defaultCulture))
            {
                

                if (!newData.lcidToCulture.TryGetValue(CultureInfo.CurrentCulture.LCID, out newData.defaultCulture))
                {
                    
                    
                    newData.defaultCulture = newData.lcidToCulture[0x409];
                }
            }

            

            
            
            
            
            
            

            
            

            /* example of the configuration:

            <Exchange.Data.Globalization>

                <AddCharsetAliasName Charset="us-ascii" AliasName="foo"/>
                <OverrideCharsetDefaultName Charset="us-ascii" DefaultName="foo"/>
                <OverrideCharsetCulture Charset="us-ascii" Culture="jp-JP"/>

                <AddCulture LocaleId="1234" Name="xx-YY" WindowsCharset="1255" MimeCharset="gb-18030" WebCharset="utf-8"/>
                <AddCultureAliasName Culture="xx-YY" AliasName="xx-FOO"/>
                <OverrideCultureCharset Culture="en-US" WindowsCharset="1257" MimeCharset="65001" WebCharset="ascii"/>

                <DefaultCulture Culture="MMM"/>

            </Exchange.Data.Globalization>

            */

            string charSetArg = null;
            string aliasNameArg = null;
            string defaultNameArg = null;
            string cultureArg = null;
            string windowsCharsetArg = null;
            string mimeCharsetArg = null;
            string webCharsetArg = null;
            int codePage;
            int lcid;
            Charset charset1;
            Culture culture1;

            if (defaultCultureName != null)
            {
                
                newData.defaultCulture = newData.nameToCulture[defaultCultureName];
            }

            
            newData.defaultDetectionPriorityOrder = GetCultureSpecificCodepageDetectionPriorityOrder(newData.defaultCulture, null);
            invariantCulture.SetCodepageDetectionPriorityOrder(newData.defaultDetectionPriorityOrder);
            newData.defaultCulture.GetCodepageDetectionPriorityOrder(newData);

            newData.asciiCharset = newData.codePageToCharset[20127];
            newData.utf8Charset = newData.codePageToCharset[65001];
            newData.unicodeCharset = newData.codePageToCharset[1200];

            return newData;
        }

        internal static int[] GetCultureSpecificCodepageDetectionPriorityOrder(Culture culture, int[] originalPriorityList)
        {
            var newPriorityList = new int[CodePageDetectData.codePages.Length];
            var newPriorityListIndex = 0;

            

            newPriorityList[newPriorityListIndex++] = 20127;

            

            if (culture.MimeCharset.IsDetectable && !IsDbcs(culture.MimeCharset.CodePage) && !InList(culture.MimeCharset.CodePage, newPriorityList, newPriorityListIndex))
            {
                newPriorityList[newPriorityListIndex++] = culture.MimeCharset.CodePage;
            }

            if (culture.WebCharset.IsDetectable && !IsDbcs(culture.WebCharset.CodePage) && !InList(culture.WebCharset.CodePage, newPriorityList, newPriorityListIndex))
            {
                newPriorityList[newPriorityListIndex++] = culture.WebCharset.CodePage;
            }

            if (culture.WindowsCharset.IsDetectable && !IsDbcs(culture.WindowsCharset.CodePage) && !InList(culture.WindowsCharset.CodePage, newPriorityList, newPriorityListIndex))
            {
                newPriorityList[newPriorityListIndex++] = culture.WindowsCharset.CodePage;
            }

            
            
            

            if (originalPriorityList != null)
            {
                for (var i = 0; i < originalPriorityList.Length; i++)
                {
                    if (!IsDbcs(originalPriorityList[i]) && !InList(originalPriorityList[i], newPriorityList, newPriorityListIndex) && IsSameLanguage(originalPriorityList[i], culture.WindowsCharset.CodePage))
                    {
                        newPriorityList[newPriorityListIndex++] = originalPriorityList[i];
                    }
                }
            }
            else
            {
                for (var i = 0; i < CodePageDetectData.codePages.Length; i++)
                {
                    if (!IsDbcs(CodePageDetectData.codePages[i].cpid) && !InList(CodePageDetectData.codePages[i].cpid, newPriorityList, newPriorityListIndex) && IsSameLanguage(CodePageDetectData.codePages[i].cpid, culture.WindowsCharset.CodePage))
                    {
                        newPriorityList[newPriorityListIndex++] = CodePageDetectData.codePages[i].cpid;
                    }
                }
            }

            

            if (originalPriorityList != null)
            {
                for (var i = 0; i < originalPriorityList.Length; i++)
                {
                    if (!IsDbcs(originalPriorityList[i]) && !InList(originalPriorityList[i], newPriorityList, newPriorityListIndex))
                    {
                        newPriorityList[newPriorityListIndex++] = originalPriorityList[i];
                    }
                }
            }
            else
            {
                for (var i = 0; i < CodePageDetectData.codePages.Length; i++)
                {
                    if (!IsDbcs(CodePageDetectData.codePages[i].cpid) && !InList(CodePageDetectData.codePages[i].cpid, newPriorityList, newPriorityListIndex))
                    {
                        newPriorityList[newPriorityListIndex++] = CodePageDetectData.codePages[i].cpid;
                    }
                }
            }

            

            if (culture.MimeCharset.IsDetectable && IsDbcs(culture.MimeCharset.CodePage) && !InList(culture.MimeCharset.CodePage, newPriorityList, newPriorityListIndex))
            {
                newPriorityList[newPriorityListIndex++] = culture.MimeCharset.CodePage;
            }

            if (culture.WebCharset.IsDetectable && IsDbcs(culture.WebCharset.CodePage) && !InList(culture.WebCharset.CodePage, newPriorityList, newPriorityListIndex))
            {
                newPriorityList[newPriorityListIndex++] = culture.WebCharset.CodePage;
            }

            if (culture.WindowsCharset.IsDetectable && IsDbcs(culture.WindowsCharset.CodePage) && !InList(culture.WindowsCharset.CodePage, newPriorityList, newPriorityListIndex))
            {
                newPriorityList[newPriorityListIndex++] = culture.WindowsCharset.CodePage;
            }

            
            
            

            if (originalPriorityList != null)
            {
                for (var i = 0; i < originalPriorityList.Length; i++)
                {
                    if (IsDbcs(originalPriorityList[i]) && !InList(originalPriorityList[i], newPriorityList, newPriorityListIndex) && IsSameLanguage(originalPriorityList[i], culture.WindowsCharset.CodePage))
                    {
                        newPriorityList[newPriorityListIndex++] = originalPriorityList[i];
                    }
                }
            }
            else
            {
                for (var i = 0; i < CodePageDetectData.codePages.Length; i++)
                {
                    if (IsDbcs(CodePageDetectData.codePages[i].cpid) && !InList(CodePageDetectData.codePages[i].cpid, newPriorityList, newPriorityListIndex) && IsSameLanguage(CodePageDetectData.codePages[i].cpid, culture.WindowsCharset.CodePage))
                    {
                        newPriorityList[newPriorityListIndex++] = CodePageDetectData.codePages[i].cpid;
                    }
                }
            }

            

            if (originalPriorityList != null)
            {
                for (var i = 0; i < originalPriorityList.Length; i++)
                {
                    if (!InList(originalPriorityList[i], newPriorityList, newPriorityListIndex))
                    {
                        newPriorityList[newPriorityListIndex++] = originalPriorityList[i];
                    }
                }
            }
            else
            {
                for (var i = 0; i < CodePageDetectData.codePages.Length; i++)
                {
                    if (!InList(CodePageDetectData.codePages[i].cpid, newPriorityList, newPriorityListIndex))
                    {
                        newPriorityList[newPriorityListIndex++] = CodePageDetectData.codePages[i].cpid;
                    }
                }
            }

            
            InternalDebug.Assert(newPriorityListIndex == CodePageDetectData.codePages.Length);

            if (originalPriorityList != null)
            {
                

                for (var i = 0; i < originalPriorityList.Length; i++)
                {
                    if (newPriorityList[i] != originalPriorityList[i])
                    {
                        
                        return newPriorityList;
                    }
                }

                
                return originalPriorityList;
            }

            
            return newPriorityList;
        }

        internal static int[] GetAdjustedCodepageDetectionPriorityOrder(Charset charset, int[] originalPriorityList)
        {
            if (!charset.IsDetectable && originalPriorityList != null)
            {
                return originalPriorityList;
            }

            var newPriorityList = new int[CodePageDetectData.codePages.Length];
            var newPriorityListIndex = 0;

            

            newPriorityList[newPriorityListIndex++] = 20127;

            

            if (charset.IsDetectable && !IsDbcs(charset.CodePage) && !InList(charset.CodePage, newPriorityList, newPriorityListIndex))
            {
                newPriorityList[newPriorityListIndex++] = charset.CodePage;
            }

            
            
            

            if (originalPriorityList != null)
            {
                for (var i = 0; i < originalPriorityList.Length; i++)
                {
                    if (!IsDbcs(originalPriorityList[i]) && !InList(originalPriorityList[i], newPriorityList, newPriorityListIndex) && IsSameLanguage(originalPriorityList[i], charset.Culture.WindowsCharset.CodePage))
                    {
                        newPriorityList[newPriorityListIndex++] = originalPriorityList[i];
                    }
                }
            }
            else
            {
                for (var i = 0; i < CodePageDetectData.codePages.Length; i++)
                {
                    if (!IsDbcs(CodePageDetectData.codePages[i].cpid) && !InList(CodePageDetectData.codePages[i].cpid, newPriorityList, newPriorityListIndex) && IsSameLanguage(CodePageDetectData.codePages[i].cpid, charset.Culture.WindowsCharset.CodePage))
                    {
                        newPriorityList[newPriorityListIndex++] = CodePageDetectData.codePages[i].cpid;
                    }
                }
            }

            

            if (originalPriorityList != null)
            {
                for (var i = 0; i < originalPriorityList.Length; i++)
                {
                    if (!IsDbcs(originalPriorityList[i]) && !InList(originalPriorityList[i], newPriorityList, newPriorityListIndex))
                    {
                        newPriorityList[newPriorityListIndex++] = originalPriorityList[i];
                    }
                }
            }
            else
            {
                for (var i = 0; i < CodePageDetectData.codePages.Length; i++)
                {
                    if (!IsDbcs(CodePageDetectData.codePages[i].cpid) && !InList(CodePageDetectData.codePages[i].cpid, newPriorityList, newPriorityListIndex))
                    {
                        newPriorityList[newPriorityListIndex++] = CodePageDetectData.codePages[i].cpid;
                    }
                }
            }

            

            if (charset.IsDetectable && IsDbcs(charset.CodePage) && !InList(charset.CodePage, newPriorityList, newPriorityListIndex))
            {
                newPriorityList[newPriorityListIndex++] = charset.CodePage;
            }

            

            if (originalPriorityList != null)
            {
                for (var i = 0; i < originalPriorityList.Length; i++)
                {
                    if (IsDbcs(originalPriorityList[i]) && !InList(originalPriorityList[i], newPriorityList, newPriorityListIndex) && IsSameLanguage(originalPriorityList[i], charset.Culture.WindowsCharset.CodePage))
                    {
                        newPriorityList[newPriorityListIndex++] = originalPriorityList[i];
                    }
                }
            }
            else
            {
                for (var i = 0; i < CodePageDetectData.codePages.Length; i++)
                {
                    if (IsDbcs(CodePageDetectData.codePages[i].cpid) && !InList(CodePageDetectData.codePages[i].cpid, newPriorityList, newPriorityListIndex) && IsSameLanguage(CodePageDetectData.codePages[i].cpid, charset.Culture.WindowsCharset.CodePage))
                    {
                        newPriorityList[newPriorityListIndex++] = CodePageDetectData.codePages[i].cpid;
                    }
                }
            }

            

            if (originalPriorityList != null)
            {
                for (var i = 0; i < originalPriorityList.Length; i++)
                {
                    if (!InList(originalPriorityList[i], newPriorityList, newPriorityListIndex))
                    {
                        newPriorityList[newPriorityListIndex++] = originalPriorityList[i];
                    }
                }
            }
            else
            {
                for (var i = 0; i < CodePageDetectData.codePages.Length; i++)
                {
                    if (!InList(CodePageDetectData.codePages[i].cpid, newPriorityList, newPriorityListIndex))
                    {
                        newPriorityList[newPriorityListIndex++] = CodePageDetectData.codePages[i].cpid;
                    }
                }
            }

            
            InternalDebug.Assert(newPriorityListIndex == CodePageDetectData.codePages.Length);

            if (originalPriorityList != null)
            {
                

                for (var i = 0; i < originalPriorityList.Length; i++)
                {
                    if (newPriorityList[i] != originalPriorityList[i])
                    {
                        
                        return newPriorityList;
                    }
                }

                
                return originalPriorityList;
            }

            
            return newPriorityList;
        }

        private static bool IsDbcs(int codePage)
        {
            switch (codePage)
            {
                case 932:       
                case 949:       
                case 950:       
                case 936:       
                case 50220:     
                case 51932:     
                case 51949:     
                case 50225:     
                case 52936:     
                    return true;
            }
            return false;
        }

        private static bool InList(int codePage, int[] list, int listCount)
        {
            for (var i = 0; i < listCount; i++)
            {
                if (list[i] == codePage)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsSameLanguage(int codePage, int windowsCodePage)
        {
            return windowsCodePage == codePage ||
                (windowsCodePage == 1250 && codePage == 28592) ||
                (windowsCodePage == 1251 && (codePage == 28595 || codePage == 20866 || codePage == 21866)) ||
                (windowsCodePage == 1252 && (codePage == 28591 || codePage == 28605)) ||
                (windowsCodePage == 1253 && codePage == 28597) ||
                (windowsCodePage == 1254 && codePage == 28599) ||
                (windowsCodePage == 1255 && codePage == 38598) ||
                (windowsCodePage == 1256 && codePage == 28596) ||
                (windowsCodePage == 1257 && codePage == 28594) ||
                (windowsCodePage == 932 && (codePage == 50220 || codePage == 51932)) ||
                (windowsCodePage == 949 &&  (codePage == 50225 || codePage == 51949)) ||
                (windowsCodePage == 936 &  codePage == 52936);
        }

        private class IntComparer : IEqualityComparer<int>
        {
            public bool Equals(int x, int y)
            {
                return x == y;
            }
        
            public int GetHashCode(int obj)
            {
                return obj;
            }
        }
    }


    
    [Serializable]
    internal class RemapEncoding : Encoding
    {
        Encoding encodingEncoding;
        Encoding decodingEncoding;

        public RemapEncoding(int codePage) : base(codePage)
        {
            if (codePage == 28591)
            {
                encodingEncoding = GetEncoding(28591);
                decodingEncoding = GetEncoding(1252);
            }
            else if (codePage == 28599)
            {
                encodingEncoding = GetEncoding(28599);
                decodingEncoding = GetEncoding(1254);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public override int CodePage => encodingEncoding.CodePage;
        public override String BodyName => encodingEncoding.BodyName;
        public override String EncodingName => encodingEncoding.EncodingName;
        public override String HeaderName => encodingEncoding.HeaderName;
        public override String WebName => encodingEncoding.WebName;
        public override int WindowsCodePage => encodingEncoding.WindowsCodePage;
        public override bool IsBrowserDisplay => encodingEncoding.IsBrowserDisplay;
        public override bool IsBrowserSave => encodingEncoding.IsBrowserSave;
        public override bool IsMailNewsDisplay => encodingEncoding.IsMailNewsDisplay;
        public override bool IsMailNewsSave => encodingEncoding.IsMailNewsSave;
        public override bool IsSingleByte => encodingEncoding.IsSingleByte;

        public override byte[] GetPreamble()
        {
            return encodingEncoding.GetPreamble();
        }

        public override int GetMaxByteCount(int charCount)
        {
            return encodingEncoding.GetMaxByteCount(charCount);
        }

        public override int GetMaxCharCount(int byteCount)
        {
            return decodingEncoding.GetMaxCharCount(byteCount);
        }

        public override int GetByteCount(char[] chars, int index, int count)
        {
            return encodingEncoding.GetByteCount(chars, index, count);
        }

        public override int GetByteCount(String s)
        {
            return encodingEncoding.GetByteCount(s);
        }       

        public override unsafe int GetByteCount(char* chars, int count)
        {
            return encodingEncoding.GetByteCount(chars, count);
        }

        public override int GetBytes(String s, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            return encodingEncoding.GetBytes(s, charIndex, charCount, bytes, byteIndex);
        }
    
        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            return encodingEncoding.GetBytes(chars, charIndex, charCount, bytes, byteIndex);
        }

        public override unsafe int GetBytes(char* chars, int charCount, byte* bytes, int byteCount)
        {
            return encodingEncoding.GetBytes(chars, charCount, bytes, byteCount);
        }                                              

        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            return decodingEncoding.GetCharCount(bytes, index, count);
        }

        public override unsafe int GetCharCount(byte* bytes, int count)
        {
            return decodingEncoding.GetCharCount(bytes, count);
        }        

        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            return decodingEncoding.GetChars(bytes, byteIndex, byteCount, chars, charIndex);
        }

        public override unsafe int GetChars(byte* bytes, int byteCount, char* chars, int charCount)
        {
            return decodingEncoding.GetChars(bytes, byteCount, chars, charCount);
        }
    
        public override String GetString(byte[] bytes, int index, int count)
        {
            return decodingEncoding.GetString(bytes, index, count);
        }

        public override Decoder GetDecoder()
        {
            return decodingEncoding.GetDecoder();
        }

        public override Encoder GetEncoder()
        {
            return encodingEncoding.GetEncoder();
        }

        public override Object Clone()
        {
            var newEncoding = (Encoding)MemberwiseClone();
            return newEncoding;
        }
    }

    

    internal class AsciiEncoderFallback : EncoderFallback
    {
        public override EncoderFallbackBuffer CreateFallbackBuffer()
        {
            return new AsciiFallbackBuffer();
        }

        
        public override int MaxCharCount => AsciiFallbackBuffer.MaxCharCount;

        public static string GetCharacterFallback(char charUnknown)
        {
            
            

            
            

            if (charUnknown <= 0x153 && charUnknown >= 0x82)
            {
                switch ((int)charUnknown)
                {
                    

                    case 0x0083:    
                        return "f";

                    case 0x0085:    
                        return "...";

                    case 0x008B:    
                        return "<";

                    case 0x008C:    
                        return "OE";

                    case 0x009B:    
                        return ">";

                    case 0x009C:    
                        return "oe";

                    case 0x0082:    
                    case 0x0091:    
                    case 0x0092:    
                        return "'";

                    case 0x0084:    
                    case 0x0093:    
                    case 0x0094:    
                        return "\"";

                    case 0x0095:    
                        return "*";

                    case 0x0096:    
                        return "-";

                    case 0x0097:    
                        return "-";

                    case 0x0098:    
                        return "~";

                    case 0x0099:    
                        return "(tm)";

                    

                    case 0x00A0:    
                        return " ";

                    case 0x00A2:    
                        return "c";

                    case 0x00A4:    
                        return "$";

                    case 0x00A5:    
                        return "Y";

                    case 0x00A6:    
                        return "|";

                    case 0x00A9:    
                        return "(c)";

                    case 0x00AB:    
                        return "<";

                    case 0x00AD:    
                        return "";

                    case 0x00AE:    
                        return "(r)";

                    case 0x00B2:    
                        return "^2";

                    case 0x00B3:    
                        return "^3";

                    case 0x00B7:    
                        return "*";

                    case 0x00B8:    
                        return ",";

                    case 0x00B9:    
                        return "^1";

                    case 0x00BB:    
                        return ">";

                    case 0x00BC:    
                        return "(1/4)";

                    case 0x00BD:    
                        return "(1/2)";

                    case 0x00BE:    
                        return "(3/4)";

                    case 0x00C6:    
                        return "AE";

                    case 0x00E6:    
                        return "ae";

                    case 0x0132:    
                        return "IJ";

                    case 0x0133:    
                        return "ij";

                    case 0x0152:    
                        return "OE";

                    case 0x0153:    
                        return "oe";
                }
            }
            else if (charUnknown >= 0x2002 && charUnknown <= 0x2122)
            {
                switch ((int)charUnknown)
                {
                    case 0x2002:    
                    case 0x2003:    
                        return " ";

                    case 0x2011:    
                        return "-";

                    case 0x2013:    
                    case 0x2014:    
                        return "-";

                    case 0x2018:    
                    case 0x2019:    
                    case 0x201A:    
                        return "'";

                    case 0x201C:    
                    case 0x201D:    
                    case 0x201E:    
                        return "\"";

                    case 0x2022:    
                        return "*";

                    case 0x2026:    
                        return "...";

                    case 0x2039:    
                        return "<";

                    case 0x203A:    
                        return ">";

                    case 0x20AC:    
                        return "EUR";

                    case 0x2122:    
                        return "(tm)";
                }
            }
            else if (charUnknown >= 0x2639 && charUnknown <= 0x263A)
            {
                switch ((int)charUnknown)
                {
                    case 0x263A:    
                        return ":)";
                    case 0x2639:    
                        return ":(";
                }
            }

            return null;
        }


        

        private class AsciiFallbackBuffer : EncoderFallbackBuffer
        {
            private int fallbackIndex;
            private string fallbackString;

            public static int MaxCharCount => 5;


            public override bool Fallback(char charUnknown, int index)
            {
                InternalDebug.Assert(fallbackString == null || fallbackIndex == fallbackString.Length);

                fallbackIndex = 0;

                fallbackString = GetCharacterFallback(charUnknown);

                if (fallbackString == null)
                {
                    
                    
                    fallbackString = "?";
                }

                return true;
            }

            public override bool Fallback(char charUnknownHigh, char charUnknownLow, int index)
            {
                
                InternalDebug.Assert(Char.IsHighSurrogate(charUnknownHigh) && Char.IsLowSurrogate(charUnknownLow));
                InternalDebug.Assert(fallbackString == null || fallbackIndex == fallbackString.Length);

                fallbackIndex = 0;

                
                fallbackString = "?";

                
                return true;
            }

            public override char GetNextChar()
            {
                if (fallbackString == null || fallbackIndex == fallbackString.Length)
                {
                    return '\0';
                }

                return fallbackString[fallbackIndex++];
            }

            public override bool MovePrevious()
            {
                
                if (fallbackIndex > 0)
                {
                    fallbackIndex--;
                    return true;
                }

                
                return false;
            }

            
            public override int Remaining => fallbackString == null ? 0 : fallbackString.Length - fallbackIndex;


            public override void Reset()
            {
                fallbackString = "?";
                fallbackIndex = 0;
            }
        }
    }
}
