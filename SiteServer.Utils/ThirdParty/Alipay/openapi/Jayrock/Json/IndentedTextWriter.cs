#region License, Terms and Conditions
//
// Jayrock - JSON and JSON-RPC for Microsoft .NET Framework and Mono
// Written by Atif Aziz (atif.aziz@skybow.com)
// Copyright (c) 2005 Atif Aziz. All rights reserved.
//
// This library is free software; you can redistribute it and/or modify it under
// the terms of the GNU Lesser General Public License as published by the Free
// Software Foundation; either version 2.1 of the License, or (at your option)
// any later version.
//
// This library is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
// details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this library; if not, write to the Free Software Foundation, Inc.,
// 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 
//
#endregion

namespace Jayrock
{
    #region Imports

    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;

    #endregion

    /// <summary>
    /// Drop-in replacement for <see cref="System.CodeDom.Compiler.IndentedTextWriter"/>
    /// that does not require a full-trust link and inheritance demand.
    /// </summary>

    public sealed class IndentedTextWriter : TextWriter
    {
        private TextWriter _writer;
        private int _level;
        private bool _tabsPending;
        private string _tab;

        public const string DefaultTabString = "\x20\x20\x20\x20";

        public IndentedTextWriter(TextWriter writer) : 
            this(writer, DefaultTabString) {}

        public IndentedTextWriter(TextWriter writer, string tabString) : 
            base(CultureInfo.InvariantCulture)
        {
            _writer = writer;
            _tab = tabString;
            _level = 0;
            _tabsPending = false;
        }

        public override Encoding Encoding
        {
            get { return _writer.Encoding; }
        }

        public override string NewLine
        {
            get { return _writer.NewLine; }

            set { _writer.NewLine = value; }
        }

        public int Indent
        {
            get { return _level; }
            set { _level = value < 0 ? 0 : value; }
        }

        public TextWriter InnerWriter
        {
            get { return _writer; }
        }

        internal string TabString
        {
            get { return _tab; }
        }

        public override void Close()
        {
            _writer.Close();
        }

        public override void Flush()
        {
            _writer.Flush();
        }

        public override void Write(string s)
        {
            WritePendingTabs();
            _writer.Write(s);
        }

        public override void Write(bool value)
        {
            WritePendingTabs();
            _writer.Write(value);
        }

        public override void Write(char value)
        {
            WritePendingTabs();
            _writer.Write(value);
        }

        public override void Write(char[] buffer)
        {
            WritePendingTabs();
            _writer.Write(buffer);
        }

        public override void Write(char[] buffer, int index, int count)
        {
            WritePendingTabs();
            _writer.Write(buffer, index, count);
        }

        public override void Write(double value)
        {
            WritePendingTabs();
            _writer.Write(value);
        }

        public override void Write(float value)
        {
            WritePendingTabs();
            _writer.Write(value);
        }

        public override void Write(int value)
        {
            WritePendingTabs();
            _writer.Write(value);
        }

        public override void Write(long value)
        {
            WritePendingTabs();
            _writer.Write(value);
        }

        public override void Write(object value)
        {
            WritePendingTabs();
            _writer.Write(value);
        }

        public override void Write(string format, object arg0)
        {
            WritePendingTabs();
            _writer.Write(format, arg0);
        }

        public override void Write(string format, object arg0, object arg1)
        {
            WritePendingTabs();
            _writer.Write(format, arg0, arg1);
        }

        public override void Write(string format, params object[] arg)
        {
            WritePendingTabs();
            _writer.Write(format, arg);
        }

        public void WriteLineNoTabs(string s)
        {
            _writer.WriteLine(s);
        }

        public override void WriteLine(string s)
        {
            WritePendingTabs();
            _writer.WriteLine(s);
            _tabsPending = true;
        }

        public override void WriteLine()
        {
            WritePendingTabs();
            _writer.WriteLine();
            _tabsPending = true;
        }

        public override void WriteLine(bool value)
        {
            WritePendingTabs();
            _writer.WriteLine(value);
            _tabsPending = true;
        }

        public override void WriteLine(char value)
        {
            WritePendingTabs();
            _writer.WriteLine(value);
            _tabsPending = true;
        }

        public override void WriteLine(char[] buffer)
        {
            WritePendingTabs();
            _writer.WriteLine(buffer);
            _tabsPending = true;
        }

        public override void WriteLine(char[] buffer, int index, int count)
        {
            WritePendingTabs();
            _writer.WriteLine(buffer, index, count);
            _tabsPending = true;
        }

        public override void WriteLine(double value)
        {
            WritePendingTabs();
            _writer.WriteLine(value);
            _tabsPending = true;
        }

        public override void WriteLine(float value)
        {
            WritePendingTabs();
            _writer.WriteLine(value);
            _tabsPending = true;
        }

        public override void WriteLine(int value)
        {
            WritePendingTabs();
            _writer.WriteLine(value);
            _tabsPending = true;
        }

        public override void WriteLine(long value)
        {
            WritePendingTabs();
            _writer.WriteLine(value);
            _tabsPending = true;
        }

        public override void WriteLine(object value)
        {
            WritePendingTabs();
            _writer.WriteLine(value);
            _tabsPending = true;
        }

        public override void WriteLine(string format, object arg0)
        {
            WritePendingTabs();
            _writer.WriteLine(format, arg0);
            _tabsPending = true;
        }

        public override void WriteLine(string format, object arg0, object arg1)
        {
            WritePendingTabs();
            _writer.WriteLine(format, arg0, arg1);
            _tabsPending = true;
        }

        public override void WriteLine(string format, params object[] arg)
        {
            WritePendingTabs();
            _writer.WriteLine(format, arg);
            _tabsPending = true;
        }

        private void WritePendingTabs()
        {
            if (!_tabsPending)
                return;

            _tabsPending = false;

            for (int i = 0; i < _level; i++)
                _writer.Write(_tab);
        }
    }
}
