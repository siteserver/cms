var $url = '/cms/forms/formDataLayerView';

var data = utils.init({
  siteId: utils.getQueryInt('siteId'),
  formId: utils.getQueryInt('formId'),
  dataId: utils.getQueryInt('dataId'),
  styles: null,
  columns: null,
  formData: null,
  attributeNames: null,
  isReply: null,
  form: {
    replyContent: ''
  }
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId,
        formId: this.formId,
        dataId: this.dataId
      }
    }).then(function (response) {
      var res = response.data;

      $this.styles = res.styles;
      $this.columns = res.columns;
      $this.formData = res.formData;
      $this.attributeNames = res.attributeNames;
      $this.isReply = res.isReply;
      $this.form.replyContent = $this.formData.replyContent;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  getAttributeText: function (attributeName) {
    var column = this.columns.find(function (x) {
      return x.attributeName === attributeName;
    })
    return column.displayName;
  },

  getAttributeValue: function (attributeName) {
    return utils.getValue(this.styles, this.formData, attributeName);
  },

  btnCancelClick: function () {
    utils.closeLayer(false);
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiGet();
    utils.keyPress(null, this.btnCancelClick);
  }
});
