<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Sys.PagePublishmentSystemReplace" %>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>
<head>
<meta charset="utf-8">
<!--#include file="../inc/header.aspx"-->
</head>

<body>
<!--#include file="../inc/openWindow.html"-->
<form class="form-inline" runat="server">
  <asp:Literal id="ltlBreadCrumb" runat="server" />
  <bairong:alerts runat="server" />

  <div class="popover popover-static">
    <h3 class="popover-title">整站替换</h3>
    <div class="popover-content">
    
      <asp:PlaceHolder id="ChooseSiteTemplate" runat="server">

      <blockquote>
        <p>欢迎使用整站替换向导，整站替换将改变现有网站，请谨慎使用</p>
        <small>您选择的站点为<strong>
        <asp:Literal ID="PublishmentSystemName" runat="server"></asp:Literal>
        </strong>，请选择需要替换的站点模板：</small>
      </blockquote>

      <input type="hidden" id="SiteTemplateDir" value="" runat="server" />

      <asp:dataGrid id="dgContents" showHeader="true" AutoGenerateColumns="false" DataKeyField="Name" HeaderStyle-CssClass="info thead" CssClass="table table-bordered table-hover" runat="server">
        <Columns>
          <asp:TemplateColumn HeaderText="选择">
            <ItemTemplate>
              <input type="radio" name="choose" id="choose" onClick="document.getElementById('SiteTemplateDir').value=this.value;" value='<%# DataBinder.Eval(Container.DataItem,"Name")%>' />
            </ItemTemplate>
            <ItemStyle Width="50" cssClass="center" />
          </asp:TemplateColumn>
          <asp:TemplateColumn HeaderText="站点模板名称">
            <ItemTemplate> <%#GetSiteTemplateName((string)DataBinder.Eval(Container.DataItem,"Name")) %> </ItemTemplate>
            <ItemStyle Width="120" cssClass="center"/>
          </asp:TemplateColumn>
          <asp:BoundColumn HeaderText="站点模板文件夹" DataField="Name" >
            <ItemStyle Width="100" cssClass="center" />
          </asp:BoundColumn>
          <asp:TemplateColumn HeaderText="站点模板介绍">
            <ItemTemplate> <%#GetDescription((string)DataBinder.Eval(Container.DataItem,"Name")) %> </ItemTemplate>
            <ItemStyle HorizontalAlign="left" />
          </asp:TemplateColumn>
          <asp:TemplateColumn HeaderText="样图">
            <ItemTemplate> <%#GetSamplePicHtml((string)DataBinder.Eval(Container.DataItem,"Name")) %> </ItemTemplate>
            <ItemStyle Width="130" cssClass="center"/>
          </asp:TemplateColumn>
        </Columns>
      </asp:dataGrid>

    </asp:PlaceHolder>

    <asp:PlaceHolder id="CreateSiteParameters" runat="server" Visible="false">

      <blockquote>
        <p>设置导入选项</p>
        <small>您选择的站点模板为：
        <asp:Literal ID="ltlSiteTemplateName" runat="server"></asp:Literal>
        ，请设置导入选项：</small>
      </blockquote>

      <table class="table table-noborder table-hover">
        <tr>
          <td width="160">清除站点栏目及内容：</td>
          <td>
            <asp:RadioButtonList ID="IsDeleteChannels" runat="server" RepeatDirection="Horizontal" class="noborder">
              <asp:ListItem Text="清除站点栏目及内容" Value="True"></asp:ListItem>
              <asp:ListItem Text="保留站点栏目及内容" Value="False" Selected="true"></asp:ListItem>
            </asp:RadioButtonList>
          </td>
        </tr>
        <tr>
          <td>清除站点显示模板：</td>
          <td>
            <asp:RadioButtonList ID="IsDeleteTemplates" runat="server" RepeatDirection="Horizontal" class="noborder">
              <asp:ListItem Text="清除站点显示模板" Value="True"></asp:ListItem>
              <asp:ListItem Text="保留站点显示模板" Value="False" Selected="true"></asp:ListItem>
            </asp:RadioButtonList>
          </td>
        </tr>
        <tr>
          <td>清除站点文件：</td>
          <td>
            <asp:RadioButtonList ID="IsDeleteFiles" runat="server" RepeatDirection="Horizontal" class="noborder">
              <asp:ListItem Text="清除站点文件" Value="True"></asp:ListItem>
              <asp:ListItem Text="保留站点文件" Value="False" Selected="true"></asp:ListItem>
            </asp:RadioButtonList>
          </td>
        </tr>
        <tr>
          <td>是否覆盖同名数据：</td>
          <td>
            <asp:RadioButtonList ID="IsOverride" runat="server" RepeatDirection="Horizontal" class="noborder">
              <asp:ListItem Text="覆盖同名数据" Value="True" Selected="true"></asp:ListItem>
              <asp:ListItem Text="不覆盖同名数据" Value="False"></asp:ListItem>
            </asp:RadioButtonList>
          </td>
        </tr>
      </table>

    </asp:PlaceHolder>

    <asp:PlaceHolder id="OperatingError" runat="server" Visible="false">

      <blockquote style="margin-top:20px;">
        <p>发生错误</p>
        <small>执行向导过程中出错</small>
      </blockquote>

      <div class="alert alert-block">
        <h4><asp:Literal ID="ltlErrorMessage" runat="server"></asp:Literal></h4>
      </div>

      </asp:PlaceHolder>
  
      <hr />
      <table class="table noborder">
        <tr>
          <td class="center">
            <asp:Button class="btn" ID="Previous" OnClick="PreviousPlaceHolder" CausesValidation="false" runat="server" Text="< 上一步"></asp:Button>
            <asp:Button class="btn btn-primary" id="Next" onclick="NextPlaceHolder" runat="server" text="下一步 >"></asp:button>
          </td>
        </tr>
      </table>
  
    </div>
  </div>

</form>
</body>
</html>
