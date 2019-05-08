var $url = '/pages/cms/contentsLayerImport';
var $uploadUrl = $apiUrl + '/pages/cms/contentsLayerImport';

var $data = {
  siteId: parseInt(utils.getQueryString('siteId')),
  channelId: parseInt(utils.getQueryString('channelId')),
  pageLoad: false,
  pageAlert: null,
  checkedLevels: null,
  importType: 'zip',
  file: null,
  files: [],
  checkedLevel: null,
  isOverride: false
};

var $methods = {
  loadConfig: function () {
    var $this = this;

    $api.get($url, {
      params: {
        siteId: $this.siteId,
        channelId: $this.channelId
      }
    }).then(function (response) {
      var res = response.data;

      $this.checkedLevels = res.checkedLevels;
      $this.checkedLevel = res.value;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
      setTimeout($this.loadUploader, 100);
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
      allows: ".zip,.csv,.txt",
      on: {
        add: function (task) {
          if (task.ext != '.' + $this.importType) {
            swal2({
              title: '文件错误！',
              text: '允许上传的文件格式为：.' + $this.importType,
              type: 'error',
              showConfirmButton: false
            });
            return false;
          }
        },
        complete: function (task) {
          var json = task.json;
          if (!json || json.ret != 1) {
            return swal2({
              title: "文件上传失败！",
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
      boxDropArea.innerHTML = "点击批量上传文件";
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
    this.pageAlert = null;

    var fileNames = this.getFileNames().join(',');
    if (!fileNames) {
      return swal2({
        title: "请选择需要导入的文件！",
        type: 'warning',
        showConfirmButton: false
      });
    }

    utils.loading(true);
    $api.post($url, {
      siteId: $this.siteId,
      channelId: $this.channelId,
      importType: $this.importType,
      fileNames: $this.getFileNames(),
      checkedLevel: $this.checkedLevel,
      isOverride: $this.isOverride
    }).then(function (response) {
      var res = response.data;

      parent.location.reload(true);
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  }
};

new Vue({
  el: '#main',
  data: $data,
  methods: $methods,
  created: function () {
    this.loadConfig();
  }
});