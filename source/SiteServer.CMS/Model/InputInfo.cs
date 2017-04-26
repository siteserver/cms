using System;

namespace SiteServer.CMS.Model
{
	public class InputInfo
	{
        private int inputID;
        private string inputName;
		private int publishmentSystemID;
        private DateTime addDate;
        private bool isChecked;
        private bool isReply;
        private int taxis;
        private string settingsXML;

        public InputInfo()
		{
            inputID = 0;
            inputName = string.Empty;
			publishmentSystemID = 0;
            addDate = DateTime.Now;
            isChecked = true;
            isReply = false;
            taxis = 0;
            settingsXML = string.Empty;
		}

        public InputInfo(int inputID, string inputName, int publishmentSystemID, DateTime addDate, bool isChecked, bool isReply, int taxis, string settingsXML) 
		{
            this.inputID = inputID;
            this.inputName = inputName;
			this.publishmentSystemID = publishmentSystemID;
            this.addDate = addDate;
            this.isChecked = isChecked;
            this.isReply = isReply;
            this.taxis = taxis;
            this.settingsXML = settingsXML;
        }

        public int InputID
        {
            get { return inputID; }
            set { inputID = value; }
        }

        public string InputName
		{
            get { return inputName; }
            set { inputName = value; }
		}

		public int PublishmentSystemID
		{
			get{ return publishmentSystemID; }
			set{ publishmentSystemID = value; }
		}

        public DateTime AddDate
        {
            get { return addDate; }
            set { addDate = value; }
        }

        public bool IsChecked
        {
            get { return isChecked; }
            set { isChecked = value; }
        }

        public bool IsReply
        {
            get { return isReply; }
            set { isReply = value; }
        }

        public int Taxis
        {
            get { return taxis; }
            set { taxis = value; }
        }

        public string SettingsXML
        {
            get { return settingsXML; }
            set { settingsXML = value; }
        }

        InputInfoExtend additional;
        public InputInfoExtend Additional
        {
            get
            {
                if (additional == null)
                {
                    additional = new InputInfoExtend(settingsXML);
                }
                return additional;
            }
        }
	}
}
