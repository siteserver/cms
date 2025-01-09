var $url = '/cms/material/component';
var $urlUpdate = $url + '/actions/update';
var $urlDelete = $url + '/actions/delete';
var $urlDeleteGroup = $url + '/actions/deleteGroup';
var $urlDownload = $url + '/actions/download';

var data = utils.init({
  siteId: utils.getQueryInt("siteId"),
  isSiteOnly: false,
  groups: null,
  count: null,
  items: null,
  siteType: null,
  urlList: null,
  renameId: 0,
  renameTitle: '',
  deleteId: 0,
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
  runUpdateGroups: function(groups) {
    this.groups = groups;
  },

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
      $this.items = [];
      for (var item of res.items) {
        $this.items.push(_.assign({isSelectGroups: false}, item));
      }
      $this.siteType = res.siteType;
      $this.urlList = _.map($this.items, function (item) {
        return item.url;
      });
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiDeleteGroup: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlDeleteGroup, {
      siteId: this.siteId,
      id: this.form.groupId
    }).then(function (response) {
      var res = response.data;

      $this.form.groupId = 0;
      $this.apiGet(1);
    }).catch(function (error) {
      utils.error(error);
    });
  },

  apiDelete: function (material) {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlDelete, {
      siteId: this.siteId,
      id: material.id
    }).then(function (response) {
      var res = response.data;

      utils.success('视频素材删除成功！');
      $this.apiGet(1);
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  getLinkUrl: function(materialType) {
    return utils.getCmsUrl('material' + materialType, {siteId: this.siteId})
  },

  btnTitleClick: function(material) {
    var $this = this;
    this.renameId = material.id;
    this.renameTitle = material.title;
    setTimeout(function() {
      var el = $this.$refs['renameInput' + material.id][0];
      if (el) {
        el.focus();
        el.select();
      }
    }, 100);
  },

  btnSelectGroupClick: function (groupId) {
    this.selectedGroupId = (this.selectedGroupId === groupId) ? 0 :groupId;
  },

  btnSelectGroupSubmit: function(material) {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlUpdate, {
      id: material.id,
      siteId: this.siteId,
      groupId: this.selectedGroupId,
      title: material.title
    }).then(function (response) {
      var res = response.data;

      utils.success('转移分组成功');
      material.groupId = $this.selectedGroupId;
      material.isSelectGroups = false;
      if ($this.selectedGroupId !== $this.form.groupId && $this.form.groupId !== 0) {
        $this.btnSearchClick();
      }
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnRenameClick: function(material) {
    var $this = this;

    if (this.renameId === 0) return;
    if (!this.renameTitle) {
      utils.error('名称不能为空');
      return false;
    }

    this.renameId = 0;
    if (material.title === this.renameTitle) return false;

    utils.loading(this, true);
    $api.post($urlUpdate, {
      id: material.id,
      siteId: this.siteId,
      groupId: material.groupId,
      title: this.renameTitle
    }).then(function (response) {
      var res = response.data;

      utils.success('编辑名称成功');
      material.title = $this.renameTitle;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });

    return false;
  },

  btnGroupClick: function(groupId) {
    var $this = this;

    this.form.groupId = groupId;
    this.form.page = 1;

    utils.loading(this, true);
    $api.get($url, {
      params: this.form
    }).then(function (response) {
      var res = response.data;

      $this.groups = res.groups;
      $this.count = res.count;
      $this.items = res.items;
      $this.urlList = _.map($this.items, function (item) {
        return item.url;
      });
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnGroupAddClick: function() {
    utils.openLayer({
      title: '新建分组',
      url: utils.getCmsUrl('materialLayerGroupAdd', {
        siteId: this.siteId,
        materialType: 'Video'
      }),
      width: 400,
      height: 260
    });
  },

  btnGroupEditClick: function() {
    utils.openLayer({
      title: '编辑分组',
      url: utils.getCmsUrl('materialLayerGroupAdd', {
        siteId: this.siteId,
        groupId: this.form.groupId,
        materialType: 'Video'
      }),
      width: 400,
      height: 260
    });
  },

  btnGroupDeleteClick: function () {
    var $this = this;

    utils.alertDelete({
      title: '删除分组',
      text: '仅删除分组，不删除视频，组内视频将自动归入未分组',
      callback: function () {
        $this.apiDeleteGroup();
      }
    });
  },

  btnDeleteClick: function (material) {
    var $this = this;

    utils.alertDelete({
      title: '删除素材',
      text: '确定删除此素材吗？',
      callback: function () {
        $this.apiDelete(material);
      }
    });
  },

  btnSearchClick() {
    utils.loading(this, true);
    this.apiGet(1);
  },

  btnPageClick: function(val) {
    utils.loading(this, true);
    this.apiGet(val);
  },

  btnCreateClick: function() {
    utils.addTab('创建组件', this.getEditorUrl());
  },

  btnUpdateClick: function(component) {
    utils.addTab('修改组件', this.getEditorUrl() + '&componentId=' + component.id);
  },

  getEditorUrl: function() {
    return utils.getCmsUrl('materialComponentEditor', {
      siteId: this.siteId,
      groupId: this.form.groupId,
      page: this.form.page,
      tabName: utils.getTabName()
    });
  },

  btnPreviewVideoClick: function (componentUrl) {
    if (componentUrl) {
      utils.openLayer({
        title: "预览视频",
        url: utils.getCommonUrl("editorLayerPreviewVideo", {
          siteId: this.siteId,
          componentUrl: componentUrl,
        }),
        width: 600,
        height: 500,
      });
    }
  },

  btnCloseClick: function() {
    utils.removeTab();
  },
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    utils.keyPress(this.btnSearchClick, this.btnCloseClick);
    this.apiGet(1);
  }
});
