var $url = "/wx/send";
var $urlUpload = "/wx/send/actions/upload";
var $urlPreview = "/wx/send/actions/preview";

var data = utils.init({
  siteId: utils.getQueryInt("siteId"),
  messageId: utils.getQueryInt("messageId"),
  previewDialog: false,
  previewForm: {
    wxNames: null
  },
  success: false,
  errorMessage: null,
  tags: null,
  message: null,
  image: null,
  audio: null,
  video: null,
  form: {
    siteId: utils.getQueryInt("siteId"),
    materialType: 'Message',
    materialId: 0,
    text: null,
    isToAll: true,
    tagId: 0,
    isTiming: false,
    isToday: true,
    hour: 0,
    minute: 0
  },
  hours: [],
  minutes: [],
  editEditor: null,
  sended: false
});

var methods = {
  runLayerMessage: function(message) {
    this.form.materialId = message.id;
    this.message = message;
  },

  runLayerImage: function(image) {
    this.form.materialId = image.id;
    this.image = image;
  },

  runLayerAudio: function(audio) {
    this.form.materialId = audio.id;
    this.audio = audio;
  },

  runLayerVideo: function(video) {
    this.form.materialId = video.id;
    this.video = video;
  },
  
  apiGet: function() {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId,
        messageId: this.messageId
      }
    }).then(function(response) {
      var res = response.data;
      
      $this.success = res.success;
      $this.errorMessage = res.errorMessage;
      $this.tags = res.tags;
      $this.message = res.message;
      $this.form.materialId = $this.messageId;
    })
    .catch(function(error) {
      utils.error(error);
    })
    .then(function() {
      utils.loading($this, false);
    });
  },

  apiSubmit: function() {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, this.form).then(function(response) {
      var res = response.data;
      
      $this.sended = true;
    })
    .catch(function(error) {
      utils.error(error);
    })
    .then(function() {
      utils.loading($this, false);
    });
  },

  apiPreview: function() {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlPreview, {
      siteId: this.siteId,
      materialType: this.form.materialType,
      materialId: this.form.materialId,
      text: this.form.text,
      WxNames: this.previewForm.wxNames
    })
    .then(function(response) {
      var res = response.data;

      $this.previewDialog = false;
      utils.success('预览图文消息已发送至微信，请进入微信查看！');
    })
    .catch(function(error) {
      utils.error(error);
    })
    .then(function() {
      utils.loading($this, false);
    });
  },

  loadEditor: function () {
    var $this = this;

    var E = window.wangEditor;
    this.editEditor = new E('#text1', '#text2');
    this.editEditor.customConfig.menus = [
      'head',  // 标题
      'bold',  // 粗体
      'fontSize',  // 字号
      'fontName',  // 字体
      'italic',  // 斜体
      'underline',  // 下划线
      'strikeThrough',  // 删除线
      'foreColor',  // 文字颜色
      'backColor',  // 背景颜色
      'link',  // 插入链接
      'list',  // 列表
      'justify',  // 对齐方式
      'quote',  // 引用
      'table',  // 表格
      'undo',  // 撤销
      'redo'  // 重复
    ];
    this.editEditor.customConfig.onchange = function (html) {
      $this.form.text = html;
    };
    this.editEditor.create();
    this.editEditor.txt.html(this.form.text);
  },

  btnTabClick: function(tab) {
    var $this = this;

    this.form.materialType = tab.name;
    if (this.form.materialType === 'Text') {
      setTimeout(function () {
        $this.loadEditor();
      }, 100);
    }
  },

  btnSelectClick: function() {
    utils.openLayer({
      title: '选择素材',
      url: utils.getWxUrl("layer" + this.form.materialType, {
        siteId: this.siteId
       })
    });
  },

  loadTiming: function() {
    var now = new Date();
    var currentHour = now.getHours();
    var currentMinute = now.getMinutes() + 5;
    if (this.form.isToday) {
      if (this.form.hour < currentHour) {
        this.form.hour = currentHour;
      }
      if (this.form.hour === currentHour && this.form.minute < currentMinute) {
        this.form.minute = currentMinute;
      }
    }

    this.hours = [];
    this.minutes = [];

    for (var i = 0; i < 24; i++) {
      if (this.form.isToday) {
        if (i < currentHour) continue;
      }
      this.hours.push({
        value: i,
        label: utils.pad(i)
      });
    }
    for (var i = 0; i < 60; i++) {
      if (this.form.isToday && this.form.hour === currentHour) {
        if (i < currentMinute) continue;
      }
      this.minutes.push({
        value: i,
        label: utils.pad(i)
      });
    }
  },

  isSubmit: function() {
    if (this.form.materialType === 'Text') return this.form.text && this.form.text.length > 0;
    return this.form.materialId > 0;
  },

  btnPreviewClick: function() {
    if (!this.isSubmit()) {
      utils.error('请选择或输入需要发送的消息!');
      return;
    }

    this.previewDialog = true;
  },

  btnSubmitClick: function () {
    if (!this.isSubmit()) {
      utils.error('请选择或输入需要发送的消息!');
      return;
    }
    
    var $this = this;

    this.$refs.form.validate(function(valid) {
      if (valid) {
        $this.apiSubmit();
      }
    });
  },

  btnPreviewSubmitClick: function() {
    var $this = this;

    this.$refs.previewForm.validate(function(valid) {
      if (valid) {
        $this.apiPreview();
      }
    });
  },

  btnRemoveClick: function() {
    this.form.materialId = 0;
  },

  getUploadUrl: function() {
    return $apiUrl + $urlUpload + '?siteId=' + this.siteId + '&materialType=' + this.form.materialType
  },

  uploadProgress: function() {
    utils.loading(this, true);
  },

  uploadSuccess: function(res) {
    if (this.form.materialType === 'Image') {
      this.form.materialId = res.image.id;
      this.image = res.image;
    } else if (this.form.materialType === 'Audio') {
      this.form.materialId = res.audio.id;
      this.audio = res.audio;
    } else if (this.form.materialType === 'Video') {
      this.form.materialId = res.video.id;
      this.video = res.video;
    }
    utils.loading(this, false);
  },

  uploadError: function(err) {
    utils.loading(this, false);
    var error = JSON.parse(err.message);
    utils.error(error.message);
  }
};

var $vue = new Vue({
  el: "#main",
  data: data,
  computed: {
    wxNamesCount: function (val, oldVal) {
      if (!this.previewForm.wxNames) return 1;
      return this.previewForm.wxNames.split('\n').length;
    }
  },
  methods: methods,
  created: function() {
    this.apiGet();
  }
});
