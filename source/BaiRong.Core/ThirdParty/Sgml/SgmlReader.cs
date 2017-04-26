using System;
using System.Xml;
using System.IO;
using System.Text;

namespace BaiRong.Core.Text.Sgml
{
    /// <summary>
    /// SGML is case insensitive, so here you can choose between converting
    /// to lower case or upper case tags.  "None" means that the case is left
    /// alone, except that end tags will be folded to match the start tags.
    /// </summary>
    public enum CaseFolding
    {
        None,
        ToUpper,
        ToLower
    }

    /// <summary>
    /// This stack maintains a high water mark for allocated objects so the client
    /// can reuse the objects in the stack to reduce memory allocations, this is
    /// used to maintain current state of the parser for element stack, and attributes
    /// in each element.
    /// </summary>
    internal class HWStack
    {
        object[] items;
        int size;
        int count;
        int growth;

        public HWStack(int growth)
        {
            this.growth = growth;
        }

        public int Count
        {
            get { return count; }
            set { count = value; }
        }
        public int Size => size;
        // returns the item at the requested index or null if index is out of bounds
        public object this[int i]
        {
            get { return (i >= 0 && i < size) ? items[i] : null; }
            set { items[i] = value; }
        }
        public object Pop()
        {
            count--;
            if (count > 0)
            {
                return items[count - 1];
            }
            return null;
        }
        // This method tries to reuse a slot, if it returns null then
        // the user has to call the other Push method.
        public object Push()
        {
            if (count == size)
            {
                var newsize = size + growth;
                var newarray = new object[newsize];
                if (items != null)
                    Array.Copy(items, newarray, size);
                size = newsize;
                items = newarray;
            }
            return items[count++];
        }
        public void RemoveAt(int i)
        {
            items[i] = null;
            Array.Copy(items, i + 1, items, i, count - i - 1);
            count--;

        }
    }

    /// <summary>
    /// This class represents an attribute.  The AttDef is assigned
    /// from a validation process, and is used to provide default values.
    /// </summary>
    internal class Attribute
    {
        internal string Name;    // the atomized name (using XmlNameTable).
        internal AttDef DtdType; // the AttDef of the attribute from the SGML DTD.
        internal char QuoteChar; // the quote character used for the attribute value.
        internal string literalValue; // tha attribute value

        /// <summary>
        /// Attribute objects are reused during parsing to reduce memory allocations, 
        /// hence the Reset method. 
        /// </summary>
        public void Reset(string name, string value, char quote)
        {
            Name = name;
            literalValue = value;
            QuoteChar = quote;
            DtdType = null;
        }

        public string Value
        {
            get
            {
                if (literalValue != null)
                    return literalValue;
                if (DtdType != null)
                    return DtdType.Default;
                return null;
            }
            set
            {
                literalValue = value;
            }
        }

        public bool IsDefault => (literalValue == null);
    }

    /// <summary>
    /// This class models an XML node, an array of elements in scope is maintained while parsing
    /// for validation purposes, and these Node objects are reused to reduce object allocation,
    /// hence the reset method.  
    /// </summary>
    internal class Node
    {
        internal XmlNodeType NodeType;
        internal string Value;
        internal XmlSpace Space;
        internal string XmlLang;
        internal bool IsEmpty;
        internal string Name;
        internal ElementDecl DtdType; // the DTD type found via validation
        internal State CurrentState;
        internal bool Simulated; // tag was injected into result stream.
        HWStack attributes = new HWStack(10);

        /// <summary>
        /// Attribute objects are reused during parsing to reduce memory allocations, 
        /// hence the Reset method. 
        /// </summary>
        public void Reset(string name, XmlNodeType nt, string value)
        {
            Value = value;
            Name = name;
            NodeType = nt;
            Space = XmlSpace.None;
            XmlLang = null;
            IsEmpty = true;
            attributes.Count = 0;
            DtdType = null;
        }

        public Attribute AddAttribute(string name, string value, char quotechar, bool caseInsensitive)
        {
            Attribute a;
            // check for duplicates!
            for (int i = 0, n = attributes.Count; i < n; i++)
            {
                a = (Attribute)attributes[i];
                if (caseInsensitive && string.Compare(a.Name, name, true) == 0)
                {
                    return null;
                }
                else if ((object)a.Name == (object)name)
                {
                    return null;
                }
            }
            // This code makes use of the high water mark for attribute objects,
            // and reuses exisint Attribute objects to avoid memory allocation.
            a = (Attribute)attributes.Push();
            if (a == null)
            {
                a = new Attribute();
                attributes[attributes.Count - 1] = a;
            }
            a.Reset(name, value, quotechar);
            return a;
        }

        public void RemoveAttribute(string name)
        {
            for (int i = 0, n = attributes.Count; i < n; i++)
            {
                var a = (Attribute)attributes[i];
                if (a.Name == name)
                {
                    attributes.RemoveAt(i);
                    return;
                }
            }
        }
        public void CopyAttributes(Node n)
        {
            for (int i = 0, len = n.attributes.Count; i < len; i++)
            {
                var a = (Attribute)n.attributes[i];
                var na = AddAttribute(a.Name, a.Value, a.QuoteChar, false);
                na.DtdType = a.DtdType;
            }
        }

        public int AttributeCount => attributes.Count;

        public int GetAttribute(string name)
        {
            for (int i = 0, n = attributes.Count; i < n; i++)
            {
                var a = (Attribute)attributes[i];
                if (a.Name == name)
                {
                    return i;
                }
            }
            return -1;
        }

        public Attribute GetAttribute(int i)
        {
            if (i >= 0 && i < attributes.Count)
            {
                var a = (Attribute)attributes[i];
                return a;
            }
            return null;
        }
    }

