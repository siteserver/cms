var $api = new apiUtils.Api(apiUrl + '/pages/settings/userHome');

var data = {
  pageLoad: false,
  pageAlert: null,
  pageType: null,
  config: null,
  files: [],
  uploadLogoUrl: null,
  homeDirectory: null,
  isHomeClosed: null,
  homeTitle: null,
  isHomeLogo: null,
  homeLogoUrl: null,
  homeDefaultAvatarUrl: null,
  userRegistrationAttributes: [],
  isUserRegistrationGroup: null,
  isHomeAgreement: null,
  homeAgreementHtml: null,
  styles: null,
};

var methods = {
  getConfig: function () {
    var $this = this;

    $api.get(null, function (err, res) {
      if (err || !res || !res.value) return;

      $this.config = _.clone(res.value);
      $this.homeDirectory = res.homeDirectory;

      $this.isHomeClosed = res.value.isHomeClosed;
      $this.homeTitle = res.value.homeTitle;
      $this.isHomeLogo = res.value.isHomeLogo;
      $this.homeLogoUrl = res.value.homeLogoUrl;
      $this.homeDefaultAvatarUrl = res.value.homeDefaultAvatarUrl;
      if (res.value.userRegistrationAttributes) {
        $this.userRegistrationAttributes = res.value.userRegistrationAttributes.split(',');
      }
      $this.isUserRegistrationGroup = res.value.isUserRegistrationGroup;
      $this.isHomeAgreement = res.value.isHomeAgreement;
      $this.homeAgreementHtml = res.value.homeAgreementHtml;
      $this.uploadUrl = apiUrl + '/pages/settings/userHome/upload?adminToken=' + res.adminToken;
      $this.styles = res.styles;

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
      this.homeLogoUrl = newFile.response.value;
    }
  },
  inputAvatar(newFile, oldFile) {
    if (Boolean(newFile) !== Boolean(oldFile) || oldFile.error !== newFile.error) {
      if (!this.$refs.avatar.active) {
        this.$refs.avatar.active = true
      }
    }

    if (newFile && oldFile && newFile.xhr && newFile.success !== oldFile.success) {
      this.homeDefaultAvatarUrl = newFile.response.value;
    }
  },
  getUserRegistrationAttributes: function () {
    var str = '用户名, 密码';
    for (var i = 0; i < this.userRegistrationAttributes.length; i++) {
      var attributeName = this.userRegistrationAttributes[i];
      var style = _.find(this.styles, function (x) {
        return x.attributeName === attributeName
      });
      if (style) {
        str += ", " + style.displayName;
      }
    }
    return str;
  },
  getUserRegistrationAttribute: function (val) {
    return val;
  },
  submit: function (item) {
    var $this = this;

    pageUtils.loading(true);
    $api.post({
      isHomeClosed: $this.isHomeClosed,
      homeTitle: $this.homeTitle,
      isHomeLogo: $this.isHomeLogo,
      homeLogoUrl: $this.homeLogoUrl,
      homeDefaultAvatarUrl: $this.homeDefaultAvatarUrl,
      userRegistrationAttributes: $this.userRegistrationAttributes.join(','),
      isUserRegistrationGroup: $this.isUserRegistrationGroup,
      isHomeAgreement: $this.isHomeAgreement,
      homeAgreementHtml: $this.homeAgreementHtml
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
        html: '用户中心设置保存成功！'
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