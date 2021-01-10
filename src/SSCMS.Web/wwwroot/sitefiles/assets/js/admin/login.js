var $url = '/login';
var $urlCaptcha = '/login/captcha';
var $urlCaptchaCheck = '/login/captcha/actions/check';
var $urlSendSms = '/login/actions/sendSms';

if (window.top != self) {
  window.top.location = self.location;
}

var data = utils.init({
  status: utils.getQueryInt('status'),
  pageSubmit: false,
  pageAlert: null,
  account: null,
  password: null,
  isPersistent: false,
  captchaToken: null,
  captchaValue: null,
  captchaUrl: null,
  version: null,
  adminTitle: null,
  isSmsEnabled: false,
  isSmsLogin: false,
  mobile: null,
  code: null,
  countdown: 0
});

var methods = {
  apiGet: function () {
    var $this = this;

    if (this.status === 401) {
      this.pageAlert = {
        type: 'danger',
        html: '您的账号登录已过期或失效，请重新登录'
      };
    }

    utils.loading(this, true);
    $api.get($url).then(function (response) {
      var res = response.data;

      if (res.success) {
        $this.version = res.version;
        $this.adminTitle = res.adminTitle;
        $this.isSmsEnabled = res.isSmsEnabled;
        $this.apiCaptcha();
      } else {
        location.href = res.redirectUrl;
      }
    }).catch(function (error) {
      utils.error(error);
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
      $this.captchaValue = '';
      $this.pageSubmit = false;
      $this.captchaUrl = $apiUrl + $urlCaptcha + '?token=' + $this.captchaToken;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiCaptchaCheck: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlCaptchaCheck, {
      token: this.captchaToken,
      value: this.captchaValue
    }).then(function (response) {
      $this.apiSubmit();
    }).catch(function (error) {
      $this.apiCaptcha();
      utils.loading($this, false);
      utils.error(error);
    });
  },

  apiSendSms: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlSendSms, {
      mobile: this.mobile
    }).then(function (response) {
      var res = response.data;

      utils.success('验证码发送成功，10分钟内有效');
      $this.countdown = 60;
      var interval = setInterval(function () {
        $this.countdown -= 1;
        if ($this.countdown <= 0){
          clearInterval(interval);
        }
      }, 1000);
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
      isSmsLogin: this.isSmsLogin,
      account: this.account,
      password: md5(this.password),
      mobile: this.mobile,
      code: this.code,
      isPersistent: this.isPersistent,
    }).then(function (response) {
      var res = response.data;

      localStorage.setItem('sessionId', res.sessionId);
      
      localStorage.removeItem(ACCESS_TOKEN_NAME);
      sessionStorage.removeItem(ACCESS_TOKEN_NAME);
      if ($this.isPersistent) {
        localStorage.setItem(ACCESS_TOKEN_NAME, res.token);
      } else {
        sessionStorage.setItem(ACCESS_TOKEN_NAME, res.token);
      }
      if (res.isEnforcePasswordChange) {
        $this.redirectPassword();
      } else {
        $this.redirectIndex();
      }
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      $this.apiCaptcha();
      utils.loading($this, false);
    });
  },

  redirectPassword: function () {
    location.href = utils.getSettingsUrl('administratorsPassword');
  },

  redirectIndex: function () {
    location.href = utils.getIndexUrl();
  },

  redirectLostPassword: function () {
    location.href = utils.getRootUrl('lostPassword');
  },

  isMobile: function (value) {
    return /^1[3|4|5|7|8][0-9]\d{8}$/.test(value);
  },

  btnSendSmsClick: function () {
    if (this.countdown > 0) return;
    if (!this.mobile) {
      this.pageAlert = {
        type: 'danger',
        html: '手机号码不能为空'
      };
      return;
    } else if (!this.isMobile(this.mobile)) {
      this.pageAlert = {
        type: 'danger',
        html: '请输入有效的手机号码'
      };
      return;
    }

    this.pageAlert = null;
    this.apiSendSms();
  },

  btnCaptchaClick: function() {
    this.apiCaptcha();
  },

  btnSubmitClick: function (e) {
    e.preventDefault();

    this.pageSubmit = true;
    this.pageAlert = null;
    if (this.isSmsLogin) {
      if (!this.mobile || !this.code) return;
      this.apiSubmit();
    } else {
      if (!this.account || !this.password || !this.captchaValue) return;
      this.apiCaptchaCheck();
    }
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  directives: {
    focus: {
      inserted: function (el) {
        el.focus()
      }
    }
  },
  methods: methods,
  created: function () {
    this.apiGet();
  }
});
