var $url = "/wx/chatSend";
var $urlUpload = "/wx/chatSend/actions/upload";

var data = utils.init({
  success: false,
  errorMessage: null,
  user: null,
  chats: null,
  message: null,
  image: null,
  audio: null,
  video: null,
  form: {
    siteId: utils.getQueryInt("siteId"),
    openId: utils.getQueryString("openId"),
    materialType: 'Text',
    materialId: 0,
    text: null
  }
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
        siteId: this.form.siteId,
        openId: this.form.openId
      }
    }).then(function(response) {
      var res = response.data;
      
      $this.success = res.success;
      $this.errorMessage = res.errorMessage;
      $this.user = res.user;
      $this.chats = res.chats;
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
      
      $this.form.materialType = 'Text';
      $this.form.materialId = 0;
      $this.form.text = null;
      $this.chats = res.chats;
      utils.success('消息回复成功!');
    })
    .catch(function(error) {
      utils.error(error);
    })
    .then(function() {
      utils.loading($this, false);
    });
  },

  btnTabClick: function(tab) {
    this.form.materialType = tab.name;
  },

  btnSelectClick: function() {
    utils.openLayer({
      title: '选择素材',
      url: utils.getWxUrl("layer" + this.form.materialType, {
        siteId: this.form.siteId
       })
    });
  },

  tableRowClassName: function(scope) {
    if (scope.row.isReply) {
      return 'reply-row';
    }
    return '';
  },

  getUserTitle: function() {
    if (this.user.remark) return this.user.remark + '(' + this.user.nickname + ')';
    return this.user.nickname;
  },

  isSubmit: function() {
    if (this.form.materialType === 'Text') return this.form.text && this.form.text.length > 0;
    return this.form.materialId > 0;
  },

  btnSubmitClick: function () {
    if (!this.isSubmit()) {
      utils.error('请选择或输入需要发送的消息!');
      return;
    }
    
    this.apiSubmit();
  },

  btnCloseClick: function() {
    utils.removeTab();
  },

  btnRemoveClick: function() {
    this.form.materialId = 0;
  },

  getUploadUrl: function() {
    return $apiUrl + $urlUpload + '?siteId=' + this.form.siteId + '&materialType=' + this.form.materialType
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
  methods: methods,
  created: function() {
    this.apiGet();
  }
});
