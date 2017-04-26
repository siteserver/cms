using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Wcm.Model
{
	public class GovPublicIdentifierRuleInfo
	{
        private int ruleID;
        private string ruleName;
        private int publishmentSystemID;
        private EGovPublicIdentifierType identifierType;
        private int minLength;
        private string suffix;
        private string formatString;
        private string attributeName;
        private int sequence;
		private int taxis;
        private string settingsXML = string.Empty;

		public GovPublicIdentifierRuleInfo()
		{
            ruleID = 0;
            ruleName = string.Empty;
            publishmentSystemID = 0;
            identifierType = EGovPublicIdentifierType.Department;
            minLength = 5;
            suffix = string.Empty;
            formatString = string.Empty;
            attributeName = string.Empty;
            sequence = 0;
			taxis = 0;
            settingsXML = string.Empty;
		}

        public GovPublicIdentifierRuleInfo(int ruleID, string ruleName, int publishmentSystemID, EGovPublicIdentifierType identifierType, int minLength, string suffix, string formatString, string attributeName, int sequence, int taxis, string settingsXML) 
		{
            this.ruleID = ruleID;
            this.ruleName = ruleName;
            this.publishmentSystemID = publishmentSystemID;
            this.identifierType = identifierType;
            this.minLength = minLength;
            this.suffix = suffix;
            this.formatString = formatString;
            this.attributeName = attributeName;
            this.sequence = sequence;
            this.taxis = taxis;
            this.settingsXML = settingsXML;
		}

        public int RuleID
		{
            get { return ruleID; }
            set { ruleID = value; }
		}

        public string RuleName
        {
            get { return ruleName; }
            set { ruleName = value; }
        }

        public int PublishmentSystemID
        {
            get { return publishmentSystemID; }
            set { publishmentSystemID = value; }
        }

        public EGovPublicIdentifierType IdentifierType
		{
            get { return identifierType; }
            set { identifierType = value; }
		}

        public int MinLength
		{
            get { return minLength; }
            set { minLength = value; }
		}

        public string Suffix
		{
            get { return suffix; }
            set { suffix = value; }
		}

        public string FormatString
		{
            get { return formatString; }
            set { formatString = value; }
		}

        public string AttributeName
		{
            get { return attributeName; }
            set { attributeName = value; }
		}

        public int Sequence
        {
            get { return sequence; }
            set { sequence = value; }
        }

		public int Taxis
		{
			get{ return taxis; }
			set{ taxis = value; }
		}

        public string SettingsXML
        {
            get { return settingsXML; }
            set
            {
                additional = null;
                settingsXML = value;
            }
        }

        GovPublicIdentifierRuleInfoExtend additional;
        public GovPublicIdentifierRuleInfoExtend Additional
        {
            get
            {
                if (additional == null)
                {
                    additional = new GovPublicIdentifierRuleInfoExtend(settingsXML);
                }
                return additional;
            }
        }
	}
}
