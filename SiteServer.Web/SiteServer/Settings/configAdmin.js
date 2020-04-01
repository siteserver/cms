var $api = new apiUtils.Api(apiUrl + '/pages/settings/configAdmin');

var data = {
  pageLoad: false,
  pageAlert: null,
  pageType: null,
  config: null,
  files: [],
  uploadUrl: null,
  adminTitle: null,
  adminLogoUrl: null,
  adminWelcomeHtml: null,
};

var methods = {
  getConfig: function () {
    var $this = this;

    $api.get(null, function (err, res) {
      if (err || !res || !res.value) return;

      $this.config = _.clone(res.value);

      $this.adminTitle = res.value.adminTitle;
      $this.adminLogoUrl = res.value.adminLogoUrl;
      $this.adminWelcomeHtml = res.value.adminWelcomeHtml || '欢迎使用 SiteServer CMS 管理后台';
      $this.uploadUrl = apiUrl + '/pages/settings/configAdmin/upload?adminToken=' + res.adminToken;

      $this.pageType = 'list';
      $this.pageLoad = true;
    });
  },
  inputLogo(newFile, oldFile) {
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

    pageUtils.loading(true);
    $api.post({
      adminTitle: $this.adminTitle,
      adminLogoUrl: $this.adminLogoUrl,
      adminWelcomeHtml: $this.adminWelcomeHtml
    }, function (err, res) {
      pageUtils.loading(false);
      if (err) {
        $this.pageAlert = {
          type: 'danger',
          html: err.message
        };
        return;
      }

      $this.pageAlert = {
        type: 'success',
        html: '管理后台设置保存成功！'
      };
      $this.config = _.clone(res.value);
      $this.pageType = 'list';
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

new Vue({
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