// Extended fabric line class
fabric.LineArrow = fabric.util.createClass(fabric.Line, {

  type: 'lineArrow',

  initialize: function(element, options) {
    options || (options = {});
    this.callSuper('initialize', element, options);
  },

  toObject: function() {
    return fabric.util.object.extend(this.callSuper('toObject'));
  },

  _render: function(ctx) {
    this.callSuper('_render', ctx);

    // do not render if width/height are zeros or object is not visible
    if (this.width === 0 || this.height === 0 || !this.visible) return;

    ctx.save();

    var xDiff = this.x2 - this.x1;
    var yDiff = this.y2 - this.y1;
    var angle = Math.atan2(yDiff, xDiff);
    ctx.translate((this.x2 - this.x1) / 2, (this.y2 - this.y1) / 2);
    ctx.rotate(angle);
    ctx.beginPath();
    //move 10px in front of line to start the arrow so it does not have the square line end showing in front (0,0)
    ctx.moveTo(10, 0);
    ctx.lineTo(-20, 15);
    ctx.lineTo(-20, -15);
    ctx.closePath();
    ctx.fillStyle = this.stroke;
    ctx.fill();

    ctx.restore();

  }
});

fabric.LineArrow.fromObject = function(object, callback) {
  callback && callback(new fabric.LineArrow([object.x1, object.y1, object.x2, object.y2], object));
};

fabric.LineArrow.async = true;

// http://fabricjs.com/
// https://github.com/Couy69/vue-fabric-drawingboard/blob/master/src/App.vue
// http://couy.xyz/fabricDrawingBoard/

var deleteIcon = "data:image/svg+xml,%3C%3Fxml version='1.0' encoding='utf-8'%3F%3E%3C!DOCTYPE svg PUBLIC '-//W3C//DTD SVG 1.1//EN' 'http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd'%3E%3Csvg version='1.1' id='Ebene_1' xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' x='0px' y='0px' width='595.275px' height='595.275px' viewBox='200 215 230 470' xml:space='preserve'%3E%3Ccircle style='fill:%23F44336;' cx='299.76' cy='439.067' r='218.516'/%3E%3Cg%3E%3Crect x='267.162' y='307.978' transform='matrix(0.7071 -0.7071 0.7071 0.7071 -222.6202 340.6915)' style='fill:white;' width='65.545' height='262.18'/%3E%3Crect x='266.988' y='308.153' transform='matrix(0.7071 0.7071 -0.7071 0.7071 398.3889 -83.3116)' style='fill:white;' width='65.544' height='262.179'/%3E%3C/g%3E%3C/svg%3E";

var img = document.createElement('img');
img.src = deleteIcon;

fabric.Object.prototype.transparentCorners = false;
fabric.Object.prototype.cornerColor = 'blue';
fabric.Object.prototype.cornerStyle = 'circle';

fabric.Object.prototype.originX = fabric.Object.prototype.originY = 'center';
fabric.Text.prototype.originX = 'center';

fabric.Object.prototype.controls.deleteControl = new fabric.Control({
  x: 0.5,
  y: -0.5,
  offsetY: 16,
  cursorStyle: 'pointer',
  mouseUpHandler: deleteObject,
  render: renderIcon,
  cornerSize: 24
});

function deleteObject(eventData, transform) {
  var target = transform.target;
  var canvas = target.canvas;
      canvas.remove(target);
      canvas.requestRenderAll();
}

function renderIcon(ctx, left, top, styleOverride, fabricObject) {
  var size = this.cornerSize;
  ctx.save();
  ctx.translate(left, top);
  ctx.rotate(fabric.util.degreesToRadians(fabricObject.angle));
  ctx.drawImage(img, -size/2, -size/2, size, size);
  ctx.restore();
}

var $url = "/cms/editor/imageEditor";

