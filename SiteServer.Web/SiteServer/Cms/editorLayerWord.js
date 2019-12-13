var $url = '/pages/cms/editorLayerWord';
var $urlUpload = apiUrl + '/pages/cms/editorLayerWord/actions/upload?siteId=' + utils.getQueryInt('siteId') + '&channelId=' + utils.getQueryInt('channelId');

var data = {
  siteId: utils.getQueryInt('siteId'),
  channelId: utils.getQueryInt('channelId'),
  pageLoad: false,
  pageAlert: null,
  file: null,
  files: [],
  isClearFormat: true,
  isFirstLineIndent: true,
  isClearFontSize: true,
  isClearFontFamily: true,
  isClearImages: false,
  checkedLevels: null,
  checkedLevel: null,

  uploadList: [],
};

var methods = {
  loadConfig: function () {
    var $this = this;
    $this.pageLoad = true;

    $api
      .get($url, {
        params: {
          siteId: $this.siteId,
          channelId: $this.channelId
        }
      })
      .then(function(response) {
        var res = response.data;

        $this.checkedLevels = res.value;
        $this.checkedLevel = res.checkedLevel;

        // $this.loadUploader();
      })
      .catch(function(error) {
        $this.pageAlert = utils.getPageAlert(error);
      })
      .then(function() {
        $this.pageLoad = true;
      });
  },

  uploadRemove(file, fileList) {
    console.log(file.response.name);
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

    utils.loading(true);
    $api.post($url, {
      siteId: $this.siteId,
      channelId: $this.channelId,
      isFirstLineIndent: $this.isFirstLineIndent,
      isClearFontSize: $this.isClearFontSize,
      isClearFontFamily: $this.isClearFontFamily,
      isClearImages: $this.isClearImages,
      checkedLevel: $this.checkedLevel,
      fileNames: fileNames
    }).then(function(response) {
      var res = response.data;

      var contentIdList = res.value;
      if (contentIdList.length === 1) {
        parent.location.href = 'pageContentAdd.aspx?siteId=' + $this.siteId + '&channelId=' + $this.channelId + '&id=' + contentIdList[0];
      } else {
        parent.location.reload(true);
      }
    })
    .catch(function(error) {
      $this.pageAlert = utils.getPageAlert(error);
    })
    .then(function() {
      utils.loading(false);
    });
  },

  uploadBefore(file) {
    var isWord = file.name.indexOf('.doc', file.name.length - '.docx'.length) !== -1;
    if (!isWord) {
      this.$message.error('上传的文件只能是 Word 格式!');
    }
    return isWord;
  },

  uploadProgress: function() {
    utils.loading(true)
  },

  uploadSuccess: function(res, file) {
    console.log(this.uploadList);
    utils.loading(false);
    var success = res.success;
    var failure = res.failure;
    var errorMessage = res.errorMessage;

    var $this = this;

    // $api.get($url, {
    //   params: this.formInline
    // }).then(function (response) {
    //   var res = response.data;

    //   $this.items = res.value;
    //   $this.count = res.count;
    //   $this.roles = res.roles;
    //   $this.isSuperAdmin = res.isSuperAdmin;
    //   $this.adminId = res.adminId;
    // }).catch(function (error) {
    //   $this.pageAlert = utils.getPageAlert(error);
    // }).then(function () {
    //   if (success) {
    //     $this.$message.success('成功导入 ' + success + ' 名管理员！');
    //   }
    //   if (errorMessage) {
    //     $this.$message.error(failure + ' 名管理员导入失败：' + errorMessage);
    //   }
    //   utils.loading(false);
    // });
  },

  uploadError: function(err) {
    utils.loading(false);
    var error = JSON.parse(err.message);
    this.$message.error(error.message);
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