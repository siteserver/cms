var $url = '/admin/shared/tableStyleLayerValidate';

var data = utils.initData({
  tableName: utils.getQueryString('tableName'),
  attributeName: utils.getQueryString('attributeName'),
  relatedIdentities: utils.getQueryIntList('relatedIdentities'),
  options: null,
  rules: [],

  addPanel: false,
  addForm: null
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        tableName: this.tableName,
        attributeName: this.attributeName,
        relatedIdentities: this.relatedIdentities
      }
    }).then(function (response) {
      var res = response.data;

      $this.options = res.options;
      $this.rules = res.rules || [];
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnSubmitClick: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, {
      tableName: this.tableName,
      attributeName: this.attributeName,
      relatedIdentities: this.relatedIdentities,
      rules: this.rules
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

  btnRemoveClick: function (row) {
    this.rules.splice(this.rules.indexOf(row), 1);
  },

  btnCancelClick: function (){
    utils.closeLayer();
  },

  handleRuleChange: function() {
    this.addForm.value = null;
    this.addForm.message = null;
    for (var i = 0; i < this.options.length; i++) {
      var element = this.options[i];
      if (element.value == this.addForm.type){
        this.addForm.message = element.label;
        return;
      }
    }
  },

  getAvailableRules: function() {
    var rules = [];
    for (var i = 0; i < this.options.length; i++) {
      var element = this.options[i];
      if (element.value == 'None') continue;
      var index = _.findIndex(this.rules, function (o) {
        return o.type == element.value;
      });
      if (index === -1) {
        rules.push(this.options[i])
      }
    }
    return rules;
  },

  btnAddCancelClick: function() {
    this.addPanel = false;
  },

  btnAddClick: function() {
    this.addForm = {
      type: null,
      value: null,
      message: null
    };
    this.addPanel = true;
  },

  btnAddSubmitClick: function() {
    var $this = this;
    this.$refs.addForm.validate(function(valid) {
      if (valid) {
        $this.rules.push({
          type: $this.addForm.type,
          value: $this.addForm.value,
          message: $this.addForm.message
        });
        $this.addPanel = false;
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