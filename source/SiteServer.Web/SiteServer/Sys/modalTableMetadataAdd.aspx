<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Sys.ModalTableMetadataAdd" %>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>
<head>
<meta charset="utf-8">
<!--#include file="../inc/header.aspx"-->
</head>

<body <%if (Request.QueryString["TableMetadataID"] != null){%>onload="InitialTextData();"<%}else{%>onload="updateTextData();"<%}%>>
<!--#include file="../inc/openWindow.html"-->
<form class="form-inline" runat="server" id="MyForm">
<asp:Button id="btnSubmit" useSubmitBehavior="false" OnClick="Submit_OnClick" runat="server" style="display:none" />
<bairong:alerts runat="server"></bairong:alerts>

  <script language="javascript">
  var vArr = new Array();
  //datatype,length,length_editable
  vArr[0] = new Array('DateTime',8,false);
  vArr[1] = new Array('Integer',4,false);
  vArr[2] = new Array('NChar',50,true);
  vArr[3] = new Array('NText',16,false);
  vArr[4] = new Array('NVarChar',255,true);

  function updateTextData() {
      var myForm = document.forms.MyForm;
      var valueStr = document.forms.MyForm.DataType.options[document.forms.MyForm.DataType.selectedIndex].value;
      for (var i = 0; i < vArr.length; i++) {
          if (valueStr == vArr[i][0]) {
              myForm.DataLength.value = vArr[i][1];
              myForm.DataLength.disabled = (vArr[i][2])?null:"disabled";
              myForm.DataLengthHidden.value = vArr[i][1];
          }
      }
  }

  function InitialTextData() {
      var myForm = document.forms.MyForm;
      var valueStr = document.forms.MyForm.DataType.options[document.forms.MyForm.DataType.selectedIndex].value;
      for (var i = 0; i < vArr.length; i++) {
          if (valueStr == vArr[i][0]) {
              myForm.DataLength.disabled = (vArr[i][2])?null:"disabled";
              myForm.DataLengthHidden.value = vArr[i][1];
          }
      }
  }
  </script>
    
  <table class="table table-noborder table-hover">
    <tr>
      <td><bairong:help HelpText="需要添加的字段名称" Text="字段名：" runat="server" ></bairong:help></td>
      <td><asp:TextBox Columns="25" MaxLength="50" id="AttributeName" runat="server" />
        <asp:RequiredFieldValidator id="RequiredFieldValidator" ControlToValidate="AttributeName" errorMessage=" *" foreColor="red" display="Dynamic"
                    runat="server" />
        <asp:RegularExpressionValidator runat="server" ControlToValidate="AttributeName"
                ValidationExpression="[a-zA-Z0-9_]+" ErrorMessage="<br/>只允许包含字母、数字以及下划线" foreColor="red" Display="Dynamic" /></td>
    </tr>
    <tr>
      <td><bairong:help HelpText="此字段的数据类型" Text="数据类型：" runat="server" ></bairong:help></td>
      <td><asp:DropDownList ID="DataType" runat="server" onChange="updateTextData();"> </asp:DropDownList></td>
    </tr>
    <tr>
      <td><bairong:help HelpText="此字段的数据长度" Text="数据长度：" runat="server" ></bairong:help></td>
      <td>
        <asp:TextBox Columns="25" MaxLength="50" id="DataLength" runat="server" />
        <asp:RequiredFieldValidator ControlToValidate="DataLength" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
        <asp:RegularExpressionValidator
          ControlToValidate="DataLength"
          ValidationExpression="\d+"
          Display="Dynamic"
          ErrorMessage="数据长度必须为数字"
          foreColor="red"
          runat="server"/>
        <input type="hidden" value="0" id="DataLengthHidden">
      </td>
    </tr>
  </table>

</form>
</body>
</html>
