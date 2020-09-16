function RunGLNL(isTraditional){
var today=new Date();
//var d=new Array("星期日","星期一","星期二","星期三","星期四","星期五","星期六");
var d=new Array(decodeURIComponent("%E6%98%9F%E6%9C%9F%E6%97%A5"),decodeURIComponent("%E6%98%9F%E6%9C%9F%E4%B8%80"),decodeURIComponent("%E6%98%9F%E6%9C%9F%E4%BA%8C"),decodeURIComponent("%E6%98%9F%E6%9C%9F%E4%B8%89"),decodeURIComponent("%E6%98%9F%E6%9C%9F%E5%9B%9B"),decodeURIComponent("%E6%98%9F%E6%9C%9F%E4%BA%94"),decodeURIComponent("%E6%98%9F%E6%9C%9F%E5%85%AD"));
//var DDDD=(today.getFullYear()<100 ? today.getFullYear()+1900:today.getFullYear())+"年"+(today.getMonth()+1)+"月"+today.getDate()+"日";
var DDDD=(today.getFullYear()<100 ? today.getFullYear()+1900:today.getFullYear())+decodeURIComponent("%E5%B9%B4")+(today.getMonth()+1)+decodeURIComponent("%E6%9C%88")+today.getDate()+decodeURIComponent("%E6%97%A5");
DDDD = DDDD + " " + d[today.getDay()];
if (isTraditional)
{
DDDD = DDDD+ " " + (CnDateofDateStr(today));
}
document.write(DDDD);
}
function DaysNumberofDate(DateGL){
return parseInt((Date.parse(DateGL)-Date.parse(DateGL.getFullYear()+"/1/1"))/86400000)+1;
}
function CnDateofDate(DateGL){
var CnData=new Array(
0x16,0x2a,0xda,0x00,0x83,0x49,0xb6,0x05,0x0e,0x64,0xbb,0x00,0x19,0xb2,0x5b,0x00,
0x87,0x6a,0x57,0x04,0x12,0x75,0x2b,0x00,0x1d,0xb6,0x95,0x00,0x8a,0xad,0x55,0x02,
0x15,0x55,0xaa,0x00,0x82,0x55,0x6c,0x07,0x0d,0xc9,0x76,0x00,0x17,0x64,0xb7,0x00,
0x86,0xe4,0xae,0x05,0x11,0xea,0x56,0x00,0x1b,0x6d,0x2a,0x00,0x88,0x5a,0xaa,0x04,
0x14,0xad,0x55,0x00,0x81,0xaa,0xd5,0x09,0x0b,0x52,0xea,0x00,0x16,0xa9,0x6d,0x00,
0x84,0xa9,0x5d,0x06,0x0f,0xd4,0xae,0x00,0x1a,0xea,0x4d,0x00,0x87,0xba,0x55,0x04
);
var CnMonth=new Array();
var CnMonthDays=new Array();
var CnBeginDay;
var LeapMonth;
var Bytes=new Array();
var I;
var CnMonthData;
var DaysCount;
var CnDaysCount;
var ResultMonth;
var ResultDay;
var yyyy=DateGL.getFullYear();
var mm=DateGL.getMonth()+1;
var dd=DateGL.getDate();
if(yyyy<100) yyyy+=1900;
  if ((yyyy < 1997) || (yyyy > 2020)){
    return 0;
    }
  Bytes[0] = CnData[(yyyy - 1997) * 4];
  Bytes[1] = CnData[(yyyy - 1997) * 4 + 1];
  Bytes[2] = CnData[(yyyy - 1997) * 4 + 2];
  Bytes[3] = CnData[(yyyy - 1997) * 4 + 3];
  if ((Bytes[0] & 0x80) != 0) {CnMonth[0] = 12;}
  else {CnMonth[0] = 11;}
  CnBeginDay = (Bytes[0] & 0x7f);
  CnMonthData = Bytes[1];
  CnMonthData = CnMonthData << 8;
  CnMonthData = CnMonthData | Bytes[2];
  LeapMonth = Bytes[3];
for (I=15;I>=0;I--){
    CnMonthDays[15 - I] = 29;
    if (((1 << I) & CnMonthData) != 0 ){
      CnMonthDays[15 - I]++;}
    if (CnMonth[15 - I] == LeapMonth ){
      CnMonth[15 - I + 1] = - LeapMonth;}
    else{
      if (CnMonth[15 - I] < 0 ){CnMonth[15 - I + 1] = - CnMonth[15 - I] + 1;}
      else {CnMonth[15 - I + 1] = CnMonth[15 - I] + 1;}
      if (CnMonth[15 - I + 1] > 12 ){ CnMonth[15 - I + 1] = 1;}
    }
  }
  DaysCount = DaysNumberofDate(DateGL) - 1;
  if (DaysCount <= (CnMonthDays[0] - CnBeginDay)){
    if ((yyyy > 1901) && (CnDateofDate(new Date((yyyy - 1)+"/12/31")) < 0)){
      ResultMonth = - CnMonth[0];}
    else {ResultMonth = CnMonth[0];}
    ResultDay = CnBeginDay + DaysCount;
  }
  else{
    CnDaysCount = CnMonthDays[0] - CnBeginDay;
    I = 1;
    while ((CnDaysCount < DaysCount) && (CnDaysCount + CnMonthDays[I] < DaysCount)){
      CnDaysCount+= CnMonthDays[I];
      I++;
    }
    ResultMonth = CnMonth[I];
    ResultDay = DaysCount - CnDaysCount;
  }
  if (ResultMonth > 0){
    return ResultMonth * 100 + ResultDay;}
  else{return ResultMonth * 100 - ResultDay;}
}
function CnYearofDate(DateGL){
var YYYY=DateGL.getFullYear();
var MM=DateGL.getMonth()+1;
var CnMM=parseInt(Math.abs(CnDateofDate(DateGL))/100);
if(YYYY<100) YYYY+=1900;
if(CnMM>MM) YYYY--;
YYYY-=1864;
//return CnEra(YYYY)+"年";
return CnEra(YYYY)+decodeURIComponent("%E5%B9%B4");
}
function CnMonthofDate(DateGL){
//var  CnMonthStr=new Array("零","正","二","三","四","五","六","七","八","九","十","冬","腊");
var  CnMonthStr=new Array(decodeURIComponent("%E9%9B%B6"),decodeURIComponent("%E6%AD%A3"),decodeURIComponent("%E4%BA%8C"),decodeURIComponent("%E4%B8%89"),decodeURIComponent("%E5%9B%9B"),decodeURIComponent("%E4%BA%94"),decodeURIComponent("%E5%85%AD"),decodeURIComponent("%E4%B8%83"),decodeURIComponent("%E5%85%AB"),decodeURIComponent("%E4%B9%9D"),decodeURIComponent("%E5%8D%81"),decodeURIComponent("%E5%86%AC"),decodeURIComponent("%E8%85%8A"));
var  Month;
  Month = parseInt(CnDateofDate(DateGL)/100);
  //if (Month < 0){return "闰" + CnMonthStr[-Month] + "月";}
  if (Month < 0){return decodeURIComponent("%E9%97%B0") + CnMonthStr[-Month] + decodeURIComponent("%E6%9C%88");}
  else{return CnMonthStr[Month] + decodeURIComponent("%E6%9C%88");}
}
function CnDayofDate(DateGL){
/*var CnDayStr=new Array("零",

    "初一", "初二", "初三", "初四", "初五",
    "初六", "初七", "初八", "初九", "初十",
    "十一", "十二", "十三", "十四", "十五",
    "十六", "十七", "十八", "十九", "二十",
    "廿一", "廿二", "廿三", "廿四", "廿五",
    "廿六", "廿七", "廿八", "廿九", "三十");*/
var CnDayStr=new Array(decodeURIComponent("%E9%9B%B6"),
    decodeURIComponent("%E5%88%9D%E4%B8%80"), decodeURIComponent("%E5%88%9D%E4%BA%8C"), decodeURIComponent("%E5%88%9D%E4%B8%89"), decodeURIComponent("%E5%88%9D%E5%9B%9B"), decodeURIComponent("%E5%88%9D%E4%BA%94"),
    decodeURIComponent("%E5%88%9D%E5%85%AD"), decodeURIComponent("%E5%88%9D%E4%B8%83"), decodeURIComponent("%E5%88%9D%E5%85%AB"), decodeURIComponent("%E5%88%9D%E4%B9%9D"), decodeURIComponent("%E5%88%9D%E5%8D%81"),
    decodeURIComponent("%E5%8D%81%E4%B8%80"), decodeURIComponent("%E5%8D%81%E4%BA%8C"), decodeURIComponent("%E5%8D%81%E4%B8%89"), decodeURIComponent("%E5%8D%81%E5%9B%9B"), decodeURIComponent("%E5%8D%81%E4%BA%94"),
    decodeURIComponent("%E5%8D%81%E5%85%AD"), decodeURIComponent("%E5%8D%81%E4%B8%83"), decodeURIComponent("%E5%8D%81%E5%85%AB"), decodeURIComponent("%E5%8D%81%E4%B9%9D"),decodeURIComponent( "%E4%BA%8C%E5%8D%81"),
    decodeURIComponent("%E5%BB%BF%E4%B8%80"), decodeURIComponent("%E5%BB%BF%E4%BA%8C"), decodeURIComponent("%E5%BB%BF%E4%B8%89"), decodeURIComponent("%E5%BB%BF%E5%9B%9B"), decodeURIComponent("%E5%BB%BF%E4%BA%94"),
    decodeURIComponent("%E5%BB%BF%E5%85%AD"), decodeURIComponent("%E5%BB%BF%E4%B8%83"), decodeURIComponent("%E5%BB%BF%E5%85%AB"), decodeURIComponent("%E5%BB%BF%E4%B9%9D"), decodeURIComponent("%E4%B8%89%E5%8D%81"));
var Day;
  Day = (Math.abs(CnDateofDate(DateGL)))%100;
  return CnDayStr[Day];
}
function DaysNumberofMonth(DateGL){
var MM1=DateGL.getFullYear();
    MM1<100 ? MM1+=1900:MM1;
var MM2=MM1;
    MM1+="/"+(DateGL.getMonth()+1);
    MM2+="/"+(DateGL.getMonth()+2);
    MM1+="/1";
    MM2+="/1";
return parseInt((Date.parse(MM2)-Date.parse(MM1))/86400000);
}
function CnEra(YYYY){
//var Tiangan=new Array("甲","乙","丙","丁","戊","己","庚","辛","壬","癸");
var Tiangan=new Array(decodeURIComponent("%E7%94%B2"),decodeURIComponent("%E4%B9%99"),decodeURIComponent("%E4%B8%99"),decodeURIComponent("%E4%B8%81"),decodeURIComponent("%E6%88%8A"),decodeURIComponent("%E5%B7%B1"),decodeURIComponent("%E5%BA%9A"),decodeURIComponent("%E8%BE%9B"),decodeURIComponent("%E5%A3%AC"),decodeURIComponent("%E7%99%B8"));
//var Dizhi=new Array("子(鼠)","丑(牛)","寅(虎)","卯(兔)","辰(龙)","巳(蛇)",
                    //"午(马)","未(羊)","申(猴)","酉(鸡)","戌(狗)","亥(猪)");
//var Dizhi=new Array("子","丑","寅","卯","辰","巳","午","未","申","酉","戌","亥");
var Dizhi=new Array(decodeURIComponent("%E5%AD%90"),decodeURIComponent("%E4%B8%91"),decodeURIComponent("%E5%AF%85"),decodeURIComponent("%E5%8D%AF"),decodeURIComponent("%E8%BE%B0"),decodeURIComponent("%E5%B7%B3"),decodeURIComponent("%E5%8D%88"),decodeURIComponent("%E6%9C%AA"),decodeURIComponent("%E7%94%B3"),decodeURIComponent("%E9%85%89"),decodeURIComponent("%E6%88%8C"),decodeURIComponent("%E4%BA%A5"));
return Tiangan[YYYY%10]+Dizhi[YYYY%12];
}
function CnDateofDateStr(DateGL){
//  if(CnMonthofDate(DateGL)=="零月") return "　请调整您的计算机日期!";
  if(CnMonthofDate(DateGL)==decodeURIComponent("%E9%9B%B6%E6%9C%88")) return decodeURIComponent("%E3%80%80%E8%AF%B7%E8%B0%83%E6%95%B4%E6%82%A8%E7%9A%84%E8%AE%A1%E7%AE%97%E6%9C%BA%E6%97%A5%E6%9C%9F!");
  //else return "农历"+CnYearofDate(DateGL)+ " " + CnMonthofDate(DateGL) + CnDayofDate(DateGL);
  else return decodeURIComponent("%E5%86%9C%E5%8E%86")+CnYearofDate(DateGL)+ " " + CnMonthofDate(DateGL) + CnDayofDate(DateGL);
}


function CurentTime(){ 
    var now = new Date(); 
    var hh = now.getHours(); 
    var mm = now.getMinutes(); 
    var ss = now.getTime() % 60000; 
    ss = (ss - (ss % 1000)) / 1000; 
    var clock = hh+':'; 
    if (mm < 10) clock += '0'; 
    clock += mm+':'; 
    if (ss < 10) clock += '0'; 
    clock += ss; 
    return(clock); 
} 