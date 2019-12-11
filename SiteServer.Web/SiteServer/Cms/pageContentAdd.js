var $url = '/pages/main';

var data = {
  allTagNames: null,
  tagNames: []
};

var methods = {
  getConfig: function () {
    this.allTagNames = window.allTagNames;
    this.tagNames = window.tagNames;
  }
};

var $vue = new Vue({
  el: "#myForm",
  data: data,
  methods: methods,
  created: function () {
    this.getConfig();
  }
});