    // This enum is used to track the current state of te SgmlReader
    internal enum State
    {
        Initial,    // The initial state (Read has not been called yet)
        Markup,     // Expecting text or markup
        EndTag,     // Positioned on an end tag
        Attr,       // Positioned on an attribute
        AttrValue,  // Positioned in an attribute value
        Text,       // Positioned on a Text node.
        PartialTag, // Positioned on a text node, and we have hit a start tag
        AutoClose,  // We are auto-closing tags (this is like State.EndTag), but end tag was generated
        CData,      // We are on a CDATA type node, eg. <scipt> where we have special parsing rules.
        PartialText,
        PseudoStartTag, // we pushed a pseudo-start tag, need to continue with previous start tag.
        Eof
    }


    /// <summary>
    /// SgmlReader is an XmlReader API over any SGML document (including built in 
    /// support for HTML).  
    /// </summary>
    public class SgmlReader : XmlReader
    {
        SgmlDtd dtd;
        Entity current;
        State state;
        XmlNameTable nametable;
        char partial;
        object endTag;
        HWStack stack;
        Node node; // current node (except for attributes)
        // Attributes are handled separately using these members.
        Attribute a;
        int apos; // which attribute are we positioned on in the collection.
        Uri baseUri;
        StringBuilder sb;
        StringBuilder name;
        TextWriter log;
        bool foundRoot;

        // autoclose support
        Node newnode;
        int poptodepth;
        int rootCount;
        bool isHtml;
        string rootElementName;

        string href;
        string errorLogFile;
        Entity lastError;
        string proxy;
        TextReader inputStream;
        string syslit;
        string pubid;
        string subset;
        string docType;
        WhitespaceHandling whitespaceHandling;
        CaseFolding folding = CaseFolding.None;
        bool stripDocType = true;
        string startTag = null;

        public SgmlReader()
        {
            Init();
            nametable = new NameTable();
        }

        /// <summary>
        /// Specify the SgmlDtd object directly.  This allows you to cache the Dtd and share
        /// it across multipl SgmlReaders.  To load a DTD from a URL use the SystemLiteral property.
        /// </summary>
        public SgmlDtd Dtd
        {
            get
            {
                LazyLoadDtd(baseUri);
                return dtd;
            }
            set { dtd = value; }
        }

        private void LazyLoadDtd(Uri baseUri)
        {
            if (dtd == null)
            {
                if (syslit == null || syslit == string.Empty)
                {
                    if (docType != null && StringUtilities.EqualsIgnoreCase(docType, "html"))
                    {
                        var a = typeof(SgmlReader).Assembly;
                        var name = a.FullName.Split(',')[0] + ".Html.dtd";
                        var stm = a.GetManifestResourceStream(name);
                        if (stm != null)
                        {
                            var sr = new StreamReader(stm);
                            dtd = SgmlDtd.Parse(baseUri, "HTML", null, sr, null, proxy, nametable);
                        }
                    }
                }
                else
                {
                    if (baseUri != null)
                    {
                        baseUri = new Uri(baseUri, syslit);
                    }
                    else if (this.baseUri != null)
                    {
                        baseUri = new Uri(this.baseUri, syslit);
                    }
                    else
                    {
                        baseUri = new Uri(new Uri(Directory.GetCurrentDirectory() + "\\"), syslit);
                    }
                    dtd = SgmlDtd.Parse(baseUri, docType, pubid, baseUri.AbsoluteUri, subset, proxy, nametable);
                }

                if (dtd != null && dtd.Name != null)
                {
                    switch (CaseFolding)
                    {
                        case CaseFolding.ToUpper:
                            rootElementName = dtd.Name.ToUpper();
                            break;
                        case CaseFolding.ToLower:
                            rootElementName = dtd.Name.ToLower();
                            break;
                        default:
                            rootElementName = dtd.Name;
                            break;
                    }
                    isHtml = StringUtilities.EqualsIgnoreCase(dtd.Name, "html");
                }

            }
        }

        /// <summary>
        /// The name of root element specified in the DOCTYPE tag.
        /// </summary>
        public string DocType
        {
            get { return docType; }
            set { docType = value; }
        }

        /// <summary>
        /// The PUBLIC identifier in the DOCTYPE tag
        /// </summary>
        public string PublicIdentifier
        {
            get { return pubid; }
            set { pubid = value; }
        }

        /// <summary>
        /// The SYSTEM literal in the DOCTYPE tag identifying the location of the DTD.
        /// </summary>
        public string SystemLiteral
        {
            get { return syslit; }
            set { syslit = value; }
        }

        /// <summary>
        /// The DTD internal subset in the DOCTYPE tag
        /// </summary>
        public string InternalSubset
        {
            get { return subset; }
            set { subset = value; }
        }

        /// <summary>
        /// The input stream containing SGML data to parse.
        /// You must specify this property or the Href property before calling Read().
        /// </summary>
        public TextReader InputStream
        {
            get { return inputStream; }
            set { inputStream = value; Init(); }
        }

        /// <summary>
        /// Sometimes you need to specify a proxy server in order to load data via HTTP
        /// from outside the firewall.  For example: "itgproxy:80".
        /// </summary>
        public string WebProxy
        {
            get { return proxy; }
            set { proxy = value; }
        }

        /// <summary>
        /// The base Uri is used to resolve relative Uri's like the SystemLiteral and
        /// Href properties.  This is a method because BaseURI is a read-only
        /// property on the base XmlReader class.
        /// </summary>
        public void SetBaseUri(string uri)
        {
            baseUri = new Uri(uri);
        }

        /// <summary>
        /// Specify the location of the input SGML document as a URL.
        /// </summary>
        public string Href
        {
            get { return href; }
            set
            {
                href = value;
                Init();
                if (baseUri == null)
                {
                    if (href.IndexOf("://") > 0)
                    {
                        baseUri = new Uri(href);
                    }
                    else
                    {
                        baseUri = new Uri("file:///" + Directory.GetCurrentDirectory() + "//");
                    }
                }
            }
        }

        /// <summary>
        /// Whether to strip out the DOCTYPE tag from the output (default true)
        /// </summary>
        public bool StripDocType
        {
            get { return stripDocType; }
            set { stripDocType = value; }
        }

