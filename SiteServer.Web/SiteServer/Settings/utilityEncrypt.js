var $url = '/pages/settings/utilityEncrypt';

var data = {
  pageLoad: false,
  pageAlert: null,

  isEncrypt: true,
  value: null,
  results: null
};

var methods = {
  submit: function () {
    var $this = this;

    utils.loading(true);
    $api.post($url, {
      isEncrypt: this.isEncrypt,
      value: this.value
    }).then(function (response) {
      var res = response.data;

      $this.results = res.value;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  radioChanged: function() {
    this.results = '';
  },

  btnSubmitClick: function () {
    this.pageAlert = null;
    var $this = this;
    this.$validator.validate().then(function (result) {
      if (result) {
        $this.submit();
      }
    });
  }
};

new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.pageLoad = true;
  }
});