var $url = '/common/tableStyle/layerEditor';

var data = utils.init({
  tableName: utils.getQueryString('tableName'),
  attributeName: utils.getQueryString('attributeName'),
  relatedIdentities: utils.getQueryString('relatedIdentities'),
  excludes: utils.getQueryStringList('excludes'),
  inputTypes: null,
  form: null
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

      $this.inputTypes = res.inputTypes.filter(function (x) {
        return $this.excludes.indexOf(x.key) == -1;
      });

      $this.form = res.form;
      if (!$this.form.items || $this.form.items.length === 0) {
        $this.form.items.push({
          label: '',
          value: '',
          selected: false
        });
      }
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, _.assign({}, this.form, {
      tableName: this.tableName,
      relatedIdentities: this.relatedIdentities
    })).then(function (response) {
      var res = response.data;

      utils.closeLayer();
      if ($this.attributeName !== '') {
        utils.success('字段编辑成功!');
      } else {
        utils.success('字段新增成功!');
      }
      if (parent.$vue.runTableStyleLayerEditor) {
        parent.$vue.runTableStyleLayerEditor();
      }
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  validateAttributeName: function(rule, value, callback) {
    if (!/^\+?[A-Za-z0-9]+$/.test(value)) {
      return callback(new Error('字段名称只允许输入字母或者数字'));
    }
    callback();
  },

  btnSubmitClick() {
    var $this = this;
    var forms = [
      this.$refs['form1'],
      this.$refs['form2'],
      this.$refs['form3'],
      this.$refs['form4']
    ];
    var success = true;
    _.forEach(forms, function(value) {
      if (value) {
        value.validate(function(valid) {
          if (!valid) {
            success = false;
          }
        });
      }
    });
    if (success) {
      $this.apiSubmit();
    }
  },

  btnCancelClick: function (){
    utils.closeLayer();
  },

  btnStyleItemRemoveClick: function (index) {
    this.form.items.splice(index, 1);
    if (this.form.items.length === 0) {
      this.btnStyleItemAddClick();
    }
  },

  btnStyleItemAddClick: function () {
    this.form.items.push({
      label: '',
      value: '',
      selected: false
    })
  },

  btnRadioClick: function (index) {
    for (var i = 0; i < this.form.items.length; i++) {
      var element = this.form.items[i];
      element.selected = false;
    }
    this.form.items[index].selected = true;
  }
};

var computed = {
  isSelect: function () {
    return this.form.inputType === 'CheckBox' || this.form.inputType === 'Radio' || this.form.inputType === 'SelectOne' || this.form.inputType === 'SelectMultiple';
  }
}

var $vue = new Vue({
  el: '#main',
  data: data,
  computed: computed,
  methods: methods,
  created: function () {
    this.apiGet();
  }
});