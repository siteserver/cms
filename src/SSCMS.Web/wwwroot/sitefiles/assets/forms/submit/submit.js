$apiUrl = utils.getQueryString('apiUrl') || $formConfigApiUrl;
$rootUrl = "/";
$token = localStorage.getItem(ACCESS_TOKEN_NAME);

var $api = axios.create({
  baseURL: $apiUrl,
  headers: {
    Authorization: "Bearer " + $token,
  },
});

var $url = '/v1/forms';
var $urlStyles = '/v1/forms/styles';
var $urlSendSms = '/v1/forms/actions/sendSms';
var $urlUpload = '/v1/forms/actions/upload';

var data = utils.init({
  siteId: utils.getQueryInt('siteId') || $formConfigSiteId,
  channelId: utils.getQueryInt('channelId') || $formConfigChannelId,
  contentId: utils.getQueryInt('contentId') || $formConfigContentId,
  formId: utils.getQueryInt('formId') || $formConfigFormId,
  pageType: '',
  siteUrl: null,
  styles: [],
  title: '',
  description: '',
  successMessage: '',
  successCallback: '',
  isSms: false,
  smsCountdown: 0,
  isCaptcha: false,
  captcha: '',
  captchaValue: '',
  captchaUrl: null,
  uploadUrl: null,
  form: null,
  uploadImageUrl: null,
  uploadImageList: [],
});

var methods = {
  runFormLayerImageUploadText: function(attributeName, no, text) {
    this.insertText(attributeName, no, text);
  },

  insertText: function(attributeName, no, text) {
    var count = this.form[utils.getCountName(attributeName)] || 0;
    if (count <= no) {
      this.form[utils.getCountName(attributeName)] = no;
    }
    this.form[utils.getExtendName(attributeName, no)] = text;
    this.form = _.assign({}, this.form);
  },

  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($urlStyles, {
      params: {
        siteId: this.siteId,
        formId: this.formId,
      }
    }).then(function (response) {
      var res = response.data;

      $this.title = res.title;
      $this.description = res.description;
      $this.successMessage = res.successMessage;
      $this.successCallback = res.successCallback;
      $this.isSms = res.isSms;
      $this.isCaptcha = res.isCaptcha;
      if ($this.isCaptcha) {
        $this.apiCaptchaLoad();
      }
      $this.siteUrl = res.siteUrl;
      $this.styles = res.styles;
      $this.form = utils.getForm(res.styles, _.assign({}, res.formData, {
        captcha: '',
        smsMobile: '',
        smsCode: ''
      }));
      $this.pageType = 'form';
      $this.uploadUrl = $apiUrl + $urlUpload + '?siteId=' + $this.siteId + '&formId=' + $this.formId;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSendSms: function () {
    var $this = this;

    utils.loading(this, true);
    var urlSendSms = $urlSendSms + '?siteId=' + this.siteId + '&formId=' + this.formId;
    $api.post(urlSendSms, {
      mobile: this.form.smsMobile
    }).then(function (response) {
      var res = response.data;

      utils.notifySuccess('验证码发送成功，10分钟内有效');
      $this.smsCountdown = 60;
      var interval = setInterval(function () {
        $this.smsCountdown -= 1;
        if ($this.smsCountdown <= 0){
          clearInterval(interval);
        }
      }, 1000);
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiCaptchaLoad: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post('/v1/captcha').then(function (response) {
      var res = response.data;

      $this.captchaValue = res.value;
      $this.captchaUrl = $apiUrl + '/v1/captcha/' + res.value;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiCaptchaCheck: function () {
    var $this = this;

    $api.post('/v1/captcha/actions/check', {
      captcha: this.form.captcha,
      value: this.captchaValue
    }).then(function (res) {
      $this.apiSubmit();
    })
    .catch(function (error) {
      utils.error(error);
    });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(true);
    var urlPost = $url + '?siteId=' + this.siteId + '&channelId=' + this.channelId + '&contentId=' + this.contentId + '&formId=' + this.formId;
    $api.post(urlPost, _.assign({}, this.form)).then(function (response) {
      var res = response.data;

      $this.pageType = 'success';
      if ($this.successCallback) {
        var callback = $this.successCallback;
        if (callback.indexOf('parent.') === -1) {
          callback = 'parent.' + callback;
        }
        if (callback.indexOf('(') === -1) {
          callback += '()';
        }
        eval(callback);
      }
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  getUploadUrl: function(style) {
    return this.uploadUrl + '&attributeName=' + style.attributeName;
  },

  isMobile: function (value) {
    return /^1[3-9]\d{9}$/.test(value);
  },

  btnSendSmsClick: function () {
    if (this.smsCountdown > 0) return;
    if (!this.form.smsMobile) {
      utils.error('手机号码不能为空');
      return;
    } else if (!this.isMobile(this.form.smsMobile)) {
      utils.error('请输入有效的手机号码');
      return;
    }

    this.apiSendSms();
  },

  btnSubmitClick: function () {
    var $this = this;
    this.$refs.form.validate(function(valid) {
      if (valid) {
        if ($this.isCaptcha) {
          $this.apiCaptchaCheck();
        } else {
          $this.apiSubmit();
        }
      } else {
        utils.scrollToError();
      }
    });
  },

  uploadImageBefore(file) {
    var re = /(\.jpg|\.jpeg|\.bmp|\.gif|\.png|\.webp)$/i;
    if(!re.exec(file.name))
    {
      utils.error('图片格式不正确，请选择有效的文件上传!');
      return false;
    }

    var isLt10M = file.size / 1024 / 1024 < 10;
    if (!isLt10M) {
      utils.error('图片大小不能超过 10MB!');
      return false;
    }
    return true;
  },

  uploadProgress: function() {
    utils.loading(this, true);
  },

  uploadSuccess: function(res, file, fileList) {
    var attributeName = utils.toCamelCase(res.attributeName);
    this.form[attributeName] = res.virtualUrl;
    utils.loading(this, false);
    if (fileList.length > 1) fileList.splice(0, 1);
  },

  uploadError: function(err) {
    utils.loading(this, false);
    var error = JSON.parse(err.message);
    utils.error(error.message);
  },

  uploadRemove(file, fileList) {
    var res = file.response;
    var attributeName = utils.toCamelCase(res.attributeName);
    this.form[attributeName] = '';
  },
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    this.apiGet();
  }
});
