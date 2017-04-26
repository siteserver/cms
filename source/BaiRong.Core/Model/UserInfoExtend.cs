namespace BaiRong.Core.Model
{
    public class UserInfoExtend : ExtendedAttributes
    {
        public UserInfoExtend(string extendValues)
        {
            var nameValueCollection = TranslateUtils.ToNameValueCollection(extendValues);
            SetExtendedAttribute(nameValueCollection);
        }

        public int LastWritingPublishmentSystemId
        {
            get { return GetInt("LastWritingPublishmentSystemId", 0); }
            set { SetExtendedAttribute("LastWritingPublishmentSystemId", value.ToString()); }
        }

        public int LastWritingNodeId
        {
            get { return GetInt("LastWritingNodeId", 0); }
            set { SetExtendedAttribute("LastWritingNodeId", value.ToString()); }
        }
    }
}
