var $url = '/pages/plugins/addLayerUpload';
var $urlUpload = apiUrl + '/pages/plugins/addLayerUpload/actions/upload';

var data = {
  pageLoad: false,
  pageAlert: null,
  importType: 'nupkg',
  file: null,
  files: []
};

var methods = {
  loadConfig: function () {
    var $this = this;
    $this.pageLoad = true;

    $api.get($url).then(function (response) {
      setTimeout(function () {
        $this.loadUploader();
      }, 100);
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
    });
  },

  loadUploader: function () {
    var $this = this;

    var E = Q.event,
      Uploader = Q.Uploader;

    var boxDropArea = document.getElementById("drop-area");

    var uploader = new Uploader({
      url: $urlUpload,
      target: document.getElementById("drop-area"),
      allows: ".nupkg",
      on: {
        add: function (task) {
          if (task.ext != '.' + $this.importType) {
            alert({
              title: '文件错误！',
              text: '离线插件文件格式为：.' + $this.importType,
              type: 'error',
              showConfirmButton: false
            });
            return false;
          }
        },
        complete: function (task) {
          var json = task.json;
          if (!json || json.ret != 1) {
            return alert({
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
      return alert({
        title: "请上传插件安装包文件！",
        type: 'warning',
        showConfirmButton: false
      });
    }

    parent.pageUtils.loading(true);
    $api.post({
        siteId: $this.siteId,
        channelId: $this.channelId,
        importType: $this.importType,
        fileNames: $this.getFileNames(),
        checkedLevel: $this.checkedLevel,
        isOverride: $this.isOverride
      },
      function (err, res) {
        parent.pageUtils.loading(false);

        if (err) {
          return $this.pageAlert = {
            type: 'danger',
            html: res.message
          };
        }

        parent.location.reload(true);
      }
    );
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