var $url = '/login';
var $urlCaptcha = '/login/captcha';
var $urlCaptchaCheck = '/login/captcha/actions/check';

var data = utils.init({
  pageSubmit: false,
  account: null,
  password: null,
  captchaToken: null,
  captchaValue: null,
  captchaUrl: null,
  version: null,
  homeTitle: null,
  returnUrl: utils.getQueryString('returnUrl')
});

var methods = {
  load: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url).then(function (response) {
      var res = response.data;

      $this.version = res.version;
      $this.homeTitle = res.homeTitle;
      $this.reload();
    }).catch(function (error) {
      utils.error(error);
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
      utils.error(error);
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
      utils.error(error);
    });
  },

  login: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, {
      account: this.account,
      password: md5(this.password),
      isPersistent: true
    }).then(function (response) {
      var res = response.data;
      
      localStorage.removeItem(ACCESS_TOKEN_NAME);
      localStorage.setItem(ACCESS_TOKEN_NAME, res.token);
      if (res.isEnforcePasswordChange) {
        $this.redirectPassword();
      } else if ($this.returnUrl) {
        location.href = $this.returnUrl;
      } else {
        $this.redirectIndex();
      }
    }).catch(function (error) {
      utils.error(error);
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
    if (!this.account || !this.password || !this.captchaValue) return;
    this.checkCaptcha();
  },

  btnRegisterClick: function(e) {
    e.preventDefault();
    location.href = utils.getRootUrl('register');
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
