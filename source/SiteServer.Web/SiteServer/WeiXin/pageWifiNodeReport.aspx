<%@ Page Language="C#" Inherits="SSiteServer.BackgroundPages.WeiXin.PageifiNodeReport" %>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <!--#include file="../inc/header.aspx"-->
    <link href="css/echartsHome.css" rel="stylesheet" />
    <script src="js/echarts/esl.js"></script>
</head>
<body>
    <!--#include file="../inc/openWindow.html"-->

    <form class="form-inline" runat="server">
        <asp:Literal ID="LtlBreadCrumb" runat="server" />
        <bairong:Alerts runat="server" />

        <script type="text/javascript">
            $(document).ready(function () {
                loopRows(document.getElementById('contents'), function (cur) { cur.onclick = chkSelect; });
                $(".popover-hover").popover({ trigger: 'hover', html: true });
            });
        </script>
        <div class="well well-small">
            <table class="table table-noborder">
                <tr>
                    <td>路由器列表：
                        <asp:DropDownList ID="DdlWifiNode" runat="server"></asp:DropDownList>
                        开始时间：
                        <bairong:DateTimeTextBox ID="TbBeginTime" Columns="30" runat="server"/>
                        结束时间：
                        <bairong:DateTimeTextBox ID="TbEndTime" Columns="30" runat="server"/>
                        <asp:Button class="btn" ID="Search" OnClick="Search_OnClick" Text="搜 索" runat="server" />
                    </td>
                </tr>
            </table>
        </div>
        <div class="container-fluid">
            <div class="row-fluid example">
                <!--/span-->
                <div class="col-md-12">
                    <div id="mainLineReport" class="main"></div>                     
                    <asp:TextBox style="display:none;" class="input-xlarge" ID="TbOptionJson" runat="server" />
                </div>
                <!--/span-->
            </div>
            <!--/row-->
        </div>
        <!--/.fluid-container-->
    </form>

    <script type="text/javascript">

        require.config({
            paths: {
                echarts: './js/echarts/echarts',
                'echarts/chart/bar': './js/echarts/echarts',
                'echarts/chart/line': './js/echarts/echarts'
            }
        });

        require(
            [
                'echarts',
                'echarts/chart/bar',
                'echarts/chart/line'
            ],
            function (ec) {

                var myChart = ec.init(document.getElementById('mainLineReport'));

                var option = $("#<%=tbOptionJson.ClientID%>").val();
                option = eval('(' + option + ')');                
                myChart.setOption(option);
            }
        );
        
    </script>
</body>
</html>
