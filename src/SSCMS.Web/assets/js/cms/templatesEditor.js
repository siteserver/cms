var $url = '/admin/cms/templates/templatesEditor';

var validateRelatedFileName = function(rule, value, callback) {
  if (value === '' || value === 'T_') {
    callback(new Error('请输入模板文件'));
  } else {
    callback();
  }
};

var data = utils.initData({
  siteId: utils.getQueryInt("siteId"),
  templateId: utils.getQueryInt("templateId"),
  templateType: utils.getQueryString("templateType"),
  createdFileFullNameTips: '以“~/”开头代表系统根目录，以“@/”开头代表站点根目录',
  template: null,
  contentEditor: null,
  rules: {
    templateName: [
      { required: true, message: '请输入模板名称', trigger: 'blur' }
    ],
    relatedFileName: [
      { required: true, message: '请输入模板文件', trigger: 'blur' },
      { validator: validateRelatedFileName, trigger: 'blur' }
    ],
    createdFileFullName: [
      { required: true, message: '请输入模板文件', trigger: 'blur' }
    ],
  }
});

var methods = {
  apiConfig: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId,
        templateType: this.templateType,
        templateId: this.templateId
      }
    }).then(function (response) {
      var res = response.data;

      $this.template = res.template;
      $this.setEditorContent($this.template.content);
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function (isReturn) {
    this.template.content = this.getEditorContent();
    var $this = this;

    utils.loading(this, true);
    if (this.templateId > 0) {
      $api.put($url, this.template).then(function (response) {
        var res = response.data;

        $this.template = res.template;
        $this.$message({
          type: 'success',
          message: '模板保存成功!'
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
      $api.post($url, this.template).then(function (response) {
        var res = response.data;
  
        $this.templateId = res.template.id;
        $this.template = res.template;
        $this.$message({
          type: 'success',
          message: '模板保存成功!'
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

  getTemplateType: function() {
    if (this.template.templateType === 'IndexPageTemplate') {
      return '首页模板';
    } else if (this.template.templateType === 'ChannelTemplate') {
      return '栏目模板';
    } else if (this.template.templateType === 'ContentTemplate') {
      return '内容模板';
    } else if (this.template.templateType === 'FileTemplate') {
      return '单页模板';
    }
    return '';
  },

  isCreatedFileFullName: function() {
    return this.template.templateType === 'IndexPageTemplate' || this.template.templateType === 'FileTemplate';
  },

  getEditorContent: function() {
    return this.contentEditor.getModel().getValue();
  },

  setEditorContent: function(val) {
    var $this = this;

    

    if (this.contentEditor) {
      this.contentEditor.getModel().setValue(val);
      this.contentEditor.focus();
    } else {
      setTimeout(function () {
        require.config({ paths: { 'vs': utils.getAssetsUrl('lib/monacoeditor/min/vs') }});
        require(['vs/editor/editor.main'], function() {
          $this.contentEditor = monaco.editor.create(document.getElementById('templateContent'), {
              value: val,
              language: 'html'
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
        message: '模板代码格式化成功!'
      });
    });
  },

  btnRestoreClick: function() {
    utils.openLayer({
      title: '还原历史版本',
      url: utils.getCmsUrl('templatesEditorLayerRestore', {
        siteId: this.siteId,
        templateId: this.templateId
      }),
      full: true
    });
  },

  btnCreateClick: function() {
    var $this = this;

    utils.loading(this, true);
    $api.post($url + '/actions/create', {
      siteId: this.siteId,
      templateId: this.templateId
    }).then(function (response) {
      var res = response.data;

      parent.$vue.openPageCreateStatus();
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnSubmitClick: function(isReturn) {
    var $this = this;
    this.$refs.template.validate(function(valid) {
      if (valid) {
        $this.apiSubmit(isReturn);
      }
    });
  },

  btnCancelClick: function() {
    location.href = utils.getCmsUrl('templates', {
      siteId: this.siteId
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