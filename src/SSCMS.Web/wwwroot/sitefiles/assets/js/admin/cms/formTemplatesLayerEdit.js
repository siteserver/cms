var $url = '/cms/forms/formTemplatesLayerEdit';
var $urlClone = $url + '/actions/clone';
var $urlUpdate = $url + '/actions/update';

var data = utils.init({
  siteId: utils.getQueryInt('siteId'),
  isSystem: utils.getQueryBoolean('isSystem'),
  name: utils.getQueryString('name'),
  isClone: utils.getQueryBoolean('isClone'),
  isHtml: utils.getQueryBoolean('isHtml'),
  form: {
    name: ''
  },
});

var methods = {
  getTemplateHtml: function() {
    return this.isHtml ? parent.$vue.getEditorContent() : '';
  },

  apiClone: function() {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlClone, {
      siteId: this.siteId,
      isSystemOriginal: this.isSystem,
      nameOriginal: this.name,
      name: this.form.name,
      isHtml: this.isHtml,
      templateHtml: this.getTemplateHtml()
    }).then(function (response) {
      utils.success('模板克隆成功！');
      parent.location.href = $this.getTemplatesUrl();
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiUpdate: function() {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlUpdate, {
      siteId: this.siteId,
      isSystemOriginal: this.isSystem,
      nameOriginal: this.name,
      name: this.form.name,
    }).then(function (response) {

      utils.success('模板编辑成功！');
      parent.location.href = $this.getTemplatesUrl();
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnSubmitClick: function () {
    var $this = this;

    this.$refs.form.validate(function(valid) {
      if (valid) {
        if ($this.isClone) {
          $this.apiClone();
        } else {
          $this.apiUpdate();
        }
      }
    });
  },

  getTemplatesUrl: function() {
    return utils.getCmsUrl('formTemplates', {
      siteId: this.siteId,
      formId: this.formId,
    });
  },

  btnCancelClick: function() {
    window.parent.layer.closeAll()
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    utils.keyPress(this.btnSubmitClick, this.btnCancelClick);
    this.form.name = this.isClone ? '' : this.name;
    utils.loading(this, false);
    utils.focus(this, 'name');
  }
});
