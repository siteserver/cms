var $url = '/cms/templates/templatesAssets';
var $urlDelete = $url + '/actions/delete';
var $urlConfig = $url + '/actions/config';

var data = utils.init({
  siteId: utils.getQueryInt("siteId"),
  directories: null,
  allFiles: null,
  files: null,
  siteUrl: null,
  cssDir: null,
  jsDir: null,
  imagesDir: null,
  fileType: utils.getQueryString("fileType") || 'css',
  directoryPaths: [],
  keyword: '',

  configPanel: false,
  configParent: null,
  configForm: null,
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId,
        fileType: this.fileType
      }
    }).then(function (response) {
      var res = response.data;

      $this.directoryPaths = [];
      $this.keyword = '';

      $this.directories = res.directories;
      $this.allFiles = res.files;
      $this.siteUrl = res.siteUrl;
      $this.cssDir = res.cssDir;
      $this.jsDir = res.jsDir;
      $this.imagesDir = res.imagesDir;
      $this.reload();
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiDelete: function (file) {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlDelete, {
      siteId: this.siteId,
      fileType: this.fileType,
      directoryPath: file.directoryPath,
      fileName: file.fileName
    }).then(function (response) {
      var res = response.data;

      $this.allFiles.splice($this.allFiles.indexOf(file), 1);
      $this.reload();
      utils.success('文件删除成功！');
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiConfig: function () {
    var $this = this;

    this.loading = this.$loading();
    $api.post($urlConfig, this.configForm).then(function (response) {
      var res = response.data;

      $this.configPanel = false;
      utils.success('文件夹路径设置成功!');

      $this.apiGet();
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  getFileType: function(fileType) {
    if (fileType === 'css') {
      return '样式文件';
    } else if (fileType === 'js') {
      return '脚本文件';
    } else if (fileType === 'images') {
      return '图片文件';
    }
    return '';
  },

  btnNavClick: function() {
    this.apiGet();
  },

  btnDeleteClick: function (file) {
    var $this = this;

    utils.alertDelete({
      title: '删除文件',
      text: '此操作将删除文件 ' + file.directoryPath + '/' + file.fileName + '，确认吗？',
      callback: function () {
        $this.apiDelete(file);
      }
    });
  },

  btnAddClick: function(fileType) {
    utils.addTab('新增' + this.getFileType(fileType), this.getEditorUrl('', '', fileType));
  },

  btnEditClick: function(file) {
    if (this.fileType == 'images') {
      var url = this.getPageUrl(file.directoryPath + '/' + file.fileName);
      window.open(url);
      return;
    }
    utils.addTab('编辑' + ':' + file.directoryPath + '/' + file.fileName, this.getEditorUrl(file.directoryPath, file.fileName, file.fileType));
  },

  getEditorUrl: function(directoryPath, fileName, fileType) {
    return utils.getCmsUrl('templatesAssetsEditor', {
      siteId: this.siteId,
      directoryPath: directoryPath,
      fileName: fileName,
      fileType: fileType,
      tabName: utils.getTabName()
    });
  },

  reload: function() {
    var $this = this;

    this.files = _.filter(this.allFiles, function(o) {
      var isDirectoryPath = true;
      var isKeyword = true;
      if ($this.directoryPaths.length > 0) {
        isDirectoryPath = false;
        for (var i = 0; i < $this.directoryPaths.length; i++) {
          var directoryPath = $this.directoryPaths[i][$this.directoryPaths[i].length - 1];
          if (o.directoryPath == directoryPath) {
            isDirectoryPath = true;
          }
        }
      }
      if ($this.keyword) {
        isKeyword = (o.directoryPath || '').indexOf($this.keyword) !== -1 || (o.fileName || '').indexOf($this.keyword) !== -1;
      }

      return isDirectoryPath && isKeyword;
    });
  },

  getPageUrl: function(directoryPath) {
    return this.siteUrl + '/' + directoryPath;
  },

  btnConfigClick: function() {
    this.configForm = {
      siteId: this.siteId,
      fileType: this.fileType,
      cssDir: this.cssDir,
      jsDir: this.jsDir,
      imagesDir: this.imagesDir
    };
    this.configPanel = true;
  },

  btnConfigCancelClick: function() {
    this.configPanel = false;
  },

  btnConfigSubmitClick: function() {
    var $this = this;
    this.$refs.configForm.validate(function(valid) {
      if (valid) {
        $this.apiConfig();
      }
    });
  },

  getUploadUrl: function() {
    var directories = '';
    if (this.directoryPaths.length > 0) {
      var arr = this.directoryPaths[this.directoryPaths.length - 1];
      directories = arr[arr.length - 1];
    }
    return $apiUrl + $url + '/actions/upload?siteId=' + this.siteId + '&fileType=' + this.fileType + '&directories=' + encodeURIComponent(directories);
  },

  uploadBefore(file) {
    return true;
  },

  uploadProgress: function() {
    utils.loading(this, true);
  },

  uploadSuccess: function(res) {
    utils.success('文件上传成功!');
    this.apiGet();
  },

  uploadError: function(err) {
    utils.loading(this, false);
    var error = JSON.parse(err.message);
    utils.error(error.message);
  },

  btnCloseClick: function() {
    utils.removeTab();
  },
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    var $this = this;
    utils.keyPress(function() {
      if ($this.configPanel) {
        $this.btnConfigSubmitClick();
      }
    }, function() {
      if ($this.configPanel) {
        $this.btnConfigCancelClick();
      } else {
        $this.btnCloseClick();
      }
    });
    this.apiGet();
  }
});
