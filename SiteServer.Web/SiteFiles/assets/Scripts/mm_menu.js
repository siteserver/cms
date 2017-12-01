/**
 * mm_menu 20MAR2002 Version 6.0
 * Andy Finnell, March 2002
 * Copyright (c) 2000-2002 Macromedia, Inc.
 *
 * based on menu.js
 * by gary smith, July 1997
 * Copyright (c) 1997-1999 Netscape Communications Corp.
 *
 * Netscape grants you a royalty free license to use or modify this
 * software provided that this copyright notice appears on all copies.
 * This software is provided "AS IS," without a warranty of any kind.
 */
function Menu(label, mw, mh, fnt, fs, fclr, fhclr, bg, bgh, halgn, valgn, pad, space, to, sx, sy, srel, opq, vert, idt, aw, ah) 
{
	this.version = "020320 [Menu; mm_menu.js]";
	this.type = "Menu";
	this.menuWidth = mw;
	this.menuItemHeight = mh;
	this.fontSize = fs;
	this.fontWeight = "plain";
	this.fontFamily = fnt;
	this.fontColor = fclr;
	this.fontColorHilite = fhclr;
	this.bgColor = "#555555";
	this.menuBorder = 1;
	this.menuBgOpaque=opq;
	this.menuItemBorder = 1;
	this.menuItemIndent = idt;
	this.menuItemBgColor = bg;
	this.menuItemVAlign = valgn;
	this.menuItemHAlign = halgn;
	this.menuItemPadding = pad;
	this.menuItemSpacing = space;
	this.menuLiteBgColor = "#ffffff";
	this.menuBorderBgColor = "#777777";
	this.menuHiliteBgColor = bgh;
	this.menuContainerBgColor = "#cccccc";
	this.childMenuIcon = "arrows.gif";
	this.submenuXOffset = sx;
	this.submenuYOffset = sy;
	this.submenuRelativeToItem = srel;
	this.vertical = vert;
	this.items = new Array();
	this.actions = new Array();
	this.childMenus = new Array();
	this.hideOnMouseOut = true;
	this.hideTimeout = to;
	this.addMenuItem = addMenuItem;
	this.writeMenus = writeMenus;
	this.MM_showMenu = MM_showMenu;
	this.onMenuItemOver = onMenuItemOver;
	this.onMenuItemAction = onMenuItemAction;
	this.hideMenu = hideMenu;
	this.hideChildMenu = hideChildMenu;
	if (!window.menus) window.menus = new Array();
	this.label = " " + label;
	window.menus[this.label] = this;
	window.menus[window.menus.length] = this;
	if (!window.activeMenus) window.activeMenus = new Array();
}

function addMenuItem(label, action) {
	this.items[this.items.length] = label;
	this.actions[this.actions.length] = action;
}

function FIND(item) {
	if( window.mmIsOpera ) return(document.getElementById(item));
	if (document.all) return(document.all[item]);
	if (document.getElementById) return(document.getElementById(item));
	return(false);
}

