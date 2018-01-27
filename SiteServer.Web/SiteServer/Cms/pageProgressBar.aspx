<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageProgressBar" Trace="false" EnableViewState="false" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html>

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
    </head>

    <body>
      <form class="m-l-15 m-r-15" runat="server">
        <ctrl:alerts runat="server" />

        <div class="card-box">
          <div class="m-t-0 header-title">
            <asp:Literal ID="LtlTitle" runat="server" />
          </div>
          <p class="text-muted font-13 m-b-25"></p>

          <div id="progressBar" style="margin: 1em 2em 2em 2em;">
            <div id="theMeter">
              <div id="progressBarText" style="font-weight: bold; padding: 5px;">任务完成:
                <font id="prgressBarPercetage">0</font>%</div>
              <div id="progressBarBox" style="width: 350px; height: 25px;  background: #eee;">
                <div id="progressBarBoxContent" style="width: 0; height: 25px; background: #00b19d;"></div>
              </div>
            </div>
          </div>

          <div id="progressError" style="position: relative; margin: 2em; display: none;">
            <p style="padding-left: 25px; padding-bottom: 5px; color: red; text-align: left; vertical-align: middle; background: url(../Pic/icon/error.jpg) no-repeat left top;">执行出错，错误信息为：
              <font id="progressErrorText"></font>
            </p>
          </div>

          <div id="progressWarning" style="position: relative; margin: 2em;">
            <p style="padding-bottom: 5px; text-align: left; vertical-align: middle;">进度：
              <font id="progressWarningText">任务初始化...</font>
            </p>
          </div>

          <asp:Literal ID="LtlRegisterScripts" runat="server" />

        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->