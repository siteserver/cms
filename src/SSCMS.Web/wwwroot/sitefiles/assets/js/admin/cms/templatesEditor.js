﻿var $url = '/cms/templates/templatesEditor';
var $urlSettings = $url + '/actions/settings';
var $urlPreview = $url + '/actions/preview';
var $urlGetContents = $url + '/actions/getContents';

var validateRelatedFileName = function(rule, value, callback) {
  if (value === '' || value === 'T_') {
    callback(new Error('请输入模板文件'));
  } else {
    callback();
  }
};

var data = utils.init({
  siteId: utils.getQueryInt("siteId"),
  templateId: utils.getQueryInt("templateId"),
  templateType: utils.getQueryString("templateType"),
  createdFileFullNameTips: '以“~/”开头代表系统根目录，以“@/”开头代表站点根目录',
  templateName: null,
  relatedFile: null,
  channels: null,
  contents: null,
  content: null,
  contentEditor: null,
  createdFileExtNames: [
    '.html',
    '.htm',
    '.shtml',
    '.xml',
    '.json',
    '.js'
  ],

  panelSettings: false,
  settings: null,
  panelDataSource: false,
  dataSource: {
    channelIds: [],
    channelId: 0,
    contentId: null
  },
  next: '',
  winHeight: 0,
  isPreview: false,
});

