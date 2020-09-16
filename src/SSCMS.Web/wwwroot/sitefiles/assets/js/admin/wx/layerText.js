var data = utils.init({
  siteId: utils.getQueryInt("siteId"),
  index: utils.getQueryString("index"),
  form: {
    text: ''
  }
});

var methods = {
  load: function() {
    if (this.index) {
      this.form.text = parent.$vue.messages[parseInt(this.index)].text;
    }
    utils.loading(this, false);
  },

  btnSubmitClick: function() {
    var $this = this;

    this.$refs.form.validate(function(valid) {
      if (valid) {
        parent.$vue.runLayerText($this.form.text, $this.index);
        utils.closeLayer();
      }
    });
  },

  btnCancelClick: function () {
    utils.closeLayer();
  },
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.load();
  }
});