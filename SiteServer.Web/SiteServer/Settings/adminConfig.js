var $url = '/pages/settings/adminConfig';

var data = utils.initData({
  pageType: null,
  config: null,

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
});

var methods = {
  getConfig: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url).then(function (response) {
      var res = response.data;

      $this.config = _.assign({}, res.value);

      $this.adminUserNameMinLength = res.value.adminUserNameMinLength;
      $this.adminPasswordMinLength = res.value.adminPasswordMinLength;
      $this.adminPasswordRestriction = res.value.adminPasswordRestriction;
      $this.isAdminLockLogin = res.value.isAdminLockLogin;
      $this.adminLockLoginCount = res.value.adminLockLoginCount;
      $this.adminLockLoginType = res.value.adminLockLoginType;
      $this.adminLockLoginHours = res.value.adminLockLoginHours;
      $this.isAdminEnforcePasswordChange = res.value.isAdminEnforcePasswordChange;
      $this.adminEnforcePasswordChangeDays = res.value.adminEnforcePasswordChangeDays;
      $this.isAdminEnforceLogout = res.value.isAdminEnforceLogout;
      $this.adminEnforceLogoutMinutes = res.value.adminEnforceLogoutMinutes;

      $this.pageType = 'list';
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function (item) {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, {
      adminUserNameMinLength: this.adminUserNameMinLength,
      adminPasswordMinLength: this.adminPasswordMinLength,
      adminPasswordRestriction: this.adminPasswordRestriction,
      isAdminLockLogin: this.isAdminLockLogin,
      adminLockLoginCount: this.adminLockLoginCount,
      adminLockLoginType: this.adminLockLoginType,
      adminLockLoginHours: this.adminLockLoginHours,
      isAdminEnforcePasswordChange: this.isAdminEnforcePasswordChange,
      adminEnforcePasswordChangeDays: this.adminEnforcePasswordChangeDays,
      isAdminEnforceLogout: this.isAdminEnforceLogout,
      adminEnforceLogoutMinutes: this.adminEnforceLogoutMinutes,
    }).then(function (response) {
      var res = response.data;

      $this.config = _.assign({}, res.value);
      $this.pageType = 'list';
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
    this.$validator.validate().then(function (result) {
      if (result) {
        $this.apiSubmit($this.item);
      }
    });
  }
};

new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.getConfig();
  }
});