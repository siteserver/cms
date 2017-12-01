/*
  年月日联动下拉选择JS封装类 Ver 1.0 版
  制作时间:2013-3-12
　更新时间:2013-3-12
  应用说明:页面包含<script type="text/javascript" src="YMDClass.js" charset="utf-8"></script>
  <select name="year1"></select>
  <select name="month1"></select>
  <select name="day1"></select>
  <script>
  new YMDselect('year1','month1','day1',1990,2,10);
  </script>
	年月联动
	  new YMDselect('year1','month1');
	  new YMDselect('year1','month1',1990);
	  new YMDselect('year1','month1',1990,2);
	年月日联动
	  new YMDselect('year1','month1','day1');
	  new YMDselect('year1','month1','day1',1990);
	  new YMDselect('year1','month1','day1',1990,2);
	  new YMDselect('year1','month1','day1',1990,2,10);
\*** 程序制作/版权所有:Kevin QQ:251378427 E-Mail:yeminch@qq.com 网址:http://iulog.com ***/
SYT="请选择";
SMT="请选择";
SDT="请选择";
BYN=50;//年份范围往前50年
AYN=0;//年份范围往后0年
function YMDselect(){
	this.SelY=document.getElementsByName(arguments[0])[0];
	this.SelM=document.getElementsByName(arguments[1])[0];
	this.SelD=document.getElementsByName(arguments[2])[0];
	this.DefY=this.SelD?arguments[3]:arguments[2];
	this.DefM=this.SelD?arguments[4]:arguments[3];
	this.DefD=this.SelD?arguments[5]:arguments[4];
	this.SelY.YMD=this;
	this.SelM.YMD=this;
	this.SelY.onchange=function(){YMDselect.SetM(this.YMD)};
	if(this.SelD)this.SelM.onchange=function(){YMDselect.SetD(this.YMD)};
	YMDselect.SetY(this)
};
//设置年份
YMDselect.SetY=function(YMD){
	dDate = new Date();
	dCurYear = dDate.getFullYear();
	YMD.SelY.options.add(new Option(SYT,'0'));
	for(i = dCurYear+AYN; i>(dCurYear-BYN); i--){
	    YMDYT = i ;//i+'年'
		YMDYV=i;
		OptY = new Option(YMDYT,YMDYV);
		YMD.SelY.options.add(OptY);
		if(YMD.DefY==YMDYV) OptY.selected=true
	}
	YMDselect.SetM(YMD)
};
//设置月份
YMDselect.SetM=function(YMD){
	YMD.SelM.length = 0;
	YMD.SelM.options.add(new Option(SMT,'0'));
	if(YMD.SelY.value>0){
		for(var i=1;i<=12;i++){
			YMDMT=i+'月';
			YMDMV=i;
			OptM=new Option(YMDMT,YMDMV);
			YMD.SelM.options.add(OptM);
			if(YMD.DefM==YMDMV) OptM.selected=true
		}
	}
	if(YMD.SelD)YMDselect.SetD(YMD)
};
//设置日期
YMDselect.SetD=function(YMD){
	YI=YMD.SelY.value;
	MI=YMD.SelM.value;
	YMD.SelD.length = 0;
	YMD.SelD.options.add(new Option(SDT,'0'));
	if(YI>0 && MI>0){
		dPrevDate = new Date(YI, MI, 0);
		daysInMonth=dPrevDate.getDate();
		for (d = 1; d <= parseInt(daysInMonth); d++) {
			YMDDT=d+'日';
			YMDDV=d;
			OptD=new Option(YMDDT,YMDDV);
			YMD.SelD.options.add(OptD);
			if(YMD.DefD==YMDDV)OptD.selected=true
		}
	}
}