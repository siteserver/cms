var $api = new apiUtils.Api(apiUrl + '/pages/settings/adminConfig');

var data = {
  pageLoad: false,
  pageAlert: null,
  pageType: null,
  config: null,

  adminUserNameMinLength: null,
  adminPasswordMinLength: null,
  adminPasswordRestriction: null,
  isAdminLockLogin: null,
  adminLockLoginCount: null,
  adminLockLoginType: null,
  adminLockLoginHours: null,
  isViewContentOnlySelf: null,
  isAdminEnforcePasswordChange: null,
  adminEnforcePasswordChangeDays: null,
  isAdminEnforceLogout: null,
  adminEnforceLogoutMinutes: null,
};

var methods = {
  getConfig: function () {
    var $this = this;

    $api.get(null, function (err, res) {
      if (err || !res || !res.value) return;

      $this.config = _.clone(res.value);

      $this.adminUserNameMinLength = res.value.adminUserNameMinLength;
      $this.adminPasswordMinLength = res.value.adminPasswordMinLength;
      $this.adminPasswordRestriction = res.value.adminPasswordRestriction;
      $this.isAdminLockLogin = res.value.isAdminLockLogin;
      $this.adminLockLoginCount = res.value.adminLockLoginCount;
      $this.adminLockLoginType = res.value.adminLockLoginType;
      $this.adminLockLoginHours = res.value.adminLockLoginHours;
      $this.isViewContentOnlySelf = res.value.isViewContentOnlySelf;
      $this.isAdminEnforcePasswordChange = res.value.isAdminEnforcePasswordChange;
      $this.adminEnforcePasswordChangeDays = res.value.adminEnforcePasswordChangeDays;
      $this.isAdminEnforceLogout = res.value.isAdminEnforceLogout;
      $this.adminEnforceLogoutMinutes = res.value.adminEnforceLogoutMinutes;

      $this.pageType = 'list';
      $this.pageLoad = true;
    });
  },
  submit: function (item) {
    var $this = this;

    pageUtils.loading(true);
    $api.post({
      adminUserNameMinLength: $this.adminUserNameMinLength,
      adminPasswordMinLength: $this.adminPasswordMinLength,
      adminPasswordRestriction: $this.adminPasswordRestriction,
      isAdminLockLogin: $this.isAdminLockLogin,
      adminLockLoginCount: $this.adminLockLoginCount,
      adminLockLoginType: $this.adminLockLoginType,
      adminLockLoginHours: $this.adminLockLoginHours,
      isViewContentOnlySelf: $this.isViewContentOnlySelf,
      isAdminEnforcePasswordChange: $this.isAdminEnforcePasswordChange,
      adminEnforcePasswordChangeDays: $this.adminEnforcePasswordChangeDays,
      isAdminEnforceLogout: $this.isAdminEnforceLogout,
      adminEnforceLogoutMinutes: $this.adminEnforceLogoutMinutes,
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
        html: '管理员设置保存成功！'
      };
      $this.config = _.clone(res.value);
      $this.pageType = 'list';
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
        $this.submit($this.item);
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