function writeMenus(container) {
	if (window.triedToWriteMenus) return;
	var agt = navigator.userAgent.toLowerCase();
	window.mmIsOpera = agt.indexOf("opera") != -1;
	if (!container && document.layers) {
		window.delayWriteMenus = this.writeMenus;
		var timer = setTimeout('delayWriteMenus()', 500);
		container = new Layer(100);
		clearTimeout(timer);
	} else if (document.all || document.hasChildNodes || window.mmIsOpera) {
		document.writeln('<span id="menuContainer"></span>');
		container = FIND("menuContainer");
	}

	window.mmHideMenuTimer = null;
	if (!container) return;	
	window.triedToWriteMenus = true; 
	container.isContainer = true;
	container.menus = new Array();
	for (var i=0; i<window.menus.length; i++) 
		container.menus[i] = window.menus[i];
	window.menus.length = 0;
	var countMenus = 0;
	var countItems = 0;
	var top = 0;
	var content = '';
	var lrs = false;
	var theStat = "";
	var tsc = 0;
	if (document.layers) lrs = true;
	for (var i=0; i<container.menus.length; i++, countMenus++) {
		var menu = container.menus[i];
		if (menu.bgImageUp || !menu.menuBgOpaque) {
			menu.menuBorder = 0;
			menu.menuItemBorder = 0;
		}
		if (lrs) {
			var menuLayer = new Layer(100, container);
			var lite = new Layer(100, menuLayer);
			lite.top = menu.menuBorder;
			lite.left = menu.menuBorder;
			var body = new Layer(100, lite);
			body.top = menu.menuBorder;
			body.left = menu.menuBorder;
		} else {
			content += ''+
			'<div id="menuLayer'+ countMenus +'" style="position:absolute;z-index:1;left:10px;top:'+ (i * 100) +'px;visibility:hidden;color:' +  menu.menuBorderBgColor + ';">\n'+
			'  <div id="menuLite'+ countMenus +'" style="position:absolute;z-index:1;left:'+ menu.menuBorder +'px;top:'+ menu.menuBorder +'px;visibility:hide;" onmouseout="mouseoutMenu();">\n'+
			'	 <div id="menuFg'+ countMenus +'" style="position:absolute;left:'+ menu.menuBorder +'px;top:'+ menu.menuBorder +'px;visibility:hide;">\n'+
			'';
		}
		var x=i;
		for (var i=0; i<menu.items.length; i++) {
			var item = menu.items[i];
			var childMenu = false;
			var defaultHeight = menu.fontSize+2*menu.menuItemPadding;
			if (item.label) {
				item = item.label;
				childMenu = true;
			}
			menu.menuItemHeight = menu.menuItemHeight || defaultHeight;
			var itemProps = '';
			if( menu.fontFamily != '' ) itemProps += 'font-family:' + menu.fontFamily +';';
			itemProps += 'font-weight:' + menu.fontWeight + ';fontSize:' + menu.fontSize + 'px;';
			if (menu.fontStyle) itemProps += 'font-style:' + menu.fontStyle + ';';
			if (document.all || window.mmIsOpera) 
				itemProps += 'font-size:' + menu.fontSize + 'px;" onmouseover="onMenuItemOver(null,this);" onclick="onMenuItemAction(null,this);';
			else if (!document.layers) {
				itemProps += 'font-size:' + menu.fontSize + 'px;';
			}
			var l;
			if (lrs) {
				var lw = menu.menuWidth;
				if( menu.menuItemHAlign == 'right' ) lw -= menu.menuItemPadding;
				l = new Layer(lw,body);
			}
			var itemLeft = 0;
			var itemTop = i*menu.menuItemHeight;
			if( !menu.vertical ) {
				itemLeft = i*menu.menuWidth;
				itemTop = 0;
			}
			var dTag = '<div id="menuItem'+ countItems +'" style="position:absolute;left:' + itemLeft + 'px;top:'+ itemTop +'px;'+ itemProps +'">';
			var dClose = '</div>'
			if (menu.bgImageUp) dTag = '<div id="menuItem'+ countItems +'" style="background:url('+menu.bgImageUp+');position:absolute;left:' + itemLeft + 'px;top:'+ itemTop +'px;'+ itemProps +'">';

			var left = 0, top = 0, right = 0, bottom = 0;
			left = 1 + menu.menuItemPadding + menu.menuItemIndent;
			right = left + menu.menuWidth - 2*menu.menuItemPadding - menu.menuItemIndent;
			if( menu.menuItemVAlign == 'top' ) top = menu.menuItemPadding;
			if( menu.menuItemVAlign == 'bottom' ) top = menu.menuItemHeight-menu.fontSize-1-menu.menuItemPadding;
			if( menu.menuItemVAlign == 'middle' ) top = ((menu.menuItemHeight/2)-(menu.fontSize/2)-1);
			bottom = menu.menuItemHeight - 2*menu.menuItemPadding;
			var textProps = 'position:absolute;left:' + left + 'px;top:' + top + 'px;';
			if (lrs) {
				textProps +=itemProps + 'right:' + right + ';bottom:' + bottom + ';';
				dTag = "";
				dClose = "";
			}
			
			if(document.all && !window.mmIsOpera) {
				item = '<div align="' + menu.menuItemHAlign + '">' + item + '</div>';
			} else if (lrs) {
				item = '<div style="text-align:' + menu.menuItemHAlign + ';">' + item + '</div>';
			} else {
				var hitem = null;
				if( menu.menuItemHAlign != 'left' ) {
					if(window.mmIsOpera) {
						var operaWidth = menu.menuItemHAlign == 'center' ? -(menu.menuWidth-2*menu.menuItemPadding) : (menu.menuWidth-6*menu.menuItemPadding);
						hitem = '<div id="menuItemHilite' + countItems + 'Shim" style="position:absolute;top:1px;left:' + menu.menuItemPadding + 'px;width:' + operaWidth + 'px;text-align:' 
							+ menu.menuItemHAlign + ';visibility:visible;">' + item + '</div>';
						item = '<div id="menuItemText' + countItems + 'Shim" style="position:absolute;top:1px;left:' + menu.menuItemPadding + 'px;width:' + operaWidth + 'px;text-align:' 
							+ menu.menuItemHAlign + ';visibility:visible;">' + item + '</div>';
					} else {
						hitem = '<div id="menuItemHilite' + countItems + 'Shim" style="position:absolute;top:1px;left:1px;right:-' + (left+menu.menuWidth-3*menu.menuItemPadding) + 'px;text-align:' 
							+ menu.menuItemHAlign + ';visibility:visible;">' + item + '</div>';
						item = '<div id="menuItemText' + countItems + 'Shim" style="position:absolute;top:1px;left:1px;right:-' + (left+menu.menuWidth-3*menu.menuItemPadding) + 'px;text-align:' 
							+ menu.menuItemHAlign + ';visibility:visible;">' + item + '</div>';
					}
				} else hitem = null;
			}
			if(document.all && !window.mmIsOpera) item = '<div id="menuItemShim' + countItems + '" style="position:absolute;left:0px;top:0px;">' + item + '</div>';
			var dText	= '<div id="menuItemText'+ countItems +'" style="' + textProps + 'color:'+ menu.fontColor +';">'+ item +'&nbsp</div>\n'
						+ '<div id="menuItemHilite'+ countItems +'" style="' + textProps + 'color:'+ menu.fontColorHilite +';visibility:hidden;">' 
						+ (hitem||item) +'&nbsp</div>';
			var imgHtml = "";
			if (menu.childMenuIcon != "") imgHtml = '<img src="'+ menu.childMenuIcon +'">';
			if (childMenu) content += ( dTag + dText + '<div id="childMenu'+ countItems +'" style="position:absolute;left:0px;top:3px;">'+ imgHtml +'</div>\n' + dClose);
			else content += ( dTag + dText + dClose);
			if (lrs) {
				l.document.open("text/html");
				l.document.writeln(content);
				l.document.close();	
				content = '';
				theStat += "-";
				tsc++;
				if (tsc > 50) {
					tsc = 0;
					theStat = "";
				}
				status = theStat;
			}
			countItems++;  
		}
		if (lrs) {
			var focusItem = new Layer(100, body);
			focusItem.visiblity="hidden";
			focusItem.document.open("text/html");
			focusItem.document.writeln("&nbsp;");
			focusItem.document.close();	
		} else {
		  content += '	  <div id="focusItem'+ countMenus +'" style="position:absolute;left:0px;top:0px;visibility:hide;" onclick="onMenuItemAction(null,this);">&nbsp;</div>\n';
		  content += '   </div>\n  </div>\n</div>\n';
		}
		i=x;
	}
	if (document.layers) {		
		container.clip.width = window.innerWidth;
		container.clip.height = window.innerHeight;
		container.onmouseout = mouseoutMenu;
		container.menuContainerBgColor = this.menuContainerBgColor;
		for (var i=0; i<container.document.layers.length; i++) {
			proto = container.menus[i];
			var menu = container.document.layers[i];
			container.menus[i].menuLayer = menu;
			container.menus[i].menuLayer.Menu = container.menus[i];
			container.menus[i].menuLayer.Menu.container = container;
			var body = menu.document.layers[0].document.layers[0];
			body.clip.width = proto.menuWidth || body.clip.width;
			body.clip.height = proto.menuHeight || body.clip.height;
			for (var n=0; n<body.document.layers.length-1; n++) {
				var l = body.document.layers[n];
				l.Menu = container.menus[i];
				l.menuHiliteBgColor = proto.menuHiliteBgColor;
				l.document.bgColor = proto.menuItemBgColor;
				l.saveColor = proto.menuItemBgColor;
				l.onmouseover = proto.onMenuItemOver;
				l.onclick = proto.onMenuItemAction;
				l.mmaction = container.menus[i].actions[n];
				l.focusItem = body.document.layers[body.document.layers.length-1];
				l.clip.width = proto.menuWidth || body.clip.width;
				l.clip.height = proto.menuItemHeight || l.clip.height;
				if (n>0) {
					if( l.Menu.vertical ) l.top = body.document.layers[n-1].top + body.document.layers[n-1].clip.height + proto.menuItemBorder + proto.menuItemSpacing;
					else l.left = body.document.layers[n-1].left + body.document.layers[n-1].clip.width + proto.menuItemBorder + proto.menuItemSpacing;
				}
				l.hilite = l.document.layers[1];
				if (proto.bgImageUp) l.background.src = proto.bgImageUp;
				l.document.layers[1].isHilite = true;
				if (l.document.layers.length > 2) {
					l.childMenu = container.menus[i].items[n].menuLayer;
					l.document.layers[2].left = l.clip.width -13;
					l.document.layers[2].top = (l.clip.height / 2) -4;
					l.document.layers[2].clip.left += 3;
					l.Menu.childMenus[l.Menu.childMenus.length] = l.childMenu;
				}
			}
			if( proto.menuBgOpaque ) body.document.bgColor = proto.bgColor;
			if( proto.vertical ) {
				body.clip.width  = l.clip.width +proto.menuBorder;
				body.clip.height = l.top + l.clip.height +proto.menuBorder;
			} else {
				body.clip.height  = l.clip.height +proto.menuBorder;
				body.clip.width = l.left + l.clip.width  +proto.menuBorder;
				if( body.clip.width > window.innerWidth ) body.clip.width = window.innerWidth;
			}
			var focusItem = body.document.layers[n];
			focusItem.clip.width = body.clip.width;
			focusItem.Menu = l.Menu;
			focusItem.top = -30;
            focusItem.captureEvents(Event.MOUSEDOWN);
            focusItem.onmousedown = onMenuItemDown;
			if( proto.menuBgOpaque ) menu.document.bgColor = proto.menuBorderBgColor;
			var lite = menu.document.layers[0];
			if( proto.menuBgOpaque ) lite.document.bgColor = proto.menuLiteBgColor;
			lite.clip.width = body.clip.width +1;
			lite.clip.height = body.clip.height +1;
			menu.clip.width = body.clip.width + (proto.menuBorder * 3) ;
			menu.clip.height = body.clip.height + (proto.menuBorder * 3);
		}
	} else {
		if ((!document.all) && (container.hasChildNodes) && !window.mmIsOpera) {
			container.innerHTML=content;
		} else {
			container.document.open("text/html");
			container.document.writeln(content);
			container.document.close();	
		}
		if (!FIND("menuLayer0")) return;
		var menuCount = 0;
		for (var x=0; x<container.menus.length; x++) {
			var menuLayer = FIND("menuLayer" + x);
			container.menus[x].menuLayer = "menuLayer" + x;
			menuLayer.Menu = container.menus[x];
			menuLayer.Menu.container = "menuLayer" + x;
			menuLayer.style.zindex = 1;
		    var s = menuLayer.style;
			s.pixeltop = -300;
			s.pixelleft = -300;
			s.top = '-300px';
			s.left = '-300px';

			var menu = container.menus[x];
			menu.menuItemWidth = menu.menuWidth || menu.menuIEWidth || 140;
			if( menu.menuBgOpaque ) menuLayer.style.backgroundColor = menu.menuBorderBgColor;
			var top = 0;
			var left = 0;
			menu.menuItemLayers = new Array();
			for (var i=0; i<container.menus[x].items.length; i++) {
				var l = FIND("menuItem" + menuCount);
				l.Menu = container.menus[x];
				l.Menu.menuItemLayers[l.Menu.menuItemLayers.length] = l;
				if (l.addEventListener || window.mmIsOpera) {
					l.style.width = menu.menuItemWidth + 'px';
					l.style.height = menu.menuItemHeight + 'px';
					l.style.pixelWidth = menu.menuItemWidth;
					l.style.pixelHeight = menu.menuItemHeight;
					l.style.top = top + 'px';
					l.style.left = left + 'px';
					if(l.addEventListener) {
						l.addEventListener("mouseover", onMenuItemOver, false);
						l.addEventListener("click", onMenuItemAction, false);
						l.addEventListener("mouseout", mouseoutMenu, false);
					}
					if( menu.menuItemHAlign != 'left' ) {
						l.hiliteShim = FIND("menuItemHilite" + menuCount + "Shim");
						l.hiliteShim.style.visibility = "inherit";
						l.textShim = FIND("menuItemText" + menuCount + "Shim");
						l.hiliteShim.style.pixelWidth = menu.menuItemWidth - 2*menu.menuItemPadding - menu.menuItemIndent;
						l.hiliteShim.style.width = l.hiliteShim.style.pixelWidth;
						l.textShim.style.pixelWidth = menu.menuItemWidth - 2*menu.menuItemPadding - menu.menuItemIndent;
						l.textShim.style.width = l.textShim.style.pixelWidth;	
					}
				} else {
					l.style.pixelWidth = menu.menuItemWidth;
					l.style.pixelHeight = menu.menuItemHeight;
					l.style.pixelTop = top;
					l.style.pixelLeft = left;
					if( menu.menuItemHAlign != 'left' ) {
						var shim = FIND("menuItemShim" + menuCount);
						shim[0].style.pixelWidth = menu.menuItemWidth - 2*menu.menuItemPadding - menu.menuItemIndent;
						shim[1].style.pixelWidth = menu.menuItemWidth - 2*menu.menuItemPadding - menu.menuItemIndent;
						shim[0].style.width = shim[0].style.pixelWidth + 'px';
						shim[1].style.width = shim[1].style.pixelWidth + 'px';
					}
				}
				if( menu.vertical ) top = top + menu.menuItemHeight+menu.menuItemBorder+menu.menuItemSpacing;
				else left = left + menu.menuItemWidth+menu.menuItemBorder+menu.menuItemSpacing;
				l.style.fontSize = menu.fontSize + 'px';
				l.style.backgroundColor = menu.menuItemBgColor;
				l.style.visibility = "inherit";
				l.saveColor = menu.menuItemBgColor;
				l.menuHiliteBgColor = menu.menuHiliteBgColor;
				l.mmaction = container.menus[x].actions[i];
				l.hilite = FIND("menuItemHilite" + menuCount);
				l.focusItem = FIND("focusItem" + x);
				l.focusItem.style.pixelTop = -30;
				l.focusItem.style.top = '-30px';
				var childItem = FIND("childMenu" + menuCount);
				if (childItem) {
					l.childMenu = container.menus[x].items[i].menuLayer;
					childItem.style.pixelLeft = menu.menuItemWidth -11;
					childItem.style.left = childItem.style.pixelLeft + 'px';
					childItem.style.pixelTop = (menu.menuItemHeight /2) -4;
					childItem.style.top = childItem.style.pixelTop + 'px';
					l.Menu.childMenus[l.Menu.childMenus.length] = l.childMenu;
				}
				l.style.cursor = "hand";
				menuCount++;
			}
			if( menu.vertical ) {
				menu.menuHeight = top-1-menu.menuItemSpacing;
				menu.menuWidth = menu.menuItemWidth;
			} else {
				menu.menuHeight = menu.menuItemHeight;
				menu.menuWidth = left-1-menu.menuItemSpacing;
			}

			var lite = FIND("menuLite" + x);
			var s = lite.style;
			s.pixelHeight = menu.menuHeight +(menu.menuBorder * 2);
			s.height = s.pixelHeight + 'px';
			s.pixelWidth = menu.menuWidth + (menu.menuBorder * 2);
			s.width = s.pixelWidth + 'px';
			if( menu.menuBgOpaque ) s.backgroundColor = menu.menuLiteBgColor;

			var body = FIND("menuFg" + x);
			s = body.style;
			s.pixelHeight = menu.menuHeight + menu.menuBorder;
			s.height = s.pixelHeight + 'px';
			s.pixelWidth = menu.menuWidth + menu.menuBorder;
			s.width = s.pixelWidth + 'px';
			if( menu.menuBgOpaque ) s.backgroundColor = menu.bgColor;

			s = menuLayer.style;
			s.pixelWidth  = menu.menuWidth + (menu.menuBorder * 4);
			s.width = s.pixelWidth + 'px';
			s.pixelHeight  = menu.menuHeight+(menu.menuBorder*4);
			s.height = s.pixelHeight + 'px';
		}
	}
	if (document.captureEvents) document.captureEvents(Event.MOUSEUP);
	if (document.addEventListener) document.addEventListener("mouseup", onMenuItemOver, false);
	if (document.layers && window.innerWidth) {
		window.onresize = NS4resize;
		window.NS4sIW = window.innerWidth;
		window.NS4sIH = window.innerHeight;
		setTimeout("NS4resize()",500);
	}
	document.onmouseup = mouseupMenu;
	window.mmWroteMenu = true;
	status = "";
}

