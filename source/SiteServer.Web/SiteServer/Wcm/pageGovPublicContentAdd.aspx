<%@ Page Language="C#" validateRequest="false" Inherits="SiteServer.BackgroundPages.Wcm.PageGovPublicContentAdd" %>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>
<head>
<meta charset="utf-8">
<!--#include file="../inc/header.aspx"-->
</head>

<body>
<!--#include file="../inc/openWindow.html"-->
<form id="myForm" class="form-inline" enctype="multipart/form-data" runat="server">
  <asp:Literal id="ltlBreadCrumb" runat="server" />
  <bairong:alerts runat="server" />

  <script type="text/javascript" charset="utf-8" src="../assets/validate.js"></script>
  <script language="javascript" type="text/javascript">
    function translateNodeAdd(name, value){
      $('#translateContainer').append("<div id='translate_" + value + "' class='addr_base addr_normal'><b>" + name + "</b> <a class='addr_del' href='javascript:;' onClick=\"translateNodeRemove('" + value + "')\"></a></div>");
      $('#translateCollection').val(value + ',' + $('#translateCollection').val());
      $('#translateType').show();
    }
    function translateNodeRemove(value){
      $('#translate_' + value).remove();
      var val = '';
      var values = $('#translateCollection').val().split(",");
      for (i=0;i<values.length ;i++ )
      {
        if (values[i] && value != values[i]){val = values[i] + ',';}
      }
      $('#translateCollection').val(val);
      if (val == ''){
        $('#translateType').hide();
      }
    }
    $(document).keypress(function(e){
      if(e.ctrlKey && e.which == 13 || e.which == 10) {
        e.preventDefault();
        $("#Submit").click();
      } else if (e.shiftKey && e.which==13 || e.which == 10) {
        e.preventDefault();
        $("#Submit").click();
      } else if (e.which==13) {
        e.preventDefault();
      }
    })
  </script>

  <div class="popover popover-static">
    <h3 class="popover-title"><asp:Literal id="ltlPageTitle" runat="server" /></h3>
    <div class="popover-content">

      <table class="table table-fixed noborder" style="position:relative;top:-30px;">
        <tr><td width="100">&nbsp;</td><td></td><td width="100"></td><td></td></tr>
        <bairong:AuxiliarySingleControl ID="ascTitle" runat="server"/>
        <tr>
          <td height="30">索引号：</td>
          <td height="30"><asp:TextBox ID="tbIdentifier" style="background-color:#F8F8F8" disabled="disabled" size="40" runat="server"></asp:TextBox>
            （自动生成）</td>
          <td height="30">发布机构：</td>
          <td height="30"><asp:TextBox ID="tbPublisher" size="40" isValidate="true" isRequire="true" minNum="0" maxNum="0" validateType="None" regExp="" errorMessage="" runat="server"></asp:TextBox>
            &nbsp;<span id="tbPublisher_msg" style="color:red;display:none"></span><script>event_observe('tbPublisher', 'blur', checkAttributeValue);</script></td>
        </tr>
        <tr>
          <td height="30">文号：</td>
          <td height="30"><asp:TextBox ID="tbDocumentNo" size="40" runat="server"></asp:TextBox></td>
          <td height="30">发文日期：</td>
          <td height="30"><bairong:DateTimeTextBox id="dtbPublishDate" now="true" Columns="20" runat="server" /></td>
        </tr>
        <tr>
          <td height="30">关键词：</td>
          <td height="30"><asp:TextBox ID="tbKeywords" size="40" runat="server"></asp:TextBox></td>
          <td height="30">生效日期：</td>
          <td height="30"><bairong:DateTimeTextBox id="dtbEffectDate" now="true" Columns="20" runat="server" /></td>
        </tr>
        <tr>
          <td height="30">是否废止：</td>
          <td height="30"><asp:RadioButtonList ID="rblIsAbolition" size="40" runat="server" RepeatDirection="Horizontal"></asp:RadioButtonList></td>
          <td height="30">废止日期：</td>
          <td height="30"><bairong:DateTimeTextBox ID="dtbAbolitionDate" Columns="20" runat="server" /></td>
        </tr>
        <tr>
          <td height="30">主题分类：</td>
          <td height="30"><div class="fill_box" id="categoryChannelContainer" style="display:none">
              <div class="addr_base addr_normal"> <b id="categoryChannelName"></b> <a class="addr_del" href="javascript:;" onClick="showCategoryChannel('', '0')"></a>
                <input id="categoryChannelID" name="categoryChannelID" value="0" type="hidden">
              </div>
            </div>
            <div ID="divAddChannel" class="btn_pencil" runat="server"><span class="pencil"></span>　修改</div>
            <script language="javascript">
              function showCategoryChannel(categoryName, categoryID){
                $('#categoryChannelName').html(categoryName);
                $('#categoryChannelID').val(categoryID);
                if (categoryID == '0'){
                $('#categoryChannelContainer').hide();
                }else{
                  $('#categoryChannelContainer').show();
                }
              }
              </script>
            </td>
          <td height="30">机构分类：</td>
          <td height="30"><div class="fill_box" id="categoryDepartmentContainer" style="display:none">
              <div class="addr_base addr_normal"> <b id="categoryDepartmentName"></b> <a class="addr_del" href="javascript:;" onClick="showCategoryDepartment('', '0')"></a>
                <input id="categoryDepartmentID" name="categoryDepartmentID" value="0" type="hidden">
              </div>
            </div>
            <div ID="divAddDepartment" class="btn_pencil" runat="server"><span class="pencil"></span>　修改</div>
            <script language="javascript">
            function showCategoryDepartment(departmentName, departmentID){
              $('#categoryDepartmentName').html(departmentName);
              $('#categoryDepartmentID').val(departmentID);
              if (departmentID == '0'){
              $('#categoryDepartmentContainer').hide();
              }else{
                $('#categoryDepartmentContainer').show();
                <%if (IsPublisherRelatedDepartmentId && string.IsNullOrEmpty(this.tbPublisher.Text)){%>
                $('#tbPublisher').val(departmentName);
                <%}%>
              }
            }
            </script>
          </td>
        </tr>
        <asp:Literal ID="ltlCategoryScript" runat="server"></asp:Literal>
        <tr>
          <td height="30">内容概述：</td>
          <td height="30" colspan="3"><asp:TextBox ID="tbDescription" TextMode="MultiLine" style="width:90%; height:70px;" runat="server"></asp:TextBox></td>
        </tr>
        <bairong:AuxiliaryControl ID="acAttributes" runat="server"/>
        <asp:PlaceHolder ID="phContentAttributes" runat="server">
          <tr>
            <td width="100">内容属性：</td>
            <td colspan="3"><asp:CheckBoxList CssClass="checkboxlist" ID="ContentAttributes" RepeatDirection="Horizontal" class="noborder" RepeatColumns="5" runat="server"/></td>
          </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="phContentGroup" runat="server">
          <tr>
            <td>所属内容组：</td>
            <td colspan="3"><asp:CheckBoxList CssClass="checkboxlist" ID="ContentGroupNameCollection" RepeatDirection="Horizontal" class="noborder" RepeatColumns="5" runat="server"/></td>
          </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="phTags" runat="server">
          <tr>
            <td>内容标签：</td>
            <td colspan="3"><asp:TextBox id="Tags" MaxLength="50" Width="380" runat="server"/>
              &nbsp;<span>请用空格或英文逗号分隔</span>
              <asp:Literal ID="ltlTags" runat="server"></asp:Literal></td>
          </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="phStatus" runat="server">
          <tr>
            <td>状态：</td>
            <td colspan="3"><asp:RadioButtonList CssClass="radiobuttonlist" ID="ContentLevel" RepeatDirection="Horizontal" class="noborder" runat="server"/></td>
          </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="phTranslate" runat="server">
          <tr>
            <td>转移到：</td>
            <td colspan="3">
              <div class="fill_box" id="translateContainer" style="display:"></div>
              <input id="translateCollection" name="translateCollection" value="" type="hidden">
              <div ID="divTranslateAdd" class="btn_pencil" runat="server"><span class="pencil"></span>　选择</div>
              <span id="translateType" style="padding-left:5px;display:none">
                <asp:DropDownList ID="ddlTranslateType" class="input-small" runat="server"></asp:DropDownList>
              <span>
            </td>
          </tr>
        </asp:PlaceHolder>
      </table>

      <hr />
      <table class="table noborder">
        <tr>
          <td class="center">
            <asp:Button class="btn btn-primary" id="Submit" text="确 定" onclick="Submit_OnClick" runat="server"/>
            <input class="btn btn-info" type="button" onClick="submitPreview();" value="预 览" />
            <input class="btn" type="button" onClick="location.href='<%=ReturnUrl%>';return false;" value="返 回" />
            <br><span class="gray">提示：按CTRL+回车可以快速提交</span>
          </td>
        </tr>
      </table>

    </div>
  </div>
</form>
</body>
</html>