var data = utils.init({
  siteId: utils.getQueryInt('siteId'),
  imageUrl: utils.getQueryString('imageUrl'),
  attributeName: utils.getQueryString('attributeName'),
  no: utils.getQueryInt('no'),
  tabName: utils.getQueryString('tabName'),
  content: null,
  canvas: null,
  backgroundWidth: null,
  backgroundHeight: null,
  isChanged: false,
  isDrawingMode: false,
  dialogTextVisible: false,
  textForm: {
    arrowType: '1',
    isLarge: false,
    isEdit: false,
    text: '',
  },

  width: 1280,
  height: 720,
  rect: [],
  showMenu: false,
  x: "",
  y: "",

  mouseFrom: {},
  mouseTo: {},
  drawType: 'select',  //当前绘制图像的种类
  canvasObjectIndex: 0,
  rectangleLabel: "warning",
  drawWidth: 4, //笔触宽度
  color: "#E34F51", //画笔颜色
  drawingObject: null, //当前绘制对象
  moveCount: 1, //绘制移动计数器
  doDrawing: false, // 绘制状态
});

var methods = {
  drawing: function(e) {
    if (this.drawingObject) {
      this.canvas.remove(this.drawingObject);
    }
    var canvasObject = null;
    var left = this.mouseFrom.x,
      top = this.mouseFrom.y,
      mouseFrom = this.mouseFrom,
      mouseTo = this.mouseTo;
    switch (this.drawType) {
      case "arrow": //箭头
        var x1 = mouseFrom.x,
          x2 = mouseTo.x,
          y1 = mouseFrom.y,
          y2 = mouseTo.y;
        var w = x2 - x1,
          h = y2 - y1,
          sh = Math.cos(Math.PI / 4) * 16;
        var sin = h / Math.sqrt(Math.pow(w, 2) + Math.pow(h, 2));
        var cos = w / Math.sqrt(Math.pow(w, 2) + Math.pow(h, 2));
        var w1 = (16 * sin) / 4,
          h1 = (16 * cos) / 4,
          centerx = sh * cos,
          centery = sh * sin;
        /**
         * centerx,centery 表示起始点，终点连线与箭头尖端等边三角形交点相对x，y
         * w1 ，h1用于确定四个点
         */

        var points = [x1, y1, x2, y2];
        canvasObject = new fabric.LineArrow(points, {
          strokeWidth: 5,
          fill: 'red',
          stroke: 'red',
          originX: 'center',
          originY: 'center',
          hasBorders: false,
          hasControls: false
        });

        // var path = " M " + x1 + " " + y1;
        // path += " L " + (x2 - centerx + w1) + " " + (y2 - centery - h1);
        // path +=
        //   " L " + (x2 - centerx + w1 * 2) + " " + (y2 - centery - h1 * 2);
        // path += " L " + x2 + " " + y2;
        // path +=
        //   " L " + (x2 - centerx - w1 * 2) + " " + (y2 - centery + h1 * 2);
        // path += " L " + (x2 - centerx - w1) + " " + (y2 - centery + h1);
        // path += " Z";
        // canvasObject = new fabric.Path(path, {
        //   stroke: this.color,
        //   fill: this.color,
        //   strokeWidth: this.drawWidth
        // });
        break;
      case "ellipse": //椭圆
        // 按shift时画正圆，只有在鼠标移动时才执行这个，所以按了shift但是没有拖动鼠标将不会画圆
        if (e.e.shiftKey) {
          mouseTo.x - left > mouseTo.y - top ? mouseTo.y = top + mouseTo.x - left : mouseTo.x = left + mouseTo.y - top
        }
        var radius =
          Math.sqrt(
            (mouseTo.x - left) * (mouseTo.x - left) +
            (mouseTo.y - top) * (mouseTo.y - top)
          ) / 2;
        canvasObject = new fabric.Ellipse({
          left: (mouseTo.x - left) / 2 + left,
          top: (mouseTo.y - top) / 2 + top,
          stroke: this.color,
          fill: "rgba(255, 255, 255, 0)",
          originX: "center",
          originY: "center",
          rx: Math.abs(left - mouseTo.x) / 2,
          ry: Math.abs(top - mouseTo.y) / 2,
          strokeWidth: this.drawWidth
        });
        break;
      case "rectangle": //长方形
        // 按shift时画正方型
        if (e.e.shiftKey) {
          mouseTo.x - left > mouseTo.y - top ? mouseTo.y = top + mouseTo.x - left : mouseTo.x = left + mouseTo.y - top
        }
        var path =
          "M " +
          mouseFrom.x +
          " " +
          mouseFrom.y +
          " L " +
          mouseTo.x +
          " " +
          mouseFrom.y +
          " L " +
          mouseTo.x +
          " " +
          mouseTo.y +
          " L " +
          mouseFrom.x +
          " " +
          mouseTo.y +
          " L " +
          mouseFrom.x +
          " " +
          mouseFrom.y +
          " z";
        canvasObject = new fabric.Path(path, {
          left: (mouseTo.x - left) / 2 + left,
          top: (mouseTo.y - top) / 2 + top,
          stroke: this.color,
          strokeWidth: this.drawWidth,
          fill: "rgba(255, 255, 255, 0)",
        });
        //也可以使用fabric.Rect
        break;

      default:
        break;
    }

    if (canvasObject) {
      // canvasObject.index = getCanvasObjectIndex();\
      this.canvas.add(canvasObject); //.setActiveObject(canvasObject)
      this.drawingObject = canvasObject;
    }
  },

  deleteObj: function() {
    var $this = this;
    this.canvas.getActiveObjects().map(function(item) {
      $this.canvas.remove(item);
    });
    this.canvas.discardActiveObject().renderAll();
  },

  mousedown: function(e) {
    // 记录鼠标按下时的坐标
    var xy = e.pointer || this.transformMouse(e.e.offsetX, e.e.offsetY);
    this.mouseFrom.x = xy.x;
    this.mouseFrom.y = xy.y;
    this.doDrawing = true;
  },

  // 鼠标松开执行
  mouseup: function(e) {
    var xy = e.pointer || this.transformMouse(e.e.offsetX, e.e.offsetY);
    this.mouseTo.x = xy.x;
    this.mouseTo.y = xy.y;
    this.drawingObject = null;
    this.moveCount = 1;
    this.doDrawing = false;
  },

  //鼠标移动过程中已经完成了绘制
  mousemove: function(e) {
    if (this.moveCount % 2 && !this.doDrawing) {
      //减少绘制频率
      return;
    }
    this.moveCount++;
    var xy = e.pointer || this.transformMouse(e.e.offsetX, e.e.offsetY);
    this.mouseTo.x = xy.x;
    this.mouseTo.y = xy.y;
    this.drawing(e);
  },

  mousedblclick: function(e) {
    var activeObject = this.canvas.getActiveObject();
    if (activeObject && activeObject._objects && activeObject._objects.length == 2) {
      var textObject = activeObject._objects[1];
      this.btnTextClick(textObject.text);
    }
  },

  drawTypeChange: function() {
    var $this = this;
    this.canvas.skipTargetFind = this.drawType != 'select';
    if (this.drawType == "mosaic") {
      this.btnMosaicClick();
      // this.canvas.isDrawingMode = true;
    } else if (this.drawType == 'text') {
      this.btnTextClick();
      setTimeout(function() {
        $this.drawType = 'select';
        $this.canvas.isDrawingMode = false;
        $this.canvas.skipTargetFind = false;
      }, 100);
    } else {
      this.canvas.isDrawingMode = false;
    }
  },

  // 保存当前画布为png图片
  save: function() {
    var canvas = document.getElementById('canvas')
    var imgData = canvas.toDataURL('png');
    imgData = imgData.replace('image/png', 'image/octet-stream');

    // 下载后的问题名，可自由指定
    var filename = 'image.png';
    this.saveFile(imgData, filename);
  },

  saveFile: function(data, filename) {
    var save_link = document.createElement('a');
    save_link.href = data;
    save_link.download = filename;

    var event = document.createEvent('MouseEvents');
    event.initMouseEvent('click', true, false, window, 0, 0, 0, 0, 0, false, false, false, false, 0, null);
    save_link.dispatchEvent(event);
  },

  runRestore: function(json, imageUrl) {
    if (json) {
      this.loadJson(json);
    } else {
      this.loadImage(imageUrl);
    }
    this.isChanged = true;
  },

  apiGet: function () {
    var $this = this;

    setTimeout(function() {
      utils.loading($this, false);

      $this.loadImage($this.imageUrl);

      document.onkeydown = function(e) {
        if (e.key == 'Backspace') {
          var activeObject = $this.canvas.getActiveObject();
          if (activeObject && activeObject.isEditing) return;
          $this.deleteObj();
        }
      };
    }, 100);
  },

  canvasModifiedCallback: function() {
    this.isChanged = true;
  },

  loadImage: function(imageUrl) {
    var $this = this;

    var background = new Image();
    background.src = imageUrl;

    background.onload = function () {
      var canvas = $this.canvas = new fabric.Canvas("canvas", {
        hoverCursor: "pointer",
        selection: true,
        selectionBorderColor: "green",
        backgroundColor: null,
      });

      var width = $('.el-card__body').width() * 0.8;
      var height = (width / background.width) * background.height;

      canvas.setDimensions({ width: width, height: height });
      var f_img = new fabric.Image(background);
      let scaleRatio = Math.min(canvas.width / background.width, canvas.height / background.height);
      canvas.setBackgroundImage(f_img, canvas.renderAll.bind(canvas), {
        scaleX: scaleRatio,
        scaleY: scaleRatio,
        left: canvas.width / 2,
        top: canvas.height / 2,
        originX: 'middle',
        originY: 'middle',
        backgroundImageOpacity: 0.5,
        backgroundImageStretch: false,
      });

      $this.canvas.on('object:added', $this.canvasModifiedCallback);
      $this.canvas.on('object:removed', $this.canvasModifiedCallback);
      $this.canvas.on('object:modified', $this.canvasModifiedCallback);

      $this.canvas.on("mouse:down", $this.mousedown);
      $this.canvas.on("mouse:move", $this.mousemove);
      $this.canvas.on("mouse:up", $this.mouseup);

      $this.canvas.on("mouse:dblclick", $this.mousedblclick);
    };
  },

  loadJson: function(json) {
    var $this = this;

    var imageUrl = this.content.imageUrl;
    var background = new Image();
    background.src = imageUrl;

    background.onload = function () {
      var canvas = $this.canvas = new fabric.Canvas("canvas", {
        hoverCursor: "pointer",
        selection: true,
        selectionBorderColor: "green",
        backgroundColor: null,
      });

      var width = $('.el-card__body').width() * 0.8;
      var height = (width / background.width) * background.height;
      canvas.setDimensions({ width: width, height: height });
      canvas.loadFromJSON(json, function() {
        canvas.renderAll();

        canvas.on('object:added', $this.canvasModifiedCallback);
        canvas.on('object:removed', $this.canvasModifiedCallback);
        canvas.on('object:modified', $this.canvasModifiedCallback);

        canvas.on("mouse:down", $this.mousedown);
        canvas.on("mouse:move", $this.mousemove);
        canvas.on("mouse:up", $this.mouseup);

        canvas.on("mouse:dblclick", $this.mousedblclick);
      });
    };
  },

  apiSubmit: function () {
    var $this = this;
    utils.loading(this, true);

    var json = JSON.stringify(this.canvas);

    var canvas = document.getElementById('canvas')
    var imgData = canvas.toDataURL('png');
    var base64String = imgData.replace('data:image/png;base64,', '');

    $api.post($url, {
      siteId: this.siteId,
      json: json,
      base64String: base64String,
    }).then(function (response) {
      var res = response.data;

      var tabVue = utils.getTabVue($this.tabName);
      if (tabVue) {
        if (tabVue.runMaterialLayerImageSelect) {
          utils.success('图片编辑成功！');
          tabVue.runMaterialLayerImageSelect($this.attributeName, $this.no, res.value);
        }
      }
      utils.removeTab();
      utils.openTab($this.tabName);
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnText1Click: function() {
    var text = new fabric.IText('hello world', {
      left: 200,
      top: 100,
      backgroundColor: "yellow",
      borderColor : 'red'
    })
    this.canvas.add(text);
    // this.canvas.getObjects()[0].drawBorders(this.canvas.getContext());
  },

  btnText2Click: function() {
    var text = new fabric.IText('hello world', {
      left: 200,
      top: 100,
      backgroundColor: "yellow",
      stroke: 'red',
      strokeWidth: 4,
      paintFirst: 'stroke', // stroke behind fill
    })
    this.canvas.add(text);
    // this.canvas.getObjects()[0].drawBorders(this.canvas.getContext());
  },

  btnText3Click: function() {
    var circle = new fabric.Circle({
      radius: 100,
      fill: '#eef',
      scaleY: 0.5,
      originX: 'center',
      originY: 'center'
    });

    var text = new fabric.IText('hello world', {
      fontSize: 30,
      originX: 'center',
      originY: 'center'
    });

    var group = new fabric.Group([ circle, text ], {
      left: 150,
      top: 100,
    });

    this.canvas.add(group);
  },

  btnText4Click: function() {
    var rect = new fabric.Rect({
      left: 100,
      top: 50,
      fill: 'yellow',
      width: 220,
      height: 60,
      objectCaching: false,
      stroke: 'red',
      strokeWidth: 4,
    });
    this.canvas.add(rect);

    var text = new fabric.IText('hello world', {
      left: 120,
      top: 60,
      backgroundColor: "yellow",
      originX: 'center',
      originY: 'center',
      centeredRotation: true,
    })
    this.canvas.add(text);
    this.canvas.setActiveObject(text);
  },

  btnArrowClick: function() {
    let x1 = mouseFrom.x,x2= mouseTo.x,y1 = mouseFrom.y,y2= mouseTo.y;
    let w = (x2-x1),h = (y2-y1),sh = Math.cos(Math.PI/4)*16
    let sin = h/Math.sqrt(Math.pow(w,2)+Math.pow(h,2))
    let cos = w/Math.sqrt(Math.pow(w,2)+Math.pow(h,2))
    let w1 =((16*sin)/4),h1 = ((16*cos)/4),centerx=sh*cos,centery=sh*sin
    /**
     * centerx,centery 表示起始点，终点连线与箭头尖端等边三角形交点相对x，y
     * w1 ，h1用于确定四个点
    */
    let path = " M " + x1 + " " + (y1);
      path += " L " + (x2-centerx+w1) + " " + (y2-centery-h1);
      path += " L " + (x2-centerx+w1*2) + " " + (y2-centery-h1*2);
      path += " L " + (x2) + " " + y2;
      path += " L " + (x2-centerx-w1*2) + " " + (y2-centery+h1*2);
      path += " L " + (x2-centerx-w1) + " " + (y2-centery+h1);
      path += " Z";
    canvasObject = new fabric.Path(
      path,
      {
        stroke: this.color,
        fill: this.color,
        strokeWidth: this.drawWidth
      }
    );
    this.canvas.add(canvasObject);
  },

  btnTextClick: function(text) {
    this.dialogTextVisible = true;
    this.textForm.arrowType = '1';
    this.textForm.isLarge = false;
    if (text) {
      this.textForm.isEdit = true;
      this.textForm.text = text;
    } else {
      this.textForm.isEdit = false;
    }
  },

  btnTextBoxClick: function(command) {
    this.dialogTextVisible = true;
    this.textForm.arrowType = command;
    // if (text) {
    //   this.textForm.isEdit = true;
    //   this.textForm.text = text;
    // } else {
    //   this.textForm.isEdit = false;
    // }
  },

  btnTextSubmitClick: function() {
    var $this = this;
    if (!this.textForm.text) return;

    if (this.textForm.arrowType) {
      // var top = 180;
      // var left = 200;
      // if (this.textForm.arrowType == '2' || this.textForm.arrowType == '4') {
      //   top = 135;
      // }
      // var scale = 0.3;
      // if (this.textForm.isLarge) {
      //   scale = 0.45;
      //   left += 70;
      //   top = 230;
      //   if (this.textForm.arrowType == '2' || this.textForm.arrowType == '4') {
      //     top = 143;
      //   }
      // }

      var imageName = 'arrow' + this.textForm.arrowType + (this.textForm.isLarge ? 'l' : 's') + '.png';
      var top = 200;
      var left = 200;
      if (this.textForm.arrowType == '2' || this.textForm.arrowType == '4') {
        top = 135;
      }
      var scale = 1;
      if (this.textForm.isLarge) {
        scale = 1;
        left += 70;
        top = 230;
        if (this.textForm.arrowType == '2' || this.textForm.arrowType == '4') {
          top = 143;
        }
      }

      fabric.Image.fromURL('/sitefiles/assets/images/' + imageName, function(img) {
        img.scale(scale).set({
          left: left,
          top: top,
          angle: 0,
        });

        var text = new fabric.IText($this.textForm.text, {
          left: left,
          top: 160,
          fontSize: 20,
          fontFamily: "Microsoft YaHei",
          strokeWidth:0,
          originX: 'center',
          originY: 'center',
          fill: "red"
        });

        $this.canvas.add(img, text).calcOffset();

        $this.dialogTextVisible = false;
        $this.textForm.text = '';
      });


      return;
    }

    var maxLines = [];
    for (const line of this.textForm.text.split('\n')) {
      if (line.length >= 40) {
        maxLines.push(line.substring(0, 40));
      } else {
        maxLines.push(line);
      }
    }
    this.textForm.text = maxLines.join('\n');
    var lines = this.textForm.text.split('\n');
    var length = 0;
    for (const line of lines) {
      if (line.length > length) {
        length = line.length;
      }
    }
    var bgWidth = 22 * length;
    var bgHeight = 28 * lines.length;
    var triangleTop = 20 * lines.length + 75;
    var groupLeft = bgWidth + 100;
    var groupTop = 60;
    if (this.textForm.isEdit) {
      var activeObject = this.canvas.getActiveObject();
      if (activeObject) {
        groupLeft = activeObject.left;
        groupTop = activeObject.top;
        this.canvas.remove(activeObject);
        this.canvas.requestRenderAll();
      }
    }

    var rect = new fabric.Rect({
      width: bgWidth + 20,
      height: bgHeight + 20,
      fill: '#fece00',
      rx: 10,
      ry: 10,
      stroke: 'red',
      strokeWidth: 0,
    });
    var text = new fabric.Text(this.textForm.text, { fontSize: 20, fontFamily: "Microsoft YaHei", strokeWidth:0 });
    var group = new fabric.Group([ rect, text ], { left: groupLeft, top: groupTop });

    if (this.textForm.isEdit) {
      this.canvas.add(group).calcOffset();
    } else {
      var triangle = new fabric.Triangle({
        left: bgWidth + 90,
        top: triangleTop,
        angle: 210,
        width: 20,
        height: 50,
        fill: '#fece00',
        rx: 10,
        ry: 10,
        stroke: 'red',
        strokeWidth: 0,
      });
      this.canvas.add(triangle, group).calcOffset();
    }

    this.dialogTextVisible = false;
    this.textForm.text = '';
  },

  btnText5Click: function() {
    var text = new fabric.IText("你好，世界", {
      left: 100,
      top: 100,
      fontFamily: "Microsoft YaHei",
      fontSize: 30,
      originX: 'center',
      originY: 'center',
      centeredRotation: true,
      fill: "red"
    });

    // var text = new fabric.IText("你好，世界", {
    //     originX: "left",
    //     originY: "top",
    //     lockMovementX: false,
    //     lockMovementY: false,
    //     lockScalingX: false,
    //     lockScalingY: false,
    //     lockRotation: false,
    //     lockUniScaling: false,
    //     lockScalingFlip: false,
    //     borderColor: "rgba(102,153,255,0.75)",
    //     cornerColor: "rgba(102,153,255,0.5)",
    //     transparentCorners: true,
    //     padding: 0,
    //     hasBorders: true,
    //     hasControls: true,
    //     cornerSize: 12,
    //     id: 5,
    //     nombre: "Objeto_5",
    //     lnk: "Pantalla_1",
    //     left: 10,
    //     top: 50,
    //     width: 300,
    //     height: 52,
    //     fill: "red",
    //     stroke: null,
    //     strokeWidth: 1,
    //     strokeDashArray: null,
    //     strokeLineCap: "butt",
    //     strokeLineJoin: "miter",
    //     strokeMiterLimit: 10,
    //     scaleX: 1,
    //     scaleY: 1,
    //     angle: 0,
    //     flipX: false,
    //     flipY: false,
    //     opacity: 1,
    //     shadow: null,
    //     visible: true,
    //     clipTo: null,
    //     backgroundColor: "",
    //     fillRule: "nonzero",
    //     globalCompositeOperation: "source-over",
    //     transformMatrix: null,
    //     fontSize: 40,
    //     fontWeight: "normal",
    //     fontStyle: "",
    //     lineHeight: 1.16,
    //     textDecoration: "",
    //     textAlign: "left",
    //     textBackgroundColor: ""
    // });
    this.canvas.add(text);
    this.canvas.setActiveObject(text);
  },

  btnDoneClick: function() {
    this.canvas.isDrawingMode = false;
    this.isDrawingMode = false;
  },

  btnMosaicClick: function() {
    this.canvas.freeDrawingBrush = new fabric["SprayBrush"](this.canvas);
    var brush = this.canvas.freeDrawingBrush;
    brush.color = '#999999';
    brush.width = 20;
    brush.density = 500;
    this.canvas.isDrawingMode = true;
    this.isDrawingMode = true;
  },

  btnMosaic1Click: function() {
    var rect = new fabric.Rect({
      left: 100,
      top: 50,
      fill: 'yellow',
      width: 220,
      height: 60,
      rx: 20, // 圆角半径（x轴方向）
      ry: 20, // 圆角半径（y轴方向）
      objectCaching: false,
      stroke: 'lightgreen',
      strokeWidth: 4,
    });

    this.canvas.add(rect);
    this.canvas.setActiveObject(rect);
  },

  btnMosaic2Click: function() {
    var rect = new fabric.Rect({
      top: 100,
      left: 100,
      width: 220,
      height: 60,
      fill: 'gray',
    });
    this.canvas.add(rect);
  },

  btnMosaic3Click: function() {
    var $this = this;
    fabric.loadSVGFromURL("/sitefiles/assets/images/mosaic.svg", function(objects, options) {
      var obj = fabric.util.groupSVGElements(objects, options);
      $this.canvas.add(obj).renderAll();
    });
  },

  btnTestClick: function() {

  },

  btnDownloadClick: function() {
    this.save();
  },

  btnSubmitClick: function () {
    if (!this.isChanged) return;
    this.apiSubmit();
  },

  btnCancelClick: function () {
    utils.removeTab();
  },
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    this.apiGet();
  }
});
