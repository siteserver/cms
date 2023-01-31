var $url = '/cms/forms/formTemplates';
var $urlDelete = $url + '/actions/delete';
var $urlExport = $url + '/actions/export';
var $urlImport = $url + '/actions/import';

var data = utils.init({
  siteId: utils.getQueryInt('siteId'),
  formId: utils.getQueryInt('formId'),
  forms: null,
  templates: null,
  siteUrl: null,
  name: null,
  templateHtml: null,
  urlUpload: null,
  files: []
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId
      }
    }).then(function (response) {
      var res = response.data;

      $this.forms = res.forms;
      $this.templates = res.templates;
      $this.siteUrl = res.siteUrl;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiDelete: function (template) {
    var $this = this;

    utils.loading(true);
    $api.post($urlDelete, {
      siteId: $this.siteId,
      name: template.name
    }).then(function (response) {
      var res = response.data;

      $this.apiGet();
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  getTemplatePath: function (template) {
    if (template.isSystem) {
      return '/sitefiles/assets/forms/' + template.name + '/index.html';
    }
    return _.trimEnd(this.siteUrl, '/') + '/forms/' + template.name + '/index.html';
  },

  getCode: function (template) {
    return '<stl:form name="表单名称" type="' + template.name + '"></stl:form>';
  },

  btnExportClick: function(template) {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlExport, {
      siteId: this.siteId,
      isSystem: template.isSystem,
      name: template.name
    }).then(function (response) {
      var res = response.data;

      window.open(res.value);
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnHtmlClick: function (template) {
    var url = utils.getCmsUrl('formTemplateHtml', {
      siteId: this.siteId,
      isSystem: template.isSystem,
      name: template.name
    });
    utils.addTab('代码编辑', url);
  },

  btnEditClick: function (template) {
    utils.openLayer({
      title: '模板设置',
      url: utils.getCmsUrl('formTemplatesLayerEdit', {
        siteId: this.siteId,
        isSystem: template.isSystem,
        name: template.name,
        isClone: false,
        isHtml: false
      }),
      width: 500,
      height: 300
    });
  },

  btnCloneClick: function (template) {
    utils.openLayer({
      title: '克隆模板',
      url: utils.getCmsUrl('formTemplatesLayerEdit', {
        siteId: this.siteId,
        isSystem: template.isSystem,
        name: template.name,
        isClone: true,
        isHtml: false
      }),
      width: 500,
      height: 300
    });
  },

  btnDeleteClick: function (template) {
    var $this = this;
    utils.alertDelete({
      title: '删除模板',
      text: '此操作将删除模板' + template.name + '，确认吗？',
      callback: function () {
        $this.apiDelete(template);
      }
    });
  },

  btnPreviewClick: function(args) {
    var form = args.form;
    var template = args.template;
    var url = '';
    if (template.isSystem) {
      url = '/sitefiles/assets/forms/' + template.name + '/index.html?siteId=' + this.siteId + '&formId=' + form.id + '&apiUrl=' + encodeURIComponent('/api');
    } else {
      url = _.trimEnd(this.siteUrl, '/') + '/forms/' + template.name + '/index.html?siteId=' + this.siteId + '&formId=' + form.id + '&apiUrl=' + encodeURIComponent('/api');
    }
    window.open(url);
  },

  uploadBefore(file) {
    var re = /(\.zip)$/i;
    if(!re.exec(file.name))
    {
      utils.error('上传格式错误，请上传zip压缩包!');
      return false;
    }

    return true;
  },

  uploadProgress: function() {
    utils.loading(this, true);
  },

  uploadSuccess: function(res, file) {
    utils.loading(this, false);

    utils.success('表单导入成功');
    location.reload();
  },

  uploadError: function(err) {
    utils.loading(this, false);
    var error = JSON.parse(err.message);
    utils.error(error.message);
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
    utils.keyPress(null, this.btnCloseClick);
    this.apiGet();
    this.urlUpload = $apiUrl + $urlImport + '?siteId=' + this.siteId;
  }
});
