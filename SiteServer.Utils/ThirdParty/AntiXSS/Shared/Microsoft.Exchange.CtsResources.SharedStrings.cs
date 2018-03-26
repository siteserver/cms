// ***************************************************************
// <copyright file="Microsoft.Exchange.CtsResources.SharedStrings.cs" company="Microsoft">
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
	
	
	
	static class SharedStrings
	{
		
		
		
		
		private static string[] stringIDs = 
		{
			"CountTooLarge", 
			"CannotSeekBeforeBeginning", 
			"CannotSetNegativelength", 
			"InvalidFactory", 
			"OffsetOutOfRange", 
			"CountOutOfRange", 
			"StringArgumentMustBeAscii", 
		};

		
		
		
		public enum IDs
		{
			
			
			
			CountTooLarge, 
			
			
			
			CannotSeekBeforeBeginning, 
			
			
			
			CannotSetNegativelength, 
			
			
			
			InvalidFactory, 
			
			
			
			OffsetOutOfRange, 
			
			
			
			CountOutOfRange, 
			
			
			
			StringArgumentMustBeAscii, 
		}

		
		
		
		public enum ParamIDs
		{
			
			
			
			CreateFileFailed, 
		}

		
		
		
		public static string CountTooLarge => ResourceManager.GetString("CountTooLarge");


	    public static string CannotSeekBeforeBeginning => ResourceManager.GetString("CannotSeekBeforeBeginning");


	    public static string CannotSetNegativelength => ResourceManager.GetString("CannotSetNegativelength");


	    public static string CreateFileFailed (string filePath)
		{
			return Format(ResourceManager.GetString("CreateFileFailed"), filePath);
		}
		
		
		public static string InvalidFactory => ResourceManager.GetString("InvalidFactory");


	    public static string OffsetOutOfRange => ResourceManager.GetString("OffsetOutOfRange");


	    public static string CountOutOfRange => ResourceManager.GetString("CountOutOfRange");


	    public static string StringArgumentMustBeAscii => ResourceManager.GetString("StringArgumentMustBeAscii");


	    public static string GetLocalizedString( IDs key )
		{
			return ResourceManager.GetString(stringIDs[(int)key]);
		}

		
		
		
		private static ResourceManager ResourceManager = new ResourceManager("Microsoft.Exchange.CtsResources.SharedStrings", typeof(SharedStrings).Assembly);
	}
}
