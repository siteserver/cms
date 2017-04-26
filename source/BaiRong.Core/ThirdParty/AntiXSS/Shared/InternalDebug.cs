// ***************************************************************
// <copyright file="InternalDebug.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.Internal
{
    using System;
    using System.Diagnostics;

    
    
    
    internal static class InternalDebug
    {
        internal static bool useSystemDiagnostics = false;

        
        
        internal static bool UseSystemDiagnostics
        {
            get { return useSystemDiagnostics; }
            set { useSystemDiagnostics = value; }
        }

        
        
        
        
        
        
        [Conditional("DEBUG")]
        public static void Trace(long traceType, string format, params object[] traceObjects)
        {
#if DEBUG
            if (useSystemDiagnostics)
            {
                
                
                
            }
#endif
        }

        
        
        
        
        
        [Conditional("DEBUG")]
        public static void Assert(bool condition, string formatString)
        {
#if DEBUG
            if (!useSystemDiagnostics)
            {
                
                
                if (!condition)
                {
                    throw new DebugAssertionViolationException(formatString);
                }
            }
            else
            {
                Debug.Assert(condition, formatString);
            }
#endif
        }

        
        
        
        
        [Conditional("DEBUG")]
        public static void Assert(bool condition)
        {
#if DEBUG
            if (!useSystemDiagnostics)
            {
                
                
                if (!condition)
                {
                    throw new DebugAssertionViolationException("Assertion failed");
                }
            }
            else
            {
                Debug.Assert(condition);
            }
#endif
        }

        
        
        
        internal class DebugAssertionViolationException : Exception
        {
            public DebugAssertionViolationException() : base()
            {
            }

            public DebugAssertionViolationException(string message) : base(message)
            {
            }
        }
    }
}
