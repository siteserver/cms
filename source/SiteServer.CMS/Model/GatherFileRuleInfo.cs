using System;
using BaiRong.Core.Model.Enumerations;

namespace SiteServer.CMS.Model
{
	public class GatherFileRuleInfo
	{
	    public GatherFileRuleInfo()
		{
			GatherRuleName = string.Empty;
			PublishmentSystemId = 0;
			GatherUrl = string.Empty;
            Charset = ECharset.utf_8;
            LastGatherDate = DateTime.Now;
            IsToFile = true;

            FilePath = string.Empty;
            IsSaveRelatedFiles = false;
            IsRemoveScripts = false;
            StyleDirectoryPath = string.Empty;
            ScriptDirectoryPath = string.Empty;
            ImageDirectoryPath = string.Empty;

            NodeId = 0;
            IsSaveImage = true;
            IsChecked = true;
            IsAutoCreate = false;
            ContentExclude = string.Empty;
			ContentHtmlClearCollection = string.Empty;
            ContentHtmlClearTagCollection = string.Empty;
			ContentTitleStart = string.Empty;
			ContentTitleEnd = string.Empty;
			ContentContentStart = string.Empty;
			ContentContentEnd = string.Empty;
            ContentAttributes = string.Empty;
            ContentAttributesXml = string.Empty;
		}

        public GatherFileRuleInfo(string gatherRuleName, int publishmentSystemId, string gatherUrl, ECharset charset, DateTime lastGatherDate, bool isToFile, string filePath, bool isSaveRelatedFiles, bool isRemoveScripts, string styleDirectoryPath, string scriptDirectoryPath, string imageDirectoryPath, int nodeId, bool isSaveImage, bool isChecked, string contentExclude, string contentHtmlClearCollection, string contentHtmlClearTagCollection, string contentTitleStart, string contentTitleEnd, string contentContentStart, string contentContentEnd, string contentAttributes, string contentAttributesXml, bool isAutoCreate)
		{
            GatherRuleName = gatherRuleName;
            PublishmentSystemId = publishmentSystemId;
            GatherUrl = gatherUrl;
            Charset = charset;
            LastGatherDate = lastGatherDate;
            IsToFile = isToFile;

            FilePath = filePath;
            IsSaveRelatedFiles = isSaveRelatedFiles;
            IsRemoveScripts = isRemoveScripts;
            StyleDirectoryPath = styleDirectoryPath;
            ScriptDirectoryPath = scriptDirectoryPath;
            ImageDirectoryPath = imageDirectoryPath;

            NodeId = nodeId;
            IsSaveImage = isSaveImage;
            IsChecked = isChecked;
            IsAutoCreate = isAutoCreate;
            ContentExclude = contentExclude;
            ContentHtmlClearCollection = contentHtmlClearCollection;
            ContentHtmlClearTagCollection = contentHtmlClearTagCollection;
            ContentTitleStart = contentTitleStart;
            ContentTitleEnd = contentTitleEnd;
            ContentContentStart = contentContentStart;
            ContentContentEnd = contentContentEnd;
            ContentAttributes = contentAttributes;
            ContentAttributesXml = contentAttributesXml;
		}

		public string GatherRuleName { get; set; }

	    public int PublishmentSystemId { get; set; }

	    public string GatherUrl { get; set; }

	    public ECharset Charset { get; set; }

	    public DateTime LastGatherDate { get; set; }

	    public bool IsToFile { get; set; }

	    public string FilePath { get; set; }

	    public bool IsSaveRelatedFiles { get; set; }

	    public bool IsRemoveScripts { get; set; }

	    public string StyleDirectoryPath { get; set; }

	    public string ScriptDirectoryPath { get; set; }

	    public string ImageDirectoryPath { get; set; }

	    public int NodeId { get; set; }

	    public bool IsSaveImage { get; set; }

	    public bool IsChecked { get; set; }

	    public bool IsAutoCreate { get; set; }

	    public string ContentExclude { get; set; }

	    public string ContentHtmlClearCollection { get; set; }

	    public string ContentHtmlClearTagCollection { get; set; }

	    public string ContentTitleStart { get; set; }

	    public string ContentTitleEnd { get; set; }

	    public string ContentContentStart { get; set; }

	    public string ContentContentEnd { get; set; }

	    public string ContentAttributes { get; set; }

	    public string ContentAttributesXml { get; set; }
	}
}
