<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Sys.PagePublishmentSystemEdit" %>
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
    <h3 class="popover-title">修改站点</h3>
    <div class="popover-content">

      <table class="table noborder table-hover">
        <tr>
          <td width="160">站点名称：</td>
          <td>
            <asp:TextBox Columns="25" MaxLength="50" id="PublishmentSystemName" runat="server"/>
            <asp:RequiredFieldValidator
              ControlToValidate="PublishmentSystemName"
              errorMessage=" *" foreColor="red"
              Display="Dynamic"
              runat="server"/>
            <asp:RegularExpressionValidator
              runat="server"
              ControlToValidate="PublishmentSystemName"
              ValidationExpression="[^']+"
              errorMessage=" *" foreColor="red"
              Display="Dynamic" />
          </td>
        </tr>
        <tr id="PublishmentSystemDirRow" runat="server">
          <td>文件夹名称：</td>
          <td>
            <asp:TextBox Columns="25" MaxLength="50" id="PublishmentSystemDir" style="ime-mode:disabled;" runat="server"/>
            <asp:RequiredFieldValidator
              ControlToValidate="PublishmentSystemDir"
              errorMessage=" *" foreColor="red"
              Display="Dynamic"
              runat="server"/>
            <asp:RegularExpressionValidator runat="server" ControlToValidate="PublishmentSystemDir"
              ValidationExpression="[\\.a-zA-Z0-9_-]+" foreColor="red" ErrorMessage=" 只允许包含字母、数字、下划线、中划线及小数点" Display="Dynamic" />
            <br>
            <span>实际在服务器中保存此网站的文件夹名称，此路径必须以英文或拼音命名。</span>
          </td>
        </tr>
        <tr id="ParentPublishmentSystemIDRow" runat="server">
          <td>上级站点：</td>
          <td>
            <asp:DropDownList ID="ParentPublishmentSystemID" runat="server"></asp:DropDownList>
          </td>
        </tr>
        <tr>
          <td>内容辅助表：</td>
          <td>
            <asp:DropDownList ID="AuxiliaryTableForContent" runat="server" > </asp:DropDownList>
            <asp:RequiredFieldValidator
              ControlToValidate="AuxiliaryTableForContent"
              ErrorMessage="辅助表不能为空！"
              foreColor="red"
              Display="Dynamic"
              runat="server"/>
          </td>
        </tr>
        <asp:PlaceHolder id="phWCMTables" visible="false" runat="server">
          <tr>
            <td>信息公开辅助表：</td>
            <td>
              <asp:DropDownList ID="AuxiliaryTableForGovPublic" runat="server" ></asp:DropDownList>
              <asp:RequiredFieldValidator
                ControlToValidate="AuxiliaryTableForGovPublic"
                ErrorMessage="辅助表不能为空！"
                foreColor="red"
                Display="Dynamic"
                runat="server"/>
            </td>
          </tr>
         <tr>
            <td>互动交流辅助表：</td>
            <td>
              <asp:DropDownList ID="AuxiliaryTableForGovInteract" runat="server" > </asp:DropDownList>
              <asp:RequiredFieldValidator
                ControlToValidate="AuxiliaryTableForGovInteract"
                ErrorMessage="辅助表不能为空！"
                foreColor="red"
                Display="Dynamic"
                runat="server"/>
            </td>
          </tr>
        </asp:PlaceHolder>
        <tr>
          <td>投票辅助表：</td>
          <td>
            <asp:DropDownList ID="AuxiliaryTableForVote" runat="server" ></asp:DropDownList>
            <asp:RequiredFieldValidator
              ControlToValidate="AuxiliaryTableForVote"
              ErrorMessage="辅助表不能为空！"
              foreColor="red"
              Display="Dynamic"
              runat="server"/>
          </td>
        </tr>
        <tr>
          <td width="160">招聘辅助表：</td>
          <td>
            <asp:DropDownList ID="AuxiliaryTableForJob" runat="server" ></asp:DropDownList>
            <asp:RequiredFieldValidator
              ControlToValidate="AuxiliaryTableForJob"
              ErrorMessage="辅助表不能为空！"
              foreColor="red"
              Display="Dynamic"
              runat="server"/>
          </td>
        </tr>
        <tr>
          <td>站点排序：</td>
          <td>
            <asp:TextBox ID="Taxis" class="input-mini" MaxLength="50" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ControlToValidate="Taxis" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
            <asp:RegularExpressionValidator
              ControlToValidate="Taxis"
              ValidationExpression="\d+"
              Display="Dynamic"
              ErrorMessage="排序必须为数字"
              foreColor="red"
              runat="server"/>
            <br>
            <span>设置站点排序，排序数字大的站点将排在其他站点之前</span>
          </td>
        </tr>
        <tr>
          <td>内容审核机制：</td>
          <td>
            <asp:RadioButtonList id="IsCheckContentUseLevel" AutoPostBack="true" OnSelectedIndexChanged="IsCheckContentUseLevel_OnSelectedIndexChanged" RepeatDirection="Horizontal" class="noborder" runat="server"></asp:RadioButtonList>
          </td>
        </tr>
        <tr id="CheckContentLevelRow" runat="server">
          <td>内容审核级别：</td>
          <td>
            <asp:DropDownList id="CheckContentLevel" runat="server">
              <asp:ListItem Value="2" Text="二级" Selected="true"></asp:ListItem>
              <asp:ListItem Value="3" Text="三级"></asp:ListItem>
              <asp:ListItem Value="4" Text="四级"></asp:ListItem>
              <asp:ListItem Value="5" Text="五级"></asp:ListItem>
            </asp:DropDownList>
          </td>
        </tr>
      </table>

      <hr />
      <table class="table noborder">
        <tr>
          <td class="center">
            <asp:Button class="btn btn-primary" id="Submit" text="修 改" OnClick="Submit_OnClick" runat="server"/>
            <input type="button" class="btn" value="返 回" onClick="javascript:location.href='pagePublishmentSystem.aspx';" />
          </td>
        </tr>
      </table>

    </div>
  </div>

</form>
</body>
</html>
