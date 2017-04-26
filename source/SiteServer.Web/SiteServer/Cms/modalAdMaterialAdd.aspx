<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalAdMaterialAdd" %>
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
  <asp:Button id="btnSubmit" useSubmitBehavior="false" OnClick="Submit_OnClick" runat="server" style="display:none" />
  <bairong:alerts runat="server"></bairong:alerts>
  
  <table class="table table-noborder table-hover">
        <tr>
          <td width="160">广告物料名称：</td>
          <td colspan="3">
            <asp:TextBox Columns="45" MaxLength="50" id="AdMaterialName" runat="server"/>
            <asp:RequiredFieldValidator
              ControlToValidate="AdMaterialName"
              errorMessage=" *" foreColor="red" 
              Display="Dynamic"
              runat="server"/>
          </td>
        </tr>
        <tr>
          <td>类型：</td>
          <td colspan="3">
            <asp:DropDownList ID="AdMaterialType" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ReFresh"></asp:DropDownList>
          </td>
        </tr>
        <tr>
          <td>是否生效：</td>
          <td colspan="3">
            <asp:RadioButtonList ID="IsEnabled" RepeatDirection="Horizontal" class="noborder" runat="server"></asp:RadioButtonList>
          </td>
        </tr>
        <asp:PlaceHolder ID="phCode" runat="server">
          <tr>
            <td>广告内容：</td>
            <td colspan="3">
              <asp:TextBox style="height:150px; width:70%" TextMode="MultiLine" id="Code" runat="server" Wrap="false" />
              <asp:RequiredFieldValidator
                ControlToValidate="Code"
                errorMessage=" *" foreColor="red" 
                Display="Dynamic"
                runat="server"/>
            </td>
          </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="phText" runat="server" Visible="false">
          <tr>
            <td>文字内容：</td>
            <td colspan="3">
              <asp:TextBox Columns="65" MaxLength="50" id="TextWord" runat="server"/>
              <asp:RequiredFieldValidator
                ControlToValidate="TextWord"
                errorMessage=" *" foreColor="red" 
                Display="Dynamic"
                runat="server"/>
            </td>
          </tr>
          <tr>
            <td>文字链接：</td>
            <td colspan="3">
              <asp:TextBox Columns="65" MaxLength="100" id="TextLink" runat="server"/>
              <asp:RequiredFieldValidator
                ControlToValidate="TextLink"
                errorMessage=" *" foreColor="red" 
                Display="Dynamic"
                runat="server"/>
            </td>
          </tr>
          <tr>
            <td>文字颜色：</td>
            <td><asp:TextBox Columns="25" MaxLength="50" id="TextColor" runat="server"/></td>
            <td width="160">字体大小：</td>
            <td>
              <asp:TextBox Columns="25" MaxLength="50" id="TextFontSize" runat="server"/>
              <asp:RegularExpressionValidator
                ControlToValidate="TextFontSize"
                ValidationExpression="\d+"
                Display="Dynamic"
                ErrorMessage="字体大小必须为数字"
                foreColor="red"
                runat="server"/></td>
          </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="phImage" runat="server" Visible="false">
          <tr>
            <td>图片地址：</td>
            <td colspan="3">
              <asp:TextBox Columns="65" MaxLength="100" id="ImageUrl" runat="server"/>
              <asp:RequiredFieldValidator
                ControlToValidate="ImageUrl"
                errorMessage=" *" foreColor="red" 
                Display="Dynamic"
                runat="server"/>
              &nbsp;
              <asp:Button ID="ImageUrlSelect" runat="server" class="btn" text="选择"></asp:Button>
              &nbsp;
              <asp:Button ID="ImageUrlUpload" runat="server" class="btn" text="上传"></asp:Button>
              </td>
          </tr>
          <tr>
            <td>广告链接：</td>
            <td colspan="3">
              <asp:TextBox Columns="65" MaxLength="100" id="ImageLink" runat="server"/>
              <asp:RequiredFieldValidator
                ControlToValidate="ImageLink"
                errorMessage=" *" foreColor="red" 
                Display="Dynamic"
                runat="server"/>
            </td>
          </tr>
          <tr>
            <td>宽度：</td>
            <td>
              <asp:TextBox class="input-mini" id="ImageWidth" runat="server"/>
              <asp:RegularExpressionValidator
                ControlToValidate="ImageWidth"
                ValidationExpression="\d+"
                Display="Dynamic"
                ErrorMessage="宽度必须为数字"
                foreColor="red"
                runat="server"/>
            </td>
            <td>高度：</td>
            <td>
              <asp:TextBox class="input-mini" id="ImageHeight" runat="server"/>
              <asp:RegularExpressionValidator
                ControlToValidate="ImageHeight"
                ValidationExpression="\d+"
                Display="Dynamic"
                ErrorMessage="高度必须为数字"
                foreColor="red"
                runat="server"/>
            </td>
          </tr>
          <tr>
            <td>图片替换文字：</td>
            <td colspan="3">
              <asp:TextBox Columns="65" MaxLength="50" id="ImageAlt" runat="server"/>
            </td>
          </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="phFlash" runat="server" Visible="false">
          <tr>
            <td>Flash地址：</td>
            <td colspan="3">
              <asp:TextBox Columns="65" MaxLength="100" id="FlashUrl" runat="server"/>
              <asp:RequiredFieldValidator
                ControlToValidate="FlashUrl"
                errorMessage=" *" foreColor="red" 
                Display="Dynamic"
                runat="server"/>
              &nbsp;
              <asp:Button ID="FlashUrlSelect" runat="server" class="btn" text="选择"></asp:Button>
              &nbsp;
              <asp:Button ID="FlashUrlUpload" runat="server" class="btn" text="上传"></asp:Button>
            </td>
          </tr>
          <tr>
            <td>宽度：</td>
            <td><asp:TextBox class="input-mini" id="FlashWidth" runat="server"/>
              <asp:RegularExpressionValidator
                ControlToValidate="FlashWidth"
                ValidationExpression="\d+"
                Display="Dynamic"
                ErrorMessage="宽度必须为数字"
                foreColor="red"
                runat="server"/>
            </td>
            <td>高度：</td>
            <td>
              <asp:TextBox class="input-mini" MaxLength="50" id="FlashHeight" runat="server"/>
              <asp:RegularExpressionValidator
                ControlToValidate="FlashHeight"
                ValidationExpression="\d+"
                Display="Dynamic"
                ErrorMessage="高度必须为数字"
                foreColor="red"
                runat="server"/>
            </td>
          </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="phWeight" runat="server" Visible="false">
         <tr>
            <td>权重：</td>
            <td colspan="3">
              <asp:DropDownList ID="Weight" runat="server"></asp:DropDownList>
            </td>
          </tr>
         </asp:PlaceHolder>
      </table>

</form>
</body>
</html>