        public CaseFolding CaseFolding
        {
            get { return folding; }
            set { folding = value; }
        }

        /// <summary>
        /// DTD validation errors are written to this stream.
        /// </summary>
        public TextWriter ErrorLog
        {
            get { return log; }
            set { log = value; }
        }

        /// <summary>
        /// DTD validation errors are written to this log file.
        /// </summary>
        public string ErrorLogFile
        {
            get { return errorLogFile; }
            set
            {
                errorLogFile = value;
                ErrorLog = new StreamWriter(value);
            }
        }

        void Log(string msg, params string[] args)
        {
            if (ErrorLog != null)
            {
                var err = string.Format(msg, args);
                if (lastError != current)
                {
                    err = err + "    " + current.Context();
                    lastError = current;
                    ErrorLog.WriteLine("### Error:" + err);
                }
                else
                {
                    var path = string.Empty;
                    if (current.ResolvedUri != null)
                    {
                        path = current.ResolvedUri.AbsolutePath;
                    }
                    ErrorLog.WriteLine("### Error in " +
                        path + "#" +
                        current.Name +
                        ", line " + current.Line + ", position " + current.LinePosition + ": " +
                        err);
                }
            }
        }
        void Log(string msg, char ch)
        {
            Log(msg, ch.ToString());
        }


        void Init()
        {
            state = State.Initial;
            stack = new HWStack(10);
            node = Push(null, XmlNodeType.Document, null);
            node.IsEmpty = false;
            sb = new StringBuilder();
            name = new StringBuilder();
            poptodepth = 0;
            current = null;
            partial = '\0';
            endTag = null;
            a = null;
            apos = 0;
            newnode = null;
            rootCount = 0;
            foundRoot = false;
        }

        Node Push(string name, XmlNodeType nt, string value)
        {
            var result = (Node)stack.Push();
            if (result == null)
            {
                result = new Node();
                stack[stack.Count - 1] = result;
            }
            result.Reset(name, nt, value);
            node = result;
            return result;
        }

        void SwapTopNodes()
        {
            var top = stack.Count - 1;
            if (top > 0)
            {
                var n = (Node)stack[top - 1];
                stack[top - 1] = stack[top];
                stack[top] = n;
            }
        }

        Node Push(Node n)
        {
            // we have to do a deep clone of the Node object because
            // it is reused in the stack.
            var n2 = Push(n.Name, n.NodeType, n.Value);
            n2.DtdType = n.DtdType;
            n2.IsEmpty = n.IsEmpty;
            n2.Space = n.Space;
            n2.XmlLang = n.XmlLang;
            n2.CurrentState = n.CurrentState;
            n2.CopyAttributes(n);
            node = n2;
            return n2;
        }

        void Pop()
        {
            if (stack.Count > 1)
            {
                node = (Node)stack.Pop();
            }
        }

        Node Top()
        {
            var top = stack.Count - 1;
            if (top > 0)
            {
                return (Node)stack[top];
            }
            return null;
        }

        public override XmlNodeType NodeType
        {
            get
            {
                if (state == State.Attr)
                {
                    return XmlNodeType.Attribute;
                }
                else if (state == State.AttrValue)
                {
                    return XmlNodeType.Text;
                }
                else if (state == State.EndTag || state == State.AutoClose)
                {
                    return XmlNodeType.EndElement;
                }
                return node.NodeType;
            }
        }

        public override string Name => LocalName;

        public override string LocalName
        {
            get
            {
                string result = null;
                if (state == State.Attr)
                {
                    result = a.Name;
                }
                else if (state == State.AttrValue)
                {
                    result = null;
                }
                else
                {
                    result = node.Name;
                }

                return result;
            }
        }

        public override string NamespaceURI
        {
            get
            {
                // SGML has no namespaces, unless this turned out to be an xmlns attribute.
                if (state == State.Attr && StringUtilities.EqualsIgnoreCase(a.Name, "xmlns"))
                {
                    return "http://www.w3.org/2000/xmlns/";
                }
                return String.Empty;
            }
        }

        public override string Prefix => String.Empty;

        public override bool HasValue
        {
            get
            {
                if (state == State.Attr || state == State.AttrValue)
                {
                    return true;
                }
                return (node.Value != null);
            }
        }

        public override string Value
        {
            get
            {
                if (state == State.Attr || state == State.AttrValue)
                {
                    return a.Value;
                }
                return node.Value;
            }
        }

        public override int Depth
        {
            get
            {
                if (state == State.Attr)
                {
                    return stack.Count;
                }
                else if (state == State.AttrValue)
                {
                    return stack.Count + 1;
                }
                return stack.Count - 1;
            }
        }

        public override string BaseURI => baseUri == null ? "" : baseUri.AbsoluteUri;

        public override bool IsEmptyElement
        {
            get
            {
                if (state == State.Markup || state == State.Attr || state == State.AttrValue)
                {
                    return node.IsEmpty;
                }
                return false;
            }
        }
        public override bool IsDefault
        {
            get
            {
                if (state == State.Attr || state == State.AttrValue)
                    return a.IsDefault;
                return false;
            }
        }
        public override char QuoteChar
        {
            get
            {
                if (a != null) return a.QuoteChar;
                return '\0';
            }
        }

        public override XmlSpace XmlSpace
        {
            get
            {
                for (var i = stack.Count - 1; i > 1; i--)
                {
                    var n = (Node)stack[i];
                    var xs = n.Space;
                    if (xs != XmlSpace.None) return xs;
                }
                return XmlSpace.None;
            }
        }

        public override string XmlLang
        {
            get
            {
                for (var i = stack.Count - 1; i > 1; i--)
                {
                    var n = (Node)stack[i];
                    var xmllang = n.XmlLang;
                    if (xmllang != null) return xmllang;
                }
                return String.Empty;
            }
        }

        public WhitespaceHandling WhitespaceHandling
        {
            get
            {
                return whitespaceHandling;
            }
            set
            {
                whitespaceHandling = value;
            }
        }

