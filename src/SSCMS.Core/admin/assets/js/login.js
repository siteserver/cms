var $url = '/admin/login';

if (window.top != self) {
  window.top.location = self.location;
}

var data = utils.initData({
  pageSubmit: false,
  pageAlert: null,
  account: null,
  password: null,
  isAutoLogin: false,
  captcha: null,
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
    this.captcha = '';
    this.pageSubmit = false;
    this.captchaUrl = $apiUrl + $url + '/actions/captcha?r=' + Math.random();
  },

  checkCaptcha: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url + '/actions/captcha', {
      captcha: this.captcha
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
      isAutoLogin: this.isAutoLogin
    }).then(function (response) {
      var res = response.data;

      localStorage.setItem('sessionId', res.sessionId);
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
