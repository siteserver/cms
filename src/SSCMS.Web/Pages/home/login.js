var $url = '/home/login';
var $urlCaptcha = '/home/login/captcha';
var $urlCaptchaCheck = '/home/login/captcha/actions/check';

var data = {
  pageLoad: false,
  homeTitle: null,
  isUserRegistrationAllowed: null,
  pageSubmit: false,
  account: null,
  password: null,
  isPersistent: false,
  captchaToken: null,
  captchaValue: null,
  captchaUrl: null
};

var methods = {
  apiGet: function() {
    var $this = this;

    utils.loading(this, true);
    $api.get($url).then(function (response) {
      var res = response.data;
      $this.config = res;

      $this.homeTitle = res.homeTitle;
      $this.isUserRegistrationAllowed = res.isUserRegistrationAllowed;
      $this.reload();
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
      
      localStorage.removeItem(utils.USER_ACCESS_TOKEN_NAME);
      sessionStorage.removeItem(utils.USER_ACCESS_TOKEN_NAME);
      if ($this.isPersistent) {
        localStorage.setItem(utils.USER_ACCESS_TOKEN_NAME, res.token);
      } else {
        sessionStorage.setItem(utils.USER_ACCESS_TOKEN_NAME, res.token);
      }
      $this.redirectIndex();
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      $this.reload();
      utils.loading($this, false);
    });
  },

  btnLoginClick: function (e) {
    e.preventDefault();

    this.pageSubmit = true;
    if (!this.account || !this.password || !this.captchaValue) return;
    this.checkCaptcha();
  },

  redirectIndex: function () {
    location.href = utils.getQueryString('returnUrl') || 'index.html';
  },

  btnRegisterClick: function () {
    location.href = 'register.html?returnUrl=' + (utils.getQueryString('returnUrl') || 'login.html');
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