using BaiRong.Core.Model.Enumerations;

namespace BaiRong.Core.Model
{
    public class ThirdLoginInfo
    {
        public ThirdLoginInfo()
        {
            Id = 0;
            ThirdLoginType = EThirdLoginType.QQ;
            ThirdLoginName = string.Empty;
            IsEnabled = true;
            Taxis = 0;
            Description = string.Empty;
            SettingsXml = string.Empty;
        }

        public ThirdLoginInfo(int id, EThirdLoginType thirdLoginType, string thirdLoginName, bool isEnabled, int taxis, string description, string settingsXml)
        {
            Id = id;
            ThirdLoginType = thirdLoginType;
            ThirdLoginName = thirdLoginName;
            IsEnabled = isEnabled;
            Taxis = taxis;
            Description = description;
            SettingsXml = settingsXml;
        }

        public int Id { get; set; }

        public EThirdLoginType ThirdLoginType { get; set; }

        public string ThirdLoginName { get; set; }

        public bool IsEnabled { get; set; }

        public int Taxis { get; set; }

        public string Description { get; set; }

        public string SettingsXml { get; set; }
    }
}
