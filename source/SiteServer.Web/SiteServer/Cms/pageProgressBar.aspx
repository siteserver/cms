<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageProgressBar" Trace="false" EnableViewState="false" %>
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
        <asp:Literal ID="ltlBreadCrumb" runat="server" />
        <bairong:Alerts runat="server" />

        <script type="text/javascript" language="javascript">
            function writeProgressBar(totalCount, currentCount, message) {
                if (currentCount > totalCount) {
                    currentCount = totalCount;
                }
                if (totalCount != 0 && currentCount != 0) {
                    $('#prgressBarPercetage').html(parseInt(currentCount * 100 / totalCount) + '');
                    $('#progressBarBoxContent').width(parseInt(currentCount * 350 / totalCount));
                    if (totalCount == currentCount) {
                        $('#progressWarningText').html('任务完成。');
                    }
                    else {
                        $('#progressWarningText').html('(' + currentCount + '/' + totalCount + ')&nbsp;');
                        if (message) $('#progressWarningText').html($('#progressWarningText').html() + message);
                    }
                }
                else {
                    if (message) $('#progressWarningText').html(message);
                }
            }

            function writeResult(resultMessage, errorMessage) {
                $('#progressError').hide();
                $('#progressWarning').hide();
                if (errorMessage != '' && errorMessage != undefined) {
                    $('#progressError').show();
                    $('#progressErrorText').html(errorMessage);
                }
                if (resultMessage != '') {
                    writeProgressBar(1, 1);
                    $('#progressWarning').show();
                    $('#progressWarningText').html(resultMessage);
                }
            }
        </script>

        <div class="popover popover-static">
            <h3 class="popover-title"><asp:Literal ID="LtlTitle" runat="server" /></h3>
            <div class="popover-content">

                <div id="progressBar" style="margin: 1em 2em 2em 2em;">
                    <div id="theMeter">
                        <div id="progressBarText" style="font-weight: bold; padding: 5px;">任务完成: <font id="prgressBarPercetage">0</font>%</div>
                        <div id="progressBarBox" style="width: 350px; height: 20px; border: 1px inset; background: #eee;">
                            <div id="progressBarBoxContent" style="width: 0; height: 20px; border-right: 1px solid #444; background: #9ACB34;"></div>
                        </div>
                    </div>
                </div>
                <div id="progressError" style="position: relative; margin: 2em; display: none;">
                    <p style="padding-left: 25px; padding-bottom: 5px; color: red; text-align: left; vertical-align: middle; background: url(../Pic/icon/error.jpg) no-repeat left top;">执行出错，错误信息为：<font id="progressErrorText"></font></p>
                </div>
                <div id="progressWarning" style="position: relative; margin: 2em;">
                    <p style="padding-left: 25px; padding-bottom: 5px; text-align: left; vertical-align: middle; background: url(../Pic/icon/warn.jpg) no-repeat left top;">进度：<font id="progressWarningText">任务初始化...</font></p>
                </div>

            </div>
        </div>

        <asp:Literal ID="LtlRegisterScripts" runat="server" />

    </form>
</body>
</html>