var methods = {
  apiGet: function () {
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

      $this.templateName = res.settings.templateName;
      $this.relatedFile = res.settings.relatedFileName + res.settings.createdFileExtName;
      $this.settings = res.settings;
      $this.content = res.content;
      $this.channels = res.channels;
      $this.dataSource.channelIds = res.channelIds;
      $this.dataSource.channelId = res.channelId;
      $this.contents = res.contents;
      $this.dataSource.contentId = res.contentId === 0 ? null : res.contentId;
      $this.setEditorContent($this.content);

      window.addEventListener("beforeunload", function(e) {
        var editorContent = $this.getEditorContent();
        if ($this.content == editorContent) {
          delete e['returnValue'];
        } else {
          e.preventDefault();
          e.returnValue = '';
        }
      });
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function (isClose) {
    this.content = this.getEditorContent();
    var $this = this;

    utils.loading(this, true);
    $api.post($url, {
      siteId: this.siteId,
      templateId: this.settings.templateId,
      content: this.content
    }).then(function (response) {
      var res = response.data;

      utils.success('模板代码保存成功!');
      if (isClose) {
        setTimeout(function () {
          window.close();
        }, 1000);
      }
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSettings: function () {
    this.content = this.getEditorContent();
    var $this = this;

    utils.loading(this, true);
    $api.post($urlSettings, {
      settings: this.settings,
      content: this.content
    }).then(function (response) {
      var res = response.data;

      $this.panelSettings = false;
      $this.templateName = res.settings.templateName;
      $this.relatedFile = res.settings.relatedFileName + res.settings.createdFileExtName;
      $this.settings = res.settings;
      if ($this.next == 'submit') {
        $this.apiSubmit(false);
      } else if ($this.next == 'submitAndClose') {
        $this.apiSubmit(true);
      } else if ($this.next == 'preview') {
        $this.apiPreview();
      } else {
        utils.success('模板设置保存成功!');
      }
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiPreview: function () {
    this.content = this.getEditorContent();
    var $this = this;

    utils.loading(this, true);
    $api.post($urlPreview, {
      siteId: this.siteId,
      channelId: this.dataSource.channelId || 0,
      contentId: this.dataSource.contentId || 0,
      templateId: this.settings.templateId || 0,
      content: this.content
    }).then(function (response) {
      var res = response.data;
      var baseUrl = res.baseUrl;
      var html = res.html; 

      var preview = document.getElementById('preview');
      if (preview.children.length > 0) {
        preview.removeChild(preview.firstChild);
      }

      var iframe = document.createElement("iframe");
      iframe.style.width = "100%";
      iframe.style.height = "100%";
      iframe.style.border = "0";
      preview.appendChild(iframe);

      var content = iframe.contentDocument || iframe.contentWindow.document;

      // workaround for chrome bug
      // http://code.google.com/p/chromium/issues/detail?id=35980#c12

      value = html.replace(
        "<head>",
        "<head><base href='" + baseUrl + "' /><script>if ( window.innerWidth === 0 ) { window.innerWidth = parent.innerWidth; window.innerHeight = parent.innerHeight; }</script>"
      );

      content.open();
      content.write(value);
      content.close();

      $this.isPreview = true;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiGetContents: function() {
    var $this = this;

    $api.post($urlGetContents, {
      siteId: this.siteId,
      channelId: this.dataSource.channelId
    }).then(function (response) {
      var res = response.data;

      $this.contents = res.contents;
      $this.dataSource.contentId = res.contentId === 0 ? null : res.contentId;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  getTemplateType: function() {
    if (this.templateType === 'IndexPageTemplate') {
      return '首页模板';
    } else if (this.templateType === 'ChannelTemplate') {
      return '栏目模板';
    } else if (this.templateType === 'ContentTemplate') {
      return '内容模板';
    } else if (this.templateType === 'FileTemplate') {
      return '单页模板';
    }
    return '';
  },

  handleDataSourceChannelChange: function() {
    this.dataSource.channelId = this.dataSource.channelIds[this.dataSource.channelIds.length - 1];
    this.dataSource.contentId = null;
    if (this.templateType === 'ContentTemplate') {
      this.apiGetContents();
    }
  },

  isCreatedFileFullName: function() {
    return this.templateType === 'IndexPageTemplate' || this.templateType === 'FileTemplate';
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
      utils.loading(this, true);
      setTimeout(function () {
        require.config({ paths: { 'vs': utils.getAssetsUrl('lib/monaco-editor/min/vs') }});
        require(['vs/editor/editor.main'], function() {
          $this.contentEditor = monaco.editor.create(document.getElementById('editor'), {
              value: val,
              language: 'html'
          });
          $this.contentEditor.focus();
          utils.loading($this, false);
        });
      }, 100);
    }
  },

  btnCodeClick: function() {
    this.isPreview = false;
  },

  btnFormatClick: function() {
    this.contentEditor.getAction('editor.action.formatDocument').run().then(function() {
      utils.success('模板代码格式化成功!');
    });
  },

  btnSettingsClick: function() {
    this.panelSettings = true;
    this.next = '';
  },

  btnDataSourceClick: function() {
    this.panelDataSource = true;
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

      utils.openLayer({
        title: '生成进度查看',
        url: utils.getCmsUrl('createStatus', {siteId: $this.siteId}),
        width: '50%',
        height: '50%'
      });
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnPreviewClick: function() {
    if (this.settings.templateId == 0) {
      this.panelSettings = true;
      this.next = 'preview';
    } else if (this.dataSource.channelId === 0 && this.templateType === 'ChannelTemplate') {
      this.panelDataSource = true;
      this.next = 'preview';
    } else if ((this.dataSource.channelId === 0 || !this.dataSource.contentId || this.dataSource.contentId === 0) && this.templateType === 'ContentTemplate') {
      this.panelDataSource = true;
      this.next = 'preview';
    } else {
      this.apiPreview();
    }
  },

  btnSubmitClick: function(isClose) {
    if (this.settings.templateId == 0) {
      this.panelSettings = true;
      this.next = isClose ? 'submitAndClose' : 'submit';
    } else {
      this.apiSubmit(isClose);
    }
  },

  btnSettingsSubmitClick: function() {
    var $this = this;
    this.$refs.settings.validate(function(valid) {
      if (valid) {
        $this.apiSettings();
      }
    });
  },

  btnSettingsCancelClick: function () {
    this.panelSettings = false;
  },

  btnDataSourceSubmitClick: function() {
    var $this = this;
    this.$refs.dataSource.validate(function(valid) {
      if (valid) {
        $this.panelDataSource = false;
        $this.dataSource.channelId = $this.dataSource.channelIds[$this.dataSource.channelIds.length - 1];
        if ($this.next === 'preview') {
          $this.apiPreview();
        }
      }
    });
  },

  btnDataSourceCancelClick: function () {
    this.panelDataSource = false;
  },
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.winHeight = $(window).height();
    this.apiGet();
  }
});