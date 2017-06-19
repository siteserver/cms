<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.WeiXin.PageAppointmentMultipleAdd" %>
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
  <asp:Literal id="LtlBreadCrumb" runat="server" />
  <bairong:alerts runat="server" />
    <script type="text/javascript" src="background_appointmentSingleAdd.js"></script>
  <script type="text/javascript">
      function addItem(title, mapAddress, tel, publishmentSystemID, appointmentID, appointmentItemID,isAdd) {
          if (isAdd == "True") {
              var count = $("#contents").find("tr").length;
              var tr = $("<tr><td class='center'>" + count + "</td><td>" + title + "</td><td>" + mapAddress + "</td><td class='center'>" + tel + "</td><td class='center'><a id='" + appointmentItemID + "' href='javascript:;'>编辑</a></td><td class='center'><input name='IDCollection' type='checkbox' value='" + appointmentItemID + "'/></td></tr>");
              $("#contents").append(tr);
              $("#" + appointmentItemID).bind("click", function () { openWindow('编辑预约项目', '/siteserver/weixin/modal_appointmentItemAdd.aspx?publishmentSystemID=' + publishmentSystemID + '&appointmentID=' + appointmentID + '&appointmentItemID=' + appointmentItemID + '', 0, 0, 'false'); return false; });
          }
          else {
              $(":checkbox[value=" + appointmentItemID + "]").parent().parent().children().eq(1).html(title);
              $(":checkbox[value=" + appointmentItemID + "]").parent().parent().children().eq(2).html(mapAddress);
              $(":checkbox[value=" + appointmentItemID + "]").parent().parent().children().eq(3).html(tel);
          }
       }
  </script>
  <style type="text/css">
  div.step {
  font-weight: bold;
  font-size: 16px;
  margin-bottom: 10px;
  }

  span.activate_title {
  line-height: 34px;
  font-size: 16px;
  color: #333;
  }

  p.activate_desc {
  width: 100%;
  margin-left: 32px;
  font-size: 13px;
  font-weight: bold;
  }

  div.step_one, div.step_two, div.step_three {
    display: inline-block;
    margin-left: 30px;
    width: 280px;
    height: 190px;
    background: transparent url("images/weixin-activate.png") no-repeat;
  }
  div.step_two, div.step_three {
    margin-top: 20px;
  }
  div.step_one {
    background-position: -40px -48px;
  }
  div.step_two {
    background-position: -395px -48px;
  }
  div.step_three {
    background-position: -760px -48px;
  }
  </style>

  <bairong:Code type="ajaxupload" runat="server" />
  <script type="text/javascript" src="../../sitefiles/bairong/scripts/swfUpload/swfupload.js"></script>
  <script type="text/javascript" src="../../sitefiles/bairong/scripts/swfUpload/handlers.js"></script>

  <div class="popover popover-static operation-area">
    <h3 class="popover-title">
      <asp:Literal id="LtlPageTitle" runat="server" />
    </h3>
    <div class="popover-content">
      <div class="container-fluid" id="weixinactivate">

      <asp:PlaceHolder id="PhStep1" runat="server">
        <div class="row-fluid">

          <div class="Span6">
            <div class="step">第一步：配置微预约开始属性</div>
            <table class="table noborder table-hover">
              <tr>
                <td width="120">主题：</td>
                <td>
                  <asp:TextBox class="input-xlarge" id="TbTitle" runat="server"/>
                  <asp:RequiredFieldValidator
                    ControlToValidate="TbTitle"
                    errorMessage=" *" foreColor="red" 
                    Display="Dynamic"
                    runat="server"/>
                  <asp:RegularExpressionValidator
                    runat="server"
                    ControlToValidate="TbTitle"
                    ValidationExpression="[^']+"
                    errorMessage=" *" foreColor="red" 
                    Display="Dynamic" />
                </td>
              </tr>
              <tr>
                <td>摘要：</td>
                <td>
                  <asp:TextBox id="TbSummary" textMode="Multiline" class="textarea" rows="4" style="width:95%; padding:5px;" runat="server" />
                </td>
              </tr>
              <tr>
                <td>开始时间：</td>
                <td>
                  <bairong:DateTimeTextBox id="dtbStartDate" now="true" showTime="true" Columns="20" runat="server" />
                </td>
              </tr>
              <tr>
                <td>结束时间：</td>
                <td>
                  <bairong:DateTimeTextBox id="dtbEndDate" showTime="true" Columns="20" runat="server" />
                </td>
              </tr>
              <tr>
                <td>预约状态：</td>
                <td class="checkbox">
                  <asp:CheckBox id="CbIsEnabled" runat="server" checked="true" text="启用预约" />
                </td>
              </tr>
              <tr>
                <td>触发关键词：</td>
                <td>
                  <asp:TextBox id="TbKeywords" runat="server" />
                  <asp:RegularExpressionValidator
                    runat="server"
                    ControlToValidate="TbKeywords"
                    ValidationExpression="[^']+"
                    errorMessage=" *" foreColor="red" 
                    Display="Dynamic" />
                    <br>
                  <span class="gray">多个关键词请用空格格开：例如: 微信 腾讯</span> 
                </td>
              </tr>
            </table>
          </div>

          <div class="Span6">
            <div class="intro-grid">
              <p><strong>预约进行中显示图片：</strong></p>
              <asp:Literal id="LtlImageUrl" runat="server" />
              <a id="js_imageUrl" href="javascript:;" onclick="return false;" class="btn btn-success">上传</a>
            </div>
          </div>
        
        </div>

        <script type="text/javascript">
        new AjaxUpload('js_imageUrl', {
         action: '<%=GetUploadUrl()%>',
         name: "Upload",
         data: {},
         onSubmit: function(file, ext) {
           var reg = /^(jpg|jpeg|png|gif)$/i;
           if (ext && reg.test(ext)) {
             //$('#img_upload_txt_').text('上传中... ');
           } else {
             //$('#img_upload_txt_').text('只允许上传JPG,PNG,GIF图片');
             alert('只允许上传JPG,PNG,GIF图片');
             return false;
           }
         },
         onComplete: function(file, response) {
           if (response) {
             response = eval("(" + response + ")");
             if (response.success == 'true') {
               $('#preview_imageUrl').attr('src', response.url);
               $('#imageUrl').val(response.virtualUrl);
             } else {
               alert(response.message);
             }
           }
         }
        });
        </script>
      </asp:PlaceHolder>

      <asp:PlaceHolder id="PhStep2" visible="false" runat="server">
        <div class="row-fluid">

          <div class="Span6">
            <div class="step">第二步：配置预约详情页</div>
            <table class="table noborder table-hover">
              <tr>
                <td width="120">预约说明：</td>
                <td>
                  <asp:TextBox id="TbContentDescription" textMode="Multiline" class="textarea" rows="4" style="width:95%; padding:5px;" runat="server" />
                </td>
              </tr>
             </table>
          </div>

          <div class="Span6">
            <div class="intro-grid">
              <p><strong>预约列表页头部图片：</strong></p>
              <asp:Literal id="LtlContentImageUrl" runat="server" />
              <a id="js_contentImageUrl" href="javascript:;" onclick="return false;" class="btn btn-success">上传</a>
            </div>
          </div>

           <div class="Span6" style="float:right; margin-top:15px;">
            <div class="intro-grid">
              <p><strong>我的预约页头部图片：</strong></p>
              <asp:Literal id="LtlContentResultTopImageUrl" runat="server" />
              <a id="js_contentResultTopImageUrl" href="javascript:;" onclick="return false;" class="btn btn-success">上传</a>
             </div>
          </div>
 
        </div>

        <script type="text/javascript">
        new AjaxUpload('js_contentImageUrl', {
         action: '<%=GetUploadUrl()%>',
         name: "Upload",
         data: {},
         onSubmit: function(file, ext) {
           var reg = /^(jpg|jpeg|png|gif)$/i;
           if (ext && reg.test(ext)) {
             //$('#img_upload_txt_').text('上传中... ');
           } else {
             //$('#img_upload_txt_').text('只允许上传JPG,PNG,GIF图片');
             alert('只允许上传JPG,PNG,GIF图片');
             return false;
           }
         },
         onComplete: function(file, response) {
           if (response) {
             response = eval("(" + response + ")");
             if (response.success == 'true') {
               $('#preview_contentImageUrl').attr('src', response.url);
               $('#contentImageUrl').val(response.virtualUrl);
             } else {
               alert(response.message);
             }
           }
         }
        });
        </script>

         <script type="text/javascript">
              new AjaxUpload('js_contentResultTopImageUrl', {
                  action: '<%=GetUploadUrl()%>',
            name: "Upload",
            data: {},
            onSubmit: function (file, ext) {
                var reg = /^(jpg|jpeg|png|gif)$/i;
                if (ext && reg.test(ext)) {
                    //$('#img_upload_txt_').text('上传中... ');
                } else {
                    //$('#img_upload_txt_').text('只允许上传JPG,PNG,GIF图片');
                    alert('只允许上传JPG,PNG,GIF图片');
                    return false;
                }
            },
            onComplete: function (file, response) {
                if (response) {
                    response = eval("(" + response + ")");
                    if (response.success == 'true') {
                        $('#preview_contentResultTopImageUrl').attr('src', response.url);
                        $('#contentResultTopImageUrl').val(response.virtualUrl);
                    } else {
                        alert(response.message);
                    }
                }
            }
        });
        </script>
      </asp:PlaceHolder>
      <asp:PlaceHolder id="PhStep3" visible="false" runat="server">
       <div>
         <div class="step">第三步：配置预约项目属性</div>
         <table id="contents" class="table table-bordered table-hover" >
        <tr class="info thead">
          <td width="20"></td>
          <td>标题</td>
          <td>预约地址</td>
          <td>预约电话</td>
          <td width="100"></td>
          <td width="20"><input type="checkbox" onClick="selectRows(document.getElementById('contents'), this.checked);" /></td>
        </tr>
        <asp:Repeater ID="RptContents" runat="server">
          <itemtemplate>
            <tr>
              <td class="center">
                <asp:Literal ID="LtlItemIndex" runat="server"></asp:Literal>
              </td>
              <td>
                <asp:Literal ID="LtlTitle" runat="server"></asp:Literal>
              </td>
                 <td>
                <asp:Literal ID="LtlMapAddress" runat="server"></asp:Literal>
              </td>
               <td class="center">
                <asp:Literal ID="LtlTel" runat="server"></asp:Literal>
              </td>
              <td class="center">
                <asp:Literal ID="LtlEditUrl" runat="server"></asp:Literal>
              </td>
              <td class="center">
                <input type="checkbox" name="IDCollection" value='<%#DataBinder.Eval(Container.DataItem, "ID")%>' />
              </td>
            </tr>
          </itemtemplate>
        </asp:Repeater>
      </table>

      <bairong:sqlPager id="SpContents" runat="server" class="table table-pager" />

        <ul class="breadcrumb breadcrumb-button">
        <asp:Button class="btn btn-success" id="BtnAdd" Text="添 加" runat="server" />
       <%-- <asp:Button class="btn" id="BtnDelete" Text="删 除" runat="server" /> --%>
         <input type="button" class="btn" id="BtnDelete" value="删 除" />
        </ul>
          
       </div>
     </asp:PlaceHolder>
    <script type="text/javascript">
       $("#btnDelete").click(function () {
            var idCollection = "";
            $('input[name="IDCollection"]:checked').each(function(){ 
                idCollection+=$(this).val()+','; 
            }); 
           if (idCollection.length <= 0) {
                alert("请选择要删除的预约");
                return false;
            }
            else {
               if (confirm("确定要删除所选择的需要吗？")) {
                     $.post("background_appointmentItemDelete.aspx", { "IDCollection": idCollection }, function (data, status) {
                         if (status == "success") {
                            var IDs = idCollection.split(',');
                            for (var i = 0; i < IDs.length; i++)
                            { 
                                $(":checkbox[value=" + IDs[i]+ "]").parent().parent().remove();
                            }
                        }
                        else {
                            alert("删除失败" + status);
                        }
                    });
                 }
            }
         });

    </script>
          <asp:PlaceHolder ID="PhStep4" Visible="false" runat="server">
                        <div class="row-fluid">
                            <script type="text/javascript"><asp:Literal id="LtlAwardItems" runat="server" /></script>
                            <script type="text/html" class="itemController">
                                <input id="itemCount" name="itemCount" type="hidden" value="{{itemCount}}" />
                                <div>
                                    <div class="step">第四步：配置预约提交表单</div>
                                    <table class="table noborder">
                                        <tr>
                                            <td>
                                                <table class="table noborder table-hover">
                                                    <tbody>
                                                        <tr>
                                                            <td width="160">是否显示姓名字段：</td>
                                                            <td class="checkbox">
                                                                <asp:CheckBox ID="CbIsFormRealName" runat="server" Checked="true" Text="显示" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>姓名重命名：</td>
                                                            <td>
                                                                <asp:TextBox class="input-xlarge" ID="TbFormRealNameTitle" Text="姓名" runat="server" />
                                                                <asp:RequiredFieldValidator
                                                                    ControlToValidate="TbFormRealNameTitle"
                                                                    ErrorMessage=" *" ForeColor="red"
                                                                    Display="Dynamic"
                                                                    runat="server" />
                                                                <asp:RegularExpressionValidator
                                                                    runat="server"
                                                                    ControlToValidate="TbFormRealNameTitle"
                                                                    ValidationExpression="[^']+"
                                                                    ErrorMessage=" *" ForeColor="red"
                                                                    Display="Dynamic" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>是否显示手机字段：</td>
                                                            <td class="checkbox">
                                                                <asp:CheckBox ID="CbIsFormMobile" runat="server" Checked="true" Text="显示" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>手机重命名：</td>
                                                            <td>
                                                                <asp:TextBox class="input-xlarge" ID="TbFormMobileTitle" Text="手机" runat="server" />
                                                                <asp:RequiredFieldValidator
                                                                    ControlToValidate="TbFormMobileTitle"
                                                                    ErrorMessage=" *" ForeColor="red"
                                                                    Display="Dynamic"
                                                                    runat="server" />
                                                                <asp:RegularExpressionValidator
                                                                    runat="server"
                                                                    ControlToValidate="TbFormMobileTitle"
                                                                    ValidationExpression="[^']+"
                                                                    ErrorMessage=" *" ForeColor="red"
                                                                    Display="Dynamic" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>是否显示邮箱字段：</td>
                                                            <td class="checkbox">
                                                                <asp:CheckBox ID="CbIsFormEmail" runat="server" Checked="true" Text="显示" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>邮箱重命名：</td>
                                                            <td>
                                                                <asp:TextBox class="input-xlarge" ID="TbFormEmailTitle" Text="邮箱" runat="server" />
                                                                <asp:RequiredFieldValidator
                                                                    ControlToValidate="TbFormEmailTitle"
                                                                    ErrorMessage=" *" ForeColor="red"
                                                                    Display="Dynamic"
                                                                    runat="server" />
                                                                <asp:RegularExpressionValidator
                                                                    runat="server"
                                                                    ControlToValidate="TbFormEmailTitle"
                                                                    ValidationExpression="[^']+"
                                                                    ErrorMessage=" *" ForeColor="red"
                                                                    Display="Dynamic" />
                                                            </td>
                                                        </tr>
                                                        {{each items}}
                                                        <tr>
                                                            <td>是否显示新字段：</td>
                                                            <td class="checkbox">
                                                                <input id="Cbox_{{$index}}" type="checkbox" name="itemIsVisible" class="itemIsVisible" {{$value.isVisible}}><label for="Cbox_{{$index}}">显示</label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>新字段重命名：
                                                            </td>
                                                            <td>
                                                                <input type="hidden" name="itemID" class="itemID" value="{{$value.id ? $value.id + '' : '0'}}">
                                                                <input type="text" name="itemAttributeName" value="{{$value.attributeName}}" class="itemAttributeName input-xlarge">
                                                                <a href="javascript:;" onclick="itemController.removeItem({{$index}});">删除</a>
                                                            </td>
                                                        </tr>
                                                        {{/each}}
                                                        <tr>
                                                            <td colspan="3" style="text-align: left">
                                                                <a href="javascript:;" onclick="itemController.addItem({})" class="btn btn-success">添加新字段</a>
                                                            </td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </script>
                        </div>
                    </asp:PlaceHolder>
      <asp:PlaceHolder id="PhStep5" visible="false" runat="server">
        <div class="row-fluid">

          <div class="Span6">
            <div class="step">第五步：配置预约结束属性</div>
            <table class="table noborder table-hover">
              <tr>
                <td width="120">预约结束主题：</td>
                <td>
                  <asp:TextBox class="input-large" id="TbEndTitle" runat="server"/>
                  <asp:RequiredFieldValidator
                    ControlToValidate="TbEndTitle"
                    errorMessage=" *" foreColor="red" 
                    Display="Dynamic"
                    runat="server"/>
                  <asp:RegularExpressionValidator
                    runat="server"
                    ControlToValidate="TbEndTitle"
                    ValidationExpression="[^']+"
                    errorMessage=" *" foreColor="red" 
                    Display="Dynamic" />
                </td>
              </tr>
              <tr>
                <td>预约结束摘要：</td>
                <td>
                  <asp:TextBox id="TbEndSummary" textMode="Multiline" class="textarea" rows="4" style="width:95%; padding:5px;" runat="server" />
                </td>
              </tr>
            </table>
          </div>

          <div class="Span6">
            <div class="intro-grid">
              <p><strong>预约已结束显示图片：</strong></p>
              <asp:Literal id="LtlEndImageUrl" runat="server" />
              <a id="js_endImageUrl" href="javascript:;" onclick="return false;" class="btn btn-success">上传</a>
            </div>
          </div>

        </div>

        <script type="text/javascript">
        new AjaxUpload('js_endImageUrl', {
         action: '<%=GetUploadUrl()%>',
         name: "Upload",
         data: {},
         onSubmit: function(file, ext) {
           var reg = /^(jpg|jpeg|png|gif)$/i;
           if (ext && reg.test(ext)) {
             //$('#img_upload_txt_').text('上传中... ');
           } else {
             //$('#img_upload_txt_').text('只允许上传JPG,PNG,GIF图片');
             alert('只允许上传JPG,PNG,GIF图片');
             return false;
           }
         },
         onComplete: function(file, response) {
           if (response) {
             response = eval("(" + response + ")");
             if (response.success == 'true') {
               $('#preview_endImageUrl').attr('src', response.url);
               $('#endImageUrl').val(response.virtualUrl);
             } else {
               alert(response.message);
             }
           }
         }
        });
        </script>
      </asp:PlaceHolder>

      </div>

      <input id="imageUrl" name="imageUrl" type="hidden" runat="server" />
      <input id="contentImageUrl" name="contentImageUrl" type="hidden" runat="server" />
      <input id="contentResultTopImageUrl" name="contentResultTopImageUrl" type="hidden" runat="server" />
      <input id="endImageUrl" name="endImageUrl" type="hidden" runat="server" />
  
      <hr />
      <table class="table table-noborder">
        <tr>
          <td class="center">
            <asp:Button class="btn btn-primary" id="BtnSubmit" text="下一步" OnClick="Submit_OnClick" runat="server"/>
            <asp:Button class="btn" id="BtnReturn" text="返 回" runat="server"/>
          </td>
        </tr>
      </table>
  
    </div>
  </div>

</form>
</body>
</html>
