var $url = '/cms/forms/formDataLayerReply';

var data = utils.init({
  siteId: utils.getQueryInt('siteId'),
  formId: utils.getQueryInt('formId'),
  dataId: utils.getQueryInt('dataId'),
  styles: null,
  columns: null,
  formData: null,
  attributeNames: null,
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
      $this.form.replyContent = $this.formData.replyContent;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function() {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, {
      siteId: this.siteId,
      formId: this.formId,
      dataId: this.dataId,
      replyContent: this.form.replyContent
    }).then(function (response) {
      var res = response.data;

      utils.success('回复成功！');
      parent.location.reload();
      utils.closeLayer();
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
        $this.apiSubmit();
      }
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
    utils.keyPress(this.btnSubmitClick, this.btnCancelClick);
    this.apiGet();
  }
});
