var $url = "/clouds/mail"

var data = utils.init({
  form: {
    isCloudMail: false,
    cloudMailFromAlias: '',
    isCloudMailContentAdd: false,
    isCloudMailContentEdit: false,
    cloudMailAddress: '',
  }
});

var methods = {
  apiGet: function() {
    var $this = this;

    utils.loading(this, true);
    $api.get($url).then(function (response) {
      var res = response.data;

      $this.form.isCloudMail = res.isCloudMail;
      $this.form.cloudMailFromAlias = res.cloudMailFromAlias;
      $this.form.isCloudMailContentAdd = res.isCloudMailContentAdd;
      $this.form.isCloudMailContentEdit = res.isCloudMailContentEdit;
      $this.form.cloudMailAddress = res.cloudMailAddress;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, this.form).then(function (response) {
      var res = response.data;

      utils.success('邮件集成设置保存成功！');
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnSubmitClick: function () {
    var $this = this;
    this.$refs.form.validate(function(valid) {
      if (valid) {
        $this.apiSubmit();
      }
    });
  },

  validateEmail: function (rule, value, callback) {
    if (!value) {
      callback(new Error('请输入通知邮箱'));
    } else {
      var re = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
      if (!re.test(value))
      {
        callback(new Error('通知邮箱格式不正确'));
      } else {
        callback();
      }
    }
  },

  btnCloseClick: function() {
    utils.removeTab();
  },
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    utils.keyPress(this.btnSubmitClick, this.btnCloseClick);
    var $this = this;
    cloud.checkAuth(function() {
      $this.apiGet();
    });
  }
});
