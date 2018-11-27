var $api = new utils.Api('/v1/users/actions/login');
var $captchaGetUrl = utils.getApiUrl('/v1/captcha/LOGIN-CAPTCHA');
var $captchaCheckApi = new utils.Api('/v1/captcha/LOGIN-CAPTCHA/actions/check');

if (window.top != self) {
  window.top.location = self.location;
}

var data = {
  pageConfig: null,
  pageSubmit: false,
  pageAlert: null,
  account: null,
  password: null,
  isAutoLogin: false,
  captcha: null,
  captchaUrl: null
};

var methods = {
  load: function (pageConfig) {
    this.pageConfig = pageConfig;
    this.reload();
  },

  reload: function () {
    this.captcha = '';
    this.pageSubmit = false;
    this.captchaUrl = $captchaGetUrl + '?r=' + new Date().getTime();
  },

  checkCaptcha: function () {
    var $this = this;

    utils.loading(true);
    $captchaCheckApi.post({
      captcha: $this.captcha
    }, function (err, res) {
      utils.loading(false);
      $this.reload();
      if (err) {
        $this.pageAlert = {
          type: 'danger',
          html: err.message
        };
        return;
      }

      $this.login();
    });
  },

  login: function () {
    var $this = this;

    utils.loading(true);
    $api.post({
      account: $this.account,
      password: md5($this.password),
      isAutoLogin: $this.isAutoLogin
    }, function (err, res) {
      utils.loading(false);
      if (err) {
        $this.pageAlert = {
          type: 'danger',
          html: err.message
        };
        return;
      }

      utils.setToken(res.accessToken, res.expiresAt);

      location.href = utils.getQueryString('returnUrl') || '../index.html';
    });
  },

  btnLoginClick: function (e) {
    e.preventDefault();

    this.pageSubmit = true;
    this.pageAlert = null;
    if (!this.account || !this.password || !this.captcha) return;
    this.checkCaptcha();
  },

  btnRegisterClick: function () {
    location.href = 'register.html?returnUrl=' + (utils.getQueryString('returnUrl') || 'login.html');
  }
};

new Vue({
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
    var $this = this;
    utils.getConfig('login', function (res) {
      $this.load(res.config);
    });
  }
});