        public override int AttributeCount
        {
            get
            {
                if (state == State.Attr || state == State.AttrValue)
                    return 0;
                if (node.NodeType == XmlNodeType.Element ||
                    node.NodeType == XmlNodeType.DocumentType)
                    return node.AttributeCount;
                return 0;
            }
        }

        public override string GetAttribute(string name)
        {
            if (state != State.Attr && state != State.AttrValue)
            {
                var i = node.GetAttribute(name);
                if (i >= 0) return GetAttribute(i);
            }
            return null;
        }

        public override string GetAttribute(string name, string namespaceURI)
        {
            return GetAttribute(name); // SGML has no namespaces.
        }

        public override string GetAttribute(int i)
        {
            if (state != State.Attr && state != State.AttrValue)
            {
                var a = node.GetAttribute(i);
                if (a != null)
                    return a.Value;
            }
            throw new IndexOutOfRangeException();
        }

        public override string this[int i] => GetAttribute(i);

        public override string this[string name] => GetAttribute(name);

        public override string this[string name, string namespaceURI] => GetAttribute(name, namespaceURI);

        public override bool MoveToAttribute(string name)
        {
            var i = node.GetAttribute(name);
            if (i >= 0)
            {
                MoveToAttribute(i);
                return true;
            }
            return false;
        }

        public override bool MoveToAttribute(string name, string ns)
        {
            return MoveToAttribute(name);
        }

        public override void MoveToAttribute(int i)
        {
            var a = node.GetAttribute(i);
            if (a != null)
            {
                apos = i;
                this.a = a;
                if (state != State.Attr)
                {
                    node.CurrentState = state;//save current state.
                }
                state = State.Attr;
                return;
            }
            throw new IndexOutOfRangeException();
        }

        public override bool MoveToFirstAttribute()
        {
            if (node.AttributeCount > 0)
            {
                MoveToAttribute(0);
                return true;
            }
            return false;
        }

        public override bool MoveToNextAttribute()
        {
            if (state != State.Attr && state != State.AttrValue)
            {
                return MoveToFirstAttribute();
            }
            if (apos < node.AttributeCount - 1)
            {
                MoveToAttribute(apos + 1);
                return true;
            }
            return false;
        }

        public override bool MoveToElement()
        {
            if (state == State.Attr || state == State.AttrValue)
            {
                state = node.CurrentState;
                a = null;
                return true;
            }
            return (node.NodeType == XmlNodeType.Element);
        }

        bool IsHtml => isHtml;

        public Encoding GetEncoding()
        {
            if (current == null)
            {
                OpenInput();
            }
            return current.GetEncoding();
        }

        void OpenInput()
        {
            LazyLoadDtd(baseUri);

            if (Href != null)
            {
                current = new Entity("#document", null, href, proxy);
            }
            else if (inputStream != null)
            {
                current = new Entity("#document", null, inputStream, proxy);
            }
            else
            {
                throw new InvalidOperationException("You must specify input either via Href or InputStream properties");
            }
            current.Html = IsHtml;
            current.Open(null, baseUri);
            if (current.ResolvedUri != null)
                baseUri = current.ResolvedUri;

            if (current.Html && dtd == null)
            {
                docType = "HTML";
                LazyLoadDtd(baseUri);
            }
        }

        public override bool Read()
        {
            if (current == null)
            {
                OpenInput();
            }
            var start = state;
            if (node.Simulated)
            {
                // return the next node
                node.Simulated = false;
                node = Top();
                state = node.CurrentState;
                return true;
            }

            var foundnode = false;
            while (!foundnode)
            {
                switch (state)
                {
                    case State.Initial:
                        state = State.Markup;
                        current.ReadChar();
                        goto case State.Markup;
                    case State.Eof:
                        if (current.Parent != null)
                        {
                            current.Close();
                            current = current.Parent;
                        }
                        else
                        {
                            return false;
                        }
                        break;
                    case State.EndTag:
                        if (endTag == (object)node.Name)
                        {
                            Pop(); // we're done!
                            state = State.Markup;
                            goto case State.Markup;
                        }
                        Pop(); // close one element
                        foundnode = true;// return another end element.
                        break;
                    case State.Markup:
                        if (node.IsEmpty)
                        {
                            Pop();
                        }
                        var n = node;
                        foundnode = ParseMarkup();
                        break;
                    case State.PartialTag:
                        Pop(); // remove text node.
                        state = State.Markup;
                        foundnode = ParseTag(partial);
                        break;
                    case State.PseudoStartTag:
                        foundnode = ParseStartTag('<');
                        break;
                    case State.AutoClose:
                        Pop(); // close next node.
                        if (stack.Count <= poptodepth)
                        {
                            state = State.Markup;
                            if (newnode != null)
                            {
                                Push(newnode); // now we're ready to start the new node.
                                newnode = null;
                                state = State.Markup;
                            }
                            else if (node.NodeType == XmlNodeType.Document)
                            {
                                state = State.Eof;
                                goto case State.Eof;
                            }
                        }
                        foundnode = true;
                        break;
                    case State.CData:
                        foundnode = ParseCData();
                        break;
                    case State.Attr:
                        goto case State.AttrValue;
                    case State.AttrValue:
                        state = State.Markup;
                        goto case State.Markup;
                    case State.Text:
                        Pop();
                        goto case State.Markup;
                    case State.PartialText:
                        if (ParseText(current.Lastchar, false))
                        {
                            node.NodeType = XmlNodeType.Whitespace;
                        }
                        foundnode = true;
                        break;
                }
                if (foundnode && node.NodeType == XmlNodeType.Whitespace && whitespaceHandling == WhitespaceHandling.None)
                {
                    // strip out whitespace (caller is probably pretty printing the XML).
                    foundnode = false;
                }
                if (!foundnode && state == State.Eof && stack.Count > 1)
                {
                    poptodepth = 1;
                    state = State.AutoClose;
                    node = Top();
                    return true;
                }
            }
            if (!foundRoot && (NodeType == XmlNodeType.Element ||
                    NodeType == XmlNodeType.Text ||
                    NodeType == XmlNodeType.CDATA))
            {
                foundRoot = true;
                if (IsHtml && (NodeType != XmlNodeType.Element ||
                    string.Compare(LocalName, "html", true, System.Globalization.CultureInfo.InvariantCulture) != 0))
                {
                    // Simulate an HTML root element!
                    node.CurrentState = state;
                    var root = Push("html", XmlNodeType.Element, null);
                    SwapTopNodes(); // make html the outer element.
                    node = root;
                    root.Simulated = true;
                    root.IsEmpty = false;
                    state = State.Markup;
                    //this.state = State.PseudoStartTag;
                    //this.startTag = name;
                }
                return true;
            }
            return true;
        }

