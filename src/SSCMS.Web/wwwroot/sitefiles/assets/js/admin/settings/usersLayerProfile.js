var $url = '/settings/usersLayerProfile';

var data = utils.init({
  userId: utils.getQueryInt('userId'),
  uploadUrl: null,
  uploadFileList: [],
  form: null,
  groups: null,
  styles: null,
  settings: null,
});

var methods = {
  runFormLayerImageUploadText: function(attributeName, no, text) {
    this.insertText(attributeName, no, text);
  },

  runMaterialLayerImageSelect: function(attributeName, no, text) {
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

    $api.get($url, {
      params: {
        userId: this.userId
      }
    }).then(function (response) {
      var res = response.data;

      $this.form = _.assign({}, res.user);
      $this.groups = res.groups;
      $this.styles = res.styles;
      $this.settings = res.settings;
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

  btnImageSelectClick: function(args) {
    var attributeName = args.attributeName;
    var no = args.no;
    var type = args.type;

    if (type === 'materialImages') {
      this.btnLayerClick({
        title: '选择素材库图片',
        name: 'materialLayerImageSelect',
        attributeName: attributeName,
        no: no,
        full: true
      });
    } else if (type === 'cloudImages') {
      utils.openLayer({
        title: '选择免版权图库',
        url: utils.getCloudsUrl('layerImagesSelect', {
          attributeName: args.attributeName,
          no: args.no,
        }),
      });
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
  },

  btnLayerClick: function(options) {
    var query = {
      userId: this.userId,
    };

    if (options.attributeName) {
      query.attributeName = options.attributeName;
    }
    if (options.no) {
      query.no = options.no;
    }

    var args = {
      title: options.title,
      url: utils.getCommonUrl(options.name, query)
    };
    if (!options.full) {
      args.width = options.width ? options.width : 700;
      args.height = options.height ? options.height : 500;
    }

    utils.openLayer(args);
  },

  btnExtendAddClick: function(style) {
    var no = this.form[utils.getCountName(style.attributeName)] + 1;
    this.form[utils.getCountName(style.attributeName)] = no;
    this.form[utils.getExtendName(style.attributeName, no)] = '';
    this.form = _.assign({}, this.form);
  },

  btnExtendRemoveClick: function(style) {
    var no = this.form[utils.getCountName(style.attributeName)];
    this.form[utils.getCountName(style.attributeName)] = no - 1;
    this.form[utils.getExtendName(style.attributeName, no)] = '';
    this.form = _.assign({}, this.form);
  },

  btnExtendPreviewClick: function(attributeName, no) {
    var count = this.form[utils.getCountName(attributeName)];
    var data = [];
    for (var i = 0; i <= count; i++) {
      var imageUrl = this.form[utils.getExtendName(attributeName, i)];
      imageUrl = utils.getUrl(this.siteUrl, imageUrl);
      data.push({
        "src": imageUrl
      });
    }
    layer.photos({
      photos: {
        "start": no,
        "data": data
      }
      ,anim: 5
    });
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    utils.keyPress(this.btnSubmitClick, this.btnCancelClick);
    this.uploadUrl = $apiUrl + $url + '/actions/upload?userId=' + this.userId;
    this.apiGet();
  }
});
