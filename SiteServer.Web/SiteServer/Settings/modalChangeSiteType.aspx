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
          <div class="form-group form-row">
            <label class="col-3 text-right col-form-label">站点文件夹名称</label>
            <div class="col-8">
              <asp:TextBox cssClass="form-control" id="TbSiteDir" runat="server" />
              <small class="form-text text-muted">实际在服务器中保存此网站的文件夹名称，此路径必须以英文或拼音命名</small>
            </div>
            <div class="col-1">
              <asp:RequiredFieldValidator ControlToValidate="TbSiteDir" errorMessage=" *" foreColor="red" Display="Dynamic"
                runat="server" />
              <asp:RegularExpressionValidator runat="server" ControlToValidate="TbSiteDir" ValidationExpression="[^']+" errorMessage=" *"
                foreColor="red" Display="Dynamic" />
            </div>
          </div>
          <div class="form-group form-row">
            <label class="col-3 text-right col-form-label">从系统根目录选择需要转移到子站点的文件夹及文件</label>
            <div class="col-8">
              <asp:CheckBoxList ID="CblFilesToSite" class="checkbox checkbox-primary" RepeatColumns="4" runat="server"></asp:CheckBoxList>
            </div>
            <div class="col-1">

            </div>
          </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="PhChangeToHeadquarters" runat="server">
          <div class="form-group form-row">
            <label class="col-3 text-right col-form-label">转移文件夹及文件</label>
            <div class="col-8">
              <asp:DropDownList ID="DdlIsMoveFiles" runat="server" class="form-control">
                <asp:ListItem Text="转移" Value="true" Selected="true"></asp:ListItem>
                <asp:ListItem Text="不转移" Value="false"></asp:ListItem>
              </asp:DropDownList>
              <small class="form-text text-muted">选择转移将把此站点内的文件夹及文件转移到系统根目录</small>
            </div>
            <div class="col-1">

            </div>
          </div>
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