        bool ParseMarkup()
        {
            var ch = current.Lastchar;
            if (ch == '<')
            {
                ch = current.ReadChar();
                return ParseTag(ch);
            }
            else if (ch != Entity.EOF)
            {
                if (node.DtdType != null && node.DtdType.ContentModel.DeclaredContent == DeclaredContent.CDATA)
                {
                    // e.g. SCRIPT or STYLE tags which contain unparsed character data.
                    partial = '\0';
                    state = State.CData;
                    return false;
                }
                else if (ParseText(ch, true))
                {
                    node.NodeType = XmlNodeType.Whitespace;
                }
                return true;
            }
            state = State.Eof;
            return false;
        }

        static string declterm = " \t\r\n><";
        bool ParseTag(char ch)
        {
            if (ch == '%')
            {
                return ParseAspNet();
            }
            if (ch == '!')
            {
                ch = current.ReadChar();
                if (ch == '-')
                {
                    return ParseComment();
                }
                else if (ch == '[')
                {
                    return ParseConditionalBlock();
                }
                else if (ch != '_' && !Char.IsLetter(ch))
                {
                    // perhaps it's one of those nasty office document hacks like '<![if ! ie ]>'
                    var value = current.ScanToEnd(sb, "Recovering", ">"); // skip it
                    Log("Ignoring invalid markup '<!" + value + ">");
                    return false;
                }
                else
                {
                    var name = current.ScanToken(sb, declterm, false);
                    if (name == "DOCTYPE")
                    {
                        ParseDocType();
                        // In SGML DOCTYPE SYSTEM attribute is optional, but in XML it is required,
                        // therefore if there is no SYSTEM literal then add an empty one.
                        if (GetAttribute("SYSTEM") == null && GetAttribute("PUBLIC") != null)
                        {
                            node.AddAttribute("SYSTEM", "", '"', folding == CaseFolding.None);
                        }
                        if (stripDocType)
                        {
                            return false;
                        }
                        else
                        {
                            node.NodeType = XmlNodeType.DocumentType;
                            return true;
                        }
                    }
                    else
                    {
                        Log("Invalid declaration '<!{0}...'.  Expecting '<!DOCTYPE' only.", name);
                        current.ScanToEnd(null, "Recovering", ">"); // skip it
                        return false;
                    }
                }
            }
            else if (ch == '?')
            {
                current.ReadChar();// consume the '?' character.
                return ParsePI();
            }
            else if (ch == '/')
            {
                return ParseEndTag();
            }
            else
            {
                return ParseStartTag(ch);
            }
        }

        string ScanName(string terminators)
        {
            var name = current.ScanToken(sb, terminators, false);
            switch (folding)
            {
                case CaseFolding.ToUpper:
                    name = name.ToUpper();
                    break;
                case CaseFolding.ToLower:
                    name = name.ToLower();
                    break;
            }
            return nametable.Add(name);
        }

        static string tagterm = " \t\r\n=/><";
        static string aterm = " \t\r\n='\"/>";
        static string avterm = " \t\r\n>";
        bool ParseStartTag(char ch)
        {
            string name = null;
            if (state != State.PseudoStartTag)
            {
                if (tagterm.IndexOf(ch) >= 0)
                {
                    sb.Length = 0;
                    sb.Append('<');
                    state = State.PartialText;
                    return false;
                }
                name = ScanName(tagterm);
            }
            else
            {
                name = startTag;
                state = State.Markup;
            }
            var n = Push(name, XmlNodeType.Element, null);
            n.IsEmpty = false;
            Validate(n);
            ch = current.SkipWhitespace();
            while (ch != Entity.EOF && ch != '>')
            {
                if (ch == '/')
                {
                    n.IsEmpty = true;
                    ch = current.ReadChar();
                    if (ch != '>')
                    {
                        Log("Expected empty start tag '/>' sequence instead of '{0}'", ch);
                        current.ScanToEnd(null, "Recovering", ">");
                        return false;
                    }
                    break;
                }
                else if (ch == '<')
                {
                    Log("Start tag '{0}' is missing '>'", name);
                    break;
                }
                var aname = ScanName(aterm);
                ch = current.SkipWhitespace();
                if (aname == "," || aname == "=" || aname == ":" || aname == ";")
                {
                    continue;
                }
                string value = null;
                var quote = '\0';
                if (ch == '=' || ch == '"' || ch == '\'')
                {
                    if (ch == '=')
                    {
                        current.ReadChar();
                        ch = current.SkipWhitespace();
                    }
                    if (ch == '\'' || ch == '\"')
                    {
                        quote = ch;
                        value = ScanLiteral(sb, ch);
                    }
                    else if (ch != '>')
                    {
                        var term = avterm;
                        value = current.ScanToken(sb, term, false);
                    }
                }
                if (aname.Length > 0)
                {
                    var a = n.AddAttribute(aname, value, quote, folding == CaseFolding.None);
                    if (a == null)
                    {
                        Log("Duplicate attribute '{0}' ignored", aname);
                    }
                    else
                    {
                        ValidateAttribute(n, a);
                    }
                }
                ch = current.SkipWhitespace();
            }
            if (ch == Entity.EOF)
            {
                current.Error("Unexpected EOF parsing start tag '{0}'", name);
            }
            else if (ch == '>')
            {
                current.ReadChar(); // consume '>'
            }
            if (Depth == 1)
            {
                if (rootCount == 1)
                {
                    // Hmmm, we found another root level tag, soooo, the only
                    // thing we can do to keep this a valid XML document is stop
                    state = State.Eof;
                    return false;
                }
                rootCount++;
            }
            ValidateContent(n);
            return true;
        }

