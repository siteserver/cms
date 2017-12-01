<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.WeiXin.PageVoteAdd" %>
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

  <script type="text/javascript" src="background_voteAdd.js"></script>
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
            <div class="step">第一步：配置投票开始属性</div>
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
                <td>投票状态：</td>
                <td class="checkbox">
                  <asp:CheckBox id="CbIsEnabled" runat="server" checked="true" text="启用投票" />
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
              <p><strong>投票进行中显示图片：</strong></p>
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
            <div class="step">第二步：配置投票详情页</div>
            <table class="table noborder table-hover">
              <tr>
                <td width="120">投票规则说明：</td>
                <td>
                  <asp:TextBox id="TbContentDescription" textMode="Multiline" class="textarea" rows="4" style="width:95%; padding:5px;" runat="server" />
                </td>
              </tr>
              <tr>
                <td>投票类型：</td>
                <td>                  
                  <asp:DropDownList id="DdlContentIsImageOption" class="input-medium" runat="server"></asp:DropDownList>
                </td>
              </tr>
              <tr>
                <td>单选/多选：</td>
                <td>
                  <asp:DropDownList id="DdlContentIsCheckBox" class="input-medium" runat="server"></asp:DropDownList>
                </td>
              </tr>
            </table>
          </div>

          <div class="Span6">
            <div class="intro-grid">
              <p><strong>投票详情页头部图片：</strong></p>
              <asp:Literal id="LtlContentImageUrl" runat="server" />
              <a id="js_contentImageUrl" href="javascript:;" onclick="return false;" class="btn btn-success">上传</a>
            </div>
          </div>

          <div class="clearfix"></div>
          <hr />

          <script type="text/javascript"><asp:Literal id="LtlVoteItems" runat="server" /></script>
          <script type="text/html" class="itemController">
          <input id="itemCount" name="itemCount" type="hidden" value="{{itemCount}}" />
          <div>
            <div class="step">投票项设置</div>
            <table class="table noborder">
              <tr>
                <td>
                  <table class="table table-bordered table-hover">
                      <tr class="info thead">
                        <td>选项标题</td>
                        {{if isImageOption}}
                        <td>图片地址</td>
                        {{/if}}
                        <td>链接网址</td>
                        <td width="40">票数</td>
                        <td></td>
                      </tr>
                      {{each items}}
                      <tr>
                        <td>
                          <input type="hidden" name="itemID" class="itemID" value="{{$value.id ? $value.id + '' : '0'}}">
                          <input type="text" name="itemTitle" value="{{$value.title}}" class="itemTitle input-xlarge">
                        </td>
                        {{if isImageOption}}
                        <td>
                          <div>
                            <input class="itemImageUrl input-medium" name="itemImageUrl" id="itemImageUrl_{{$index}}" value="{{$value.imageUrl}}" type="text">
                            <div class="btn-group">
                              <a class="btn" href="javascript:;" onclick="itemController.selectImageClick({{$index}});return false;" title="选择"><i class="icon-th"></i></a>
                              <a class="btn" href="javascript:;" onclick="itemController.uploadImageClick({{$index}});return false;" title="上传"><i class="icon-arrow-up"></i></a>
                              <a class="btn" href="javascript:;" onclick="itemController.cuttingImageClick({{$index}});return false;" title="裁切"><i class="icon-crop"></i></a>
                              <a class="btn" href="javascript:;" onclick="itemController.previewImageClick({{$index}});return false;" title="预览"><i class="icon-eye-open"></i></a>
                            </div>
                          </div>
                        </td>
                        {{/if}}
                        <td>
                          <input type="text" name="itemNavigationUrl"  value="{{$value.navigationUrl}}" class="itemNavigationUrl input-medium">
                        </td>
                        <td>
                          <input type="text" name="itemVoteNum" value="{{$value.voteNum ? $value.voteNum + '' : '0'}}" class="itemVoteNum input-mini">
                        </td>
                        <td class="center">
                          {{if $index > 1}}
                          <a href="javascript:;" onclick="itemController.removeItem({{$index}});">删除</a>
                          {{/if}}
                        </td>
                      </tr>
                      {{/each}}
                      <tr>
                        <td colspan="5">
                          <a href="javascript:;" onclick="itemController.addItem({})" class="btn btn-success">再加一项</a>
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
      </asp:PlaceHolder>

      <asp:PlaceHolder id="PhStep3" visible="false" runat="server">
        <div class="row-fluid">

          <div class="Span6">
            <div class="step">第三步：配置投票结束属性</div>
            <table class="table noborder table-hover">
              <tr>
                <td width="120">投票结束主题：</td>
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
                <td>投票结束摘要：</td>
                <td>
                  <asp:TextBox id="TbEndSummary" textMode="Multiline" class="textarea" rows="4" style="width:95%; padding:5px;" runat="server" />
                </td>
              </tr>
            </table>
          </div>

          <div class="Span6">
            <div class="intro-grid">
              <p><strong>投票已结束显示图片：</strong></p>
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