function NS4resize() {
	if (NS4sIW != window.innerWidth || NS4sIH != window.innerHeight) window.location.reload();
}

function onMenuItemOver(e, l) {
	MM_clearTimeout();
	l = l || this;
	var a = window.ActiveMenuItem;
	if (document.layers) {
		if (a) {
			a.document.bgColor = a.saveColor;
			if (a.hilite) a.hilite.visibility = "hidden";
			if (a.Menu.bgImageOver) a.background.src = a.Menu.bgImageUp;
			a.focusItem.top = -100;
			a.clicked = false;
		}
		if (l.hilite) {
			l.document.bgColor = l.menuHiliteBgColor;
			l.zIndex = 1;
			l.hilite.visibility = "inherit";
			l.hilite.zIndex = 2;
			l.document.layers[1].zIndex = 1;
			l.focusItem.zIndex = this.zIndex +2;
		}
		if (l.Menu.bgImageOver) l.background.src = l.Menu.bgImageOver;
		l.focusItem.top = this.top;
		l.focusItem.left = this.left;
		l.focusItem.clip.width = l.clip.width;
		l.focusItem.clip.height = l.clip.height;
		l.Menu.hideChildMenu(l);
	} else if (l.style && l.Menu) {
		if (a) {
			a.style.backgroundColor = a.saveColor;
			if (a.hilite) a.hilite.style.visibility = "hidden";
			if (a.hiliteShim) a.hiliteShim.style.visibility = "inherit";
			if (a.Menu.bgImageUp) a.style.background = "url(" + a.Menu.bgImageUp +")";;
		} 
		l.style.backgroundColor = l.menuHiliteBgColor;
		l.zIndex = 1;
		if (l.Menu.bgImageOver) l.style.background = "url(" + l.Menu.bgImageOver +")";
		if (l.hilite) {
			l.hilite.style.visibility = "inherit";
			if( l.hiliteShim ) l.hiliteShim.style.visibility = "visible";
		}
		l.focusItem.style.pixelTop = l.style.pixelTop;
		l.focusItem.style.top = l.focusItem.style.pixelTop + 'px';
		l.focusItem.style.pixelLeft = l.style.pixelLeft;
		l.focusItem.style.left = l.focusItem.style.pixelLeft + 'px';
		l.focusItem.style.zIndex = l.zIndex +1;
		l.Menu.hideChildMenu(l);
	} else return;
	window.ActiveMenuItem = l;
}

