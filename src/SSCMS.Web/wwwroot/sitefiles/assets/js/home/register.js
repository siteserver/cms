var $url = '/register';
var $urlCaptcha = '/register/captcha';
var $urlCaptchaCheck = '/register/captcha/actions/check';

var data = utils.init({
  returnUrl: decodeURIComponent(utils.getQueryString('returnUrl')),
  isUserRegistrationGroup: null,
  isHomeAgreement: null,
  homeAgreementHtml: null,
  styles: null,
  groups: null,
  isAgreement: false,
  captchaToken: null,
  captchaUrl: null,
  form: null
});

var methods = {
  insertText: function(attributeName, no, text) {
    var count = this.form[utils.getCountName(attributeName)];
    if (count < no) {
      this.form[utils.getCountName(attributeName)] = no;
    }
    this.form[utils.getExtendName(attributeName, no)] = text;
    this.form = _.assign({}, this.form);
  },

  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url).then(function (response) {
      var res = response.data;

      $this.form = {
        captchaValue: '',
        groupId: 0
      };
      $this.isUserRegistrationGroup = res.isUserRegistrationGroup;
      $this.isHomeAgreement = res.isHomeAgreement;
      $this.homeAgreementHtml = res.homeAgreementHtml;
      $this.styles = res.styles;
      for (var i = 0; i < res.styles.length; i++) {
        var style = res.styles[i];
        $this.form[_.lowerFirst(style.attributeName)] = style.defaultValue;
      }
      $this.form = _.assign({}, $this.form);
      $this.groups = res.groups;

      $this.apiCaptchaReload();
    }).catch(function (error) {
      utils.error(error, {redirect: true});
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnExtendAddClick: function(style) {
    var no = this.form[utils.getCountName(style.attributeName)] + 1;
    this.form[utils.getCountName(style.attributeName)] = no;
    this.form[utils.getExtendName(style.attributeName, no)] = '';
  },

  btnExtendRemoveClick: function(style) {
    var no = this.form[utils.getCountName(style.attributeName)] - 1;
    this.form[utils.getCountName(style.attributeName)] = no;
    this.form[utils.getExtendName(style.attributeName, no)] = '';
  },

  btnPreviewClick: function(attributeName, n) {
    var imageUrl = n ? this.form[utils.getExtendName(attributeName, n)] : this.form[attributeName];
    window.open(imageUrl);
  },

  apiCaptchaReload: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlCaptcha).then(function (response) {
      var res = response.data;

      $this.captchaToken = res.value;
      $this.form.captchaValue = '';
      $this.captchaUrl = $apiUrl + $urlCaptcha + '?token=' + $this.captchaToken;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiCheckCaptcha: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlCaptchaCheck, {
      token: this.captchaToken,
      value: this.form.captchaValue
    }).then(function (response) {
      $this.apiSubmit();
    }).catch(function (error) {
      $this.apiCaptchaReload();
      utils.loading($this, false);
      utils.error(error);
    });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);

    var payload = _.assign({}, this.form);
    delete payload.captchaValue;
    delete payload.confirmPassword;

    $api.post($url, payload).then(function (response) {
      var res = response.data;

      if (res.value) {
        utils.alertSuccess({
          title: "恭喜，账号注册成功",
          button: "进入登录页",
          callback: function() {
            location.href = utils.getRootUrl('login', {
              returnUrl: $this.returnUrl
            });
          }
        });
      } else {
        utils.alertSuccess({
          title: "账号注册成功，请等待管理员审核",
          button: "进入登录页",
          callback: function() {
            location.href = utils.getRootUrl('login', {
              returnUrl: $this.returnUrl
            });
          }
        });
      }
    }).catch(function (error) {
      $this.apiCaptchaReload();
      utils.loading($this, false);
      utils.error(error);
    });
  },

  btnRegisterClick: function () {
    var $this = this;

    this.$refs.form.validate(function(valid) {
      if (valid) {
        $this.apiCheckCaptcha();
      }
    });
  },

  btnLoginClick: function() {
    location.href = utils.getRootUrl('login');
  },

  validatePass: function(rule, value, callback) {
    if (value === '') {
      callback(new Error('请再次输入密码'));
    } else if (value !== this.form.password) {
      callback(new Error('两次输入密码不一致!'));
    } else {
      callback();
    }
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiGet();
  }
});