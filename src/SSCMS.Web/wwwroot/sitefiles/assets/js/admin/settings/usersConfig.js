var $url = '/settings/usersConfig';

var data = utils.init({
  isSmsEnabled: false,
  form: {
    isUserRegistrationAllowed: null,
    isUserRegistrationChecked: null,
    isUserUnRegistrationAllowed: null,
    isUserForceVerifyMobile: null,
    userPasswordMinLength: null,
    userPasswordRestriction: null,
    userRegistrationMinMinutes: null,
    isUserLockLogin: null,
    userLockLoginCount: null,
    userLockLoginType: null,
    userLockLoginHours: null,
    userFindPasswordSmsTplId: null
  }
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url).then(function (response) {
      var res = response.data;

      $this.isSmsEnabled = res.isSmsEnabled;
      $this.form.isUserRegistrationAllowed = res.config.isUserRegistrationAllowed;
      $this.form.isUserRegistrationChecked = res.config.isUserRegistrationChecked;
      $this.form.isUserUnRegistrationAllowed = res.config.isUserUnRegistrationAllowed;
      $this.form.isUserForceVerifyMobile = res.config.isUserForceVerifyMobile && res.isSmsEnabled;
      $this.form.userPasswordMinLength = res.config.userPasswordMinLength;
      $this.form.userPasswordRestriction = res.config.userPasswordRestriction;
      $this.form.userRegistrationMinMinutes = res.config.userRegistrationMinMinutes;
      $this.form.isUserLockLogin = res.config.isUserLockLogin;
      $this.form.userLockLoginCount = res.config.userLockLoginCount;
      $this.form.userLockLoginType = res.config.userLockLoginType;
      $this.form.userLockLoginHours = res.config.userLockLoginHours;
      $this.form.userFindPasswordSmsTplId = res.config.userFindPasswordSmsTplId;
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
      isUserRegistrationAllowed: this.form.isUserRegistrationAllowed,
      isUserRegistrationChecked: this.form.isUserRegistrationChecked,
      isUserUnRegistrationAllowed: this.form.isUserUnRegistrationAllowed,
      isUserForceVerifyMobile: this.form.isUserForceVerifyMobile,
      userPasswordMinLength: this.form.userPasswordMinLength,
      userPasswordRestriction: this.form.userPasswordRestriction,
      userRegistrationMinMinutes: this.form.userRegistrationMinMinutes,
      isUserLockLogin: this.form.isUserLockLogin,
      userLockLoginCount: this.form.userLockLoginCount,
      userLockLoginType: this.form.userLockLoginType,
      userLockLoginHours: this.form.userLockLoginHours,
      userFindPasswordSmsTplId: this.form.userFindPasswordSmsTplId,
    }).then(function (response) {
      var res = response.data;

      utils.success('用户设置保存成功！');
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  getPasswordRestrictionText: function (val) {
    if (val === 'LetterAndDigit') return '字母和数字组合';
    else if (val === 'LetterAndDigitAndSymbol') return '字母、数字以及符号组合';
    else return '不限制';
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