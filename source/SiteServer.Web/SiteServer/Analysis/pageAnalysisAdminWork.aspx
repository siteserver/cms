<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Analysis.PageAnalysisAdminWork" EnableViewState="false" %>
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
        <bairong:Alerts runat="server" />
        <asp:Literal ID="ltlBreadCrumb" runat="server" />

        <div class="well well-small">
            开始时间：
            <bairong:DateTimeTextBox ID="TbStartDate" class="input-small" Columns="30" runat="server" />
            结束时间：
            <bairong:DateTimeTextBox ID="TbEndDate" class="input-small" Columns="30" runat="server" />
            <asp:Button class="btn" style="margin-bottom: 0px;" OnClick="Analysis_OnClick" Text="分 析" runat="server" />
        </div>

        <div class="popover popover-static">
            <h3 class="popover-title">按栏目统计</h3>
            <div class="popover-content">

                <div style="width: 100%">
                    <!-- 为ECharts准备一个具备大小（宽高）的Dom -->
                    <div id="new" style="height: 400px; width: 90%; display: inline-block"></div>
                    <!-- ECharts单文件引入 -->
                    <script src="../assets/echarts/echarts.js"></script>
                    <script type="text/javascript">
                        // 路径配置
                        require.config({
                            paths: {
                                echarts: '../assets/echarts'
                            }
                        });
                        // 新增信息数目
                        require(
                            [
                                'echarts',
                                'echarts/chart/bar' // 使用柱状图就加载bar模块，按需加载
                            ],
                            function (ec) {
                                // 基于准备好的dom，初始化echarts图表
                                var newChart = ec.init(document.getElementById('new'));
                                //x array
                                var xArrayNew = [];

                                //y array
                                var yArrayNew = [];
                                var yArrayUpdate = [];
                                //title
                                var newTitle = "新增信息数目";
                                var updateTitle = "更新信息数目";

                                <asp:Literal id="LtlArray1" runat="server"></asp:Literal>

                                if (xArrayNew.length == 0) {
                                    xArrayNew = ["暂无数据"];
                                    yArrayNew = [0];
                                }

                                var option = {
                                    tooltip: {
                                        show: true
                                    },
                                    legend: {
                                        data: [newTitle, updateTitle]
                                    },
                                    toolbox: {
                                        show: true,
                                        feature: {
                                            dataView: { show: true, readOnly: false },
                                            restore: { show: true },
                                            saveAsImage: { show: true }
                                        }
                                    },
                                    xAxis: [
                                        {
                                            type: 'category',
                                            data: []
                                        }
                                    ],
                                    yAxis: [
                                        {
                                            type: 'value'
                                        }
                                    ],
                                    series: [
                                        {
                                            "name": newTitle,
                                            "type": "bar",
                                            "data": []
                                        },
                                       {
                                           "name": updateTitle,
                                           "type": "bar",
                                           "data": []
                                       },
                                    ]
                                };
                                // 新增
                                option.xAxis[0].data = xArrayNew;
                                option.series[0].data = yArrayNew;
                                //更新
                                option.series[1].data = yArrayUpdate;
                                newChart.setOption(option);
                            }
                    );
                    </script>
                </div>

            </div>
        </div>

        <div class="popover popover-static">
            <h3 class="popover-title">按栏目统计</h3>
            <div class="popover-content">
                <table class="table table-bordered table-hover">
                    <tr class="info thead">
                        <td>栏目名 </td>
                        <td width="70">新增内容 </td>
                        <td width="70">修改内容 </td>
                    </tr>
                    <asp:Repeater ID="RptChannels" runat="server">
                        <ItemTemplate>
                            <bairong:NoTagText ID="ElHtml" runat="server" />
                        </ItemTemplate>
                    </asp:Repeater>
                </table>
            </div>
        </div>

        <div class="popover popover-static">
            <h3 class="popover-title">按管理员统计</h3>
            <div class="popover-content">
                <div style="width: 100%">
                    <!-- 为ECharts准备一个具备大小（宽高）的Dom -->
                    <div id="user" style="height: 400px; width: 90%; display: inline-block"></div>
                    <!-- ECharts单文件引入 -->
                    <script type="text/javascript">
                        // 路径配置
                        require.config({
                            paths: {
                                echarts: '../assets/echarts'
                            }
                        });
                        // 新增信息数目
                        require(
                            [
                                'echarts',
                                'echarts/chart/bar' // 使用柱状图就加载bar模块，按需加载
                            ],
                            function (ec) {
                                // 基于准备好的dom，初始化echarts图表
                                var newChart = ec.init(document.getElementById('user'));
                                //x array
                                var xArrayNew = [];

                                //y array
                                var yArrayNew = [];
                                var yArrayUpdate = [];
                                //title
                                var newTitle = "新增信息数目";
                                var updateTitle = "更新信息数目";

                                <asp:Literal id="LtlArray2" runat="server"></asp:Literal>

                                if (xArrayNew.length == 0) {
                                    xArrayNew = ["暂无数据"];
                                    yArrayNew = [0];
                                }

                                var option = {
                                    tooltip: {
                                        show: true
                                    },
                                    legend: {
                                        data: [newTitle, updateTitle]
                                    },
                                    toolbox: {
                                        show: true,
                                        feature: {
                                            dataView: { show: true, readOnly: false },
                                            restore: { show: true },
                                            saveAsImage: { show: true }
                                        }
                                    },
                                    xAxis: [
                                        {
                                            type: 'category',
                                            data: []
                                        }
                                    ],
                                    yAxis: [
                                        {
                                            type: 'value'
                                        }
                                    ],
                                    series: [
                                        {
                                            "name": newTitle,
                                            "type": "bar",
                                            "data": []
                                        },
                                       {
                                           "name": updateTitle,
                                           "type": "bar",
                                           "data": []
                                       }
                                    ]
                                };
                                // 新增
                                option.xAxis[0].data = xArrayNew;
                                option.series[0].data = yArrayNew;
                                //更新
                                option.series[1].data = yArrayUpdate;
                                newChart.setOption(option);
                            }
                    );
                    </script>
                </div>
            </div>
        </div>

        <div class="popover popover-static">
            <h3 class="popover-title">按管理员统计</h3>
            <div class="popover-content">
                <table class="table table-bordered table-hover">
                    <tr class="info thead">
                        <td>登录名 </td>
                        <td>显示名 </td>
                        <td width="70">新增内容 </td>
                        <td width="70">更新内容 </td>
                    </tr>
                    <asp:Repeater ID="RptContents" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td>
                                    <asp:Literal ID="ltlUserName" runat="server"></asp:Literal></td>
                                <td>
                                    <asp:Literal ID="ltlDisplayName" runat="server"></asp:Literal></td>
                                <td>
                                    <asp:Literal ID="ltlContentAdd" runat="server"></asp:Literal></td>
                                <td>
                                    <asp:Literal ID="ltlContentUpdate" runat="server"></asp:Literal></td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </table>
            </div>
        </div>

        <bairong:SqlPager ID="SpContents" runat="server" class="table table-pager" />

    </form>
</body>
</html>
