var $url = '/profile';
var $urlUpload = '/profile/actions/upload'
var $urlSendSms = '/profile/actions/sendSms';
var $urlVerifyMobile = '/profile/actions/verifyMobile';

var data = utils.init({
  uploadUrl: null,
  uploadFileList: [],
  isSmsEnabled: false,
  isUserVerifyMobile: false,
  user: null,
  form: null,
  styles: null,
  mobileValidateRules: null,
  countdown: 0
});

var methods = {
  apiGet: function () {
    var $this = this;

    $api.get($url).then(function (response) {
      var res = response.data;

      $this.isSmsEnabled = res.isSmsEnabled;
      $this.isUserVerifyMobile = res.isUserVerifyMobile;
      $this.user = res.user;
      $this.form = _.assign({}, res.user, {
        code: ''
      });
      $this.styles = res.styles;

      if ($this.isUserVerifyMobile) {
        $this.mobileValidateRules = [
          { required: true, message: '请输入手机号码' },
          { validator: utils.validateMobile, message: '请输入有效的手机号码' }
        ];
      } else {
        $this.mobileValidateRules = [
          { validator: utils.validateMobile, message: '请输入有效的手机号码' }
        ];
      }

      if ($this.form.avatarUrl) {
        $this.uploadFileList.push({name: 'avatar', url: $this.form.avatarUrl});
      }
    }).catch(function (error) {
      utils.error(error);
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

  apiVerifyMobile: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlVerifyMobile, {
      mobile: this.form.mobile,
      code: this.form.code
    }).then(function (response) {
      $this.form.mobileVerified = true;
      $this.apiSubmit();
    }).catch(function (error) {
      utils.loading($this, false);
      utils.notifyError(error);
    });
  },

  apiSubmit: function () {
    var $this = this;

    var payload = _.assign({}, this.form);
    delete payload.code;

    utils.loading(this, true);
    $api.post($url, this.form).then(function (response) {
      utils.success('资料修改成功！');
    }).catch(function (error) {
      utils.error(error);
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

  btnSubmitClick: function () {
    var $this = this;

    this.$refs.form.validate(function(valid) {
      if (valid) {
        if ($this.isMobileCode) {
          $this.apiVerifyMobile();
        } else {
          $this.apiSubmit();
        }
      }
    });
  },

  uploadBefore(file) {
    var re = /(\.jpg|\.jpeg|\.bmp|\.gif|\.png|\.webp)$/i;
    if(!re.exec(file.name))
    {
      utils.error('头像只能是图片格式，请选择有效的文件上传!');
      return false;
    }

    var isLt10M = file.size / 1024 / 1024 < 10;
    if (!isLt10M) {
      utils.error('头像图片大小不能超过 10MB!');
      return false;
    }
    return true;
  },

  uploadProgress: function() {
    utils.loading(this, true);
  },

  uploadSuccess: function(res, file, fileList) {
    this.form.avatarUrl = res.value;
    utils.loading(this, false);
    if (fileList.length > 1) fileList.splice(0, 1);
  },

  uploadError: function(err) {
    utils.loading(this, false);
    var error = JSON.parse(err.message);
    utils.error(error.message);
  },

  uploadRemove(file) {
    this.form.avatarUrl = null;
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  computed: {
    isMobileCode: function () {
      if (this.isUserVerifyMobile) {
        return !this.user.mobileVerified || this.user.mobile !== this.form.mobile;
      } else if (this.isSmsEnabled) {
        return this.user.mobile !== this.form.mobile;
      }
      return false;
    }
  },
  created: function () {
    this.uploadUrl = $apiUrl + $urlUpload;
    this.apiGet();
  }
});