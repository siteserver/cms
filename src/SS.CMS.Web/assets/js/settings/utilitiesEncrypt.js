var $url = '/admin/settings/utilitiesEncrypt';

var data = utils.initData({
  form: {
    isEncrypt: true,
    value: null,
  },
  results: null
});

var methods = {
  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, {
      isEncrypt: this.form.isEncrypt,
      value: this.form.value
    }).then(function (response) {
      var res = response.data;

      $this.results = res.value;
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  radioChanged: function() {
    this.results = '';
  },

  btnSubmitClick: function () {
    var $this = this;

    this.$refs.form.validate(function(valid) {
      if (valid) {
        $this.apiSubmit();
      }
    });
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    utils.loading(this, false);
  }
});