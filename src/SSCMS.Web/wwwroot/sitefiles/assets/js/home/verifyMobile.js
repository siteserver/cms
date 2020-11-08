var $url = '/verifyMobile';
var $urlSendSms = '/verifyMobile/actions/sendSms';

var data = utils.init({
  countdown: 0,
  form: {
    mobile: null,
    code: null
  },
  returnUrl: utils.getQueryString('returnUrl')
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url).then(function (response) {
      var res = response.data;

      $this.form.mobile = res.mobile;
    }).catch(function (error) {
      utils.notifyError(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

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
      code: this.form.code
    }).then(function (response) {
      var res = response.data;

      utils.alertSuccess({
        title: "手机号码验证成功",
        button: "确 定",
        callback: function() {
          if ($this.returnUrl) {
            location.href = $this.returnUrl;
          } else {
            $this.redirectIndex();
          }
        }
      });
    }).catch(function (error) {
      utils.notifyError(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  redirectIndex: function () {
    location.href = utils.getIndexUrl();
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

  btnSubmitClick: function () {
    var $this = this;

    this.$refs.form.validate(function(valid) {
      if (valid) {
        $this.apiSubmit();
      }
    });
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
    this.apiGet();
  }
});
