var $url = '/admin/login';
var $urlCaptcha = '/admin/login/captcha';
var $urlCaptchaCheck = '/admin/login/captcha/actions/check';

if (window.top != self) {
  window.top.location = self.location;
}

var data = utils.initData({
  pageSubmit: false,
  pageAlert: null,
  account: null,
  password: null,
  isPersistent: false,
  captchaToken: null,
  captchaValue: null,
  captchaUrl: null,
  productVersion: null,
  adminTitle: null
});

var methods = {
  load: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url).then(function (response) {
      var res = response.data;

      if (res.success) {
        $this.productVersion = res.productVersion;
        $this.adminTitle = res.adminTitle;
        $this.reload();
      } else {
        location.href = res.redirectUrl;
      }
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  reload: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlCaptcha).then(function (response) {
      var res = response.data;

      $this.captchaToken = res.value;
      $this.captchaValue = '';
      $this.pageSubmit = false;
      $this.captchaUrl = $apiUrl + $urlCaptcha + '?token=' + $this.captchaToken;
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  checkCaptcha: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlCaptchaCheck, {
      token: this.captchaToken,
      value: this.captchaValue
    }).then(function (response) {
      $this.login();
    }).catch(function (error) {
      $this.reload();
      utils.loading($this, false);
      utils.error($this, error);
    });
  },

  login: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, {
      account: this.account,
      password: md5(this.password),
      isPersistent: this.isPersistent
    }).then(function (response) {
      var res = response.data;

      localStorage.setItem('sessionId', res.sessionId);
      
      localStorage.removeItem(utils.ACCESS_TOKEN_NAME);
      sessionStorage.removeItem(utils.ACCESS_TOKEN_NAME);
      if ($this.isPersistent) {
        localStorage.setItem(utils.ACCESS_TOKEN_NAME, res.token);
      } else {
        sessionStorage.setItem(utils.ACCESS_TOKEN_NAME, res.token);
      }
      if (res.isEnforcePasswordChange) {
        $this.redirectPassword();
      } else {
        $this.redirectIndex();
      }
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      $this.reload();
      utils.loading($this, false);
    });
  },

  redirectPassword: function () {
    location.href = utils.getSettingsUrl('administratorsPassword');
  },

  redirectIndex: function () {
    location.href = utils.getIndexUrl();
  },

  btnLoginClick: function (e) {
    e.preventDefault();

    this.pageSubmit = true;
    this.pageAlert = null;
    if (!this.account || !this.password || !this.captchaValue) return;
    this.checkCaptcha();
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
    this.load();
  }
});
