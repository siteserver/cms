var $url = '/pages/settings/configAdmin';

var data = utils.initData({
  pageType: null,
  config: null,
  files: [],
  uploadUrl: null,
  adminTitle: null,
  adminLogoUrl: null,
  adminWelcomeHtml: null,
});

var methods = {
  getConfig: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url).then(function (response) {
      var res = response.data;

      $this.config = _.assign({}, res.value);

      $this.adminTitle = res.value.adminTitle;
      $this.adminLogoUrl = res.value.adminLogoUrl;
      $this.adminWelcomeHtml = res.value.adminWelcomeHtml || '欢迎使用 SiteServer CMS 管理后台';
      $this.uploadUrl = $apiUrl + '/pages/settings/configAdmin/upload?adminToken=' + res.adminToken;

      $this.pageType = 'list';
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  inputLogo: function(newFile, oldFile) {
    if (Boolean(newFile) !== Boolean(oldFile) || oldFile.error !== newFile.error) {
      if (!this.$refs.logo.active) {
        this.$refs.logo.active = true
      }
    }

    if (newFile && oldFile && newFile.xhr && newFile.success !== oldFile.success) {
      this.adminLogoUrl = newFile.response.value;
    }
  },

  getUserRegistrationAttribute: function (val) {
    return val;
  },

  submit: function (item) {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, {
      adminTitle: $this.adminTitle,
      adminLogoUrl: $this.adminLogoUrl,
      adminWelcomeHtml: $this.adminWelcomeHtml
    }).then(function (response) {
      var res = response.data;

      $this.$message.success('管理后台设置保存成功！');
      $this.config = _.assign({}, res.value);
      $this.pageType = 'list';
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },
  
  btnSubmitClick: function () {
    var $this = this;
    this.$validator.validate().then(function (result) {
      if (result) {
        $this.submit($this.item);
      }
    });
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  components: {
    FileUpload: VueUploadComponent
  },
  methods: methods,
  created: function () {
    this.getConfig();
  }
});