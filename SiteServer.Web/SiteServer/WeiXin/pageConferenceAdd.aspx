<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.WeiXin.PageConferenceAdd" %>
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

  <script type="text/javascript" src="background_conferenceAdd.js"></script>
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
            <div class="step">第一步：配置会议（活动）开始属性</div>
            <table class="table noborder table-hover">
              <tr>
                <td width="150">主题：</td>
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
                <td>会议（活动）状态：</td>
                <td class="checkbox">
                  <asp:CheckBox id="CbIsEnabled" runat="server" checked="true" text="启用会议（活动）" />
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
              <p><strong>会议（活动）进行中显示图片：</strong></p>
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
            <div class="step">第二步：配置会议（活动）</div>
            <table class="table noborder table-hover">
              <tr>
                <td width="170">会议（活动）名称：</td>
                <td>
                  <asp:TextBox id="TbConferenceName" class="input-xlarge" style="width:95%; padding:5px;" runat="server" />
                  <asp:RequiredFieldValidator ControlToValidate="TbConferenceName" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
                </td>
              </tr>
              <tr>
                <td>会议（活动）举办地址：</td>
                <td>
                  <asp:TextBox id="TbAddress" class="input-xlarge" style="width:95%; padding:5px;" runat="server" />
                  <asp:RequiredFieldValidator ControlToValidate="TbAddress" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
                </td>
              </tr>
              <tr>
                <td>会议（活动）举办日期：</td>
                <td>
                  <asp:TextBox id="TbDuration" class="input-xlarge" style="width:95%; padding:5px;" runat="server" />
                  <asp:RequiredFieldValidator ControlToValidate="TbDuration" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
                </td>
              </tr>
              <tr>
                <td colspan="2">会议（活动）详细介绍：</td>
              </tr>
              <tr>
                <td colspan="2">
                  <bairong:BREditor id="breIntroduction" width="100%" runat="server" />
                </td>
              </tr>
            </table>
          </div>

          <div class="Span6">
            <div class="intro-grid">
              <p><strong>会议（活动）背景图片：</strong></p>
              <asp:Literal id="LtlBackgroundImageUrl" runat="server" />
              <a id="js_backgroundImageUrl" href="javascript:;" onclick="return false;" class="btn btn-success">上传</a>
            </div>
          </div>

        </div>

        <script type="text/javascript">
        new AjaxUpload('js_backgroundImageUrl', {
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
               $('#preview_backgroundImageUrl').attr('src', response.url);
               $('#backgroundImageUrl').val(response.virtualUrl);
             } else {
               alert(response.message);
             }
           }
         }
        });
        </script>
      </asp:PlaceHolder>

      <asp:PlaceHolder id="PhStep3" visible="false" runat="server">
        <div class="row-fluid">

          <div class="step">第三步：配置会议（活动）日程项</div>

          <table class="table noborder table-hover">
              <tr>
                <td width="100">是否显示日程：</td>
                <td>
                  <asp:CheckBox id="CbIsAgenda" text="显示" checked="true" class="checkbox" runat="server" />
                </td>
                <td width="100">日程项标题：</td>
                <td>
                  <asp:TextBox id="TbAgendaTitle" class="input-xlarge" runat="server" />
                  <asp:RequiredFieldValidator ControlToValidate="TbAgendaTitle" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
                </td>
              </tr>
          </table>

          <script type="text/javascript"><asp:Literal id="LtlAgendaScript" runat="server" /></script>
          <script type="text/html" class="agendaController">
          <input id="agendaCount" name="agendaCount" type="hidden" value="{{agendaCount}}" />
          <div>
            <table class="table noborder">
              <tr>
                <td>
                  <table class="table table-bordered table-hover">
                      <tr class="info thead">
                        <td>时间</td>
                        <td>会议（活动）安排</td>
                        <td>简介</td>
                        <td></td>
                      </tr>
                      {{each items}}
                      <tr>
                        <td>
                          <input type="text" name="itemDateTime"  value="{{$value.dateTime}}" class="itemDateTime input-medium">
                        </td>
                        <td>
                          <input type="text" name="itemTitle" value="{{$value.title}}" class="itemTitle input-xlarge">
                        </td>
                        <td>
                          <input type="text" name="itemSummary" value="{{$value.summary}}" class="itemSummary input-xlarge">
                        </td>
                        <td class="center">
                          {{if $index > 1}}
                          <a href="javascript:;" onclick="agendaController.removeItem({{$index}});">删除</a>
                          {{/if}}
                        </td>
                      </tr>
                      {{/each}}
                      <tr>
                        <td colspan="4">
                          <a href="javascript:;" onclick="agendaController.addItem({})" class="btn btn-success">再加一项</a>
                          <span>至少设置两项</span>
                        </td>
                      </tr>
                  </table>
                </td>
              </tr>
            </table>
          </div>
          </script>

        </div>
      </asp:PlaceHolder>

      <asp:PlaceHolder id="PhStep4" visible="false" runat="server">
        <div class="row-fluid">

          <div class="step">第四步：配置会议（活动）嘉宾项</div>

          <table class="table noborder table-hover">
              <tr>
                <td width="120">是否显示嘉宾：</td>
                <td>
                  <asp:CheckBox id="CbIsGuest" text="显示" checked="true" class="checkbox" runat="server" />
                </td>
                <td width="120">嘉宾项标题：</td>
                <td>
                  <asp:TextBox id="TbGuestTitle" class="input-xlarge" runat="server" />
                  <asp:RequiredFieldValidator ControlToValidate="TbGuestTitle" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
                </td>
              </tr>
          </table>

          <script type="text/javascript"><asp:Literal id="LtlGuestScript" runat="server" /></script>
          <script type="text/html" class="guestController">
          <input id="guestCount" name="guestCount" type="hidden" value="{{guestCount}}" />
          <div>
            <table class="table noborder">
              <tr>
                <td>
                  <table class="table table-bordered table-hover">
                      <tr class="info thead">
                        <td>嘉宾姓名</td>
                        <td>嘉宾职务</td>
                        <td>嘉宾图片</td>
                        <td></td>
                      </tr>
                      {{each items}}
                      <tr>
                        <td>
                          <input type="text" name="itemDisplayName" value="{{$value.displayName}}" class="itemDisplayName input-medium">
                        </td>
                        <td>
                          <input type="text" name="itemPosition"  value="{{$value.position}}" class="itemPosition input-xlarge">
                        </td>
                        <td>
                          <div>
                            <input class="itemPicUrl input-medium" name="itemPicUrl" id="itemPicUrl_{{$index}}" value="{{$value.picUrl}}" type="text">
                            <div class="btn-group">
                              <a class="btn" href="javascript:;" onclick="guestController.selectImageClick({{$index}});return false;" title="选择"><i class="icon-th"></i></a>
                              <a class="btn" href="javascript:;" onclick="guestController.uploadImageClick({{$index}});return false;" title="上传"><i class="icon-arrow-up"></i></a>
                              <a class="btn" href="javascript:;" onclick="guestController.cuttingImageClick({{$index}});return false;" title="裁切"><i class="icon-crop"></i></a>
                              <a class="btn" href="javascript:;" onclick="guestController.previewImageClick({{$index}});return false;" title="预览"><i class="icon-eye-open"></i></a>
                            </div>
                          </div>
                        </td>
                        <td class="center">
                          {{if $index > 1}}
                          <a href="javascript:;" onclick="guestController.removeItem({{$index}});">删除</a>
                          {{/if}}
                        </td>
                      </tr>
                      {{/each}}
                      <tr>
                        <td colspan="5">
                          <a href="javascript:;" onclick="guestController.addItem({})" class="btn btn-success">再加一项</a>
                          <span>至少设置两项，除链接网址外，其它均为必填项</span>
                        </td>
                      </tr>
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
            <div class="step">第五步：配置会议（活动）结束属性</div>
            <table class="table noborder table-hover">
              <tr>
                <td width="170">会议（活动）结束主题：</td>
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
                <td>会议（活动）结束摘要：</td>
                <td>
                  <asp:TextBox id="TbEndSummary" textMode="Multiline" class="textarea" rows="4" style="width:95%; padding:5px;" runat="server" />
                </td>
              </tr>
            </table>
          </div>

          <div class="Span6">
            <div class="intro-grid">
              <p><strong>会议（活动）已结束显示图片：</strong></p>
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
      <input id="backgroundImageUrl" name="backgroundImageUrl" type="hidden" runat="server" />
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
