<%@ Page Language="C#" ValidateRequest="false" Inherits="SiteServer.BackgroundPages.Cms.PageContentAdd" %>
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
        <asp:Literal ID="ltlBreadCrumb" runat="server" />
        <bairong:Alerts runat="server" />

        <script type="text/javascript" charset="utf-8" src="../assets/validate.js"></script>
        <script type="text/javascript" charset="utf-8" src="../assets/jquery/jquery.form.js"></script>
        <script src="../assets/jscolor/jscolor.js"></script>
        <script type="text/javascript" charset="utf-8" src="js/contentAdd.js"></script>

        <div class="popover popover-static">
            <h3 class="popover-title"><asp:Literal ID="LtlPageTitle" runat="server" /></h3>
            <div class="popover-content">

                <table class="table table-fixed noborder" style="position: relative; top: -30px;">
                    <tr>
                        <td width="100">&nbsp;</td>
                        <td></td>
                        <td width="100"></td>
                        <td></td>
                    </tr>

                    <bairong:AuxiliaryControl ID="AcAttributes" runat="server" />

                    <asp:PlaceHolder ID="PhContentAttributes" runat="server">
                        <tr>
                            <td>内容属性：</td>
                            <td colspan="3">
                                <asp:CheckBoxList CssClass="checkboxlist" ID="CblContentAttributes" RepeatDirection="Horizontal" class="noborder" RepeatColumns="5" runat="server" /></td>
                        </tr>
                    </asp:PlaceHolder>

                    <asp:PlaceHolder ID="PhContentGroup" runat="server">
                        <tr>
                            <td>所属内容组：</td>
                            <td colspan="3">
                                <asp:CheckBoxList CssClass="checkboxlist" ID="CblContentGroupNameCollection" RepeatDirection="Horizontal" class="noborder" RepeatColumns="5" runat="server" /></td>
                        </tr>
                    </asp:PlaceHolder>

                    <asp:PlaceHolder ID="PhTags" runat="server">
                        <tr>
                            <td>内容标签：</td>
                            <td colspan="3">
                                <asp:TextBox ID="TbTags" MaxLength="50" Width="380" runat="server" />
                                &nbsp;<span>请用空格或英文逗号分隔</span>
                                <asp:Literal ID="LtlTags" runat="server"></asp:Literal></td>
                        </tr>
                    </asp:PlaceHolder>

                    <asp:PlaceHolder ID="PhStatus" runat="server">
                        <tr>
                            <td>状态：</td>
                            <td colspan="3">
                                <asp:RadioButtonList CssClass="radiobuttonlist" ID="RblContentLevel" RepeatDirection="Horizontal" class="noborder" runat="server" /></td>
                        </tr>
                    </asp:PlaceHolder>

                    <asp:PlaceHolder ID="PhTranslate" runat="server">
                        <tr>
                            <td>转移到：</td>
                            <td colspan="3">
                                <div id="DivTranslateAdd" class="btn_select" runat="server">选择栏目</div>
                                <div class="fill_box" id="translateContainer" style="display: "></div>
                                <input id="translateCollection" name="translateCollection" value="" type="hidden">
                                <span id="translateType" style="padding-left: 5px; display: none">
                                    <asp:DropDownList ID="DdlTranslateType" class="input-small" runat="server"></asp:DropDownList>
                                </span>
                            </td>
                        </tr>
                    </asp:PlaceHolder>

                </table>

                <hr />
                <table class="table noborder">
                    <tr>
                        <td class="center">
                            <asp:Button class="btn btn-primary" itemIndex="1" ID="BtnSubmit" Text="确 定" OnClick="Submit_OnClick" runat="server" />
                            <input class="btn btn-info" type="button" onClick="previewSave();" value="预 览" />
                            <%if (!string.IsNullOrEmpty(ReturnUrl))
                              {%>
                            <input class="btn" type="button" onclick="location.href='<%=ReturnUrl%>    ';return false;" value="返 回" />
                            <%}%>
                            <br>
                            <span class="gray">提示：按CTRL+回车可以快速提交</span>
                        </td>
                    </tr>
                </table>

            </div>
        </div>
    </form>
</body>
</html>
