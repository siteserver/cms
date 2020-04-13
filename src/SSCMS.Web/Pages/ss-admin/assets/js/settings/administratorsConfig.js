var $url = '/admin/settings/administratorsConfig';

var data = utils.initData({
  form: {
    adminUserNameMinLength: null,
    adminPasswordMinLength: null,
    adminPasswordRestriction: null,
    isAdminLockLogin: null,
    adminLockLoginCount: null,
    adminLockLoginType: null,
    adminLockLoginHours: null,
    isAdminEnforcePasswordChange: null,
    adminEnforcePasswordChangeDays: null,
    isAdminEnforceLogout: null,
    adminEnforceLogoutMinutes: null,
  }
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url).then(function (response) {
      var res = response.data;

      $this.form.adminUserNameMinLength = res.config.adminUserNameMinLength;
      $this.form.adminPasswordMinLength = res.config.adminPasswordMinLength;
      $this.form.adminPasswordRestriction = res.config.adminPasswordRestriction;
      $this.form.isAdminLockLogin = res.config.isAdminLockLogin;
      $this.form.adminLockLoginCount = res.config.adminLockLoginCount;
      $this.form.adminLockLoginType = res.config.adminLockLoginType;
      $this.form.adminLockLoginHours = res.config.adminLockLoginHours;
      $this.form.isAdminEnforcePasswordChange = res.config.isAdminEnforcePasswordChange;
      $this.form.adminEnforcePasswordChangeDays = res.config.adminEnforcePasswordChangeDays;
      $this.form.isAdminEnforceLogout = res.config.isAdminEnforceLogout;
      $this.form.adminEnforceLogoutMinutes = res.config.adminEnforceLogoutMinutes;
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, {
      adminUserNameMinLength: this.form.adminUserNameMinLength,
      adminPasswordMinLength: this.form.adminPasswordMinLength,
      adminPasswordRestriction: this.form.adminPasswordRestriction,
      isAdminLockLogin: this.form.isAdminLockLogin,
      adminLockLoginCount: this.form.adminLockLoginCount,
      adminLockLoginType: this.form.adminLockLoginType,
      adminLockLoginHours: this.form.adminLockLoginHours,
      isAdminEnforcePasswordChange: this.form.isAdminEnforcePasswordChange,
      adminEnforcePasswordChangeDays: this.form.adminEnforcePasswordChangeDays,
      isAdminEnforceLogout: this.form.isAdminEnforceLogout,
      adminEnforceLogoutMinutes: this.form.adminEnforceLogoutMinutes,
    }).then(function (response) {
      var res = response.data;

      $this.$message.success('管理员设置保存成功！');
    }).catch(function (error) {
      utils.error($this, error);
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