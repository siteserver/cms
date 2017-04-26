// ***************************************************************
// <copyright file="HtmlFormatOutput.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters.Internal.Html
{
    using System;
    using System.IO;
    using System.Text;
    using Data.Internal;
    using Globalization;
    using Format;

    

    internal class HtmlFormatOutput : FormatOutput, IRestartable
    {
        private const int MaxRecognizedHyperlinkLength = 4096;

        

        internal HtmlWriter writer;

        private HtmlInjection injection;

        private bool filterHtml;
        private HtmlTagCallback callback;
        private HtmlFormatOutputCallbackContext callbackContext;

        private bool outputFragment;

        private bool recognizeHyperlinks;

        
#if DEBUG
        private Stream formatTraceStream;
#endif
        private int hyperlinkLevel;

        private struct EndTagActionEntry
        {
            public int tagLevel;
            public bool drop;
            public bool callback;
        }

        private EndTagActionEntry[] endTagActionStack;
        private int endTagActionStackTop;

        

        public HtmlFormatOutput(
                HtmlWriter writer,
                HtmlInjection injection,
                bool outputFragment,
                Stream formatTraceStream,
                Stream formatOutputTraceStream,
                bool filterHtml,
                HtmlTagCallback callback,
                bool recognizeHyperlinks) :
                base(formatOutputTraceStream)
        {
            this.writer = writer;
            this.injection = injection;
            this.outputFragment = outputFragment;
#if DEBUG
            this.formatTraceStream = formatTraceStream;
#endif
            this.filterHtml = filterHtml;
            this.callback = callback;

            this.recognizeHyperlinks = recognizeHyperlinks;
        }

        internal void SetWriter(HtmlWriter writer)
        {
            this.writer = writer;
        }

        

        bool IRestartable.CanRestart()
        {
            if (writer is IRestartable)
            {
                return ((IRestartable)writer).CanRestart();
            }

            return false;
        }

        

        void IRestartable.Restart()
        {
            InternalDebug.Assert(((IRestartable)this).CanRestart());

            ((IRestartable)writer).Restart();

            

            Restart();

            if (injection != null)
            {
                injection.Reset();
            }

            hyperlinkLevel = 0;
        }

        

        void IRestartable.DisableRestart()
        {
            if (writer is IRestartable)
            {
                ((IRestartable)writer).DisableRestart();
            }
        }

        

        public override bool Flush()
        {
            if (!base.Flush())
            {
                return false;
            }

            writer.Flush();
            return true;
        }

        

        public override bool OutputCodePageSameAsInput => false;


        public override Encoding OutputEncoding
        {
            set
            {
                InternalDebug.Assert(false, "this should never happen");
                throw new InvalidOperationException();
            }
        }

        

        public override bool CanAcceptMoreOutput => writer.CanAcceptMore;


        protected override bool StartRoot()
        {
            return true;
        }

        

        protected override void EndRoot()
        {
        }

        

        protected override bool StartDocument()
        {
#if DEBUG
            if (formatTraceStream != null)
            {
                FormatStore.Dump(formatTraceStream, true);
            }
#endif
            if (!outputFragment)
            {
                var endTagCallbackRequested = false;
                var dropInnerContent = false;
                var dropEndTag = false;

                writer.WriteStartTag(HtmlNameIndex.Html);

                if (callback != null)
                {
                    if (callbackContext == null)
                    {
                        callbackContext = new HtmlFormatOutputCallbackContext(this);
                    }

                    
                    callbackContext.InitializeTag(false, HtmlNameIndex.Head, false);
                }
                else
                {
                    writer.WriteStartTag(HtmlNameIndex.Head);
                }

                if (callback != null)
                {
                    callbackContext.InitializeFragment(false);

                    callback(callbackContext, writer);

                    callbackContext.UninitializeFragment();

                    if (callbackContext.IsInvokeCallbackForEndTag)
                    {
                        
                        

                        endTagCallbackRequested = true;
                    }

                    if (callbackContext.IsDeleteInnerContent)
                    {
                        dropInnerContent = true;
                    }

                    if (callbackContext.IsDeleteEndTag)
                    {
                        dropEndTag = true;
                    }
                }

                if (!dropInnerContent)
                {
                    if (writer.HasEncoding)
                    {
                        writer.WriteStartTag(HtmlNameIndex.Meta);
                        writer.WriteAttribute(HtmlNameIndex.HttpEquiv, "Content-Type");
                        writer.WriteAttributeName(HtmlNameIndex.Content);
                        writer.WriteAttributeValueInternal("text/html; charset=");
                        writer.WriteAttributeValue(Charset.GetCharset(writer.Encoding.CodePage).Name);
                        writer.WriteNewLine(true);
                    }

                    writer.WriteStartTag(HtmlNameIndex.Meta);
                    writer.WriteAttribute(HtmlNameIndex.Name, "Generator");
                    writer.WriteAttribute(HtmlNameIndex.Content, "Microsoft Exchange Server");     
                    writer.WriteNewLine(true);

                    if (Comment != null)
                    {
                        writer.WriteMarkupText("<!-- " + Comment + " -->");
                        writer.WriteNewLine(true);
                    }

                    writer.WriteStartTag(HtmlNameIndex.Style);
                    
                    
                    writer.WriteMarkupText("<!-- .EmailQuote { margin-left: 1pt; padding-left: 4pt; border-left: #800000 2px solid; } -->");
                    writer.WriteEndTag(HtmlNameIndex.Style);
                }

                if (endTagCallbackRequested)
                {
                    callbackContext.InitializeTag(true, HtmlNameIndex.Head, dropEndTag);

                    callbackContext.InitializeFragment(false);

                    callback(callbackContext, writer);

                    callbackContext.UninitializeFragment();
                }
                else if (!dropEndTag)
                {
                    writer.WriteEndTag(HtmlNameIndex.Head);
                    writer.WriteNewLine(true);
                }

                writer.WriteStartTag(HtmlNameIndex.Body);
                writer.WriteNewLine(true);
            }
            else
            {
                writer.WriteStartTag(HtmlNameIndex.Div);
                writer.WriteAttribute(HtmlNameIndex.Class, "BodyFragment");
                writer.WriteNewLine(true);
            }

            if (injection != null && injection.HaveHead)
            {
                InternalDebug.Assert(!injection.HeadDone);
                injection.Inject(true, writer);
            }

            ApplyCharFormat();
            return true;
        }

        

        protected override void EndDocument()
        {
            RevertCharFormat();

            if (injection != null && injection.HaveTail)
            {
                InternalDebug.Assert(!injection.TailDone);
                injection.Inject(false, writer);
            }

            if (!outputFragment)
            {
                writer.WriteNewLine(true);
                writer.WriteEndTag(HtmlNameIndex.Body);
                writer.WriteNewLine(true);
                writer.WriteEndTag(HtmlNameIndex.Html);
            }
            else
            {
                writer.WriteNewLine(true);
                writer.WriteEndTag(HtmlNameIndex.Div);
            }

            writer.WriteNewLine(true);
        }

        

        protected override void StartEndBaseFont()
        {
        }

        

        protected override bool StartTable()
        {
            var fontFaceValue = GetDistinctProperty(PropertyId.FontFace);
            if (!fontFaceValue.IsNull)
            {
                writer.WriteStartTag(HtmlNameIndex.Font);

                writer.WriteAttributeName(HtmlNameIndex.Face);

                string name;

                StringValue sv;
                MultiValue mv;

                if (fontFaceValue.IsMultiValue)
                {
                    mv = FormatStore.GetMultiValue(fontFaceValue);

                    for (var i = 0; i < mv.Length; i++)
                    {
                        sv = mv.GetStringValue(i);
                        name = sv.GetString();
                        if (i != 0)
                        {
                            writer.WriteAttributeValue(",");
                        }
                        writer.WriteAttributeValue(name);
                    }
                }
                else
                {
                    sv = FormatStore.GetStringValue(fontFaceValue);
                    name = sv.GetString();
                    writer.WriteAttributeValue(name);
                }
            }

            writer.WriteNewLine(true);
            writer.WriteStartTag(HtmlNameIndex.Table);

            OutputTableTagAttributes();

            var styleAttributeOpen = false;

            OutputTableCssProperties(ref styleAttributeOpen);

            OutputBlockCssProperties(ref styleAttributeOpen);

            writer.WriteNewLine(true);

            return true;
        }

        

        protected override void EndTable()
        {
            writer.WriteNewLine(true);
            writer.WriteEndTag(HtmlNameIndex.Table);
            writer.WriteNewLine(true);

            var fontFaceValue = GetDistinctProperty(PropertyId.FontFace);
            if (!fontFaceValue.IsNull)
            {
                writer.WriteEndTag(HtmlNameIndex.Font);
            }
        }

        

        protected override bool StartTableColumnGroup()
        {
            writer.WriteNewLine(true);
            writer.WriteStartTag(HtmlNameIndex.ColGroup);

            
            var width = GetDistinctProperty(PropertyId.Width);
            if (!width.IsNull && width.IsAbsRelLength)
            {
                writer.WriteAttribute(HtmlNameIndex.Width, width.PixelsInteger.ToString());
            }

            var span = GetDistinctProperty(PropertyId.NumColumns);
            if (!span.IsNull && span.IsAbsRelLength)
            {
                writer.WriteAttribute(HtmlNameIndex.Span, span.Integer.ToString());
            }

            var styleAttributeOpen = false;

            OutputTableColumnCssProperties(ref styleAttributeOpen);

            return true;
        }

        

        protected override void EndTableColumnGroup()
        {
            writer.WriteEndTag(HtmlNameIndex.ColGroup);
            writer.WriteNewLine(true);
        }

        

        protected override void StartEndTableColumn()
        {
            writer.WriteStartTag(HtmlNameIndex.Col);

            
            var width = GetDistinctProperty(PropertyId.Width);
            if (!width.IsNull && width.IsAbsRelLength)
            {
                writer.WriteAttribute(HtmlNameIndex.Width, width.PixelsInteger.ToString());
            }

            var span = GetDistinctProperty(PropertyId.NumColumns);
            if (!span.IsNull && span.IsAbsRelLength)
            {
                writer.WriteAttribute(HtmlNameIndex.Span, span.Integer.ToString());
            }

            var styleAttributeOpen = false;

            OutputTableColumnCssProperties(ref styleAttributeOpen);

            writer.WriteNewLine(true);
        }

        

        protected override bool StartTableCaption()
        {
            writer.WriteNewLine(true);

            if (!CurrentNode.Parent.IsNull && CurrentNode.Parent.NodeType == FormatContainerType.Table)
            {
                writer.WriteStartTag(HtmlNameIndex.Caption);

                var defaultStyle = FormatStore.GetStyle(HtmlConverterData.DefaultStyle.Caption);
                SubtractDefaultContainerPropertiesFromDistinct(defaultStyle.FlagProperties, defaultStyle.PropertyList);

                
                var pv = GetDistinctProperty(PropertyId.BlockAlignment);
                if (!pv.IsNull)
                {
                    var val = HtmlSupport.GetBlockAlignmentString(pv);
                    if (val != null)
                    {
                        writer.WriteAttribute(HtmlNameIndex.Align, val);
                    }
                }

                writer.WriteNewLine(true);
            }

            ApplyCharFormat();

            return true;
        }

        

        protected override void EndTableCaption()
        {
            RevertCharFormat();

            if (!CurrentNode.Parent.IsNull && CurrentNode.Parent.NodeType == FormatContainerType.Table)
            {
                writer.WriteNewLine(true);
                writer.WriteEndTag(HtmlNameIndex.Caption);
            }

            writer.WriteNewLine(true);
        }

        protected override bool StartTableExtraContent()
        {
            return StartBlockContainer();
        }

        protected override void EndTableExtraContent()
        {
            EndBlockContainer();
        }

        

        protected override bool StartTableRow()
        {
            writer.WriteNewLine(true);
            writer.WriteStartTag(HtmlNameIndex.TR);

            
            var height = GetDistinctProperty(PropertyId.Height);
            if (!height.IsNull && height.IsAbsRelLength)
            {
                writer.WriteAttribute(HtmlNameIndex.Height, height.PixelsInteger.ToString());
            }

            var styleAttributeOpen = false;

            OutputBlockCssProperties(ref styleAttributeOpen);

            writer.WriteNewLine(true);

            return true;
        }

        

        protected override void EndTableRow()
        {
            writer.WriteNewLine(true);
            writer.WriteEndTag(HtmlNameIndex.TR);
            writer.WriteNewLine(true);
        }

        

        protected override bool StartTableCell()
        {
            var mergeCell = GetDistinctProperty(PropertyId.MergedCell);

            
            if (mergeCell.IsNull || !mergeCell.Bool)
            {
                writer.WriteNewLine(true);
                writer.WriteStartTag(HtmlNameIndex.TD);

                OutputTableCellTagAttributes();

                var styleAttributeOpen = false;

                OutputBlockCssProperties(ref styleAttributeOpen);

                ApplyCharFormat();
            }

            return true;
        }

        

        protected override void EndTableCell()
        {
            var mergeCell = GetDistinctProperty(PropertyId.MergedCell);

            if (mergeCell.IsNull || !mergeCell.Bool)
            {
                RevertCharFormat();

                writer.WriteEndTag(HtmlNameIndex.TD);
                writer.WriteNewLine(true);
            }
        }

        

        private static string[] listType =
        {
            null,   
            null,   
            "1",
            "a",
            "A",
            "i",
            "I",
        };

        protected override bool StartList()
        {
            writer.WriteNewLine(true);

            var listStyle = GetEffectiveProperty(PropertyId.ListStyle);

            var bulletedList = true;

            if (listStyle.IsNull || (ListStyle)listStyle.Enum == ListStyle.Bullet)
            {
                writer.WriteStartTag(HtmlNameIndex.UL);
            }
            else
            {
                writer.WriteStartTag(HtmlNameIndex.OL);
                bulletedList = false;
            }

            
            var rtl = GetDistinctProperty(PropertyId.RightToLeft);
            if (!rtl.IsNull)
            {
                writer.WriteAttribute(HtmlNameIndex.Dir, rtl.Bool ? "rtl" : "ltr");
            }

            
            if (!bulletedList && (ListStyle)listStyle.Enum != ListStyle.Decimal)
            {
                writer.WriteAttribute(HtmlNameIndex.Type, listType[listStyle.Enum]);
            }

            
            var listStart = GetDistinctProperty(PropertyId.ListStart);
            if (!bulletedList && listStart.IsInteger && listStart.Integer != 1)
            {
                writer.WriteAttribute(HtmlNameIndex.Start, listStart.Integer.ToString());
            }

            var styleAttributeOpen = false;

            OutputBlockCssProperties(ref styleAttributeOpen);

            writer.WriteNewLine(true);

            ApplyCharFormat();

            return true;
        }

        

        protected override void EndList()
        {
            RevertCharFormat();

            var listStyle = GetEffectiveProperty(PropertyId.ListStyle);

            writer.WriteNewLine(true);

            if (listStyle.IsNull || (ListStyle)listStyle.Enum == ListStyle.Bullet)
            {
                writer.WriteEndTag(HtmlNameIndex.UL);
            }
            else
            {
                writer.WriteEndTag(HtmlNameIndex.OL);
            }

            writer.WriteNewLine(true);
        }

        

        protected override bool StartListItem()
        {
            writer.WriteNewLine(true);
            writer.WriteStartTag(HtmlNameIndex.LI);

            var styleAttributeOpen = false;

            OutputBlockCssProperties(ref styleAttributeOpen);

            ApplyCharFormat();

            return true;
        }

        

        protected override void EndListItem()
        {
            RevertCharFormat();

            writer.WriteEndTag(HtmlNameIndex.LI);
            writer.WriteNewLine(true);
        }

        

        private static Property[] DefaultHyperlinkProperties = new Property[]
        {
            new Property(PropertyId.FontColor, new PropertyValue(new RGBT(0,0,255))),
        };

        protected override bool StartHyperLink()
        {
            var dropEndTag = false;
            var dropInnerContent = false;
            var endTagCallbackRequested = false;

            var defaultFlags = new FlagProperties();
            defaultFlags.Set(PropertyId.Underline, true);
            SubtractDefaultContainerPropertiesFromDistinct(defaultFlags, DefaultHyperlinkProperties);

            if (callback != null)
            {
                if (callbackContext == null)
                {
                    callbackContext = new HtmlFormatOutputCallbackContext(this);
                }

                
                callbackContext.InitializeTag(false, HtmlNameIndex.A, false);
            }
            else
            {
                writer.WriteStartTag(HtmlNameIndex.A);
            }

            var pv = GetDistinctProperty(PropertyId.HyperlinkUrl);
            if (!pv.IsNull)
            {
                var sv = FormatStore.GetStringValue(pv);
                var url = sv.GetString();

                if (filterHtml && !HtmlToHtmlConverter.IsUrlSafe(url, callback != null))
                {
                    url = string.Empty;
                }

                if (callback != null)
                {
                    callbackContext.AddAttribute(HtmlNameIndex.Href, url);
                }
                else
                {
                    writer.WriteAttributeName(HtmlNameIndex.Href);
                    writer.WriteAttributeValue(url);
                }

                pv = GetDistinctProperty(PropertyId.HyperlinkTarget);
                if (!pv.IsNull)
                {
                    var target = HtmlSupport.GetTargetString(pv);

                    if (callback != null)
                    {
                        callbackContext.AddAttribute(HtmlNameIndex.Target, target);
                    }
                    else
                    {
                        writer.WriteAttributeName(HtmlNameIndex.Target);
                        writer.WriteAttributeValue(target);
                    }
                }
            }

            

            if (callback != null)
            {
                callbackContext.InitializeFragment(false);

                callback(callbackContext, writer);

                callbackContext.UninitializeFragment();

                if (callbackContext.IsInvokeCallbackForEndTag)
                {
                    
                    

                    endTagCallbackRequested = true;
                }

                if (callbackContext.IsDeleteInnerContent)
                {
                    dropInnerContent = true;
                }

                if (callbackContext.IsDeleteEndTag)
                {
                    dropEndTag = true;
                }

                if (dropEndTag || endTagCallbackRequested)
                {
                    if (endTagActionStack == null)
                    {
                        endTagActionStack = new EndTagActionEntry[4];
                    }
                    else if (endTagActionStack.Length == endTagActionStackTop)
                    {
                        var newEndTagActionStack = new EndTagActionEntry[endTagActionStack.Length * 2];
                        Array.Copy(endTagActionStack, 0, newEndTagActionStack, 0, endTagActionStackTop);
                        endTagActionStack = newEndTagActionStack;
                    }

                    endTagActionStack[endTagActionStackTop].tagLevel = hyperlinkLevel;
                    endTagActionStack[endTagActionStackTop].drop = dropEndTag;
                    endTagActionStack[endTagActionStackTop].callback = endTagCallbackRequested;

                    endTagActionStackTop++;
                }
            }

            hyperlinkLevel ++;

            if (!dropInnerContent)
            {
                ApplyCharFormat();
            }
            else
            {
                
                CloseHyperLink();
            }

            if (writer.IsTagOpen)
            {
                writer.WriteTagEnd();
            }

            return !dropInnerContent;
        }

        

        protected override void EndHyperLink()
        {
            hyperlinkLevel --;

            RevertCharFormat();

            CloseHyperLink();

            if (writer.IsTagOpen)
            {
                writer.WriteTagEnd();
            }
        }

        

        private void CloseHyperLink()
        {
            var dropTag = false;
            var endTagCallbackRequested = false;

            if (endTagActionStackTop != 0)
            {
                InternalDebug.Assert(callback != null && callbackContext != null);
                InternalDebug.Assert(endTagActionStack[endTagActionStackTop - 1].tagLevel <= hyperlinkLevel);

                
                
                

                if (endTagActionStack[endTagActionStackTop - 1].tagLevel == hyperlinkLevel)
                {
                    endTagActionStackTop --;

                    dropTag = endTagActionStack[endTagActionStackTop].drop;
                    endTagCallbackRequested = endTagActionStack[endTagActionStackTop].callback;
                }
            }

            if (endTagCallbackRequested)
            {
                InternalDebug.Assert(callback != null && callbackContext != null);

                callbackContext.InitializeTag(true, HtmlNameIndex.A, dropTag);
                callbackContext.InitializeFragment(false);

                callback(callbackContext, writer);

                callbackContext.UninitializeFragment();
            }
            else if (!dropTag)
            {
                writer.WriteEndTag(HtmlNameIndex.A);
            }
        }

        

        protected override bool StartBookmark()
        {
            var pv = GetDistinctProperty(PropertyId.BookmarkName);
            if (!pv.IsNull)
            {
                

                writer.WriteStartTag(HtmlNameIndex.A);

                var sv = FormatStore.GetStringValue(pv);
                var name = sv.GetString();

                writer.WriteAttributeName(HtmlNameIndex.Name);
                writer.WriteAttributeValue(name);
            }

            ApplyCharFormat();

            if (writer.IsTagOpen)
            {
                writer.WriteTagEnd();
            }

            return true;
        }

        

        protected override void EndBookmark()
        {
            RevertCharFormat();

            var pv = GetDistinctProperty(PropertyId.BookmarkName);
            if (!pv.IsNull)
            {
                writer.WriteEndTag(HtmlNameIndex.A);
            }

            if (writer.IsTagOpen)
            {
                writer.WriteTagEnd();
            }
        }

        

        protected override void StartEndImage()
        {
            if (callback != null)
            {
                if (callbackContext == null)
                {
                    callbackContext = new HtmlFormatOutputCallbackContext(this);
                }

                
                callbackContext.InitializeTag(false, HtmlNameIndex.Img, false);
            }
            else
            {
                writer.WriteStartTag(HtmlNameIndex.Img);
            }

            var pv = GetDistinctProperty(PropertyId.Width);
            if (!pv.IsNull)
            {
                var val = HtmlSupport.FormatPixelOrPercentageLength(ref scratchBuffer, pv);
                if (val.Length != 0)
                {
                    if (callback != null)
                    {
                        callbackContext.AddAttribute(HtmlNameIndex.Width, val.ToString());
                    }
                    else
                    {
                        writer.WriteAttribute(HtmlNameIndex.Width, val);
                    }
                }
            }

            pv = GetDistinctProperty(PropertyId.Height);
            if (!pv.IsNull)
            {
                var val = HtmlSupport.FormatPixelOrPercentageLength(ref scratchBuffer, pv);
                if (val.Length != 0)
                {
                    if (callback != null)
                    {
                        callbackContext.AddAttribute(HtmlNameIndex.Height, val.ToString());
                    }
                    else
                    {
                        writer.WriteAttribute(HtmlNameIndex.Height, val);
                    }
                }
            }

            pv = GetDistinctProperty(PropertyId.BlockAlignment);
            if (!pv.IsNull)
            {
                var val = HtmlSupport.GetBlockAlignmentString(pv);
                if (val != null)
                {
                    if (callback != null)
                    {
                        callbackContext.AddAttribute(HtmlNameIndex.Align, val);
                    }
                    else
                    {
                        writer.WriteAttribute(HtmlNameIndex.Align, val);
                    }
                }
            }

            pv = GetDistinctProperty(PropertyId.ImageBorder);
            if (!pv.IsNull)
            {
                var val = HtmlSupport.FormatPixelOrPercentageLength(ref scratchBuffer, pv);
                if (val.Length != 0)
                {
                    if (callback != null)
                    {
                        callbackContext.AddAttribute(HtmlNameIndex.Border, val.ToString());
                    }
                    else
                    {
                        writer.WriteAttribute(HtmlNameIndex.Border, val);
                    }
                }
            }

            pv = GetDistinctProperty(PropertyId.ImageUrl);
            if (!pv.IsNull)
            {
                

                var sv = FormatStore.GetStringValue(pv);
                var url = sv.GetString();

                if (filterHtml && !HtmlToHtmlConverter.IsUrlSafe(url, callback != null))
                {
                    url = string.Empty;
                }

                if (callback != null)
                {
                    callbackContext.AddAttribute(HtmlNameIndex.Src, url);
                }
                else
                {
                    writer.WriteAttributeName(HtmlNameIndex.Src);
                    writer.WriteAttributeValue(url);
                }
            }

            pv = GetDistinctProperty(PropertyId.RightToLeft);
            if (pv.IsBool)
            {
                if (callback != null)
                {
                    callbackContext.AddAttribute(HtmlNameIndex.Dir, pv.Bool ? "rtl" : "ltr");
                }
                else
                {
                    writer.WriteAttributeName(HtmlNameIndex.Dir);
                    writer.WriteAttributeValue(pv.Bool ? "rtl" : "ltr");
                }
            }

            pv = GetDistinctProperty(PropertyId.Language);
            if (pv.IsInteger)
            {
                Culture culture;

                if (Culture.TryGetCulture(pv.Integer, out culture) || String.IsNullOrEmpty(culture.Name))
                {
                    if (callback != null)
                    {
                        callbackContext.AddAttribute(HtmlNameIndex.Lang, culture.Name);
                    }
                    else
                    {
                        writer.WriteAttributeName(HtmlNameIndex.Lang);
                        writer.WriteAttributeValue(culture.Name);
                    }
                }
            }

            pv = GetDistinctProperty(PropertyId.ImageAltText);
            if (!pv.IsNull)
            {
                

                var sv = FormatStore.GetStringValue(pv);
                var altText = sv.GetString();

                if (callback != null)
                {
                    callbackContext.AddAttribute(HtmlNameIndex.Alt, altText);
                }
                else
                {
                    writer.WriteAttributeName(HtmlNameIndex.Alt);
                    writer.WriteAttributeValue(altText);
                }
            }

            

            if (callback != null)
            {
                callbackContext.InitializeFragment(true);

                callback(callbackContext, writer);

                callbackContext.UninitializeFragment();

                
            }

            if (writer.IsTagOpen)
            {
                writer.WriteTagEnd();
            }
        }

        

        protected override void StartEndHorizontalLine()
        {
            writer.WriteNewLine(true);

            writer.WriteStartTag(HtmlNameIndex.HR);

            
            var width = GetDistinctProperty(PropertyId.Width);
            if (!width.IsNull)
            {
                var val = HtmlSupport.FormatPixelOrPercentageLength(ref scratchBuffer, width);
                if (val.Length != 0)
                {
                    writer.WriteAttribute(HtmlNameIndex.Width, val);
                }
            }

            
            var size = GetDistinctProperty(PropertyId.Height);
            if (!size.IsNull && size.IsAbsRelLength)
            {
                writer.WriteAttribute(HtmlNameIndex.Size, size.PixelsInteger.ToString());
            }

            
            var align = GetDistinctProperty(PropertyId.HorizontalAlignment);
            if (!align.IsNull)
            {
                var val = HtmlSupport.GetHorizontalAlignmentString(align);
                if (val != null)
                {
                    writer.WriteAttribute(HtmlNameIndex.Align, val);
                }
            }

            
            var color = GetDistinctProperty(PropertyId.FontColor);
            if (!color.IsNull)
            {
                var val = HtmlSupport.FormatColor(ref scratchBuffer, color);
                if (val.Length != 0)
                {
                    writer.WriteAttribute(HtmlNameIndex.Color, val);
                }
            }

            
            if (!width.IsNull)
            {
                writer.WriteAttributeName(HtmlNameIndex.Style);

                if (!width.IsNull)
                {
                    var val = HtmlSupport.FormatLength(ref scratchBuffer, width);
                    if (val.Length != 0)
                    {
                        writer.WriteAttributeValue("width:");
                        writer.WriteAttributeValue(val);
                        writer.WriteAttributeValue(";");
                    }
                }
            }

            if (writer.LiteralWhitespaceNesting == 0)
            {
                writer.WriteNewLine(true);
            }
        }

        

        protected override bool StartInline()
        {
            ApplyCharFormat();
            return true;
        }

        

        protected override void EndInline()
        {
            RevertCharFormat();
        }

        

        protected override bool StartMap()
        {
            return StartBlockContainer();
        }

        

        protected override void EndMap()
        {
            EndBlockContainer();
        }

        

        protected override void StartEndArea()
        {
        }

        

        protected override bool StartForm()
        {
            return StartInlineContainer();
        }

        

        protected override void EndForm()
        {
            EndInlineContainer();
        }

        

        protected override bool StartFieldSet()
        {
            return StartBlockContainer();
        }

        

        protected override void EndFieldSet()
        {
            EndBlockContainer();
        }

        

        protected override bool StartSelect()
        {
            return true;
        }

        

        protected override void EndSelect()
        {
        }

        

        protected override bool StartOptionGroup()
        {
            return true;
        }

        

        protected override void EndOptionGroup()
        {
        }

        

        protected override bool StartOption()
        {
            return true;
        }

        

        protected override void EndOption()
        {
        }

        

        protected override bool StartText()
        {
            ApplyCharFormat();
            writer.StartTextChunk();
            return true;
        }

        

        protected override bool ContinueText(uint beginTextPosition, uint endTextPosition)
        {
            InternalDebug.Assert(CurrentNode.IsText);
            InternalDebug.Assert(CurrentNode.BeginTextPosition <= beginTextPosition &&
                                beginTextPosition <= endTextPosition &&
                                endTextPosition <= CurrentNode.EndTextPosition);

            if (beginTextPosition != endTextPosition)
            {
                var run = FormatStore.GetTextRun(beginTextPosition);

                do
                {
                    var effectiveLength = run.EffectiveLength;
                    InternalDebug.Assert(effectiveLength > 0 || run.Type == TextRunType.Invalid);

                    switch (run.Type)
                    {
                        case TextRunType.NewLine:

                            while (0 != effectiveLength--)
                            {
                                if (writer.LiteralWhitespaceNesting == 0)
                                {
                                    writer.WriteStartTag(HtmlNameIndex.BR);
                                }
                                writer.WriteNewLine(false);
                            }
                            break;

                        case TextRunType.Tabulation:

                            writer.WriteTabulation(effectiveLength);
                            break;

                        case TextRunType.Space:

                            writer.WriteSpace(effectiveLength);
                            break;

                        case TextRunType.NbSp:

                            writer.WriteNbsp(effectiveLength);
                            break;

                        case TextRunType.NonSpace:

                            var start = 0;

                            if (recognizeHyperlinks && hyperlinkLevel == 0 && effectiveLength > 10 && effectiveLength < MaxRecognizedHyperlinkLength)
                            {
                                

                                bool addHttpPrefix;
                                bool addFilePrefix;
                                int offset;             
                                int length;             

                                var link = RecognizeHyperLink(run, out offset, out length, out addFilePrefix, out addHttpPrefix);

                                if (link)
                                {
                                    if (offset != 0)
                                    {
                                        
                                        writer.WriteTextInternal(scratchBuffer.Buffer, 0, offset);
                                    }

                                    if (callback != null)
                                    {
                                        if (callbackContext == null)
                                        {
                                            callbackContext = new HtmlFormatOutputCallbackContext(this);
                                        }

                                        
                                        callbackContext.InitializeTag(false, HtmlNameIndex.A, false);

                                        
                                        var href = new string(scratchBuffer.Buffer, offset, length);
                                        if (addHttpPrefix)
                                        {
                                            href = "http://" + href;
                                        }
                                        else if (addFilePrefix)
                                        {
                                            href = "file://" + href;
                                        }

                                        callbackContext.AddAttribute(HtmlNameIndex.Href, href);

                                        callbackContext.InitializeFragment(false);

                                        callback(callbackContext, writer);

                                        callbackContext.UninitializeFragment();

                                        if (writer.IsTagOpen)
                                        {
                                            writer.WriteTagEnd();
                                        }

                                        if (!callbackContext.IsDeleteInnerContent)
                                        {
                                            writer.WriteTextInternal(scratchBuffer.Buffer, offset, length);
                                        }

                                        if (callbackContext.IsInvokeCallbackForEndTag)
                                        {
                                            
                                            

                                            
                                            callbackContext.InitializeTag(true, HtmlNameIndex.A, callbackContext.IsDeleteEndTag);

                                            callbackContext.InitializeFragment(false);

                                            callback(callbackContext, writer);

                                            callbackContext.UninitializeFragment();
                                        }
                                        else if (!callbackContext.IsDeleteEndTag)
                                        {
                                            writer.WriteEndTag(HtmlNameIndex.A);
                                        }

                                        if (writer.IsTagOpen)
                                        {
                                            writer.WriteTagEnd();
                                        }
                                    }
                                    else
                                    {
                                        writer.WriteStartTag(HtmlNameIndex.A);

                                        
                                        writer.WriteAttributeName(HtmlNameIndex.Href);
                                        if (addHttpPrefix)
                                        {
                                            writer.WriteAttributeValue("http://");
                                        }
                                        else if (addFilePrefix)
                                        {
                                            writer.WriteAttributeValue("file://");
                                        }

                                        writer.WriteAttributeValue(scratchBuffer.Buffer, offset, length);
                                        writer.WriteTagEnd();

                                        writer.WriteTextInternal(scratchBuffer.Buffer, offset, length);

                                        writer.WriteEndTag(HtmlNameIndex.A);
                                    }

                                    start += offset + length;

                                    if (start == effectiveLength)
                                    {
                                        
                                        run.MoveNext();
                                        continue;
                                    }

                                    
                                }

                                
                            }

                            

                            do
                            {
                                char[] buffer;
                                int offset;
                                int count;

                                run.GetChunk(start, out buffer, out offset, out count);
                                writer.WriteTextInternal(buffer, offset, count);
                                start += count;
                            }
                            while (start != effectiveLength);

                            break;
                    }

                    run.MoveNext();
                }
                while (run.Position < endTextPosition);
            }

            return true;
        }

        

        protected override void EndText()
        {
            writer.EndTextChunk();

            RevertCharFormat();
        }

        

        protected override bool StartBlockContainer()
        {
            writer.WriteNewLine(true);

            var preformatted = GetDistinctProperty(PropertyId.Preformatted);
            var quotingLevel = GetDistinctProperty(PropertyId.QuotingLevelDelta);

            if (!preformatted.IsNull && preformatted.Bool)
            {
                var defaultStyle = FormatStore.GetStyle(HtmlConverterData.DefaultStyle.Pre);
                
                SubtractDefaultContainerPropertiesFromDistinct(FlagProperties.AllOff, defaultStyle.PropertyList);

                writer.WriteStartTag(HtmlNameIndex.Pre);
            }
            else if (!quotingLevel.IsNull && quotingLevel.Integer != 0)
            {
                for (var i = 0; i < quotingLevel.Integer; i++)
                {
                    writer.WriteStartTag(HtmlNameIndex.Div);

                    
                    writer.WriteAttribute(HtmlNameIndex.Class, "EmailQuote");
                }
            }
            else
            {
                
                
                if (SourceFormat == SourceFormat.Text)
                {
                    ApplyCharFormat();
                }

                writer.WriteStartTag(HtmlNameIndex.Div);

                if (SourceFormat == SourceFormat.Text)
                {
                    
                    writer.WriteAttribute(HtmlNameIndex.Class, "PlainText");
                }
            }

            OutputBlockTagAttributes();

            var styleAttributeOpen = false;

            OutputBlockCssProperties(ref styleAttributeOpen);

            if (SourceFormat != SourceFormat.Text)
            {
                ApplyCharFormat();
            }

            if (CurrentNode.FirstChild.IsNull)
            {
                
                writer.WriteText('\xA0');
            }
            else if (CurrentNode.FirstChild == CurrentNode.LastChild && CurrentNode.FirstChild.NodeType == FormatContainerType.Text)
            {
                var child = CurrentNode.FirstChild;
                if (child.BeginTextPosition + 1 == child.EndTextPosition)
                {
                    var run = FormatStore.GetTextRun(child.BeginTextPosition);

                    InternalDebug.Assert(run.Length == 1);

                    if (run.Type == TextRunType.Space)
                    {
                        
                        writer.WriteText('\xA0');
                        EndBlockContainer();
                        return false;
                    }
                }
            }

            return true;
        }

        

        protected override void EndBlockContainer()
        {
            var preformatted = GetDistinctProperty(PropertyId.Preformatted);
            var quotingLevel = GetDistinctProperty(PropertyId.QuotingLevelDelta);

            if (SourceFormat != SourceFormat.Text)
            {
                RevertCharFormat();
            }

            if (!preformatted.IsNull && preformatted.Bool)
            {
                writer.WriteEndTag(HtmlNameIndex.Pre);
            }
            else if (!quotingLevel.IsNull && quotingLevel.Integer != 0)
            {
                for (var i = 0; i < quotingLevel.Integer; i++)
                {
                    writer.WriteEndTag(HtmlNameIndex.Div);
                }
            }
            else
            {
                writer.WriteEndTag(HtmlNameIndex.Div);

                if (SourceFormat == SourceFormat.Text)
                {
                    RevertCharFormat();
                }
            }

            writer.WriteNewLine(true);
        }

        

        protected override bool StartInlineContainer()
        {
            return true;
        }

        

        protected override void EndInlineContainer()
        {
        }

        

        private void ApplyCharFormat()
        {
            BufferString val;

            scratchBuffer.Reset();

            var flags = GetDistinctFlags();

            var fontSizeValue = GetDistinctProperty(PropertyId.FontSize);
            if (!fontSizeValue.IsNull && !fontSizeValue.IsHtmlFontUnits && !fontSizeValue.IsRelativeHtmlFontUnits)
            {
                scratchBuffer.Append("font-size:");
                HtmlSupport.AppendCssFontSize(ref scratchBuffer, fontSizeValue);
                scratchBuffer.Append(';');
            }

            var backColorValue = GetDistinctProperty(PropertyId.BackColor);
            if (backColorValue.IsColor)
            {
                scratchBuffer.Append("background-color:");
                HtmlSupport.AppendColor(ref scratchBuffer, backColorValue);
                scratchBuffer.Append(';');
            }

            Culture culture = null;

            var languageValue = GetDistinctProperty(PropertyId.Language);
            if (languageValue.IsInteger)
            {
                if (!Culture.TryGetCulture(languageValue.Integer, out culture) || String.IsNullOrEmpty(culture.Name))
                {
                    culture = null;
                }
            }

            if (0 == (CurrentNode.NodeType & FormatContainerType.BlockFlag))
            {
                var displayValue = GetDistinctProperty(PropertyId.Display);
                var unicodeBiDiValue = GetDistinctProperty(PropertyId.UnicodeBiDi);

                if (!displayValue.IsNull)
                {
                    var str = HtmlSupport.GetDisplayString(displayValue);
                    if (str != null)
                    {
                        scratchBuffer.Append("display:");
                        scratchBuffer.Append(str);
                        scratchBuffer.Append(";");
                    }
                }

                if (flags.IsDefined(PropertyId.Visible))
                {
                    scratchBuffer.Append(flags.IsOn(PropertyId.Visible) ? "visibility:visible;" : "visibility:hidden;");
                }

                if (!unicodeBiDiValue.IsNull)
                {
                    var str = HtmlSupport.GetUnicodeBiDiString(unicodeBiDiValue);
                    if (str != null)
                    {
                        scratchBuffer.Append("unicode-bidi:");
                        scratchBuffer.Append(str);
                        scratchBuffer.Append(";");
                    }
                }
            }

            if (flags.IsDefinedAndOff(PropertyId.Bold))
            {
                
                scratchBuffer.Append("font-weight:normal;");
            }

            if (flags.IsDefined(PropertyId.SmallCaps))
            {
                scratchBuffer.Append(flags.IsOn(PropertyId.SmallCaps) ? "font-variant:small-caps;" : "font-variant:normal;");
            }

            if (flags.IsDefined(PropertyId.Capitalize))
            {
                scratchBuffer.Append(flags.IsOn(PropertyId.Capitalize) ? "text-transform:uppercase;" : "text-transform:none;");
            }

            
            

            var fontFaceValue = GetDistinctProperty(PropertyId.FontFace);
            var fontColorValue = GetDistinctProperty(PropertyId.FontColor);

            if (!fontFaceValue.IsNull || !fontSizeValue.IsNull || !fontColorValue.IsNull)
            {
                writer.WriteStartTag(HtmlNameIndex.Font);

                if (!fontFaceValue.IsNull)
                {
                    writer.WriteAttributeName(HtmlNameIndex.Face);

                    string name;

                    StringValue sv;
                    MultiValue mv;

                    if (fontFaceValue.IsMultiValue)
                    {
                        mv = FormatStore.GetMultiValue(fontFaceValue);

                        for (var i = 0; i < mv.Length; i++)
                        {
                            sv = mv.GetStringValue(i);
                            name = sv.GetString();
                            if (i != 0)
                            {
                                writer.WriteAttributeValue(",");
                            }
                            writer.WriteAttributeValue(name);
                        }
                    }
                    else
                    {
                        sv = FormatStore.GetStringValue(fontFaceValue);
                        name = sv.GetString();
                        writer.WriteAttributeValue(name);
                    }
                }

                if (!fontSizeValue.IsNull)
                {
                    val = HtmlSupport.FormatFontSize(ref scratchValueBuffer, fontSizeValue);
                    if (val.Length != 0)
                    {
                        writer.WriteAttribute(HtmlNameIndex.Size, val);
                    }
                }

                if (!fontColorValue.IsNull)
                {
                    val = HtmlSupport.FormatColor(ref scratchValueBuffer, fontColorValue);
                    if (val.Length != 0)
                    {
                        writer.WriteAttribute(HtmlNameIndex.Color, val);
                    }
                }
            }

            if (scratchBuffer.Length != 0 || flags.IsDefined(PropertyId.RightToLeft) || culture != null)
            {
                writer.WriteStartTag(HtmlNameIndex.Span);

                if (scratchBuffer.Length != 0)
                {
                    writer.WriteAttributeName(HtmlNameIndex.Style);
                    writer.WriteAttributeValue(scratchBuffer.BufferString);
                }

                if (flags.IsDefined(PropertyId.RightToLeft))
                {
                    writer.WriteAttributeName(HtmlNameIndex.Dir);
                    writer.WriteAttributeValue(flags.IsOn(PropertyId.RightToLeft) ? "rtl" : "ltr");
                }

                if (culture != null)
                {
                    writer.WriteAttributeName(HtmlNameIndex.Lang);
                    writer.WriteAttributeValue(culture.Name);
                }
            }

            if (flags.IsDefinedAndOn(PropertyId.Bold))
            {
                writer.WriteStartTag(HtmlNameIndex.B);
            }

            if (flags.IsDefinedAndOn(PropertyId.Italic))
            {
                writer.WriteStartTag(HtmlNameIndex.I);
            }

            if (flags.IsDefinedAndOn(PropertyId.Underline))
            {
                writer.WriteStartTag(HtmlNameIndex.U);
            }

            if (flags.IsDefinedAndOn(PropertyId.Subscript))
            {
                writer.WriteStartTag(HtmlNameIndex.Sub);
            }

            if (flags.IsDefinedAndOn(PropertyId.Superscript))
            {
                writer.WriteStartTag(HtmlNameIndex.Sup);
            }

            if (flags.IsDefinedAndOn(PropertyId.Strikethrough))
            {
                writer.WriteStartTag(HtmlNameIndex.Strike);
            }
        }

        

        private void RevertCharFormat()
        {
            var flags = GetDistinctFlags();

            var closeFontTag = false;
            var closeSpanTag = false;

            var fontSizeValue = GetDistinctProperty(PropertyId.FontSize);
            if (!fontSizeValue.IsNull && !fontSizeValue.IsHtmlFontUnits && !fontSizeValue.IsRelativeHtmlFontUnits)
            {
                closeSpanTag = true;
            }

            var backColorValue = GetDistinctProperty(PropertyId.BackColor);
            if (backColorValue.IsColor)
            {
                closeSpanTag = true;
            }

            Culture culture = null;

            var languageValue = GetDistinctProperty(PropertyId.Language);
            if (languageValue.IsInteger)
            {
                if (Culture.TryGetCulture(languageValue.Integer, out culture) && !String.IsNullOrEmpty(culture.Name))
                {
                    closeSpanTag = true;
                }
            }

            if (0 == (CurrentNode.NodeType & FormatContainerType.BlockFlag))
            {
                var displayValue = GetDistinctProperty(PropertyId.Display);
                var unicodeBiDiValue = GetDistinctProperty(PropertyId.UnicodeBiDi);

                if (!displayValue.IsNull)
                {
                    var str = HtmlSupport.GetDisplayString(displayValue);
                    if (str != null)
                    {
                        closeSpanTag = true;
                    }
                }

                if (flags.IsDefined(PropertyId.Visible))
                {
                    closeSpanTag = true;
                }

                if (!unicodeBiDiValue.IsNull)
                {
                    var str = HtmlSupport.GetUnicodeBiDiString(unicodeBiDiValue);
                    if (str != null)
                    {
                        closeSpanTag = true;
                    }
                }
            }

            if (flags.IsDefinedAndOff(PropertyId.Bold))
            {
                
                closeSpanTag = true;
            }

            if (flags.IsDefined(PropertyId.SmallCaps))
            {
                closeSpanTag = true;
            }

            if (flags.IsDefined(PropertyId.Capitalize))
            {
                closeSpanTag = true;
            }

            if (flags.IsDefined(PropertyId.RightToLeft))
            {
                closeSpanTag = true;
            }

            var fontFaceValue = GetDistinctProperty(PropertyId.FontFace);
            var fontColorValue = GetDistinctProperty(PropertyId.FontColor);

            if (!fontFaceValue.IsNull || !fontSizeValue.IsNull || !fontColorValue.IsNull)
            {
                closeFontTag = true;
            }

            
            

            if (flags.IsDefinedAndOn(PropertyId.Strikethrough))
            {
                writer.WriteEndTag(HtmlNameIndex.Strike);
            }

            if (flags.IsDefinedAndOn(PropertyId.Superscript))
            {
                writer.WriteEndTag(HtmlNameIndex.Sup);
            }

            if (flags.IsDefinedAndOn(PropertyId.Subscript))
            {
                writer.WriteEndTag(HtmlNameIndex.Sub);
            }

            if (flags.IsDefinedAndOn(PropertyId.Underline))
            {
                writer.WriteEndTag(HtmlNameIndex.U);
            }

            if (flags.IsDefinedAndOn(PropertyId.Italic))
            {
                writer.WriteEndTag(HtmlNameIndex.I);
            }

            if (flags.IsDefinedAndOn(PropertyId.Bold))
            {
                writer.WriteEndTag(HtmlNameIndex.B);
            }

            if (closeSpanTag)
            {
                writer.WriteEndTag(HtmlNameIndex.Span);
            }

            if (closeFontTag)
            {
                writer.WriteEndTag(HtmlNameIndex.Font);
            }
        }

        

        private void OutputBlockCssProperties(ref bool styleAttributeOpen)
        {
            var display = GetDistinctProperty(PropertyId.Display);
            var visible = GetDistinctProperty(PropertyId.Visible);

            var height = GetDistinctProperty(PropertyId.Height);
            var width = GetDistinctProperty(PropertyId.Width);

            var unicodeBiDiValue = GetDistinctProperty(PropertyId.UnicodeBiDi);

            var firstLineIndent = GetDistinctProperty(PropertyId.FirstLineIndent);
            var textAlignment = GetDistinctProperty(PropertyId.TextAlignment);
            var backColor = GetDistinctProperty(PropertyId.BackColor);
 
            var topMargin = GetDistinctProperty(PropertyId.TopMargin);
            var rightMargin = GetDistinctProperty(PropertyId.RightMargin);
            var bottomMargin = GetDistinctProperty(PropertyId.BottomMargin);
            var leftMargin = GetDistinctProperty(PropertyId.LeftMargin);
            var topPadding = GetDistinctProperty(PropertyId.TopPadding);
            var rightPadding = GetDistinctProperty(PropertyId.RightPadding);
            var bottomPadding = GetDistinctProperty(PropertyId.BottomPadding);
            var leftPadding = GetDistinctProperty(PropertyId.LeftPadding);

            var topBorderWidth = GetDistinctProperty(PropertyId.TopBorderWidth);
            var rightBorderWidth = GetDistinctProperty(PropertyId.RightBorderWidth);
            var bottomBorderWidth = GetDistinctProperty(PropertyId.BottomBorderWidth);
            var leftBorderWidth = GetDistinctProperty(PropertyId.LeftBorderWidth);
            var topBorderStyle = GetDistinctProperty(PropertyId.TopBorderStyle);
            var rightBorderStyle = GetDistinctProperty(PropertyId.RightBorderStyle);
            var bottomBorderStyle = GetDistinctProperty(PropertyId.BottomBorderStyle);
            var leftBorderStyle = GetDistinctProperty(PropertyId.LeftBorderStyle);
            var topBorderColor = GetDistinctProperty(PropertyId.TopBorderColor);
            var rightBorderColor = GetDistinctProperty(PropertyId.RightBorderColor);
            var bottomBorderColor = GetDistinctProperty(PropertyId.BottomBorderColor);
            var leftBorderColor = GetDistinctProperty(PropertyId.LeftBorderColor);

            if (!visible.IsNull ||
                !display.IsNull ||
                !unicodeBiDiValue.IsNull ||
                !width.IsNull ||
                !height.IsNull)
            {
                if (!styleAttributeOpen)
                {
                    writer.WriteAttributeName(HtmlNameIndex.Style);
                    styleAttributeOpen = true;
                }

                if (!display.IsNull)
                {
                    var val = HtmlSupport.GetDisplayString(display);
                    if (val != null)
                    {
                        scratchBuffer.Append("display:");
                        scratchBuffer.Append(val);
                        scratchBuffer.Append(";");
                    }
                }

                if (!visible.IsNull)
                {
                    scratchBuffer.Append(visible.Bool ? "visibility:visible;" : "visibility:hidden;");
                }

                if (!width.IsNull)
                {
                    var val = HtmlSupport.FormatLength(ref scratchBuffer, width);
                    if (val.Length != 0)
                    {
                        writer.WriteAttributeValue("width:");
                        writer.WriteAttributeValue(val);
                        writer.WriteAttributeValue(";");
                    }
                }

                if (!height.IsNull)
                {
                    var val = HtmlSupport.FormatLength(ref scratchBuffer, height);
                    if (val.Length != 0)
                    {
                        writer.WriteAttributeValue("height:");
                        writer.WriteAttributeValue(val);
                        writer.WriteAttributeValue(";");
                    }
                }

                if (!unicodeBiDiValue.IsNull)
                {
                    var str = HtmlSupport.GetUnicodeBiDiString(unicodeBiDiValue);
                    if (str != null)
                    {
                        writer.WriteAttributeValue("unicode-bidi:");
                        writer.WriteAttributeValue(str);
                        writer.WriteAttributeValue(";");
                    }
                }
            }

            if (!firstLineIndent.IsNull ||
                !textAlignment.IsNull ||
                !backColor.IsNull)
            {
                if (!styleAttributeOpen)
                {
                    writer.WriteAttributeName(HtmlNameIndex.Style);
                    styleAttributeOpen = true;
                }

                if (!firstLineIndent.IsNull)
                {
                    var val = HtmlSupport.FormatLength(ref scratchBuffer, firstLineIndent);
                    if (val.Length != 0)
                    {
                        writer.WriteAttributeValue("text-indent:");
                        writer.WriteAttributeValue(val);
                        writer.WriteAttributeValue(";");
                    }
                }

                if (!textAlignment.IsNull)
                {
                    if (textAlignment.IsEnum && textAlignment.Enum < HtmlSupport.TextAlignmentEnumeration.Length)
                    {
                        writer.WriteAttributeValue("text-align:");
                        writer.WriteAttributeValue(HtmlSupport.TextAlignmentEnumeration[textAlignment.Enum].name);
                        writer.WriteAttributeValue(";");
                    }
                }

                if (!backColor.IsNull)
                {
                    var val = HtmlSupport.FormatColor(ref scratchBuffer, backColor);
                    if (val.Length != 0)
                    {
                        writer.WriteAttributeValue("background-color:");
                        writer.WriteAttributeValue(val);
                        writer.WriteAttributeValue(";");
                    }
                }
            }

            if (!topMargin.IsNull ||
                !rightMargin.IsNull ||
                !bottomMargin.IsNull ||
                !leftMargin.IsNull)
            {
                if (!styleAttributeOpen)
                {
                    writer.WriteAttributeName(HtmlNameIndex.Style);
                    styleAttributeOpen = true;
                }

                OutputMarginAndPaddingProperties(
                        "margin",
                        topMargin, rightMargin, bottomMargin, leftMargin);
            }

            if (!topPadding.IsNull ||
                !rightPadding.IsNull ||
                !bottomPadding.IsNull ||
                !leftPadding.IsNull)
            {
                if (!styleAttributeOpen)
                {
                    writer.WriteAttributeName(HtmlNameIndex.Style);
                    styleAttributeOpen = true;
                }

                OutputMarginAndPaddingProperties(
                        "padding",
                        topPadding, rightPadding, bottomPadding, leftPadding);
            }

            if (!topBorderWidth.IsNull ||
                !rightBorderWidth.IsNull ||
                !bottomBorderWidth.IsNull ||
                !leftBorderWidth.IsNull ||
                !topBorderStyle.IsNull ||
                !rightBorderStyle.IsNull ||
                !bottomBorderStyle.IsNull ||
                !leftBorderStyle.IsNull ||
                !topBorderColor.IsNull ||
                !rightBorderColor.IsNull ||
                !bottomBorderColor.IsNull ||
                !leftBorderColor.IsNull)
            {
                if (!styleAttributeOpen)
                {
                    writer.WriteAttributeName(HtmlNameIndex.Style);
                    styleAttributeOpen = true;
                }

                OutputBorderProperties(
                        topBorderWidth, rightBorderWidth, bottomBorderWidth, leftBorderWidth,
                        topBorderStyle, rightBorderStyle, bottomBorderStyle, leftBorderStyle,
                        topBorderColor, rightBorderColor, bottomBorderColor, leftBorderColor);
            }
        }

        

        private void OutputMarginAndPaddingProperties(
                            string name,    
                            PropertyValue topValue, PropertyValue rightValue, PropertyValue bottomValue, PropertyValue leftValue)
        {
            var numSidesDefined = 0;

            if (!topValue.IsNull)
            {
                numSidesDefined ++;
            }

            if (!rightValue.IsNull)
            {
                numSidesDefined ++;
            }

            if (!bottomValue.IsNull)
            {
                numSidesDefined ++;
            }

            if (!leftValue.IsNull)
            {
                numSidesDefined ++;
            }

            if (numSidesDefined == 4)
            {
                
                writer.WriteAttributeValue(name);
                writer.WriteAttributeValue(":");

                if (topValue == rightValue && topValue == bottomValue && topValue == leftValue)
                {
                    
                    OutputLengthPropertyValue(topValue);
                }
                else
                {
                    if (topValue == bottomValue && rightValue == leftValue)
                    {
                        
                        OutputCompositeLengthPropertyValue(topValue, rightValue);
                    }
                    else
                    {
                        
                        OutputCompositeLengthPropertyValue(topValue, rightValue, bottomValue, leftValue);
                    }
                }

                writer.WriteAttributeValue(";");
            }
            else
            {
                

                if (!topValue.IsNull)
                {
                    
                    writer.WriteAttributeValue(name);
                    writer.WriteAttributeValue("-top:");
                    OutputLengthPropertyValue(topValue);
                    writer.WriteAttributeValue(";");
                }

                if (!rightValue.IsNull)
                {
                    
                    writer.WriteAttributeValue(name);
                    writer.WriteAttributeValue("-right:");
                    OutputLengthPropertyValue(rightValue);
                    writer.WriteAttributeValue(";");
                }

                if (!bottomValue.IsNull)
                {
                    
                    writer.WriteAttributeValue(name);
                    writer.WriteAttributeValue("-bottom:");
                    OutputLengthPropertyValue(bottomValue);
                    writer.WriteAttributeValue(";");
                }

                if (!leftValue.IsNull)
                {
                    
                    writer.WriteAttributeValue(name);
                    writer.WriteAttributeValue("-left:");
                    OutputLengthPropertyValue(leftValue);
                    writer.WriteAttributeValue(";");
                }
            }
        }

        

        private void OutputBorderProperties(
                            PropertyValue topBorderWidth, PropertyValue rightBorderWidth, PropertyValue bottomBorderWidth, PropertyValue leftBorderWidth,
                            PropertyValue topBorderStyle, PropertyValue rightBorderStyle, PropertyValue bottomBorderStyle, PropertyValue leftBorderStyle,
                            PropertyValue topBorderColor, PropertyValue rightBorderColor, PropertyValue bottomBorderColor, PropertyValue leftBorderColor)
        {
            var numSidesDefinedWidth = 0;
            var numSidesDefinedStyle = 0;
            var numSidesDefinedColor = 0;

            var numPropsDefinedTop = 0;
            var numPropsDefinedRight = 0;
            var numPropsDefinedBottom = 0;
            var numPropsDefinedLeft = 0;

            if (!topBorderWidth.IsNull)
            {
                numSidesDefinedWidth ++;
                numPropsDefinedTop ++;
            }

            if (!rightBorderWidth.IsNull)
            {
                numSidesDefinedWidth ++;
                numPropsDefinedRight ++;
            }

            if (!bottomBorderWidth.IsNull)
            {
                numSidesDefinedWidth ++;
                numPropsDefinedBottom ++;
            }

            if (!leftBorderWidth.IsNull)
            {
                numSidesDefinedWidth ++;
                numPropsDefinedLeft ++;
            }

            if (!topBorderStyle.IsNull)
            {
                numSidesDefinedStyle ++;
                numPropsDefinedTop ++;
            }

            if (!rightBorderStyle.IsNull)
            {
                numSidesDefinedStyle ++;
                numPropsDefinedRight ++;
            }

            if (!bottomBorderStyle.IsNull)
            {
                numSidesDefinedStyle ++;
                numPropsDefinedBottom ++;
            }

            if (!leftBorderStyle.IsNull)
            {
                numSidesDefinedStyle ++;
                numPropsDefinedLeft ++;
            }

            if (!topBorderColor.IsNull)
            {
                numSidesDefinedColor ++;
                numPropsDefinedTop ++;
            }

            if (!rightBorderColor.IsNull)
            {
                numSidesDefinedColor ++;
                numPropsDefinedRight ++;
            }

            if (!bottomBorderColor.IsNull)
            {
                numSidesDefinedColor ++;
                numPropsDefinedBottom ++;
            }

            if (!leftBorderColor.IsNull)
            {
                numSidesDefinedColor ++;
                numPropsDefinedLeft ++;
            }

            var allSidesEqualWidth = false;
            var hvSidesEqualWidth = false;
            var allSidesEqualStyle = false;
            var hvSidesEqualStyle = false;
            var allSidesEqualColor = false;
            var hvSidesEqualColor = false;

            if (numSidesDefinedWidth == 4)
            {
                if (topBorderWidth == bottomBorderWidth && rightBorderWidth == leftBorderWidth)
                {
                    hvSidesEqualWidth = true;
                    allSidesEqualWidth = (topBorderWidth == rightBorderWidth);
                }
            }

            if (numSidesDefinedStyle == 4)
            {
                if (topBorderStyle == bottomBorderStyle && rightBorderStyle == leftBorderStyle)
                {
                    hvSidesEqualStyle = true;
                    allSidesEqualStyle = (topBorderStyle == rightBorderStyle);
                }
            }

            if (numSidesDefinedColor == 4)
            {
                if (topBorderColor == bottomBorderColor && rightBorderColor == leftBorderColor)
                {
                    hvSidesEqualColor = true;
                    allSidesEqualColor = (topBorderColor == rightBorderColor);
                }
            }

            if (numSidesDefinedWidth == 4 && numSidesDefinedStyle == 4 && numSidesDefinedColor == 4)
            {
                
                if (allSidesEqualWidth && allSidesEqualStyle && allSidesEqualColor)
                {
                    

                    writer.WriteAttributeValue("border:");
                    OutputCompositeBorderSidePropertyValue(topBorderWidth, topBorderStyle, topBorderColor);
                    writer.WriteAttributeValue(";");
                }
                else
                {
                    
                    

                    writer.WriteAttributeValue("border-width:");
                    if (allSidesEqualWidth)
                    {
                        OutputBorderWidthPropertyValue(topBorderWidth);
                    }
                    else if (hvSidesEqualWidth)
                    {
                        OutputCompositeBorderWidthPropertyValue(topBorderWidth, rightBorderWidth);
                    }
                    else
                    {
                        OutputCompositeBorderWidthPropertyValue(topBorderWidth, rightBorderWidth, bottomBorderWidth, leftBorderWidth);
                    }

                    writer.WriteAttributeValue(";");

                    writer.WriteAttributeValue("border-style:");
                    if (allSidesEqualStyle)
                    {
                        OutputBorderStylePropertyValue(topBorderStyle);
                    }
                    else if (hvSidesEqualStyle)
                    {
                        OutputCompositeBorderStylePropertyValue(topBorderStyle, rightBorderStyle);
                    }
                    else
                    {
                        OutputCompositeBorderStylePropertyValue(topBorderStyle, rightBorderStyle, bottomBorderStyle, leftBorderStyle);
                    }
                    writer.WriteAttributeValue(";");

                    writer.WriteAttributeValue("border-color:");
                    if (allSidesEqualColor)
                    {
                        OutputBorderColorPropertyValue(topBorderColor);
                    }
                    else if (hvSidesEqualColor)
                    {
                        OutputCompositeBorderColorPropertyValue(topBorderColor, rightBorderColor);
                    }
                    else
                    {
                        OutputCompositeBorderColorPropertyValue(topBorderColor, rightBorderColor, bottomBorderColor, leftBorderColor);
                    }
                    writer.WriteAttributeValue(";");
                }
            }
            else
            {
                

                bool topWidthOutput = false, rightWidthOutput = false, bottomWidthOutput = false, leftWidthOutput = false;
                bool topStyleOutput = false, rightStyleOutput = false, bottomStyleOutput = false, leftStyleOutput = false;
                bool topColorOutput = false, rightColorOutput = false, bottomColorOutput = false, leftColorOutput = false;

                if (numSidesDefinedWidth == 4 || numSidesDefinedStyle == 4 || numSidesDefinedColor == 4)
                {
                    

                    if (numSidesDefinedWidth == 4)
                    {
                        writer.WriteAttributeValue("border-width:");
                        if (allSidesEqualWidth)
                        {
                            OutputBorderWidthPropertyValue(topBorderWidth);
                        }
                        else if (hvSidesEqualWidth)
                        {
                            OutputCompositeBorderWidthPropertyValue(topBorderWidth, rightBorderWidth);
                        }
                        else
                        {
                            OutputCompositeBorderWidthPropertyValue(topBorderWidth, rightBorderWidth, bottomBorderWidth, leftBorderWidth);
                        }
                        writer.WriteAttributeValue(";");

                        topWidthOutput = true;
                        rightWidthOutput = true;
                        bottomWidthOutput = true;
                        leftWidthOutput = true;
                    }

                    if (numSidesDefinedStyle == 4)
                    {
                        writer.WriteAttributeValue("border-style:");
                        if (allSidesEqualStyle)
                        {
                            OutputBorderStylePropertyValue(topBorderStyle);
                        }
                        else if (hvSidesEqualStyle)
                        {
                            OutputCompositeBorderStylePropertyValue(topBorderStyle, rightBorderStyle);
                        }
                        else
                        {
                            OutputCompositeBorderStylePropertyValue(topBorderStyle, rightBorderStyle, bottomBorderStyle, leftBorderStyle);
                        }
                        writer.WriteAttributeValue(";");

                        topStyleOutput = true;
                        rightStyleOutput = true;
                        bottomStyleOutput = true;
                        leftStyleOutput = true;
                    }

                    if (numSidesDefinedColor == 4)
                    {
                        writer.WriteAttributeValue("border-color:");
                        if (allSidesEqualColor)
                        {
                            OutputBorderColorPropertyValue(topBorderColor);
                        }
                        else if (hvSidesEqualColor)
                        {
                            OutputCompositeBorderColorPropertyValue(topBorderColor, rightBorderColor);
                        }
                        else
                        {
                            OutputCompositeBorderColorPropertyValue(topBorderColor, rightBorderColor, bottomBorderColor, leftBorderColor);
                        }
                        writer.WriteAttributeValue(";");

                        topColorOutput = true;
                        rightColorOutput = true;
                        bottomColorOutput = true;
                        leftColorOutput = true;
                    }
                }
                else if (numPropsDefinedTop == 3 || numPropsDefinedRight == 3 || numPropsDefinedBottom == 3 || numPropsDefinedLeft == 3)
                {
                    

                    if (numPropsDefinedTop == 3)
                    {
                        writer.WriteAttributeValue("border-top:");
                        OutputCompositeBorderSidePropertyValue(topBorderWidth, topBorderStyle, topBorderColor);
                        writer.WriteAttributeValue(";");

                        topWidthOutput = true;
                        topStyleOutput = true;
                        topColorOutput = true;
                    }

                    if (numPropsDefinedRight == 3)
                    {
                        writer.WriteAttributeValue("border-right:");
                        OutputCompositeBorderSidePropertyValue(rightBorderWidth, rightBorderStyle, rightBorderColor);
                        writer.WriteAttributeValue(";");

                        rightWidthOutput = true;
                        rightStyleOutput = true;
                        rightColorOutput = true;
                    }

                    if (numPropsDefinedBottom == 3)
                    {
                        writer.WriteAttributeValue("border-bottom:");
                        OutputCompositeBorderSidePropertyValue(bottomBorderWidth, bottomBorderStyle, bottomBorderColor);
                        writer.WriteAttributeValue(";");

                        bottomWidthOutput = true;
                        bottomStyleOutput = true;
                        bottomColorOutput = true;
                    }

                    if (numPropsDefinedLeft == 3)
                    {
                        writer.WriteAttributeValue("border-left:");
                        OutputCompositeBorderSidePropertyValue(leftBorderWidth, leftBorderStyle, leftBorderColor);
                        writer.WriteAttributeValue(";");

                        leftWidthOutput = true;
                        leftStyleOutput = true;
                        leftColorOutput = true;
                    }
                }

                

                if (!topWidthOutput && !topBorderWidth.IsNull)
                {
                    writer.WriteAttributeValue("border-top-width:");
                    OutputBorderWidthPropertyValue(topBorderWidth);
                    writer.WriteAttributeValue(";");
                }

                if (!rightWidthOutput && !rightBorderWidth.IsNull)
                {
                    writer.WriteAttributeValue("border-right-width:");
                    OutputBorderWidthPropertyValue(rightBorderWidth);
                    writer.WriteAttributeValue(";");
                }

                if (!bottomWidthOutput && !bottomBorderWidth.IsNull)
                {
                    writer.WriteAttributeValue("border-bottom-width:");
                    OutputBorderWidthPropertyValue(bottomBorderWidth);
                    writer.WriteAttributeValue(";");
                }

                if (!leftWidthOutput && !leftBorderWidth.IsNull)
                {
                    writer.WriteAttributeValue("border-left-width:");
                    OutputBorderWidthPropertyValue(leftBorderWidth);
                    writer.WriteAttributeValue(";");
                }

                if (!topStyleOutput && !topBorderStyle.IsNull)
                {
                    writer.WriteAttributeValue("border-top-style:");
                    OutputBorderStylePropertyValue(topBorderStyle);
                    writer.WriteAttributeValue(";");
                }

                if (!rightStyleOutput && !rightBorderStyle.IsNull)
                {
                    writer.WriteAttributeValue("border-right-style:");
                    OutputBorderStylePropertyValue(rightBorderStyle);
                    writer.WriteAttributeValue(";");
                }

                if (!bottomStyleOutput && !bottomBorderStyle.IsNull)
                {
                    writer.WriteAttributeValue("border-bottom-style:");
                    OutputBorderStylePropertyValue(bottomBorderStyle);
                    writer.WriteAttributeValue(";");
                }

                if (!leftStyleOutput && !leftBorderStyle.IsNull)
                {
                    writer.WriteAttributeValue("border-left-style:");
                    OutputBorderStylePropertyValue(leftBorderStyle);
                    writer.WriteAttributeValue(";");
                }

                if (!topColorOutput && !topBorderColor.IsNull)
                {
                    writer.WriteAttributeValue("border-top-color:");
                    OutputBorderColorPropertyValue(topBorderColor);
                    writer.WriteAttributeValue(";");
                }

                if (!rightColorOutput && !rightBorderColor.IsNull)
                {
                    writer.WriteAttributeValue("border-right-color:");
                    OutputBorderColorPropertyValue(rightBorderColor);
                    writer.WriteAttributeValue(";");
                }

                if (!bottomColorOutput && !bottomBorderColor.IsNull)
                {
                    writer.WriteAttributeValue("border-bottom-color:");
                    OutputBorderColorPropertyValue(bottomBorderColor);
                    writer.WriteAttributeValue(";");
                }

                if (!leftColorOutput && !leftBorderColor.IsNull)
                {
                    writer.WriteAttributeValue("border-left-color:");
                    OutputBorderColorPropertyValue(leftBorderColor);
                    writer.WriteAttributeValue(";");
                }
            }
        }

        private void OutputCompositeBorderSidePropertyValue(PropertyValue width, PropertyValue style, PropertyValue color)
        {
            OutputBorderWidthPropertyValue(width);
            writer.WriteAttributeValue(" ");
            OutputBorderStylePropertyValue(style);
            writer.WriteAttributeValue(" ");
            OutputBorderColorPropertyValue(color);
        }

        private void OutputCompositeLengthPropertyValue(PropertyValue topBottom, PropertyValue rightLeft)
        {
            OutputLengthPropertyValue(topBottom);
            writer.WriteAttributeValue(" ");
            OutputLengthPropertyValue(rightLeft);
        }

        private void OutputCompositeLengthPropertyValue(PropertyValue top, PropertyValue right, PropertyValue bottom, PropertyValue left)
        {
            OutputLengthPropertyValue(top);
            writer.WriteAttributeValue(" ");
            OutputLengthPropertyValue(right);
            writer.WriteAttributeValue(" ");
            OutputLengthPropertyValue(bottom);
            writer.WriteAttributeValue(" ");
            OutputLengthPropertyValue(left);
        }

        private void OutputCompositeBorderWidthPropertyValue(PropertyValue topBottom, PropertyValue rightLeft)
        {
            OutputBorderWidthPropertyValue(topBottom);
            writer.WriteAttributeValue(" ");
            OutputBorderWidthPropertyValue(rightLeft);
        }

        private void OutputCompositeBorderWidthPropertyValue(PropertyValue top, PropertyValue right, PropertyValue bottom, PropertyValue left)
        {
            OutputBorderWidthPropertyValue(top);
            writer.WriteAttributeValue(" ");
            OutputBorderWidthPropertyValue(right);
            writer.WriteAttributeValue(" ");
            OutputBorderWidthPropertyValue(bottom);
            writer.WriteAttributeValue(" ");
            OutputBorderWidthPropertyValue(left);
        }

        private void OutputCompositeBorderStylePropertyValue(PropertyValue topBottom, PropertyValue rightLeft)
        {
            OutputBorderStylePropertyValue(topBottom);
            writer.WriteAttributeValue(" ");
            OutputBorderStylePropertyValue(rightLeft);
        }

        private void OutputCompositeBorderStylePropertyValue(PropertyValue top, PropertyValue right, PropertyValue bottom, PropertyValue left)
        {
            OutputBorderStylePropertyValue(top);
            writer.WriteAttributeValue(" ");
            OutputBorderStylePropertyValue(right);
            writer.WriteAttributeValue(" ");
            OutputBorderStylePropertyValue(bottom);
            writer.WriteAttributeValue(" ");
            OutputBorderStylePropertyValue(left);
        }

        private void OutputCompositeBorderColorPropertyValue(PropertyValue topBottom, PropertyValue rightLeft)
        {
            OutputBorderColorPropertyValue(topBottom);
            writer.WriteAttributeValue(" ");
            OutputBorderColorPropertyValue(rightLeft);
        }

        private void OutputCompositeBorderColorPropertyValue(PropertyValue top, PropertyValue right, PropertyValue bottom, PropertyValue left)
        {
            OutputBorderColorPropertyValue(top);
            writer.WriteAttributeValue(" ");
            OutputBorderColorPropertyValue(right);
            writer.WriteAttributeValue(" ");
            OutputBorderColorPropertyValue(bottom);
            writer.WriteAttributeValue(" ");
            OutputBorderColorPropertyValue(left);
        }

        private void OutputLengthPropertyValue(PropertyValue width)
        {
            var val = HtmlSupport.FormatLength(ref scratchBuffer, width);
            InternalDebug.Assert(val.Length != 0);
            if (val.Length != 0)
            {
                writer.WriteAttributeValue(val);
            }
            else
            {
                writer.WriteAttributeValue("0");
            }
        }

        private void OutputBorderWidthPropertyValue(PropertyValue width)
        {
            var val = HtmlSupport.FormatLength(ref scratchBuffer, width);
            InternalDebug.Assert(val.Length != 0);
            if (val.Length != 0)
            {
                writer.WriteAttributeValue(val);
            }
            else
            {
                writer.WriteAttributeValue("medium");
            }
        }

        private void OutputBorderStylePropertyValue(PropertyValue style)
        {
            InternalDebug.Assert(!style.IsNull);

            var str = HtmlSupport.GetBorderStyleString(style);
            InternalDebug.Assert(str != null);
            if (str != null)
            {
                writer.WriteAttributeValue(str);
            }
            else
            {
                writer.WriteAttributeValue("solid");
            }
        }

        private void OutputBorderColorPropertyValue(PropertyValue color)
        {
            InternalDebug.Assert(!color.IsNull);

            var val = HtmlSupport.FormatColor(ref scratchBuffer, color);
            InternalDebug.Assert(val.Length != 0);
            if (val.Length != 0)
            {
                writer.WriteAttributeValue(val);
            }
            else
            {
                writer.WriteAttributeValue("black");
            }
        }

        

        private void OutputTableCssProperties(ref bool styleAttributeOpen)
        {
            var layoutFixed = GetDistinctProperty(PropertyId.TableLayoutFixed);
            var borderCollapse = GetDistinctProperty(PropertyId.TableBorderCollapse);
            var showEmptyCells = GetDistinctProperty(PropertyId.TableShowEmptyCells);
            var captionSideTop = GetDistinctProperty(PropertyId.TableCaptionSideTop);
            var spacingVertical = GetDistinctProperty(PropertyId.TableBorderSpacingVertical);
            var spacingHorizontal = GetDistinctProperty(PropertyId.TableBorderSpacingHorizontal);

            if (!layoutFixed.IsNull ||
                !borderCollapse.IsNull ||
                !showEmptyCells.IsNull ||
                !captionSideTop.IsNull ||
                !spacingVertical.IsNull ||
                !spacingHorizontal.IsNull)
            {
                if (!styleAttributeOpen)
                {
                    writer.WriteAttributeName(HtmlNameIndex.Style);
                    styleAttributeOpen = true;
                }

                if (!layoutFixed.IsNull)
                {
                    writer.WriteAttributeValue("table-layout:");
                    writer.WriteAttributeValue(layoutFixed.Bool ? "fixed" : "auto");
                    writer.WriteAttributeValue(";");
                }

                if (!borderCollapse.IsNull)
                {
                    writer.WriteAttributeValue("border-collapse:");
                    writer.WriteAttributeValue(borderCollapse.Bool ? "collapse" : "separate");
                    writer.WriteAttributeValue(";");
                }

                if (!showEmptyCells.IsNull)
                {
                    writer.WriteAttributeValue("empty-cells:");
                    writer.WriteAttributeValue(showEmptyCells.Bool ? "show" : "hide");
                    writer.WriteAttributeValue(";");
                }

                if (!captionSideTop.IsNull)
                {
                    writer.WriteAttributeValue("caption-side:");
                    writer.WriteAttributeValue(captionSideTop.Bool ? "top" : "bottom");
                    writer.WriteAttributeValue(";");
                }

                if (!spacingVertical.IsNull && !spacingVertical.IsNull)
                {
                    var val = HtmlSupport.FormatLength(ref scratchBuffer, spacingVertical);
                    if (val.Length != 0)
                    {
                        writer.WriteAttributeValue("border-spacing:");
                        writer.WriteAttributeValue(val);

                        if (spacingVertical != spacingHorizontal)
                        {
                            val = HtmlSupport.FormatLength(ref scratchBuffer, spacingHorizontal);
                            if (val.Length != 0)
                            {
                                writer.WriteAttributeValue(" ");
                                writer.WriteAttributeValue(val);
                            }
                        }
                        writer.WriteAttributeValue(";");
                    }
                }
            }
        }

        

        private void OutputTableColumnCssProperties(ref bool styleAttributeOpen)
        {
            var width = GetDistinctProperty(PropertyId.Width);
            var backColor = GetDistinctProperty(PropertyId.BackColor);

            if (!backColor.IsNull ||
                !width.IsNull)
            {
                if (!styleAttributeOpen)
                {
                    writer.WriteAttributeName(HtmlNameIndex.Style);
                    styleAttributeOpen = true;
                }

                if (!width.IsNull)
                {
                    var val = HtmlSupport.FormatLength(ref scratchBuffer, width);
                    if (val.Length != 0)
                    {
                        writer.WriteAttributeValue("width:");
                        writer.WriteAttributeValue(val);
                        writer.WriteAttributeValue(";");
                    }
                }

                if (!backColor.IsNull)
                {
                    var val = HtmlSupport.FormatColor(ref scratchBuffer, backColor);
                    if (val.Length != 0)
                    {
                        writer.WriteAttributeValue("background-color:");
                        writer.WriteAttributeValue(val);
                    }
                }
            }
        }

        

        private void OutputBlockTagAttributes()
        {
            var rtl = GetDistinctProperty(PropertyId.RightToLeft);
            if (!rtl.IsNull)
            {
                writer.WriteAttribute(HtmlNameIndex.Dir, rtl.Bool ? "rtl" : "ltr");
            }

            var textAlignment = GetDistinctProperty(PropertyId.TextAlignment);
            if (!textAlignment.IsNull)
            {
                var val = HtmlSupport.GetTextAlignmentString(textAlignment);
                if (val != null)
                {
                    writer.WriteAttribute(HtmlNameIndex.Align, val);
                }
            }
        }

        

        private void OutputTableTagAttributes()
        {
            var width = GetDistinctProperty(PropertyId.Width);
            if (!width.IsNull)
            {
                var val = HtmlSupport.FormatPixelOrPercentageLength(ref scratchBuffer, width);
                if (val.Length != 0)
                {
                    writer.WriteAttribute(HtmlNameIndex.Width, val);
                }
            }

            var align = GetDistinctProperty(PropertyId.HorizontalAlignment);
            if (!align.IsNull)
            {
                var val = HtmlSupport.GetHorizontalAlignmentString(align);
                if (val != null)
                {
                    writer.WriteAttribute(HtmlNameIndex.Align, val);
                }
            }

            var rtl = GetDistinctProperty(PropertyId.RightToLeft);
            if (!rtl.IsNull)
            {
                writer.WriteAttribute(HtmlNameIndex.Dir, rtl.Bool ? "rtl" : "ltr");
            }

            var border = GetDistinctProperty(PropertyId.TableBorder);
            if (!border.IsNull)
            {
                var val = HtmlSupport.FormatPixelOrPercentageLength(ref scratchBuffer, border);
                if (val.Length != 0)
                {
                    writer.WriteAttribute(HtmlNameIndex.Border, val);
                }
            }

            var frame = GetDistinctProperty(PropertyId.TableFrame);
            if (!frame.IsNull)
            {
                var val = HtmlSupport.GetTableFrameString(frame);
                if (val != null)
                {
                    writer.WriteAttribute(HtmlNameIndex.Frame, val);
                }
            }

            var rules = GetDistinctProperty(PropertyId.TableRules);
            if (!rules.IsNull)
            {
                var val = HtmlSupport.GetTableRulesString(rules);
                if (val != null)
                {
                    writer.WriteAttribute(HtmlNameIndex.Rules, val);
                }
            }

            var cellSpacing = GetDistinctProperty(PropertyId.TableCellSpacing);
            if (!cellSpacing.IsNull)
            {
                var val = HtmlSupport.FormatPixelOrPercentageLength(ref scratchBuffer, cellSpacing);
                if (val.Length != 0)
                {
                    writer.WriteAttribute(HtmlNameIndex.CellSpacing, val);
                }
            }

            var cellPadding = GetDistinctProperty(PropertyId.TableCellPadding);
            if (!cellPadding.IsNull)
            {
                var val = HtmlSupport.FormatPixelOrPercentageLength(ref scratchBuffer, cellPadding);
                if (val.Length != 0)
                {
                    writer.WriteAttribute(HtmlNameIndex.CellPadding, val);
                }
            }

        }

        

        private void OutputTableCellTagAttributes()
        {
            var colSpan = GetDistinctProperty(PropertyId.NumColumns);
            if (colSpan.IsInteger && colSpan.Integer != 1)
            {
                writer.WriteAttribute(HtmlNameIndex.ColSpan, colSpan.Integer.ToString());
            }

            var rowSpan = GetDistinctProperty(PropertyId.NumRows);
            if (rowSpan.IsInteger && rowSpan.Integer != 1)
            {
                writer.WriteAttribute(HtmlNameIndex.RowSpan, rowSpan.Integer.ToString());
            }

            var width = GetDistinctProperty(PropertyId.Width);
            if (!width.IsNull && width.IsAbsRelLength)
            {
                writer.WriteAttribute(HtmlNameIndex.Width, width.PixelsInteger.ToString());
            }

            var textAlignment = GetDistinctProperty(PropertyId.TextAlignment);
            if (!textAlignment.IsNull)
            {
                var val = HtmlSupport.GetTextAlignmentString(textAlignment);
                if (val != null)
                {
                    writer.WriteAttribute(HtmlNameIndex.Align, val);
                }
            }

            var verticalAlignment = GetDistinctProperty(PropertyId.VerticalAlignment);
            if (!verticalAlignment.IsNull)
            {
                var val = HtmlSupport.GetVerticalAlignmentString(verticalAlignment);
                if (val != null)
                {
                    writer.WriteAttribute(HtmlNameIndex.Valign, val);
                }
            }

            var noWrap = GetDistinctProperty(PropertyId.TableCellNoWrap);
            if (!noWrap.IsNull && noWrap.Bool)
            {
                writer.WriteAttribute(HtmlNameIndex.NoWrap, "");
            }
        }

        

        private bool RecognizeHyperLink(TextRun run, out int offset, out int length, out bool addFilePrefix, out bool addHttpPrefix)
        {
            
            
            

            
            
            

            scratchBuffer.Reset();

            run.AppendFragment(0, ref scratchBuffer, 10);

            offset = 0;
            length = 0;

            
            while (offset < 3 && (scratchBuffer[offset] == '<' || scratchBuffer[offset] == '\"' || scratchBuffer[offset] == '\'' ||
                scratchBuffer[offset] == '(' || scratchBuffer[offset] == '['))
            {
                offset ++;
            }

            
            
            
            
            
            

            var link = false;
            addHttpPrefix = false;
            addFilePrefix = false;

            if (scratchBuffer[offset] == '\\')
            {
                if (scratchBuffer[offset + 1] == '\\' &&
                    Char.IsLetterOrDigit(scratchBuffer[offset + 2]))
                {
                    
                    link = true;
                    addFilePrefix = true;
                }
            }
            else if (scratchBuffer[offset] == 'h')
            {
                if (scratchBuffer[offset + 1] == 't' &&
                    scratchBuffer[offset + 2] == 't' &&
                    scratchBuffer[offset + 3] == 'p' &&
                        (scratchBuffer[offset + 4] == ':' ||
                        (scratchBuffer[offset + 4] == 's' && scratchBuffer[offset + 5] == ':')))
                {
                    
                    link = true;
                }
            }
            else if (scratchBuffer[offset] == 'f')
            {
                if (scratchBuffer[offset + 1] == 't' &&
                    scratchBuffer[offset + 2] == 'p' &&
                    scratchBuffer[offset + 3] == ':')
                {
                    
                    link = true;
                }
                else if (scratchBuffer[offset + 1] == 'i' &&
                    scratchBuffer[offset + 2] == 'l' &&
                    scratchBuffer[offset + 3] == 'e' &&
                    scratchBuffer[offset + 4] == ':' &&
                    scratchBuffer[offset + 5] == '/' &&
                    scratchBuffer[offset + 6] == '/')
                {
                    
                    
                    
                    link = true;
                }
            }
            else if (scratchBuffer[offset] == 'm')
            {
                if (scratchBuffer[offset + 1] == 'a' &&
                    scratchBuffer[offset + 2] == 'i' &&
                    scratchBuffer[offset + 3] == 'l' &&
                    scratchBuffer[offset + 4] == 't' &&
                    scratchBuffer[offset + 5] == 'o' &&
                    scratchBuffer[offset + 6] == ':')
                {
                    
                    link = true;
                }
            }
            else if (scratchBuffer[offset] == 'w')
            {
                if (scratchBuffer[offset + 1] == 'w' &&
                    scratchBuffer[offset + 2] == 'w' &&
                    scratchBuffer[offset + 3] == '.')
                {
                    
                    link = true;
                    addHttpPrefix = true;
                }
            }

            if (link)
            {
                var end = 10 + run.AppendFragment(10, ref scratchBuffer, MaxRecognizedHyperlinkLength - 10);

                
                while (scratchBuffer[end - 1] == '>' || scratchBuffer[end - 1] == '\"' || scratchBuffer[end - 1] == '\'' || 
                    scratchBuffer[end - 1] == ')' || scratchBuffer[end - 1] == ']' || 
                    scratchBuffer[end - 1] == '.' || scratchBuffer[end - 1] == ',' || scratchBuffer[end - 1] == ';')
                {
                    end --;
                }

                length = end - offset;
            }

            return link;
        }

        

        protected override void Dispose(bool disposing)
        {
            if (writer != null && writer is IDisposable)
            {
                ((IDisposable)writer).Dispose();
            }

            writer = null;

            base.Dispose(disposing);
        }
    }

    

    internal class HtmlFormatOutputCallbackContext : HtmlTagContext
    {
        private const int MaxCallbackAttributes = 10;

        private HtmlFormatOutput formatOutput;
        private int countAttributes;
        private AttributeDescriptor[] attributes = new AttributeDescriptor[MaxCallbackAttributes];

        private static readonly HtmlAttributeParts CompleteAttributeParts = new HtmlAttributeParts(HtmlToken.AttrPartMajor.Complete, HtmlToken.AttrPartMinor.CompleteName | HtmlToken.AttrPartMinor.CompleteValue);
        private static readonly HtmlTagParts CompleteTagWithAttributesParts = new HtmlTagParts(HtmlToken.TagPartMajor.Complete, HtmlToken.TagPartMinor.CompleteName | HtmlToken.TagPartMinor.Attributes);
        private static readonly HtmlTagParts CompleteTagWithoutAttributesParts = new HtmlTagParts(HtmlToken.TagPartMajor.Complete, HtmlToken.TagPartMinor.CompleteName);

        private struct AttributeDescriptor
        {
            public HtmlNameIndex nameIndex;
            public string value;
            public int readIndex;
        }

        public HtmlFormatOutputCallbackContext(HtmlFormatOutput formatOutput)
        {
            this.formatOutput = formatOutput;
        }

        public new void InitializeTag(bool isEndTag, HtmlNameIndex tagNameIndex, bool tagDropped)
        {
            base.InitializeTag(isEndTag, tagNameIndex, tagDropped);
            countAttributes = 0;
        }

        internal void Reset()
        {
            countAttributes = 0;
        }

        internal void AddAttribute(HtmlNameIndex nameIndex, string value)
        {
            InternalDebug.Assert(countAttributes < MaxCallbackAttributes);

            attributes[countAttributes].nameIndex = nameIndex;
            attributes[countAttributes].value = value;
            attributes[countAttributes].readIndex = 0;
            countAttributes ++;
        }

        public void InitializeFragment(bool isEmptyElementTag)
        {
            base.InitializeFragment(isEmptyElementTag, countAttributes, countAttributes == 0 ? CompleteTagWithoutAttributesParts : CompleteTagWithAttributesParts);
        }

        
        

        internal override string GetTagNameImpl()
        {
            InternalDebug.Assert(TagNameIndex > HtmlNameIndex.Unknown);
            return HtmlNameData.names[(int)TagNameIndex].name;
        }

        internal override HtmlAttributeId GetAttributeNameIdImpl(int attributeIndex)
        {
            return HtmlNameData.names[(int)attributes[attributeIndex].nameIndex].publicAttributeId;
        }

        internal override HtmlAttributeParts GetAttributePartsImpl(int attributeIndex)
        {
            return CompleteAttributeParts;
        }

        internal override string GetAttributeNameImpl(int attributeIndex)
        {
            InternalDebug.Assert(attributes[attributeIndex].nameIndex > HtmlNameIndex.Unknown);
            return HtmlNameData.names[(int)attributes[attributeIndex].nameIndex].name;
        }

        

        internal override string GetAttributeValueImpl(int attributeIndex)
        {
            return attributes[attributeIndex].value;
        }

        internal override int ReadAttributeValueImpl(int attributeIndex, char[] buffer, int offset, int count)
        {
            var countToCopy = Math.Min(count, attributes[attributeIndex].value.Length - attributes[attributeIndex].readIndex);

            if (countToCopy != 0)
            {
                attributes[attributeIndex].value.CopyTo(attributes[attributeIndex].readIndex, buffer, offset, countToCopy);
                attributes[attributeIndex].readIndex += countToCopy;
            }

            return countToCopy;
        }

        internal override void WriteTagImpl(bool copyTagAttributes)
        {
            formatOutput.writer.WriteTagBegin(TagNameIndex, null, IsEndTag, false, false);

            if (copyTagAttributes)
            {
                for (var i = 0; i < countAttributes; i++)
                {
                    WriteAttributeImpl(i, true, true);
                }
            }
        }

        internal override void WriteAttributeImpl(int attributeIndex, bool writeName, bool writeValue)
        {
            if (writeName)
            {
                formatOutput.writer.WriteAttributeName(attributes[attributeIndex].nameIndex);
            }

            if (writeValue)
            {
                formatOutput.writer.WriteAttributeValue(attributes[attributeIndex].value);
            }
        }
    }
}

