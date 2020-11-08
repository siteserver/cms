var $url = '/lostPassword';
var $urlSendSms = '/lostPassword/actions/sendSms';

var data = utils.init({
  form: {
    mobile: '',
    code: '',
    password: '',
    confirmPassword: ''
  },
  countdown: 0
});

var methods = {
  apiSendSms: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlSendSms, {
      mobile: this.form.mobile
    }).then(function (response) {
      var res = response.data;

      utils.notifySuccess('验证码发送成功，10分钟内有效');
      $this.countdown = 60;
      var interval = setInterval(function () {
        $this.countdown -= 1;
        if ($this.countdown <= 0){
          clearInterval(interval);
        }
      }, 1000);
    }).catch(function (error) {
      utils.notifyError(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, {
      mobile: this.form.mobile,
      code: this.form.code,
      password: this.form.password
    }).then(function (response) {
      var res = response.data;

      utils.alertSuccess({
        title: "恭喜，密码重置成功",
        button: "进入登录页",
        callback: function() {
          location.href = utils.getRootUrl('login');
        }
      });
    }).catch(function (error) {
      utils.notifyError(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  isMobile: function (value) {
    return /^1[3|4|5|7|8][0-9]\d{8}$/.test(value);
  },

  btnSendSmsClick: function () {
    if (this.countdown > 0) return;
    if (!this.form.mobile) {
      utils.notifyError('手机号码不能为空');
      return;
    } else if (!this.isMobile(this.form.mobile)) {
      utils.notifyError('请输入有效的手机号码');
      return;
    }

    this.apiSendSms();
  },

  btnLoginClick: function() {
    location.href = utils.getRootUrl('login');
  },

  btnSubmitClick: function () {
    var $this = this;

    this.$refs.form.validate(function(valid) {
      if (valid) {
        $this.apiSubmit();
      }
    });
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
    utils.loading(this, false);
  }
});