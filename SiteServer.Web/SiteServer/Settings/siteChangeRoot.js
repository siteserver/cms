var $url = '/pages/settings/siteChangeRoot';

var data = {
  siteId: utils.getQueryInt('siteId'),
  pageLoad: false,
  pageAlert: null,
  pageTitle: null,
  root: false,
  directories: null,
  files: null,
  siteName: null,
  siteDir: '',
  checkedDirectories: [],
  checkedFiles: [],
  checkAllDirectories: false,
  checkAllFiles: false,
  isMoveFiles: true
};

var methods = {
  getConfig: function () {
    var $this = this;

    $api.get($url, {
      params: {
        siteId: this.siteId
      }
    }).then(function (response) {
      var res = response.data;

      $this.site = res.value;
      $this.directories = res.directories;
      $this.files = res.files;

      $this.siteName = res.value.siteName;
      $this.pageTitle = $this.site.root ? '转移到子目录' : '转移到根目录';
      $this.root = $this.site.root;
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      $this.pageLoad = true;
    });
  },

  handleCheckAllDirectoriesChange(val) {
    this.checkedDirectories = val ? this.directories : [];
    this.checkAllDirectories = val;
  },

  handleCheckedDirectoriesChange(value) {
    this.checkAllDirectories = this.checkedDirectories.length === this.directories.length;
  },

  handleCheckAllFilesChange(val) {
    this.checkedFiles = val ? this.files : [];
    this.checkAllFiles = val;
  },

  handleCheckedFilesChange(value) {
    this.checkAllFiles = this.checkedFiles.length === this.files.length;
  },

  btnCancelClick: function () {
    location.href = 'site.cshtml';
  },

  btnSubmitClick: function () {
    var $this = this;
    this.pageAlert = null;

    this.$validator.validate().then(function (result) {
      if (result) {
        $this.apiSubmit();
      }
    });
  },

  apiSubmit: function () {
    var $this = this;
    utils.loading($this, true);
    $api.post($url, {
      siteId: this.siteId,
      siteDir: this.siteDir,
      checkedDirectories: this.checkedDirectories,
      checkedFiles: this.checkedFiles,
      isMoveFiles: this.isMoveFiles
    }).then(function (response) {
      var res = response.data;

      $this.$notify.success({
        title: '成功',
        message: '站点成功' + $this.pageTitle
      });

      setTimeout(function () {
        location.href = 'site.cshtml';
      }, 1500);

    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
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