        bool ParseEndTag()
        {
            state = State.EndTag;
            current.ReadChar(); // consume '/' char.
            var name = ScanName(tagterm);
            var ch = current.SkipWhitespace();
            if (ch != '>')
            {
                Log("Expected empty start tag '/>' sequence instead of '{0}'", ch);
                current.ScanToEnd(null, "Recovering", ">");
            }
            current.ReadChar(); // consume '>'

            endTag = name;
            // Make sure there's a matching start tag for it.                        
            var caseInsensitive = (folding == CaseFolding.None);
            node = (Node)stack[stack.Count - 1];
            for (var i = stack.Count - 1; i > 0; i--)
            {
                var n = (Node)stack[i];
                if (caseInsensitive && string.Compare(n.Name, name, true) == 0)
                {
                    endTag = n.Name;
                    return true;
                }
                else if ((object)n.Name == (object)name)
                {
                    return true;
                }
            }
            Log("No matching start tag for '</{0}>'", name);
            state = State.Markup;
            return false;
        }

        bool ParseAspNet()
        {
            var value = "<%" + current.ScanToEnd(sb, "AspNet", "%>") + "%>";
            Push(null, XmlNodeType.CDATA, value);
            return true;
        }

        bool ParseComment()
        {
            var ch = current.ReadChar();
            if (ch != '-')
            {
                Log("Expecting comment '<!--' but found {0}", ch);
                current.ScanToEnd(null, "Comment", ">");
                return false;
            }
            var value = current.ScanToEnd(sb, "Comment", "-->");

            // Make sure it's a valid comment!
            var i = value.IndexOf("--");
            while (i >= 0)
            {
                var j = i + 2;
                while (j < value.Length && value[j] == '-')
                    j++;
                if (i > 0)
                {
                    value = value.Substring(0, i - 1) + "-" + value.Substring(j);
                }
                else
                {
                    value = "-" + value.Substring(j);
                }
                i = value.IndexOf("--");
            }
            if (value.Length > 0 && value[value.Length - 1] == '-')
            {
                value += " "; // '-' cannot be last character
            }
            Push(null, XmlNodeType.Comment, value);
            return true;
        }

        static string cdataterm = "\t\r\n[<>";
        bool ParseConditionalBlock()
        {
            var ch = current.ReadChar(); // skip '['
            ch = current.SkipWhitespace();
            var name = current.ScanToken(sb, cdataterm, false);
            if (name != "CDATA")
            {
                Log("Expecting CDATA but found '{0}'", name);
                current.ScanToEnd(null, "CDATA", ">");
                return false;
            }
            ch = current.SkipWhitespace();
            if (ch != '[')
            {
                Log("Expecting '[' but found '{0}'", ch);
                current.ScanToEnd(null, "CDATA", ">");
                return false;
            }
            var value = current.ScanToEnd(sb, "CDATA", "]]>");

            Push(null, XmlNodeType.CDATA, value);
            return true;
        }

        static string dtterm = " \t\r\n>";
        void ParseDocType()
        {
            var ch = current.SkipWhitespace();
            var name = ScanName(dtterm);
            Push(name, XmlNodeType.DocumentType, null);
            ch = current.SkipWhitespace();
            if (ch != '>')
            {
                var subset = string.Empty;
                var pubid = string.Empty;
                var syslit = string.Empty;

                if (ch != '[')
                {
                    var token = current.ScanToken(sb, dtterm, false);
                    if (token == "PUBLIC")
                    {
                        ch = current.SkipWhitespace();
                        if (ch == '\"' || ch == '\'')
                        {
                            pubid = current.ScanLiteral(sb, ch);
                            node.AddAttribute(token, pubid, ch, folding == CaseFolding.None);
                        }
                    }
                    else if (token != "SYSTEM")
                    {
                        Log("Unexpected token in DOCTYPE '{0}'", token);
                        current.ScanToEnd(null, "DOCTYPE", ">");
                    }
                    ch = current.SkipWhitespace();
                    if (ch == '\"' || ch == '\'')
                    {
                        token = nametable.Add("SYSTEM");
                        syslit = current.ScanLiteral(sb, ch);
                        node.AddAttribute(token, syslit, ch, folding == CaseFolding.None);
                    }
                    ch = current.SkipWhitespace();
                }
                if (ch == '[')
                {
                    subset = current.ScanToEnd(sb, "Internal Subset", "]");
                    node.Value = subset;
                }
                ch = current.SkipWhitespace();
                if (ch != '>')
                {
                    Log("Expecting end of DOCTYPE tag, but found '{0}'", ch);
                    current.ScanToEnd(null, "DOCTYPE", ">");
                }

                if (dtd == null)
                {
                    docType = name;
                    this.pubid = pubid;
                    this.syslit = syslit;
                    this.subset = subset;
                    LazyLoadDtd(current.ResolvedUri);
                }
            }
            current.ReadChar();
        }

        static string piterm = " \t\r\n?";
        bool ParsePI()
        {
            var name = current.ScanToken(sb, piterm, false);
            string value = null;
            if (current.Lastchar != '?')
            {
                // Notice this is not "?>".  This is because Office generates bogus PI's that end with "/>".
                value = current.ScanToEnd(sb, "Processing Instruction", ">");
            }
            else
            {
                // error recovery.
                value = current.ScanToEnd(sb, "Processing Instruction", ">");
            }
            // skip xml declarations, since these are generated in the output instead.
            if (name != "xml")
            {
                Push(nametable.Add(name), XmlNodeType.ProcessingInstruction, value);
                return true;
            }
            return false;
        }

