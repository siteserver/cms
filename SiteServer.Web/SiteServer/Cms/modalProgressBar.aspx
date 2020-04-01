<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalProgressBar" Trace="false" EnableViewState="false" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html class="modalPage">

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
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
            } else {
              $('#progressWarningText').html('(' + currentCount + '/' + totalCount + ')&nbsp;');
              if (message) $('#progressWarningText').html($('#progressWarningText').html() + message);
            }
          } else {
            if (message) $('#progressWarningText').html(message);
          }
        }

        function writeResult(resultMessage, errorMessage) {
          $('#progressError').hide();
          $('#progressWarning').hide();
          if (errorMessage != '') {
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
    </head>

    <body>
      <form runat="server">
        <ctrl:alerts runat="server" />

        <div id="progressBar" style="margin: 1em 2em 2em 2em;">
          <div id="theMeter">
            <div id="progressBarText" style="font-weight: bold; padding: 5px;">任务完成:
              <span id="prgressBarPercetage">0</span>%</div>
            <div id="progressBarBox" style="width: 350px; height: 20px; background: #eee;">
              <div id="progressBarBoxContent" style="width: 0; height: 20px; background: #00b19d;"></div>
            </div>
          </div>
        </div>
        <div id="progressError" style="position:relative; margin: 2em; display:none;">
          <p style=" padding-left: 25px; padding-bottom: 5px; color:red; text-align: left; vertical-align: middle; background:url(../Pic/icon/error.jpg) no-repeat left top;">
            执行出错，错误信息为：
            <span id="progressErrorText"></span>
          </p>
        </div>
        <div id="progressWarning" style="position:relative; margin: 2em;">
          <p style=" padding-left: 25px; padding-bottom: 5px; text-align: left; vertical-align: middle; background:url(../Pic/icon/warn.jpg) no-repeat left top;">
            进度：
            <span id="progressWarningText">任务初始化...</span>
          </p>
        </div>

        <asp:Literal id="LtlScripts" runat="server" />

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->