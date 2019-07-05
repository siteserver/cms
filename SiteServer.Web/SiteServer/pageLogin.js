var $url = '/pages/login';
var $urlLogin = '/v1/administrators/actions/login';
var $urlGetCaptcha = '/v1/captcha/LOGIN-CAPTCHA';
var $urlCheckCaptcha = '/v1/captcha/LOGIN-CAPTCHA/actions/check';

if (window.top != self) {
  window.top.location = self.location;
}

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
  load: function () {
    var $this = this;

    $api.get($url).then(function (response) {
      var res = response.data;

      if (res.value) {
        $this.reload();
      } else {
        location.href = res.redirectUrl;
      }
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
    });
  },

  reload: function () {
    this.pageLoad = true;
    this.captcha = '';
    this.pageSubmit = false;
    this.captchaUrl = apiUrl + $urlGetCaptcha + '?r=' + new Date().getTime();
  },

  checkCaptcha: function () {
    var $this = this;

    utils.loading(true);
    $api.post($urlCheckCaptcha, {
      captcha: $this.captcha
    }).then(function (response) {
      $this.login();
    }).catch(function (error) {
      utils.loading(false);
      $this.reload();
      $this.pageAlert = utils.getPageAlert(error);
    });
  },

  login: function () {
    var $this = this;

    $api.post($urlLogin, {
      account: $this.account,
      password: md5($this.password),
      isAutoLogin: $this.isAutoLogin
    }).then(function (response) {
      $this.redirect();
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
      $this.reload();
    });
  },

  redirect: function () {
    location.href = 'pageInitialization.aspx';
  },

  btnLoginClick: function (e) {
    e.preventDefault();

    this.pageSubmit = true;
    this.pageAlert = null;
    if (!this.account || !this.password || !this.captcha) return;
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
