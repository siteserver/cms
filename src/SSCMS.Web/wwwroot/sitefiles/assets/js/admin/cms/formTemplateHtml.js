var $url = '/cms/forms/formTemplateHtml';

var data = utils.init({
  siteId: utils.getQueryInt('siteId'),
  isSystem: utils.getQueryBoolean('isSystem'),
  name: utils.getQueryString('name'),
  templateHtml: null,
  contentEditor: null,
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId,
        isSystem: this.isSystem,
        name: this.name
      }
    }).then(function (response) {
      var res = response.data;

      $this.templateHtml = res.templateHtml;

      setTimeout(function () {
        $('#templateContent').height($(document).height() - 80);
        require.config({ paths: { 'vs': utils.getAssetsUrl('lib/monaco-editor/min/vs') }});
        require(['vs/editor/editor.main'], function() {
          $this.contentEditor = monaco.editor.create(document.getElementById('templateContent'), {
              value: $this.templateHtml,
              language: 'html'
          });
          $this.contentEditor.focus();
        });
      }, 100);
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function(isClose) {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, {
      siteId: this.siteId,
      name: this.name,
      templateHtml: this.templateHtml
    }).then(function (response) {
      var res = response.data;

      utils.success('模板编辑成功！');
      if (isClose) {
        utils.removeTab();
      }
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  getEditorContent: function() {
    return this.contentEditor.getModel().getValue();
  },

  btnSubmitClick: function (isClose) {
    if (this.isSystem) {
      utils.openLayer({
        title: '克隆模板',
        url: utils.getCmsUrl('formTemplatesLayerEdit', {
          siteId: this.siteId,
          isSystem: this.isSystem,
          name: this.name,
          isClone: true,
          isHtml: true
        }),
        width: 500,
        height: 300
      });
      return;
    }

    this.templateHtml = this.getEditorContent();
    this.apiSubmit(isClose);
  },

  btnCloseClick: function() {
    utils.removeTab();
  },
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    utils.keyPress(this.btnSubmitClick, this.btnCloseClick);
    this.apiGet();
  }
});
