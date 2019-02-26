var $url = "/pages/cloud/layerLogin";

var $data = {
  pageLoad: false,
  pageSubmit: false,
  pageAlert: null,
  account: null,
  password: null,
  isAutoLogin: false,
  captcha: null,
  captchaUrl: null
};

var $methods = {
  load: function() {
    this.reload();
    this.pageLoad = true;
  },

  reload: function() {
    this.captcha = "";
    this.pageSubmit = false;
    this.captchaUrl =
      $ssApiUrl + "/" + $ssUrlCaptchaGet + "?r=" + new Date().getTime();
  },

  checkCaptcha: function() {
    var $this = this;

    utils.loading(true);
    $ssApi
      .post($ssUrlCaptchaCheck, {
        captcha: $this.captcha
      })
      .then(function(response) {
        var res = response.data;

        $this.login();
      })
      .catch(function(error) {
        $this.pageAlert = utils.getPageAlert(error);
      })
      .then(function() {
        utils.loading(false);
        $this.reload();
      });
  },

  login: function() {
    var $this = this;

    utils.loading(true);
    $ssApi
      .post($ssUrlLogin, {
        account: $this.account,
        password: md5($this.password),
        isAutoLogin: $this.isAutoLogin
      })
      .then(function(response) {
        var res = response.data;

        top.location.reload();

        // $api.post($url, {
        //   userName: res.value.userName,
        //   accessToken: res.accessToken,
        //   expiresAt: res.expiresAt
        // }).then(function (response) {
        //   var res = response.data;

        //   location.href = utils.getQueryString('returnUrl') || 'dashboard.cshtml';
        // }).catch(function (error) {
        //   utils.loading(false);
        //   $this.pageAlert = utils.getPageAlert(error);
        // });
      })
      .catch(function(error) {
        utils.loading(false);
        $this.pageAlert = utils.getPageAlert(error);
      });
  },

  btnLoginClick: function(e) {
    e.preventDefault();

    this.pageSubmit = true;
    this.pageAlert = null;
    if (!this.account || !this.password || !this.captcha) return;
    this.checkCaptcha();
  },

  btnRegisterClick: function() {
    location.href =
      "register.cshtml?returnUrl=" +
      (utils.getQueryString("returnUrl") || "login.cshtml");
  }
};

new Vue({
  el: "#main",
  data: $data,
  directives: {
    focus: {
      inserted: function(el) {
        el.focus();
      }
    }
  },
  methods: $methods,
  created: function() {
    this.load();
  }
});
