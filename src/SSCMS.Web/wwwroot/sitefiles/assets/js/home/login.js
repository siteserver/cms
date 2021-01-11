var $url = '/login';
var $urlCaptcha = '/login/captcha';
var $urlCaptchaCheck = '/login/captcha/actions/check';
var $urlSendSms = '/login/actions/sendSms';

var data = utils.init({
  status: utils.getQueryInt('status'),
  pageAlert: null,
  captchaToken: null,
  captchaUrl: null,
  version: null,
  homeTitle: null,
  isSmsEnabled: false,
  countdown: 0,
  form: {
    type: 'account',
    account: null,
    password: null,
    mobile: null,
    code: null,
    isPersistent: false,
    captchaValue: null,
  },
  returnUrl: utils.getQueryString('returnUrl')
});

var methods = {
  apiGet: function () {
    var $this = this;

    if (this.status === 401) {
      this.pageAlert = {
        type: 'error',
        title: '您的账号登录已过期或失效，请重新登录'
      };
    }

    utils.loading(this, true);
    $api.get($url).then(function (response) {
      var res = response.data;

      $this.version = res.version;
      $this.homeTitle = res.homeTitle;
      $this.isSmsEnabled = res.isSmsEnabled;
      $this.apiCaptcha();
    }).catch(function (error) {
      utils.notifyError(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiCaptcha: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlCaptcha).then(function (response) {
      var res = response.data;

      $this.captchaToken = res.value;
      $this.captchaUrl = $apiUrl + $urlCaptcha + '?token=' + $this.captchaToken;
      $this.btnTypeClick();
    }).catch(function (error) {
      utils.notifyError(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiCaptchaCheck: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlCaptchaCheck, {
      token: this.captchaToken,
      value: this.form.captchaValue
    }).then(function (response) {
      $this.apiSubmit();
    }).catch(function (error) {
      $this.apiCaptcha();
      utils.loading($this, false);
      utils.notifyError(error);
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

  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, {
      isSmsLogin: this.form.type == 'mobile',
      account: this.form.account,
      password: this.form.password ? md5(this.form.password) : '',
      mobile: this.form.mobile,
      code: this.form.code,
      isPersistent: this.form.isPersistent
    }).then(function (response) {
      var res = response.data;
      
      localStorage.removeItem(ACCESS_TOKEN_NAME);
      localStorage.setItem(ACCESS_TOKEN_NAME, res.token);
      if (res.redirectToVerifyMobile) {
        location.href = utils.getRootUrl('verifyMobile', {
          returnUrl: $this.returnUrl
        });
      } else if ($this.returnUrl) {
        location.href = $this.returnUrl;
      } else {
        $this.redirectIndex();
      }
    }).catch(function (error) {
      utils.notifyError(error);
    }).then(function () {
      $this.apiCaptcha();
      utils.loading($this, false);
    });
  },

  redirectIndex: function () {
    location.href = utils.getIndexUrl();
  },

  redirectLostPassword: function () {
    location.href = utils.getRootUrl('lostPassword');
  },

  btnTypeClick: function() {
    var $this = this;

    this.$refs.formAccount.clearValidate();
    this.$refs.formMobile.clearValidate();
    if (this.form.type == 'account') {
      setTimeout(function () {
        $this.$refs['account'].focus();
      }, 100);
    } else if (this.form.type == 'mobile') {
      setTimeout(function () {
        $this.$refs['mobile'].focus();
      }, 100);
    }
  },

  btnCaptchaClick: function () {
    this.apiCaptcha();
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

  btnSubmitClick: function () {
    var $this = this;

    if (this.form.type == 'account') {
      this.$refs.formAccount.validate(function(valid) {
        if (valid) {
          $this.apiCaptchaCheck();
        }
      });
    } else {
      this.$refs.formMobile.validate(function(valid) {
        if (valid) {
          $this.apiSubmit();
        }
      });
    }
  },

  btnRegisterClick: function(e) {
    e.preventDefault();
    location.href = utils.getRootUrl('register');
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