function onMenuItemAction(e, l) {
	l = window.ActiveMenuItem;
	if (!l) return;
	hideActiveMenus();
	if (l.mmaction) eval("" + l.mmaction);
	window.ActiveMenuItem = 0;
}

function MM_clearTimeout() {
	if (mmHideMenuTimer) clearTimeout(mmHideMenuTimer);
	mmHideMenuTimer = null;
	mmDHFlag = false;
}

function MM_startTimeout() {
	if( window.ActiveMenu ) {
		mmStart = new Date();
		mmDHFlag = true;
		mmHideMenuTimer = setTimeout("mmDoHide()", window.ActiveMenu.Menu.hideTimeout);
	}
}

function mmDoHide() {
	if (!mmDHFlag || !window.ActiveMenu) return;
	var elapsed = new Date() - mmStart;
	var timeout = window.ActiveMenu.Menu.hideTimeout;
	if (elapsed < timeout) {
		mmHideMenuTimer = setTimeout("mmDoHide()", timeout+100-elapsed);
		return;
	}
	mmDHFlag = false;
	hideActiveMenus();
	window.ActiveMenuItem = 0;
}

function MM_showMenu(menu, x, y, child, imgname) {
	if (!window.mmWroteMenu) return;
	MM_clearTimeout();
	if (menu) {
		var obj = FIND(imgname) || document.images[imgname] || document.links[imgname] || document.anchors[imgname];
		x = moveXbySlicePos (x, obj);
		y = moveYbySlicePos (y, obj);
	}
	if (document.layers) {
		if (menu) {
			var l = menu.menuLayer || menu;
			l.top = l.left = 1;
			hideActiveMenus();
			if (this.visibility) l = this;
			window.ActiveMenu = l;
		} else {
			var l = child;
		}
		if (!l) return;
		for (var i=0; i<l.layers.length; i++) { 			   
			if (!l.layers[i].isHilite) l.layers[i].visibility = "inherit";
			if (l.layers[i].document.layers.length > 0) MM_showMenu(null, "relative", "relative", l.layers[i]);
		}
		if (l.parentLayer) {
			if (x != "relative") l.parentLayer.left = x || window.pageX || 0;
			if (l.parentLayer.left + l.clip.width > window.innerWidth) l.parentLayer.left -= (l.parentLayer.left + l.clip.width - window.innerWidth);
			if (y != "relative") l.parentLayer.top = y || window.pageY || 0;
			if (l.parentLayer.isContainer) {
				l.Menu.xOffset = window.pageXOffset;
				l.Menu.yOffset = window.pageYOffset;
				l.parentLayer.clip.width = window.ActiveMenu.clip.width +2;
				l.parentLayer.clip.height = window.ActiveMenu.clip.height +2;
				if (l.parentLayer.menuContainerBgColor && l.Menu.menuBgOpaque ) l.parentLayer.document.bgColor = l.parentLayer.menuContainerBgColor;
			}
		}
		l.visibility = "inherit";
		if (l.Menu) l.Menu.container.visibility = "inherit";
	} else if (FIND("menuItem0")) {
		var l = menu.menuLayer || menu;	
		hideActiveMenus();
		if (typeof(l) == "string") l = FIND(l);
		window.ActiveMenu = l;
		var s = l.style;
		s.visibility = "inherit";
		if (x != "relative") {
			s.pixelLeft = x || (window.pageX + document.body.scrollLeft) || 0;
			s.left = s.pixelLeft + 'px';
		}
		if (y != "relative") {
			s.pixelTop = y || (window.pageY + document.body.scrollTop) || 0;
			s.top = s.pixelTop + 'px';
		}
		l.Menu.xOffset = document.body.scrollLeft;
		l.Menu.yOffset = document.body.scrollTop;
	}
	if (menu) window.activeMenus[window.activeMenus.length] = l;
	MM_clearTimeout();
}

