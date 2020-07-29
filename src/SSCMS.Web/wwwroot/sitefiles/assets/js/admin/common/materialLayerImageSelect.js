var $url = '/common/material/layerImageSelect';

var data = utils.init({
  siteId: utils.getQueryInt("siteId"),
  attributeName: utils.getQueryString('attributeName'),
  no: utils.getQueryInt('no'),
  pageType: 'card',

  groups: null,
  count: null,
  items: null,
  selectedGroupId: 0,
  
  form: {
    siteId: utils.getQueryInt("siteId"),
    keyword: '',
    groupId: -utils.getQueryInt("siteId"),
    page: 1,
    perPage: 24
  }
});

var methods = {
  insert: function(imageUrl) {
    parent.$vue.insertText(this.attributeName, this.no, imageUrl);
  },

  apiList: function (page) {
    var $this = this;
    this.form.page = page;

    utils.loading(this, true);
    $api.post($url, this.form).then(function (response) {
      var res = response.data;

      $this.groups = res.groups;
      $this.count = res.count;
      $this.items = res.items;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  getLinkUrl: function(materialType) {
    return utils.getCmsUrl('material' + materialType, {siteId: this.siteId})
  },

  btnSelectClick: function(material) {
    var $this = this;
  
    utils.loading(this, true);
    $api.post($url + '/actions/select', {
      siteId: this.siteId,
      materialId: material.id
    })
    .then(function(response) {
      var res = response.data;
  
      $this.insert(res.value);
      utils.closeLayer();
    })
    .catch(function(error) {
      utils.error(error);
    })
    .then(function() {
      utils.loading($this, false);
    });
  },

  btnSelectGroupClick: function (groupId) {
    this.selectedGroupId = (this.selectedGroupId === groupId) ? 0 :groupId;
  },

  btnGroupClick: function(groupId) {
    var $this = this;

    this.form.groupId = groupId;
    this.form.page = 1;

    utils.loading(this, true);
    $api.post($url, this.form).then(function (response) {
      var res = response.data;

      $this.groups = res.groups;
      $this.count = res.count;
      $this.items = res.items;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnDropdownClick: function(command) {
    this.pageType = command;
  },

  btnSearchClick() {
    utils.loading(this, true);
    this.apiList(1);
  },

  btnPageClick: function(val) {
    utils.loading(this, true);
    this.apiList(val);
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiList(1);
  }
});