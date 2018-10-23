var $api = new utils.Api('/home/contentAddLayerImage');
var $uploadUrl = utils.getApiUrl('/home/contentAddLayerImage');

var data = {
  siteId: parseInt(utils.getQueryString('siteId')),
  channelId: parseInt(utils.getQueryString('channelId')),
  attributeName: utils.getQueryString('attributeName'),
  inputType: utils.getQueryString('inputType'),
  pageLoad: false,
  pageAlert: null,
  file: null,
  files: [],
  isFix: true,
  fixWidth: '300',
  fixHeight: '',
  isEditor: true,
  editorIsFix: true,
  editorFixWidth: '500',
  editorFixHeight: '',
  editorIsLinkToOriginal: false
};

var methods = {
  loadConfig: function () {
    var $this = this;
    $this.pageLoad = true;

    $api.get({
      siteId: $this.siteId,
      channelId: $this.channelId
    }, function (err, res) {
      if (err || !res || !res.value) return;

      $this.isFix = res.value.configImageIsFix;
      $this.fixWidth = res.value.configImageFixWidth;
      $this.fixHeight = res.value.configImageFixHeight;
      $this.isEditor = res.value.configImageIsEditor;
      $this.editorIsFix = res.value.configImageEditorIsFix;
      $this.editorFixWidth = res.value.configImageEditorFixWidth;
      $this.editorFixHeight = res.value.configImageEditorFixHeight;
      $this.editorIsLinkToOriginal = res.value.configImageEditorIsLinkToOriginal;

      $this.loadUploader();
    });
  },
  loadUploader: function () {
    var $this = this;

    var E = Q.event,
      Uploader = Q.Uploader;

    var boxDropArea = document.getElementById("drop-area");

    var uploader = new Uploader({
      url: $uploadUrl + '/actions/upload?siteId=' + $this.siteId + '&channelId=' + $this.channelId + '&userToken=' + utils.getToken(),
      target: document.getElementById("drop-area"),
      allows: ".gif,.jpg,.jpeg,.bmp,.png,.pneg,.webp",
      multiple: $this.inputType === 'TextEditor',
      on: {
        select: function () {
          if ($this.inputType !== 'TextEditor' && $this.files.length > 0) return false;
          return true;
        },
        add: function (task) {
          if (task.disabled) {
            return alert({
              title: '文件错误！',
              text: '允许上传的文件格式为：' + this.ops.allows,
              type: 'error',
              showConfirmButton: false
            });
          }
        },
        complete: function (task) {
          var json = task.json;
          if (!json || !json.path || !json.url) {
            return alert({
              title: "图片传失败！",
              type: 'error',
              showConfirmButton: false
            });
          } else {
            $this.files.push(json);
          }
        }
      }
    });

    //若浏览器不支持html5上传，则禁止拖拽上传
    if (!Uploader.support.html5 || !uploader.html5) {
      boxDropArea.innerHTML = "点击批量上传图片";
      return;
    }

    //阻止浏览器默认拖放行为
    E.add(boxDropArea, "dragleave", E.stop);
    E.add(boxDropArea, "dragenter", E.stop);
    E.add(boxDropArea, "dragover", E.stop);

    E.add(boxDropArea, "drop", function (e) {
      E.stop(e);

      //获取文件对象
      var files = e.dataTransfer.files;

      uploader.addList(files);
    });
  },

  del: function (file) {
    this.files.splice(this.files.indexOf(file), 1);
  },

  getFilePaths: function () {
    var arr = [];
    for (var i = 0; i < this.files.length; i++) {
      arr.push(this.files[i].path);
    }
    return arr;
  },

  btnSubmitClick: function () {
    var $this = this;
    var filePaths = this.getFilePaths().join(',');
    if (!filePaths) {
      return alert({
        title: "请选择需要上传的图片！",
        type: 'warning',
        showConfirmButton: false
      });
    }

    top.utils.loading(true);
    $api.post({
      siteId: $this.siteId,
      channelId: $this.channelId,
      isFix: $this.isFix,
      fixWidth: $this.fixWidth,
      fixHeight: $this.fixHeight,
      isEditor: $this.isEditor,
      editorIsFix: $this.editorIsFix,
      editorFixWidth: $this.editorFixWidth,
      editorFixHeight: $this.editorFixHeight,
      editorIsLinkToOriginal: $this.editorIsLinkToOriginal,
      filePaths: filePaths
    }, function (err, res) {
      top.utils.loading(false);
      if (err || !res || !res.value) return;

      var win = top.getContentWindow();
      var editorAttributeName = $this.attributeName;

      if ($this.inputType != 'TextEditor') {
        editorAttributeName = 'Content';
        for (var i = 0; i < res.value.length; i++) {
          var val = res.value[i];
          win.setValue($this.attributeName, val);
        }
      }

      var ue = win.UE.getEditor(editorAttributeName);
      if (ue && typeof ue.execCommand === "function") {
        for (var i = 0; i < res.editors.length; i++) {
          var editor = res.editors[i];
          var html = '<img src="' + editor.imageUrl + '" border="0" />';
          if ($this.editorIsFix && $this.editorIsLinkToOriginal) {
            html = '<a href="' + editor.originalUrl + '" target="_blank">' + html + '</a>';
          }
          ue.execCommand("insertHTML", html);
        }
      }

      top.layer.closeAll();
    });
  }
};

new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.loadConfig();
  }
});