        bool ParseText(char ch, bool newtext)
        {
            var ws = !newtext || current.IsWhitespace;
            if (newtext) sb.Length = 0;
            //this.sb.Append(ch);
            //ch = this.current.ReadChar();
            state = State.Text;
            while (ch != Entity.EOF)
            {
                if (ch == '<')
                {
                    ch = current.ReadChar();
                    if (ch == '/' || ch == '!' || ch == '?' || Char.IsLetter(ch))
                    {
                        // Hit a tag, so return XmlNodeType.Text token
                        // and remember we partially started a new tag.
                        state = State.PartialTag;
                        partial = ch;
                        break;
                    }
                    else
                    {
                        // not a tag, so just proceed.
                        sb.Append('<');
                        sb.Append(ch);
                        ws = false;
                        ch = current.ReadChar();
                    }
                }
                else if (ch == '&')
                {
                    ExpandEntity(sb, '<');
                    ws = false;
                    ch = current.Lastchar;
                }
                else
                {
                    if (!current.IsWhitespace) ws = false;
                    sb.Append(ch);
                    ch = current.ReadChar();
                }
            }
            var value = sb.ToString();
            Push(null, XmlNodeType.Text, value);
            return ws;
        }

        // This version is slightly different from Entity.ScanLiteral in that
        // it also expands entities.
        public string ScanLiteral(StringBuilder sb, char quote)
        {
            sb.Length = 0;
            var ch = current.ReadChar();
            while (ch != Entity.EOF && ch != quote)
            {
                if (ch == '&')
                {
                    ExpandEntity(this.sb, quote);
                    ch = current.Lastchar;
                }
                else
                {
                    sb.Append(ch);
                    ch = current.ReadChar();
                }
            }
            current.ReadChar(); // consume end quote.          
            return sb.ToString();
        }

        bool ParseCData()
        {
            // Like ParseText(), only it doesn't allow elements in the content.  
            // It allows comments and processing instructions and text only and
            // text is not returned as text but CDATA (since it may contain angle brackets).
            // And initial whitespace is ignored.  It terminates when we hit the
            // end tag for the current CDATA node (e.g. </style>).
            var ws = current.IsWhitespace;
            sb.Length = 0;
            var ch = current.Lastchar;
            if (partial != '\0')
            {
                Pop(); // pop the CDATA
                switch (partial)
                {
                    case '!':
                        partial = ' '; // and pop the comment next time around
                        return ParseComment();
                    case '?':
                        partial = ' '; // and pop the PI next time around
                        return ParsePI();
                    case '/':
                        state = State.EndTag;
                        return true;    // we are done!
                    case ' ':
                        break; // means we just needed to pop the Comment, PI or CDATA.
                }
            }
            else
            {
                ch = current.ReadChar();
            }

            // if this.partial == '!' then parse the comment and return
            // if this.partial == '?' then parse the processing instruction and return.            
            while (ch != Entity.EOF)
            {
                if (ch == '<')
                {
                    ch = current.ReadChar();
                    if (ch == '!')
                    {
                        ch = current.ReadChar();
                        if (ch == '-')
                        {
                            // return what CDATA we have accumulated so far
                            // then parse the comment and return to here.
                            if (ws)
                            {
                                partial = ' '; // pop comment next time through
                                return ParseComment();
                            }
                            else
                            {
                                // return what we've accumulated so far then come
                                // back in and parse the comment.
                                partial = '!';
                                break;
                            }
#if FIX
                        } else if (ch == '['){
                            // We are about to wrap this node as a CDATA block because of it's
                            // type in the DTD, but since we found a CDATA block in the input
                            // we have to parse it as a CDATA block, otherwise we will attempt
                            // to output nested CDATA blocks which of course is illegal.
                            if (this.ParseConditionalBlock()){
                                this.partial = ' ';
                                return true;
                            }
#endif
                        }
                        else
                        {
                            // not a comment, so ignore it and continue on.
                            sb.Append('<');
                            sb.Append('!');
                            sb.Append(ch);
                            ws = false;
                        }
                    }
                    else if (ch == '?')
                    {
                        // processing instruction.
                        current.ReadChar();// consume the '?' character.
                        if (ws)
                        {
                            partial = ' '; // pop PI next time through
                            return ParsePI();
                        }
                        else
                        {
                            partial = '?';
                            break;
                        }
                    }
                    else if (ch == '/')
                    {
                        // see if this is the end tag for this CDATA node.
                        var temp = sb.ToString();
                        if (ParseEndTag() && endTag == (object)node.Name)
                        {
                            if (ws || temp == string.Empty)
                            {
                                // we are done!
                                return true;
                            }
                            else
                            {
                                // return CDATA text then the end tag
                                partial = '/';
                                sb.Length = 0; // restore buffer!
                                sb.Append(temp);
                                state = State.CData;
                                break;
                            }
                        }
                        else
                        {
                            // wrong end tag, so continue on.
                            sb.Length = 0; // restore buffer!
                            sb.Append(temp);
                            sb.Append("</" + endTag + ">");
                            ws = false;
                        }
                    }
                    else
                    {
                        // must be just part of the CDATA block, so proceed.
                        sb.Append('<');
                        sb.Append(ch);
                        ws = false;
                    }
                }
                else
                {
                    if (!current.IsWhitespace && ws) ws = false;
                    sb.Append(ch);
                }
                ch = current.ReadChar();
            }
            var value = sb.ToString();
            Push(null, XmlNodeType.CDATA, value);
            if (partial == '\0')
                partial = ' ';// force it to pop this CDATA next time in.
            return true;
        }

