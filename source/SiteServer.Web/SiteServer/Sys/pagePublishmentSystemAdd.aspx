<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Sys.PagePublishmentSystemAdd" %>
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

  <script language="javascript">
  $(document).ready(function()
  {
    if (!isNull(_get('choose')))
    {
      var item = $(":radio:checked");
      if(item.length==0){
        if (!isNull(_get('choose').length) && _get('choose').length > 1)
        {
          _get('choose')[0].checked = true;
          document.getElementById('SiteTemplateDir').value=_get('choose')[0].value;
        }
        else
        {
          _get('choose').checked = true;
          document.getElementById('SiteTemplateDir').value=_get('choose').value;
        }
      }
    }
  });

  function displaySiteTemplateDiv(obj)
  {
    if (obj.checked){
      document.getElementById('SiteTemplateDiv').style.display = '';
    }else{
      document.getElementById('SiteTemplateDiv').style.display = 'none';
    }
  }
  if (document.getElementById('choose ')!= null){
    if (document.getElementById('choose').length > 1){
      document.getElementById('choose')[0].checked = 'true';
      document.getElementById('SiteTemplateDir').value=document.getElementById('choose')[0].value;
    }else{
      document.getElementById('choose').checked = 'true';
      document.getElementById('SiteTemplateDir').value=document.getElementById('choose').value;
    }
  }
  </script>

  <div class="popover popover-static">
    <h3 class="popover-title"><asp:Literal id="ltlPageTitle" runat="server" /></h3>
    <div class="popover-content">

      <asp:PlaceHolder id="ChooseSiteTemplate" runat="server">

      <blockquote>
        <p>选择站点模板</p>
        <small>欢迎使用新建站点向导，您可以选择使用站点模板作为新建站点的基础。</small>
      </blockquote>

      <div class="well well-small">
        <table class="table table-noborder">
          <tr>
            <td>
              是否使用站点模板：
              <asp:CheckBox id="UseSiteTemplate" runat="server" Checked="true" Text="使用"> </asp:CheckBox>
            </td>
          </tr>
        </table>
      </div>

      <div id="SiteTemplateDiv">
        <input type="hidden" id="SiteTemplateDir" value="" runat="server" />
        <asp:dataList id="dlContents" DataKeyField="Name" CssClass="table table-bordered table-hover" repeatDirection="Horizontal" repeatColumns="4" runat="server">
          <ItemTemplate>
            <asp:Literal ID="ltlImageUrl" runat="server"></asp:Literal>
            <asp:Literal ID="ltlDescription" runat="server"></asp:Literal>
            <asp:Literal ID="ltlRadio" runat="server"></asp:Literal>
          </ItemTemplate>
          <ItemStyle cssClass="center" />
        </asp:dataList>
      </div>

      </asp:PlaceHolder>

      <asp:PlaceHolder id="CreateSiteParameters" runat="server" Visible="false">

      <blockquote>
        <p>设置站点参数</p>
        <small>在此设置新建站点的名称、文件夹以及辅助表等信息。</small>
      </blockquote>

      <table class="table table-hover table-noborder">
          <tr id="RowSiteTemplateName" runat="server">
            <td>使用的站点模板名称：</td>
            <td>
              <asp:Label ID="SiteTemplateName" runat="server"></asp:Label>
            </td>
          </tr>
          <tr>
            <td width="160">站点名称：</td>
            <td>
              <asp:TextBox Columns="35" MaxLength="50" id="PublishmentSystemName" runat="server"/>
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
          <tr>
            <td width="160">站点类型：</td>
            <td>
              <asp:Literal ID="ltlPublishmentSystemType" runat="server"/>
            </td>
          </tr>
          <tr>
            <td>站点级别：</td>
            <td>
              <asp:RadioButtonList ID="IsHeadquarters" AutoPostBack="true" OnSelectedIndexChanged="IsHeadquarters_SelectedIndexChanged" RepeatDirection="Horizontal" class="noborder" runat="server" >
                <asp:ListItem Text="主站" Value="True"></asp:ListItem>
                <asp:ListItem Text="子站" Value="False" Selected="true"></asp:ListItem>
              </asp:RadioButtonList>
            </td>
          </tr>
          <asp:PlaceHolder ID="phNotIsHeadquarters" runat="server">
            <tr>
              <td>上级站点：</td>
              <td>
                <asp:DropDownList ID="ParentPublishmentSystemID" runat="server"></asp:DropDownList>
              </td>
            </tr>
            <tr>
              <td>文件夹名称：</td>
              <td>
                <asp:TextBox Columns="25" MaxLength="50" id="PublishmentSystemDir" runat="server"/>
                <asp:RequiredFieldValidator
                  ControlToValidate="PublishmentSystemDir"
                  errorMessage=" *" foreColor="red"
                  Display="Dynamic"
                  runat="server"/>
                <asp:RegularExpressionValidator runat="server" ControlToValidate="PublishmentSystemDir" ValidationExpression="[\\.a-zA-Z0-9_-]+" foreColor="red" ErrorMessage=" 只允许包含字母、数字、下划线、中划线及小数点" Display="Dynamic" />
                <br>
                <span>实际在服务器中保存此网站的文件夹名称，此路径必须以英文或拼音命名。</span>
              </td>
            </tr>
          </asp:PlaceHolder>
          <asp:PlaceHolder ID="phNodeRelated" runat="server">
          <tr>
            <td>网页编码：</td>
            <td>
              <asp:DropDownList id="Charset" runat="server"></asp:DropDownList>
            </td>
          </tr>
          <tr id="RowIsImportContents" runat="server">
            <td>是否导入栏目及内容：</td>
            <td>
              <asp:CheckBox id="IsImportContents" runat="server" Checked="true" Text="导入"></asp:CheckBox>
            </td>
          </tr>
          <tr id="RowIsImportTableStyles" runat="server">
            <td>是否导入表单样式：</td>
            <td>
              <asp:CheckBox id="IsImportTableStyles" runat="server" Checked="true" Text="导入"></asp:CheckBox>
            </td>
          </tr>
          <tr id="RowIsUserSiteTemplateAuxiliaryTables" runat="server">
            <td>站点表结构设置：</td>
            <td>
              <asp:RadioButtonList ID="IsUserSiteTemplateAuxiliaryTables" AutoPostBack="true" OnSelectedIndexChanged="IsUserSiteTemplateAuxiliaryTables_SelectedIndexChanged" RepeatDirection="Horizontal" class="noborder" runat="server">
                <asp:ListItem Text="使用站点模板中的辅助表" Value="True"></asp:ListItem>
                <asp:ListItem Text="使用指定的辅助表" Value="False" Selected="true"></asp:ListItem>
              </asp:RadioButtonList>
            </td>
          </tr>
          <asp:PlaceHolder ID="phAuxiliaryTable" runat="server" Visible="false">
            <tr>
              <td>内容辅助表：</td>
              <td>
                <asp:DropDownList ID="AuxiliaryTableForContent" runat="server" ></asp:DropDownList>
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
                  <asp:DropDownList ID="AuxiliaryTableForGovPublic" runat="server" > </asp:DropDownList>
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
                <asp:DropDownList ID="AuxiliaryTableForVote" runat="server" > </asp:DropDownList>
                <asp:RequiredFieldValidator
                  ControlToValidate="AuxiliaryTableForVote"
                  ErrorMessage="辅助表不能为空！"
                  foreColor="red"
                  Display="Dynamic"
                  runat="server"/>
              </td>
            </tr>
            <tr>
              <td>招聘辅助表：</td>
              <td>
                <asp:DropDownList ID="AuxiliaryTableForJob" runat="server" > </asp:DropDownList>
                <asp:RequiredFieldValidator
                  ControlToValidate="AuxiliaryTableForJob"
                  ErrorMessage="辅助表不能为空！"
                  foreColor="red"
                  Display="Dynamic"
                  runat="server"/>
              </td>
            </tr>
          </asp:PlaceHolder>
          <tr>
            <td>内容审核机制：</td>
            <td>
              <asp:RadioButtonList id="IsCheckContentUseLevel" AutoPostBack="true" OnSelectedIndexChanged="IsCheckContentUseLevel_OnSelectedIndexChanged" RepeatDirection="Horizontal" class="noborder" runat="server"></asp:RadioButtonList>
            </td>
          </tr>
          <tr id="CheckContentLevelRow" runat="server" visible="false">
            <td>内容审核级别：</td>
            <td>
              <asp:DropDownList id="CheckContentLevel" runat="server">
                <asp:ListItem Value="2" Text="二级" Selected="true"></asp:ListItem>
                <asp:ListItem Value="3" Text="三级"></asp:ListItem>
                <asp:ListItem Value="4" Text="四级"></asp:ListItem>
                <asp:ListItem Value="5" Text="五级"></asp:ListItem>
              </asp:DropDownList>
              <br>
              <span>指此内容在添加后需要经多少次审核才能正式发布</span>
            </td>
          </tr>
          </asp:PlaceHolder>
        </table>

      </asp:PlaceHolder>

      <asp:PlaceHolder id="OperatingError" runat="server" Visible="false">

      <blockquote style="margin-top:20px;">
        <p>发生错误</p>
        <small>执行向导过程中出错</small>
      </blockquote>

      <div class="alert alert-error">
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
