function getCalendar() {
    var dayCount = 0;
    var strHtml = "";
    var myDate = new Date();
    var curYear = myDate.getFullYear();
    var curMonth = myDate.getMonth();
     
    var signLogArray = new Array();
    
    for (var i = 0; i < controller.cardSignLogInfoList.length; i++) {
        var cardSignLogInfo = controller.cardSignLogInfoList[i];
        signLogArray[getSignDate(cardSignLogInfo.signDate)] = getSignDate(cardSignLogInfo.signDate);
    }
  
    for (var i = curYear; i >= curYear - 1; i--) {
        if (i == curYear) { }
        for (var j = 1; j < 13; j++) {
            strHtml += "<div class=\"date_box_item\">";
            strHtml += "<div class=\"calendar-header\">";
            strHtml += "" + i + "年";
            strHtml += "" + j + "月";
            strHtml += "</div>";
            strHtml += "<table style=\"width: 100%;\">";
            strHtml += "<thead><tr><th>日</th><th>一</th><th>二</th><th>三</th><th>四</th><th>五</th><th>六</th></tr></thead>";

            dayCount = getDaysInOneMonth(i, j);
            strHtml += "<tbody id=\"tab_" + i + "_" + j + "\">";
            var curXDay = getDay(i + "/" + j + "/1");
            strHtml += "<tr>";

            for (var h = 0; h < curXDay; h++) {
                preDayCount = getDaysInOneMonth(i, j - 1);
                var aaDay = preDayCount - curXDay + h;
                strHtml += "<td class=\"enable\">" + aaDay + "</td>";
            }
            for (var k = 1; k < dayCount; k++) {
                if (curXDay % 7 == 0) {
                    strHtml += "</tr><tr>";
                }
                if (!signLogArray[i + '-' + j + '-' + k]) {
                    strHtml += "<td class=\"enable\">" + k + "</td>";
                }
                else {
                    strHtml += "<td class=\"selected\">" + k + "</td>";
                }
                curXDay++;
            }
            strHtml += "</tr>";
            strHtml += "</tbody>";
            strHtml += "</table>";
            strHtml += "</div>";
        }
    }

    $("#divDate").html(strHtml);
    for (var i = curYear - 1; i < curYear + 1; i++) {
        for (var j = 1; j < 13; j++) {
            var lastTdCount = $("#tab_" + i + "_" + j + " tr:last").children().length;
            for (var t = 1; t <= 7 - lastTdCount; t++) {
                var newhtml = "<td class=\"enable next_td\">" + t + "</td>";
                $($("#tab_" + i + "_" + j + " tr:last")).append(newhtml);
            }
        }
    }
    $('#curMonth').val(curMonth);
    window.mySwipe = $('#mySwipe').Swipe().data('Swipe');
   
}


function getDaysInOneMonth(year, month) {
    month = parseInt(month, 10);
    var d = new Date(year, month, 0);
    return d.getDate() + 1;
}

function getDay(date) {
    var newDate = new Date(date).getDay();
    return newDate;
}

function getSignDate(d) {
    var year = parseInt(d.split('T')[0].split('-')[0]);
    var month = parseInt(d.split('T')[0].split('-')[1]);
    var day =parseInt(d.split('T')[0].split('-')[2]);
    return date = year +'-'+ month +'-'+ day;
}
