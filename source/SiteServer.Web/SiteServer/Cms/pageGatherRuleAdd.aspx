<%@ Page Language="C#" ValidateRequest="false" Inherits="SiteServer.BackgroundPages.Cms.PageGatherRuleAdd" %>
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
        <asp:Literal ID="ltlBreadCrumb" runat="server" />
        <bairong:Alerts runat="server" />

        <script language="JavaScript" type="text/JavaScript">
            function AddOnPos(obj, charvalue) {
                //obj代表要插入字符的输入框
                //value代表要插入的字符

                obj.focus();
                var r = document.selection.createRange();
                var ctr = obj.createTextRange();
                var i;
                var s = obj.value;

                //注释掉的这种方法只能用在单行的输入框input内
                //对多行输入框textarea无效
                //r.setEndPoint("StartToStart", ctr);
                //i = r.text.length;
                //取到光标位置----Start----
                var ivalue = "&^asdjfls2FFFF325%$^&";
                r.text = ivalue;
                i = obj.value.indexOf(ivalue);
                r.moveStart("character", -ivalue.length);
                r.text = "";
                //取到光标位置----End----
                //插入字符
                obj.value = s.substr(0, i) + charvalue + s.substr(i, s.length);
                ctr.collapse(true);
                ctr.moveStart("character", i + charvalue.length);
                ctr.select();
            }
        </script>

        <div class="popover popover-static">
            <h3 class="popover-title">
                <asp:Literal ID="ltlPageTitle" runat="server" /></h3>
            <div class="popover-content">

                <asp:PlaceHolder ID="GatherRuleBase" runat="server" Visible="false">
                    <table class="table noborder table-hover">
                        <tr>
                            <td width="200">采集规则名称：</td>
                            <td>
                                <asp:TextBox Columns="25" MaxLength="50" ID="GatherRuleName" runat="server" /></td>
                        </tr>
                        <tr>
                            <td>采集到栏目：</td>
                            <td>
                                <asp:DropDownList ID="NodeIDDropDownList" runat="server"></asp:DropDownList></td>
                        </tr>
                        <tr>
                            <td>网页编码：</td>
                            <td>
                                <asp:DropDownList ID="Charset" runat="server"></asp:DropDownList></td>
                        </tr>
                        <tr>
                            <td>采集内容数：</td>
                            <td>
                                <asp:TextBox class="input-mini" Columns="4" MaxLength="4" ID="GatherNum" Style="text-align: right" Text="0" runat="server" />
                                <asp:RequiredFieldValidator
                                    ControlToValidate="GatherNum"
                                    ErrorMessage=" *" ForeColor="red"
                                    Display="Dynamic"
                                    runat="server" />
                                <asp:RegularExpressionValidator
                                    ControlToValidate="GatherNum"
                                    ValidationExpression="\d+"
                                    ErrorMessage="采集数只能是数字"
                                    foreColor="red"
                                    Display="Dynamic"
                                    runat="server" />
                                <br>
                                <span>需要采集的内容数，0代表采集所有内容</span>
                            </td>
                        </tr>
                        <tr>
                            <td>下载内容图片：</td>
                            <td>
                                <asp:RadioButtonList ID="IsSaveImage" RepeatDirection="Horizontal" class="radiobuttonlist" runat="server">
                                    <asp:ListItem Value="True">是</asp:ListItem>
                                    <asp:ListItem Value="False" Selected="true">否</asp:ListItem>
                                </asp:RadioButtonList>
                                <span>下载所采集内容的图片到服务器中</span>
                            </td>
                        </tr>
                        <tr>
                            <td>将首幅图片设为标题图片：</td>
                            <td>
                                <asp:RadioButtonList ID="IsSetFirstImageAsImageUrl" RepeatDirection="Horizontal" class="radiobuttonlist"
                                    runat="server">
                                    <asp:ListItem Value="True" Selected="true">是</asp:ListItem>
                                    <asp:ListItem Value="False">否</asp:ListItem>
                                </asp:RadioButtonList>
                                <span>将内容正文中的首幅图片设为内容的标题图片</span>
                            </td>
                        </tr>
                        <tr>
                            <td>当内容为空时是否采集：</td>
                            <td>
                                <asp:RadioButtonList ID="IsEmptyContentAllowed" RepeatDirection="Horizontal" class="radiobuttonlist" runat="server">
                                    <asp:ListItem Value="True">是</asp:ListItem>
                                    <asp:ListItem Value="False" Selected="true">否</asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                        <tr>
                            <td>当内容标题相同时是否采集：</td>
                            <td>
                                <asp:RadioButtonList ID="IsSameTitleAllowed" RepeatDirection="Horizontal" class="radiobuttonlist" runat="server">
                                    <asp:ListItem Value="True">是</asp:ListItem>
                                    <asp:ListItem Value="False" Selected="true">否</asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                        <tr>
                            <td>不经过审核：</td>
                            <td>
                                <asp:RadioButtonList ID="IsChecked" RepeatDirection="Horizontal" class="radiobuttonlist" runat="server">
                                    <asp:ListItem Value="True" Selected="true">是</asp:ListItem>
                                    <asp:ListItem Value="False">否</asp:ListItem>
                                </asp:RadioButtonList>
                                <span>采集的内容是否不经过审核直接添加到栏目中</span>
                            </td>
                        </tr>
                        <tr>
                            <td>自动生成：</td>
                            <td>
                                <asp:RadioButtonList ID="IsAutoCreate" RepeatDirection="Horizontal" class="radiobuttonlist" runat="server">
                                    <asp:ListItem Value="True" Selected="true">是</asp:ListItem>
                                    <asp:ListItem Value="False">否</asp:ListItem>
                                </asp:RadioButtonList>
                                <span>采集的内容是否自动生成</span>
                            </td>
                        </tr>
                        <tr>
                            <td>倒序采集：</td>
                            <td>
                                <asp:RadioButtonList ID="IsOrderByDesc" RepeatDirection="Horizontal" class="radiobuttonlist" runat="server">
                                    <asp:ListItem Value="True" Selected="true">是</asp:ListItem>
                                    <asp:ListItem Value="False">否</asp:ListItem>
                                </asp:RadioButtonList>
                                <span>采用倒序采集可以保持和被采集的新闻列表顺序一致，建议您选“是”</span>
                            </td>
                        </tr>
                    </table>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="GatherRuleList" runat="server" Visible="false">
                    <table class="table noborder table-hover">
                        <tr>
                            <td width="200">起始网页地址：</td>
                            <td colspan="3">
                                <table cellpadding="4" cellspacing="4" width="100%">
                                    <tr>
                                        <td>
                                            <asp:CheckBox ID="GatherUrlIsCollection" runat="server" AutoPostBack="true" OnCheckedChanged="GatherUrl_CheckedChanged" Text="从多个网址" Checked="true"></asp:CheckBox>
                                            &nbsp;&nbsp;
                    <asp:CheckBox ID="GatherUrlIsSerialize" runat="server" AutoPostBack="true" OnCheckedChanged="GatherUrl_CheckedChanged" Text="从序列相似网址"></asp:CheckBox>
                                        </td>
                                    </tr>
                                    <tr id="GatherUrlCollectionRow" runat="server">
                                        <td>
                                            <asp:TextBox Columns="60" TextMode="MultiLine" Style="height: 60px" ID="GatherUrlCollection" runat="server" />
                                            <span>以换行分割</span>
                                        </td>
                                    </tr>
                                    <tr id="GatherUrlSerializeRow" runat="server">
                                        <td>
                                            <asp:TextBox Columns="60" ID="GatherUrlSerialize" runat="server" />
                                            变动数字: <a href="javascript:;" onclick="AddOnPos(document.getElementById('<%=GatherUrlSerialize.ClientID%>'), '*');" title="遇到变动数字用*代替"><font color="#0000FF">*</font>&nbsp;</a><br>
                                            <br>
                                            变动数字范围: 从
                    <asp:TextBox Columns="5" ID="SerializeFrom" Text="1" runat="server" />
                                            到
                    <asp:TextBox Columns="5" ID="SerializeTo" Text="10" runat="server" />
                                            数字变动倍数:
                    <asp:TextBox Columns="5" Text="1" ID="SerializeInterval" runat="server" />
                                            &nbsp;&nbsp;
                    <asp:CheckBox ID="SerializeIsOrderByDesc" runat="server" Text="倒序"></asp:CheckBox>
                                            <asp:CheckBox ID="SerializeIsAddZero" runat="server" Text="补零"></asp:CheckBox></td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>内容地址包含：</td>
                            <td colspan="3">
                                <asp:TextBox Columns="60" MaxLength="200" ID="UrlInclude" runat="server" />
                                <a href="javascript:;" onclick="AddOnPos(document.getElementById('<%=UrlInclude.ClientID%>'), '*');" title="遇到变动字符用*代替">&nbsp;<font color="#0000FF">*</font>&nbsp;</a><a href="javascript:;" onclick="AddOnPos(document.getElementById('<%=UrlInclude.ClientID%>'), '|');" title="多个条件用|分隔各字符串">&nbsp;<font color="#0000FF">|</font>&nbsp;</a>
                                <br>
                                <span>限定采集内容的地址必须包含的字符串</span>
                            </td>
                        </tr>
                    </table>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="GatherRuleContent" runat="server" Visible="false">
                    <table class="table noborder table-hover">
                        <tr>
                            <td>内容标题开始<br>
                                （必填）：</td>
                            <td>
                                <asp:TextBox Columns="30" Style="height: 60px" TextMode="MultiLine" ID="ContentTitleStart" runat="server" />
                                <a href="javascript:;" onclick="AddOnPos(document.getElementById('<%=ContentTitleStart.ClientID%>'), '*');" title="遇到变动字符用*代替">&nbsp;<font color="#0000FF">*</font></a><a href="javascript:;" onclick="AddOnPos(document.getElementById('<%=ContentTitleStart.ClientID%>'), '|');" title="多个条件用|分隔各字符串">&nbsp;<font color="#0000FF">|</font></a>
                            </td>
                            <td>内容标题结束<br>
                                （必填）：</td>
                            <td>
                                <asp:TextBox Columns="30" Style="height: 60px" TextMode="MultiLine" ID="ContentTitleEnd" runat="server" />
                                <a href="javascript:;" onclick="AddOnPos(document.getElementById('<%=ContentTitleEnd.ClientID%>'), '*');" title="遇到变动字符用*代替">&nbsp;<font color="#0000FF">*</font></a><a href="javascript:;" onclick="AddOnPos(document.getElementById('<%=ContentTitleEnd.ClientID%>'), '|');" title="多个条件用|分隔各字符串">&nbsp;<font color="#0000FF">|</font></a>
                            </td>
                        </tr>
                        <tr>
                            <td>内容正文开始<br>
                                （必填）：</td>
                            <td>
                                <asp:TextBox Columns="30" Style="height: 60px" TextMode="MultiLine" ID="ContentContentStart" runat="server" />
                                <a href="javascript:;" onclick="AddOnPos(document.getElementById('<%=ContentContentStart.ClientID%>'), '*');" title="遇到变动字符用*代替">&nbsp;<font color="#0000FF">*</font></a><a href="javascript:;" onclick="AddOnPos(document.getElementById('<%=ContentContentStart.ClientID%>'), '|');" title="多个条件用|分隔各字符串">&nbsp;<font color="#0000FF">|</font></a>
                            </td>
                            <td>内容正文结束<br>
                                （必填）：</td>
                            <td>
                                <asp:TextBox Columns="30" Style="height: 60px" TextMode="MultiLine" ID="ContentContentEnd" runat="server" />
                                <a href="javascript:;" onclick="AddOnPos(document.getElementById('<%=ContentContentEnd.ClientID%>'), '*');" title="遇到变动字符用*代替">&nbsp;<font color="#0000FF">*</font></a><a href="javascript:;" onclick="AddOnPos(document.getElementById('<%=ContentContentEnd.ClientID%>'), '|');" title="多个条件用|分隔各字符串">&nbsp;<font color="#0000FF">|</font></a>
                            </td>
                        </tr>
                        <tr>
                            <td>内容正文开始2<br>
                                （选填）：</td>
                            <td>
                                <asp:TextBox Columns="30" Style="height: 60px" TextMode="MultiLine" ID="ContentContentStart2" runat="server" />
                                <a href="javascript:;" onclick="AddOnPos(document.getElementById('<%=ContentContentStart2.ClientID%>'), '*');" title="遇到变动字符用*代替">&nbsp;<font color="#0000FF">*</font></a><a href="javascript:;" onclick="AddOnPos(document.getElementById('<%=ContentContentStart2.ClientID%>'), '|');" title="多个条件用|分隔各字符串">&nbsp;<font color="#0000FF">|</font></a>
                            </td>
                            <td>内容正文结束2<br>
                                （选填）：</td>
                            <td>
                                <asp:TextBox Columns="30" Style="height: 60px" TextMode="MultiLine" ID="ContentContentEnd2" runat="server" />
                                <a href="javascript:;" onclick="AddOnPos(document.getElementById('<%=ContentContentEnd2.ClientID%>'), '*');" title="遇到变动字符用*代替">&nbsp;<font color="#0000FF">*</font></a><a href="javascript:;" onclick="AddOnPos(document.getElementById('<%=ContentContentEnd2.ClientID%>'), '|');" title="多个条件用|分隔各字符串">&nbsp;<font color="#0000FF">|</font></a>
                            </td>
                        </tr>
                        <tr>
                            <td>内容正文开始3<br>
                                （选填）：</td>
                            <td>
                                <asp:TextBox Columns="30" Style="height: 60px" TextMode="MultiLine" ID="ContentContentStart3" runat="server" />
                                <a href="javascript:;" onclick="AddOnPos(document.getElementById('<%=ContentContentStart3.ClientID%>'), '*');" title="遇到变动字符用*代替">&nbsp;<font color="#0000FF">*</font></a><a href="javascript:;" onclick="AddOnPos(document.getElementById('<%=ContentContentStart3.ClientID%>'), '|');" title="多个条件用|分隔各字符串">&nbsp;<font color="#0000FF">|</font></a>
                            </td>
                            <td>内容正文结束3<br>
                                （选填）：</td>
                            <td>
                                <asp:TextBox Columns="30" Style="height: 60px" TextMode="MultiLine" ID="ContentContentEnd3" runat="server" />
                                <a href="javascript:;" onclick="AddOnPos(document.getElementById('<%=ContentContentEnd3.ClientID%>'), '*');" title="遇到变动字符用*代替">&nbsp;<font color="#0000FF">*</font></a><a href="javascript:;" onclick="AddOnPos(document.getElementById('<%=ContentContentEnd3.ClientID%>'), '|');" title="多个条件用|分隔各字符串">&nbsp;<font color="#0000FF">|</font></a>
                            </td>
                        </tr>
                        <tr>
                            <td>内容下一页开始<br />
                                （选填）：</td>
                            <td>
                                <asp:TextBox Columns="30" Style="height: 60px" TextMode="MultiLine" ID="ContentNextPageStart" runat="server" />
                                <a href="javascript:;" onclick="AddOnPos(document.getElementById('<%=ContentNextPageStart.ClientID%>'), '*');" title="遇到变动字符用*代替">&nbsp;<font color="#0000FF">*</font></a><a href="javascript:;" onclick="AddOnPos(document.getElementById('<%=ContentNextPageStart.ClientID%>'), '|');" title="多个条件用|分隔各字符串">&nbsp;<font color="#0000FF">|</font></a>
                            </td>
                            <td>内容下一页结束<br />
                                （选填）：</td>
                            <td>
                                <asp:TextBox Columns="30" Style="height: 60px" TextMode="MultiLine" ID="ContentNextPageEnd" runat="server" />
                                <a href="javascript:;" onclick="AddOnPos(document.getElementById('<%=ContentNextPageEnd.ClientID%>'), '*');" title="遇到变动字符用*代替">&nbsp;<font color="#0000FF">*</font></a><a href="javascript:;" onclick="AddOnPos(document.getElementById('<%=ContentNextPageEnd.ClientID%>'), '|');" title="多个条件用|分隔各字符串">&nbsp;<font color="#0000FF">|</font></a>
                            </td>
                        </tr>
                    </table>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="GatherRuleOthers" runat="server" Visible="false">
                    <table class="table noborder table-hover">
                        <tr>
                            <td>内容标题包含（选填）：</td>
                            <td>
                                <asp:TextBox Columns="50" MaxLength="200" ID="TitleInclude" runat="server" />
                                <a href="javascript:;" onclick="AddOnPos(document.getElementById('<%=TitleInclude.ClientID%>'), '*');" title="遇到变动字符用*代替">&nbsp;<font color="#0000FF">*</font>&nbsp;</a><a href="javascript:;" onclick="AddOnPos(document.getElementById('<%=TitleInclude.ClientID%>'), '|');" title="多个条件用|分隔各字符串">&nbsp;<font color="#0000FF">|</font>&nbsp;</a>
                                <br>
                                <span>限定采集内容的标题必须包含的字符串</span>
                            </td>
                        </tr>
                        <tr>
                            <td>区域内网址开始<br />
                                （选填）：</td>
                            <td>
                                <asp:TextBox Columns="30" Style="height: 60px" TextMode="MultiLine" ID="ListAreaStart" runat="server" />
                                <a href="javascript:;" onclick="AddOnPos(document.getElementById('<%=ListAreaStart.ClientID%>'), '*');" title="遇到变动字符用*代替">&nbsp;<font color="#0000FF">*</font></a><a href="javascript:;" onclick="AddOnPos(document.getElementById('<%=ListAreaStart.ClientID%>'), '|');" title="多个条件用|分隔各字符串">&nbsp;<font color="#0000FF">|</font></a>
                            </td>
                            <td>区域内网址结束<br />
                                （选填）：</td>
                            <td>
                                <asp:TextBox Columns="30" Style="height: 60px" TextMode="MultiLine" ID="ListAreaEnd" runat="server" />
                                <a href="javascript:;" onclick="AddOnPos(document.getElementById('<%=ListAreaEnd.ClientID%>'), '*');" title="遇到变动字符用*代替">&nbsp;<font color="#0000FF">*</font></a><a href="javascript:;" onclick="AddOnPos(document.getElementById('<%=ListAreaEnd.ClientID%>'), '|');" title="多个条件用|分隔各字符串">&nbsp;<font color="#0000FF">|</font></a>
                            </td>
                        </tr>
                        <tr>
                            <td>登陆网站Cookie（选填）：</td>
                            <td colspan="3">
                                <asp:TextBox Columns="60" ID="CookieString" runat="server" />
                                <br>
                                <span>采集登陆网站时需要的Cookie字符串</span>
                            </td>
                        </tr>
                        <tr>
                            <td>内容正文排除<br />
                                （选填）：</td>
                            <td colspan="3">
                                <asp:TextBox Columns="60" Style="height: 60px" TextMode="MultiLine" ID="ContentExclude" runat="server" />
                                <a href="javascript:;" onclick="AddOnPos(document.getElementById('<%=ContentExclude.ClientID%>'), '*');" title="遇到变动字符用*代替">&nbsp;<font color="#0000FF">*</font></a><a href="javascript:;" onclick="AddOnPos(document.getElementById('<%=ContentExclude.ClientID%>'), '|');" title="多个条件用|分隔各字符串">&nbsp;<font color="#0000FF">|</font></a>
                                <br>
                                <span>限定采集内容的正文必须排除的字符串</span>
                            </td>
                        </tr>
                        <tr>
                            <td>清除Html标签及包含文字<br />
                                （选填）：</td>
                            <td colspan="3">
                                <asp:CheckBoxList ID="ContentHtmlClearCollection" RepeatDirection="Horizontal" class="checkboxlist" runat="server" RepeatColumns="7">
                                    <asp:ListItem Value="script" Selected="true" Text="脚本&amp;lt;script"></asp:ListItem>
                                    <asp:ListItem Value="object" Selected="true" Text="对象&amp;lt;object"></asp:ListItem>
                                    <asp:ListItem Value="iframe" Selected="true" Text="框架&amp;lt;iframe"></asp:ListItem>
                                    <asp:ListItem Value="a" Text="链接&amp;lt;a"></asp:ListItem>
                                    <asp:ListItem Value="br" Text="换行&amp;lt;br&amp;gt;"></asp:ListItem>
                                    <asp:ListItem Value="table" Text="表格&amp;lt;table"></asp:ListItem>
                                    <asp:ListItem Value="tbody" Text="表格体&amp;lt;tbody"></asp:ListItem>
                                    <asp:ListItem Value="tr" Text="表格行&amp;lt;tr"></asp:ListItem>
                                    <asp:ListItem Value="td" Text="单元&amp;lt;td"></asp:ListItem>
                                    <asp:ListItem Value="font" Text="字体&amp;lt;font"></asp:ListItem>
                                    <asp:ListItem Value="div" Text="层&amp;lt;div"></asp:ListItem>
                                    <asp:ListItem Value="span" Text="SPAN&amp;lt;span"></asp:ListItem>
                                    <asp:ListItem Value="img" Text="图象&amp;lt;img"></asp:ListItem>
                                    <asp:ListItem Value="&nbsp;" Text="空格&amp;nbsp;"></asp:ListItem>
                                </asp:CheckBoxList>
                                <span>采集内容的正文必须清除的Html标签及包含的文字</span>
                            </td>
                        </tr>
                        <tr>
                            <td>清除Html标签<br />
                                （选填）：</td>
                            <td colspan="3">
                                <asp:CheckBoxList ID="ContentHtmlClearTagCollection" RepeatDirection="Horizontal" class="checkboxlist" runat="server" RepeatColumns="7">
                                    <asp:ListItem Value="a" Text="链接&amp;lt;a"></asp:ListItem>
                                    <asp:ListItem Value="table" Text="表格&amp;lt;table"></asp:ListItem>
                                    <asp:ListItem Value="tbody" Text="表格体&amp;lt;tbody"></asp:ListItem>
                                    <asp:ListItem Value="tr" Text="表格行&amp;lt;tr"></asp:ListItem>
                                    <asp:ListItem Value="td" Text="单元&amp;lt;td"></asp:ListItem>
                                    <asp:ListItem Value="p" Text="段落&amp;lt;p"></asp:ListItem>
                                    <asp:ListItem Value="font" Selected="true" Text="字体&amp;lt;font"></asp:ListItem>
                                    <asp:ListItem Value="div" Selected="true" Text="层&amp;lt;div"></asp:ListItem>
                                    <asp:ListItem Value="span" Selected="true" Text="SPAN&amp;lt;span"></asp:ListItem>
                                </asp:CheckBoxList>
                                <span>采集内容的正文必须清除的Html标签</span>
                            </td>
                        </tr>
                        <tr>
                            <td>字符串替换：<br />
                                （选填）：</td>
                            <td>
                                <asp:TextBox Columns="30" Style="height: 60px" TextMode="MultiLine" ID="ContentReplaceFrom" runat="server" />
                                <br>
                                <span>多个替换字符串请用","格开</span>
                            </td>
                            <td>替换为<br />
                                （选填）：</td>
                            <td>
                                <asp:TextBox Columns="30" Style="height: 60px" TextMode="MultiLine" ID="ContentReplaceTo" runat="server" />
                                <br>
                                <span>如果是替换为字符串是多个，可以用","格开，系统会对应替换，否则系统将统一替换</span>
                            </td>
                        </tr>
                        <tr>
                            <td>内容栏目开始<br />
                                （选填）：</td>
                            <td>
                                <asp:TextBox Columns="30" Style="height: 60px" TextMode="MultiLine" ID="ContentChannelStart" runat="server" />
                                <a href="javascript:;" onclick="AddOnPos(document.getElementById('<%=ContentChannelStart.ClientID%>'), '*');" title="遇到变动字符用*代替">&nbsp;<font color="#0000FF">*</font></a><a href="javascript:;" onclick="AddOnPos(document.getElementById('<%=ContentChannelStart.ClientID%>'), '|');" title="多个条件用|分隔各字符串">&nbsp;<font color="#0000FF">|</font></a>
                            </td>
                            <td>内容栏目结束<br />
                                （选填）：</td>
                            <td>
                                <asp:TextBox Columns="30" Style="height: 60px" TextMode="MultiLine" ID="ContentChannelEnd" runat="server" />
                                <a href="javascript:;" onclick="AddOnPos(document.getElementById('<%=ContentChannelEnd.ClientID%>'), '*');" title="遇到变动字符用*代替">&nbsp;<font color="#0000FF">*</font></a><a href="javascript:;" onclick="AddOnPos(document.getElementById('<%=ContentChannelEnd.ClientID%>'), '|');" title="多个条件用|分隔各字符串">&nbsp;<font color="#0000FF">|</font></a>
                            </td>
                        </tr>
                        <tr>
                            <td>其他需要采集的字段<br />
                                （选填）：</td>
                            <td colspan="3">
                                <asp:CheckBoxList ID="ContentAttributes" AutoPostBack="true" OnSelectedIndexChanged="ContentAttributes_SelectedIndexChanged" RepeatColumns="8" RepeatDirection="Horizontal" class="checkboxlist" runat="server"></asp:CheckBoxList>
                            </td>
                        </tr>
                        <asp:Repeater ID="ContentAttributesRepeater" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <bairong:NoTagText ID="HelpStart" runat="server"></bairong:NoTagText>
                                        <span style="display: none">
                                            <asp:Literal ID="ltlAttributeName" runat="server"></asp:Literal>
                                        </span>
                                    </td>
                                    <td>
                                        <asp:TextBox Columns="30" Style="height: 60px" TextMode="MultiLine" ID="ContentStart" runat="server" />
                                        <a href="javascript:;" onclick="AddOnPos(this.parentNode.firstChild, '*');" title="遇到变动字符用*代替">&nbsp;<font color="#0000FF">*</font></a><a href="javascript:;" onclick="AddOnPos(this.parentNode.firstChild, '|');" title="多个条件用|分隔各字符串">&nbsp;<font color="#0000FF">|</font></a>
                                    </td>
                                    <td>
                                        <bairong:NoTagText ID="HelpEnd" runat="server"></bairong:NoTagText>
                                    </td>
                                    <td>
                                        <asp:TextBox Columns="30" Style="height: 60px" TextMode="MultiLine" ID="ContentEnd" runat="server" />
                                        <a href="javascript:;" onclick="AddOnPos(this.parentNode.firstChild, '*');" title="遇到变动字符用*代替">&nbsp;<font color="#0000FF">*</font></a><a href="javascript:;" onclick="AddOnPos(this.parentNode.firstChild, '|');" title="多个条件用|分隔各字符串">&nbsp;<font color="#0000FF">|</font></a>
                                    </td>
                                    <td>
                                        <bairong:NoTagText ID="HelpDefault" runat="server"></bairong:NoTagText>
                                    </td>
                                    <td>
                                        <asp:TextBox runat="server" ID="ContentDefault"></asp:TextBox>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </table>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="Done" runat="server" Visible="false">

                    <blockquote>
                        <p>完成!</p>
                        <small>操作成功。</small>
                    </blockquote>

                </asp:PlaceHolder>
                <asp:PlaceHolder ID="OperatingError" runat="server" Visible="false">

                    <blockquote>
                        <p>发生错误</p>
                        <small>执行向导过程中出错。</small>
                    </blockquote>

                    <div class="alert alert-error">
                        <h4>
                            <asp:Label ID="ErrorLabel" runat="server"></asp:Label></h4>
                    </div>

                </asp:PlaceHolder>

                <hr />
                <table class="table noborder">
                    <tr>
                        <td class="center">
                            <asp:Button class="btn" ID="Previous" OnClick="PreviousPanel" runat="server" Text="< 上一步"></asp:Button>
                            <asp:Button class="btn btn-primary" ID="Next" OnClick="NextPanel" runat="server" Text="下一步 >"></asp:Button>
                        </td>
                    </tr>
                </table>

            </div>
        </div>

    </form>
</body>
</html>

