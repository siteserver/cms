var $url = '/settings/usersLayerProfile';

var data = utils.init({
  userId: utils.getQueryInt('userId'),
  uploadUrl: null,
  uploadFileList: [],
  form: null,
  groups: null,
  styles: null
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

      $this.form = _.assign({}, res.user);
      $this.groups = res.groups;
      $this.styles = res.styles;
      if (this.userId === 0) {
        for (var i = 0; i < res.styles.length; i++) {
          var style = res.styles[i];
          $this.form[utils.toCamelCase(style.attributeName)] = style.defaultValue;
        }
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

  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, this.form).then(function (response) {
      utils.success($this.form.id > 0 ? '用户编辑成功！' : '用户添加成功！');
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
    this.uploadUrl = $apiUrl + $url + '/actions/upload?userId=' + this.userId;
    this.apiGet();
  }
});