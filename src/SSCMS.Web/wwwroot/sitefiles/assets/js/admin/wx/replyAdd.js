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
    keywords: null,
    messages: null
  }
});

var methods = {
  runOpenSendLayerSelect: function(material) {
    this.form.materialId = material.id;

    if (this.form.materialType === 'Message') {
      this.message = material;
    } else if (this.form.materialType === 'Image') {
      this.image = material;
    } else if (this.form.materialType === 'Audio') {
      this.audio = material;
    } else if (this.form.materialType === 'Video') {
      this.video = material;
    }
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
      $this.form.keywords = res.keywords;
      $this.form.messages = res.messages;

      if (!$this.form.keywords) {
        $this.form.keywords = [{
          exact: false,
          text: null
        }];
      }
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
      keywords: this.form.keywords,
      messages: this.form.messages
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

  btnSelectClick: function(key) {
    utils.openLayer({
      title: '选择回复消息',
      url: utils.getWxUrl("layer" + key, {
        siteId: this.siteId
       })
    });
  },

  btnRemoveClick: function() {
    
  },

  btnSubmitClick: function () {
    var $this = this;

    this.$refs.form.validate(function(valid) {
      if (valid) {
        $this.apiSubmit();
      }
    });
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
