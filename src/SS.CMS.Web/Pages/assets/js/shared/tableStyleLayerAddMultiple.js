var $url = '/admin/shared/tableStyleLayerAddMultiple';

var data = utils.initData({
  tableName: utils.getQueryString('tableName'),
  relatedIdentities: utils.getQueryIntList('relatedIdentities'),
  inputTypes: null,
  form: {
    styles: []
  }
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        tableName: this.tableName,
        relatedIdentities: this.relatedIdentities
      }
    }).then(function (response) {
      var res = response.data;

      $this.inputTypes = res.inputTypes;
      $this.form.styles = res.styles;
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, {
      tableName: this.tableName,
      relatedIdentities: this.relatedIdentities,
      styles: this.form.styles
    }).then(function (response) {
      var res = response.data;

      utils.closeLayer();
      parent.$vue.apiList();
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnSubmitClick() {
    var $this = this;

    this.$refs.form.validate(function(valid) {
      if (valid) {
        $this.apiSubmit();
      }
    });
  },

  btnCancelClick: function (){
    utils.closeLayer();
  },

  btnStyleRemoveClick: function (index) {
    this.form.styles.splice(index, 1);
    if (this.form.styles.length === 0) {
      this.btnStyleAddClick();
    }
  },

  btnStyleAddClick: function () {
    this.form.styles.push({
      attributeName: '',
      displayName: '',
      inputType: 'Text'
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