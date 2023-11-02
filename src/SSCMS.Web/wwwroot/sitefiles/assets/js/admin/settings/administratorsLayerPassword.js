var $url = '/settings/administratorsLayerPassword';

var data = utils.init({
  userName: utils.getQueryString('userName'),
  isEnforcePasswordChange: utils.getQueryBoolean('isEnforcePasswordChange'),
  administrator: null,
  oldPassword: false,
  form: {
    oldPassword: null,
    password: null,
    confirmPassword: null
  }
});

var methods = {
  apiGet: function () {
    var $this = this;

    $api.get($url, {
      params: {
        userName: this.userName
      }
    }).then(function (response) {
      var res = response.data;

      $this.administrator = res.administrator;
      $this.oldPassword = res.oldPassword;

      if (!$this.userName) {
        $this.$message({
          type: 'warning',
          message: '您的密码已过期，请更改登录密码',
          showClose: true,
          duration: 0
        });
      }
    }).catch(function (error) {
      utils.error(error, {layer: true});
    }).then(function () {
      utils.loading($this, false);
    });
  },

  toBase64: function(text) {
    var bytes = new TextEncoder().encode(text);
    var binString = Array.from(bytes, function (x) { return String.fromCodePoint(x) }).join("");
    return btoa(binString);
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, {
      userName: this.userName,
      oldPassword: this.form.oldPassword ? this.toBase64(this.form.oldPassword) : '',
      password: this.form.password ? this.toBase64(this.form.password) : '',
    }).then(function (response) {
      var res = response.data;

      utils.success('密码更改成功！', {layer: true});

      setTimeout(function () {
        if ($this.isEnforcePasswordChange) {
          window.top.location = utils.getIndexUrl();
        } else {
          utils.closeLayer();
        }
      }, 1000);
    }).catch(function (error) {
      utils.error(error, {layer: true});
    }).then(function () {
      utils.loading($this, false);
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
  },

  btnSubmitClick: function() {
    var $this = this;

    this.$refs.form.validate(function(valid) {
      if (valid) {
        $this.apiSubmit();
      }
    });
  },

  btnCancelClick: function () {
    utils.closeLayer();
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    utils.keyPress(this.btnSubmitClick, this.btnCancelClick);
    this.apiGet();
  }
});
