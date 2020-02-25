var $url = '/admin/cms/templates/templatesAssets';

var data = utils.initData({
  siteId: utils.getQueryInt("siteId"),
  directories: null,
  allFiles: null,
  files: null,
  siteUrl: null,
  includeDir: null,
  cssDir: null,
  jsDir: null,
  fileType: utils.getQueryString("fileType") || 'All',
  directoryPaths: [],
  keyword: '',

  configPanel: false,
  configParent: null,
  configForm: null,
});

var methods = {
  apiList: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId
      }
    }).then(function (response) {
      var res = response.data;

      $this.directories = res.directories;
      $this.allFiles = res.files;
      $this.siteUrl = res.siteUrl;
      $this.includeDir = res.includeDir;
      $this.cssDir = res.cssDir;
      $this.jsDir = res.jsDir;
      $this.reload();
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiDelete: function (file) {
    var $this = this;

    utils.loading(this, true);
    $api.delete($url, {
      data: {
        siteId: this.siteId,
        directoryPath: file.key,
        fileName: file.value
      }
    }).then(function (response) {
      var res = response.data;

      $this.allFiles.splice($this.allFiles.indexOf(file), 1);
      $this.reload();
      $this.$message({
        type: 'success',
        message: '文件删除成功！'
      });
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiConfig: function () {
    var $this = this;

    this.loading = this.$loading();
    $api.post($url + '/actions/config', this.configForm).then(function (response) {
      var res = response.data;

      $this.directories = res.directories;
      $this.allFiles = res.files;
      $this.siteUrl = res.siteUrl;
      $this.includeDir = res.includeDir;
      $this.cssDir = res.cssDir;
      $this.jsDir = res.jsDir;
      $this.reload();

      $this.configPanel = false;
      $this.$message({
        type: 'success',
        message: '文件夹路径设置成功!'
      });
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnDefaultClick: function (template) {
    var $this = this;

    utils.alertWarning({
      title: '设置默认文件',
      text: '此操作将把文件 ' + template.templateName + ' 设为默认' + this.getTemplateType(template.fileType) + '，确认吗？',
      callback: function () {
        $this.apiDefault(template);
      }
    });
  },

  btnCopyClick: function(template) {
    var $this = this;

    utils.loading(this, true);
    $api.post($url + '/actions/copy', {
      siteId: this.siteId,
      templateId: template.id
    }).then(function (response) {
      var res = response.data;

      $this.directories = res.directories;
      $this.allFiles = res.files;
      $this.reload();
      $this.$message({
        type: 'success',
        message: '快速复制成功！'
      });
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnDeleteClick: function (file) {
    var $this = this;

    utils.alertDelete({
      title: '删除文件',
      text: '此操作将删除文件 ' + file.key + '/' + file.value + '，确认吗？',
      callback: function () {
        $this.apiDelete(file);
      }
    });
  },

  btnAddClick: function(fileType) {
    location.href = this.getEditorUrl('', '', fileType);
  },

  getEditorUrl: function(directoryPath, fileName, extName) {
    return 'templateAssetsEditor.cshtml?siteId=' + this.siteId + '&directoryPath=' + encodeURIComponent(directoryPath) + '&fileName=' + encodeURIComponent(fileName) + '&extName=' + encodeURIComponent(extName);
  },

  getPageUrl: function(virtualPath) {
    return this.siteUrl + '/' + virtualPath;
  },

  reload: function() {
    var $this = this;

    this.files = _.filter(this.allFiles, function(o) {
      var isFileType = true;
      var isDirectoryPath = true;
      var isKeyword = true;
      if ($this.fileType != 'All') {
        isFileType = _.endsWith(o.value, $this.fileType);
      }
      if ($this.directoryPaths.length > 0) {
        isDirectoryPath = false;
        for (var i = 0; i < $this.directoryPaths.length; i++) {
          var directoryPath = $this.directoryPaths[i][$this.directoryPaths[i].length - 1];
          if (o.key == directoryPath) {
            isDirectoryPath = true;
          }
        }
      }
      if ($this.keyword) {
        isKeyword = (o.key || '').indexOf($this.keyword) !== -1 || (o.value || '').indexOf($this.keyword) !== -1;
      }
      
      return isFileType && isDirectoryPath && isKeyword;
    });
  },

  btnConfigClick: function() {
    this.configForm = {
      siteId: this.siteId,
      includeDir: this.includeDir,
      cssDir: this.cssDir,
      jsDir: this.jsDir
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
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiList();
  }
});