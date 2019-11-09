var $url = '/pages/settings/siteSave';

var data = {
  siteId: utils.getQueryInt('siteId'),
  pageLoad: false,
  pageAlert: null,
  active: 0,
  site: null,
  templateDir: null,
  directories: null,
  files: null,
  channelInfo: null,

  templateName: null,
  webSiteUrl: null,
  description: null,
  isAllFiles: true,
  checkedDirectories: [],
  checkedFiles: [],
  checkAllDirectories: false,
  checkAllFiles: false,

  isSaveContents: true,
  isSaveAllChannels: true,
  checkedChannelIds: []
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
      $this.templateName = res.value.siteName;
      $this.templateDir = res.templateDir;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
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

  btnNextClick: function () {
    var $this = this;
    this.pageAlert = null;

    if ($this.active === 0) {
      this.$validator.validate().then(function (result) {
        if (result) {
          $this.apiSaveSettings();
        }
      });
    } else if ($this.active === 1) {
      $this.apiSaveFiles();
    } else if ($this.active === 2) {
      $this.apiSaveData();
    }
  },

  apiSaveSettings: function () {
    var $this = this;
    utils.loading(true);
    $api.post($url + '/actions/settings', {
      siteId: this.siteId,
      templateName: this.templateName,
      templateDir: this.templateDir,
      webSiteUrl: this.webSiteUrl,
      description: this.description
    }).then(function (response) {
      var res = response.data;

      $this.directories = res.directories;
      $this.files = res.files;
      $this.active = 1;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  apiSaveFiles: function () {
    var $this = this;
    utils.loading(true);
    $api.post($url + '/actions/files', {
      siteId: this.siteId,
      templateDir: this.templateDir,
      isAllFiles: this.isAllFiles,
      checkedDirectories: this.checkedDirectories,
      checkedFiles: this.checkedFiles
    }).then(function (response) {
      var res = response.data;

      $this.channelInfo = res.channelInfo;
      $this.active = 2;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  apiSaveData: function () {
    var $this = this;
    utils.loading(true);
    $api.post($url + '/actions/data', {
      siteId: this.siteId,
      templateName:  this.templateName,
      templateDir: this.templateDir,
      webSiteUrl: this.webSiteUrl,
      description: this.description,
      isSaveContents: this.isSaveContents,
      isSaveAllChannels: this.isSaveAllChannels,
      checkedChannelIds: this.checkedChannelIds
    }).then(function (response) {
      var res = response.data;
      $this.active = 3;

      setTimeout(function () {
        location.href = 'siteTemplates.cshtml';
      }, 3000);
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  handleTreeChanged: function() {
    this.checkedChannelIds = this.$refs.tree.getCheckedKeys();
  },
};

new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.getConfig();
  }
});