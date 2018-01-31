<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageChannelTranslate" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
      <script>
        function setOptionColor(obj) {
          for (var i = 0; i < obj.options.length; i++) {
            if (obj.options[i].value == "") {
              obj.options[i].style.color = "gray";
            } else {
              obj.options[i].style.color = "black";
            }
          }
        }
        $(document).ready(function () {
          setOptionColor(document.getElementById('<%=LbChannelIdFrom.ClientID%>'));
        });
      </script>
    </head>

    <body>
      <form class="m-l-15 m-r-15" runat="server">
        <ctrl:alerts runat="server" />

        <div class="card-box">
          <div class="m-t-0 header-title">
            批量转移
          </div>
          <p class="text-muted font-13 m-b-25"></p>

          <div class="form-group form-row">
            <label class="col-sm-2 col-form-label">从栏目</label>
            <div class="col-sm-4">
              <asp:ListBox ID="LbChannelIdFrom" Height="360" style="width:auto" class="form-control" SelectionMode="Multiple" runat="server"></asp:ListBox>
            </div>
            <div class="col-sm-6">
              <asp:RequiredFieldValidator ControlToValidate="LbChannelIdFrom" errorMessage=" *" foreColor="red" Display="Dynamic" runat="server"
              />
            </div>
          </div>

          <div class="form-group form-row">
            <label class="col-sm-2 col-form-label">转移到站点</label>
            <div class="col-sm-4">
              <asp:DropDownList ID="DdlSiteId" class="form-control" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DdlSiteId_OnSelectedIndexChanged"></asp:DropDownList>
            </div>
            <div class="col-sm-6"></div>
          </div>

          <div class="form-group form-row">
            <label class="col-sm-2 col-form-label">转移到栏目</label>
            <div class="col-sm-4">
              <asp:DropDownList ID="DdlChannelIdTo" class="form-control" runat="server"></asp:DropDownList>
            </div>
            <div class="col-sm-6">
              <asp:RequiredFieldValidator ControlToValidate="DdlChannelIdTo" errorMessage=" *" foreColor="red" Display="Dynamic" runat="server"
              />
            </div>
          </div>

          <div class="form-group form-row">
            <label class="col-sm-2 col-form-label">转移类型</label>
            <div class="col-sm-4">
              <asp:DropDownList ID="DdlTranslateType" class="form-control" runat="server"></asp:DropDownList>
            </div>
            <div class="col-sm-6"></div>
          </div>

          <div class="form-group form-row">
            <label class="col-sm-2 col-form-label">转移后删除</label>
            <div class="col-sm-4">
              <asp:RadioButtonList id="RblIsDeleteAfterTranslate" RepeatDirection="Horizontal" class="radio radio-primary" runat="server">
                <asp:ListItem Text="是" Value="True" />
                <asp:ListItem Text="否" Value="False" Selected="True" />
              </asp:RadioButtonList>
            </div>
            <div class="col-sm-6"></div>
          </div>

          <hr />

          <div class="form-group m-b-0">
            <div class="col-sm-12 text-center">
              <asp:Button class="btn btn-primary" id="BtnSubmit" text="转 移" onclick="Submit_OnClick" runat="server" />
              <asp:PlaceHolder ID="PhReturn" runat="server">
                <input class="btn m-l-5" type="button" onClick="location.href='<%=ReturnUrl%>';return false;" value="返 回" />
              </asp:PlaceHolder>
            </div>
          </div>

        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->