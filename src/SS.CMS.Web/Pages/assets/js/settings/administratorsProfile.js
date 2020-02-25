var $url = '/admin/settings/administratorsProfile';
var $pageTypeAdmin = 'admin';
var $pageTypeUser = 'user';

var data = utils.initData({
  pageType: utils.getQueryString('pageType'),
  userId: utils.getQueryInt('userId'),
  uploadUrl: null,
  uploadFileList: [],
  form: {
    userName: null,
    displayName: null,
    password: null,
    confirmPassword: null,
    avatarUrl: null,
    mobile: null,
    email: null
  }
});

var methods = {
  apiGet: function () {
    var $this = this;

    $api.get($url, {
      params: {
        userId: this.userId
      }
    }).then(function (response) {
      var res = response.data;

      $this.form.userName = res.userName;
      $this.form.displayName = res.displayName;
      $this.form.avatarUrl = res.avatarUrl;
      $this.form.mobile = res.mobile;
      $this.form.email = res.email;

      if ($this.form.avatarUrl) {
        $this.uploadFileList.push({name: 'avatar', url: $this.form.avatarUrl});
      }

    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, {
      userId: this.userId,
      userName: this.form.userName,
      displayName: this.form.displayName,
      password: this.form.password,
      avatarUrl: this.form.avatarUrl,
      mobile: this.form.mobile,
      email: this.form.email
    }).then(function (response) {
      $this.$message.success('管理员资料保存成功！');

      setTimeout(function () {
        if ($this.pageType == $pageTypeAdmin) {
          location.href = utils.getSettingsUrl('administrators');
        } else {
          top.location.href = utils.getIndexUrl();
        }
      }, 3000);
    }).catch(function (error) {
      utils.error($this, error);
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

  btnSubmitClick: function () {
    var $this = this;

    this.$refs.form.validate(function(valid) {
      if (valid) {
        $this.apiSubmit();
      }
    });
  },

  btnReturnClick: function () {
    location.href = 'admin.cshtml';
  },

  uploadBefore(file) {
    var re = /(\.jpg|\.jpeg|\.bmp|\.gif|\.png|\.webp)$/i;
    if(!re.exec(file.name))
    {
      this.$message.error('头像只能是图片格式，请选择有效的文件上传!');
      return false;
    }

    var isLt10M = file.size / 1024 / 1024 < 10;
    if (!isLt10M) {
      this.$message.error('头像图片大小不能超过 10MB!');
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
    this.$message.error(error.message);
  },

  uploadRemove(file) {
    this.form.avatarUrl = null;
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.uploadUrl = $apiUrl + $url + '/actions/upload?userId=' + this.userId;
    if (this.userId > 0) {
      this.apiGet();
    } else {
      utils.loading(this, false);
    }
  }
});