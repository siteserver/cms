var $url = '/wx/replyMessage';
var $urlUpload = "/wx/replyMessage/actions/upload";

var data = utils.init({
  siteId: utils.getQueryInt('siteId'),
  activeName: utils.getQueryString('activeName'),
  success: false,
  errorMessage: null,
  form: {
    id: 0,
    materialType: 'Text',
    materialId: 0,
    text: ''
  },
  items: null,
  image: null,
  audio: null,
  video: null,
  editEditor: null,
});

var methods = {
  runLayerMessage: function(message) {
    this.form.materialId = 0;
    if (message) {
      this.form.materialId = message.id;
      this.items = message.items;
    }
  },

  runLayerImage: function(image) {
    this.form.materialId = 0;
    if (image) {
      this.form.materialId = image.id;
      this.image = image;
    }
  },

  runLayerAudio: function(audio) {
    this.form.materialId = 0;
    if (audio) {
      this.form.materialId = audio.id;
      this.audio = audio;
    }
  },

  runLayerVideo: function(video) {
    this.form.materialId = 0;
    if (video) {
      this.form.materialId = video.id;
      this.video = video;
    }
  },

  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId,
        activeName: this.activeName
      }
    }).then(function (response) {
      var res = response.data;

      $this.success = res.success;
      $this.errorMessage = res.errorMessage;
      if (res.message) {
        $this.form = res.message;
        $this.items = res.message.items;
        $this.image = res.message.image;
        $this.audio = res.message.audio;
        $this.video = res.message.video;
      }
      
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function() {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, {
      siteId: this.siteId,
      activeName: this.activeName,
      materialType: this.form.materialType,
      materialId: this.form.materialId,
      text: this.form.text,
    }).then(function(response) {
      var res = response.data;
      
      $this.form.id = res.value;
      utils.success('自动回复消息保存成功！');
    })
    .catch(function(error) {
      utils.error(error);
    })
    .then(function() {
      utils.loading($this, false);
    });
  },

  apiDelete: function () {
    var $this = this;

    utils.loading(this, true);
    $api.delete($url, {
      data: {
        siteId: this.siteId,
        activeName: this.activeName
      }
    }).then(function (response) {
      var res = response.data;

      $this.form = {
        id: 0,
        materialType: 'Text',
        materialId: 0,
        text: ''
      };
      $this.items = null;
      $this.image = null;
      $this.audio = null;
      $this.video = null;
      $this.editEditor.txt.html('');

    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnTabClick: function(tab) {
    this.form.materialType = tab.name;
    if (tab.name === 'Message') {
      this.runLayerMessage(this.message);
    } else if (tab.name === 'Image') {
      this.runLayerImage(this.image);
    } else if (tab.name === 'Audio') {
      this.runLayerAudio(this.audio);
    } else if (tab.name === 'Video') {
      this.runLayerVideo(this.video);
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
    var error = JSON.parse(err.form);
    utils.error(error.form);
  },

  btnRemoveClick: function() {
    this.form.materialId = 0;
  },

  btnSubmitClick: function () {
    var isSubmit = false;
    if (this.form.materialType === 'Text') {
      if (this.form.text && this.form.text.length > 0) {
        isSubmit = true;
      } else {
        utils.error('请输入需要回复的消息!');
      }
    } else {
      if (this.form.materialId > 0) {
        isSubmit = true;
      } else {
        utils.error('请选择或上传需要回复的消息!');
      }
    }

    if (isSubmit) {
      this.apiSubmit();
    }
  },

  btnDeleteClick: function() {
    var $this = this;

    utils.alertDelete({
      title: '删除回复',
      text: '删除后，关注该公众号的用户将不再接收该回复，确定删除？',
      callback: function () {
        $this.apiDelete();
      }
    });
  },
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiGet();
  }
});