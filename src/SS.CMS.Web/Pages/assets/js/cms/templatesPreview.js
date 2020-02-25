var $url = '/admin/cms/templates/templatesPreview';

var data = utils.initData({
  siteId: utils.getQueryInt("siteId"),
  activeName: 'first',
  channels: null,
  form: {
    templateType: 'IndexPageTemplate',
    channelIds: [utils.getQueryInt("siteId")]
  },
  content: null,
  contentEditor: null,
  parsedContentEditor: null,
  parsedContent: ''
});

var methods = {
  apiConfig: function () {
    var $this = this;

    $api.get($url, {
      params: {
        siteId: this.siteId
      }
    }).then(function (response) {
      var res = response.data;

      $this.channels = res.channels;
      $this.content = res.content;
      
      setTimeout(function () {
        require.config({ paths: { 'vs': '../assets/lib/monaco-editor/min/vs' }});
        require(['vs/editor/editor.main'], function() {
            $this.contentEditor = monaco.editor.create(document.getElementById('content'), {
                value: $this.content,
                language: 'html'
            });
            $this.contentEditor.focus();
        });
      }, 100);
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
      siteId: this.siteId,
      templateType: this.form.templateType,
      channelId: this.form.channelIds[this.form.channelIds.length - 1],
      content: this.content
    }).then(function (response) {
      var res = response.data;

      $this.parsedContent = res.value;

      $this.activeName = 'second';
      setTimeout(function () {
        require.config({ paths: { 'vs': '../assets/lib/monaco-editor/min/vs' }});
        require(['vs/editor/editor.main'], function() {
            $this.parsedContentEditor = monaco.editor.create(document.getElementById('parsedContent'), {
                value: $this.parsedContent,
                language: 'html'
            });
            $this.parsedContentEditor.focus();
        });
      }, 100);
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  getEditorContent: function() {
    return this.contentEditor.getModel().getValue();
  },

  btnSubmitClick: function() {
    this.content = this.getEditorContent();
    if (!this.content) {
      this.$message({
        type: 'error',
        message: '请输入需要解析的模板标签!'
      });
      return;
    }

    this.apiSubmit();
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiConfig();
  }
});