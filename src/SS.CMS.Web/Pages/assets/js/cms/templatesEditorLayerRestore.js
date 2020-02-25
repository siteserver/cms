var $url = '/admin/cms/templates/templatesEditorLayerRestore';

var data = utils.initData({
  siteId: utils.getQueryInt("siteId"),
  templateId: utils.getQueryInt("templateId"),
  logs: null,
  logId: 0,
  original: null,
  modified: null,
  diffEditor: null
});

var methods = {
  apiConfig: function (logId) {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId,
        templateId: this.templateId,
        logId: logId
      }
    }).then(function (response) {
      var res = response.data;

      $this.logs = res.logs;
      $this.logId = res.logId;
      $this.original = res.original;
      $this.modified = res.modified;
      if ($this.logId) {
        $this.setEditorContent($this.original, $this.modified);
      }
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiDelete: function () {
    var $this = this;

    utils.loading(this, true);
    $api.delete($url, {
      data: {
        siteId: this.siteId,
        templateId: this.templateId,
        logId: this.logId
      }
    }).then(function (response) {
      var res = response.data;

      $this.logs = res.logs;
      $this.logId = res.logId;
      $this.original = res.original;
      $this.modified = res.modified;
      if ($this.logId) {
        $this.setEditorContent($this.original, $this.modified);
      }
      $this.$message({
        type: 'success',
        message: '历史版本删除成功！'
      });
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  handleLogIdChange: function() {
    this.apiConfig(this.logId);
  },

  btnDeleteClick: function() {
    var $this = this;
    utils.alertDelete({
      title: '删除历史版本',
      text: '此操作将删除此历史版本，确认吗？',
      callback: function () {
        $this.apiDelete();
      }
    });
  },

  setEditorContent: function(original, modified) {
    var $this = this;
    if (this.diffEditor) {
      var originalModel = monaco.editor.createModel(original, "text/html");
      var modifiedModel = monaco.editor.createModel(modified, "text/html");
      $this.diffEditor.setModel({
        original: originalModel,
        modified: modifiedModel
      });
    } else {
      setTimeout(function () {
        require.config({ paths: { 'vs': '../assets/lib/monaco-editor/min/vs' }});
        require(['vs/editor/editor.main'], function() {
            var originalModel = monaco.editor.createModel(original, "text/html");
            var modifiedModel = monaco.editor.createModel(modified, "text/html");
            $this.diffEditor = monaco.editor.createDiffEditor(document.getElementById('content'), {
                language: 'html'
            });
            $this.diffEditor.setModel({
              original: originalModel,
              modified: modifiedModel
            });
        });
      }, 100);
    }
  },

  btnSubmitClick: function() {
    var $parent = parent.$vue;
    $parent.setEditorContent(this.original);
    utils.closeLayer();
  },

  btnCancelClick: function() {
    utils.closeLayer();
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiConfig(0);
  }
});