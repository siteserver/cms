﻿<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Analysis.PageAnalysisUser" %>
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
        <script src="../assets/echarts/echarts.js"></script>
        <asp:Literal ID="ltlBreadCrumb" runat="server" />
        <bairong:Alerts runat="server" />

        <div class="well well-small">
            <div id="contentSearch" style="margin-top: 10px;">
                时间从：
                <bairong:DateTimeTextBox ID="TbDateFrom" class="input-small" Columns="12" runat="server" />
                到：
                <bairong:DateTimeTextBox ID="TbDateTo" class="input-small" Columns="12" runat="server" />
                x轴：
                <asp:DropDownList ID="DdlXType" runat="server"></asp:DropDownList>
                <asp:Button class="btn" OnClick="Search_OnClick" Text="搜 索" runat="server" />
            </div>
        </div>


        <div class="popover popover-static">
            <h3 class="popover-title"><asp:Literal id="LtlPageTitle" runat="server"></asp:Literal></h3>
            <div class="popover-content">
                <!-- 为ECharts准备一个具备大小（宽高）的Dom -->
                <div id="main" style="height: 400px"></div>
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
                            'echarts/chart/line' // 使用柱状图就加载bar模块，按需加载
                        ],
                        function (ec) {
                            // 基于准备好的dom，初始化echarts图表
                            var myChart = ec.init(document.getElementById('main'));
                            //x array
                            var xArray = [];
                            //y array
                            var yArray = [];

                            <asp:Literal id="LtlArray" runat="server"></asp:Literal>

                            var option = {
                                tooltip: {
                                    show: true
                                },
                                legend: {
                                    data: ['用户']
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
                                        "name": "增加量",
                                        "type": "line",
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

    </form>
</body>
</html>
