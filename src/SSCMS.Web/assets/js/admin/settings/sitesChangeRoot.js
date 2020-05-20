var $url = '/settings/sitesChangeRoot';

var data = utils.init({
  pageTitle: null,
  root: false,
  directories: null,
  files: null,
  siteName: null,
  
  checkAllDirectories: false,
  checkAllFiles: false,
  form: {
    siteId: utils.getQueryInt('siteId'),
    siteDir: '',
    checkedDirectories: [],
    checkedFiles: [],
    isMoveFiles: true
  }
});

var methods = {
  apiGet: function () {
    var $this = this;

    $api.get($url, {
      params: {
        siteId: this.form.siteId
      }
    }).then(function (response) {
      var res = response.data;

      $this.site = res.site;
      $this.directories = res.directories;
      $this.files = res.files;

      $this.siteName = res.site.siteName;
      $this.pageTitle = $this.site.root ? '转移到子目录' : '转移到根目录';
      $this.root = $this.site.root;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function () {
    var $this = this;
    
    utils.loading(this, true);
    $api.post($url, this.form).then(function (response) {
      var res = response.data;

      $this.$notify.success({
        title: '成功',
        message: '站点成功' + $this.pageTitle
      });

      setTimeout(function () {
        location.href = utils.getSettingsUrl('sites');
      }, 1500);

    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  handleCheckAllDirectoriesChange(val) {
    this.form.checkedDirectories = val ? this.directories : [];
    this.checkAllDirectories = val;
  },

  handleCheckedDirectoriesChange(value) {
    this.checkAllDirectories = this.form.checkedDirectories.length === this.directories.length;
  },

  handleCheckAllFilesChange(val) {
    this.form.checkedFiles = val ? this.files : [];
    this.checkAllFiles = val;
  },

  handleCheckedFilesChange(value) {
    this.checkAllFiles = this.form.checkedFiles.length === this.files.length;
  },

  btnSubmitClick: function () {
    var $this = this;

    this.$refs.form.validate(function(valid) {
      if (valid) {
        $this.apiSubmit();
      }
    });
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiGet();
  }
});