<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.WeiXin.PageCount" %>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>
<head>
<meta charset="utf-8">
<!--#include file="../inc/header.aspx"-->
</head>

<body>
<!--#include file="../inc/openWindow.html"-->
<form class="form-inline" runat="server">
  <asp:Literal id="LtlBreadCrumb" runat="server" />
  <bairong:alerts runat="server" />

  <script type="text/javascript">
  function formatCurrency(num) {  
       num = num.toString().replace(/\$|\,/g,'');  
       if(isNaN(num))  
       num = "0";  
       sign = (num == (num = Math.abs(num)));  
       num = Math.floor(num*100+0.50000000001);  
       cents = num%100;  
       num = Math.floor(num/100).toString();  
       if(cents<10)  
       cents = "0" + cents;  
       for (var i = 0; i < Math.floor((num.length-(1+i))/3); i++)  
       num = num.substring(0,num.length-(4*i+3))+','+  
       num.substring(num.length-(4*i+3));  
       return (((sign)?'':'-')  + num);  
}  

$(function () {
    
        var colors = Highcharts.getOptions().colors,
            categories = [<asp:Literal id="LtlCategories" runat="server" />],
            name = '月份',
            data = [<asp:Literal id="LtlData" runat="server" />];
    
        function setChart(name, categories, data, color) {
            chart.xAxis[0].setCategories(categories, false);
            chart.series[0].remove(false);
            chart.addSeries({
                name: name,
                data: data,
                color: color || 'white'
            }, false);
            chart.redraw();
        }
    
        var chart = $('#container').highcharts({
            chart: {
                type: 'column'
            },
            title: {
                text: '用户关注分析'
            },
            subtitle: {
                text: '点击查看详情，再次点击返回'
            },
            xAxis: {
                categories: categories
            },
            yAxis: {
                title: {
                    text: '新增关注数'
                }
            },
            plotOptions: {
                column: {
                    cursor: 'pointer',
                    point: {
                        events: {
                            click: function() {
                                var drilldown = this.drilldown;
                                if (drilldown) { // drill down
                                    setChart(drilldown.name, drilldown.categories, drilldown.data, drilldown.color);
                                } else { // restore
                                    setChart(name, categories, data);
                                }
                            }
                        }
                    },
                    dataLabels: {
                        enabled: true,
                        color: colors[0],
                        style: {
                            fontWeight: 'bold'
                        },
                        formatter: function() {
                            return formatCurrency(this.y);
                        }
                    }
                }
            },
            tooltip: {
                formatter: function() {
                    var point = this.point;
                    if (point.drilldown) {
                        return '点击查看 '+ point.category +' 详情';
                    } else {
                        return point.category;
                    }
                }
            },
            series: [{
                name: name,
                data: data,
                color: 'white'
            }],
            exporting: {
                enabled: false
            }
        })
        .highcharts(); // return chart
    });
    </script>
    <bairong:Code type="highcharts" runat="server" />

  <div class="popover popover-static">
    <h3 class="popover-title">用户关注分析</h3>
    <div class="popover-content">
    
      <div id="container" style="min-width: 400px; height: 400px; margin: 0 auto"></div>
  
    </div>
  </div>

</form>
</body>
</html>