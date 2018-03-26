<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalTableStyleValidateAdd" Trace="false" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html class="modalPage">

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <form runat="server">
        <ctrl:alerts runat="server" />

        <div class="form-group form-row">
          <label class="col-2 text-right col-form-label">是否启用表单验证</label>
          <div class="col-6">
            <asp:DropDownList id="DdlIsValidate" class="form-control" OnSelectedIndexChanged="DdlValidate_SelectedIndexChanged" AutoPostBack="true"
              runat="server">
              <asp:ListItem Text="启用" />
              <asp:ListItem Text="不启用" Selected="True" />
            </asp:DropDownList>
          </div>
          <div class="col-4"></div>
        </div>

        <asp:PlaceHolder ID="PhValidate" runat="server">

          <div class="form-group form-row">
            <label class="col-2 text-right col-form-label">是否为必填项</label>
            <div class="col-6">
              <asp:DropDownList id="DdlIsRequired" class="form-control" runat="server">
                <asp:ListItem Text="是" />
                <asp:ListItem Text="否" Selected="True" />
              </asp:DropDownList>
            </div>
            <div class="col-4"></div>
          </div>

          <asp:PlaceHolder ID="PhNum" runat="server">
            <div class="form-group form-row">
              <label class="col-2 text-right col-form-label">最小字符数</label>
              <div class="col-6">
                <asp:TextBox class="form-control" MaxLength="50" Text="0" id="TbMinNum" runat="server" />
              </div>
              <div class="col-4">
                个字符（0代表不限制）
                <asp:RegularExpressionValidator ControlToValidate="TbMinNum" ValidationExpression="\d+" Display="Dynamic" errorMessage=" *"
                  foreColor="red" runat="server" />
              </div>
            </div>
            <div class="form-group form-row">
              <label class="col-2 text-right col-form-label">最大字符数</label>
              <div class="col-6">
                <asp:TextBox class="form-control" MaxLength="50" Text="0" id="TbMaxNum" runat="server" />
              </div>
              <div class="col-4">
                个字符（0代表不限制）
                <asp:RegularExpressionValidator ControlToValidate="TbMaxNum" ValidationExpression="\d+" Display="Dynamic" errorMessage=" *"
                  foreColor="red" runat="server" />
              </div>
            </div>
          </asp:PlaceHolder>

          <div class="form-group form-row">
            <label class="col-2 text-right col-form-label">高级验证</label>
            <div class="col-6">
              <asp:DropDownList ID="DdlValidateType" class="form-control" OnSelectedIndexChanged="DdlValidate_SelectedIndexChanged" AutoPostBack="true"
                runat="server"></asp:DropDownList>
            </div>
            <div class="col-4"></div>
          </div>

          <asp:PlaceHolder ID="PhRegExp" runat="server">
            <div class="form-group form-row">
              <label class="col-2 text-right col-form-label">验证正则表达式</label>
              <div class="col-6">
                <asp:TextBox class="form-control" TextMode="MultiLine" id="TbRegExp" runat="server" />
              </div>
              <div class="col-4"></div>
            </div>
          </asp:PlaceHolder>

          <div class="form-group form-row">
            <label class="col-2 text-right col-form-label">验证失败提示信息</label>
            <div class="col-6">
              <asp:TextBox class="form-control" TextMode="MultiLine" id="TbErrorMessage" runat="server" />
            </div>
            <div class="col-4">
              不设置系统将使用默认提示
            </div>
          </div>

        </asp:PlaceHolder>

        <hr />

        <div class="text-right mr-1">
          <asp:Button class="btn btn-primary m-l-5" text="确 定" runat="server" onClick="Submit_OnClick" />
          <button type="button" class="btn btn-default m-l-5" onclick="window.parent.layer.closeAll()">取 消</button>
        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->