var $url = '/settings/homeConfig';

var data = utils.init({
  config: null,
  uploadUrl: null,
  uploadFileListHomeLogoUrl: [],
  uploadFileListHomeDefaultAvatarUrl: [],
  uploadType: null,

  homeDirectory: null,

  form: {
    uploadLogoUrl: null,
    
    isHomeClosed: null,
    homeTitle: null,
    isHomeLogo: null,
    homeLogoUrl: null,
    homeDefaultAvatarUrl: null,
    userRegistrationAttributes: [],
    isUserRegistrationMobile: null,
    isUserRegistrationEmail: null,
    isUserRegistrationGroup: null,
    isHomeAgreement: null,
    homeAgreementHtml: null,
    homeWelcomeHtml: null,
    styles: null
  }
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url).then(function (response) {
      var res = response.data;

      $this.config = res.config;

      $this.homeDirectory = res.homeDirectory;
      $this.form.isHomeClosed = res.config.isHomeClosed;
      $this.form.homeTitle = res.config.homeTitle;
      $this.form.isHomeLogo = res.config.isHomeLogo;
      $this.form.homeLogoUrl = res.config.homeLogoUrl;
      $this.form.homeDefaultAvatarUrl = res.config.homeDefaultAvatarUrl;
      $this.form.userRegistrationAttributes = res.config.userRegistrationAttributes || [];
      $this.form.isUserRegistrationMobile = res.config.isUserRegistrationMobile;
      $this.form.isUserRegistrationEmail = res.config.isUserRegistrationEmail;
      $this.form.isUserRegistrationGroup = res.config.isUserRegistrationGroup;
      $this.form.isHomeAgreement = res.config.isHomeAgreement;
      $this.form.homeAgreementHtml = res.config.homeAgreementHtml;
      $this.form.homeWelcomeHtml = res.homeWelcomeHtml || '欢迎使用用户中心';
      $this.styles = res.styles;

      if ($this.form.homeLogoUrl) {
        $this.uploadFileListHomeLogoUrl.push({name: 'avatar', url: $this.form.homeLogoUrl});
      }
      if ($this.form.homeDefaultAvatarUrl) {
        $this.uploadFileListHomeDefaultAvatarUrl.push({name: 'avatar', url: $this.form.homeDefaultAvatarUrl});
      }
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, {
      isHomeClosed: this.form.isHomeClosed,
      homeTitle: this.form.homeTitle,
      isHomeLogo: this.form.isHomeLogo,
      homeLogoUrl: this.form.homeLogoUrl,
      homeDefaultAvatarUrl: this.form.homeDefaultAvatarUrl,
      userRegistrationAttributes: this.form.userRegistrationAttributes,
      isUserRegistrationMobile: this.form.isUserRegistrationMobile,
      isUserRegistrationEmail: this.form.isUserRegistrationEmail,
      isUserRegistrationGroup: this.form.isUserRegistrationGroup,
      isHomeAgreement: this.form.isHomeAgreement,
      homeAgreementHtml: this.form.homeAgreementHtml,
      homeWelcomeHtml: this.form.homeWelcomeHtml
    }).then(function (response) {
      var res = response.data;

      utils.success('用户中心设置保存成功！');
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  getUserRegistrationAttribute: function (val) {
    return val;
  },

  btnUsersStyleClick: function () {
    utils.addTab('用户字段', utils.getSettingsUrl('usersStyle'));
  },
  
  btnSubmitClick: function () {
    var $this = this;

    this.$refs.form.validate(function(valid) {
      if (valid) {
        $this.apiSubmit();
      }
    });
  },

  uploadBeforeHomeLogoUrl(file) {
    this.uploadType = 'homeLogoUrl';
    return this.uploadBefore(file);
  },

  uploadBeforeHomeDefaultAvatarUrl(file) {
    this.uploadType = 'homeDefaultAvatarUrl';
    return this.uploadBefore(file);
  },

  uploadBefore(file) {
    var re = /(\.jpg|\.jpeg|\.bmp|\.gif|\.png|\.webp)$/i;
    if(!re.exec(file.name))
    {
      utils.error('管理后台Logo只能是图片格式，请选择有效的文件上传!');
      return false;
    }

    var isLt10M = file.size / 1024 / 1024 < 10;
    if (!isLt10M) {
      utils.error('管理后台Logo图片大小不能超过 10MB!');
      return false;
    }
    return true;
  },

  uploadProgress: function() {
    utils.loading(this, true);
  },

  uploadSuccess: function(res, file, fileList) {
    if (this.uploadType === 'homeLogoUrl') {
      this.form.homeLogoUrl = res.value;
    } else if (this.uploadType === 'homeDefaultAvatarUrl') {
      this.form.homeDefaultAvatarUrl = res.value;
    }
    
    utils.loading(this, false);
    if (fileList.length > 1) fileList.splice(0, 1);
  },

  uploadError: function(err) {
    utils.loading(this, false);
    var error = JSON.parse(err.message);
    utils.error(error.message);
  },

  uploadRemoveHomeLogoUrl(file) {
    this.form.homeLogoUrl = null;
  },

  uploadRemoveHomeDefaultAvatarUrl(file) {
    this.form.homeDefaultAvatarUrl = null;
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.uploadUrl = $apiUrl + $url + '/actions/upload';
    this.apiGet();
  }
});