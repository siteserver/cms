using Atom.Core;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.ImportExport.Components
{
    internal class PhotoIe
	{
        private readonly PublishmentSystemInfo _publishmentSystemInfo;
		private readonly string _directoryPath;

        public PhotoIe(PublishmentSystemInfo publishmentSystemInfo, string directoryPath)
		{
            _publishmentSystemInfo = publishmentSystemInfo;
			_directoryPath = directoryPath;
		}

        public void ExportPhoto(int contentId)
		{
            var filePath = PathUtils.Combine(_directoryPath, contentId + ".xml");

            var feed = AtomUtility.GetEmptyFeed();

            var photoInfoList = DataProvider.PhotoDao.GetPhotoInfoList(_publishmentSystemInfo.PublishmentSystemId, contentId);

            foreach (var photoInfo in photoInfoList)
			{
                AddAtomEntry(feed, photoInfo);
			}
			feed.Save(filePath);
		}

        private static void AddAtomEntry(AtomFeed feed, PhotoInfo photoInfo)
		{
			var entry = AtomUtility.GetEmptyEntry();

            AtomUtility.AddDcElement(entry.AdditionalElements, "ID", photoInfo.ID.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, "PublishmentSystemID", photoInfo.PublishmentSystemID.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, "ContentID", photoInfo.ContentID.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, "SmallUrl", photoInfo.SmallUrl);
            AtomUtility.AddDcElement(entry.AdditionalElements, "MiddleUrl", photoInfo.MiddleUrl);
            AtomUtility.AddDcElement(entry.AdditionalElements, "LargeUrl", photoInfo.LargeUrl);
            AtomUtility.AddDcElement(entry.AdditionalElements, "Taxis", photoInfo.Taxis.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, "Description", AtomUtility.Encrypt(photoInfo.Description));//加密

            feed.Entries.Add(entry);
		}

		public void ImportPhoto(int contentIdFromFile, int contentId)
		{
            var filePath = PathUtils.Combine(_directoryPath, contentIdFromFile + ".xml");
            if (!FileUtils.IsFileExists(filePath)) return;

            var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(filePath));

            foreach (AtomEntry entry in feed.Entries)
            {
                var smallUrl = AtomUtility.GetDcElementContent(entry.AdditionalElements, "SmallUrl");
                var middleUrl = AtomUtility.GetDcElementContent(entry.AdditionalElements, "MiddleUrl");
                var largeUrl = AtomUtility.GetDcElementContent(entry.AdditionalElements, "LargeUrl");
                var description = AtomUtility.Decrypt(AtomUtility.GetDcElementContent(entry.AdditionalElements, "Description"));

                var photoInfo = new PhotoInfo(0, _publishmentSystemInfo.PublishmentSystemId, contentId, smallUrl, middleUrl, largeUrl, 0, description);

                DataProvider.PhotoDao.Insert(photoInfo);
            }
		}

	}
}
