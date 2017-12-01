<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.WeiXin.PageView360Add" %>
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

      <asp:PlaceHolder id="PhStep1" runat="server">
        <div class="row-fluid">

          <div class="Span6">
            <div class="step">第一步：配置360全景开始属性</div>
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
                <td>360全景状态：</td>
                <td class="checkbox">
                  <asp:CheckBox id="CbIsEnabled" runat="server" checked="true" text="启用360全景" />
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
              <p><strong>360全景进行中显示图片：</strong></p>
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

          <div class="step">第二步：配置360全景图</div>
          
          <div>
            <table class="table table-bordered table-hover">
              <tr class="info thead">
                <td width="40">序号</td>
                <td width="100">图片类型</td>
                <td>图片地址</td>
                <td width="140">操作</td>
              </tr>
              <tr>
                <td class="center">
                  1
                </td>
                <td class="center">
                  前侧图片：
                </td>
                <td>
                  <asp:TextBox id="TbContentImageUrl1" class="itemImageUrl input-xlarge" width="95%" runat="server" />
                </td>
                <td class="center">
                  <div class="btn-group">
                    <asp:Literal id="LtlContentImageUrl1" runat="server" />
                  </div>
                </td>
              </tr>
              <tr>
                <td class="center">
                  2
                </td>
                <td class="center">
                  右侧图片：
                </td>
                <td>
                  <asp:TextBox id="TbContentImageUrl2" class="itemImageUrl input-xlarge" width="95%" runat="server" />
                </td>
                <td class="center">
                  <div class="btn-group">
                    <asp:Literal id="LtlContentImageUrl2" runat="server" />
                  </div>
                </td>
              </tr>
              <tr>
                <td class="center">
                  3
                </td>
                <td class="center">
                  后侧图片：
                </td>
                <td>
                  <asp:TextBox id="TbContentImageUrl3" class="itemImageUrl input-xlarge" width="95%" runat="server" />
                </td>
                <td class="center">
                  <div class="btn-group">
                    <asp:Literal id="LtlContentImageUrl3" runat="server" />
                  </div>
                </td>
              </tr>
              <tr>
                <td class="center">
                  4
                </td>
                <td class="center">
                  左侧图片：
                </td>
                <td>
                  <asp:TextBox id="TbContentImageUrl4" class="itemImageUrl input-xlarge" width="95%" runat="server" />
                </td>
                <td class="center">
                  <div class="btn-group">
                    <asp:Literal id="LtlContentImageUrl4" runat="server" />
                  </div>
                </td>
              </tr>
              <tr>
                <td class="center">
                  5
                </td>
                <td class="center">
                  顶部图片：
                </td>
                <td>
                  <asp:TextBox id="TbContentImageUrl5" class="itemImageUrl input-xlarge" width="95%" runat="server" />
                </td>
                <td class="center">
                  <div class="btn-group">
                    <asp:Literal id="LtlContentImageUrl5" runat="server" />
                  </div>
                </td>
              </tr>
              <tr>
                <td class="center">
                  6
                </td>
                <td class="center">
                  底部图片：
                </td>
                <td>
                  <asp:TextBox id="TbContentImageUrl6" class="itemImageUrl input-xlarge" width="95%" runat="server" />
                </td>
                <td class="center">
                  <div class="btn-group">
                    <asp:Literal id="LtlContentImageUrl6" runat="server" />
                  </div>
                </td>
              </tr>
            </table>
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
      </asp:PlaceHolder>

      </div>

      <input id="imageUrl" name="imageUrl" type="hidden" runat="server" />
      <input id="contentImageUrl" name="contentImageUrl" type="hidden" runat="server" />
  
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
