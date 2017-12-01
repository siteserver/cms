using System;

namespace SiteServer.CMS.Model
{
	[Serializable]
	public class MenuDisplayInfo
	{
		private int _menuDisplayId;
		private int _publishmentSystemId;
		private string _menuDisplayName;
		//外观
		private string _vertical;
		private string _fontFamily;
		private int _fontSize;
		private string _fontWeight;
		private string _fontStyle;
		private string _menuItemHAlign;
		private string _menuItemVAlign;
		private string _fontColor;
		private string _menuItemBgColor;
		private string _fontColorHilite;
		private string _menuHiliteBgColor;
		//位置
		private string _xPosition;
		private string _yPosition;
		private string _hideOnMouseOut;
		//高级
		private int _menuWidth;
		private int _menuItemHeight;
		private int _menuItemPadding;
		private int _menuItemSpacing;
		private int _menuItemIndent;
		private int _hideTimeout;
		private string _menuBgOpaque;
		private int _menuBorder;
		private string _bgColor;
		private string _menuBorderBgColor;
		private string _menuLiteBgColor;
		private string _childMenuIcon;
		//其他
		private DateTime _addDate;
        private bool _isDefault;
		private string _description;

		public MenuDisplayInfo()
		{
			_menuDisplayId = 0;
			_publishmentSystemId = 0;
			_menuDisplayName = string.Empty;
			_vertical = string.Empty;
			_fontFamily = string.Empty;
			_fontSize = 0;
			_fontWeight = string.Empty;
			_fontStyle = string.Empty;
			_menuItemHAlign = string.Empty;
			_menuItemVAlign = string.Empty;
			_fontColor = string.Empty;
			_menuItemBgColor = string.Empty;
			_fontColorHilite = string.Empty;
			_menuHiliteBgColor = string.Empty;
			_xPosition = "0";
			_yPosition = "0";
			_hideOnMouseOut = string.Empty;
			_menuWidth = 0;
			_menuItemHeight = 0;
			_menuItemPadding = 0;
			_menuItemSpacing = 0;
			_menuItemIndent = 0;
			_hideTimeout = 0;
			_menuBgOpaque = string.Empty;
			_menuBorder = 0;
			_bgColor = string.Empty;
			_menuBorderBgColor = string.Empty;
			_menuLiteBgColor = string.Empty;
			_childMenuIcon = string.Empty;
			_addDate = DateTime.Now;
			_isDefault = false;
			_description = string.Empty;
		}

        public MenuDisplayInfo(int menuDisplayId, int publishmentSystemId, string menuDisplayName, string vertical, string fontFamily, int fontSize, string fontWeight, string fontStyle, string menuItemHAlign, string menuItemVAlign, string fontColor, string menuItemBgColor, string fontColorHilite, string menuHiliteBgColor, string xPosition, string yPosition, string hideOnMouseOut, int menuWidth, int menuItemHeight, int menuItemPadding, int menuItemSpacing, int menuItemIndent, int hideTimeout, string menuBgOpaque, int menuBorder, string bgColor, string menuBorderBgColor, string menuLiteBgColor, string childMenuIcon, DateTime addDate, bool isDefault, string description) 
		{
			_menuDisplayId = menuDisplayId;
			_publishmentSystemId = publishmentSystemId;
			_menuDisplayName = menuDisplayName;
			_vertical = vertical;
			_fontFamily = fontFamily;
			_fontSize = fontSize;
			_fontWeight = fontWeight;
			_fontStyle = fontStyle;
			_menuItemHAlign = menuItemHAlign;
			_menuItemVAlign = menuItemVAlign;
			_fontColor = fontColor;
			_menuItemBgColor = menuItemBgColor;
			_fontColorHilite = fontColorHilite;
			_menuHiliteBgColor = menuHiliteBgColor;
			_xPosition = xPosition;
			_yPosition = yPosition;
			_hideOnMouseOut = hideOnMouseOut;
			_menuWidth = menuWidth;
			_menuItemHeight = menuItemHeight;
			_menuItemPadding = menuItemPadding;
			_menuItemSpacing = menuItemSpacing;
			_menuItemIndent = menuItemIndent;
			_hideTimeout = hideTimeout;
			_menuBgOpaque = menuBgOpaque;
			_menuBorder = menuBorder;
			_bgColor = bgColor;
			_menuBorderBgColor = menuBorderBgColor;
			_menuLiteBgColor = menuLiteBgColor;
			_childMenuIcon = childMenuIcon;
			_addDate = addDate;
			_isDefault = isDefault;
			_description = description;
		}

