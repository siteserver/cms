<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalContentAttributes" Trace="false"%>
  <%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html class="modalPage">

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
      <script language="javascript" type="text/javascript">
        function _toggleTab(no, totalNum) {
          $("#tab" + no).removeClass("tabOff");
          $("#tab" + no).addClass("tabOn");
          $("#column" + no).show();

          document.getElementById("HihType").value = no + "";
          for (var i = 1; i <= totalNum; i++) {
            if (i != no) {
              $("#tab" + i).removeClass("tabOn");
              $("#tab" + i).addClass("tabOff");
              if (i != 2) {
                $("#column" + i).hide();
              }
            }
          }
        }

        function _toggleTab2(no, totalNum) {

          $("#tab" + no).removeClass("tabOff");
          $("#tab" + no).addClass("tabOn");
          $("#column" + 1).show();

          document.getElementById("HihType").value = no + "";
          for (var i = 1; i <= totalNum; i++) {
            if (i != no) {
              $("#tab" + i).removeClass("tabOn");
              $("#tab" + i).addClass("tabOff");
              $("#column" + 3).hide();
            }
          }
        }

        function _toggleTab(no, totalNum) {
          document.getElementById("HihType").value = no + "";
          $('#tab' + no).addClass("active");

          for (var i = 1; i <= totalNum; i++) {
            if (i != no) {
              $('#tab' + i).removeClass("active");
              $('#column' + i).hide();
            }
          }

          if (no == 2) no = 1;
          $('#column' + no).show();
        }
      </script>
    </head>

    <body>
      <!--#include file="../inc/openWindow.html"-->

      <form runat="server">
        <bairong:alerts runat="server" />
        <input id="HihType" type="hidden" runat="server" value="1" />

        <div class="raw">
          <ul class="nav nav-tabs tabs m-b-10">
            <li id="tab1" class="active tab">
              <a href="javascript:;" onClick="_toggleTab(1,3);">设置属性</a>
            </li>
            <li id="tab2" class="tab">
              <a href="javascript:;" onClick="_toggleTab(2,3);">取消属性</a>
            </li>
            <li id="tab3" class="tab">
              <a href="javascript:;" onClick="_toggleTab(3,3);">设置点击量</a>
            </li>
          </ul>
        </div>

        <div class="form-horizontal">

          <div class="form-group" id="column1">
            <label class="col-xs-3 control-label text-right">属性</label>
            <div class="col-xs-8">
              <asp:CheckBox class="checkbox checkbox-primary" ID="IsRecommend" runat="server" Text="推荐"></asp:CheckBox>
              <asp:CheckBox class="checkbox checkbox-primary" ID="IsHot" runat="server" Text="热点"></asp:CheckBox>
              <asp:CheckBox class="checkbox checkbox-primary" ID="IsColor" runat="server" Text="醒目"></asp:CheckBox>
              <asp:CheckBox class="checkbox checkbox-primary" ID="IsTop" runat="server" Text="置顶"></asp:CheckBox>
            </div>
            <div class="col-xs-1">

            </div>
          </div>

          <div class="form-group" id="column3" style="display: none">
            <label class="col-xs-3 control-label text-right">点击量</label>
            <div class="col-xs-8">
              <asp:TextBox class="form-control" MaxLength="50" id="Hits" Text="0" runat="server" />
            </div>
            <div class="col-xs-1">
              <asp:RegularExpressionValidator ControlToValidate="Hits" ValidationExpression="\d+" Display="Dynamic" ErrorMessage="点击量必须为整数"
                foreColor="red" runat="server" />
            </div>
          </div>

          <hr />

          <div class="form-group m-b-0">
            <div class="col-xs-11 text-right">
              <asp:Button class="btn btn-primary m-l-10" text="确 定" runat="server" onClick="Submit_OnClick" />
              <button type="button" class="btn btn-default m-l-10" onclick="window.parent.layer.closeAll()">取 消</button>
            </div>
            <div class="col-xs-1"></div>
          </div>
        </div>

      </form>
    </body>

    </html>