function onMenuItemDown(e, l) {
	var a = window.ActiveMenuItem;
	if (document.layers && a) {
		a.eX = e.pageX;
		a.eY = e.pageY;
		a.clicked = true;
    }
}

function mouseupMenu(e) {
	hideMenu(true, e);
	hideActiveMenus();
	return true;
}

function getExplorerVersion() {
	var ieVers = parseFloat(navigator.appVersion);
	if( navigator.appName != 'Microsoft Internet Explorer' ) return ieVers;
	var tempVers = navigator.appVersion;
	var i = tempVers.indexOf( 'MSIE ' );
	if( i >= 0 ) {
		tempVers = tempVers.substring( i+5 );
		ieVers = parseFloat( tempVers ); 
	}
	return ieVers;
}

function mouseoutMenu() {
	if ((navigator.appName == "Microsoft Internet Explorer") && (getExplorerVersion() < 4.5))
		return true;
	hideMenu(false, false);
	return true;
}

function hideMenu(mouseup, e) {
	var a = window.ActiveMenuItem;
	if (a && document.layers) {
		a.document.bgColor = a.saveColor;
		a.focusItem.top = -30;
		if (a.hilite) a.hilite.visibility = "hidden";
		if (mouseup && a.mmaction && a.clicked && window.ActiveMenu) {
 			if (a.eX <= e.pageX+15 && a.eX >= e.pageX-15 && a.eY <= e.pageY+10 && a.eY >= e.pageY-10) {
				setTimeout('window.ActiveMenu.Menu.onMenuItemAction();', 500);
			}
		}
		a.clicked = false;
		if (a.Menu.bgImageOver) a.background.src = a.Menu.bgImageUp;
	} else if (window.ActiveMenu && FIND("menuItem0")) {
		if (a) {
			a.style.backgroundColor = a.saveColor;
			if (a.hilite) a.hilite.style.visibility = "hidden";
			if (a.hiliteShim) a.hiliteShim.style.visibility = "inherit";
			if (a.Menu.bgImageUp) a.style.background = "url(" + a.Menu.bgImageUp +")";
		}
	}
	if (!mouseup && window.ActiveMenu) {
		if (window.ActiveMenu.Menu) {
			if (window.ActiveMenu.Menu.hideOnMouseOut) MM_startTimeout();
			return(true);
		}
	}
	return(true);
}

