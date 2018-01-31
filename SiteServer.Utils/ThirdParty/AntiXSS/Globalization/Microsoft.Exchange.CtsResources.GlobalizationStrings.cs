// ***************************************************************
// <copyright file="Microsoft.Exchange.CtsResources.GlobalizationStrings.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

using System.Resources;
using static System.String;

namespace Microsoft.Exchange.CtsResources
{
	
	
	
	static class GlobalizationStrings
	{
		
		
		
		
		private static string[] stringIDs = 
		{
			"MaxCharactersCannotBeNegative", 
			"PriorityListIncludesNonDetectableCodePage", 
			"IndexOutOfRange", 
			"CountTooLarge", 
			"OffsetOutOfRange", 
			"CountOutOfRange", 
		};

		
		
		
		public enum IDs
		{
			
			
			
			MaxCharactersCannotBeNegative, 
			
			
			
			PriorityListIncludesNonDetectableCodePage, 
			
			
			
			IndexOutOfRange, 
			
			
			
			CountTooLarge, 
			
			
			
			OffsetOutOfRange, 
			
			
			
			CountOutOfRange, 
		}

		
		
		
		public enum ParamIDs
		{
			
			
			
			InvalidCharset, 
			
			
			
			InvalidLocaleId, 
			
			
			
			NotInstalledCodePage, 
			
			
			
			NotInstalledCharset, 
			
			
			
			InvalidCodePage, 
			
			
			
			NotInstalledCharsetCodePage, 
			
			
			
			InvalidCultureName, 
		}

		
		
		
		
		public static string InvalidCharset (string charsetName)
		{
			return Format(ResourceManager.GetString("InvalidCharset"), charsetName);
		}
		
		
		
		public static string MaxCharactersCannotBeNegative => ResourceManager.GetString("MaxCharactersCannotBeNegative");


	    public static string PriorityListIncludesNonDetectableCodePage => ResourceManager.GetString("PriorityListIncludesNonDetectableCodePage");


	    public static string IndexOutOfRange => ResourceManager.GetString("IndexOutOfRange");


	    public static string InvalidLocaleId (int localeId)
		{
			return Format(ResourceManager.GetString("InvalidLocaleId"), localeId);
		}
		
		
		
		
		public static string NotInstalledCodePage (int codePage)
		{
			return Format(ResourceManager.GetString("NotInstalledCodePage"), codePage);
		}
		
		
		
		
		public static string NotInstalledCharset (string charsetName)
		{
			return Format(ResourceManager.GetString("NotInstalledCharset"), charsetName);
		}
		
		
		
		
		public static string InvalidCodePage (int codePage)
		{
			return Format(ResourceManager.GetString("InvalidCodePage"), codePage);
		}
		
		
		
		
		
		public static string NotInstalledCharsetCodePage (int codePage, string charsetName)
		{
			return Format(ResourceManager.GetString("NotInstalledCharsetCodePage"), codePage, charsetName);
		}
		
		
		
		
		public static string InvalidCultureName (string cultureName)
		{
			return Format(ResourceManager.GetString("InvalidCultureName"), cultureName);
		}
		
		
		
		public static string CountTooLarge => ResourceManager.GetString("CountTooLarge");


	    public static string OffsetOutOfRange => ResourceManager.GetString("OffsetOutOfRange");


	    public static string CountOutOfRange => ResourceManager.GetString("CountOutOfRange");


	    public static string GetLocalizedString( IDs key )
		{
			return ResourceManager.GetString(stringIDs[(int)key]);
		}

		
		
		
		private static ResourceManager ResourceManager = new ResourceManager("Microsoft.Exchange.CtsResources.GlobalizationStrings", typeof(GlobalizationStrings).Assembly);
	}
}
