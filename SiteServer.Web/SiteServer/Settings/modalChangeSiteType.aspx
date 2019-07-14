<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.ModalChangeSiteType" Trace="false"%>
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

        <asp:PlaceHolder ID="PhChangeToSite" runat="server">
            <div class="form-group">
                <label class="col-form-label">站点域名
                <asp:RequiredFieldValidator ControlToValidate="TbDomainName" errorMessage=" *" foreColor="red" Display="Dynamic" runat="server" />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="TbDomainName" ValidationExpression="([\\.a-zA-Z0-9_-]+(:\d*)?[;]{0,1})*"
	                errorMessage=" 只允许包含字母、数字、下划线、中划线、小数点及分号" foreColor="red" Display="Dynamic" />
                </label>
                <asp:TextBox cssClass="form-control" id="TbDomainName" runat="server" />
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="PhChangeToHeadquarters" runat="server">
        </asp:PlaceHolder>
        <hr />
        <div class="text-right mr-1">
          <asp:Button id="BtnSubmit" class="btn btn-primary m-l-5" text="确 定" runat="server" onClick="Submit_OnClick" />
          <button type="button" class="btn btn-default m-l-5" onclick="window.parent.layer.closeAll()">取 消</button>
        </div>
      </form>
    </body>
    </html>
    <!--#include file="../inc/foot.html"-->