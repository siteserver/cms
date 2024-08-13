var $url = '/login';
var $urlCaptcha = '/login/captcha';
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
  isAdminCaptchaDisabled: false,
  isSmsAdmin: false,
  isSmsAdminAndDisableAccount: false,

  isSmsLogin: false,
  mobile: null,
  code: null,
  countdown: 0,
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
        document.title = $this.adminTitle + ' 管理后台';
        $this.isAdminCaptchaDisabled = res.isAdminCaptchaDisabled;
        $this.isSmsAdmin = res.isSmsAdmin;
        $this.isSmsAdminAndDisableAccount = res.isSmsAdminAndDisableAccount;
        if (res.isSmsAdmin && res.isSmsAdminAndDisableAccount) {
          $this.isSmsLogin = true;
        }

        if (res.adminFaviconUrl) {
          var head = document.querySelector('head');
          var favicon = document.createElement('link');
          favicon.setAttribute('rel', 'shortcut icon');
          favicon.setAttribute('href', res.adminFaviconUrl || utils.getAssetsUrl('images/favicon.png'));
          head.appendChild(favicon);
        }

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

  apiSubmit: function (isForceLogoutAndLogin) {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, {
      isSmsLogin: this.isSmsLogin,
      account: this.account,
      password: md5(this.password),
      mobile: this.mobile,
      code: this.code,
      isPersistent: this.isPersistent,
      isForceLogoutAndLogin: isForceLogoutAndLogin,
      token: this.captchaToken,
      value: this.captchaValue
    }).then(function (response) {
      var res = response.data;

      if (res.isLoginExists) {
        $this.$confirm('该用户正在登录状态，可能是其他人正在使用或您上一次登录没有正常退出，是否强制注销并登录？', '强制登录提示', {
          confirmButtonText: '强制注销并登录',
          cancelButtonText: '取消',
          type: 'warning'
        }).then(() => {
          $this.apiSubmit(true);
        }).catch(() => {
          $this.$message({
            type: 'success',
            message: '已取消登录'
          });
          utils.loading($this, false);
        });
      } else {
        localStorage.setItem(SESSION_ID_NAME, res.sessionId);
        localStorage.removeItem(ACCESS_TOKEN_NAME);
        sessionStorage.removeItem(ACCESS_TOKEN_NAME);
        if ($this.isPersistent) {
          localStorage.setItem(ACCESS_TOKEN_NAME, res.token);
        } else {
          sessionStorage.setItem(ACCESS_TOKEN_NAME, res.token);
        }
        if (res.isEnforcePasswordChange) {
          $this.redirectPassword(res.administrator.userName);
        } else {
          $this.redirectIndex();
        }
      }
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      $this.apiCaptcha();
      utils.loading($this, false);
    });
  },

  redirectPassword: function (userName) {
    utils.openLayer({
      title: '更改密码',
      url: utils.getSettingsUrl('administratorsLayerPassword', {userName: userName, isEnforcePasswordChange: true}),
      width: 550,
      height: 300
    });
  },

  redirectIndex: function () {
    location.href = utils.getIndexUrl();
  },

  redirectLostPassword: function () {
    location.href = utils.getRootUrl('lostPassword');
  },

  isMobile: function (value) {
    return /^1[3-9]\d{9}$/.test(value);
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
    e && e.preventDefault();
    this.pageSubmit = true;
    this.pageAlert = null;

    if (this.isSmsLogin) {
      if (!this.mobile || !this.code) return;
      this.apiSubmit(false);
    } else {
      if (!this.account || !this.password) return;
      if (!this.isAdminCaptchaDisabled && !this.captchaValue) return;
      this.apiSubmit(false);
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
    utils.keyPress(this.btnSubmitClick);
    this.apiGet();
  }
});
