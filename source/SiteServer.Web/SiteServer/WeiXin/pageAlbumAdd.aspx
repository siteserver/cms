<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.WeiXin.PageAlbumAdd" %>
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
       <div class="row-fluid">

          <div class="Span6">
            <div class="step">配置微相册属性</div>
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
                <td>微相册状态：</td>
                <td class="checkbox">
                  <asp:CheckBox id="CbIsEnabled" runat="server" checked="true" text="启用微相册" />
                </td>
              </tr>
              <tr>
                <td>触发关键词：</td>
                <td>
                  <asp:TextBox id="TbKeywords" runat="server" />
                  <asp:RequiredFieldValidator ControlToValidate="TbKeywords" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
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
              <p><strong>微相册显示图片：</strong></p>
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
     </div>

      <input id="imageUrl" name="imageUrl" type="hidden" runat="server" />
      <input id="contentImageUrl" name="contentImageUrl" type="hidden" runat="server" />
  
      <hr />
      <table class="table table-noborder">
        <tr>
          <td class="center">
            <asp:Button class="btn btn-primary" id="BtnSubmit" text="确 定" OnClick="Submit_OnClick" runat="server"/>
            <asp:Button class="btn" id="BtnReturn" text="返 回" runat="server"/>
          </td>
        </tr>
      </table>
  
    </div>
  </div>

</form>
</body>
</html>
