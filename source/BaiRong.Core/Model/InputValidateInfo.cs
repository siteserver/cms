namespace BaiRong.Core.Model
{
	public class InputValidateInfo
    {
        public InputValidateInfo()
		{
            IsRequire = false;
            MinNum = 0;
            MaxNum = 0;
            RegExp = string.Empty;
            ErrorMessage = string.Empty;
		}

        public InputValidateInfo(bool isRequire, int minNum, int maxNum, string regExp, string errorMessage) 
		{
            IsRequire = isRequire;
            MinNum = minNum;
            MaxNum = maxNum;
            RegExp = regExp;
            ErrorMessage = errorMessage;
		}

        public bool IsRequire { get; set; }

        public int MinNum { get; set; }

        public int MaxNum { get; set; }

        public string RegExp { get; set; }

        public string ErrorMessage { get; set; }
    }
}
