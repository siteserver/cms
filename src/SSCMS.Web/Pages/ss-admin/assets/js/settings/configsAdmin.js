var $url = '/admin/settings/configsAdmin';

var data = utils.initData({
  uploadUrl: null,
  uploadFileList: [],
  form: {
    adminTitle: null,
    adminLogoUrl: null,
    adminWelcomeHtml: null,
  }
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url).then(function (response) {
      var res = response.data;

      $this.form.adminTitle = res.adminTitle;
      $this.form.adminLogoUrl = res.adminLogoUrl;
      $this.form.adminWelcomeHtml = res.adminWelcomeHtml || '欢迎使用 SS CMS 管理后台';

      if ($this.form.adminLogoUrl) {
        $this.uploadFileList.push({name: 'avatar', url: $this.form.adminLogoUrl});
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
      adminTitle: $this.form.adminTitle,
      adminLogoUrl: $this.form.adminLogoUrl,
      adminWelcomeHtml: $this.form.adminWelcomeHtml
    }).then(function (response) {
      var res = response.data;

      $this.$message.success('管理后台设置保存成功！');
    }).catch(function (error) {
      utils.error($this, error);
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

  uploadBefore(file) {
    var re = /(\.jpg|\.jpeg|\.bmp|\.gif|\.png|\.webp)$/i;
    if(!re.exec(file.name))
    {
      this.$message.error('管理后台Logo只能是图片格式，请选择有效的文件上传!');
      return false;
    }

    var isLt10M = file.size / 1024 / 1024 < 10;
    if (!isLt10M) {
      this.$message.error('管理后台Logo图片大小不能超过 10MB!');
      return false;
    }
    return true;
  },

  uploadProgress: function() {
    utils.loading(this, true);
  },

  uploadSuccess: function(res, file, fileList) {
    this.form.adminLogoUrl = res.value;
    utils.loading(this, false);
    if (fileList.length > 1) fileList.splice(0, 1);
  },

  uploadError: function(err) {
    utils.loading(this, false);
    var error = JSON.parse(err.message);
    this.$message.error(error.message);
  },

  uploadRemove(file) {
    this.form.adminLogoUrl = null;
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.uploadUrl = $apiUrl + $url + '/actions/upload';
    this.apiGet();
  }
});