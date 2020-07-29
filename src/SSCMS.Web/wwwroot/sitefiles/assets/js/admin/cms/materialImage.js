var $url = '/cms/material/image';
var $urlActionsDeleteGroup = '/cms/material/image/actions/deleteGroup';
var $urlActionsPull = '/cms/material/image/actions/pull';
var $urlActionsDownload = '/cms/material/image/actions/download';

var data = utils.init({
  siteId: utils.getQueryInt("siteId"),
  showType: 'card',
  groups: null,
  count: null,
  items: null,
  isOpen: false,
  urlList: null,
  renameId: 0,
  renameTitle: '',
  deleteId: 0,
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
  runUpdateGroups: function(groups) {
    this.groups = groups;
  },

  apiList: function (page) {
    var $this = this;
    this.form.page = page;

    utils.loading(this, true);
    $api.get($url, {
      params: this.form
    }).then(function (response) {
      var res = response.data;

      $this.groups = res.groups;
      $this.count = res.count;
      $this.items = res.items;
      $this.isOpen = res.isOpen;
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
    $api.delete($urlActionsDeleteGroup, {
      data: {
        siteId: this.siteId,
        id: this.form.groupId
      }
    }).then(function (response) {
      var res = response.data;

      $this.form.groupId = 0;
      $this.apiList(1);
    }).catch(function (error) {
      utils.error(error);
    });
  },

  apiDelete: function (material) {
    var $this = this;

    utils.loading(this, true);
    $api.delete($url, {
      data: {
        siteId: this.siteId,
        id: material.id
      }
    }).then(function (response) {
      var res = response.data;

      utils.success('图片素材删除成功！');
      $this.apiList(1);
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiPull: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlActionsPull, {
      siteId: this.siteId,
      groupId: this.form.groupId
    }).then(function (response) {
      var res = response.data;

      utils.success('公众号图片素材拉取成功！');
      $this.apiList(1);
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
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
    $api.put($url, {
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
    $api.put($url, {
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
  
  btnPullClick: function() {
    var $this = this;
    
    utils.alertWarning({
      title: '拉取公众号图片素材',
      text: '此操作将拉取公众号图片素材，确认吗？',
      callback: function () {
        $this.apiPull();
      }
    });
  },

  btnGroupAddClick: function() {
    utils.openLayer({
      title: '新建分组',
      url: utils.getCmsUrl('materialLayerGroupAdd', {
        siteId: this.siteId,
        materialType: 'Image'
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
        materialType: 'Image'
      }),
      width: 400,
      height: 260
    });
  },

  btnDownloadClick: function(material) {
    window.open($apiUrl + $urlActionsDownload + '?siteId=' + this.siteId + '&id=' + material.id + '&access_token=' + $token);
  },

  btnGroupDeleteClick: function () {
    var $this = this;

    utils.alertDelete({
      title: '删除分组',
      text: '仅删除分组，不删除图片，组内图片将自动归入未分组',
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
    this.apiList(1);
  },

  btnPageClick: function(val) {
    utils.loading(this, true);
    this.apiList(val);
  },

  uploadBefore(file) {
    var re = /(\.jpg|\.jpeg|\.bmp|\.gif|\.svg|\.png|\.webp|\.jfif)$/i;
    if(!re.exec(file.name))
    {
      utils.error('文件只能是图片格式，请选择有效的文件上传!');
      return false;
    }

    var isLt10M = file.size / 1024 / 1024 < 10;
    if (!isLt10M) {
      utils.error('上传图片大小不能超过 10MB!');
      return false;
    }
    return true;
  },

  uploadProgress: function() {
    utils.loading(this, true);
  },

  uploadSuccess: function(res) {
    this.items.splice(0, 0, res);
    this.count++;
    utils.loading(this, false);
  },

  uploadError: function(err) {
    utils.loading(this, false);
    var error = JSON.parse(err.message);
    utils.error(error.message);
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