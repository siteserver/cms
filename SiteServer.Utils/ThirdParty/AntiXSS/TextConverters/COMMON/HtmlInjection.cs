// ***************************************************************
// <copyright file="HtmlInjection.cs" company="Microsoft">
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
    using Data.Internal;
    using Internal.Text;
    using Internal.Html;


    internal class HtmlInjection : Injection
    {
        protected bool filterHtml;
        protected HtmlTagCallback callback;

        protected bool injectingHead;

        protected IProgressMonitor progressMonitor;

        protected IHtmlParser documentParser;
        protected HtmlParser fragmentParser;

        protected HtmlToHtmlConverter fragmentToHtmlConverter;
        protected HtmlToTextConverter fragmentToTextConverter;

        public HtmlInjection(
                    string injectHead, 
                    string injectTail, 
                    HeaderFooterFormat injectionFormat,
                    bool filterHtml,
                    HtmlTagCallback callback,
                    bool testBoundaryConditions,
                    Stream traceStream,
                    IProgressMonitor progressMonitor)
        {
            InternalDebug.Assert(progressMonitor != null);
            this.injectHead = injectHead;
            this.injectTail = injectTail;
            this.injectionFormat = injectionFormat;
            this.filterHtml = filterHtml;
            this.callback = callback;
            this.testBoundaryConditions = testBoundaryConditions;
#if DEBUG
            this.traceStream = traceStream;
#endif
            this.progressMonitor = progressMonitor;
        }

        public bool Active => documentParser != null;
        public bool InjectingHead { get { InternalDebug.Assert(Active); return injectingHead; } }

        public IHtmlParser Push(bool head, IHtmlParser documentParser)
        {
            if (head)
            {
                if (injectHead != null && !headInjected)
                {
                    this.documentParser = documentParser;

                    if (fragmentParser == null)
                    {
                        
                        fragmentParser = new HtmlParser(
                                        new ConverterBufferInput(injectHead, progressMonitor), 
                                        false,  
                                        (injectionFormat == HeaderFooterFormat.Text), 
                                        64,     
                                        8,      
                                        testBoundaryConditions);
                    }
                    else
                    {
                        fragmentParser.Initialize(
                                    injectHead, 
                                    (injectionFormat == HeaderFooterFormat.Text));
                    }

                    injectingHead = true;

                    return fragmentParser;
                }
            }
            else
            {
                if (injectHead != null && !headInjected)
                {
                    
                    InternalDebug.Assert(false);
                    

                    headInjected = true;
                }

                if (injectTail != null && !tailInjected)
                {
                    this.documentParser = documentParser;

                    if (fragmentParser == null)
                    {
                        fragmentParser = new HtmlParser(
                                    new ConverterBufferInput(injectTail, progressMonitor), 
                                    false,      
                                    (injectionFormat == HeaderFooterFormat.Text), 
                                    64,         
                                    8,          
                                    testBoundaryConditions);
                    }
                    else
                    {
                        fragmentParser.Initialize(
                                    injectTail, 
                                    (injectionFormat == HeaderFooterFormat.Text));
                    }

                    injectingHead = false;

                    return fragmentParser;
                }
            }

            
            return documentParser;
        }

        public IHtmlParser Pop()
        {
            InternalDebug.Assert(Active);

            if (injectingHead)
            {
                headInjected = true;

                
                if (injectTail == null)
                {
                    ((IDisposable)fragmentParser).Dispose();
                    fragmentParser = null;
                }
            }
            else
            {
                tailInjected = true;

                
                ((IDisposable)fragmentParser).Dispose();
                fragmentParser = null;
            }

            var parser = documentParser;
            documentParser = null;

            return parser;
        }

        public void Inject(bool head, HtmlWriter writer)
        {
            if (head)
            {
                if (injectHead != null && !headInjected)
                {
                    if (injectionFormat == HeaderFooterFormat.Text)
                    {
                        writer.WriteStartTag(HtmlNameIndex.TT);
                        writer.WriteStartTag(HtmlNameIndex.Pre);
                        writer.WriteNewLine();
                    }

                    CreateHtmlToHtmlConverter(injectHead, writer);

                    while (!fragmentToHtmlConverter.Flush())
                    {
                    }

                    headInjected = true;

                    
                    if (injectTail == null)
                    {
                        ((IDisposable)fragmentToHtmlConverter).Dispose();
                        fragmentToHtmlConverter = null;
                    }

                    if (injectionFormat == HeaderFooterFormat.Text)
                    {
                        writer.WriteEndTag(HtmlNameIndex.Pre);
                        writer.WriteEndTag(HtmlNameIndex.TT);
                    }
                }
            }
            else
            {
                if (injectHead != null && !headInjected)
                {
                    
                    InternalDebug.Assert(false);
                    

                    headInjected = true;
                }

                if (injectTail != null && !tailInjected)
                {
                    if (injectionFormat == HeaderFooterFormat.Text)
                    {
                        writer.WriteStartTag(HtmlNameIndex.TT);
                        writer.WriteStartTag(HtmlNameIndex.Pre);
                        writer.WriteNewLine();
                    }

                    if (fragmentToHtmlConverter == null)
                    {
                        CreateHtmlToHtmlConverter(injectTail, writer);
                    }
                    else
                    {
                        fragmentToHtmlConverter.Initialize(
                                        injectTail, 
                                        (injectionFormat == HeaderFooterFormat.Text));
                    }

                    while (!fragmentToHtmlConverter.Flush())
                    {
                    }

                    
                    ((IDisposable)fragmentToHtmlConverter).Dispose();
                    fragmentToHtmlConverter = null;

                    tailInjected = true;

                    if (injectionFormat == HeaderFooterFormat.Text)
                    {
                        writer.WriteEndTag(HtmlNameIndex.Pre);
                        writer.WriteEndTag(HtmlNameIndex.TT);
                    }
                }
            }
        }

        private void CreateHtmlToHtmlConverter(string fragment, HtmlWriter writer)
        {
            var parser = new HtmlParser(
                                new ConverterBufferInput(fragment, progressMonitor), 
                                false,      
                                (injectionFormat == HeaderFooterFormat.Text), 
                                64,         
                                8,          
                                testBoundaryConditions);

            IHtmlParser parserInterface = parser;

            
            if (injectionFormat == HeaderFooterFormat.Html)
            {
                parserInterface = new HtmlNormalizingParser(
                                    parser,
                                    null,   
                                    false,  
                                    HtmlSupport.HtmlNestingLimit,
                                    testBoundaryConditions,
                                    null,   
                                    true,   
                                    0);
            }

            fragmentToHtmlConverter = new HtmlToHtmlConverter(
                            parserInterface, 
                            writer, 
                            true,                           
                            injectionFormat == HeaderFooterFormat.Html,  
                            filterHtml,
                            callback,
                            true,                           
                            false,                          
                            traceStream,
                            true,                           
                            0,                              
                            -1,                             
                            false,                          
                            progressMonitor);
        }

        public override void Inject(bool head, TextOutput output)
        {
            HtmlParser parser;

            if (head)
            {
                if (injectHead != null && !headInjected)
                {
                    parser = new HtmlParser(
                                        new ConverterBufferInput(injectHead, progressMonitor), 
                                        false,      
                                        (injectionFormat == HeaderFooterFormat.Text), 
                                        64,         
                                        8,          
                                        testBoundaryConditions);

                    fragmentToTextConverter = new HtmlToTextConverter(
                                        parser, 
                                        output,
                                        null,   
                                        true,   
                                        injectionFormat == HeaderFooterFormat.Text,
                                        false,  
                                        traceStream,
                                        true,   
                                        0);     

                    while (!fragmentToTextConverter.Flush())
                    {
                    }

                    headInjected = true;

                    
                    if (injectTail == null)
                    {
                        ((IDisposable)fragmentToTextConverter).Dispose();
                        fragmentToTextConverter = null;
                    }
                }
            }
            else
            {
                if (injectHead != null && !headInjected)
                {
                    
                    InternalDebug.Assert(false);
                    

                    headInjected = true;
                }

                if (injectTail != null && !tailInjected)
                {
                    if (fragmentToTextConverter == null)
                    {
                        parser = new HtmlParser(
                                        new ConverterBufferInput(injectTail, progressMonitor), 
                                        false,      
                                        (injectionFormat == HeaderFooterFormat.Text), 
                                        64, 
                                        8, 
                                        testBoundaryConditions);

                        fragmentToTextConverter = new HtmlToTextConverter(
                                        parser, 
                                        output,
                                        null,   
                                        true,   
                                        injectionFormat == HeaderFooterFormat.Text,
                                        false,  
                                        traceStream,
                                        true,   
                                        0);     
                    }
                    else
                    {
                        fragmentToTextConverter.Initialize(
                                        injectTail, 
                                        (injectionFormat == HeaderFooterFormat.Text));
                    }

                    while (!fragmentToTextConverter.Flush())
                    {
                    }

                    
                    ((IDisposable)fragmentToTextConverter).Dispose();
                    fragmentToTextConverter = null;

                    tailInjected = true;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (fragmentToHtmlConverter != null)
            {
                ((IDisposable)fragmentToHtmlConverter).Dispose();
                fragmentToHtmlConverter = null;
            }

            if (fragmentToTextConverter != null)
            {
                ((IDisposable)fragmentToTextConverter).Dispose();
                fragmentToTextConverter = null;
            }
#if false
            if (this.fragmentToRtfConverter != null)
            {
                ((IDisposable)this.fragmentToRtfConverter).Dispose();
                this.fragmentToRtfConverter = null;
            }
#endif
            if (fragmentParser != null)
            {
                ((IDisposable)fragmentParser).Dispose();
                fragmentParser = null;
            }

            Reset();
            base.Dispose(disposing);
        }

        

        
        
        

        
        
        
        
        

        
        

        
        
        
        
        
        

        
        
        

        
        

        public override void InjectRtfFonts(int firstAvailableFontHandle)
        {
            
            
            
            
        }

        public override void InjectRtfColors(int nextColorIndex)
        {
            
            
            
            
        }

        
        
        
        
        
        
        

        
        
        
        
        
        
        
        
        

        
        

        
        

        
        
        

        
        
        
        
    }

    

























































































    
    
    
    
    






































































}

