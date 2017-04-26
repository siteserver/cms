using System;
using System.Collections;
using System.Reflection;

namespace BaiRong.Core.Model
{
	[Serializable]
	public abstract class Copyable
	{
		private static readonly Hashtable Objects = new Hashtable();

		protected object CreateNewInstance()
		{
			var ci = Objects[GetType()] as ConstructorInfo;
			if(ci == null)
			{
				ci = GetType().GetConstructor(new Type[0]);
                Objects[GetType()] = ci;
			}

		    return ci != null ? ci.Invoke(null) : null;
		}

		public virtual object Copy()
		{
			return CreateNewInstance();
		}
	}
}
