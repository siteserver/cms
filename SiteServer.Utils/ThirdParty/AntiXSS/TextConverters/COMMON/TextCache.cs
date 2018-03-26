// ***************************************************************
// <copyright file="TextCache.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters
{
    using System;
    using Data.Internal;


    internal class TextCache
    {
        
        private int cachedLength;

        private CacheEntry headEntry;
        private CacheEntry tailEntry;
        private CacheEntry freeList;

        
        
        public TextCache()
        {
        }

        
        
        public int Length => cachedLength;


        public void Reset()
        {
            while (headEntry != null)
            {
                headEntry.Reset();

                

                var newFree = headEntry;

                headEntry = headEntry.Next;

                if (headEntry == null)
                {
                    tailEntry = null;
                }

                

                newFree.Next = freeList;
                freeList = newFree;
            }

            cachedLength = 0;
        }

        
        
        
        
        
        
        public void GetBuffer(int size, out char[] buffer, out int offset, out int realSize)
        {
            if (tailEntry != null)
            {
                if (tailEntry.GetBuffer(size, out buffer, out offset, out realSize))
                {
                    return;
                }

                
            }

            

            AllocateTail(size);

            InternalDebug.Assert(tailEntry != null);

            var success = tailEntry.GetBuffer(size, out buffer, out offset, out realSize);

            InternalDebug.Assert(success);
        }

        
        
        
        public void Commit(int count)
        {
            InternalDebug.Assert(tailEntry != null);

            tailEntry.Commit(count);

            cachedLength += count;
        }

        
        
        
        
        
        public void GetData(out char[] outputBuffer, out int outputOffset, out int outputCount)
        {
            InternalDebug.Assert(headEntry != null);

            headEntry.GetData(out outputBuffer, out outputOffset, out outputCount);
        }

        
        
        
        public void ReportRead(int count)
        {
            InternalDebug.Assert(headEntry != null);

            headEntry.ReportRead(count);

            cachedLength -= count;

            if (0 == headEntry.Length)
            {
                

                var newFree = headEntry;

                headEntry = headEntry.Next;

                if (headEntry == null)
                {
                    InternalDebug.Assert(cachedLength == 0);

                    tailEntry = null;
                }

                newFree.Next = freeList;
                freeList = newFree;
            }
        }

        
        
        
        
        
        
        public int Read(char[] buffer, int offset, int count)
        {
            var countCopiedTotal = 0;

            while (0 != count)
            {
                var countCopied = headEntry.Read(buffer, offset, count);

                offset += countCopied;
                count -= countCopied;

                countCopiedTotal += countCopied;

                cachedLength -= countCopied;

                if (0 == headEntry.Length)
                {
                    

                    var newFree = headEntry;

                    headEntry = headEntry.Next;

                    if (headEntry == null)
                    {
                        InternalDebug.Assert(cachedLength == 0);

                        tailEntry = null;
                    }

                    newFree.Next = freeList;
                    freeList = newFree;
                }

                if (0 == count || headEntry == null)
                {
                    break;
                }

                

                InternalDebug.Assert(0 != cachedLength && tailEntry != null);
            }

            return countCopiedTotal;
        }

        
        
        
        private void AllocateTail(int size)
        {
            var newEntry = freeList;
            if (newEntry != null)
            {
                freeList = newEntry.Next;
                newEntry.Next = null;
            }
            else
            {
                newEntry = new CacheEntry(size);
            }

            InternalDebug.Assert(newEntry.Length == 0);

            if (tailEntry != null)
            {
                tailEntry.Next = newEntry;
            }
            else
            {
                InternalDebug.Assert(headEntry == null);
                headEntry = newEntry;
            }

            tailEntry = newEntry;
        }

        
        
        internal class CacheEntry
        {
            private const int DefaultMaxLength = 4096;

            private char[] buffer;
            private int count;
            private int offset;

            private CacheEntry next;

            
            
            
            public CacheEntry(int size)
            {
                AllocateBuffer(size);
            }

            
            
            public int Length => count;


            public CacheEntry Next
            {
                get { return next; }
                set { next = value; }
            }
        
            
            
            public void Reset()
            {
                count = 0;
            }

            
            
            
            
            
            
            
            public bool GetBuffer(int size, out char[] buffer, out int offset, out int realSize)
            {
                if (count == 0)
                {
                    this.offset = 0;

                    if (this.buffer.Length < size)
                    {
                        
                        

                        AllocateBuffer(size);
                    }
                }

                if (this.buffer.Length - (this.offset + count) >= size)
                {
                    buffer = this.buffer;
                    offset = this.offset + count;
                    realSize = this.buffer.Length - offset;
                    return true;
                }

                InternalDebug.Assert(count != 0);

                if (count < 64 && this.buffer.Length - count >= size)
                {
                    
                    
                    
                    Buffer.BlockCopy(this.buffer, this.offset * 2, this.buffer, 0, count * 2);
                    this.offset = 0;

                    buffer = this.buffer;
                    offset = this.offset + count;
                    realSize = this.buffer.Length - offset;
                    return true;
                }

                

                buffer = null;
                offset = 0;
                realSize = 0;
                return false;
            }

            
            
            
            public void Commit(int count)
            {
                InternalDebug.Assert(buffer.Length - (offset + this.count) >= count);

                this.count += count;
            }

            
            
            
            
            
            public void GetData(out char[] outputBuffer, out int outputOffset, out int outputCount)
            {
                InternalDebug.Assert(count > 0);

                outputBuffer = buffer;
                outputOffset = offset;
                outputCount = count;
            }

            
            
            
            public void ReportRead(int count)
            {
                InternalDebug.Assert(this.count >= count);

                offset += count;
                this.count -= count;
            }

            
            
            
            
            
            
            public int Read(char[] buffer, int offset, int count)
            {
                InternalDebug.Assert(this.count > 0);

                var countToCopy = Math.Min(count, this.count);

                Buffer.BlockCopy(this.buffer, this.offset * 2, buffer, offset * 2, countToCopy * 2);

                this.count -= countToCopy;
                this.offset += countToCopy;
            
                count -= countToCopy;
                offset += countToCopy;

                return countToCopy;
            }

            
            
            
            private void AllocateBuffer(int size)
            {
                if (size < DefaultMaxLength / 2)
                {
                    size = DefaultMaxLength / 2;
                }

                size = (size * 2 + 1023) / 1024 * 1024;

                buffer = new char[size];
            }
        }
    }
}

