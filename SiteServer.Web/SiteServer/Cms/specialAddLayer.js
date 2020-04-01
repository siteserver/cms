var $api = new apiUtils.Api(apiUrl + '/pages/cms/specialAddLayer');
var $uploadUrl = apiUrl + '/pages/cms/specialAddLayer';

function formatDate() {
  var d = new Date(),
      month = '' + (d.getMonth() + 1),
      day = '' + d.getDate(),
      year = d.getFullYear();

  if (month.length < 2) 
      month = '0' + month;
  if (day.length < 2) 
      day = '0' + day;

  return [year, month, day].join('-');
}

var data = {
  siteId: parseInt(pageUtils.getQueryString('siteId')),
  specialId: pageUtils.getQueryString('specialId'),
  isEditOnly : pageUtils.getQueryBoolean('isEditOnly'),
  isUploadOnly : pageUtils.getQueryBoolean('isUploadOnly'),
  guid: null,
  pageLoad: false,
  pageAlert: null,
  title: null,
  url: '/special/' + formatDate(),
  file: null,
  files: [],
  checkedLevel: null,
  isOverride: false,
  uploading: false
};

var methods = {
  getConfig: function () {
    var $this = this;

    $api.get({
      siteId: $this.siteId,
      specialId: $this.specialId
    }, function (err, res) {
      if (err || !res) return;

      if (res.value) {
        $this.title = res.value.title;
        $this.url = res.value.url;
      }
      $this.guid = res.guid;

      $this.pageLoad = true;
      if (!$this.isEditOnly) {
        setTimeout(function () {
          $this.loadUploader();
        }, 200);
      }
    });
  },

  loadUploader: function() {
    var $this = this;

    var E = Q.event;
    var Uploader = Q.Uploader;

    var boxDropArea = document.getElementById("drop-area");

    var uploader = new Uploader({
      url: $uploadUrl + '/actions/upload?siteId=' + this.siteId + '&guid=' + this.guid,
      target: document.getElementById("drop-area"),
      on: {
        add: function (task) {
          $this.uploading = true;
        },
        complete: function (task) {
          $this.uploading = false;
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

  getFileNames: function() {
    var arr = [];
    for (var i = 0; i < this.files.length; i++) {
      arr.push(this.files[i].fileName);
    }
    return arr;
  },

  btnSubmitClick: function () {
    var $this = this;
    this.$validator.validate().then(function (result) {
      if (result) {
        $this.submit();
      }
    });
  },

  submit: function() {
    var $this = this;
    this.pageAlert = null;

    var fileNames = this.getFileNames().join(',');
    if (!$this.isEditOnly && !fileNames) {
      return alert({
        title: "请选择专题文件！",
        type: 'warning',
        showConfirmButton: false
      });
    }

    parent.pageUtils.loading(true);
    $api.post({
        siteId: this.siteId,
        guid: this.guid,
        specialId: this.specialId,
        isEditOnly : this.isEditOnly,
        isUploadOnly : this.isUploadOnly,
        title: this.title,
        url: this.url,
        fileNames: fileNames
      },
      function (err, res) {
        parent.pageUtils.loading(false);

        if (err) {
          return $this.pageAlert = {
            type: 'danger',
            html: err.message
          };
        }

        parent.location.reload(true);
      }
    );
  },

  btnCancelClick: function() {
    pageUtils.closeLayer();
  }
};

new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.getConfig();
  }
});