var $url = '/settings/sitesSave';

var data = utils.init({
  active: 0,
  site: null,
  directories: null,
  files: null,
  channel: null,
  
  checkAllDirectories: false,
  checkAllFiles: false,

  form: {
    siteId: utils.getQueryInt('siteId'),
    templateName: null,
    templateDir: null,
    webSiteUrl: null,
    description: null,
    checkedDirectories: [],
    checkedFiles: [],
    isAllFiles: true,
    isSaveContents: true,
    isSaveAllChannels: true,
    checkedChannelIds: [],
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
      $this.form.templateName = res.site.siteName;
      $this.form.templateDir = res.templateDir;
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

  btnNextClick: function () {
    var $this = this;

    if ($this.active === 0) {
      this.$refs.form.validate(function(valid) {
        if (valid) {
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

    utils.loading(this, true);
    $api.post($url + '/actions/settings', this.form).then(function (response) {
      var res = response.data;

      $this.directories = res.directories;
      $this.files = res.files;
      $this.active = 1;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSaveFiles: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url + '/actions/files', this.form).then(function (response) {
      var res = response.data;

      $this.channel = res.channel;
      $this.active = 2;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSaveData: function () {
    var $this = this;
    
    utils.loading(this, true);
    $api.post($url + '/actions/data', this.form).then(function (response) {
      var res = response.data;
      $this.active = 3;

      setTimeout(function () {
        location.href = utils.getSettingsUrl('sitesTemplates');
      }, 3000);
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  handleTreeChanged: function() {
    this.form.checkedChannelIds = this.$refs.tree.getCheckedKeys();
  },
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiGet();
  }
});