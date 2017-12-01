using System;

namespace SiteServer.CMS.Model
{
	public class InputInfo
	{
	    public InputInfo()
		{
            InputId = 0;
            InputName = string.Empty;
			PublishmentSystemId = 0;
            AddDate = DateTime.Now;
            IsChecked = true;
            IsReply = false;
            Taxis = 0;
            SettingsXml = string.Empty;
		}

        public InputInfo(int inputId, string inputName, int publishmentSystemId, DateTime addDate, bool isChecked, bool isReply, int taxis, string settingsXml) 
		{
            InputId = inputId;
            InputName = inputName;
			PublishmentSystemId = publishmentSystemId;
            AddDate = addDate;
            IsChecked = isChecked;
            IsReply = isReply;
            Taxis = taxis;
            SettingsXml = settingsXml;
        }

        public int InputId { get; set; }

	    public string InputName { get; set; }

	    public int PublishmentSystemId { get; set; }

	    public DateTime AddDate { get; set; }

	    public bool IsChecked { get; set; }

	    public bool IsReply { get; set; }

	    public int Taxis { get; set; }

	    public string SettingsXml { get; set; }

	    private InputInfoExtend _additional;
        public InputInfoExtend Additional => _additional ?? (_additional = new InputInfoExtend(SettingsXml));
	}
}
