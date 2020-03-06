var $url = '/admin/cms/templates/templatesAssetsEditor';

var validateRelatedFileName = function(rule, value, callback) {
  if (value === '' || value === 'T_') {
    callback(new Error('请输入文件文件'));
  } else {
    callback();
  }
};

var data = utils.initData({
  siteId: utils.getQueryInt("siteId"),
  directoryPath: utils.getQueryString("directoryPath"),
  fileName: utils.getQueryString("fileName"),
  extName: utils.getQueryString("extName"),
  form: {
    path: null
  },
  contentEditor: null
});

var methods = {
  apiConfig: function () {
    var $this = this;

    if (this.fileName) {
      utils.loading(this, true);
      $api.get($url, {
        params: {
          siteId: this.siteId,
          directoryPath: this.directoryPath,
          fileName: this.fileName
        }
      }).then(function (response) {
        var res = response.data;

        $this.form.path = res.path;
        $this.extName = res.extName;
        $this.setEditorContent(res.content);
      }).catch(function (error) {
        utils.error($this, error);
      }).then(function () {
        utils.loading($this, false);
      });
    } else {
      this.form.path = '';
      this.setEditorContent('');
      utils.loading(this, false);
    }
  },

  apiSubmit: function (isReturn) {
    var data = {
      siteId: this.siteId,
      path: this.form.path,
      extName: this.extName,
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
  
        $this.$message({
          type: 'success',
          message: '文件保存成功!'
        });
        if (isReturn) {
          setTimeout(function () {
            $this.btnCancelClick();
          }, 1000);
        }
      }).catch(function (error) {
        utils.error($this, error);
      }).then(function () {
        utils.loading($this, false);
      });
    } else {
      $api.post($url, data).then(function (response) {
        var res = response.data;

        $this.directoryPath = res.directoryPath;
        $this.fileName = res.fileName;
  
        $this.$message({
          type: 'success',
          message: '文件保存成功!'
        });
        if (isReturn) {
          setTimeout(function () {
            $this.btnCancelClick();
          }, 1000);
        }
      }).catch(function (error) {
        utils.error($this, error);
      }).then(function () {
        utils.loading($this, false);
      });
    }
  },

  getFileType: function() {
    if (this.extName === '.html') {
      return '包含文件';
    } else if (this.extName === '.css') {
      return '样式文件';
    } else if (this.extName === '.js') {
      return '脚本文件';
    }
    return '';
  },

  getFileTypeDir: function() {
    if (this.extName === '.html') {
      return 'include/';
    } else if (this.extName === '.css') {
      return 'css/';
    } else if (this.extName === '.js') {
      return 'js/';
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
      if (this.extName === '.css') {
        lang = 'css';
      } else if (this.extName === '.js') {
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
    var $this = this;
    this.contentEditor.getAction('editor.action.formatDocument').run().then(function() {
      $this.$message({
        type: 'success',
        message: '文件代码格式化成功!'
      });
    });
  },

  btnSubmitClick: function(isReturn) {
    var $this = this;
    this.$refs.form.validate(function(valid) {
      if (valid) {
        $this.apiSubmit(isReturn);
      }
    });
  },

  btnCancelClick: function() {
    location.href = utils.getCmsUrl('templateAssets', {
      siteId: this.siteId,
      fileType: this.extName
    });
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