function hideChildMenu(hcmLayer) {
	MM_clearTimeout();
	var l = hcmLayer;
	for (var i=0; i < l.Menu.childMenus.length; i++) {
		var theLayer = l.Menu.childMenus[i];
		if (document.layers) theLayer.visibility = "hidden";
		else {
			theLayer = FIND(theLayer);
			theLayer.style.visibility = "hidden";
			if( theLayer.Menu.menuItemHAlign != 'left' ) {
				for(var j = 0; j < theLayer.Menu.menuItemLayers.length; j++) {
					var itemLayer = theLayer.Menu.menuItemLayers[j];
					if(itemLayer.textShim) itemLayer.textShim.style.visibility = "inherit";
				}
			}
		}
		theLayer.Menu.hideChildMenu(theLayer);
	}
	if (l.childMenu) {
		var childMenu = l.childMenu;
		if (document.layers) {
			l.Menu.MM_showMenu(null,null,null,childMenu.layers[0]);
			childMenu.zIndex = l.parentLayer.zIndex +1;
			childMenu.top = l.Menu.menuLayer.top + l.Menu.submenuYOffset;
			if( l.Menu.vertical ) {
				if( l.Menu.submenuRelativeToItem ) childMenu.top += l.top + l.parentLayer.top;
				childMenu.left = l.parentLayer.left + l.parentLayer.clip.width - (2*l.Menu.menuBorder) + l.Menu.menuLayer.left + l.Menu.submenuXOffset;
			} else {
				childMenu.top += l.top + l.parentLayer.top;	
				if( l.Menu.submenuRelativeToItem ) childMenu.left = l.Menu.menuLayer.left + l.left + l.clip.width + (2*l.Menu.menuBorder) + l.Menu.submenuXOffset;
				else childMenu.left = l.parentLayer.left + l.parentLayer.clip.width - (2*l.Menu.menuBorder) + l.Menu.menuLayer.left + l.Menu.submenuXOffset;
			}
			if( childMenu.left < l.Menu.container.clip.left ) l.Menu.container.clip.left = childMenu.left;
			var w = childMenu.clip.width+childMenu.left-l.Menu.container.clip.left;
			if (w > l.Menu.container.clip.width)  l.Menu.container.clip.width = w;
			var h = childMenu.clip.height+childMenu.top-l.Menu.container.clip.top;
			if (h > l.Menu.container.clip.height) l.Menu.container.clip.height = h;
			l.document.layers[1].zIndex = 0;
			childMenu.visibility = "inherit";
		} else if (FIND("menuItem0")) {
			childMenu = FIND(l.childMenu);
			var menuLayer = FIND(l.Menu.menuLayer);
			var s = childMenu.style;
			s.zIndex = menuLayer.style.zIndex+1;
			if (document.all || window.mmIsOpera) {
				s.pixelTop = menuLayer.style.pixelTop + l.Menu.submenuYOffset;
				if( l.Menu.vertical ) {
					if( l.Menu.submenuRelativeToItem ) s.pixelTop += l.style.pixelTop;
					s.pixelLeft = l.style.pixelWidth + menuLayer.style.pixelLeft + l.Menu.submenuXOffset;
					s.left = s.pixelLeft + 'px';
				} else {
					s.pixelTop += l.style.pixelTop;
					if( l.Menu.submenuRelativeToItem ) s.pixelLeft = menuLayer.style.pixelLeft + l.style.pixelLeft + l.style.pixelWidth + (2*l.Menu.menuBorder) + l.Menu.submenuXOffset;
					else s.pixelLeft = (menuLayer.style.pixelWidth-4*l.Menu.menuBorder) + menuLayer.style.pixelLeft + l.Menu.submenuXOffset;
					s.left = s.pixelLeft + 'px';
				}
			} else {
				var top = parseInt(menuLayer.style.top) + l.Menu.submenuYOffset;
				var left = 0;
				if( l.Menu.vertical ) {
					if( l.Menu.submenuRelativeToItem ) top += parseInt(l.style.top);
					left = (parseInt(menuLayer.style.width)-4*l.Menu.menuBorder) + parseInt(menuLayer.style.left) + l.Menu.submenuXOffset;
				} else {
					top += parseInt(l.style.top);
					if( l.Menu.submenuRelativeToItem ) left = parseInt(menuLayer.style.left) + parseInt(l.style.left) + parseInt(l.style.width) + (2*l.Menu.menuBorder) + l.Menu.submenuXOffset;
					else left = (parseInt(menuLayer.style.width)-4*l.Menu.menuBorder) + parseInt(menuLayer.style.left) + l.Menu.submenuXOffset;
				}
				s.top = top + 'px';
				s.left = left + 'px';
			}
			childMenu.style.visibility = "inherit";
		} else return;
		window.activeMenus[window.activeMenus.length] = childMenu;
	}
}

