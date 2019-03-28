var $url = '/v1/users/actions/login';
var $urlGetCaptcha = '/v1/captcha/LOGIN-CAPTCHA';
var $urlCaptchaCheck = '/v1/captcha/LOGIN-CAPTCHA/actions/check';

var $api = axios.create({
  baseURL: utils.getApiUrl(),
  withCredentials: true
});

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
    if (this.pageConfig.isHomeBackground && this.pageConfig.homeBackgroundUrl) {
      $('body').css({
        'background-image': 'url(' + this.pageConfig.homeBackgroundUrl + ')',
        'background-size': 'cover'
      });
    }
    this.reload();
  },

  reload: function () {
    this.captcha = '';
    this.pageSubmit = false;
    this.captchaUrl = utils.getApiUrl($urlGetCaptcha) + '?r=' + new Date().getTime();
  },

  checkCaptcha: function () {
    var $this = this;

    utils.loading(true);
    $api.post($urlCaptchaCheck, {
      captcha: $this.captcha
    }).then(function (response) {
      var res = response.data;

      $this.login();
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
      $this.reload();
    });
  },

  login: function () {
    var $this = this;

    utils.loading(true);
    $api.post($url, {
      account: $this.account,
      password: md5($this.password),
      isAutoLogin: $this.isAutoLogin
    }).then(function (response) {
      var res = response.data;

      utils.setToken(res.accessToken, res.expiresAt);
      location.href = utils.getQueryString('returnUrl') || '../index.html';
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
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
