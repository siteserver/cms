<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageTrackerYear" %>
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
  <asp:Literal id="ltlBreadCrumb" runat="server" />
  <bairong:alerts runat="server" />

  <div class="popover popover-static">
    <h3 class="popover-title">访问量年分配图表</h3>
    <div class="popover-content">
    
                      <div id="hits" style="height: 400px"></div>
                <!-- ECharts单文件引入 -->
                <script src="../assets/echarts/echarts.js"></script>
                <script type="text/javascript">
                    // 路径配置
                    require.config({
                        paths: {
                            echarts: '../assets/echarts'
                        }
                    });
                    // 使用
                    require(
                        [
                            'echarts',
                            'echarts/chart/bar' // 使用柱状图就加载bar模块，按需加载
                        ],
                        function (ec) {
                            // 基于准备好的dom，初始化echarts图表
                            var myChart = ec.init(document.getElementById('hits'));
                            //x array
                            var xArray = [];
                            //y array
                            var yArray = [];

                    <%for (int i = 1; i <= this.count; i++)
                      {%>
                            xArray.push('<%=GetGraphicX(i)%>');
                            yArray.push('<%=GetGraphicY(i)%>');
                    <%}%>

                            var option = {
                                tooltip: {
                                    show: true
                                },
                                legend: {
                                    data: ['访问量']
                                },
                                xAxis: [
                                    {
                                        type: 'category',
                                        data: xArray
                                    }
                                ],
                                yAxis: [
                                    {
                                        type: 'value'
                                    }
                                ],
                                series: [
                                    {
                                        "name": "访问量",
                                        "type": "bar",
                                        "data": yArray
                                    }
                                ]
                            };

                            // 为echarts对象加载数据 
                            myChart.setOption(option);
                        }
            );
                </script>
  
    </div>
  </div>

  <div class="popover popover-static">
    <h3 class="popover-title">访客年分配图表</h3>
    <div class="popover-content">
    
                      <div id="user" style="height: 400px"></div>
                <!-- ECharts单文件引入 -->
                <script type="text/javascript">
                    // 路径配置
                    require.config({
                        paths: {
                            echarts: '../assets/echarts'
                        }
                    });
                    // 使用
                    require(
                        [
                            'echarts',
                            'echarts/chart/bar' // 使用柱状图就加载bar模块，按需加载
                        ],
                        function (ec) {
                            // 基于准备好的dom，初始化echarts图表
                            var myChart = ec.init(document.getElementById('user'));
                            //x array
                            var xArray = [];
                            //y array
                            var yArray = [];

                    <%for (int i = 1; i <= this.count; i++)
                      {%>
                            xArray.push('<%=GetGraphicX(i)%>');
                            yArray.push('<%=GetUniqueGraphicY(i)%>');
                    <%}%>

                            var option = {
                                tooltip: {
                                    show: true
                                },
                                legend: {
                                    data: ['访客量']
                                },
                                xAxis: [
                                    {
                                        type: 'category',
                                        data: xArray
                                    }
                                ],
                                yAxis: [
                                    {
                                        type: 'value'
                                    }
                                ],
                                series: [
                                    {
                                        "name": "访客量",
                                        "type": "bar",
                                        "data": yArray
                                    }
                                ]
                            };

                            // 为echarts对象加载数据 
                            myChart.setOption(option);
                        }
            );
                </script>
  
    </div>
  </div>

  <ul class="breadcrumb breadcrumb-button">
    <asp:Button class="btn" ID="ExportTracking" runat="server" Text="导出Excel"></asp:Button>
  </ul>

</form>
</body>
</html>
