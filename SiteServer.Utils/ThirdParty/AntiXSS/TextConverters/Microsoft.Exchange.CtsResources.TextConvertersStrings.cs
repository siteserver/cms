// ***************************************************************
// <copyright file="Microsoft.Exchange.CtsResources.TextConvertersStrings.cs" company="Microsoft">
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
	
	
	
	static class TextConvertersStrings
	{
		
		
		
		
		private static string[] stringIDs = 
		{
			"AttributeNotStarted", 
			"InputEncodingRequired", 
			"AttributeCollectionNotInitialized", 
			"ConverterWriterInInconsistentStare", 
			"CannotUseConverterReader", 
			"CannotReadFromSource", 
			"AttributeIdIsUnknown", 
			"TagTooLong", 
			"EndTagCannotHaveAttributes", 
			"CannotUseConverterWriter", 
			"WriteAfterFlush", 
			"PriorityListIncludesNonDetectableCodePage", 
			"TagNotStarted", 
			"CannotSetNegativelength", 
			"TooManyIterationsToFlushConverter", 
			"IndexOutOfRange", 
			"TagNameIsEmpty", 
			"MaxCharactersCannotBeNegative", 
			"AttributeNotValidForThisContext", 
			"InputDocumentTooComplex", 
			"TextWriterUnsupported", 
			"PropertyNotValidForCodepageConversionMode", 
			"CountTooLarge", 
			"TooManyIterationsToProduceOutput", 
			"AttributeNotInitialized", 
			"AttributeIdInvalid", 
			"TooManyIterationsToProcessInput", 
			"BufferSizeValueRange", 
			"CannotWriteWhileCopyPending", 
			"AttributeNotValidInThisState", 
			"ParametersCannotBeChangedAfterConverterObjectIsUsed", 
			"CountOutOfRange", 
			"WriteUnsupported", 
			"AccessShouldBeReadOrWrite", 
			"ContextNotValidInThisState", 
			"CannotSeekBeforeBeginning", 
			"OffsetOutOfRange", 
			"ReadUnsupported", 
			"SeekUnsupported", 
			"CallbackTagAlreadyWritten", 
			"ConverterStreamInInconsistentStare", 
			"AttributeNameIsEmpty", 
			"HtmlNestingTooDeep", 
			"CannotWriteToDestination", 
			"ConverterReaderInInconsistentStare", 
			"TextReaderUnsupported", 
			"PropertyNotValidForTextExtractionMode", 
			"TagIdIsUnknown", 
			"TagIdInvalid", 
			"CallbackTagAlreadyDeleted", 
		};

		
		
		
		public enum IDs
		{
			
			
			
			AttributeNotStarted, 
			
			
			
			InputEncodingRequired, 
			
			
			
			AttributeCollectionNotInitialized, 
			
			
			
			ConverterWriterInInconsistentStare, 
			
			
			
			CannotUseConverterReader, 
			
			
			
			CannotReadFromSource, 
			
			
			
			AttributeIdIsUnknown, 
			
			
			
			TagTooLong, 
			
			
			
			EndTagCannotHaveAttributes, 
			
			
			
			CannotUseConverterWriter, 
			
			
			
			WriteAfterFlush, 
			
			
			
			PriorityListIncludesNonDetectableCodePage, 
			
			
			
			TagNotStarted, 
			
			
			
			CannotSetNegativelength, 
			
			
			
			TooManyIterationsToFlushConverter, 
			
			
			
			IndexOutOfRange, 
			
			
			
			TagNameIsEmpty, 
			
			
			
			MaxCharactersCannotBeNegative, 
			
			
			
			AttributeNotValidForThisContext, 
			
			
			
			InputDocumentTooComplex, 
			
			
			
			TextWriterUnsupported, 
			
			
			
			PropertyNotValidForCodepageConversionMode, 
			
			
			
			CountTooLarge, 
			
			
			
			TooManyIterationsToProduceOutput, 
			
			
			
			AttributeNotInitialized, 
			
			
			
			AttributeIdInvalid, 
			
			
			
			TooManyIterationsToProcessInput, 
			
			
			
			BufferSizeValueRange, 
			
			
			
			CannotWriteWhileCopyPending, 
			
			
			
			AttributeNotValidInThisState, 
			
			
			
			ParametersCannotBeChangedAfterConverterObjectIsUsed, 
			
			
			
			CountOutOfRange, 
			
			
			
			WriteUnsupported, 
			
			
			
			AccessShouldBeReadOrWrite, 
			
			
			
			ContextNotValidInThisState, 
			
			
			
			CannotSeekBeforeBeginning, 
			
			
			
			OffsetOutOfRange, 
			
			
			
			ReadUnsupported, 
			
			
			
			SeekUnsupported, 
			
			
			
			CallbackTagAlreadyWritten, 
			
			
			
			ConverterStreamInInconsistentStare, 
			
			
			
			AttributeNameIsEmpty, 
			
			
			
			HtmlNestingTooDeep, 
			
			
			
			CannotWriteToDestination, 
			
			
			
			ConverterReaderInInconsistentStare, 
			
			
			
			TextReaderUnsupported, 
			
			
			
			PropertyNotValidForTextExtractionMode, 
			
			
			
			TagIdIsUnknown, 
			
			
			
			TagIdInvalid, 
			
			
			
			CallbackTagAlreadyDeleted, 
		}

		
		
		
		public enum ParamIDs
		{
			
			
			
			CreateFileFailed, 
			
			
			
			InvalidConfigurationBoolean, 
			
			
			
			InvalidConfigurationStream, 
			
			
			
			CannotWriteOtherTagsInsideElement, 
			
			
			
			InvalidConfigurationInteger, 
			
			
			
			LengthExceeded, 
			
			
			
			InvalidCodePage, 
		}

		
		
		
		public static string AttributeNotStarted => ResourceManager.GetString("AttributeNotStarted");


	    public static string InputEncodingRequired => ResourceManager.GetString("InputEncodingRequired");


	    public static string CreateFileFailed (string filePath)
		{
			return Format(ResourceManager.GetString("CreateFileFailed"), filePath);
		}
		
		
		
		public static string AttributeCollectionNotInitialized => ResourceManager.GetString("AttributeCollectionNotInitialized");


	    public static string ConverterWriterInInconsistentStare => ResourceManager.GetString("ConverterWriterInInconsistentStare");


	    public static string CannotUseConverterReader => ResourceManager.GetString("CannotUseConverterReader");


	    public static string CannotReadFromSource => ResourceManager.GetString("CannotReadFromSource");


	    public static string AttributeIdIsUnknown => ResourceManager.GetString("AttributeIdIsUnknown");


	    public static string TagTooLong => ResourceManager.GetString("TagTooLong");


	    public static string EndTagCannotHaveAttributes => ResourceManager.GetString("EndTagCannotHaveAttributes");


	    public static string CannotUseConverterWriter => ResourceManager.GetString("CannotUseConverterWriter");


	    public static string WriteAfterFlush => ResourceManager.GetString("WriteAfterFlush");


	    public static string PriorityListIncludesNonDetectableCodePage => ResourceManager.GetString("PriorityListIncludesNonDetectableCodePage");


	    public static string TagNotStarted => ResourceManager.GetString("TagNotStarted");


	    public static string CannotSetNegativelength => ResourceManager.GetString("CannotSetNegativelength");


	    public static string TooManyIterationsToFlushConverter => ResourceManager.GetString("TooManyIterationsToFlushConverter");


	    public static string IndexOutOfRange => ResourceManager.GetString("IndexOutOfRange");


	    public static string TagNameIsEmpty => ResourceManager.GetString("TagNameIsEmpty");


	    public static string MaxCharactersCannotBeNegative => ResourceManager.GetString("MaxCharactersCannotBeNegative");


	    public static string AttributeNotValidForThisContext => ResourceManager.GetString("AttributeNotValidForThisContext");


	    public static string InvalidConfigurationBoolean (int propertyId)
		{
			return Format(ResourceManager.GetString("InvalidConfigurationBoolean"), propertyId);
		}
		
		
		
		public static string InputDocumentTooComplex => ResourceManager.GetString("InputDocumentTooComplex");


	    public static string TextWriterUnsupported => ResourceManager.GetString("TextWriterUnsupported");


	    public static string PropertyNotValidForCodepageConversionMode => ResourceManager.GetString("PropertyNotValidForCodepageConversionMode");


	    public static string CountTooLarge => ResourceManager.GetString("CountTooLarge");


	    public static string InvalidConfigurationStream (int propertyId)
		{
			return Format(ResourceManager.GetString("InvalidConfigurationStream"), propertyId);
		}
		
		
		
		public static string TooManyIterationsToProduceOutput => ResourceManager.GetString("TooManyIterationsToProduceOutput");


	    public static string AttributeNotInitialized => ResourceManager.GetString("AttributeNotInitialized");


	    public static string CannotWriteOtherTagsInsideElement (string elementName)
		{
			return Format(ResourceManager.GetString("CannotWriteOtherTagsInsideElement"), elementName);
		}
		
		
		
		public static string AttributeIdInvalid => ResourceManager.GetString("AttributeIdInvalid");


	    public static string InvalidConfigurationInteger (int propertyId)
		{
			return Format(ResourceManager.GetString("InvalidConfigurationInteger"), propertyId);
		}
		
		
		
		public static string TooManyIterationsToProcessInput => ResourceManager.GetString("TooManyIterationsToProcessInput");


	    public static string BufferSizeValueRange => ResourceManager.GetString("BufferSizeValueRange");


	    public static string CannotWriteWhileCopyPending => ResourceManager.GetString("CannotWriteWhileCopyPending");


	    public static string AttributeNotValidInThisState => ResourceManager.GetString("AttributeNotValidInThisState");


	    public static string ParametersCannotBeChangedAfterConverterObjectIsUsed => ResourceManager.GetString("ParametersCannotBeChangedAfterConverterObjectIsUsed");


	    public static string CountOutOfRange => ResourceManager.GetString("CountOutOfRange");


	    public static string WriteUnsupported => ResourceManager.GetString("WriteUnsupported");


	    public static string AccessShouldBeReadOrWrite => ResourceManager.GetString("AccessShouldBeReadOrWrite");


	    public static string ContextNotValidInThisState => ResourceManager.GetString("ContextNotValidInThisState");


	    public static string CannotSeekBeforeBeginning => ResourceManager.GetString("CannotSeekBeforeBeginning");


	    public static string OffsetOutOfRange => ResourceManager.GetString("OffsetOutOfRange");


	    public static string ReadUnsupported => ResourceManager.GetString("ReadUnsupported");


	    public static string LengthExceeded (int sum, int length)
		{
			return Format(ResourceManager.GetString("LengthExceeded"), sum, length);
		}
		
		
		
		public static string SeekUnsupported => ResourceManager.GetString("SeekUnsupported");


	    public static string InvalidCodePage (int codePage)
		{
			return Format(ResourceManager.GetString("InvalidCodePage"), codePage);
		}
		
		
		
		public static string CallbackTagAlreadyWritten => ResourceManager.GetString("CallbackTagAlreadyWritten");


	    public static string ConverterStreamInInconsistentStare => ResourceManager.GetString("ConverterStreamInInconsistentStare");


	    public static string AttributeNameIsEmpty => ResourceManager.GetString("AttributeNameIsEmpty");


	    public static string HtmlNestingTooDeep => ResourceManager.GetString("HtmlNestingTooDeep");


	    public static string CannotWriteToDestination => ResourceManager.GetString("CannotWriteToDestination");


	    public static string ConverterReaderInInconsistentStare => ResourceManager.GetString("ConverterReaderInInconsistentStare");


	    public static string TextReaderUnsupported => ResourceManager.GetString("TextReaderUnsupported");


	    public static string PropertyNotValidForTextExtractionMode => ResourceManager.GetString("PropertyNotValidForTextExtractionMode");


	    public static string TagIdIsUnknown => ResourceManager.GetString("TagIdIsUnknown");


	    public static string TagIdInvalid => ResourceManager.GetString("TagIdInvalid");


	    public static string CallbackTagAlreadyDeleted => ResourceManager.GetString("CallbackTagAlreadyDeleted");


	    public static string GetLocalizedString( IDs key )
		{
			return ResourceManager.GetString(stringIDs[(int)key]);
		}

		
		
		
		private static ResourceManager ResourceManager = new ResourceManager("Microsoft.Exchange.CtsResources.TextConvertersStrings", typeof(TextConvertersStrings).Assembly);
	}
}