		public int MenuDisplayId
		{
			get{ return _menuDisplayId; }
			set{ _menuDisplayId = value; }
		}

		public int PublishmentSystemId
		{
			get{ return _publishmentSystemId; }
			set{ _publishmentSystemId = value; }
		}

		public string MenuDisplayName
		{
			get{ return _menuDisplayName; }
			set{ _menuDisplayName = value; }
		}

		public string Vertical
		{
			get{ return _vertical; }
			set{ _vertical = value; }
		}

		public string FontFamily
		{
			get{ return _fontFamily; }
			set{ _fontFamily = value; }
		}

		public int FontSize
		{
			get{ return _fontSize; }
			set{ _fontSize = value; }
		}

		//
		public string FontWeight
		{
			get{ return _fontWeight; }
			set{ _fontWeight = value; }
		}

		public string FontStyle
		{
			get{ return _fontStyle; }
			set{ _fontStyle = value; }
		}

		public string MenuItemHAlign
		{
			get{ return _menuItemHAlign; }
			set{ _menuItemHAlign = value; }
		}

		public string MenuItemVAlign
		{
			get{ return _menuItemVAlign; }
			set{ _menuItemVAlign = value; }
		}

		public string FontColor
		{
			get{ return _fontColor; }
			set{ _fontColor = value; }
		}

		public string MenuItemBgColor
		{
			get{ return _menuItemBgColor; }
			set{ _menuItemBgColor = value; }
		}

		public string FontColorHilite
		{
			get{ return _fontColorHilite; }
			set{ _fontColorHilite = value; }
		}

		public string MenuHiliteBgColor
		{
			get{ return _menuHiliteBgColor; }
			set{ _menuHiliteBgColor = value; }
		}

		public string YPosition
		{
			get{ return _yPosition; }
			set{ _yPosition = value; }
		}

		public string XPosition
		{
			get{ return _xPosition; }
			set{ _xPosition = value; }
		}

		public string HideOnMouseOut
		{
			get{ return _hideOnMouseOut; }
			set{ _hideOnMouseOut = value; }
		}

		public int MenuWidth
		{
			get{ return _menuWidth; }
			set{ _menuWidth = value; }
		}

		public int MenuItemHeight
		{
			get{ return _menuItemHeight; }
			set{ _menuItemHeight = value; }
		}

		public int MenuItemPadding
		{
			get{ return _menuItemPadding; }
			set{ _menuItemPadding = value; }
		}

		public int MenuItemSpacing
		{
			get{ return _menuItemSpacing; }
			set{ _menuItemSpacing = value; }
		}

		public int MenuItemIndent
		{
			get{ return _menuItemIndent; }
			set{ _menuItemIndent = value; }
		}

		public int HideTimeout
		{
			get{ return _hideTimeout; }
			set{ _hideTimeout = value; }
		}

		public string MenuBgOpaque
		{
			get{ return _menuBgOpaque; }
			set{ _menuBgOpaque = value; }
		}

		public int MenuBorder
		{
			get{ return _menuBorder; }
			set{ _menuBorder = value; }
		}

		public string BgColor
		{
			get{ return _bgColor; }
			set{ _bgColor = value; }
		}

		public string MenuBorderBgColor
		{
			get{ return _menuBorderBgColor; }
			set{ _menuBorderBgColor = value; }
		}

		public string MenuLiteBgColor
		{
			get{ return _menuLiteBgColor; }
			set{ _menuLiteBgColor = value; }
		}

		public string ChildMenuIcon
		{
			get{ return _childMenuIcon; }
			set{ _childMenuIcon = value; }
		}
		//

		public DateTime AddDate
		{
			get{ return _addDate; }
			set{ _addDate = value; }
		}

        public bool IsDefault
		{
			get{ return _isDefault; }
			set{ _isDefault = value; }
		}

		public string Description
		{
			get{ return _description; }
			set{ _description = value; }
		}

	}
}
