<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageAdvertisementAdd" %>
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
    <h3 class="popover-title"><asp:Literal id="ltlPageTitle" runat="server" /></h3>
    <div class="popover-content">
    
      <asp:PlaceHolder id="AdvertisementBase" runat="server" Visible="false">
        <table class="table table-noborder table-hover">
          <tr>
            <td width="200">广告名称：</td>
            <td>
              <asp:TextBox Columns="25" MaxLength="50" id="AdvertisementName" runat="server" />
              <asp:RequiredFieldValidator ControlToValidate="AdvertisementName" ErrorMessage=" *" Display="Dynamic" foreColor="red" runat="server" />
              <asp:RegularExpressionValidator runat="server" ControlToValidate="AdvertisementName" ValidationExpression="[^',]+" errorMessage=" *" foreColor="red" display="Dynamic" />
            </td>
          </tr>
          <tr>
            <td>广告类型：</td>
            <td>
              <asp:RadioButtonList ID="AdvertisementType" runat="server" ></asp:RadioButtonList>
            </td>
          </tr>
          <tr>
            <td>此广告是否存在期限限制：</td>
            <td>
              <asp:CheckBox id="IsDateLimited" AutoPostBack="true" OnCheckedChanged="ReFresh" Text="存在期限限制" runat="server"> </asp:CheckBox>
            </td>
          </tr>
          <tr id="StartDateRow" runat="server">
            <td>开始时间：</td>
            <td>
              <bairong:DateTimeTextBox id="StartDate" showTime="true" Columns="30" runat="server" />
            </td>
          </tr>
          <tr id="EndDateRow" runat="server">
            <td>结束时间：</td>
            <td>
              <bairong:DateTimeTextBox id="EndDate" showTime="true" Columns="30" runat="server" />
            </td>
          </tr>
          <tr>
            <td>显示此广告的栏目页面：</td>
            <td>
              <asp:ListBox ID="NodeIDCollectionToChannel" SelectionMode="Multiple" Rows="15" runat="server"></asp:ListBox>
            </td>
          </tr>
          <tr>
            <td>显示此广告的内容页面：</td>
            <td>
              <asp:ListBox ID="NodeIDCollectionToContent" SelectionMode="Multiple" Rows="15" runat="server"></asp:ListBox>
            </td>
          </tr>
          <tr id="FileTemplateIDCollectionRow" runat="server">
            <td>显示此广告的单页模板：</td>
            <td>
              <asp:CheckBoxList ID="FileTemplateIDCollection" RepeatColumns="4" RepeatDirection="Horizontal" class="noborder" runat="server"></asp:CheckBoxList>
            </td>
          </tr>
        </table>
      </asp:PlaceHolder>
      <asp:PlaceHolder id="AdvertisementFloatImage" runat="server" Visible="false">
        <table class="table table-noborder table-hover">
          <tr>
            <td width="200">广告链接地址：</td>
            <td>
              <asp:TextBox Columns="50" MaxLength="200" id="NavigationUrl" runat="server" />
            </td>
          </tr>
          <tr>
            <td>显示图片地址：</td>
            <td>
              <asp:TextBox Columns="40" MaxLength="200" id="ImageUrl" runat="server" />
              <asp:RegularExpressionValidator runat="server" ControlToValidate="ImageUrl" ValidationExpression="[^']+" ErrorMessage=" *" foreColor="red" Display="Dynamic" />
              &nbsp;
              <asp:Button ID="SelectImage" class="btn" runat="server" text="选择"></asp:Button>
              <asp:Button ID="UploadImage" class="btn" runat="server" text="上传"></asp:Button>
            </td>
          </tr>
          <tr>
            <td>图片显示宽度：</td>
            <td>
              <asp:TextBox class="input-mini" id="Width" runat="server" />
              <asp:RegularExpressionValidator
                ControlToValidate="Width"
                ValidationExpression="\d+"
                Display="Dynamic"
                ErrorMessage="图片显示宽度必须为数字"
                foreColor="red"
                runat="server"/>
            </td>
          </tr>
          <tr>
            <td>图片显示高度：</td>
            <td>
              <asp:TextBox class="input-mini" id="Height" runat="server" />
              <asp:RegularExpressionValidator
                ControlToValidate="Height"
                ValidationExpression="\d+"
                Display="Dynamic"
                ErrorMessage="图片显示高度必须为数字"
                foreColor="red"
                runat="server"/>
            </td>
          </tr>
          <tr>
            <td>广告滚动类型：</td>
            <td>
              <asp:RadioButtonList ID="RollingType" AutoPostBack="true" OnSelectedIndexChanged="ReFresh" runat="server" ></asp:RadioButtonList>
              <span class="gray">此广告在页面中的显示方式，可以是“跟随窗体滚动”、“在窗体中不断移动”或“静止不动”。</span>
            </td>
          </tr>
          <tr>
            <td>广告位置：</td>
            <td>
              <asp:DropDownList ID="PositionType" runat="server"> </asp:DropDownList>
            </td>
          </tr>
          <tr>
            <td>水平方向距离：</td>
            <td>
              <asp:TextBox Columns="10" MaxLength="50" Text="120" id="PositionX" runat="server" />
              <asp:RegularExpressionValidator
                ControlToValidate="PositionX"
                ValidationExpression="\d+"
                Display="Dynamic"
                ErrorMessage="此项必须为数字"
                foreColor="red"
                runat="server"/>(px)
            </td>
          </tr>
          <tr>
            <td>垂直方向距离：</td>
            <td>
              <asp:TextBox Columns="10" MaxLength="50" Text="100" id="PositionY" runat="server" />
              <asp:RegularExpressionValidator
                ControlToValidate="PositionY"
                ValidationExpression="\d+"
                Display="Dynamic"
                ErrorMessage="此项必须为数字"
                foreColor="red"
                runat="server"/>(px)
            </td>
          </tr>
          <tr id="IsCloseableTR">
            <td>广告能够被关闭：</td>
            <td>
              <asp:CheckBox id="IsCloseable" runat="server"> </asp:CheckBox>
            </td>
          </tr>
        </table>
      </asp:PlaceHolder>
      <asp:PlaceHolder id="AdvertisementScreenDown" runat="server" Visible="false">
        <table class="table table-noborder table-hover">
          <tr>
            <td width="200">广告链接地址：</td>
            <td>
              <asp:TextBox Columns="50" MaxLength="200" id="ScreenDownNavigationUrl" runat="server" />
              <asp:RequiredFieldValidator ControlToValidate="ScreenDownNavigationUrl" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
              <asp:RegularExpressionValidator runat="server" ControlToValidate="ScreenDownNavigationUrl"
                ValidationExpression="[^']+" errorMessage=" *" foreColor="red" display="Dynamic" />
            </td>
          </tr>
          <tr>
            <td>显示图片地址：</td>
            <td>
              <asp:TextBox Columns="40" MaxLength="200" id="ScreenDownImageUrl" runat="server" />
              <asp:RegularExpressionValidator runat="server" ControlToValidate="ScreenDownImageUrl" ValidationExpression="[^']+" errorMessage=" *" foreColor="red" display="Dynamic" />
              <asp:Button ID="ScreenDownSelectImage" runat="server" class="btn" text="选择"></asp:Button>
              <asp:Button ID="ScreenDownUploadImage" runat="server" class="btn" text="上传"></asp:Button>
            </td>
          </tr>
          <tr>
            <td>显示广告时间：</td>
            <td>
              <asp:TextBox Columns="10" MaxLength="50" id="ScreenDownDelay" runat="server" Text="5" />
              <asp:RegularExpressionValidator
                ControlToValidate="ScreenDownDelay"
                ValidationExpression="\d+"
                Display="Dynamic"
                ErrorMessage="显示广告时间必须为数字"
                foreColor="red"
                runat="server"/> 秒
            </td>
          </tr>
          <tr>
            <td>图片显示宽度：</td>
            <td>
              <asp:TextBox Columns="10" MaxLength="50" id="ScreenDownWidth" runat="server" Text="0" />
              <asp:RegularExpressionValidator
                ControlToValidate="ScreenDownWidth"
                ValidationExpression="\d+"
                Display="Dynamic"
                ErrorMessage="图片显示宽度必须为数字"
                foreColor="red"
                runat="server"/><span class="gray">(0代表图片默认宽度)</span>
            </td>
          </tr>
          <tr>
            <td>图片显示高度：</td>
            <td>
              <asp:TextBox Columns="10" MaxLength="50" id="ScreenDownHeight" runat="server" Text="0" />
              <asp:RegularExpressionValidator
                ControlToValidate="ScreenDownHeight"
                ValidationExpression="\d+"
                Display="Dynamic"
                ErrorMessage="图片显示高度必须为数字"
                foreColor="red"
                runat="server"/><span class="gray">(0代表图片默认高度)</span>
            </td>
          </tr>
        </table>
      </asp:PlaceHolder>
      <asp:PlaceHolder id="AdvertisementOpenWindow" runat="server" Visible="false">
        <table class="table table-noborder table-hover">
          <tr>
            <td width="200">弹出窗口页面地址：</td>
            <td>
              <asp:TextBox Columns="50" MaxLength="200" id="OpenWindowFileUrl" runat="server" />
              <asp:RequiredFieldValidator ControlToValidate="OpenWindowFileUrl" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
              <asp:RegularExpressionValidator runat="server" ControlToValidate="OpenWindowFileUrl"
                ValidationExpression="[^']+" errorMessage=" *" foreColor="red" display="Dynamic" />
            </td>
          </tr>
          <tr>
            <td>弹出窗口宽度：</td>
            <td>
              <asp:TextBox Columns="10" MaxLength="50" Text="0" id="OpenWindowWidth" runat="server" />
              <asp:RegularExpressionValidator
                ControlToValidate="OpenWindowWidth"
                ValidationExpression="\d+"
                Display="Dynamic"
                ErrorMessage="弹出窗口宽度必须为数字"
                foreColor="red"
                runat="server"/><span class="gray">(0代表不限制宽度)</span>
            </td>
          </tr>
          <tr>
            <td>弹出窗口高度：</td>
            <td>
              <asp:TextBox Columns="10" MaxLength="50" Text="0" id="OpenWindowHeight" runat="server" />
              <asp:RegularExpressionValidator
                ControlToValidate="OpenWindowHeight"
                ValidationExpression="\d+"
                Display="Dynamic"
                ErrorMessage="弹出窗口高度必须为数字"
                foreColor="red"
                runat="server"/><span class="gray">(0代表不限制高度)</span>
            </td>
          </tr>
        </table>
      </asp:PlaceHolder>
      <asp:PlaceHolder id="Done" runat="server" Visible="false">

        <blockquote>
          <p>完成!</p>
          <small>操作成功。</small>
        </blockquote>

      </asp:PlaceHolder>
      <asp:PlaceHolder id="OperatingError" runat="server" Visible="false">

        <blockquote>
          <p>发生错误</p>
          <small>执行向导过程中出错</small>
        </blockquote>

        <div class="alert alert-error">
          <h4><asp:Label ID="ErrorLabel" runat="server"></asp:Label></h4>
        </div>

      </asp:PlaceHolder>
  
      <hr />
      <table class="table noborder">
        <tr>
          <td class="center">
            <asp:Button class="btn" ID="Previous" OnClick="PreviousPanel" CausesValidation="false" runat="server" Text="< 上一步"></asp:Button>
            <asp:Button class="btn btn-primary" id="Next" onclick="NextPanel" runat="server" text="下一步 >"></asp:button>
            <span style="padding-right:30 ">
          </td>
        </tr>
      </table>
  
    </div>
  </div>

</form>
</body>
</html>
