var $url = '/settings/administratorsLayerProfile';

var data = utils.init({
  userName: utils.getQueryString('userName'),
  userId: 0,
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
        userName: this.userName
      }
    }).then(function (response) {
      var res = response.data;

      $this.userId = res.userId;
      $this.form.userName = res.userName;
      $this.form.displayName = res.displayName;
      $this.form.avatarUrl = res.avatarUrl;
      $this.form.mobile = res.mobile;
      $this.form.email = res.email;

      if ($this.form.avatarUrl) {
        $this.uploadFileList.push({name: 'avatar', url: $this.form.avatarUrl});
      }

    }).catch(function (error) {
      utils.error(error);
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
      if ($this.userName) {
        utils.success('管理员编辑成功！');
      } else {
        utils.success('管理员新增成功！');
      }

      utils.closeLayer(true);
    }).catch(function (error) {
      utils.error(error);
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
    this.uploadUrl = $apiUrl + $url + '/actions/upload?userName=' + this.userName;
    if (this.userName) {
      this.apiGet();
    } else {
      utils.loading(this, false);
    }
  }
});