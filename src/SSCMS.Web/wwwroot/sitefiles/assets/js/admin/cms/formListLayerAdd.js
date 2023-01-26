var $url = '/cms/forms/formListLayerAdd';
var $urlUpdate = $url + '/actions/update';

var data = utils.init({
  siteId: utils.getQueryInt('siteId'),
  formId: utils.getQueryInt('formId'),
  form: {
    title: null,
    description: null
  }
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId,
        formId: this.formId
      }
    }).then(function (response) {
      var res = response.data;

      $this.form = res.form;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiAdd: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, {
      siteId: this.siteId,
      title: this.form.title,
      description: this.form.description
    }).then(function (response) {
      var res = response.data;

      utils.success('表单添加成功');
      utils.closeLayer(true);
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiEdit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlUpdate, {
      siteId: this.siteId,
      formId: this.form.id,
      title: this.form.title,
      description: this.form.description
    }).then(function (response) {
      var res = response.data;

      utils.success('表单修改成功');
      utils.closeLayer(true);
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
        if ($this.form.id) {
          $this.apiEdit();
        } else {
          $this.apiAdd();
        }
      }
    });
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
    if (this.formId) {
      this.apiGet();
    } else {
      utils.loading(this, false);
    }
    utils.focus(this, 'title');
  }
});
