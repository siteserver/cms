<%@ Page Language="C#" validateRequest="false" Inherits="SiteServer.BackgroundPages.WeiXin.ModalAlbumContentAdd" Trace="false"%>
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
<asp:Button id="BtnSubmit" useSubmitBehavior="false" OnClick="Submit_OnClick" runat="server" style="display:none" />
<bairong:alerts runat="server"></bairong:alerts>
  <bairong:Code type="ajaxupload" runat="server" />
  <script type="text/javascript" src="../../sitefiles/bairong/scripts/swfUpload/swfupload.js"></script>
  <script type="text/javascript" src="../../sitefiles/bairong/scripts/swfUpload/handlers.js"></script>

  <table class="table table-noborder">
     <tr>
      <td width="120">相册名称</td>
      <td >
        <asp:TextBox id="TbTitle" runat="server" />
        <asp:RequiredFieldValidator ControlToValidate="TbTitle" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
      </td>
    </tr>
    <tr>
      <td>相册封面：</td>
      <td style="width:220px;">
       <asp:Literal id="LtlImageUrl" runat="server"/>
       <a id="js_imageUrl" href="javascript:;" onclick="return false;" class="btn btn-success">上传图片</a>
       <input id="imageUrl" name="imageUrl" type="hidden" runat="server" />
      </td>
    </tr>
   
  </table>
      <script type="text/javascript">
          new AjaxUpload('js_imageUrl', {
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
                            $('#preview_imageUrl').attr('src', response.url);
                            $('#imageUrl').val(response.virtualUrl);
                        } else {
                            alert(response.message);
                        }
                    }
                }
            });
   </script>
     
</form>
</body>
</html>
