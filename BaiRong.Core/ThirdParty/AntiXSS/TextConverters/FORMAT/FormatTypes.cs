// ***************************************************************
// <copyright file="FormatType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.TextConverters.Internal.Format
{
    using System;
    using System.Runtime.InteropServices;
    using Data.Internal;

    

    internal struct FlagProperties
    {
        private const uint AllDefinedBits = 0xAAAAAAAAu;
        private const uint AllValueBits = 0x55555555u;
        private const uint ValueBit = 0x00000001u;
        private const uint DefinedBit = 0x00000002u;
        private const uint ValueAndDefinedBits = 0x00000003u;

        internal uint bits;

        public static readonly FlagProperties AllUndefined = new FlagProperties(0);
        public static readonly FlagProperties AllOff = new FlagProperties(0);
        public static readonly FlagProperties AllOn = new FlagProperties(0xFFFFFFFFu);

        internal FlagProperties(uint bits)
        {
            this.bits = bits;
        }

        internal int IntegerBag
        {
            get { return unchecked((int)bits); }
            set { bits = unchecked((uint)value); }
        }

        public bool IsClear => 0 == bits;

        public static bool IsFlagProperty(PropertyId id)
        {
            return id >= PropertyId.FirstFlag && id <= PropertyId.LastFlag;
        }

        public void Set(PropertyId id, bool value)
        {
            InternalDebug.Assert(IsFlagProperty(id));

            var shift = (id - PropertyId.FirstFlag) * 2;
            var mask = (DefinedBit | ValueBit) << shift;

            if (value)
            {
                 bits |= ((DefinedBit | ValueBit) << shift);               
            }
            else
            {
                bits &= ~(ValueBit << shift);               
                bits |= (DefinedBit << shift);                
            }
        }

        public void Remove(PropertyId id)
        {
            InternalDebug.Assert(IsFlagProperty(id));

            bits &= ~((DefinedBit | ValueBit) << ((id - PropertyId.FirstFlag) * 2));   
        }

        public void ClearAll()
        {
            bits = 0u;
        }

        public bool IsDefined(PropertyId id)
        {
            InternalDebug.Assert(IsFlagProperty(id));

            return 0 != (bits & (DefinedBit << ((id - PropertyId.FirstFlag) * 2)));    
        }

        public bool IsAnyDefined()
        {
            return bits != 0u;
        }

        public bool IsOn(PropertyId id)
        {
            InternalDebug.Assert(IsFlagProperty(id) && IsDefined(id));

            return 0 != (bits & (ValueBit << ((id - PropertyId.FirstFlag) * 2)));    
        }

        public bool IsDefinedAndOn(PropertyId id)
        {
            InternalDebug.Assert(IsFlagProperty(id));

            return ValueAndDefinedBits == ((bits >> ((id - PropertyId.FirstFlag) * 2)) & ValueAndDefinedBits);    
        }

        public bool IsDefinedAndOff(PropertyId id)
        {
            InternalDebug.Assert(IsFlagProperty(id));

            return DefinedBit == ((bits >> ((id - PropertyId.FirstFlag) * 2)) & ValueAndDefinedBits);    
        }

        public PropertyValue GetPropertyValue(PropertyId id)
        {
            InternalDebug.Assert(IsFlagProperty(id));

            var shift = (id - PropertyId.FirstFlag) * 2;
            if (0 != (bits & (DefinedBit << shift)))          
            {
                return new PropertyValue(0 != (bits & (ValueBit << shift)));
            }

            return PropertyValue.Null;
        }

        public void SetPropertyValue(PropertyId id, PropertyValue value)
        {
            if (value.IsBool)
            {
                Set(id, value.Bool);
            }
        }

        
        
        public bool IsSubsetOf(FlagProperties overrideFlags)
        {
            return 0 == ((bits & AllDefinedBits) & ~(overrideFlags.bits & AllDefinedBits));
        }

        public uint Mask => (bits & AllDefinedBits) | ((bits & AllDefinedBits) >> 1);
#if false
        public void Mask(FlagProperties maskFlags)
        {
            
            
            this.bits &= (maskFlags.bits & AllDefinedBits) | ((maskFlags.bits & AllDefinedBits) >> 1);
        }
#endif
        public void Merge(FlagProperties overrideFlags)
        {
            
            
            
            bits = (bits & ~((overrideFlags.bits & AllDefinedBits) >> 1)) | overrideFlags.bits;
        }

        public void ReverseMerge(FlagProperties baseFlags)
        {
            
            
            
            bits = (baseFlags.bits & ~((bits & AllDefinedBits) >> 1)) | bits;
        }

        public static FlagProperties Merge(FlagProperties baseFlags, FlagProperties overrideFlags)
        {
            
            
            
            return new FlagProperties((baseFlags.bits & ~((overrideFlags.bits & AllDefinedBits) >> 1)) | overrideFlags.bits);
        }

        
        
        
        public static FlagProperties operator &(FlagProperties x, FlagProperties y) 
        {
            return new FlagProperties(x.bits & ((y.bits & AllDefinedBits) | ((y.bits & AllDefinedBits) >> 1)));
        }

        
        
        
        
        
        public static FlagProperties operator |(FlagProperties x, FlagProperties y) 
        {
            return Merge(x, y);
        }

        
        
        
        public static FlagProperties operator ^(FlagProperties x, FlagProperties y) 
        {
            var tmp = (x.bits ^ y.bits) & x.Mask & y.Mask;
            return new FlagProperties(tmp | (tmp << 1));
        }

        
        
        
        public static FlagProperties operator ~(FlagProperties x) 
        {
            return new FlagProperties(~((x.bits & AllDefinedBits) | ((x.bits & AllDefinedBits) >> 1)));
        }

        
        public static bool operator ==(FlagProperties x, FlagProperties y) 
        {
            return x.bits == y.bits;
        }

        
        public static bool operator !=(FlagProperties x, FlagProperties y) 
        {
            return x.bits != y.bits;
        }

        public override bool Equals(object obj)
        {
            return (obj is FlagProperties) && bits == ((FlagProperties)obj).bits;
        }

        public override int GetHashCode()
        {
            return (int)bits;
        }

        
        public override string ToString()
        {
            var result = "";

            for (var pid = PropertyId.FirstFlag; pid <= PropertyId.LastFlag; pid++)
            {
                if (IsDefined(pid))
                {
                    if (result.Length != 0)
                    {
                        result += ", ";
                    }
                    result += pid.ToString() + (IsOn(pid) ? ":on" : ":off");
                }
            }

            return result;
        }
    }

    

    internal struct PropertyBitMask
    {
        public const PropertyId FirstNonFlag = PropertyId.LastFlag + 1;

        internal uint bits1;
        internal uint bits2;

        public static readonly PropertyBitMask AllOff = new PropertyBitMask(0, 0);
        public static readonly PropertyBitMask AllOn = new PropertyBitMask(0xFFFFFFFFu, 0xFFFFFFFFu);

        internal PropertyBitMask(uint bits1, uint bits2)
        {
            
            
            InternalDebug.Assert(PropertyId.MaxValue - FirstNonFlag <= 32 * 2);

            this.bits1 = bits1;
            this.bits2 = bits2;
        }

        internal void Set1(uint bits1)
        {
            this.bits1 = bits1;
        }

        internal void Set2(uint bits2)
        {
            this.bits2 = bits2;
        }

        public void Or(PropertyBitMask newBits)
        {
            bits1 |= newBits.bits1;
            bits2 |= newBits.bits2;
        }

        public bool IsClear => (0 == bits1 && 0 == bits2);

        public bool IsSet(PropertyId id)
        {
            InternalDebug.Assert(id >= FirstNonFlag && id < PropertyId.MaxValue);
            return 0 != (id < FirstNonFlag + 32 ? (bits1 & (1u << (id - FirstNonFlag))) : (bits2 & (1u << (id - FirstNonFlag - 32))));
        }

        public bool IsNotSet(PropertyId id)
        {
            InternalDebug.Assert(id >= FirstNonFlag && id < PropertyId.MaxValue);
            return 0 == (id < FirstNonFlag + 32 ? (bits1 & (1u << (id - FirstNonFlag))) : (bits2 & (1u << (id - FirstNonFlag - 32))));
        }

        public void Set(PropertyId id)
        {
            InternalDebug.Assert(id >= FirstNonFlag && id < PropertyId.MaxValue);
            if (id < FirstNonFlag + 32)
            {
                bits1 |= (1u << (id - FirstNonFlag));
            }
            else
            {
                bits2 |= (1u << (id - FirstNonFlag - 32));
            }
        }

        public void Clear(PropertyId id)
        {
            InternalDebug.Assert(id >= FirstNonFlag && id < PropertyId.MaxValue);
            if (id < FirstNonFlag + 32)
            {
                bits1 &= ~(1u << (id - FirstNonFlag));
            }
            else
            {
                bits2 &= ~(1u << (id - FirstNonFlag - 32));
            }
        }

        public bool IsSubsetOf(PropertyBitMask overrideFlags)
        {
            return 0 == (bits1 & ~overrideFlags.bits1) && 0 == (bits2 & ~overrideFlags.bits2);
        }

        public void ClearAll()
        {
            bits1 = 0;
            bits2 = 0;
        }

        public static PropertyBitMask operator |(PropertyBitMask x, PropertyBitMask y) 
        {
            return new PropertyBitMask(x.bits1 | y.bits1, x.bits2 | y.bits2);
        }

        public static PropertyBitMask operator &(PropertyBitMask x, PropertyBitMask y) 
        {
            return new PropertyBitMask(x.bits1 & y.bits1, x.bits2 & y.bits2);
        }

        public static PropertyBitMask operator ^(PropertyBitMask x, PropertyBitMask y) 
        {
            return new PropertyBitMask(x.bits1 ^ y.bits1, x.bits2 ^ y.bits2);
        }

        public static PropertyBitMask operator ~(PropertyBitMask x) 
        {
            return new PropertyBitMask(~x.bits1, ~x.bits2);
        }

        
        public static bool operator ==(PropertyBitMask x, PropertyBitMask y) 
        {
            return x.bits1 == y.bits1 && x.bits2 == y.bits2;
        }

        
        public static bool operator !=(PropertyBitMask x, PropertyBitMask y) 
        {
            return x.bits1 != y.bits1 || x.bits2 != y.bits2;
        }

        public override bool Equals(object obj)
        {
            return (obj is PropertyBitMask) && bits1 == ((PropertyBitMask)obj).bits1 && bits2 == ((PropertyBitMask)obj).bits2;
        }

        public override int GetHashCode()
        {
            return (int)(bits1 ^ bits2);
        }

        
        public override string ToString()
        {
            var result = "";

            for (var pid = FirstNonFlag; pid < PropertyId.MaxValue; pid++)
            {
                if (IsSet(pid))
                {
                    if (result.Length != 0)
                    {
                        result += ", ";
                    }
                    result += pid.ToString();
                }
            }

            return result;
        }

        public DefinedPropertyIdEnumerator GetEnumerator()
        {
            return new DefinedPropertyIdEnumerator(this);
        }

        public struct DefinedPropertyIdEnumerator
        {
            internal ulong bits;
            internal ulong currentBit;
            internal PropertyId currentId;

            internal DefinedPropertyIdEnumerator(PropertyBitMask mask)
            {
                bits = ((ulong)mask.bits2 << 32) | mask.bits1;
                currentBit = 1;
                
                currentId = (bits != 0) ? PropertyId.LastFlag : PropertyId.MaxValue;
            }

            public PropertyId Current => currentId;

            public bool MoveNext()
            {
                while (currentId != PropertyId.MaxValue)
                {
                    if (currentId != PropertyId.LastFlag)
                    {
                        currentBit <<= 1;
                    }

                    currentId ++;

                    if (currentId != PropertyId.MaxValue && 0 != (bits & currentBit))
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }

    
#if !DATAGEN
    internal class PropertyState
    {
        private const int MaxStackSize = 1000;

        private FlagProperties flagProperties;
        private FlagProperties distinctFlagProperties;

        private PropertyBitMask propertyMask;
        private PropertyBitMask distinctPropertyMask;
        private PropertyValue[] properties = new PropertyValue[(int)(PropertyId.MaxValue - PropertyBitMask.FirstNonFlag)];

        [StructLayout(LayoutKind.Sequential, Pack=2)]
        private struct FlagPropertiesUndo
        {
            public PropertyId fakeId;
            public FlagProperties flags;
        }

        [StructLayout(LayoutKind.Sequential, Pack=2)]
        private struct BitsUndo
        {
            public PropertyId fakeId;
            public uint bits;
        }

        [StructLayout(LayoutKind.Explicit, Pack=2)]
        private struct PropertyUndoEntry
        {
            public const PropertyId FlagPropertiesFakeId = PropertyId.MaxValue + 0;
            public const PropertyId DistinctFlagPropertiesFakeId = PropertyId.MaxValue + 1;
            public const PropertyId DistinctMask1FakeId = PropertyId.MaxValue + 2;
            public const PropertyId DistinctMask2FakeId = PropertyId.MaxValue + 3;

            [FieldOffset(0)] 
            public Property property;

            [FieldOffset(0)] 
            public FlagPropertiesUndo flags;

            [FieldOffset(0)] 
            public BitsUndo bits;

            public bool IsFlags => property.Id == FlagPropertiesFakeId;
            public bool IsDistinctFlags => property.Id == DistinctFlagPropertiesFakeId;
            public bool IsDistinctMask1 => property.Id == DistinctMask1FakeId;
            public bool IsDistinctMask2 => property.Id == DistinctMask2FakeId;

            public void Set(PropertyId id, PropertyValue value)
            {
                property.Set(id, value);
            }

            public void Set(PropertyId fakePropId, FlagProperties flagProperties)
            {
                flags.fakeId = fakePropId;
                flags.flags = flagProperties;
            }

            public void Set(PropertyId fakePropId, uint bits)
            {
                this.bits.fakeId = fakePropId;
                this.bits.bits = bits;
            }
        }

        private PropertyUndoEntry[] propertyUndoStack = new PropertyUndoEntry[(int)PropertyId.MaxValue * 2];
        private int propertyUndoStackTop;

        public int UndoStackTop => propertyUndoStackTop;

        public FlagProperties GetEffectiveFlags()
        {
            return flagProperties;
        }

        public FlagProperties GetDistinctFlags()
        {
            return distinctFlagProperties;
        }

        public PropertyValue GetEffectiveProperty(PropertyId id)
        {
            if (FlagProperties.IsFlagProperty(id))
            {
                return flagProperties.GetPropertyValue(id);
            }
            else if (propertyMask.IsSet(id))
            {
                return properties[(int)(id - PropertyBitMask.FirstNonFlag)];
            }
            return PropertyValue.Null;
        }

        public PropertyValue GetDistinctProperty(PropertyId id)
        {
            if (FlagProperties.IsFlagProperty(id))
            {
                return distinctFlagProperties.GetPropertyValue(id);
            }
            else if (distinctPropertyMask.IsSet(id))
            {
                return properties[(int)(id - PropertyBitMask.FirstNonFlag)];
            }
            return PropertyValue.Null;
        }

        public void SubtractDefaultFromDistinct(FlagProperties defaultFlags, Property[] defaultProperties)
        {
            
            var overridenFlagsPropertiesMask = defaultFlags ^ distinctFlagProperties;
            var newDistinctFlagProperties = (distinctFlagProperties & overridenFlagsPropertiesMask) | (distinctFlagProperties & ~defaultFlags);
            if (distinctFlagProperties != newDistinctFlagProperties)
            {
                PushUndoEntry(PropertyUndoEntry.DistinctFlagPropertiesFakeId, distinctFlagProperties);
                distinctFlagProperties = newDistinctFlagProperties;
            }

            if (defaultProperties != null)
            {
                var savedDistinctMask = false;
                foreach (var prop in defaultProperties)
                {
                    if (distinctPropertyMask.IsSet(prop.Id) && properties[(int)(prop.Id - PropertyBitMask.FirstNonFlag)] == prop.Value)
                    {
                        if (!savedDistinctMask)
                        {
                            PushUndoEntry(distinctPropertyMask);
                            savedDistinctMask = true;
                        }

                        distinctPropertyMask.Clear(prop.Id);
                    }
                }
            }
        }

        public int ApplyProperties(FlagProperties flagProperties, Property[] propList, FlagProperties flagInheritanceMask, PropertyBitMask propertyInheritanceMask)
        {
            var undoStackPosition = propertyUndoStackTop;

            var allInheritedFlagProperties = this.flagProperties & flagInheritanceMask;

            var newEffectiveFlagProperties = allInheritedFlagProperties | flagProperties;

            if (newEffectiveFlagProperties != this.flagProperties)
            {
                PushUndoEntry(PropertyUndoEntry.FlagPropertiesFakeId, this.flagProperties);
                this.flagProperties = newEffectiveFlagProperties;
            }

            
            var overridenFlagsPropertiesMask = allInheritedFlagProperties ^ flagProperties;

            var newDistinctFlagProperties = (flagProperties & overridenFlagsPropertiesMask) | (flagProperties & ~allInheritedFlagProperties);

            if (newDistinctFlagProperties != distinctFlagProperties)
            {
                PushUndoEntry(PropertyUndoEntry.DistinctFlagPropertiesFakeId, distinctFlagProperties);
                distinctFlagProperties = newDistinctFlagProperties;
            }

            var maskedOutProperties = propertyMask & ~propertyInheritanceMask;

            foreach (var propId in maskedOutProperties)
            {
                PushUndoEntry(propId, properties[(int)(propId - PropertyBitMask.FirstNonFlag)]);
            }

            var newDistinctPropertyMask = PropertyBitMask.AllOff;
            propertyMask &= propertyInheritanceMask;

            if (propList != null)
            {
                foreach (var prop in propList)
                {
                    if (propertyMask.IsSet(prop.Id))
                    {
                        if (properties[(int)(prop.Id - PropertyBitMask.FirstNonFlag)] != prop.Value)
                        {
                            PushUndoEntry(prop.Id, properties[(int)(prop.Id - PropertyBitMask.FirstNonFlag)]);

                            if (prop.Value.IsNull)
                            {
                                propertyMask.Clear(prop.Id);
                            }
                            else
                            {
                                properties[(int)(prop.Id - PropertyBitMask.FirstNonFlag)] = prop.Value;
                                newDistinctPropertyMask.Set(prop.Id);
                            }
                        }
                    }
                    else if (!prop.Value.IsNull)
                    {
                        if (!maskedOutProperties.IsSet(prop.Id))
                        {
                            PushUndoEntry(prop.Id, PropertyValue.Null);
                        }

                        properties[(int)(prop.Id - PropertyBitMask.FirstNonFlag)] = prop.Value;

                        propertyMask.Set(prop.Id);

                        newDistinctPropertyMask.Set(prop.Id);
                    }
                }
            }

            if (newDistinctPropertyMask != distinctPropertyMask)
            {
                PushUndoEntry(distinctPropertyMask);
                distinctPropertyMask = newDistinctPropertyMask;
            }

            return undoStackPosition;
        }

        public void UndoProperties(int undoLevel)
        {
            InternalDebug.Assert(undoLevel <= propertyUndoStackTop);

            for (var i = propertyUndoStackTop - 1; i >= undoLevel; i--)
            {
                if (propertyUndoStack[i].IsFlags)
                {
                    flagProperties = propertyUndoStack[i].flags.flags;
                }
                else if (propertyUndoStack[i].IsDistinctFlags)
                {
                    distinctFlagProperties = propertyUndoStack[i].flags.flags;
                }
                else if (propertyUndoStack[i].IsDistinctMask1)
                {
                    distinctPropertyMask.Set1(propertyUndoStack[i].bits.bits);
                }
                else if (propertyUndoStack[i].IsDistinctMask2)
                {
                    distinctPropertyMask.Set2(propertyUndoStack[i].bits.bits);
                }
                else 
                {
                    if (propertyUndoStack[i].property.Value.IsNull)
                    {
                        propertyMask.Clear(propertyUndoStack[i].property.Id);
                    }
                    else
                    {
                        properties[(int)(propertyUndoStack[i].property.Id - PropertyBitMask.FirstNonFlag)] = propertyUndoStack[i].property.Value;
                        propertyMask.Set(propertyUndoStack[i].property.Id);
                    }
                }
            }

            propertyUndoStackTop = undoLevel;
        }

        private void PushUndoEntry(PropertyId id, PropertyValue value)
        {
            if (propertyUndoStackTop == propertyUndoStack.Length)
            {
                if (propertyUndoStack.Length >= MaxStackSize)
                {
                    throw new TextConvertersException("property undo stack is too large");
                }

                var newStackSize = Math.Min(propertyUndoStack.Length * 2, MaxStackSize);

                var newPropertyUndoStack = new PropertyUndoEntry[newStackSize];
                Array.Copy(propertyUndoStack, 0, newPropertyUndoStack, 0, propertyUndoStackTop);
                propertyUndoStack = newPropertyUndoStack;
            }

            propertyUndoStack[propertyUndoStackTop++].Set(id, value);
        }

        private void PushUndoEntry(PropertyId fakePropId, FlagProperties flagProperties)
        {
            if (propertyUndoStackTop == propertyUndoStack.Length)
            {
                if (propertyUndoStack.Length >= MaxStackSize)
                {
                    throw new TextConvertersException("property undo stack is too large");
                }

                var newStackSize = Math.Min(propertyUndoStack.Length * 2, MaxStackSize);

                var newPropertyUndoStack = new PropertyUndoEntry[newStackSize];
                Array.Copy(propertyUndoStack, 0, newPropertyUndoStack, 0, propertyUndoStackTop);
                propertyUndoStack = newPropertyUndoStack;
            }

            propertyUndoStack[propertyUndoStackTop++].Set(fakePropId, flagProperties);
        }

        private void PushUndoEntry(PropertyBitMask propertyMask)
        {
            if (propertyUndoStackTop + 1 >= propertyUndoStack.Length)
            {
                if (propertyUndoStackTop + 2 >= MaxStackSize)
                {
                    throw new TextConvertersException("property undo stack is too large");
                }

                var newStackSize = Math.Min(propertyUndoStack.Length * 2, MaxStackSize);

                var newPropertyUndoStack = new PropertyUndoEntry[newStackSize];
                Array.Copy(propertyUndoStack, 0, newPropertyUndoStack, 0, propertyUndoStackTop);
                propertyUndoStack = newPropertyUndoStack;
            }

            propertyUndoStack[propertyUndoStackTop++].Set(PropertyUndoEntry.DistinctMask1FakeId, propertyMask.bits1);
            propertyUndoStack[propertyUndoStackTop++].Set(PropertyUndoEntry.DistinctMask2FakeId, propertyMask.bits2);
        }

        public override string ToString()
        {
            return "flags: (" + flagProperties.ToString() + "), props: (" + propertyMask.ToString() + "), dflags: (" + distinctFlagProperties.ToString() + "), dprops: (" + distinctPropertyMask.ToString() + ")";
        }

    }
#endif
}

