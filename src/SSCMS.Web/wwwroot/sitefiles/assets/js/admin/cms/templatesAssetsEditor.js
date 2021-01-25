var $url = '/cms/templates/templatesAssetsEditor';

var validateRelatedFileName = function(rule, value, callback) {
  if (value === '' || value === 'T_') {
    callback(new Error('请输入文件文件'));
  } else {
    callback();
  }
};

var data = utils.init({
  siteId: utils.getQueryInt("siteId"),
  directoryPath: utils.getQueryString("directoryPath"),
  fileName: utils.getQueryString("fileName"),
  fileType: utils.getQueryString("fileType"),
  tabName: utils.getQueryString("tabName"),
  templatesAssetsIncludeDir: null,
  templatesAssetsCssDir: null,
  templatesAssetsJsDir: null,
  form: {
    path: null
  },
  contentEditor: null
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId,
        directoryPath: this.directoryPath,
        fileName: this.fileName,
        fileType: this.fileType
      }
    }).then(function (response) {
      var res = response.data;

      $this.templatesAssetsIncludeDir = res.templatesAssetsIncludeDir;
      $this.templatesAssetsCssDir = res.templatesAssetsCssDir;
      $this.templatesAssetsJsDir = res.templatesAssetsJsDir;
      $this.form.path = res.path;
      $this.setEditorContent(res.content);
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function (isClose) {
    var data = {
      siteId: this.siteId,
      path: this.form.path,
      fileType: this.fileType,
      content: this.getEditorContent(),
      directoryPath: this.directoryPath,
      fileName: this.fileName
    };
    var $this = this;

    utils.loading(this, true);
    if (this.fileName) {
      $api.put($url, data).then(function (response) {
        var res = response.data;

        $this.directoryPath = res.directoryPath;
        $this.fileName = res.fileName;
  
        utils.success('文件保存成功!');
        if (isClose) {
          $this.closeAndReload();
        }
      }).catch(function (error) {
        utils.error(error);
      }).then(function () {
        utils.loading($this, false);
      });
    } else {
      $api.post($url, data).then(function (response) {
        var res = response.data;

        $this.directoryPath = res.directoryPath;
        $this.fileName = res.fileName;
  
        utils.success('文件保存成功!');
        if (isClose) {
          $this.closeAndReload();
        }
      }).catch(function (error) {
        utils.error(error);
      }).then(function () {
        utils.loading($this, false);
      });
    }
  },

  closeAndReload: function() {
    var tabVue = utils.getTabVue(this.tabName);
    if (tabVue) {
      tabVue.apiList();
    }
    utils.removeTab();
    utils.openTab(this.tabName);
  },

  getFileType: function() {
    if (this.fileType === 'html') {
      return '包含文件';
    } else if (this.fileType === 'css') {
      return '样式文件';
    } else if (this.fileType === 'js') {
      return '脚本文件';
    }
    return '';
  },

  getFileTypeDir: function() {
    if (this.fileType === 'html') {
      return this.templatesAssetsIncludeDir + '/';
    } else if (this.fileType === 'css') {
      return this.templatesAssetsCssDir + '/';
    } else if (this.fileType === 'js') {
      return this.templatesAssetsJsDir + '/';
    }
    return '';
  },

  getEditorContent: function() {
    return this.contentEditor.getModel().getValue();
  },

  setEditorContent: function(val) {
    if (this.contentEditor) {
      this.contentEditor.getModel().setValue(val);
      this.contentEditor.focus();
    } else {
      var $this = this;
      var lang = 'html';
      if (this.fileType === 'css') {
        lang = 'css';
      } else if (this.fileType === 'js') {
        lang = 'javascript';
      }
      setTimeout(function () {
        require.config({ paths: { 'vs': utils.getAssetsUrl('lib/monaco-editor/min/vs') }});
        require(['vs/editor/editor.main'], function() {
            $this.contentEditor = monaco.editor.create(document.getElementById('content'), {
                value: val,
                language: lang
            });
            $this.contentEditor.focus();
        });
      }, 100);
    }
  },

  btnFormatClick: function() {
    this.contentEditor.getAction('editor.action.formatDocument').run().then(function() {
      utils.success('文件代码格式化成功!');
    });
  },

  btnSubmitClick: function(isClose) {
    var $this = this;
    this.$refs.form.validate(function(valid) {
      if (valid) {
        $this.apiSubmit(isClose);
      }
    });
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiGet();
  }
});