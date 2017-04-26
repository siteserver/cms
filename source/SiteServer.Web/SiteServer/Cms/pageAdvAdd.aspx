<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageAdvAdd" %>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>
<head>
<meta charset="utf-8">
<!--#include file="../inc/header.aspx"-->
  <script type="text/javascript" language="javascript">
      function selectAllToChannel(isChecked)
      {
          for(var i=0; i<document.getElementById('<%=NodeIDCollectionToChannel.ClientID%>').options.length; i++)
          {
              document.getElementById('<%=NodeIDCollectionToChannel.ClientID%>').options[i].selected = isChecked;
          }
      }
      function selectAllToContent(isChecked)
      {
          for(var i=0; i<document.getElementById('<%=NodeIDCollectionToContent.ClientID%>').options.length; i++)
          {
              document.getElementById('<%=NodeIDCollectionToContent.ClientID%>').options[i].selected = isChecked;
          }
      }
  </script>
</head>

<body>
<!--#include file="../inc/openWindow.html"-->
<form class="form-inline" runat="server">
  <asp:Literal id="ltlBreadCrumb" runat="server" />
  <bairong:alerts ID="Alerts1" runat="server" />
    
  <div class="popover popover-static">
    <h3 class="popover-title"><asp:Literal id="ltlPageTitle" runat="server" /></h3>
    <div class="popover-content">
      <table class="table noborder table-hover">
        <tr>
          <td width="160">广告名称：</td>
          <td colspan="3">
            <asp:TextBox Columns="45" MaxLength="50" id="AdvName" runat="server"/>
            <asp:RequiredFieldValidator
              ControlToValidate="AdvName"
              errorMessage=" *" foreColor="red" 
              Display="Dynamic"
              runat="server"/>
          </td>
        </tr>
        <tr>
          <td>是否生效：</td>
          <td colspan="3">
            <asp:RadioButtonList ID="IsEnabled" RepeatDirection="Horizontal" class="noborder" runat="server"></asp:RadioButtonList>
          </td>
        </tr>
       <tr>
          <td>是否存在时间限制：</td>
          <td colspan="3">
            <asp:CheckBox id="IsDateLimited" AutoPostBack="true" OnCheckedChanged="ReFresh" Text="存在时间限制" runat="server"></asp:CheckBox>
          </td>
        </tr>
        <tr id="StartDateRow" runat="server">
          <td>开始时间：</td>
          <td colspan="3">
            <bairong:DateTimeTextBox id="StartDate" Columns="30" runat="server" />
          </td>
        </tr>
        <tr id="EndDateRow" runat="server">
          <td>结束时间：</td>
          <td colspan="3">
            <bairong:DateTimeTextBox id="EndDate" Columns="30" runat="server" />
          </td>
        </tr>
         <tr>
          <td>广告显示优先级：</td>
          <td colspan="3">
            <asp:DropDownList ID="LevelType" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ReFresh"></asp:DropDownList>&nbsp;
            <asp:DropDownList ID="Level" runat="server" Width="60" ></asp:DropDownList>&nbsp;
            <asp:CheckBox ID="IsWeight" Text="设置权重" runat="server" AutoPostBack="true" OnCheckedChanged="ReFresh" />
            <asp:DropDownList ID="Weight" runat="server" Width="60" ></asp:DropDownList> 
          </td>
        </tr>
         <tr>
          <td>广告物料轮换：</td>
          <td colspan="3">
             <asp:DropDownList ID="RotateType" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ReFresh"></asp:DropDownList>  
           </td>
        </tr>
           <tr id="RotateIntervalRow" runat="server">
          <td>轮换时间间隔：</td>
          <td colspan="3">
            <asp:TextBox ID="RotateInterval" runat="server">5</asp:TextBox>&nbsp;<span>秒</span>
               <asp:RegularExpressionValidator ID="RegularExpressionValidator1"
                ControlToValidate="RotateInterval"
                ValidationExpression="\d+"
                Display="Dynamic"
                ErrorMessage="必须为数字"
                foreColor="red"
                runat="server"/>
           </td>
        </tr>
          <tr>
            <td>显示此广告的栏目页面：</td>
            <td colspan="3">
              <asp:ListBox ID="NodeIDCollectionToChannel" SelectionMode="Multiple" Rows="15" runat="server"></asp:ListBox>
              &nbsp;&nbsp;
              <label class="checkbox inline" style="vertical-align:bottom">
              <input type="checkbox" onClick="selectAllToChannel(this.checked);"> 全选
            </label>
            </td>
          </tr>
          <tr>
            <td>显示此广告的内容页面：</td>
            <td colspan="3">
              <asp:ListBox ID="NodeIDCollectionToContent" SelectionMode="Multiple" Rows="15" runat="server"></asp:ListBox>
              &nbsp;&nbsp;
              <label class="checkbox inline" style="vertical-align:bottom">
              <input type="checkbox" onClick="selectAllToContent(this.checked);"> 全选
            </label>
            </td>
          </tr>
            <tr id="FileTemplateIDCollectionRow" runat="server">
            <td>显示此广告的单页模板：</td>
            <td>
              <asp:CheckBoxList ID="FileTemplateIDCollection" RepeatColumns="4" RepeatDirection="Horizontal" class="noborder" runat="server"></asp:CheckBoxList>
            </td>
          </tr>
          <tr>
            <td>描述：</td>
            <td colspan="3">
              <asp:TextBox style="height:100px; width:70%" TextMode="MultiLine" id="Summary" runat="server" Wrap="false" />
             </td>
          </tr>
       </table>
      <hr />
      <table class="table noborder">
        <tr>
          <td class="center">
            <asp:Button class="btn btn-primary" id="Submit" text="确 定" onclick="Submit_OnClick" runat="server" />
            <input class="btn" type="button" onClick="location.href='pageAdv.aspx?PublishmentSystemID=<%=PublishmentSystemId%>';return false;" value="返 回" />
          </td>
        </tr>
      </table>
  
    </div>
  </div>

</form>
</body>
</html>
