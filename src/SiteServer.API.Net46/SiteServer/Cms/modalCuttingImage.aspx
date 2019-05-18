<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalCuttingImage" Trace="false" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html class="modalPage">

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
      <link rel="stylesheet" type="text/css" href="js/jquery.imgareaselect/css/imgareaselect-default.css" />
      <script type="text/javascript" src="js/jquery.imgareaselect/scripts/jquery.imgareaselect.pack.js"></script>
      <script type="text/javascript" src="js/jQueryRotateCompressed.js"></script>

      <asp:Literal id="LtlScript" runat="server" />
      <script type="text/javascript">
        $(document).ready(function () {

          if (imageUrl) {
            $('img#imgSize').attr('src', imageUrl);
            $('img#imgRotateAndFlip').attr('src', imageUrl);
            $('#fileUrl').val(virtualUrl);
          } else {
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

          changeSize = function () {

            x1 = parseInt($("#x1").val());
            y1 = parseInt($("#y1").val());
            w = parseInt($("#w").val());
            h = parseInt($("#h").val());

            if (isNaN(x1)) {
              x1 = 0;
              $("#x1").val('0');
            }
            if (isNaN(y1)) {
              y1 = 0;
              $("#y1").val('0');
            }
            if (isNaN(w)) {
              w = 0;
              $("#w").val('0');
            }
            if (isNaN(h)) {
              h = 0;
              $("#h").val('0');
            }

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

          changeAspect = function () {
            aspectWidth = parseInt($("#aspectWidth").val());
            aspectHeight = parseInt($("#aspectHeight").val());
            if (!aspectWidth || !aspectHeight) {
              $("#aspectWidth").val('');
              $("#aspectHeight").val('');
              $("img#imgSize").imgAreaSelect({
                aspectRatio: null
              });
            } else {
              var aspectRatio = aspectWidth + ':' + aspectHeight;
              $("img#imgSize").imgAreaSelect({
                aspectRatio: aspectRatio
              });
            }
          }

          $('#aspectWidth').change(changeAspect);
          $('#aspectHeight').change(changeAspect);

          $('select#aspect').change(function () {
            var arr = this.value.split(":");
            $('#aspectWidth').val(arr[0]);
            $('#aspectHeight').val(arr[1]);
            changeAspect();
          });

          var rotate = 0;
          var flip = '';

          $('button#rotateLeft').click(function () {
            rotate -= 1;
            $("#imgRotateAndFlip").rotate(90 * rotate);
            $('#rotate').val(rotate);
            $('#flip').val('');
            return false;
          });

          $('button#rotateRight').click(function () {
            rotate += 1;
            $("#imgRotateAndFlip").rotate(90 * rotate);
            $('#rotate').val(rotate);
            $('#flip').val('');
            return false;
          });

          $('button#rotateFlipH').click(function () {
            flip = 'H'
            $("#imgRotateAndFlip").css({
              'filter': 'fliph',
              '-moz-transform': 'matrix(-1, 0, 0, 1, 0, 0)',
              '-webkit-transform': 'matrix(-1, 0, 0, 1, 0, 0)'
            });
            $('#rotate').val('');
            $('#flip').val(flip);
            return false;
          });

          $('button#rotateFlipV').click(function () {
            flip = 'V';
            $("#imgRotateAndFlip").css({
              'filter': 'flipv',
              '-moz-transform': 'matrix(1, 0, 0, -1, 0, 0)',
              '-webkit-transform': 'matrix(1, 0, 0, -1, 0, 0)'
            });
            $('#rotate').val('');
            $('#flip').val(flip);
            return false;
          });

        });
      </script>
    </head>

    <body>
      <form runat="server">
        <ctrl:alerts runat="server" />

        <div class="row">
          <ul class="nav nav-tabs tabs m-b-10" id="myTab">
            <li class="active tab">
              <a href="javascript:;" onclick="$('#size').show();$('#rotateAndFlip').hide();$('.tab').removeClass('active');$(this).parent().addClass('active')">图片裁剪</a>
            </li>
            <li class="tab">
              <a href="#rotateAndFlip" onclick="$('#size').hide();$('#rotateAndFlip').show();$('.tab').removeClass('active');$(this).parent().addClass('active')">图片旋转</a>
            </li>
            <!-- <li class="tab">
                <a href="#compression">图片压缩</a>
              </li> -->
          </ul>
        </div>

        <input id='fileUrl' name="fileUrl" type="hidden" />
        <input id='rotate' name="rotate" type="hidden" />
        <input id='flip' name="flip" type="hidden" />

        <div class="tab-content">
          <div class="tab-pane active" id="size">

            <div class="row">
              <div class="col-6">
                <input type="hidden" id="x1" name="x1" value="" class="input-mini">
                <input type="hidden" value="" id="w" name="w" class="input-mini">
                <input type="hidden" id="y1" name="y1" value="" class="input-mini">
                <input type="hidden" id="h" name="h" value="" class="input-mini">
                <input type="hidden" id="x2" value="" class="input-mini">
                <input type="hidden" id="y2" value="" class="input-mini">

                <div class="form-group form-row">
                  <label class="col-2 col-form-label text-right">比例</label>
                  <div class="col-9">
                    <select id="aspect" class="form-control">
                      <option>不设置图片比例</option>
                      <option>1:1</option>
                      <option>2:1</option>
                      <option>1:2</option>
                      <option>4:3</option>
                      <option>3:4</option>
                      <option>16:9</option>
                      <option>9:16</option>
                    </select>
                  </div>
                  <div class="col-1"></div>
                </div>
                <div class="form-group form-row">
                  <label class="col-2 col-form-label text-right">宽（比值）</label>
                  <div class="col-9">
                    <input type="number" id="aspectWidth" value="" class="form-control">
                  </div>
                  <div class="col-1"></div>
                </div>
                <div class="form-group form-row">
                  <label class="col-2 col-form-label text-right">高（比值）</label>
                  <div class="col-9">
                    <input type="number" value="" id="aspectHeight" class="form-control">
                  </div>
                  <div class="col-1"></div>
                </div>

              </div>
              <div class="col-6 text-center">
                <img id="imgSize" />
              </div>
            </div>

            <div class="clearfix"></div>

          </div>
          <div class="tab-pane" id="rotateAndFlip">

            <div class="row">
              <div class="col-6">

                <div class="form-group form-row">
                  <label class="col-2 col-form-label text-right">旋转方式</label>
                  <div class="col-8">
                    <table class="table">
                      <tr>
                        <td style="width: 50%;">
                          <button id="rotateLeft" class="btn btn-success">
                            左转</button>
                          <hr />
                          <button id="rotateFlipH" class="btn btn-success">
                            水平翻转</button>
                        </td>
                        <td style="width: 50%;">
                          <button id="rotateRight" class="btn btn-success">
                            右转</button>
                          <hr />
                          <button id="rotateFlipV" class="btn btn-success">
                            垂直翻转</button>
                        </td>
                      </tr>
                    </table>
                  </div>
                  <div class="col-2"></div>
                </div>

              </div>
              <div class="col-6 text-center">
                <img id="imgRotateAndFlip" />
              </div>
            </div>

            <div class="clearfix"></div>

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
                          <button id="rotateLeft" class="btn btn-success">
                            <i class="icon-arrow-left icon-white"></i> 左转</button>
                          <hr />
                          <button id="rotateFlipH" class="btn btn-success">
                            <i class="icon-resize-horizontal icon-white"></i> 水平翻转</button>
                        </td>
                        <td style="width: 50%;">
                          <button id="rotateRight" class="btn btn-success">
                            <i class="icon-arrow-right icon-white"></i> 右转</button>
                          <hr />
                          <button id="rotateFlipV" class="btn btn-success">
                            <i class="icon-resize-vertical icon-white"></i> 垂直翻转</button>
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

        <hr />

        <div class="text-right mr-1">
          <asp:Button class="btn btn-primary m-l-5" ID="BtnCheck" Text="确 定" OnClick="Submit_OnClick" runat="server" />
          <button type="button" class="btn btn-default m-l-5" onclick="window.parent.layer.closeAll()">取 消</button>
        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->