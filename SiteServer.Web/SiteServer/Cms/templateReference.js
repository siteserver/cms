var $apiUrl = $apiConfig.apiUrl;
var $siteId = pageUtils.getQueryStringByName('siteId');
var $api = new apiUtils.Api($apiUrl + '/pages/cms/templateReference/' + $siteId);

var data = {
  pageLoad: false,
  pageAlert: null,
  elementNames: null,
  elementName: null,
  element: null,
  attributes: null,
  menus: []
};

var methods = {
  getElements: function () {
    var $this = this;

    $api.get(null, function (err, res) {
      if (err || !res || !res.value) return;

      $this.elementNames = res.value;
      $this.pageLoad = true;
    });
  },
  btnClick: function (elementName) {
    var $this = this;
    this.elementName = elementName;
    $api.get(null, function (err, res) {
      if (err || !res || !res.value) return;

      $this.element = res.value;
      $this.attributes = res.attributes;
    }, elementName);
  },
  btnToggleAttr: function (attr) {
    var val = '#attr_' + this.elementName.replace('stl:', '') + '_' + attr.name;
    if (this.menus.indexOf(val) === -1) {
      this.menus.push(val);
    } else {
      this.menus.splice(this.menus.indexOf(val), 1)
    }
  },
  isEnumMenu: function (attr) {
    var val = '#attr_' + this.elementName.replace('stl:', '') + '_' + attr.name;
    return this.menus.indexOf(val) !== -1;
  }
};

new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.getElements();
  }
});