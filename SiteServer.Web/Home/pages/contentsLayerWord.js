var $api = new utils.Api('/home/contentsLayerWord');
var $uploadUrl = utils.getApiUrl('/home/contentsLayerWord');

var data = {
  siteId: parseInt(utils.getQueryString('siteId')),
  channelId: parseInt(utils.getQueryString('channelId')),
  returnUrl: utils.getQueryString('returnUrl'),
  pageLoad: false,
  pageAlert: null,
  file: null,
  files: [],
  isFirstLineTitle: false,
  isFirstLineRemove: true,
  isClearFormat: true,
  isFirstLineIndent: true,
  isClearFontSize: true,
  isClearFontFamily: true,
  isClearImages: false,
  checkedLevels: null,
  checkedLevel: null
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

      $this.checkedLevels = res.value;
      $this.checkedLevel = res.checkedLevel;

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
      allows: ".doc,.docx",
      on: {
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
          if (!json || json.ret != 1) {
            return alert({
              title: "Word 文件上传失败！",
              type: 'error',
              showConfirmButton: false
            });
          }

          if (json && json.fileName) {
            $this.files.push(json);
          }
        }
      }
    });

    //若浏览器不支持html5上传，则禁止拖拽上传
    if (!Uploader.support.html5 || !uploader.html5) {
      boxDropArea.innerHTML = "点击批量上传Word文件";
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
  getFileNames: function () {
    var arr = [];
    for (var i = 0; i < this.files.length; i++) {
      arr.push(this.files[i].fileName);
    }
    return arr;
  },
  btnSubmitClick: function () {
    var $this = this;
    var fileNames = this.getFileNames().join(',');
    if (!fileNames) {
      return alert({
        title: "请选择需要导入的Word文件！",
        type: 'warning',
        showConfirmButton: false
      });
    }

    parent.utils.loading(true);
    $api.post({
      siteId: $this.siteId,
      channelId: $this.channelId,
      isFirstLineTitle: $this.isFirstLineTitle,
      isFirstLineRemove: $this.isFirstLineRemove,
      isClearFormat: $this.isClearFormat,
      isFirstLineIndent: $this.isFirstLineIndent,
      isClearFontSize: $this.isClearFontSize,
      isClearFontFamily: $this.isClearFontFamily,
      isClearImages: $this.isClearImages,
      checkedLevel: $this.checkedLevel,
      fileNames: fileNames
    }, function (err, res) {
      if (err || !res || !res.value) return;

      parent.layer.closeAll();

      var contentIdList = res.value;
      if (contentIdList.length === 1) {
        parent.location.hash = 'pages/contentAdd.html?siteId=' + $this.siteId + '&channelId=' + $this.channelId + '&contentId=' + contentIdList[0] + '&returnUrl=' + encodeURIComponent($this.returnUrl);
      } else {
        parent.location.reload(true);
      }
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