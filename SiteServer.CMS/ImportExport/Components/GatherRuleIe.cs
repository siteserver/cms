using System.Collections.Generic;
using Atom.Core;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.ImportExport.Components
{
	internal class GatherRuleIe
	{
		private readonly int _publishmentSystemId;
		private readonly string _filePath;

		public GatherRuleIe(int publishmentSystemId, string filePath)
		{
			_publishmentSystemId = publishmentSystemId;
			_filePath = filePath;
		}

		public void ExportGatherRule(List<GatherRuleInfo> gatherRuleInfoList)
		{
			var feed = AtomUtility.GetEmptyFeed();

			foreach (var gatherRuleInfo in gatherRuleInfoList)
			{
				var entry = ExportGatherRuleInfo(gatherRuleInfo);
				feed.Entries.Add(entry);
			}

			feed.Save(_filePath);
		}

		private static AtomEntry ExportGatherRuleInfo(GatherRuleInfo gatherRuleInfo)
		{
			var entry = AtomUtility.GetEmptyEntry();

			AtomUtility.AddDcElement(entry.AdditionalElements, "GatherRuleName", gatherRuleInfo.GatherRuleName);
			AtomUtility.AddDcElement(entry.AdditionalElements, "PublishmentSystemID", gatherRuleInfo.PublishmentSystemId.ToString());
			AtomUtility.AddDcElement(entry.AdditionalElements, "CookieString", AtomUtility.Encrypt(gatherRuleInfo.CookieString));//加密
            AtomUtility.AddDcElement(entry.AdditionalElements, "GatherUrlIsCollection", gatherRuleInfo.GatherUrlIsCollection.ToString());
			AtomUtility.AddDcElement(entry.AdditionalElements, "GatherUrlCollection", gatherRuleInfo.GatherUrlCollection);
            AtomUtility.AddDcElement(entry.AdditionalElements, "GatherUrlIsSerialize", gatherRuleInfo.GatherUrlIsSerialize.ToString());
			AtomUtility.AddDcElement(entry.AdditionalElements, "GatherUrlSerialize", gatherRuleInfo.GatherUrlSerialize);
			AtomUtility.AddDcElement(entry.AdditionalElements, "SerializeFrom", gatherRuleInfo.SerializeFrom.ToString());
			AtomUtility.AddDcElement(entry.AdditionalElements, "SerializeTo", gatherRuleInfo.SerializeTo.ToString());
			AtomUtility.AddDcElement(entry.AdditionalElements, "SerializeInterval", gatherRuleInfo.SerializeInterval.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, "SerializeIsOrderByDesc", gatherRuleInfo.SerializeIsOrderByDesc.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, "SerializeIsAddZero", gatherRuleInfo.SerializeIsAddZero.ToString());
			AtomUtility.AddDcElement(entry.AdditionalElements, "NodeID", gatherRuleInfo.NodeId.ToString());
			AtomUtility.AddDcElement(entry.AdditionalElements, "Charset", ECharsetUtils.GetValue(gatherRuleInfo.Charset));
			AtomUtility.AddDcElement(entry.AdditionalElements, "UrlInclude", gatherRuleInfo.UrlInclude);
			AtomUtility.AddDcElement(entry.AdditionalElements, "TitleInclude", gatherRuleInfo.TitleInclude);
			AtomUtility.AddDcElement(entry.AdditionalElements, "ContentExclude", AtomUtility.Encrypt(gatherRuleInfo.ContentExclude));//加密
			AtomUtility.AddDcElement(entry.AdditionalElements, "ContentHtmlClearCollection", gatherRuleInfo.ContentHtmlClearCollection);
            AtomUtility.AddDcElement(entry.AdditionalElements, "ContentHtmlClearTagCollection", gatherRuleInfo.ContentHtmlClearTagCollection);
			AtomUtility.AddDcElement(entry.AdditionalElements, "LastGatherDate", gatherRuleInfo.LastGatherDate.ToLongDateString());
			AtomUtility.AddDcElement(entry.AdditionalElements, "ListAreaStart", AtomUtility.Encrypt(gatherRuleInfo.ListAreaStart));//加密
			AtomUtility.AddDcElement(entry.AdditionalElements, "ListAreaEnd", AtomUtility.Encrypt(gatherRuleInfo.ListAreaEnd));//加密
			AtomUtility.AddDcElement(entry.AdditionalElements, "ContentChannelStart", AtomUtility.Encrypt(gatherRuleInfo.ContentChannelStart));//加密
			AtomUtility.AddDcElement(entry.AdditionalElements, "ContentChannelEnd", AtomUtility.Encrypt(gatherRuleInfo.ContentChannelEnd));//加密
			AtomUtility.AddDcElement(entry.AdditionalElements, "ContentTitleStart", AtomUtility.Encrypt(gatherRuleInfo.ContentTitleStart));//加密
			AtomUtility.AddDcElement(entry.AdditionalElements, "ContentTitleEnd", AtomUtility.Encrypt(gatherRuleInfo.ContentTitleEnd));//加密
			AtomUtility.AddDcElement(entry.AdditionalElements, "ContentContentStart", AtomUtility.Encrypt(gatherRuleInfo.ContentContentStart));//加密
			AtomUtility.AddDcElement(entry.AdditionalElements, "ContentContentEnd", AtomUtility.Encrypt(gatherRuleInfo.ContentContentEnd));//加密
			AtomUtility.AddDcElement(entry.AdditionalElements, "ContentNextPageStart", AtomUtility.Encrypt(gatherRuleInfo.ContentNextPageStart));//加密
			AtomUtility.AddDcElement(entry.AdditionalElements, "ContentNextPageEnd", AtomUtility.Encrypt(gatherRuleInfo.ContentNextPageEnd));//加密
            AtomUtility.AddDcElement(entry.AdditionalElements, "ContentAttributes", AtomUtility.Encrypt(gatherRuleInfo.ContentAttributes));//加密
            AtomUtility.AddDcElement(entry.AdditionalElements, "ContentAttributesXML", AtomUtility.Encrypt(gatherRuleInfo.ContentAttributesXml));//加密
            AtomUtility.AddDcElement(entry.AdditionalElements, "ExtendValues", AtomUtility.Encrypt(gatherRuleInfo.ExtendValues));//加密

			return entry;
		}


		public void ImportGatherRule(bool overwrite)
		{
			if (!FileUtils.IsFileExists(_filePath)) return;
            var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(_filePath));

			foreach (AtomEntry entry in feed.Entries)
			{
				var gatherRuleName = AtomUtility.GetDcElementContent(entry.AdditionalElements, "GatherRuleName");

				if (!string.IsNullOrEmpty(gatherRuleName))
				{
				    var gatherRuleInfo = new GatherRuleInfo
				    {
				        GatherRuleName = gatherRuleName,
				        PublishmentSystemId = _publishmentSystemId,
				        CookieString =
				            AtomUtility.Decrypt(AtomUtility.GetDcElementContent(entry.AdditionalElements, "CookieString")),
				        GatherUrlIsCollection =
				            TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements, "GatherUrlIsCollection")),
				        GatherUrlCollection = AtomUtility.GetDcElementContent(entry.AdditionalElements, "GatherUrlCollection"),
				        GatherUrlIsSerialize =
				            TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements, "GatherUrlIsSerialize")),
				        GatherUrlSerialize = AtomUtility.GetDcElementContent(entry.AdditionalElements, "GatherUrlSerialize"),
				        SerializeFrom =
				            TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, "SerializeFrom")),
				        SerializeTo = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, "SerializeTo")),
				        SerializeInterval =
				            TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, "SerializeInterval")),
				        SerializeIsOrderByDesc =
				            TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements, "SerializeIsOrderByDesc")),
				        SerializeIsAddZero =
				            TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements, "SerializeIsAddZero")),
				        NodeId = _publishmentSystemId,
				        Charset = ECharsetUtils.GetEnumType(AtomUtility.GetDcElementContent(entry.AdditionalElements, "Charset")),
				        UrlInclude = AtomUtility.GetDcElementContent(entry.AdditionalElements, "UrlInclude"),
				        TitleInclude = AtomUtility.GetDcElementContent(entry.AdditionalElements, "TitleInclude"),
				        ContentExclude =
				            AtomUtility.Decrypt(AtomUtility.GetDcElementContent(entry.AdditionalElements, "ContentExclude")),
				        ContentHtmlClearCollection =
				            AtomUtility.GetDcElementContent(entry.AdditionalElements, "ContentHtmlClearCollection"),
				        ContentHtmlClearTagCollection =
				            AtomUtility.GetDcElementContent(entry.AdditionalElements, "ContentHtmlClearTagCollection"),
				        LastGatherDate = DateUtils.SqlMinValue,
				        ListAreaStart =
				            AtomUtility.Decrypt(AtomUtility.GetDcElementContent(entry.AdditionalElements, "ListAreaStart")),
				        ListAreaEnd = AtomUtility.Decrypt(AtomUtility.GetDcElementContent(entry.AdditionalElements, "ListAreaEnd")),
				        ContentChannelStart =
				            AtomUtility.Decrypt(AtomUtility.GetDcElementContent(entry.AdditionalElements, "ContentChannelStart")),
				        ContentChannelEnd =
				            AtomUtility.Decrypt(AtomUtility.GetDcElementContent(entry.AdditionalElements, "ContentChannelEnd")),
				        ContentTitleStart =
				            AtomUtility.Decrypt(AtomUtility.GetDcElementContent(entry.AdditionalElements, "ContentTitleStart")),
				        ContentTitleEnd =
				            AtomUtility.Decrypt(AtomUtility.GetDcElementContent(entry.AdditionalElements, "ContentTitleEnd")),
				        ContentContentStart =
				            AtomUtility.Decrypt(AtomUtility.GetDcElementContent(entry.AdditionalElements, "ContentContentStart")),
				        ContentContentEnd =
				            AtomUtility.Decrypt(AtomUtility.GetDcElementContent(entry.AdditionalElements, "ContentContentEnd")),
				        ContentNextPageStart =
				            AtomUtility.Decrypt(AtomUtility.GetDcElementContent(entry.AdditionalElements, "ContentNextPageStart")),
				        ContentNextPageEnd =
				            AtomUtility.Decrypt(AtomUtility.GetDcElementContent(entry.AdditionalElements, "ContentNextPageEnd")),
				        ContentAttributes =
				            AtomUtility.Decrypt(AtomUtility.GetDcElementContent(entry.AdditionalElements, "ContentAttributes")),
				        ContentAttributesXml =
				            AtomUtility.Decrypt(AtomUtility.GetDcElementContent(entry.AdditionalElements, "ContentAttributesXML")),
				        ExtendValues =
				            AtomUtility.Decrypt(AtomUtility.GetDcElementContent(entry.AdditionalElements, "ExtendValues"))
				    };

				    var srcGatherRuleInfo = DataProvider.GatherRuleDao.GetGatherRuleInfo(gatherRuleInfo.GatherRuleName, _publishmentSystemId);
					if (srcGatherRuleInfo != null)
					{
						if (overwrite)
						{
							DataProvider.GatherRuleDao.Update(gatherRuleInfo);
						}
						else
						{
							var importGatherRuleName = DataProvider.GatherRuleDao.GetImportGatherRuleName(_publishmentSystemId, gatherRuleInfo.GatherRuleName);
							gatherRuleInfo.GatherRuleName = importGatherRuleName;
							DataProvider.GatherRuleDao.Insert(gatherRuleInfo);
						}
					}
					else
					{
						DataProvider.GatherRuleDao.Insert(gatherRuleInfo);
					}
				}
			}
		}

	}
}
