var $api = new apiUtils.Api($apiConfig.apiUrl + '/v1/administrators/actions/login');
var $innerApi = new apiUtils.Api($apiConfig.innerApiUrl + '/v1/administrators/actions/login');
var $captchaGetUrl = $apiConfig.innerApiUrl + '/v1/captcha/LOGIN-CAPTCHA';
var $captchaCheckApi = new apiUtils.Api($apiConfig.innerApiUrl + '/v1/captcha/LOGIN-CAPTCHA/actions/check');

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
      this.captchaUrl = $captchaGetUrl + '?r=' + new Date().getTime();
    },
    checkCaptcha: function(account, password, captcha) {
      var $this = this;

      pageUtils.loading(true);
      $captchaCheckApi.post({
        captcha: captcha
      }, function (err, res) {
        pageUtils.loading(false);
        if (err) {
          $this.pageAlert = {
            type: 'danger',
            html: '验证码不正确，请重新输入！'
          };
          return;
        }

        $this.login(account, password);
      });
    },
    login: function (account, password) {
      var $this = this;

      pageUtils.loading(true);
      $innerApi.post({
        account: account,
        password: md5(password)
      }, function (err, res) {
        pageUtils.loading(false);
        if (err) {
          $this.pageAlert = {
            type: 'danger',
            html: err.message
          };
          return;
        }

        if ($apiConfig.isSeparatedApi) {
          $this.loginSeparatedApi(account, password);
        } else {
          $this.redirect();
        }
      });
    },
    loginSeparatedApi: function (account, password) {
      var $this = this;

      pageUtils.loading(true);
      $api.post({
        account: account,
        password: md5(password)
      }, function (err, res) {
        pageUtils.loading(false);

        if (err) {
          $this.pageAlert = {
            type: 'danger',
            html: '系统检测到API部署方式为独立部署且独立API不能正常工作，请联系系统维护人员修复此问题 <a href="pageInitialization.aspx">进入后台</a>'
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
      this.checkCaptcha(this.account, this.password, this.captcha);
    }
  }
});

$vue.reload();