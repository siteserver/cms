var $url = '/common/material/layerImageSelect';

var data = utils.init({
  siteId: utils.getQueryInt("siteId"),
  inputType: utils.getQueryString('inputType'),
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
    groupId: 0,
    page: 1,
    perPage: 24
  }
});

var methods = {
  insert: function(virtualUrl, imageUrl) {
    if (this.inputType === 'Image') {
      if (parent.$vue.runMaterialLayerImageSelect) {
        parent.$vue.runMaterialLayerImageSelect(this.attributeName, this.no, virtualUrl);
      }
    } else if (this.inputType === 'TextEditor') {
      if (parent.$vue.runEditorLayerImage) {
        var html = '<img src="' + imageUrl + '" style="border: 0; max-width: 100%" />';
        parent.$vue.runEditorLayerImage(this.attributeName, html);
      }
    }
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

      $this.insert(res.virtualUrl, res.imageUrl);
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
    utils.keyPress(null, this.btnCancelClick);
    this.apiList(1);
  }
});