        void ExpandEntity(StringBuilder sb, char terminator)
        {
            var ch = current.ReadChar();
            if (ch == '#')
            {
                var charent = current.ExpandCharEntity();
                sb.Append(charent);
                ch = current.Lastchar;
            }
            else
            {
                this.name.Length = 0;
                while (ch != Entity.EOF &&
                    (Char.IsLetter(ch) || ch == '_' || ch == '-'))
                {
                    this.name.Append(ch);
                    ch = current.ReadChar();
                }
                var name = this.name.ToString();
                if (dtd != null && name != string.Empty)
                {
                    var e = (Entity)dtd.FindEntity(name);
                    if (e != null)
                    {
                        if (e.Internal)
                        {
                            sb.Append(e.Literal);
                            if (ch != terminator)
                                ch = current.ReadChar();
                            return;
                        }
                        else
                        {
                            var ex = new Entity(name, e.PublicId, e.Uri, current.Proxy);
                            e.Open(current, new Uri(e.Uri));
                            current = ex;
                            current.ReadChar();
                            return;
                        }
                    }
                    else
                    {
                        Log("Undefined entity '{0}'", name);
                    }
                }
                // Entity is not defined, so just keep it in with the rest of the
                // text.
                sb.Append("&");
                sb.Append(name);
                if (ch != terminator)
                {
                    sb.Append(ch);
                    ch = current.ReadChar();
                }
            }
        }

        public override bool EOF => state == State.Eof;

        public override void Close()
        {
            if (current != null)
            {
                current.Close();
                current = null;
            }
            if (log != null)
            {
                log.Close();
                log = null;
            }
        }

        public override ReadState ReadState
        {
            get
            {
                if (state == State.Initial) return ReadState.Initial;
                else if (state == State.Eof) return ReadState.EndOfFile;
                return ReadState.Interactive;
            }
        }

        public override string ReadString()
        {
            if (node.NodeType == XmlNodeType.Element)
            {
                sb.Length = 0;
                while (Read())
                {
                    switch (NodeType)
                    {
                        case XmlNodeType.CDATA:
                        case XmlNodeType.SignificantWhitespace:
                        case XmlNodeType.Whitespace:
                        case XmlNodeType.Text:
                            sb.Append(node.Value);
                            break;
                        default:
                            return sb.ToString();
                    }
                }
                return sb.ToString();
            }
            return node.Value;
        }


        public override string ReadInnerXml()
        {
            var sw = new StringWriter();
            var xw = new XmlTextWriter(sw);
            xw.Formatting = Formatting.Indented;
            switch (NodeType)
            {
                case XmlNodeType.Element:
                    Read();
                    while (!EOF && NodeType != XmlNodeType.EndElement)
                    {
                        xw.WriteNode(this, true);
                    }
                    Read(); // consume the end tag
                    break;
                case XmlNodeType.Attribute:
                    sw.Write(Value);
                    break;
                default:
                    // return empty string according to XmlReader spec.
                    break;
            }
            xw.Close();
            return sw.ToString();
        }

        public override string ReadOuterXml()
        {
            var sw = new StringWriter();
            var xw = new XmlTextWriter(sw);
            xw.Formatting = Formatting.Indented;
            xw.WriteNode(this, true);
            xw.Close();
            return sw.ToString();
        }

        public override XmlNameTable NameTable => nametable;

        public override string LookupNamespace(string prefix)
        {
            return null;// there are no namespaces in SGML.
        }

        public override void ResolveEntity()
        {
            // We never return any entity reference nodes, so this should never be called.
            throw new InvalidOperationException("Not on an entity reference.");
        }

        public override bool ReadAttributeValue()
        {
            if (state == State.Attr)
            {
                state = State.AttrValue;
                return true;
            }
            else if (state == State.AttrValue)
            {
                return false;
            }
            throw new InvalidOperationException("Not on an attribute.");
        }

        void Validate(Node node)
        {
            if (dtd != null)
            {
                var e = dtd.FindElement(node.Name);
                if (e != null)
                {
                    node.DtdType = e;
                    if (e.ContentModel.DeclaredContent == DeclaredContent.EMPTY)
                        node.IsEmpty = true;
                }
            }
        }

        void ValidateAttribute(Node node, Attribute a)
        {
            var e = node.DtdType;
            if (e != null)
            {
                var ad = e.FindAttribute(a.Name);
                if (ad != null)
                {
                    a.DtdType = ad;
                }
            }
        }

        void ValidateContent(Node node)
        {
            if (dtd != null)
            {
                // See if this element is allowed inside the current element.
                // If it isn't, then auto-close elements until we find one
                // that it is allowed to be in.                                  
                var name = nametable.Add(node.Name.ToUpper()); // DTD is in upper case
                var i = 0;
                var top = stack.Count - 2;
                if (node.DtdType != null)
                {
                    // it is a known element, let's see if it's allowed in the
                    // current context.
                    for (i = top; i > 0; i--)
                    {
                        var n = (Node)stack[i];
                        if (n.IsEmpty)
                            continue; // we'll have to pop this one
                        var f = n.DtdType;
                        if (f != null)
                        {
                            if (f.Name == dtd.Name)
                                break; // can't pop the root element.
                            if (f.CanContain(name, dtd))
                            {
                                break;
                            }
                            else if (!f.EndTagOptional)
                            {
                                // If the end tag is not optional then we can't
                                // auto-close it.  We'll just have to live with the
                                // junk we've found and move on.
                                break;
                            }
                        }
                        else
                        {
                            // Since we don't understand this tag anyway,
                            // we might as well allow this content!
                            break;
                        }
                    }
                }
                if (i == 0)
                {
                    // Tag was not found or is not allowed anywhere, ignore it and 
                    // continue on.
                }
                else if (i < top)
                {
                    var n = (Node)stack[top];
                    if (i == top - 1 && name == n.Name)
                    {
                        // e.g. p not allowed inside p, not an interesting error.
                    }
                    else
                    {
                        var closing = string.Empty;
                        for (var k = top; k >= i + 1; k--)
                        {
                            if (closing != string.Empty) closing += ",";
                            var n2 = (Node)stack[k];
                            closing += "<" + n2.Name + ">";
                        }
                        Log("Element '{0}' not allowed inside '{1}', closing {2}.",
                            name, n.Name, closing);
                    }
                    state = State.AutoClose;
                    newnode = node;
                    Pop(); // save this new node until we pop the others
                    poptodepth = i + 1;
                }
            }
        }
    }
}
