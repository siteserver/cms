var $url = "/wx/replyAdd";

var data = utils.init({
  siteId: utils.getQueryInt("siteId"),
  ruleId: utils.getQueryInt("ruleId"),
  tabName: utils.getQueryString("tabName"),
  success: false,
  errorMessage: null,
  form: {
    ruleName: null,
    random: true,
    messages: null
  },
  keywords: [],
  messages: []
});

var methods = {
  runLayerMessage: function(message) {
    this.messages.push({
      materialType: 'Message',
      materialId: message.id,
      items: message.items,
      lastModifiedDate: message.lastModifiedDate
    });
    this.form.messages = true;
  },

  runLayerText: function(text, index) {
    if (index) {
      this.messages[parseInt(index)].text = text;
    } else {
      this.messages.push({
        materialType: 'Text',
        text: text
      });
    }
    
    this.form.messages = true;
  },

  runLayerImage: function(image) {
    this.messages.push({
      materialType: 'Image',
      materialId: image.id,
      image: image
    });
    this.form.messages = true;
  },

  runLayerAudio: function(audio) {
    this.messages.push({
      materialType: 'Audio',
      materialId: audio.id,
      audio: audio
    });
    this.form.messages = true;
  },

  runLayerVideo: function(video) {
    this.messages.push({
      materialType: 'Video',
      materialId: video.id,
      video: video
    });
    this.form.messages = true;
  },
  
  apiGet: function() {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId,
        ruleId: this.ruleId
      }
    }).then(function(response) {
      var res = response.data;
      
      $this.success = res.success;
      $this.errorMessage = res.errorMessage;
      $this.form.ruleName = res.ruleName;
      $this.form.random = res.random;
      $this.keywords = res.keywords || [{
        exact: false,
        text: null
      }];
      $this.messages = res.messages || [];
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
    $api.post($url, {
      siteId: this.siteId,
      ruleId: this.ruleId,
      ruleName: this.form.ruleName,
      random: this.form.random,
      keywords: this.keywords,
      messages: this.messages
    }).then(function(response) {
      var res = response.data;
      
      var vue = utils.getTabVue($this.tabName);
      if (vue) {
        vue.apiGet(1);
      }
      utils.success('关键词回复保存成功！');
      utils.removeTab();
    })
    .catch(function(error) {
      utils.error(error);
    })
    .then(function() {
      utils.loading($this, false);
    });
  },

  btnSelectClick: function(key, index) {
    utils.openLayer({
      title: '回复消息',
      url: utils.getWxUrl("layer" + key, {
        siteId: this.siteId,
        index: index || ''
       })
    });
  },

  btnAppendClick: function() {
    this.keywords.push({
      id: 0,
      exact: false,
      text: null
    });
  },

  btnRemoveKeywordClick: function(index) {
    this.keywords.splice(index, 1);
  },

  btnRemoveMessageClick: function(index) {
    this.messages.splice(index, 1);
    this.form.messages = this.messages.length === 0 ? null : true;
  },

  btnSubmitClick: function () {
    var $this = this;

    var forms = [this.$refs.rule1, this.$refs.rule2, this.$refs.rule3];
    this.keywords.forEach(function(x, index){
      forms.push($this.$refs.keywords[index]);
    });

    var validAll = true;
    forms.forEach(function(x){
      x.validate(function(valid) {
        validAll = validAll && valid;
      });
    });

    if (validAll) {
      this.apiSubmit();
    }
  },

  btnCloseClick: function() {
    utils.removeTab();
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
