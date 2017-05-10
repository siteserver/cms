using System;
using Atom.Core;
using BaiRong.Core;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.ImportExport.Components
{
	internal class InputIe
	{
		private readonly int _publishmentSystemId;
		private readonly string _directoryPath;

        public InputIe(int publishmentSystemId, string directoryPath)
		{
			_publishmentSystemId = publishmentSystemId;
			_directoryPath = directoryPath;
		}

        public void ExportInput(int inputId)
		{
            var inputInfo = DataProvider.InputDao.GetInputInfo(inputId);
            var filePath = _directoryPath + PathUtils.SeparatorChar + inputInfo.InputId + ".xml";

            var feed = ExportInputInfo(inputInfo);

            var styleDirectoryPath = PathUtils.Combine(_directoryPath, inputInfo.InputId.ToString());
            TableStyleIe.SingleExportTableStyles(ETableStyle.InputContent, DataProvider.InputContentDao.TableName, _publishmentSystemId, inputInfo.InputId, styleDirectoryPath);

            var contentIdList = DataProvider.InputContentDao.GetContentIdListWithChecked(inputInfo.InputId);
            foreach (var contentId in contentIdList)
			{
                var contentInfo = DataProvider.InputContentDao.GetContentInfo(contentId);
                var entry = GetAtomEntry(contentInfo);
				feed.Entries.Add(entry);
			}
			feed.Save(filePath);
		}

        private static AtomFeed ExportInputInfo(InputInfo inputInfo)
		{
			var feed = AtomUtility.GetEmptyFeed();

            AtomUtility.AddDcElement(feed.AdditionalElements, "InputID", inputInfo.InputId.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, "InputName", inputInfo.InputName);
            AtomUtility.AddDcElement(feed.AdditionalElements, "PublishmentSystemID", inputInfo.PublishmentSystemId.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, "AddDate", DateUtils.GetDateAndTimeString(inputInfo.AddDate));
            AtomUtility.AddDcElement(feed.AdditionalElements, "IsChecked", inputInfo.IsChecked.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, "IsReply", inputInfo.IsReply.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, "Taxis", inputInfo.Taxis.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, "SettingsXML", AtomUtility.Encrypt(inputInfo.SettingsXml));

			return feed;
		}

        private static AtomEntry GetAtomEntry(InputContentInfo contentInfo)
		{
			var entry = AtomUtility.GetEmptyEntry();

            foreach (string attributeName in contentInfo.Attributes)
            {
                AtomUtility.AddDcElement(entry.AdditionalElements, attributeName, contentInfo.Attributes[attributeName]);
            }

			return entry;
		}

		public void ImportInput(bool overwrite)
		{
			if (!DirectoryUtils.IsDirectoryExists(_directoryPath)) return;
			var filePaths = DirectoryUtils.GetFilePaths(_directoryPath);

			foreach (var filePath in filePaths)
			{
                var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(filePath));

			    var inputInfo = new InputInfo
			    {
			        InputName = AtomUtility.GetDcElementContent(feed.AdditionalElements, "InputName"),
			        PublishmentSystemId = _publishmentSystemId,
			        AddDate = DateTime.Now,
			        IsChecked =
			            TranslateUtils.ToBool(AtomUtility.GetDcElementContent(feed.AdditionalElements, "IsChecked")),
			        IsReply = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(feed.AdditionalElements, "IsReply")),
			        Taxis = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(feed.AdditionalElements, "Taxis")),
			        SettingsXml =
			            AtomUtility.Decrypt(AtomUtility.GetDcElementContent(feed.AdditionalElements, "SettingsXML"))
			    };

			    var srcInputInfo = DataProvider.InputDao.GetInputInfo(inputInfo.InputName, _publishmentSystemId);
                if (srcInputInfo != null)
				{
					if (overwrite)
					{
                        DataProvider.InputDao.Delete(srcInputInfo.InputId);
					}
					else
					{
                        inputInfo.InputName = DataProvider.InputDao.GetImportInputName(inputInfo.InputName, _publishmentSystemId);
					}
				}

                var inputId = DataProvider.InputDao.Insert(inputInfo);

                var styleDirectoryPath = PathUtils.Combine(_directoryPath, AtomUtility.GetDcElementContent(feed.AdditionalElements, "InputID"));
                TableStyleIe.SingleImportTableStyle(ETableStyle.InputContent, DataProvider.InputContentDao.TableName, styleDirectoryPath, inputId);

                foreach (AtomEntry entry in feed.Entries)
                {
                    var contentInfo = new InputContentInfo
                    {
                        InputId = inputId,
                        IsChecked = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements, InputContentAttribute.IsChecked)),
                        UserName = AtomUtility.GetDcElementContent(entry.AdditionalElements, InputContentAttribute.UserName),
                        AddDate = DateTime.Now,
                        Reply = AtomUtility.GetDcElementContent(entry.AdditionalElements, InputContentAttribute.Reply)
                    };
                    var attributes = AtomUtility.GetDcElementNameValueCollection(entry.AdditionalElements);
                    foreach (string entryName in attributes.Keys)
                    {
                        if (!InputContentAttribute.AllAttributes.Contains(entryName.ToLower()))
                        {
                            contentInfo.SetExtendedAttribute(entryName, attributes[entryName]);
                        }
                    }
                    DataProvider.InputContentDao.Insert(contentInfo);
                }
			}
		}
    }
}
