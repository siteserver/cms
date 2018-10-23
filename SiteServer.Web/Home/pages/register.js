var $api = new utils.Api('/v1/users');
var $captchaGetUrl = utils.getApiUrl('/v1/captcha/REGISTER-CAPTCHA');
var $captchaCheckApi = new utils.Api('/v1/captcha/REGISTER-CAPTCHA/actions/check');

if (window.top != self) {
  window.top.location = self.location;
}

var data = {
  pageConfig: null,
  pageAlert: null,
  userName: null,
  password: null,
  captcha: null,
  captchaUrl: null,
  styles: [],
  groups: [],
  groupId: 0,
  isAgreement: false,
};

var methods = {
  load: function (pageConfig, styles, groups) {
    this.pageConfig = pageConfig;

    if (this.pageConfig.userRegistrationAttributes) {
      var userRegistrationAttributes = this.pageConfig.userRegistrationAttributes.split(',');
      for (var i = 0; i < styles.length; i++) {
        var style = styles[i];
        if (userRegistrationAttributes.indexOf(style.attributeName) !== -1) {
          style.value = style.defaultValue;

          this.styles.push(style);
        }
      }
    }

    this.groups = groups;
    this.reload();
  },
  reload: function () {
    this.captcha = '';
    this.captchaUrl = $captchaGetUrl + '?r=' + new Date().getTime();
  },
  checkCaptcha: function () {
    var $this = this;

    utils.loading(true);
    $captchaCheckApi.post({
      captcha: $this.captcha
    }, function (err) {
      utils.loading(false);

      if (err) {
        $this.reload();
        $this.pageAlert = {
          type: 'danger',
          html: err.message
        };
        return;
      }

      $this.register();
    });
  },
  register: function () {
    var $this = this;

    var payload = {
      userName: this.userName,
      password: this.password,
      groupId: this.groupId
    };
    for (var i = 0; i < this.styles.length; i++) {
      var style = this.styles[i];
      payload[style.attributeName] = style.value;
    }

    utils.loading(true);
    $api.post(payload, function (err, res) {
      utils.loading(false);
      if (err) {
        $this.reload();
        $this.pageAlert = {
          type: 'danger',
          html: err.message
        };
        return;
      }

      if (res.isChecked) {
        alert({
          title: "恭喜，账号注册成功",
          type: "success",
          confirmButtonText: "进入登录页"
        }).then(function () {
          location.href = 'login.html';
        });
      } else {
        alert({
          title: "账号注册成功，请等待管理员审核",
          type: "success",
          confirmButtonText: "进入登录页"
        }).then(function () {
          location.href = 'login.html';
        });
      }
    });
  },
  btnRegisterClick: function (e) {
    e.preventDefault();
    this.pageAlert = null;

    var $this = this;
    this.$validator.validate().then(function (result) {
      if ($this.pageConfig.isHomeAgreement && !$this.isAgreement) {
        return $this.pageAlert = {
          type: 'danger',
          html: '请勾选' + $this.pageConfig.homeAgreementHtml
        };
      }
      if (result) {
        $this.checkCaptcha();
      }
    });
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
    utils.getConfig('register', function (res) {
      $this.load(res.config, res.styles, res.groups);
    });
  }
});