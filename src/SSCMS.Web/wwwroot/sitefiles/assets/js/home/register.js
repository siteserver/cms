var $url = '/register';
var $urlCaptcha = '/register/captcha';
var $urlCaptchaCheck = '/register/captcha/actions/check';
var $urlSendSms = '/register/actions/sendSms';
var $urlVerifyMobile = '/register/actions/verifyMobile';

var data = utils.init({
  returnUrl: decodeURIComponent(utils.getQueryString('returnUrl')),
  isSmsEnabled: null,
  isUserVerifyMobile: null,
  isUserRegistrationMobile: null,
  isUserRegistrationEmail: null,
  isUserRegistrationGroup: null,
  isHomeAgreement: null,
  homeAgreementHtml: null,
  styles: null,
  groups: null,
  isAgreement: false,
  captchaToken: null,
  captchaUrl: null,
  countdown: 0,
  form: {
    mobile: '',
    code: '',
    captchaValue: '',
    groupId: 0
  }
});

var methods = {
  insertText: function(attributeName, no, text) {
    var count = this.form[utils.getCountName(attributeName)];
    if (count < no) {
      this.form[utils.getCountName(attributeName)] = no;
    }
    this.form[utils.getExtendName(attributeName, no)] = text;
    this.form = _.assign({}, this.form);
  },

  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url).then(function (response) {
      var res = response.data;

      $this.isSmsEnabled = res.isSmsEnabled;
      $this.isUserVerifyMobile = res.isUserVerifyMobile;
      $this.isUserRegistrationMobile = res.isUserRegistrationMobile;
      $this.isUserRegistrationEmail = res.isUserRegistrationEmail;
      $this.isUserRegistrationGroup = res.isUserRegistrationGroup;
      $this.isHomeAgreement = res.isHomeAgreement;
      $this.homeAgreementHtml = res.homeAgreementHtml;
      $this.styles = res.styles;
      for (var i = 0; i < res.styles.length; i++) {
        var style = res.styles[i];
        $this.form[utils.toCamelCase(style.attributeName)] = style.defaultValue;
      }
      $this.form = _.assign({}, $this.form);
      $this.groups = res.groups;

      $this.apiCaptchaReload();
    }).catch(function (error) {
      utils.notifyError(error, {redirect: true});
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSendSms: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlSendSms, {
      mobile: this.form.mobile
    }).then(function (response) {
      var res = response.data;

      utils.notifySuccess('验证码发送成功，10分钟内有效');
      $this.countdown = 60;
      var interval = setInterval(function () {
        $this.countdown -= 1;
        if ($this.countdown <= 0){
          clearInterval(interval);
        }
      }, 1000);
    }).catch(function (error) {
      utils.notifyError(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnExtendAddClick: function(style) {
    var no = this.form[utils.getCountName(style.attributeName)] + 1;
    this.form[utils.getCountName(style.attributeName)] = no;
    this.form[utils.getExtendName(style.attributeName, no)] = '';
  },

  btnExtendRemoveClick: function(style) {
    var no = this.form[utils.getCountName(style.attributeName)] - 1;
    this.form[utils.getCountName(style.attributeName)] = no;
    this.form[utils.getExtendName(style.attributeName, no)] = '';
  },

  btnPreviewClick: function(attributeName, n) {
    var imageUrl = n ? this.form[utils.getExtendName(attributeName, n)] : this.form[attributeName];
    window.open(imageUrl);
  },

  apiCaptchaReload: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlCaptcha).then(function (response) {
      var res = response.data;

      $this.captchaToken = res.value;
      $this.form.captchaValue = '';
      $this.captchaUrl = $apiUrl + $urlCaptcha + '?token=' + $this.captchaToken;
    }).catch(function (error) {
      utils.notifyError(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiCheckCaptcha: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlCaptchaCheck, {
      token: this.captchaToken,
      value: this.form.captchaValue
    }).then(function (response) {
      $this.apiSubmit();
    }).catch(function (error) {
      $this.apiCaptchaReload();
      utils.loading($this, false);
      utils.notifyError(error);
    });
  },

  apiVerifyMobile: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlVerifyMobile, {
      mobile: this.form.mobile,
      code: this.form.code
    }).then(function (response) {
      $this.apiSubmit();
    }).catch(function (error) {
      utils.loading($this, false);
      utils.notifyError(error);
    });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);

    var payload = _.assign({}, this.form);
    delete payload.captchaValue;
    delete payload.confirmPassword;

    $api.post($url, payload).then(function (response) {
      var res = response.data;

      if (res.value) {
        utils.alertSuccess({
          title: "恭喜，账号注册成功",
          button: "进入登录页",
          callback: function() {
            location.href = utils.getRootUrl('login', {
              returnUrl: $this.returnUrl
            });
          }
        });
      } else {
        utils.alertSuccess({
          title: "账号注册成功，请等待管理员审核",
          button: "进入登录页",
          callback: function() {
            location.href = utils.getRootUrl('login', {
              returnUrl: $this.returnUrl
            });
          }
        });
      }
    }).catch(function (error) {
      $this.apiCaptchaReload();
      utils.loading($this, false);
      utils.notifyError(error);
    });
  },

  isMobile: function (value) {
    return /^1[3|4|5|7|8][0-9]\d{8}$/.test(value);
  },

  btnSendSmsClick: function () {
    if (this.countdown > 0) return;
    if (!this.form.mobile) {
      utils.notifyError('手机号码不能为空');
      return;
    } else if (!this.isMobile(this.form.mobile)) {
      utils.notifyError('请输入有效的手机号码');
      return;
    }

    this.apiSendSms();
  },

  btnRegisterClick: function () {
    var $this = this;

    this.$refs.form.validate(function(valid) {
      if (valid) {
        if ($this.isMobileCode) {
          $this.apiVerifyMobile();
        } else {
          $this.apiCheckCaptcha();
        }
      }
    });
  },

  btnLoginClick: function() {
    location.href = utils.getRootUrl('login');
  },

  validatePass: function(rule, value, callback) {
    if (value === '') {
      callback(new Error('请再次输入密码'));
    } else if (value !== this.form.password) {
      callback(new Error('两次输入密码不一致!'));
    } else {
      callback();
    }
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  computed: {
    isMobile: function () {
      return this.isUserVerifyMobile || this.isUserRegistrationMobile;
    },
    isMobileCode: function () {
      return this.isUserVerifyMobile || (this.isSmsEnabled && this.isUserRegistrationMobile);
    }
  },
  created: function () {
    this.apiGet();
  }
});