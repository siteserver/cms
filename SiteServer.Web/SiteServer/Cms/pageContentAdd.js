var $url = '/pages/main';

function insertHtml(attributeName, html)
{
    if (html)
    {
      UE.getEditor(attributeName, {allowDivTransToP: false, maximumWords:99999999}).execCommand('insertHTML', html);
    }
}

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