<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalTemplateFilePathRule" Trace="false"%>
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

        <div class="form-horizontal">

          <asp:PlaceHolder id="PhFilePath" runat="server">
            <div class="form-group">
              <label class="col-xs-2 text-right control-label">生成页面路径</label>
              <div class="col-xs-6">
                <asp:TextBox cssClass="form-control" id="TbFilePath" runat="server" />
              </div>
              <div class="col-xs-4 help-block">
                <asp:RegularExpressionValidator runat="server" ControlToValidate="TbFilePath" ValidationExpression="[^']+" errorMessage=" *"
                  foreColor="red" Display="Dynamic" />
              </div>
            </div>
          </asp:PlaceHolder>

          <div class="form-group">
            <label class="col-xs-2 text-right control-label">下级栏目页面命名规则</label>
            <div class="col-xs-5">
              <asp:TextBox cssClass="form-control" id="TbChannelFilePathRule" runat="server" />
            </div>
            <div class="col-xs-1">
              <asp:Button ID="BtnCreateChannelRule" class="btn btn-success" runat="server" text="构造" />
            </div>
            <div class="col-xs-4 help-block">
              系统生成文件时采取的下级栏目页文件名规则
            </div>
          </div>

          <div class="form-group">
            <label class="col-xs-2 text-right control-label">下级内容页面命名规则</label>
            <div class="col-xs-5">
              <asp:TextBox cssClass="form-control" id="TbContentFilePathRule" runat="server" />
            </div>
            <div class="col-xs-1">
              <asp:Button ID="BtnCreateContentRule" class="btn btn-success" runat="server" text="构造" />
            </div>
            <div class="col-xs-4 help-block">
              系统生成文件时采取的下级内容页文件名规则
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