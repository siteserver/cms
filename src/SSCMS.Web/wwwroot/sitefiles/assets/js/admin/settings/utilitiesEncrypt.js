var $url = '/settings/utilitiesEncrypt';
var $urlRotateKey = '/settings/utilitiesEncrypt/actions/rotateKey';

var data = utils.init({
  form: {
    isEncrypt: true,
    value: null,
  },
  results: null,
  rotateForm: {
    password: ''
  }
});

var methods = {
  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, {
      isEncrypt: this.form.isEncrypt,
      value: this.form.value
    }).then(function (response) {
      var res = response.data;

      $this.results = res.value;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiRotateKey: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlRotateKey, {
      password: this.rotateForm.password
    }).then(function () {
      $this.rotateForm.password = '';
      utils.alertSuccess({
        title: 'SecurityKey 已轮换',
        text: '系统正在重启，请稍后重新登录，并执行“生成全站页面”。',
        callback: function () {
          window.top.location.href = utils.getRootUrl('login');
        }
      });
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  radioChanged: function() {
    this.results = '';
  },

  btnSubmitClick: function () {
    var $this = this;

    this.$refs.form.validate(function(valid) {
      if (valid) {
        $this.apiSubmit();
      }
    });
  },

  btnRotateKeyClick: function () {
    var $this = this;

    this.$refs.rotateForm.validate(function(valid) {
      if (!valid) return;
      $this.$confirm('确定要轮换 SecurityKey 吗？系统将重启，所有用户需重新登录。', '轮换 SecurityKey', {
        confirmButtonText: '确定轮换',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(function () {
        $this.apiRotateKey();
      }).catch(function () {});
    });
  },

  btnCloseClick: function() {
    utils.removeTab();
  },
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    utils.keyPress(this.btnSubmitClick, this.btnCloseClick);
    utils.loading(this, false);
  }
});
