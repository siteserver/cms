// ***************************************************************
// <copyright file="CommonTypes.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters
{
    internal enum TextMapping : byte
    {
        Unicode = 0,
        Symbol,
        Wingdings,
        OtherSymbol
    }

    internal struct RecognizeInterestingFontName
    {
        
        
        private static byte[] CharMapToClass = new byte[]
        {
          
          0,   0,   0,   0,   0,   0,   0,   0,   0,   1,   1,   0,   0,   1,   0,   0,
          0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,
          1,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,
          
          0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   2,   0,   0,   0,   0,
          
          0,   0,  11,   0,   7,   0,   0,   6,   0,   4,   0,   0,  13,  10,   5,  12,
          
          0,   0,   0,   8,   0,   0,   0,   3,   0,   9,   0,   0,   0,   0,   0,   0,
          
          0,   0,  11,   0,   7,   0,   0,   6,   0,   4,   0,   0,  13,  10,   5,  12,
          
          0,   0,   0,   8,   0,   0,   0,   3,   0,   9,   0,   0,   0,   0,   0,   0,
        };

        private static sbyte[,] StateTransitionTable = new sbyte[,]
        {
        
        
          { -1,   0,  -1,   3,  -1,  -1,  -1,  -1,  11,  -1,  -1,  -1,  -1,  -1 },  

          { -1,   1,   1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1 },  
          { -1,   2,   2,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1 },  

          { -1,  -1,  -1,  -1,   4,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1 },  
          { -1,  -1,  -1,  -1,  -1,   5,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1 },  
          { -1,  -1,  -1,  -1,  -1,  -1,   6,  -1,  -1,  -1,  -1,  -1,  -1,  -1 },  
          { -1,  -1,  -1,  -1,  -1,  -1,  -1,   7,  -1,  -1,  -1,  -1,  -1,  -1 },  
          { -1,  -1,  -1,  -1,   8,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1 },  
          { -1,  -1,  -1,  -1,  -1,   9,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1 },  
          { -1,  -1,  -1,  -1,  -1,  -1,  10,  -1,  -1,  -1,  -1,  -1,  -1,  -1 },  
          { -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,   2,  -1,  -1,  -1,  -1,  -1 },  

          { -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  12,  -1,  -1,  -1,  -1 },  
          { -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  13,  -1,  -1,  -1 },  
          { -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  14,  -1,  -1 },  
          { -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  15,  -1 },  
          { -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,   1 },  
        };

        private sbyte state;

        public TextMapping TextMapping
        {
            get
            {
                switch (state)
                {
                    case 1: return TextMapping.Symbol;
                    case 2: return TextMapping.Wingdings;
                }
                return TextMapping.Unicode;
            }
        }

        public bool IsRejected => state < 0;

        public void AddCharacter(byte ch)
        {
            if (state >= 0)
            {
                state = StateTransitionTable[state, ch > 0x7F ? 0 : (int)CharMapToClass[ch]];
            }
        }

        public void AddCharacter(char ch)
        {
            if (state >= 0)
            {
                state = StateTransitionTable[state, ch > 0x7F ? 0 : (int)CharMapToClass[(int)ch]];
            }
        }
    }

    internal struct RecognizeInterestingFontNameInInlineStyle
    {
        
        
        private static byte[] CharMapToClass = new byte[]
        {
          
          0,   0,   0,   0,   0,   0,   0,   0,   0,   1,   1,   0,   0,   1,   0,   0,
          0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,
          
          1,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,  17,   0,   0,
          
          0,   0,   0,   0,   0,   0,   0,   0,   0,   0,  14,   2,   0,   0,   0,   0,
          
          0,  18,  11,   0,   7,   0,  15,   6,   0,   4,   0,   0,  13,  10,   5,  12,
          
          0,   0,   0,   8,  16,   0,   0,   3,   0,   9,   0,   0,   0,   0,   0,   0,
          
          0,  18,  11,   0,   7,   0,  15,   6,   0,   4,   0,   0,  13,  10,   5,  12,
          
          0,   0,   0,   8,  16,   0,   0,   3,   0,   9,   0,   0,   0,   0,   0,   0,
        };

        private static sbyte[,] StateTransitionTable = new sbyte[,]
        {
        
        
          {  1,   0,   0,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   2,   1,   1,   1 },  
          {  1,   1,   0,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1 },  

          {  1,   1,   0,   1,   1,   1,   1,   1,   1,   1,   1,   1,   3,   1,   1,   1,   1,   1,   1 },  
          {  1,   1,   0,   1,   1,   4,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1 },  
          {  1,   1,   0,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   5,   1,   1 },  
          {  1,   1,   0,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   6,   1 },  
          {  1,   1,   0,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   7,   1,   1,   1 },  
          {  1,   1,   0,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   8 },  
          {  1,   1,   0,   1,   1,   1,   1,   1,   1,   1,   9,   1,   1,   1,   1,   1,   1,   1,   1 },  
          {  1,   1,   0,   1,  10,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1 },  
          {  1,   1,   0,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,  11,   1,   1,   1,   1,   1 },  
          {  1,   1,   0,   1,   1,   1,   1,   1,   1,  12,   1,   1,   1,   1,   1,   1,   1,   1,   1 },  

          {  1,  12,  -1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,  13,   1,   1,   1,   1 },  

          { -1,  13,  -1,  14,  -1,  -1,  -1,  -1,  23,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1 },  

          { -1,  -1,  -1,  -1,  15,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1 },  
          { -1,  -1,  -1,  -1,  -1,  16,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1 },  
          { -1,  -1,  -1,  -1,  -1,  -1,  17,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1 },  
          { -1,  -1,  -1,  -1,  -1,  -1,  -1,  18,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1 },  
          { -1,  -1,  -1,  -1,  19,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1 },  
          { -1,  -1,  -1,  -1,  -1,  20,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1 },  
          { -1,  -1,  -1,  -1,  -1,  -1,  21,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1 },  
          { -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  22,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1 },  
          { -1,  22,  -2,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1 },  

          { -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  24,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1 },  
          { -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  25,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1 },  
          { -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  26,  -1,  -1,  -1,  -1,  -1,  -1,  -1 },  
          { -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  27,  -1,  -1,  -1,  -1,  -1,  -1 },  
          { -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  28,  -1,  -1,  -1,  -1,  -1 },  
          { -1,  28,  -3,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1,  -1 },  
        };

        private sbyte state;

        public TextMapping TextMapping
        {
            get
            {
                switch (state)
                {
                    case -3:
                    case 28:
                            return TextMapping.Symbol;
                    case -2:
                    case 22:
                            return TextMapping.Wingdings;
                }
                return TextMapping.Unicode;
            }
        }

        public bool IsFinished => state < 0;

        public void AddCharacter(char ch)
        {
            if (state >= 0)
            {
                state = StateTransitionTable[state, ch > 0x7F ? 0 : (int)CharMapToClass[(int)ch]];
            }
        }
    }

    
    internal interface ITextSink
    {
        bool IsEnough { get; }

        void Write(char[] buffer, int offset, int count);
        void Write(int ucs32Char);
    }

    
    internal interface ITextSinkEx : ITextSink
    {
        void Write(string value);
        void WriteNewLine();
    }

    
    internal interface IProducerConsumer
    {
        void Run();         
        bool Flush();       
    }

    
    internal interface IRestartable
    {
        bool CanRestart();
        void Restart();
        void DisableRestart();
    }

    
    internal interface IReusable
    {
        void Initialize(object newSourceOrDestination);
    }

    
    internal interface IByteSource
    {
        bool GetOutputChunk(out byte[] chunkBuffer, out int chunkOffset, out int chunkLength);
        void ReportOutput(int readCount);
    }

    
    internal interface IProgressMonitor
    {
        void ReportProgress();
    }
}

