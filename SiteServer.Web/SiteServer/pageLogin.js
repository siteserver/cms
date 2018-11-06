var $api = new apiUtils.Api(apiUrl + '/v1/administrators/actions/login');
var $captchaGetUrl = apiUrl + '/v1/captcha/LOGIN-CAPTCHA';
var $captchaCheckApi = new apiUtils.Api(apiUrl + '/v1/captcha/LOGIN-CAPTCHA/actions/check');

if (window.top != self) {
  window.top.location = self.location;
}

var $vue = new Vue({
  el: '#main',
  data: {
    pageLoad: false,
    pageSubmit: false,
    pageAlert: null,
    account: null,
    password: null,
    isAutoLogin: false,
    captcha: null,
    captchaUrl: null
  },
  directives: {
    focus: {
      inserted: function (el) {
        el.focus()
      }
    }
  },
  methods: {
    reload: function () {
      this.pageLoad = true;
      this.captcha = '';
      this.pageSubmit = false;
      this.captchaUrl = $captchaGetUrl + '?r=' + new Date().getTime();
    },
    
    checkCaptcha: function () {
      var $this = this;

      pageUtils.loading(true);
      $captchaCheckApi.post({
        captcha: $this.captcha
      }, function (err, res) {
        pageUtils.loading(false);
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

      pageUtils.loading(true);
      $api.post({
        account: $this.account,
        password: md5($this.password),
        isAutoLogin: $this.isAutoLogin
      }, function (err, res) {
        pageUtils.loading(false);
        if (err) {
          $this.pageAlert = {
            type: 'danger',
            html: err.message
          };
          return;
        }

        $this.redirect();
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
  }
});

$vue.reload();