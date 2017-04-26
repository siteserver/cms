<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalCuttingImage" Trace="false" %>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html ng-app>
<head>
<meta charset="utf-8">
<!--#include file="../inc/header.aspx"-->
</head>

<body>
<!--#include file="../inc/openWindow.html"-->
<form class="form-inline" enctype="multipart/form-data" method="post" runat="server">
<asp:Button id="btnSubmit" useSubmitBehavior="false" OnClick="Submit_OnClick" runat="server" style="display:none" />
<bairong:alerts runat="server"></bairong:alerts>

  <link rel="stylesheet" type="text/css" href="js/jquery.imgareaselect/css/imgareaselect-default.css" />
  <script type="text/javascript" src="js/jquery.imgareaselect/scripts/jquery.imgareaselect.pack.js"></script>
  <script type="text/javascript" src="js/jQueryRotateCompressed.js"></script>

  <script>
    <asp:Literal id="ltlScript" runat="server" />
  
    $(document).ready(function(){

      $('#myTab a').click(function (e) {
        e.preventDefault();
        $(this).tab('show');
      });
      if (imageUrl){
        $('img#imgSize').attr('src', imageUrl);
        $('img#imgRotateAndFlip').attr('src', imageUrl);
        $('#fileUrl').val(virtualUrl);
      }else{
        $('img#imgSize').hide();
        $('img#imgRotateAndFlip').hide();
      }
      
      function preview(img, selection) {
        if (!selection.width || !selection.height)
            return;
        
        var scaleX = 100 / selection.width;
        var scaleY = 100 / selection.height;

        $('#x1').val(selection.x1);
        $('#y1').val(selection.y1);
        $('#x2').val(selection.x2);
        $('#y2').val(selection.y2);
        $('#w').val(selection.width);
        $('#h').val(selection.height);    
      }

      $('img#imgSize').imgAreaSelect({
          handles: true,
          onSelectEnd: preview
      });

      changeSize = function(){
        
        x1 = parseInt($("#x1").val());
        y1 = parseInt($("#y1").val());
        w = parseInt($("#w").val());
        h = parseInt($("#h").val());

        if (isNaN(x1)) { x1 = 0; $("#x1").val('0'); }
        if (isNaN(y1)) { y1 = 0; $("#y1").val('0'); }
        if (isNaN(w)) { w = 0; $("#w").val('0'); }
        if (isNaN(h)) { h = 0; $("#h").val('0'); }

        $("img#imgSize").imgAreaSelect({
          x1: x1,
          y1: y1,
          x2: x1 + w,
          y2: y1 + h
        });
      };

      $('#x1').change(changeSize);
      $('#y1').change(changeSize);
      $('#w').change(changeSize);
      $('#h').change(changeSize);

      changeAspect = function(){
        aspectWidth = parseInt($("#aspectWidth").val());
        aspectHeight = parseInt($("#aspectHeight").val());
        if (!aspectWidth || !aspectHeight) { 
          $("#aspectWidth").val(''); 
          $("#aspectHeight").val('');
          $("img#imgSize").imgAreaSelect({
            aspectRatio: null
          });
        }else{
          var aspectRatio = aspectWidth+':'+aspectHeight;
          $("img#imgSize").imgAreaSelect({
            aspectRatio: aspectRatio
          });
        }
      }

      $('#aspectWidth').change(changeAspect);
      $('#aspectHeight').change(changeAspect);

      $('select#aspect').change(function(){
          var arr = this.value.split(":");
          $('#aspectWidth').val(arr[0]);
          $('#aspectHeight').val(arr[1]);
          changeAspect();
      });

      var rotate = 0;
      var flip = '';

      $('button#rotateLeft').click(function(){
        rotate -= 1;
        $("#imgRotateAndFlip").rotate(90 * rotate);
        $('#rotate').val(rotate);
        $('#flip').val('');
        return false;
      });

      $('button#rotateRight').click(function(){
        rotate += 1;
        $("#imgRotateAndFlip").rotate(90 * rotate);
        $('#rotate').val(rotate);
        $('#flip').val('');
        return false;
      });

      $('button#rotateFlipH').click(function(){
        flip = 'H'
        $("#imgRotateAndFlip").css( {'filter' : 'fliph','-moz-transform': 'matrix(-1, 0, 0, 1, 0, 0)','-webkit-transform': 'matrix(-1, 0, 0, 1, 0, 0)'} );
        $('#rotate').val('');
        $('#flip').val(flip);
        return false;
      });

      $('button#rotateFlipV').click(function(){
        flip = 'V';
        $("#imgRotateAndFlip").css( {'filter' : 'flipv','-moz-transform': 'matrix(1, 0, 0, -1, 0, 0)','-webkit-transform': 'matrix(1, 0, 0, -1, 0, 0)'} );
        $('#rotate').val('');
        $('#flip').val(flip);
        return false;
      });

    });
  </script>

  <ul class="nav nav-pills" id="myTab">
    <li class="active"><a href="#size">图片裁剪</a></li>
    <li><a href="#rotateAndFlip">图片旋转</a></li>
    <!-- <li><a href="#compression ">图片压缩</a></li> -->
  </ul>

  <input id='fileUrl' name="fileUrl" type="hidden" />
  <input id='rotate' name="rotate" type="hidden" />
  <input id='flip' name="flip" type="hidden" />

  <div class="tab-content">
    <div class="tab-pane active" id="size">

        <table class="table table-noborder">
        <tr>
          <td width="220">

            <table>
                  <thead>
                    <tr>
                      <th colspan="2" style="font-size: 110%; font-weight: bold; text-align: left; padding-left: 0.1em;">
                        坐标
                      </th>
                      <th colspan="2" style="font-size: 110%; font-weight: bold; text-align: left; padding-left: 0.1em;">
                        尺寸
                      </th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr>
                      <td style="width: 10%;"><b>X<sub>1</sub>:</b></td>
                      <td style="width: 30%;"><input type="number" id="x1" name="x1" value="" class="input-mini"></td>
                      <td style="width: 20%;"><b>宽:</b></td>
                      <td><input type="number" value="" id="w" name="w" class="input-mini"></td>
                    </tr>
                    <tr>
                      <td><b>Y<sub>1</sub>:</b></td>
                      <td><input type="number" id="y1" name="y1" value="" class="input-mini"></td>
                      <td><b>高:</b></td>
                      <td><input type="number" id="h" name="h" value="" class="input-mini"></td>
                    </tr>
                    <tr>
                      <td><b>X<sub>2</sub>:</b></td>
                      <td><input type="number" id="x2" value="" class="input-mini"></td>
                      <td></td>
                      <td></td>
                    </tr>
                    <tr>
                      <td><b>Y<sub>2</sub>:</b></td>
                      <td><input type="number" id="y2" value="" class="input-mini"></td>
                      <td></td>
                      <td></td>
                    </tr>
                  </tbody>
                </table>

                <hr />

                <table>
                  <thead>
                    <tr>
                      <th colspan="4" style="font-size: 110%; font-weight: bold; text-align: left; padding-left: 0.1em;">
                        比例
                        <select id="aspect" class="input-medium pull-right">
                          <option>选择预设比例</option>
                          <option>1:1</option>
                          <option>2:1</option>
                          <option>1:2</option>
                          <option>4:3</option>
                          <option>3:4</option>
                          <option>16:9</option>
                          <option>9:16</option>
                          <option>不设置图片比例</option>
                        </select>
                      </th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr>
                      <td style="width: 10%;"><b>宽:</b></td>
                      <td style="width: 30%;"><input type="number" id="aspectWidth" value="" class="input-mini"></td>
                      <td style="width: 20%;"><b>高:</b></td>
                      <td><input type="number" value="" id="aspectHeight" class="input-mini"></td>
                    </tr>
                  </tbody>
                </table>            

          </td>
          <td width="5" style="border-right-style: solid;border-right-width: 1px;border-right-color: #DDD;"></td>
          <td>
            <img id="imgSize" />
          </td>
        </tr>
      </table>

    </div>
    <div class="tab-pane" id="rotateAndFlip">

        <table class="table table-noborder">
        <tr>
          <td width="220">

            <table class="table table-noborder">
                  <thead>
                    <tr>
                      <th colspan="2" style="font-size: 110%; font-weight: bold; text-align: left; padding-left: 0.1em;">
                        旋转方式
                      </th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr>
                      <td style="width: 50%;">
                        <button id="rotateLeft" class="btn btn-success"><i class="icon-arrow-left icon-white"></i> 左转</button>
                        <hr />
                        <button id="rotateFlipH" class="btn btn-success"><i class="icon-resize-horizontal icon-white"></i> 水平翻转</button>
                      </td>
                      <td style="width: 50%;">
                        <button id="rotateRight" class="btn btn-success"><i class="icon-arrow-right icon-white"></i> 右转</button>
                        <hr />
                        <button id="rotateFlipV" class="btn btn-success"><i class="icon-resize-vertical icon-white"></i> 垂直翻转</button>
                      </td>
                    </tr>
                  </tbody>
                </table>            

          </td>
          <td width="5" style="border-right-style: solid;border-right-width: 1px;border-right-color: #DDD;"></td>
          <td>
            <img id="imgRotateAndFlip" />
          </td>
        </tr>
      </table>

    </div>
    <div class="tab-pane" id="compression">

        <table class="table table-noborder">
        <tr>
          <td width="220">

            <table class="table table-noborder">
                  <thead>
                    <tr>
                      <th colspan="2" style="font-size: 110%; font-weight: bold; text-align: left; padding-left: 0.1em;">
                        旋转方式
                      </th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr>
                      <td style="width: 50%;">
                        <button id="rotateLeft" class="btn btn-success"><i class="icon-arrow-left icon-white"></i> 左转</button>
                        <hr />
                        <button id="rotateFlipH" class="btn btn-success"><i class="icon-resize-horizontal icon-white"></i> 水平翻转</button>
                      </td>
                      <td style="width: 50%;">
                        <button id="rotateRight" class="btn btn-success"><i class="icon-arrow-right icon-white"></i> 右转</button>
                        <hr />
                        <button id="rotateFlipV" class="btn btn-success"><i class="icon-resize-vertical icon-white"></i> 垂直翻转</button>
                      </td>
                    </tr>
                  </tbody>
                </table>            

          </td>
          <td width="5" style="border-right-style: solid;border-right-width: 1px;border-right-color: #DDD;"></td>
          <td>
            <img id="imgRotateAndFlip" />
          </td>
        </tr>
      </table>

    </div>
  </div>

</form>
</body>
</html>
