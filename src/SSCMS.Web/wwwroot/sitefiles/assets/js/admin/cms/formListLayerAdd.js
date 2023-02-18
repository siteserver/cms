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

      utils.alertSuccess({
        title: '表单添加成功',
        text: '表单添加成功，系统需要重载页面',
        callback: function() {
          window.top.location.reload(true);
        }
      });
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

      if (!res.value) {
        return utils.error('表单修改失败，已存在同名表单，请修改表单名称！');
      }

      utils.alertSuccess({
        title: '表单修改成功',
        text: '表单修改成功，系统需要重载页面',
        callback: function() {
          window.top.location.reload(true);
        }
      });
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