function hideActiveMenus() {
	if (!window.activeMenus) return;
	for (var i=0; i < window.activeMenus.length; i++) {
		if (!activeMenus[i]) continue;
		if (activeMenus[i].visibility && activeMenus[i].Menu && !window.mmIsOpera) {
			activeMenus[i].visibility = "hidden";
			activeMenus[i].Menu.container.visibility = "hidden";
			activeMenus[i].Menu.container.clip.left = 0;
		} else if (activeMenus[i].style) {
			var s = activeMenus[i].style;
			s.visibility = "hidden";
			s.left = '-200px';
			s.top = '-200px';
		}
	}
	if (window.ActiveMenuItem) hideMenu(false, false);
	window.activeMenus.length = 0;
}

function moveXbySlicePos (x, img) { 
	if (!document.layers) {
		var onWindows = navigator.platform ? navigator.platform == "Win32" : false;
		var macIE45 = document.all && !onWindows && getExplorerVersion() == 4.5;
		var par = img;
		var lastOffset = 0;
		while(par){
			if( par.leftMargin && ! onWindows ) x += parseInt(par.leftMargin);
			if( (par.offsetLeft != lastOffset) && par.offsetLeft ) x += parseInt(par.offsetLeft);
			if( par.offsetLeft != 0 ) lastOffset = par.offsetLeft;
			par = macIE45 ? par.parentElement : par.offsetParent;
		}
	} else if (img.x) x += img.x;
	return x;
}

function moveYbySlicePos (y, img) {
	if(!document.layers) {
		var onWindows = navigator.platform ? navigator.platform == "Win32" : false;
		var macIE45 = document.all && !onWindows && getExplorerVersion() == 4.5;
		var par = img;
		var lastOffset = 0;
		while(par){
			if( par.topMargin && !onWindows ) y += parseInt(par.topMargin);
			if( (par.offsetTop != lastOffset) && par.offsetTop ) y += parseInt(par.offsetTop);
			if( par.offsetTop != 0 ) lastOffset = par.offsetTop;
			par = macIE45 ? par.parentElement : par.offsetParent;
		}		
	} else if (img.y >= 0) y += img.y;
	return y;
}

