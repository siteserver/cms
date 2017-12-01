
//��ȡ��ǰ��ӡ��������
var forSPrint = new Object();

forSPrint.data ={
	artiBodyId 	: "artibody",				//ҳ������Ҫ���� node id	
	titleId    	: "artibodyTitle",			//ҳ�����ⲿ��node id	
	pageLogoId 	: "pagelogo",	//ҳ��logo������id
	pageWayId	: "lo_links",	//ҳ��·��������id
	
	pageLogoHtml: "",	//��ʼ��ʾ��   logo html
	titleHtml	: "",	//��ʼ��ʾ�ı���html
	artiBodyHtml: "",	//��ʼ��ʾ����Ҫ����html
	pageWayHtml : "",	//��ʼҳ��·��html
	
	iconUrl		: "/sitefiles/assets/print",
	
	curPageNum 	: 1,	//��ǰҳ��
	//����ҳ��url �ӵ�1ҳ��ʼ
	pageUrl    	: new Array()
}; 

forSPrint.GetPageInitInfo = function() {
	//��ȡ��ǰҳ�� �� ����ҳ��url
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

		hrefPattern = /��(<a href=\".*?\">){0,1}\[\d+\](<\/a>){0,1}/ig;

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

   //��ȡ��ʼҳ��·��html
	if(forSPrint.data.pageWayId != '' && document.getElementById(forSPrint.data.pageWayId) != null)
	{
		forSPrint.data.pageWayHtml = document.getElementById(forSPrint.data.pageWayId).innerHTML;
	}

   //��ȡ��ʼ��ʾ�ı���html
	if(forSPrint.data.titleId != '' && document.getElementById(forSPrint.data.titleId) != null)
	{
		forSPrint.data.titleHtml = document.getElementById(forSPrint.data.titleId).innerHTML;
	}

   //��ȡ��ʼ��ʾ����Ҫ����html
	if(forSPrint.data.artiBodyId != '' && document.getElementById(forSPrint.data.artiBodyId) != null)
	{
		forSPrint.data.artiBodyHtml = document.getElementById(forSPrint.data.artiBodyId).innerHTML;
	}

   //��ȡ��ʼ��ʾ��   logo html
	if(forSPrint.data.pageLogoId != '' && document.getElementById(forSPrint.data.pageLogoId) != null)
	{
		forSPrint.data.pageLogoHtml = document.getElementById(forSPrint.data.pageLogoId).innerHTML;
	}

}


forSPrint.Print = function(){
	//��ȡ��ӡ����
	forSPrint.GetPageInitInfo();

	var info =	"<html>"	+"\n"+
		"<head>"	+"\n"+
		"<meta http-equiv=\"Content-Type\" content=\"text/html; charset=gb2312\" />"	+"\n"+
		"<title></title>"	+"\n"+
		"<meta name=\"Keywords\" content=\",\" />"	+"\n"+
		"<meta name=\"Description\" content=\"\" />"	+"\n"+
		"<style type=\"text/css\" media=\"screen\">"	+"\n"+
		"*{margin:0;padding:0; font-family:\"����\";}"	+"\n"+
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
		"*{margin:0;padding:0; font-family:\"����\";}"	+"\n"+
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
		"	artiBodyId 	: \""+forSPrint.data.artiBodyId+"\",	//ҳ������Ҫ���� node id	"	+"\n"+
		"	titleId    	: \""+forSPrint.data.titleId+"\",		//ҳ�����ⲿ��node id	"	+"\n"+
		"	pageLogoId 	: \""+forSPrint.data.pageLogoId+"\",	//ҳ��logo������id"	+"\n"+
		"	pageWayId	: \""+forSPrint.data.pageWayId+"\",		//ҳ��·��������id"	+"\n"+
		"	"	+"\n"+
		"	pageLogoHtml: \""+escape(forSPrint.data.pageLogoHtml)+"\",	//��ʼ��ʾ��   logo html"	+"\n"+
		"	titleHtml	: \""+escape(forSPrint.data.titleHtml)+"\",		//��ʼ��ʾ�ı���html"	+"\n"+
		"	artiBodyHtml: \""+escape(forSPrint.data.artiBodyHtml)+"\",	//��ʼ��ʾ����Ҫ����html"	+"\n"+
		"	pageWayHtml : \""+escape(forSPrint.data.pageWayHtml)+"\",	//��ʼҳ��·��html"	+"\n"+
		"	"	+"\n"+
		"	iconUrl		: \""+escape(forSPrint.data.iconUrl)+"\",	//icon��ַ"	+"\n"+
		"	"	+"\n"+
		"	curPageNum 	: "+forSPrint.data.curPageNum+",				//��ǰҳ��"	+"\n"+
		"	//����ҳ��url �ӵ�1ҳ��ʼ"	+"\n"+
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
		"      <td class=\"f14\" style=\"padding-top:2px;\">��ҳ��Ϊ��ӡԤ��ҳ<input name=\"printImgCheckbox\" id=\"printImgCheckbox\" type=\"checkbox\" value=\"checkbox\" checked onClick=\"sPrint.printImgCboxChg(this);\" style=\"margin: 0px 2px 4px 10px;\">��ӡͼƬ</td>"	+"\n"+
		"      <td width=\"70\" class=\"f14\" style=\"padding-top:2px;\">ѡ���ֺţ�</td>"	+"\n"+
		"      <td width=\"115\" class=\"f14\" valign=\"top\"><table cellspacing=\"0\" class=\"f14\" style=\"margin-top:8px;\">"	+"\n"+
		"          <tr align=\"left\" valign=\"middle\">"	+"\n"+
		"            <td class=\"NselFZ\" id=\"topadmenu_0\" onClick=\"sPrint.doPriZoom(20);sPrint.ChgFz(0);\" height=\"20\">����</td>"	+"\n"+
		"            <td class=\"NselFZ\" id=\"topadmenu_1\" onClick=\"sPrint.doPriZoom(16);sPrint.ChgFz(1);\">��</td>"	+"\n"+
		"            <td class=\"selFZ\" id=\"topadmenu_2\" onClick=\"sPrint.doPriZoom(14);sPrint.ChgFz(2);\">��</td>"	+"\n"+
		"            <td class=\"NselFZ\" id=\"topadmenu_3\" onClick=\"sPrint.doPriZoom(12);sPrint.ChgFz(3);\">С</td>"	+"\n"+
		"          </tr>"	+"\n"+
		"        </table></td>"	+"\n"+
		"      <td width=\"90\"><a href=\"javascript:sPrint.Print();\"><img src=\"" + forSPrint.data.iconUrl + "/print.gif\" width=\"80\" height=\"20\" alt=\"��ӡ��ҳ��\"/></a></td>"	+"\n"+
		"      <td width=\"65\"><a href=\"javascript:sPrint.GoBack()\"><img src=\"" + forSPrint.data.iconUrl + "/return.gif\" width=\"64\" height=\"20\" alt=\"��������ҳ\"/></a></td>"	+"\n"+
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
		"		<div id=\"WzLy\" style=\"font-size:12px; margin:30px 0 40px 0; padding:0 15px; text-align:left; line-height:18px;\"> ������Դ��<span id=\"artiPath\"></span></div>"	+"\n"+
		"	  </td>"	+"\n"+
		"      <td width=\"1\" style=\"background:#DDD9D3;\" id=\"NoPrinter4\"></td>"	+"\n"+
		"    </tr>"	+"\n"+
		"  </table>"	+"\n"+
		"  <table width=\"600\" cellspacing=\"0\" id=\"NoPrinter2\" style=\"border-top:1px #DDD9D3 solid;\">"	+"\n"+
		"    <tr align=\"left\" valign=\"middle\">"	+"\n"+
		"      <td width=\"4\" height=\"36\"></td>"	+"\n"+
		"      <td class=\"f14\" style=\"padding-top:2px;\">��ҳ��Ϊ��ӡԤ��ҳ<input name=\"printImgCheckbox2\" id=\"printImgCheckbox2\" type=\"checkbox\" value=\"checkbox\" checked onClick=\"sPrint.printImgCboxChg(this);\" style=\"margin: 0px 2px 4px 10px;\">��ӡͼƬ</td>"	+"\n"+
		"      <td width=\"70\" class=\"f14\" style=\"padding-top:2px;\">ѡ���ֺţ�</td>"	+"\n"+
		"      <td width=\"115\" class=\"f14\" valign=\"top\"><table cellspacing=\"0\" class=\"f14\" style=\"margin-top:8px;\">"	+"\n"+
		"          <tr align=\"left\" valign=\"middle\">"	+"\n"+
		"            <td class=\"NselFZ\" id=\"topadmenu2_0\" onClick=\"sPrint.doPriZoom(20);sPrint.ChgFz2(0);\" height=\"20\">����</td>"	+"\n"+
		"            <td class=\"NselFZ\" id=\"topadmenu2_1\" onClick=\"sPrint.doPriZoom(16);sPrint.ChgFz2(1);\">��</td>"	+"\n"+
		"            <td class=\"selFZ\" id=\"topadmenu2_2\" onClick=\"sPrint.doPriZoom(14);sPrint.ChgFz2(2);\">��</td>"	+"\n"+
		"            <td class=\"NselFZ\" id=\"topadmenu2_3\" onClick=\"sPrint.doPriZoom(12);sPrint.ChgFz2(3);\">С</td>"	+"\n"+
		"          </tr>"	+"\n"+
		"        </table></td>"	+"\n"+
		"      <td width=\"90\"><a href=\"javascript:sPrint.Print();\"><img src=\"" + forSPrint.data.iconUrl + "/print.gif\" width=\"80\" height=\"20\" alt=\"��ӡ��ҳ��\"/></a></td>"	+"\n"+
		"      <td width=\"65\"><a href=\"javascript:sPrint.GoBack()\"><img src=\"" + forSPrint.data.iconUrl + "/return.gif\" width=\"64\" height=\"20\" alt=\"��������ҳ\"/></a></td>"	+"\n"+
		"    </tr>"	+"\n"+
		"  </table>"	+"\n"+
		"</center>"	+"\n"+
		"<!-- print dialog start -->"	+"\n"+
		"<div id=\"printDialog\" style=\"width:250px; display:none; text-align:center; padding:15px 10px 10px 10px; background:#ecebea; border:1px solid #ddd9d3; position:absolute; right:10px; top:30px;\">"	+"\n"+
		"	<div id=\"pageTotalInfo\" style=\"padding:0 0 10px 0\"></div>"	+"\n"+
		"	<div id=\"printBtnSelect\"><input type=\"button\" value=\"��ӡȫ��ҳ\" onClick=\"sPrint.PrintAllPage();\" />&nbsp;&nbsp;<input type=\"button\" value=\"��ӡ��ǰҳ\" onClick=\"sPrint.PrintCurPage();\" /></div>"	+"\n"+
		"	<div id=\"onloadInfo\" style=\"padding:5px 0 0 10px;\"></div>"	+"\n"+
		"	<div id=\"startPrintDiv\" style=\"display:none; padding:10px 0 0 0;\"><input type=\"button\" value=\"&nbsp;��ӡ&nbsp;\" onClick=\" sPrint.StartPrintAllPage();\" />&nbsp;&nbsp;<input type=\"button\" value=\"ȡ����ӡ\" onClick=\"sPrint.CancelPrintAllPage();\" /></div>"	+"\n"+
		"	<div style=\"position:absolute; top:5px; right:3px; width:20px; height:15px; cursor:pointer;\" onClick=\"javascript:sPrint.ClosePrintDialog();\"><span onMouseOver=\"this.style.fontWeight = \'bold\'\" onMouseOut=\"this.style.fontWeight=\'normal\'\">��</span></div>"	+"\n"+
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
		"//============================= ѡ���ֺ� start ====================="	+"\n"+
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
		"	//������������ ������Ҫֽ����"	+"\n"+
		"	if(sPrint.startPrint){"	+"\n"+
		"		sPrint.onloadInfoNode.innerHTML = \'��\' + sPrint.pageTotal +\'ҳ��ȫ���������ϣ���Լ��Ҫ\' + sPrint.GetNeedPageTotal() + \'ҳֽ\';"	+"\n"+
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
		"//============================= ѡ���ֺ� end ====================="	+"\n"+
		""	+"\n"+
		"//print dialog ����"	+"\n"+
		"sPrint.Scroll = function(o){"	+"\n"+
		"	var initTop = parseInt(o.style.top);"	+"\n"+
		"	window.onscroll = function(){"	+"\n"+
		"		o.style.top = initTop + document.body.scrollTop + \"px\";		"	+"\n"+
		"   } "	+"\n"+
		"}"	+"\n"+
		""	+"\n"+
		"//���ض��д���string"	+"\n"+
		"sPrint.BreakWord = function(str,intLen){"	+"\n"+
		"	var strTemp = \"\";"	+"\n"+
		"	if(str.length > intLen){//���ǻ��ж���"	+"\n"+
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
		"//��ӡ����������Ϣ ����"	+"\n"+
		"sPrint.pagelogoNode 	= sPrint.$(\"pagelogo\");				//����logo node"	+"\n"+
		"sPrint.artibodyTitleNode= sPrint.$(\"artibodyTitle\"); 	//���±��� node"	+"\n"+
		"sPrint.loLinksNode 		= sPrint.$(\"lo_links\");			//ҳ��·�� node"	+"\n"+
		"sPrint.priConNode 		= sPrint.$(\"PriCon\");			//����node"	+"\n"+
		"sPrint.artiPathNode 	= sPrint.$(\"artiPath\");			//������Դ node"	+"\n"+
		"sPrint.pageTotal 		= sPrint.data.pageUrl.length;		//ҳ������"	+"\n"+
		"sPrint.printDialog 		= sPrint.$(\"printDialog\");		//��ӡ�Ի���"	+"\n"+
		"sPrint.onloadInfoNode	= sPrint.$(\"onloadInfo\"); 		//������Ϣ��ʾ node"	+"\n"+
		"sPrint.allIframeNode	= sPrint.$(\"allIframe\");		//��������ҳ���� node"	+"\n"+
		"sPrint.onloadNum 		= 0;							//��ǰ��ʼ����ҳ����������ֵ"	+"\n"+
		"sPrint.startPrint 		= false;						//��ǰ�Ƿ��Ѿ�����������д����ҳ�� ��׼����ӡ  �����¶�ҳ��ӡ����"	+"\n"+
		"sPrint.heightPerPage 	= 800; 							//ÿҳֽ���Դ�ӡ�ĸ߶�"	+"\n"+
		"sPrint.pageTotalInfoNode = sPrint.$(\"pageTotalInfo\");	//ҳ��������Ϣ"	+"\n"+
		"sPrint.printBtnSelectNode = sPrint.$(\"printBtnSelect\");	//�Ƿ���ӡѡ��node"	+"\n"+
		"sPrint.printImgCheckbox	 = sPrint.$(\"printImgCheckbox\");	//�Ƿ���ӡͼƬ checkbox"	+"\n"+
		"sPrint.printImgCheckbox2 = sPrint.$(\"printImgCheckbox2\");	//�Ƿ���ӡͼƬ checkbox �ײ�"	+"\n"+
		"sPrint.printImgOk 		= true;	//Ĭ���Ƿ���ӡͼƬ"	+"\n"+
		"sPrint.imgMaxHeight		= 900;	//ͼƬ���������߶�"	+"\n"+
		""	+"\n"+
		"//��ӡ������"	+"\n"+
		"sPrint.Print = function(){"	+"\n"+
		"	//������ҳ��"	+"\n"+
		"	if(sPrint.pageTotal == 1){"	+"\n"+
		"		window.print();"	+"\n"+
		"	}else if(sPrint.pageTotal > 1){//������ҳ��"	+"\n"+
		"		if(sPrint.onloadNum == 0){	//��δ��������ҳ ��ʾ��ҳ��ӡ����"	+"\n"+
		"			//��ʾ������ҳ��Ҫ��ӡ"	+"\n"+
		"			sPrint.pageTotalInfoNode.innerHTML = \'<p>���Ĺ���\' + sPrint.pageTotal + \'ҳ�����Ƿ�ȫ����ӡ��</p>\';"	+"\n"+
		"			//�򿪶�ҳ��ӡdialog"	+"\n"+
		"			sPrint.OpenPrintDialog();"	+"\n"+
		"		}else if(sPrint.onloadNum < sPrint.pageTotal){	//�������ڼ���ҳ ����ʾ�ȴ�"	+"\n"+
		"			//alert(\"�����ĵȴ�������ҳ��δ��������.\\n����ȷ����������\");"	+"\n"+
		"			//�򿪶�ҳ��ӡdialog"	+"\n"+
		"			sPrint.OpenPrintDialog();"	+"\n"+
		"			return;"	+"\n"+
		"		}else if(sPrint.startPrint){//������ҳ�Ѿ��������� ��ֱ�Ӵ�ӡ���˳�"	+"\n"+
		"			window.print();"	+"\n"+
		"			return;"	+"\n"+
		"		}		"	+"\n"+
		"	}"	+"\n"+
		"}"	+"\n"+
		""	+"\n"+
		"//����ӡ��ǰҳ"	+"\n"+
		"sPrint.PrintCurPage = function(){"	+"\n"+
		"	//�رմ�ӡdialog"	+"\n"+
		"	sPrint.ClosePrintDialog();	"	+"\n"+
		"	//��ӡ��ǰҳ"	+"\n"+
		"	window.print();"	+"\n"+
		"}"	+"\n"+
		""	+"\n"+
		"//ȫ����ӡ"	+"\n"+
		"sPrint.PrintAllPage = function(){"	+"\n"+
		"	//���ش�ӡ2����ť"	+"\n"+
		"	sPrint.printBtnSelectNode.style.display = \"none\";"	+"\n"+
		"	//������ʾҳ��������Ϣ"	+"\n"+
		"	sPrint.pageTotalInfoNode.innerHTML = \'<p>���Ĺ���\' + sPrint.pageTotal + \'ҳ��Ҫ���أ��������ĵȴ�</p>\';"	+"\n"+
		"	"	+"\n"+
		"	//���ص�һ��ҳ�� ����ʾ������Ϣ"	+"\n"+
		"sPrint.allIframeNode.innerHTML = \'<iframe id=\"iframeForData\" name=\"iframeForData\" src=\"\'+sPrint.data.pageUrl[0]+ \'?t=\' + new Date().getTime() + \'\" width=\"0\" marginwidth=\"0\" height=\"0\" marginheight=\"0\"  scrolling=\"no\" frameborder=\"0\" onload=\"sPrint.OnloadBack();\" onerror=\"return true;\"></iframe>\';"	+"\n"+
		"	sPrint.onloadNum = 1;"	+"\n"+
		"	sPrint.onloadInfoNode.innerHTML = \'��1ҳ���ڼ���...\';"	+"\n"+
		"}"	+"\n"+
		""	+"\n"+
		"//��ʼ��ӡ����ҳ��"	+"\n"+
		"sPrint.StartPrintAllPage = function(){	"	+"\n"+
		"	sPrint.ClosePrintDialog();	//�ر�dialog	"	+"\n"+
		"	window.print();	//��ӡ"	+"\n"+
		"}"	+"\n"+
		""	+"\n"+
		"//ȡ��ȫ����ӡ"	+"\n"+
		"sPrint.CancelPrintAllPage = function(){	"	+"\n"+
		"	sPrint.ClosePrintDialog(); //�ر�dialog"	+"\n"+
		"}"	+"\n"+
		""	+"\n"+
		"//���ص�����ҳ"	+"\n"+
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
		"//���м���ҳ���Ļص�����"	+"\n"+
		"sPrint.OnloadBack = function(){"	+"\n"+
		"	//������һ����������Ҫ���ܶ� ��Ϊ��ҳ���� ��������ӡʱ ��Ҫʹ�õ�һҳ��document.title�����⡢ ������Դ����Ϣ"	+"\n"+
		"	if(sPrint.onloadNum == 1){"	+"\n"+
		"		//����document.title"	+"\n"+
		"		//document.title = frames[\'iframeForData\'].document.title;"	+"\n"+
		"		//���±���"	+"\n"+
		"		sPrint.artibodyTitleNode.innerHTML = frames[\"iframeForData\"].document.getElementById(sPrint.data.titleId).innerHTML;"	+"\n"+
		"		//����������Դ��Ϣ �˵�����"	+"\n"+
		"		sPrint.artiPathNode.innerHTML = sPrint.BreakWord(frames[\"iframeForData\"].location.href.replace(/\\?.*/gi,\'\'),90);"	+"\n"+
		"		//���յ�ǰҳ���� ׼����ҳ½������"	+"\n"+
		"		sPrint.priConNode.innerHTML = \"\";"	+"\n"+
		"	}"	+"\n"+
		""	+"\n"+
		"	//alert(frames[\'iframeForData\'].location +\"///\"+sPrint.onloadNum);	"	+"\n"+
		"	//�������ݲ��뵽ҳ��ǰ���й���"	+"\n"+
		"	sPrint.priConNode.innerHTML += sPrint.HandlerBodyHtml(frames[\'iframeForData\'].document.getElementById(sPrint.data.artiBodyId).innerHTML);//iframe ҳ��Ҳ��GetObj ����"	+"\n"+
		"	//��������������Ϣ"	+"\n"+
		"	sPrint.onloadInfoNode.innerHTML = \'��\' + sPrint.onloadNum + \'ҳ��������\';"	+"\n"+
		"	//�����Ƿ�����ҳ����Ҫ����"	+"\n"+
		"	if(sPrint.onloadNum < sPrint.pageTotal){//������һҳ ����ʾ������Ϣ"	+"\n"+
		"		sPrint.onloadNum++;"	+"\n"+
		"		frames[\"iframeForData\"].location.href = sPrint.data.pageUrl[sPrint.onloadNum - 1] + \'?t=\' + new Date().getTime();"	+"\n"+
		"		sPrint.onloadInfoNode.innerHTML = \'��\'+(sPrint.onloadNum)+\'ҳ���ڼ���...\';		"	+"\n"+
		"	}else{//�������� ��ʾ����������Ϣ"	+"\n"+
		"		//���� ҳ��������Ϣ"	+"\n"+
		"		sPrint.pageTotalInfoNode.style.display = \"none\";"	+"\n"+
		"		sPrint.onloadInfoNode.innerHTML = \'��\' + sPrint.pageTotal +\'ҳ��ȫ���������ϣ���Լ��Ҫ\' + sPrint.GetNeedPageTotal() + \'ҳֽ\';"	+"\n"+
		"		//��ʾ��ʼ��ӡ��ť"	+"\n"+
		"		sPrint.$(\"startPrintDiv\").style.display = \"block\";"	+"\n"+
		"		//�����Ѿ�д������"	+"\n"+
		"		sPrint.startPrint = true;"	+"\n"+
		"	}"	+"\n"+
		"}"	+"\n"+
		""	+"\n"+
		"//��������html"	+"\n"+
		"sPrint.HandlerBodyHtml = function(str){"	+"\n"+
		"	var strTmp = str;"	+"\n"+
		"	//�˵� �����ڲ����ֵ���"	+"\n"+
		"	strTmp = strTmp.replace(/<!--\\s+�����ڲ����ֵ���\\s+:\\s+begin -->[\\s\\S]*<!--\\s+�����ڲ����ֵ���\\s+:\\s+end\\s+-->/,\"\");"	+"\n"+
		"	//ȥ�� artibody�ڴ�ӡ�����Լ��Ժ�������html����"	+"\n"+
		"	strTmp = strTmp.replace(/<span\\s+id=[\"\']?_function_code_page[\"\']?>[\\s\\S]*/i, \"\");"	+"\n"+
		"	//���˵�iask�ؼ��ֵ�html����"	+"\n"+
		"	strTmp = strTmp.replace(/<span\\s+class=.?yqlink.?>[\\s\\S]*?class=.?akey.?\\s+title=[\\s\\S]*?>([\\s\\S]*?)<\\/a><\\/span>/gi, \"$1\");"	+"\n"+
		"	//����img onload"	+"\n"+
		"	strTmp = strTmp.replace(/<img/gi,\"<img onload=\\\"sPrint.ResizeImg(this);\\\"\");"	+"\n"+
		"	return strTmp;"	+"\n"+
		"}"	+"\n"+
		""	+"\n"+
		"//���¶��峬��ͼƬ�߶�"	+"\n"+
		"sPrint.ResizeImg = function(obj){"	+"\n"+
		"	if(obj.height > sPrint.imgMaxHeight){"	+"\n"+
		"		obj.height = sPrint.imgMaxHeight;"	+"\n"+
		"		obj.style.pageBreakAfter = \"always\";"	+"\n"+
		"	}"	+"\n"+
		"}"	+"\n"+
		""	+"\n"+
		"//�򿪴�ӡdialog"	+"\n"+
		"sPrint.OpenPrintDialog = function(){"	+"\n"+
		"	sPrint.printDialog.style.display = \"block\";"	+"\n"+
		"}"	+"\n"+
		"//�رմ�ӡdialog"	+"\n"+
		"sPrint.ClosePrintDialog = function(){"	+"\n"+
		"	sPrint.printDialog.style.display = \"none\";"	+"\n"+
		"}"	+"\n"+
		""	+"\n"+
		"//���ش�ӡ����Ҫֽ����"	+"\n"+
		"sPrint.GetNeedPageTotal = function(){"	+"\n"+
		"	var needPageTotal;"	+"\n"+
		"	return needPageTotal = Math.round(sPrint.priConNode.offsetHeight/sPrint.heightPerPage);"	+"\n"+
		"}"	+"\n"+
		""	+"\n"+
		"//�Ƿ���ӡͼƬ"	+"\n"+
		"sPrint.printImgCboxChg = function(obj){"	+"\n"+
		"	if(obj.checked == true){		"	+"\n"+
		"		sPrint.DisplayBodyImg(true);"	+"\n"+
		"	}else{"	+"\n"+
		"		sPrint.DisplayBodyImg(false);"	+"\n"+
		"	}"	+"\n"+
		"}"	+"\n"+
		""	+"\n"+
		"//�Ƿ���ʾ����ͼƬ"	+"\n"+
		"sPrint.DisplayBodyImg = function(bol){"	+"\n"+
		"	if(bol){"	+"\n"+
		"		sPrint.printImgCheckbox.checked = true;"	+"\n"+
		"		sPrint.printImgCheckbox2.checked = true;"	+"\n"+
		"		sPrint.priConNode.className = \"\";"	+"\n"+
		"		"	+"\n"+
		"		//��ʾ��ӡͼƬǰ����ͼƬ �ض��峬��ͼƬ�߶�"	+"\n"+
		"		var imgs = sPrint.priConNode.getElementsByTagName(\"img\");"	+"\n"+
		"		for(var i = 0; i < imgs.length; i++){"	+"\n"+
		"			sPrint.ResizeImg(imgs[i]);"	+"\n"+
		"		}"	+"\n"+
		"	}else{"	+"\n"+
		"		sPrint.printImgCheckbox.checked = false;"	+"\n"+
		"		sPrint.printImgCheckbox2.checked = false;"	+"\n"+
		"		sPrint.priConNode.className = \"noImg\";"	+"\n"+
		"	}"	+"\n"+
		"	//������������ ���¼�����ӡҳ��"	+"\n"+
		"	if(sPrint.startPrint){"	+"\n"+
		"		sPrint.onloadInfoNode.innerHTML = \'��\' + sPrint.pageTotal +\'ҳ��ȫ���������ϣ���Լ��Ҫ\' + sPrint.GetNeedPageTotal() + \'ҳֽ\';	"	+"\n"+
		"	}	"	+"\n"+
		"}"	+"\n"+
		""	+"\n"+
		""	+"\n"+
		"//��ʼ��"	+"\n"+
		"sPrint.Init = function(){"	+"\n"+
		"	document.title = \"\";	//���õ�ǰҳtitle ������ӡ��		"	+"\n"+
		"	sPrint.pagelogoNode.innerHTML 	= unescape(sPrint.data.pageLogoHtml);	//��ʾҳ��logo"	+"\n"+
		"	sPrint.loLinksNode.innerHTML 	= unescape(sPrint.data.pageWayHtml).replace(/#000099/g,\"#000000\");	//��ʾҳ��·�� ��ʹ�ú�ɫ"	+"\n"+
		"	sPrint.artibodyTitleNode.innerHTML = unescape(sPrint.data.titleHtml);	//��ʾҳ������"	+"\n"+
		"	sPrint.priConNode.innerHTML 	= sPrint.HandlerBodyHtml(unescape(sPrint.data.artiBodyHtml));	//��ʾ��ǰҳ����"	+"\n"+
		"	sPrint.artiPathNode.innerHTML 	= sPrint.BreakWord(location.href,92);	//��ʾ������Դurl δ���Ƕ���"	+"\n"+
		"	sPrint.Scroll(sPrint.printDialog);		//printDialog ����	"	+"\n"+
		"	//��λprintDialog�ĳ�ʼλ��"	+"\n"+
		"	//��css��λ"	+"\n"+
		"}"	+"\n"+
		"sPrint.Init();"	+"\n"+
		"//����sPrint.printImgOk ��ʼ��checkbox ����ie��ˢ�º���¼��ǰcheckbox״̬��bug"	+"\n"+
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

//070411�޸� ws
//�޶�ý��logo�ߴ�
