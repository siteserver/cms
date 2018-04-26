<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageCreateChannel" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
      <script type="text/javascript" language="javascript">
        function selectAll(isChecked) {
          for (var i = 0; i < document.getElementById('<%=LbChannelIdList.ClientID%>').options.length; i++) {
            document.getElementById('<%=LbChannelIdList.ClientID%>').options[i].selected = isChecked;
          }
        }
      </script>
    </head>

    <body>
      <form class="m-l-15 m-r-15" runat="server">
        <ctrl:alerts runat="server" />

        <div class="card-box">
          <div class="m-t-0 header-title">
            生成栏目页
          </div>
          <p class="text-muted font-13 m-b-25">
            选择需要生成页面的栏目后点击“生成选定栏目”即可生成对应得栏目页面，按住Ctrl可多选
          </p>

          <div class="form-group form-row">
            <label class="col-sm-2 col-form-label">生成选定的栏目</label>
            <div class="col-sm-4">
              <asp:ListBox ID="LbChannelIdList" SelectionMode="Multiple" Rows="19" class="form-control" runat="server"></asp:ListBox>
            </div>
            <div class="col-sm-6">

            </div>
          </div>
          <div class="form-group form-row">
            <label class="col-sm-2 col-form-label">生成范围</label>
            <div class="col-sm-4">
              <asp:DropDownList ID="DdlScope" class="form-control" runat="server"></asp:DropDownList>
            </div>
            <div class="col-sm-6">
              <div class="checkbox checkbox-primary float-left">
                <input id="checkbox" type="checkbox" onClick="selectAll(this.checked);">
                <label for="checkbox">
                  全选
                </label>
              </div>
              <asp:Button class="btn btn-primary float-left" style="margin-bottom:0px;" text="生成选定栏目" onclick="Create_OnClick" runat="server"
              />
            </div>
          </div>

        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->