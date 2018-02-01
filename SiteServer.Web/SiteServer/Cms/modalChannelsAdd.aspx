<%@ Page Language="C#" ValidateRequest="false" Inherits="SiteServer.BackgroundPages.Cms.ModalChannelsAdd" Trace="false" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html class="modalPage">

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
      <script language="javascript">
        function selectChannel(nodeNames, channelId) {
          $('#nodeNames').html(nodeNames);
          $('#channelId').val(channelId);
        }
      </script>
    </head>

    <body>
      <form runat="server">
        <ctrl:alerts runat="server" />

        <div class="form-group form-row">
          <label class="col-2 col-form-label text-right">父栏目</label>
          <div class="col-9 help-block text-danger">
            <span id="nodeNames" class="m-l-10 m-r-10"></span>
            <input id="channelId" name="channelId" value="0" type="hidden">
            <asp:HyperLink id="HlSelectChannel" class="btn btn-success" runat="server">选择</asp:HyperLink>
            <asp:Literal ID="LtlSelectChannelScript" runat="server"></asp:Literal>
          </div>
          <div class="col-1"></div>
        </div>
        <div class="form-group form-row">
          <label class="col-2 col-form-label text-right">内容模型插件</label>
          <div class="col-4">
            <asp:DropDownList class="form-control" ID="DdlContentModelPluginId" runat="server"></asp:DropDownList>
          </div>
          <div class="col-6"></div>
        </div>
        <asp:PlaceHolder id="PhContentRelatedPluginIds" runat="server">
          <div class="form-group form-row" id="FilePathRow" runat="server">
            <label class="col-2 col-form-label text-right">内容关联插件</label>
            <div class="col-9">
              <asp:CheckBoxList ID="CblContentRelatedPluginIds" CssClass="checkbox checkbox-primary" RepeatDirection="Horizontal" runat="server"></asp:CheckBoxList>
            </div>
            <div class="col-1"></div>
          </div>
        </asp:PlaceHolder>
        <div class="form-group form-row">
          <label class="col-2 col-form-label text-right">栏目模板</label>
          <div class="col-4">
            <asp:DropDownList class="form-control" ID="DdlChannelTemplateId" DataTextField="TemplateName" DataValueField="Id"
              runat="server"></asp:DropDownList>
          </div>
          <div class="col-6"></div>
        </div>
        <div class="form-group form-row">
          <label class="col-2 col-form-label text-right">内容模板</label>
          <div class="col-4">
            <asp:DropDownList class="form-control" ID="DdlContentTemplateId" DataTextField="TemplateName" DataValueField="Id"
              runat="server"></asp:DropDownList>
          </div>
          <div class="col-6"></div>
        </div>
        <div class="form-group form-row">
          <label class="col-2 col-form-label text-right">栏目索引</label>
          <div class="col-4">
            <asp:CheckBox class="checkbox checkbox-primary" Text="将栏目名称作为栏目索引" ID="CbIsNameToIndex" runat="server" />
          </div>
          <div class="col-6"></div>
        </div>
        <div class="form-group form-row">
          <label class="col-2 col-form-label text-right">说明</label>
          <div class="col-10 help-block">
            栏目之间用换行分割，下级栏目在栏目前添加“－”字符，索引可以放到括号中，如：
            <code>
              <br> 栏目一(栏目索引)
              <br> －下级栏目(下级索引)
              <br> －－下下级栏目
            </code>
          </div>
        </div>
        <div class="form-group form-row">
          <label class="col-1 col-form-label"></label>
          <div class="col-10">
            <asp:TextBox class="form-control" Style="width: 98%; height: 240px" TextMode="MultiLine" ID="TbNodeNames" runat="server"
            />
          </div>
          <div class="col-1"></div>
        </div>

        <hr />

        <div class="text-right mr-1">
          <asp:Button class="btn btn-primary m-l-5" text="确 定" runat="server" onClick="Submit_OnClick" />
          <button type="button" class="btn btn-default m-l-5" onclick="window.parent.layer.closeAll()">取 消</button>
        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->