var $url = '/pages/settings/userHome';

var $data = {
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
  isHomeBackground: null,
  homeBackgroundUrl: null,
  homeDefaultAvatarUrl: null,
  userRegistrationAttributes: [],
  isUserRegistrationGroup: null,
  isHomeAgreement: null,
  homeAgreementHtml: null,
  styles: null,
};

var $methods = {
  apiSubmit: function () {
    var $this = this;

    utils.loading(true);
    $api.post($url, {
      isHomeClosed: $this.isHomeClosed,
      homeTitle: $this.homeTitle,
      isHomeLogo: $this.isHomeLogo,
      homeLogoUrl: $this.homeLogoUrl,
      isHomeBackground: $this.isHomeBackground,
      homeBackgroundUrl: $this.homeBackgroundUrl,
      homeDefaultAvatarUrl: $this.homeDefaultAvatarUrl,
      userRegistrationAttributes: $this.userRegistrationAttributes.join(','),
      isUserRegistrationGroup: $this.isUserRegistrationGroup,
      isHomeAgreement: $this.isHomeAgreement,
      homeAgreementHtml: $this.homeAgreementHtml
    }).then(function (response) {
      var res = response.data;

      swal({
        toast: true,
        type: 'success',
        title: "设置保存成功！",
        showConfirmButton: false,
        timer: 1500
      }).then(function () {
        $this.config = _.clone(res.value);
        $this.pageType = 'list';
      });
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  apiGet: function () {
    var $this = this;

    $api.get($url).then(function (response) {
      var res = response.data;

      $this.config = _.clone(res.value);
      $this.homeDirectory = res.homeDirectory;

      $this.isHomeClosed = res.value.isHomeClosed;
      $this.homeTitle = res.value.homeTitle;
      $this.isHomeLogo = res.value.isHomeLogo;
      $this.homeLogoUrl = res.value.homeLogoUrl;
      $this.isHomeBackground = res.value.isHomeBackground;
      $this.homeBackgroundUrl = res.value.homeBackgroundUrl;
      $this.homeDefaultAvatarUrl = res.value.homeDefaultAvatarUrl;
      if (res.value.userRegistrationAttributes) {
        $this.userRegistrationAttributes = res.value.userRegistrationAttributes.split(',');
      }
      $this.isUserRegistrationGroup = res.value.isUserRegistrationGroup;
      $this.isHomeAgreement = res.value.isHomeAgreement;
      $this.homeAgreementHtml = res.value.homeAgreementHtml;
      $this.uploadUrl = $apiUrl + '/pages/settings/userHome/upload?adminToken=' + res.adminToken;
      $this.styles = res.styles;
    }).catch(function (error) {
      this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
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

  inputBackground(newFile, oldFile) {
    if (Boolean(newFile) !== Boolean(oldFile) || oldFile.error !== newFile.error) {
      if (!this.$refs.background.active) {
        this.$refs.background.active = true
      }
    }

    if (newFile && oldFile && newFile.xhr && newFile.success !== oldFile.success) {
      this.homeBackgroundUrl = newFile.response.value;
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

  btnSubmitClick: function () {
    var $this = this;
    this.$validator.validate().then(function (result) {
      if (result) {
        $this.apiSubmit();
      }
    });
  }
};

new Vue({
  el: '#main',
  data: $data,
  components: {
    FileUpload: VueUploadComponent
  },
  methods: $methods,
  created: function () {
    this.apiGet();
  }
});