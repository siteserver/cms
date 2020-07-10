
//获取当前打印相关数据
var forSPrint = new Object();

forSPrint.data ={
	artiBodyId 	: "artibody",				//页面内主要内容 node id	
	titleId    	: "artibodyTitle",			//页面标题部分node id	
	pageLogoId 	: "pagelogo",	//页面logo容器的id
	pageWayId	: "lo_links",	//页面路点容器的id
	
	pageLogoHtml: "",	//初始显示的   logo html
	titleHtml	: "",	//初始显示的标题html
	artiBodyHtml: "",	//初始显示的主要内容html
	pageWayHtml : "",	//初始页面路点html
	
	iconUrl		: "/sitefiles/assets/print",
	
	curPageNum 	: 1,	//当前页码
	//相关页的url 从第1页开始
	pageUrl    	: new Array()
}; 

forSPrint.GetPageInitInfo = function() {
	//获取当前页码 和 相关页的url
	var re;
	var hrefPattern;
	var hrefStrArray = new Array();
	var hrefArray = new Array();
	var currentPageNo;

	hrefArray[0] = window.location.href;
	currentPageNo = 1;

	if(document.getElementById("_function_code_page") != null)
	{
		var html = document.getElementById("_function_code_page").innerHTML;

		hrefPattern = /　(<a href=\".*?\">){0,1}\[\d+\](<\/a>){0,1}/ig;

		if(hrefStrArray = html.match(hrefPattern))
		{
			for(var i = 0 ; i < hrefStrArray.length ; i ++)
			{
				var re = new RegExp("<a href=\"(.*?)\">\[[0-9]+\]<\/a>","i");

				if(re.exec(hrefStrArray[i]))
				{
					hrefArray[i] = RegExp.$1;
				}
				else
				{
					hrefArray[i] = window.location.href;
					currentPageNo = (i + 1);
				}
			}
		}
	}

   forSPrint.data.pageUrl = hrefArray;
   forSPrint.data.curPageNum = currentPageNo;

   //获取初始页面路点html
	if(forSPrint.data.pageWayId != '' && document.getElementById(forSPrint.data.pageWayId) != null)
	{
		forSPrint.data.pageWayHtml = document.getElementById(forSPrint.data.pageWayId).innerHTML;
	}

   //获取初始显示的标题html
	if(forSPrint.data.titleId != '' && document.getElementById(forSPrint.data.titleId) != null)
	{
		forSPrint.data.titleHtml = document.getElementById(forSPrint.data.titleId).innerHTML;
	}

   //获取初始显示的主要内容html
	if(forSPrint.data.artiBodyId != '' && document.getElementById(forSPrint.data.artiBodyId) != null)
	{
		forSPrint.data.artiBodyHtml = document.getElementById(forSPrint.data.artiBodyId).innerHTML;
	}

   //获取初始显示的   logo html
	if(forSPrint.data.pageLogoId != '' && document.getElementById(forSPrint.data.pageLogoId) != null)
	{
		forSPrint.data.pageLogoHtml = document.getElementById(forSPrint.data.pageLogoId).innerHTML;
	}

}


forSPrint.Print = function(){
	//获取打印数据
	forSPrint.GetPageInitInfo();

	var info =	"<html>"	+"\n"+
		"<head>"	+"\n"+
		"<meta http-equiv=\"Content-Type\" content=\"text/html; charset=gb2312\" />"	+"\n"+
		"<title></title>"	+"\n"+
		"<meta name=\"Keywords\" content=\",\" />"	+"\n"+
		"<meta name=\"Description\" content=\"\" />"	+"\n"+
		"<style type=\"text/css\" media=\"screen\">"	+"\n"+
		"*{margin:0;padding:0; font-family:\"宋体\";}"	+"\n"+
		"body {background: #FFF;color:#000; font-size:12px;}"	+"\n"+
		"ul,li{list-style:none;}"	+"\n"+
		"ul,p,ol{line-height:20px;}"	+"\n"+
		"select,input{vertical-align:middle;}"	+"\n"+
		"img{border:0;}"	+"\n"+
		".f12{font-size:12px; line-height:20px;}"	+"\n"+
		".f14{font-size:14px;}"	+"\n"+
		"h1{font-size:22px;margin: 0px;border-bottom:1px #7D8388 solid;}"	+"\n"+
		".from_info{ font-size:12px; text-align:center; color:#000;font-weight:normal}"	+"\n"+
		"#PriCon{font-size:14px;width:560px; text-align:left;}"	+"\n"+
		"#PriCon p{ line-height:23px;margin:15px 0;}"	+"\n"+
		"#PriCon img{border:1px #000 solid;}"	+"\n"+
		"#PriCon.noImg img{display:none;}"	+"\n"+
		".medialogo{float:right; height:18px;}"	+"\n"+
		".linkRed01 {text-decoration:underline;color:#000;}" +"\n"+
		".linkRed01 a{text-decoration:underline;color:#000;}" +"\n"+
		"a{text-decoration: underline;color: #000; }"	+"\n"+
		"a:hover{color: #f00;}"	+"\n"+
		".selFZ{padding:0 2px; background:#999; border:1px #6f6f6f solid; text-decoration:none; cursor:default; color:#fff;}"	+"\n"+
		".NselFZ{text-decoration:underlinel; cursor:pointer; padding:0 5px;"	+"\n"+
		".selFZ,.NselFZ{padding-top:2px;}"	+"\n"+
		".yqin {display:inline; height:0px; width:0px; position:absolute; text-align:left;}"	+"\n"+
		".linkRed02 a{text-decoration:none;color:#000;}"	+"\n"+
		".linkRed02 a:hover{color:#f00;}"	+"\n"+
		"</style>"	+"\n"+
		"<!--[if IE]>"	+"\n"+
		"<style type=\"text/css\" media=\"screen\">"	+"\n"+
		"#printDialog { filter:progid:DXImageTransform.Microsoft.Alpha(opacity=100); filter:progid:DXImageTransform.Microsoft.dropshadow(OffX=3, OffY=3,Color=\"#e0e0e0\", Positive=\"true\");}"	+"\n"+
		"</style>"	+"\n"+
		"<![endif]-->"	+"\n"+
		"<style type=\"text/css\" media=\"print\">"	+"\n"+
		"*{margin:0;padding:0; font-family:\"宋体\";}"	+"\n"+
		"body {background: #FFF;color:#000; font-size:12px;}"	+"\n"+
		"ul,li{list-style:none;}"	+"\n"+
		"ul,p,ol{line-height:20px;}"	+"\n"+
		"select,input{vertical-align:middle;}"	+"\n"+
		"img{border:0;}"	+"\n"+
		"h1{font-size:22px;margin: 0px;border-bottom:1px #7D8388 solid;}"	+"\n"+
		".f12{font-size:12px; line-height:20px;}"	+"\n"+
		".f14{font-size:14px;}"	+"\n"+
		"a{text-decoration: underline;color: #000}"	+"\n"+
		"a:hover{color: #f00;}"	+"\n"+
		".selFZ{padding:0 2px; background:#999; border:1px #6f6f6f solid; text-decoration:none; cursor:default; color:#fff;}"	+"\n"+
		".NselFZ{text-decoration:underlinel; cursor:pointer; padding:0 5px;}"	+"\n"+
		".selFZ,.NselFZ{padding-top:2px;}"	+"\n"+
		"#PriCon{font-size:14px;width:560px;text-align:left;}"	+"\n"+
		"#PriCon p{line-height:23px;margin:15px 0;}"	+"\n"+
		"#PriCon img{border:1px #000 solid;}"	+"\n"+
		"#PriCon.noImg img{display:none;}"	+"\n"+
		".medialogo{float:right; height:18px;}"	+"\n"+
		".linkRed01 {text-decoration:underline;color:#000;}" +"\n"+
		".linkRed01 a{text-decoration:underline;color:#000;}" +"\n"+
		".yqin {display:inline; height:0px; width:0px; position:absolute; text-align:left;}"	+"\n"+
		".from_info{ font-size:12px; text-align:center; color:#000;font-weight:normal}"	+"\n"+
		".linkRed02 a{text-decoration:none;color:#000;}"	+"\n"+
		".linkRed02 a:hover{color:#f00;}"	+"\n"+
		"#NoPrinter1,#NoPrinter2,#NoPrinter3,#NoPrinter4{display:none;}"	+"\n"+
		"#printDialog { display:none; visibility:hidden;}"	+"\n"+
		"#allIframe { display:none;}"	+"\n"+
		"</style>"	+"\n"+
		"<"+"script language=\"JavaScript\" type=\"text/javascript\">"	+"\n"+
		"<!--//--><![CDATA[//><!--"	+"\n"+
		"var sPrint = new Object();"	+"\n"+
		"sPrint.data ={"	+"\n"+
		"	artiBodyId 	: \""+forSPrint.data.artiBodyId+"\",	//页面内主要内容 node id	"	+"\n"+
		"	titleId    	: \""+forSPrint.data.titleId+"\",		//页面标题部分node id	"	+"\n"+
		"	pageLogoId 	: \""+forSPrint.data.pageLogoId+"\",	//页面logo容器的id"	+"\n"+
		"	pageWayId	: \""+forSPrint.data.pageWayId+"\",		//页面路点容器的id"	+"\n"+
		"	"	+"\n"+
		"	pageLogoHtml: \""+escape(forSPrint.data.pageLogoHtml)+"\",	//初始显示的   logo html"	+"\n"+
		"	titleHtml	: \""+escape(forSPrint.data.titleHtml)+"\",		//初始显示的标题html"	+"\n"+
		"	artiBodyHtml: \""+escape(forSPrint.data.artiBodyHtml)+"\",	//初始显示的主要内容html"	+"\n"+
		"	pageWayHtml : \""+escape(forSPrint.data.pageWayHtml)+"\",	//初始页面路点html"	+"\n"+
		"	"	+"\n"+
		"	iconUrl		: \""+escape(forSPrint.data.iconUrl)+"\",	//icon地址"	+"\n"+
		"	"	+"\n"+
		"	curPageNum 	: "+forSPrint.data.curPageNum+",				//当前页码"	+"\n"+
		"	//相关页的url 从第1页开始"	+"\n"+
		"	pageUrl    	: new Array("	+"\n";
		var pageTotalTmp = forSPrint.data.pageUrl.length;
		if(pageTotalTmp == 1){
			info += "	\""+forSPrint.data.pageUrl[0]+"\""	+"\n"
		}else if(pageTotalTmp > 1){
			for(var i = 0; i < pageTotalTmp - 1; i++){
				info += "	\""+forSPrint.data.pageUrl[i]+"\","	+"\n"
			}
			info += "	\""+forSPrint.data.pageUrl[pageTotalTmp - 1]+"\""	+"\n"
		}else{
			throw new Error("forSPrint.data.pageUrl.length < 1")
		}
		info += "	)"	+"\n"+
		"}; "	+"\n"+
		"//--><!]]>"	+"\n"+
		"</script"+">"	+"\n"+
		"<"+"script language=\"JavaScript\" type=\"text/javascript\">"	+"\n"+
		"<!--//--><![CDATA[//><!--"	+"\n"+
		"var flag=false;\n"+
		"function DrawImage(ImgD){\n"+
		"	var image=new Image();\n"+
		"	var iwidth = 999;\n"+
		"	var iheight = 21;\n"+
		"	image.src=ImgD.src;\n"+
		"	if(image.width>0 && image.height>0){\n"+
		"		flag=true;\n"+
		"		if(image.width/image.height>= iwidth/iheight){\n"+
		"			if(image.width>iwidth){\n"+
		"				ImgD.width=iwidth;\n"+
		"				ImgD.height=(image.height*iwidth)/image.width;\n"+
		"			}else{\n"+
		"				ImgD.width=image.width;\n"+
		"				ImgD.height=image.height;\n"+
		"			}\n"+
		"		}else{\n"+
		"			if(image.height>iheight){\n"+
		"				ImgD.height=iheight;\n"+
		"				ImgD.width=(image.width*iheight)/image.height;\n"+
		"			}else{\n"+
		"				ImgD.width=image.width;\n"+
		"				ImgD.height=image.height;\n"+
		"			}\n"+
		"		}\n"+
		"	}\n"+
		"}\n"+
		"//--><!]]>"	+"\n"+
		"</script"+">"	+"\n"+
		"</head>"	+"\n"+
		"<body>"	+"\n"+
		"<center>"	+"\n"+
		"  <table width=\"600\" cellspacing=\"0\" id=\"NoPrinter1\" style=\"border-bottom:1px #DDD9D3 solid;\">"	+"\n"+
		"    <tr align=\"left\" valign=\"middle\">"	+"\n"+
		"      <td width=\"4\" height=\"36\"></td>"	+"\n"+
		"      <td class=\"f14\" style=\"padding-top:2px;\">此页面为打印预览页<input name=\"printImgCheckbox\" id=\"printImgCheckbox\" type=\"checkbox\" value=\"checkbox\" checked onClick=\"sPrint.printImgCboxChg(this);\" style=\"margin: 0px 2px 4px 10px;\">打印图片</td>"	+"\n"+
		"      <td width=\"70\" class=\"f14\" style=\"padding-top:2px;\">选择字号：</td>"	+"\n"+
		"      <td width=\"115\" class=\"f14\" valign=\"top\"><table cellspacing=\"0\" class=\"f14\" style=\"margin-top:8px;\">"	+"\n"+
		"          <tr align=\"left\" valign=\"middle\">"	+"\n"+
		"            <td class=\"NselFZ\" id=\"topadmenu_0\" onClick=\"sPrint.doPriZoom(20);sPrint.ChgFz(0);\" height=\"20\">超大</td>"	+"\n"+
		"            <td class=\"NselFZ\" id=\"topadmenu_1\" onClick=\"sPrint.doPriZoom(16);sPrint.ChgFz(1);\">大</td>"	+"\n"+
		"            <td class=\"selFZ\" id=\"topadmenu_2\" onClick=\"sPrint.doPriZoom(14);sPrint.ChgFz(2);\">中</td>"	+"\n"+
		"            <td class=\"NselFZ\" id=\"topadmenu_3\" onClick=\"sPrint.doPriZoom(12);sPrint.ChgFz(3);\">小</td>"	+"\n"+
		"          </tr>"	+"\n"+
		"        </table></td>"	+"\n"+
		"      <td width=\"90\"><a href=\"javascript:sPrint.Print();\"><img src=\"" + forSPrint.data.iconUrl + "/print.gif\" width=\"80\" height=\"20\" alt=\"打印此页面\"/></a></td>"	+"\n"+
		"      <td width=\"65\"><a href=\"javascript:sPrint.GoBack()\"><img src=\"" + forSPrint.data.iconUrl + "/return.gif\" width=\"64\" height=\"20\" alt=\"返回正文页\"/></a></td>"	+"\n"+
		"    </tr>"	+"\n"+
		"  </table>"	+"\n"+
		"  <table width=\"600\" cellspacing=\"0\">"	+"\n"+
		"    <tr align=\"center\">"	+"\n"+
		"      <td width=\"1\" style=\"background:#DDD9D3;\" id=\"NoPrinter3\"></td>"	+"\n"+
		"      <td width=\"598\" valign=\"top\"><table width=\"560\" cellspacing=\"0\" style=\"margin-top:7px;\">"	+"\n"+
		"          <tr valign=\"top\">"	+"\n"+
		"            <td width=\"181\" align=\"left\" id=\"pagelogo\"></td>"	+"\n"+
		"            <td style=\"padding-top:12px; line-height:21px;\" class=\"f12\" id=\"lo_links\"></td>"	+"\n"+
		"          </tr>"	+"\n"+
		"        </table>"	+"\n"+
		"        <table cellspacing=\"0\">"	+"\n"+
		"          <tr>"	+"\n"+
		"            <td height=\"34\"></td>"	+"\n"+
		"          </tr>"	+"\n"+
		"        </table>"	+"\n"+
		"        <table cellspacing=\"0\" width=\"560\">"	+"\n"+
		"          <tr align=\"center\">"	+"\n"+
		"            <td style=\"font-size:22px; font-weight:bold; line-height:40px;\" id=\"artibodyTitle\"></td>"	+"\n"+
		"          </tr>"	+"\n"+
		"        </table>"	+"\n"+
		"        <div id=\"PriCon\"></div>"	+"\n"+
		"		<div id=\"WzLy\" style=\"font-size:12px; margin:30px 0 40px 0; padding:0 15px; text-align:left; line-height:18px;\"> 文章来源：<span id=\"artiPath\"></span></div>"	+"\n"+
		"	  </td>"	+"\n"+
		"      <td width=\"1\" style=\"background:#DDD9D3;\" id=\"NoPrinter4\"></td>"	+"\n"+
		"    </tr>"	+"\n"+
		"  </table>"	+"\n"+
		"  <table width=\"600\" cellspacing=\"0\" id=\"NoPrinter2\" style=\"border-top:1px #DDD9D3 solid;\">"	+"\n"+
		"    <tr align=\"left\" valign=\"middle\">"	+"\n"+
		"      <td width=\"4\" height=\"36\"></td>"	+"\n"+
		"      <td class=\"f14\" style=\"padding-top:2px;\">此页面为打印预览页<input name=\"printImgCheckbox2\" id=\"printImgCheckbox2\" type=\"checkbox\" value=\"checkbox\" checked onClick=\"sPrint.printImgCboxChg(this);\" style=\"margin: 0px 2px 4px 10px;\">打印图片</td>"	+"\n"+
		"      <td width=\"70\" class=\"f14\" style=\"padding-top:2px;\">选择字号：</td>"	+"\n"+
		"      <td width=\"115\" class=\"f14\" valign=\"top\"><table cellspacing=\"0\" class=\"f14\" style=\"margin-top:8px;\">"	+"\n"+
		"          <tr align=\"left\" valign=\"middle\">"	+"\n"+
		"            <td class=\"NselFZ\" id=\"topadmenu2_0\" onClick=\"sPrint.doPriZoom(20);sPrint.ChgFz2(0);\" height=\"20\">超大</td>"	+"\n"+
		"            <td class=\"NselFZ\" id=\"topadmenu2_1\" onClick=\"sPrint.doPriZoom(16);sPrint.ChgFz2(1);\">大</td>"	+"\n"+
		"            <td class=\"selFZ\" id=\"topadmenu2_2\" onClick=\"sPrint.doPriZoom(14);sPrint.ChgFz2(2);\">中</td>"	+"\n"+
		"            <td class=\"NselFZ\" id=\"topadmenu2_3\" onClick=\"sPrint.doPriZoom(12);sPrint.ChgFz2(3);\">小</td>"	+"\n"+
		"          </tr>"	+"\n"+
		"        </table></td>"	+"\n"+
		"      <td width=\"90\"><a href=\"javascript:sPrint.Print();\"><img src=\"" + forSPrint.data.iconUrl + "/print.gif\" width=\"80\" height=\"20\" alt=\"打印此页面\"/></a></td>"	+"\n"+
		"      <td width=\"65\"><a href=\"javascript:sPrint.GoBack()\"><img src=\"" + forSPrint.data.iconUrl + "/return.gif\" width=\"64\" height=\"20\" alt=\"返回正文页\"/></a></td>"	+"\n"+
		"    </tr>"	+"\n"+
		"  </table>"	+"\n"+
		"</center>"	+"\n"+
		"<!-- print dialog start -->"	+"\n"+
		"<div id=\"printDialog\" style=\"width:250px; display:none; text-align:center; padding:15px 10px 10px 10px; background:#ecebea; border:1px solid #ddd9d3; position:absolute; right:10px; top:30px;\">"	+"\n"+
		"	<div id=\"pageTotalInfo\" style=\"padding:0 0 10px 0\"></div>"	+"\n"+
		"	<div id=\"printBtnSelect\"><input type=\"button\" value=\"打印全部页\" onClick=\"sPrint.PrintAllPage();\" />&nbsp;&nbsp;<input type=\"button\" value=\"打印当前页\" onClick=\"sPrint.PrintCurPage();\" /></div>"	+"\n"+
		"	<div id=\"onloadInfo\" style=\"padding:5px 0 0 10px;\"></div>"	+"\n"+
		"	<div id=\"startPrintDiv\" style=\"display:none; padding:10px 0 0 0;\"><input type=\"button\" value=\"&nbsp;打印&nbsp;\" onClick=\" sPrint.StartPrintAllPage();\" />&nbsp;&nbsp;<input type=\"button\" value=\"取消打印\" onClick=\"sPrint.CancelPrintAllPage();\" /></div>"	+"\n"+
		"	<div style=\"position:absolute; top:5px; right:3px; width:20px; height:15px; cursor:pointer;\" onClick=\"javascript:sPrint.ClosePrintDialog();\"><span onMouseOver=\"this.style.fontWeight = \'bold\'\" onMouseOut=\"this.style.fontWeight=\'normal\'\">×</span></div>"	+"\n"+
		"</div>"	+"\n"+
		"<!-- print dialog end -->"	+"\n"+
		"<!-- iframe start -->"	+"\n"+
		"<div id=\"allIframe\"></div>"	+"\n"+
		"<!-- iframe end -->"	+"\n"+
		"<script language=\"JavaScript\" type=\"text/javascript\">"	+"\n"+
		"sPrint.$ = function(objId){"	+"\n"+
		"	if(document.getElementById){"	+"\n"+
		"		return eval(\'document.getElementById(\"\' + objId + \'\")\');"	+"\n"+
		"	}else if(document.layers){"	+"\n"+
		"		return eval(\"document.layers[\'\" + objId +\"\']\");"	+"\n"+
		"	}else{"	+"\n"+
		"		return eval(\'document.all.\' + objId);"	+"\n"+
		"	}"	+"\n"+
		" }"	+"\n"+
		" "	+"\n"+
		"//============================= 选择字号 start ====================="	+"\n"+
		"sPrint.doPriZoom = function(size){"	+"\n"+
		"	var PriCon = sPrint.$(\"PriCon\");"	+"\n"+
		"	if(!PriCon){"	+"\n"+
		"		return;"	+"\n"+
		"	}"	+"\n"+
		"	var PriConChild = PriCon.childNodes;"	+"\n"+
		"	PriCon.style.fontSize = size + \"px\";"	+"\n"+
		"	for(var i = 0; i < PriConChild.length; i++){"	+"\n"+
		"		if(PriConChild[i].nodeType == 1){"	+"\n"+
		"			PriConChild[i].style.fontSize = size + \"px\";"	+"\n"+
		"			PriConChild[i].style.lineHeight = size + size + \"px\";"	+"\n"+
		"		}"	+"\n"+
		"	}"	+"\n"+
		"	//如果加载完毕 更新需要纸张数"	+"\n"+
		"	if(sPrint.startPrint){"	+"\n"+
		"		sPrint.onloadInfoNode.innerHTML = \'共\' + sPrint.pageTotal +\'页，全部加载完毕，大约需要\' + sPrint.GetNeedPageTotal() + \'页纸\';"	+"\n"+
		"	}"	+"\n"+
		"}"	+"\n"+
		"sPrint.ChgFz = function(FZBtn_num){"	+"\n"+
		"	for(var i=0;i<4;i++){sPrint.$(\"topadmenu_\"+i).className=\"NselFZ\";}"	+"\n"+
		"	sPrint.$(\"topadmenu_\"+FZBtn_num).className=\"selFZ\";"	+"\n"+
		"	for(var i=0;i<4;i++){sPrint.$(\"topadmenu2_\"+i).className=\"NselFZ\";}"	+"\n"+
		"	sPrint.$(\"topadmenu2_\"+FZBtn_num).className=\"selFZ\";"	+"\n"+
		"}"	+"\n"+
		"sPrint.ChgFz2 = function(FZBtn_num2){"	+"\n"+
		"	for(var i=0;i<4;i++){"	+"\n"+
		"		sPrint.$(\"topadmenu2_\"+i).className=\"NselFZ\";"	+"\n"+
		"	}"	+"\n"+
		"	sPrint.$(\"topadmenu2_\"+FZBtn_num2).className=\"selFZ\";"	+"\n"+
		"	for(var i=0;i<4;i++){"	+"\n"+
		"		sPrint.$(\"topadmenu_\"+i).className=\"NselFZ\";"	+"\n"+
		"	}"	+"\n"+
		"	sPrint.$(\"topadmenu_\"+FZBtn_num2).className=\"selFZ\";"	+"\n"+
		"}"	+"\n"+
		"//============================= 选择字号 end ====================="	+"\n"+
		""	+"\n"+
		"//print dialog 滚动"	+"\n"+
		"sPrint.Scroll = function(o){"	+"\n"+
		"	var initTop = parseInt(o.style.top);"	+"\n"+
		"	window.onscroll = function(){"	+"\n"+
		"		o.style.top = initTop + document.body.scrollTop + \"px\";		"	+"\n"+
		"   } "	+"\n"+
		"}"	+"\n"+
		""	+"\n"+
		"//返回断行处理string"	+"\n"+
		"sPrint.BreakWord = function(str,intLen){"	+"\n"+
		"	var strTemp = \"\";"	+"\n"+
		"	if(str.length > intLen){//考虑换行对其"	+"\n"+
		"		strTemp = \"<br />\";"	+"\n"+
		"	}"	+"\n"+
		"	while(str.length > intLen){"	+"\n"+
		"		strTemp += str.substr(0,intLen)+\"<br />\";"	+"\n"+
		"		str = str.substr(intLen,str.length);"	+"\n"+
		"	}"	+"\n"+
		"	return strTemp += str;"	+"\n"+
		"}"	+"\n"+
		""	+"\n"+
		""	+"\n"+
		"//打印所需数据信息 对象"	+"\n"+
		"sPrint.pagelogoNode 	= sPrint.$(\"pagelogo\");				//文章logo node"	+"\n"+
		"sPrint.artibodyTitleNode= sPrint.$(\"artibodyTitle\"); 	//文章标题 node"	+"\n"+
		"sPrint.loLinksNode 		= sPrint.$(\"lo_links\");			//页面路点 node"	+"\n"+
		"sPrint.priConNode 		= sPrint.$(\"PriCon\");			//正文node"	+"\n"+
		"sPrint.artiPathNode 	= sPrint.$(\"artiPath\");			//文章来源 node"	+"\n"+
		"sPrint.pageTotal 		= sPrint.data.pageUrl.length;		//页码总数"	+"\n"+
		"sPrint.printDialog 		= sPrint.$(\"printDialog\");		//打印对话框"	+"\n"+
		"sPrint.onloadInfoNode	= sPrint.$(\"onloadInfo\"); 		//加载信息显示 node"	+"\n"+
		"sPrint.allIframeNode	= sPrint.$(\"allIframe\");		//加载所有页面的 node"	+"\n"+
		"sPrint.onloadNum 		= 0;							//当前开始加载页的索引序列值"	+"\n"+
		"sPrint.startPrint 		= false;						//当前是否已经将所有内容写入此页中 并准备打印  适用月多页打印情况"	+"\n"+
		"sPrint.heightPerPage 	= 800; 							//每页纸可以打印的高度"	+"\n"+
		"sPrint.pageTotalInfoNode = sPrint.$(\"pageTotalInfo\");	//页面总数信息"	+"\n"+
		"sPrint.printBtnSelectNode = sPrint.$(\"printBtnSelect\");	//是否打印选择node"	+"\n"+
		"sPrint.printImgCheckbox	 = sPrint.$(\"printImgCheckbox\");	//是否打印图片 checkbox"	+"\n"+
		"sPrint.printImgCheckbox2 = sPrint.$(\"printImgCheckbox2\");	//是否打印图片 checkbox 底部"	+"\n"+
		"sPrint.printImgOk 		= true;	//默认是否打印图片"	+"\n"+
		"sPrint.imgMaxHeight		= 900;	//图片限制最大高度"	+"\n"+
		""	+"\n"+
		"//打印主函数"	+"\n"+
		"sPrint.Print = function(){"	+"\n"+
		"	//如果单页数"	+"\n"+
		"	if(sPrint.pageTotal == 1){"	+"\n"+
		"		window.print();"	+"\n"+
		"	}else if(sPrint.pageTotal > 1){//如果多页数"	+"\n"+
		"		if(sPrint.onloadNum == 0){	//还未加载其他页 提示多页打印否？"	+"\n"+
		"			//显示共多少页需要打印"	+"\n"+
		"			sPrint.pageTotalInfoNode.innerHTML = \'<p>本文共有\' + sPrint.pageTotal + \'页，您是否全部打印？</p>\';"	+"\n"+
		"			//打开多页打印dialog"	+"\n"+
		"			sPrint.OpenPrintDialog();"	+"\n"+
		"		}else if(sPrint.onloadNum < sPrint.pageTotal){	//如果正在加载页 则提示等待"	+"\n"+
		"			//alert(\"请耐心等待，所有页还未加载完毕.\\n单击确定继续加载\");"	+"\n"+
		"			//打开多页打印dialog"	+"\n"+
		"			sPrint.OpenPrintDialog();"	+"\n"+
		"			return;"	+"\n"+
		"		}else if(sPrint.startPrint){//如果多页已经加载完毕 则直接打印后退出"	+"\n"+
		"			window.print();"	+"\n"+
		"			return;"	+"\n"+
		"		}		"	+"\n"+
		"	}"	+"\n"+
		"}"	+"\n"+
		""	+"\n"+
		"//仅打印当前页"	+"\n"+
		"sPrint.PrintCurPage = function(){"	+"\n"+
		"	//关闭打印dialog"	+"\n"+
		"	sPrint.ClosePrintDialog();	"	+"\n"+
		"	//打印当前页"	+"\n"+
		"	window.print();"	+"\n"+
		"}"	+"\n"+
		""	+"\n"+
		"//全部打印"	+"\n"+
		"sPrint.PrintAllPage = function(){"	+"\n"+
		"	//隐藏打印2个按钮"	+"\n"+
		"	sPrint.printBtnSelectNode.style.display = \"none\";"	+"\n"+
		"	//更改显示页面总数信息"	+"\n"+
		"	sPrint.pageTotalInfoNode.innerHTML = \'<p>本文共有\' + sPrint.pageTotal + \'页需要加载，请您耐心等待</p>\';"	+"\n"+
		"	"	+"\n"+
		"	//加载第一个页面 并显示加载信息"	+"\n"+
		"sPrint.allIframeNode.innerHTML = \'<iframe id=\"iframeForData\" name=\"iframeForData\" src=\"\'+sPrint.data.pageUrl[0]+ \'?t=\' + new Date().getTime() + \'\" width=\"0\" marginwidth=\"0\" height=\"0\" marginheight=\"0\"  scrolling=\"no\" frameborder=\"0\" onload=\"sPrint.OnloadBack();\" onerror=\"return true;\"></iframe>\';"	+"\n"+
		"	sPrint.onloadNum = 1;"	+"\n"+
		"	sPrint.onloadInfoNode.innerHTML = \'第1页正在加载...\';"	+"\n"+
		"}"	+"\n"+
		""	+"\n"+
		"//开始打印所有页面"	+"\n"+
		"sPrint.StartPrintAllPage = function(){	"	+"\n"+
		"	sPrint.ClosePrintDialog();	//关闭dialog	"	+"\n"+
		"	window.print();	//打印"	+"\n"+
		"}"	+"\n"+
		""	+"\n"+
		"//取消全部打印"	+"\n"+
		"sPrint.CancelPrintAllPage = function(){	"	+"\n"+
		"	sPrint.ClosePrintDialog(); //关闭dialog"	+"\n"+
		"}"	+"\n"+
		""	+"\n"+
		"//返回到正文页"	+"\n"+
		"sPrint.GoBack = function (){"	+"\n"+
		"	var steps = -1;"	+"\n"+
		"	if(sPrint.onloadNum == 0){"	+"\n"+
		"	steps = -1;"	+"\n"+
		"	}else{"	+"\n"+
		"		steps = (sPrint.onloadNum) * (-1);"	+"\n"+
		"	}"	+"\n"+
		"	history.go(steps);"	+"\n"+
		"}"	+"\n"+
		""	+"\n"+
		"//所有加载页面的回调函数"	+"\n"+
		"sPrint.OnloadBack = function(){"	+"\n"+
		"	//如果第一个载入后需要做很多 因为多页情况 合起来打印时 需要使用第一页的document.title、标题、 文章来源等信息"	+"\n"+
		"	if(sPrint.onloadNum == 1){"	+"\n"+
		"		//更新document.title"	+"\n"+
		"		//document.title = frames[\'iframeForData\'].document.title;"	+"\n"+
		"		//更新标题"	+"\n"+
		"		sPrint.artibodyTitleNode.innerHTML = frames[\"iframeForData\"].document.getElementById(sPrint.data.titleId).innerHTML;"	+"\n"+
		"		//更新文章来源信息 滤掉参数"	+"\n"+
		"		sPrint.artiPathNode.innerHTML = sPrint.BreakWord(frames[\"iframeForData\"].location.href.replace(/\\?.*/gi,\'\'),90);"	+"\n"+
		"		//清空当前页内容 准备按页陆续载入"	+"\n"+
		"		sPrint.priConNode.innerHTML = \"\";"	+"\n"+
		"	}"	+"\n"+
		""	+"\n"+
		"	//alert(frames[\'iframeForData\'].location +\"///\"+sPrint.onloadNum);	"	+"\n"+
		"	//将该内容插入到页面前进行过滤"	+"\n"+
		"	sPrint.priConNode.innerHTML += sPrint.HandlerBodyHtml(frames[\'iframeForData\'].document.getElementById(sPrint.data.artiBodyId).innerHTML);//iframe 页里也有GetObj 函数"	+"\n"+
		"	//输出加载完毕信息"	+"\n"+
		"	sPrint.onloadInfoNode.innerHTML = \'第\' + sPrint.onloadNum + \'页加载完毕\';"	+"\n"+
		"	//检测是否还有页面需要加载"	+"\n"+
		"	if(sPrint.onloadNum < sPrint.pageTotal){//加载下一页 并显示加载信息"	+"\n"+
		"		sPrint.onloadNum++;"	+"\n"+
		"		frames[\"iframeForData\"].location.href = sPrint.data.pageUrl[sPrint.onloadNum - 1] + \'?t=\' + new Date().getTime();"	+"\n"+
		"		sPrint.onloadInfoNode.innerHTML = \'第\'+(sPrint.onloadNum)+\'页正在加载...\';		"	+"\n"+
		"	}else{//加载完毕 显示加载完毕信息"	+"\n"+
		"		//隐藏 页面总数信息"	+"\n"+
		"		sPrint.pageTotalInfoNode.style.display = \"none\";"	+"\n"+
		"		sPrint.onloadInfoNode.innerHTML = \'共\' + sPrint.pageTotal +\'页，全部加载完毕，大约需要\' + sPrint.GetNeedPageTotal() + \'页纸\';"	+"\n"+
		"		//显示开始打印按钮"	+"\n"+
		"		sPrint.$(\"startPrintDiv\").style.display = \"block\";"	+"\n"+
		"		//标记已经写入完毕"	+"\n"+
		"		sPrint.startPrint = true;"	+"\n"+
		"	}"	+"\n"+
		"}"	+"\n"+
		""	+"\n"+
		"//处理正文html"	+"\n"+
		"sPrint.HandlerBodyHtml = function(str){"	+"\n"+
		"	var strTmp = str;"	+"\n"+
		"	//滤掉 正文内部文字导航"	+"\n"+
		"	strTmp = strTmp.replace(/<!--\\s+正文内部文字导航\\s+:\\s+begin -->[\\s\\S]*<!--\\s+正文内部文字导航\\s+:\\s+end\\s+-->/,\"\");"	+"\n"+
		"	//去除 artibody内打印容器以及以后的所有html内容"	+"\n"+
		"	strTmp = strTmp.replace(/<span\\s+id=[\"\']?_function_code_page[\"\']?>[\\s\\S]*/i, \"\");"	+"\n"+
		"	//过滤掉iask关键字的html标记"	+"\n"+
		"	strTmp = strTmp.replace(/<span\\s+class=.?yqlink.?>[\\s\\S]*?class=.?akey.?\\s+title=[\\s\\S]*?>([\\s\\S]*?)<\\/a><\\/span>/gi, \"$1\");"	+"\n"+
		"	//绑定img onload"	+"\n"+
		"	strTmp = strTmp.replace(/<img/gi,\"<img onload=\\\"sPrint.ResizeImg(this);\\\"\");"	+"\n"+
		"	return strTmp;"	+"\n"+
		"}"	+"\n"+
		""	+"\n"+
		"//重新定义超高图片高度"	+"\n"+
		"sPrint.ResizeImg = function(obj){"	+"\n"+
		"	if(obj.height > sPrint.imgMaxHeight){"	+"\n"+
		"		obj.height = sPrint.imgMaxHeight;"	+"\n"+
		"		obj.style.pageBreakAfter = \"always\";"	+"\n"+
		"	}"	+"\n"+
		"}"	+"\n"+
		""	+"\n"+
		"//打开打印dialog"	+"\n"+
		"sPrint.OpenPrintDialog = function(){"	+"\n"+
		"	sPrint.printDialog.style.display = \"block\";"	+"\n"+
		"}"	+"\n"+
		"//关闭打印dialog"	+"\n"+
		"sPrint.ClosePrintDialog = function(){"	+"\n"+
		"	sPrint.printDialog.style.display = \"none\";"	+"\n"+
		"}"	+"\n"+
		""	+"\n"+
		"//返回打印机需要纸张数"	+"\n"+
		"sPrint.GetNeedPageTotal = function(){"	+"\n"+
		"	var needPageTotal;"	+"\n"+
		"	return needPageTotal = Math.round(sPrint.priConNode.offsetHeight/sPrint.heightPerPage);"	+"\n"+
		"}"	+"\n"+
		""	+"\n"+
		"//是否打印图片"	+"\n"+
		"sPrint.printImgCboxChg = function(obj){"	+"\n"+
		"	if(obj.checked == true){		"	+"\n"+
		"		sPrint.DisplayBodyImg(true);"	+"\n"+
		"	}else{"	+"\n"+
		"		sPrint.DisplayBodyImg(false);"	+"\n"+
		"	}"	+"\n"+
		"}"	+"\n"+
		""	+"\n"+
		"//是否显示正文图片"	+"\n"+
		"sPrint.DisplayBodyImg = function(bol){"	+"\n"+
		"	if(bol){"	+"\n"+
		"		sPrint.printImgCheckbox.checked = true;"	+"\n"+
		"		sPrint.printImgCheckbox2.checked = true;"	+"\n"+
		"		sPrint.priConNode.className = \"\";"	+"\n"+
		"		"	+"\n"+
		"		//显示打印图片前遍历图片 重定义超高图片高度"	+"\n"+
		"		var imgs = sPrint.priConNode.getElementsByTagName(\"img\");"	+"\n"+
		"		for(var i = 0; i < imgs.length; i++){"	+"\n"+
		"			sPrint.ResizeImg(imgs[i]);"	+"\n"+
		"		}"	+"\n"+
		"	}else{"	+"\n"+
		"		sPrint.printImgCheckbox.checked = false;"	+"\n"+
		"		sPrint.printImgCheckbox2.checked = false;"	+"\n"+
		"		sPrint.priConNode.className = \"noImg\";"	+"\n"+
		"	}"	+"\n"+
		"	//如果加载完毕 重新计算打印页数"	+"\n"+
		"	if(sPrint.startPrint){"	+"\n"+
		"		sPrint.onloadInfoNode.innerHTML = \'共\' + sPrint.pageTotal +\'页，全部加载完毕，大约需要\' + sPrint.GetNeedPageTotal() + \'页纸\';	"	+"\n"+
		"	}	"	+"\n"+
		"}"	+"\n"+
		""	+"\n"+
		""	+"\n"+
		"//初始化"	+"\n"+
		"sPrint.Init = function(){"	+"\n"+
		"	document.title = \"\";	//设置当前页title 以免打印出		"	+"\n"+
		"	sPrint.pagelogoNode.innerHTML 	= unescape(sPrint.data.pageLogoHtml);	//显示页面logo"	+"\n"+
		"	sPrint.loLinksNode.innerHTML 	= unescape(sPrint.data.pageWayHtml).replace(/#000099/g,\"#000000\");	//显示页面路点 并使用黑色"	+"\n"+
		"	sPrint.artibodyTitleNode.innerHTML = unescape(sPrint.data.titleHtml);	//显示页面标题"	+"\n"+
		"	sPrint.priConNode.innerHTML 	= sPrint.HandlerBodyHtml(unescape(sPrint.data.artiBodyHtml));	//显示当前页内容"	+"\n"+
		"	sPrint.artiPathNode.innerHTML 	= sPrint.BreakWord(location.href,92);	//显示文章来源url 未考虑断行"	+"\n"+
		"	sPrint.Scroll(sPrint.printDialog);		//printDialog 滚动	"	+"\n"+
		"	//定位printDialog的初始位置"	+"\n"+
		"	//由css定位"	+"\n"+
		"}"	+"\n"+
		"sPrint.Init();"	+"\n"+
		"//根据sPrint.printImgOk 初始化checkbox 解决ie里刷新后记录当前checkbox状态的bug"	+"\n"+
		"window.onload=function(){"	+"\n"+
		"	if(sPrint.printImgOk){"	+"\n"+
		"		sPrint.DisplayBodyImg(true);"	+"\n"+
		"	}else{"	+"\n"+
		"		sPrint.DisplayBodyImg(false);"	+"\n"+
		"	}"	+"\n"+
		"}"	+"\n"+
		"</script>"	+"\n"+
		"</body>"  +"\n"+
		"</html>";
		//document.open();
		document.write(info);
		document.close();
}

//070411修改 ws
//限定媒体logo尺寸
