var $url = '/pages/settings/configHome';

var data = utils.initData({
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
});

var methods = {
  getConfig: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url).then(function (response) {
      var res = response.data;

      $this.config = _.assign({}, res.value);
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
      $this.uploadUrl = apiUrl + '/pages/settings/configHome/upload?adminToken=' + res.adminToken;
      $this.styles = res.styles;

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
      this.homeLogoUrl = newFile.response.value;
    }
  },

  inputAvatar: function(newFile, oldFile) {
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

    utils.loading(this, true);
    $api.post($url, {
      isHomeClosed: this.isHomeClosed,
      homeTitle: this.homeTitle,
      isHomeLogo: this.isHomeLogo,
      homeLogoUrl: this.homeLogoUrl,
      homeDefaultAvatarUrl: this.homeDefaultAvatarUrl,
      userRegistrationAttributes: this.userRegistrationAttributes.join(','),
      isUserRegistrationGroup: this.isUserRegistrationGroup,
      isHomeAgreement: this.isHomeAgreement,
      homeAgreementHtml: this.homeAgreementHtml
    }).then(function (response) {
      var res = response.data;

      $this.config = _.assign({}, res.value);
      $this.pageType = 'list';
      $this.$message.success('用户中心设置保存成功！');
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