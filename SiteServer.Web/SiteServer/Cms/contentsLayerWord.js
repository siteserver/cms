var $api = new apiUtils.Api(apiUrl + '/pages/cms/contentsLayerWord');
var $uploadUrl = apiUrl + '/pages/cms/contentsLayerWord';

var data = {
  siteId: parseInt(pageUtils.getQueryStringByName('siteId')),
  channelId: parseInt(pageUtils.getQueryStringByName('channelId')),
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
      url: $uploadUrl + '/actions/upload?siteId=' + $this.siteId + '&channelId=' + $this.channelId,
      target: document.getElementById("drop-area"),
      allows: ".doc,.docx",
      on: {
        add: function (task) {
          if (task.disabled) {
            return alert({
              title: "允许上传的文件格式为：" + this.ops.allows,
              type: 'warning',
              confirmButtonText: '关 闭'
            });
          }
        },
        complete: function (task) {
          var json = task.json;
          if (!json || json.ret != 1) {
            return alert({
              title: "上传失败！",
              type: 'warning',
              confirmButtonText: '关 闭'
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
        showConfirmButton: '关 闭'
      });
    }

    pageUtils.loading(true);
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

      var contentIdList = res.value;
      if (contentIdList.length === 1) {
        parent.location.href = 'pageContentAdd.aspx?siteId=' + $this.siteId + '&channelId=' + $this.channelId + '&id=' + contentIdList[0];
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