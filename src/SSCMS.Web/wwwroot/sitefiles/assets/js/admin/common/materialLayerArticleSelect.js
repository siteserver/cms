﻿var $url = '/common/material/layerArticleSelect';

var data = utils.init({
  siteId: utils.getQueryInt("siteId"),
  itemId: utils.getQueryInt("itemId"),
  isSiteOnly: false,
  groups: null,
  count: null,
  items: null,
  multipleSelection: [],

  form: {
    siteId: utils.getQueryInt("siteId"),
    keyword: '',
    groupId: 0,
    page: 1,
    perPage: 24,
    articleIds: utils.getQueryString("articleIds"),
  }
});

var methods = {
  apiGet: function (page) {
    var $this = this;
    this.form.page = page;

    utils.loading(this, true);
    $api.get($url, {
      params: this.form
    }).then(function (response) {
      var res = response.data;

      $this.isSiteOnly = res.isSiteOnly;
      if ($this.isSiteOnly) {
        $this.form.groupId = -$this.siteId;
      }

      $this.groups = res.groups;
      $this.count = res.count;
      $this.items = res.items;
      $this.items.forEach(function (item) {
        item.checked = false;
      });
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  handleSelectionChange: function(val) {
    this.multipleSelection = val;
  },

  getLinkUrl: function(materialType) {
    return utils.getCmsUrl('material' + materialType, {siteId: this.siteId})
  },

  getUploadUrl: function() {
    return $apiUrl + $url + '?siteId=' + this.siteId + '&groupId=' + this.form.groupId
  },

  getPreviewSrcList: function(url) {
    var list = _.map(this.items, function (item) {
      return item.url;
    });
    list.splice(list.indexOf(url), 1);
    list.splice(0, 0, url);
    return list;
  },

  btnTitleClick: function(material) {
    var $this = this;

  },

  btnGroupClick: function(groupId) {
    this.form.groupId = groupId;
    this.apiGet(1);
  },

  btnSearchClick() {
    utils.loading(this, true);
    this.apiGet(1);
  },

  btnPageClick: function(val) {
    utils.loading(this, true);
    this.apiGet(val);
  },

  btnSubmitClick: function () {
    parent.$vue.runMaterialLayerArticlesSubmit(this.multipleSelection, this.itemId);
    utils.closeLayer();
  },

  btnCancelClick: function () {
    utils.closeLayer();
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    utils.keyPress(this.btnSubmitClick, this.btnCancelClick);
    this.apiGet(1);
  }
});
