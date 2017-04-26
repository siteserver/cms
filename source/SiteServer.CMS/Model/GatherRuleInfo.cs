using System;
using BaiRong.Core.Model.Enumerations;

namespace SiteServer.CMS.Model
{
	public class GatherRuleInfo
	{
	    public GatherRuleInfo()
		{
			GatherRuleName = string.Empty;
			PublishmentSystemId = 0;
			CookieString = string.Empty;
			GatherUrlIsCollection = true;
			GatherUrlCollection = string.Empty;
			GatherUrlIsSerialize = false;
			GatherUrlSerialize = string.Empty;
			SerializeFrom = 0;
			SerializeTo = 0;
			SerializeInterval = 0;
			SerializeIsOrderByDesc = true;
			SerializeIsAddZero = false;
			NodeId = 0;
			Charset = ECharset.utf_8;
			UrlInclude = string.Empty;
			TitleInclude = string.Empty;
			ContentExclude = string.Empty;
			ContentHtmlClearCollection = string.Empty;
            ContentHtmlClearTagCollection = string.Empty;
			LastGatherDate = DateTime.Now;
			ListAreaStart = string.Empty;
			ListAreaEnd = string.Empty;
			ContentChannelStart = string.Empty;
			ContentChannelEnd = string.Empty;
			ContentTitleStart = string.Empty;
			ContentTitleEnd = string.Empty;
			ContentContentStart = string.Empty;
			ContentContentEnd = string.Empty;
			ContentNextPageStart = string.Empty;
			ContentNextPageEnd = string.Empty;
            ContentAttributes = string.Empty;
            ContentAttributesXml = string.Empty;
            ExtendValues = string.Empty;
		}

        public GatherRuleInfo(string gatherRuleName, int publishmentSystemId, string cookieString, bool gatherUrlIsCollection, string gatherUrlCollection, bool gatherUrlIsSerialize, string gatherUrlSerialize, int serializeFrom, int serializeTo, int serializeInterval, bool serializeIsOrderByDesc, bool serializeIsAddZero, int nodeId, ECharset charset, string urlInclude, string titleInclude, string contentExclude, string contentHtmlClearCollection, string contentHtmlClearTagCollection, DateTime lastGatherDate, string listAreaStart, string listAreaEnd, string contentChannelStart, string contentChannelEnd, string contentTitleStart, string contentTitleEnd, string contentContentStart, string contentContentEnd, string contentNextPageStart, string contentNextPageEnd, string contentAttributes, string contentAttributesXml, string extendValues) 
		{
			GatherRuleName = gatherRuleName;
			PublishmentSystemId = publishmentSystemId;
			CookieString = cookieString;
			GatherUrlIsCollection = gatherUrlIsCollection;
			GatherUrlCollection = gatherUrlCollection;
			GatherUrlIsSerialize = gatherUrlIsSerialize;
			GatherUrlSerialize = gatherUrlSerialize;
			SerializeFrom = serializeFrom;
			SerializeTo = serializeTo;
			SerializeInterval = serializeInterval;
			SerializeIsOrderByDesc = serializeIsOrderByDesc;
			SerializeIsAddZero = serializeIsAddZero;
			NodeId = nodeId;
			Charset = charset;
			UrlInclude = urlInclude;
			TitleInclude = titleInclude;
			ContentExclude = contentExclude;
			ContentHtmlClearCollection = contentHtmlClearCollection;
            ContentHtmlClearTagCollection = contentHtmlClearTagCollection;
			LastGatherDate = lastGatherDate;
			ListAreaStart = listAreaStart;
			ListAreaEnd = listAreaEnd;
			ContentChannelStart = contentChannelStart;
			ContentChannelEnd = contentChannelEnd;
			ContentTitleStart = contentTitleStart;
			ContentTitleEnd = contentTitleEnd;
			ContentContentStart = contentContentStart;
			ContentContentEnd = contentContentEnd;
			ContentNextPageStart = contentNextPageStart;
			ContentNextPageEnd = contentNextPageEnd;
            ContentAttributes = contentAttributes;
            ContentAttributesXml = contentAttributesXml;
            ExtendValues = extendValues;
		}

		public string GatherRuleName { get; set; }

	    public int PublishmentSystemId { get; set; }

	    public string CookieString { get; set; }

	    public bool GatherUrlIsCollection { get; set; }

	    public string GatherUrlCollection { get; set; }

	    public bool GatherUrlIsSerialize { get; set; }

	    public string GatherUrlSerialize { get; set; }

	    public int SerializeFrom { get; set; }

	    public int SerializeTo { get; set; }

	    public int SerializeInterval { get; set; }

	    public bool SerializeIsOrderByDesc { get; set; }

	    public bool SerializeIsAddZero { get; set; }

	    public int NodeId { get; set; }

	    public ECharset Charset { get; set; }

	    public string UrlInclude { get; set; }

	    public string TitleInclude { get; set; }

	    public string ContentExclude { get; set; }

	    public string ContentHtmlClearCollection { get; set; }

	    public string ContentHtmlClearTagCollection { get; set; }

	    public DateTime LastGatherDate { get; set; }

	    public string ListAreaStart { get; set; }

	    public string ListAreaEnd { get; set; }

	    public string ContentChannelStart { get; set; }

	    public string ContentChannelEnd { get; set; }

	    public string ContentTitleStart { get; set; }

	    public string ContentTitleEnd { get; set; }

	    public string ContentContentStart { get; set; }

	    public string ContentContentEnd { get; set; }

	    public string ContentNextPageStart { get; set; }

	    public string ContentNextPageEnd { get; set; }

	    public string ContentAttributes { get; set; }

	    public string ContentAttributesXml { get; set; }

	    public string ExtendValues { get; set; }

	    private GatherRuleInfoExtend _additional;
        public GatherRuleInfoExtend Additional => _additional ?? (_additional = new GatherRuleInfoExtend(ExtendValues));
	}
}
