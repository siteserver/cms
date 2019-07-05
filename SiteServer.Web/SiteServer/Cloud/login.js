var data = {
  pageLoad: false,
  pageSubmit: false,
  pageAlert: null,
  account: null,
  password: null,
  isAutoLogin: false,
  captcha: null,
  captchaUrl: null
};

var methods = {
  reload: function () {
    this.captcha = '';
    this.pageSubmit = false;
    this.captchaUrl = $urlCloud + $urlCaptchaGet + '?r=' + new Date().getTime();
    this.pageLoad = true;
  },

  checkCaptcha: function () {
    var $this = this;

    utils.loading(true);
    $ssApi.post($urlCaptchaCheck, {
        captcha: $this.captcha
      })
      .then(function (response) {
        $this.login();
      })
      .catch(function (error) {
        $this.pageAlert = utils.getPageAlert(error);
      })
      .then(function () {
        utils.loading(false);
        $this.reload();
      });
  },

  login: function () {
    var $this = this;

    utils.loading(true);
    $ssApi.post($urlLogin, {
        account: $this.account,
        password: md5($this.password),
        isAutoLogin: $this.isAutoLogin
      })
      .then(function (response) {
        var res = response.data;

        ssUtils.setToken(res.accessToken, res.expiresAt);
        location.href = utils.getQueryString('returnUrl') || 'settings.html';
      })
      .catch(function (error) {
        $this.pageAlert = utils.getPageAlert(error);
      })
      .then(function () {
        utils.loading(false);
        $this.reload();
      });
  },

  btnLoginClick: function (e) {
    e.preventDefault();

    this.pageSubmit = true;
    this.pageAlert = null;
    if (!this.account || !this.password || !this.captcha) return;
    this.checkCaptcha();
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
    this.reload();
  }
});