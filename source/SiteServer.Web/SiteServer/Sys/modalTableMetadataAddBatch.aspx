<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Sys.ModalTableMetadataAddBatch" %>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <!--#include file="../inc/header.aspx"-->
</head>

<body>
    <!--#include file="../inc/openWindow.html"-->
    <form class="form-inline" runat="server" id="MyForm">
        <asp:Button ID="btnSubmit" UseSubmitBehavior="false" OnClick="Submit_OnClick" runat="server" Style="display: none" />
        <bairong:Alerts runat="server"></bairong:Alerts>

        <script language="javascript">
            var counter = 1;
            var vArr = new Array();
            //datatype,length,length_editable
            vArr[0] = new Array('DateTime', 8, false);
            vArr[1] = new Array('Integer', 4, false);
            vArr[2] = new Array('NChar', 50, true);
            vArr[3] = new Array('NText', 16, false);
            vArr[4] = new Array('NVarChar', 255, true);

            function updateTextData(element) {
                var tr = $(element).parents("tr");
                var valueStr = tr.find("select[name='dataType']").val();
                for (var i = 0; i < vArr.length; i++) {
                    if (valueStr == vArr[i][0]) {
                        tr.find("input[name='dataLength']").val(vArr[i][1]);
                        //tr.find("input[name='dataLength']").attr("disabled", (vArr[i][2]) ? null : "disabled");
                    }
                }
            }

            $(function () {
                $("#tb,#addDiv").mouseover(function () {
                    $("#addDiv").show();
                }).mouseout(function () {
                    $("#addDiv").hide();
                });

                $("#addDiv").click(function () {
                    var htmlTmp = '<td>';
                    htmlTmp += '<input name="attributeName" type="text" />';
                    htmlTmp += '</td>';
                    htmlTmp += '<td>';
                    htmlTmp += '<select name="dataType" onchange="updateTextData(this);">';
                    htmlTmp += '<option value="NVarChar">文本</option>';
                    htmlTmp += '<option value="NText">备注</option>';
                    htmlTmp += '<option value="Integer">数字</option>';
                    htmlTmp += '<option value="DateTime">日期/时间</option>';
                    htmlTmp += '</select>';
                    htmlTmp += '</td>';
                    htmlTmp += '<td>';
                    htmlTmp += '<input name="dataLength" type="text" />';
                    htmlTmp += '</td>';
                    htmlTmp += '<td>';
                    htmlTmp += '<a class="delete" href="javascript:;">移除</a>';
                    htmlTmp += '</td>';
                    counter++;
                    htmlTmp = "<tr id='tr" + counter + "'>" + htmlTmp + "</tr>";
                    $("#tb").append(htmlTmp);
                    $("#tb").find("a").css("display", "block");
                });

                $("#tb").click(function (e) {
                    e = e || window.event;
                    var targetEle = e.target;
                    if (!!targetEle) {
                        //删除
                        if (targetEle.nodeName == "A" && $(targetEle).hasClass("delete")) {
                            if (counter > 1) {
                                $(targetEle).parents("tr").remove();
                                counter--;
                            }
                            if (counter == 1) {
                                $("#tb").find("a").css("display", "none");
                            }
                        }
                    }
                });
            });
        </script>

        <table id="tb" class="table table-bordered table-hover">
            <tr class="info thead">
                <td>字段名</td>
                <td>数据类型</td>
                <td>数据长度</td>
                <td></td>
            </tr>
            <tr class="data" id="tr1">
                <td>
                    <input name="attributeName" type="text" />
                </td>
                <td>
                    <select name="dataType" onchange="updateTextData(this);">
                        <option value="NVarChar">文本</option>
                        <option value="NText">备注</option>
                        <option value="Integer">数字</option>
                        <option value="DateTime">日期/时间</option>
                    </select>
                </td>
                <td>
                    <input name="dataLength" type="text" />
                </td>
                <td></td>
            </tr>
        </table>
        <div id="addDiv" style="border: 1px solid gray; height: 40px; border-radius: 10px; text-align: center; display: none; cursor: pointer;">
            <span style="line-height: 38px; font-family: Microsoft Yahei; font-size: 20px; color: gray; letter-spacing: 20px;">新增一行</span>
        </div>
    </form>
</body>
</html>

