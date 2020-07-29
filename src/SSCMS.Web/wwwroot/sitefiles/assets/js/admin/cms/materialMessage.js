var $url = '/cms/material/message';
var $urlActionsDeleteGroup = '/cms/material/message/actions/deleteGroup';
var $urlActionsPull = '/cms/material/message/actions/pull';

var data = utils.init({
  siteId: utils.getQueryInt("siteId"),
  showType: 'card',
  groups: null,
  count: null,
  messages: null,
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
      $this.messages = res.messages;
      $this.isOpen = res.isOpen;
      $this.urlList = _.map($this.messages, function (item) {
        return item.thumbUrl;
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

  apiDelete: function (message) {
    var $this = this;

    utils.loading(this, true);
    $api.delete($url, {
      data: {
        siteId: this.siteId,
        id: message.id
      }
    }).then(function (response) {
      var res = response.data;

      utils.success('图文消息素材删除成功!');
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

      utils.success('公众号图文消息素材拉取成功！');
      $this.apiList(1);
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  getEditorUrl: function() {
    return utils.getCmsUrl('materialEditor', {
      siteId: this.siteId,
      groupId: this.form.groupId,
      page: this.form.page,
      tabName: utils.getTabName()
    });
  },

  getGroupName: function() {
    var $this = this;
    if (this.form.groupId > 0) {
      var group = _.find(this.groups, function(o) { return o.id === $this.form.groupId; });
      return group.groupName;
    }
    return '';
  },

  getUploadUrl: function() {
    return $apiUrl + $url + '?siteId=' + this.siteId + '&groupId=' + this.form.groupId
  },

  btnCreateClick: function() {
    utils.addTab('创建图文消息', this.getEditorUrl());
  },

  btnUpdateClick: function(message) {
    utils.addTab('修改图文消息', this.getEditorUrl() + '&messageId=' + message.id);
  },

  btnSelectGroupClick: function (groupId) {
    this.selectedGroupId = (this.selectedGroupId === groupId) ? 0 :groupId;
  },

  btnSelectGroupSubmit: function(message) {
    var $this = this;

    utils.loading(this, true);
    $api.put($url, {
      id: message.id,
      siteId: this.siteId,
      groupId: this.selectedGroupId
    }).then(function (response) {
      var res = response.data;

      utils.success('转移分组成功');
      message.groupId = $this.selectedGroupId;
      message.isSelectGroups = false;
      if ($this.selectedGroupId !== $this.form.groupId && $this.form.groupId !== 0) {
        $this.btnSearchClick();
      }
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnRenameClick: function(message) {
    var $this = this;

    if (this.renameId === 0) return;
    if (!this.renameTitle) {
      utils.error('名称不能为空');
      return false;
    }
    
    this.renameId = 0;
    if (message.title === this.renameTitle) return false;

    utils.loading(this, true);
    $api.put($url, {
      id: message.id,
      siteId: this.siteId,
      groupId: message.groupId,
      title: this.renameTitle
    }).then(function (response) {
      var res = response.data;

      utils.success('编辑名称成功');
      message.title = $this.renameTitle;
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
      $this.messages = res.messages;
      $this.urlList = _.map($this.messages, function (item) {
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
      title: '拉取公众号图文消息素材',
      text: '此操作将拉取公众号图文消息素材，确认吗？',
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
        materialType: 'Message'
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
        materialType: 'Message'
      }),
      width: 400,
      height: 260
    });
  },

  btnGroupDeleteClick: function () {
    var $this = this;

    utils.alertDelete({
      title: '删除分组',
      text: '仅删除分组，不删除图文消息，组内图文消息将自动归入未分组',
      callback: function () {
        $this.apiDeleteGroup();
      }
    });
  },

  btnDeleteClick: function (message) {
    var $this = this;

    utils.alertDelete({
      title: '删除素材',
      text: '确定删除此素材吗？',
      callback: function () {
        $this.apiDelete(message);
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
    var re = /(\.docx)$/i;
    if(!re.exec(file.name))
    {
      utils.error('文件只能是以.docx结尾的 Word 格式，请选择有效的文件上传!');
      return false;
    }
    return true;
  },

  uploadProgress: function() {
    utils.loading(this, true);
  },

  uploadSuccess: function(res) {
    this.apiList(1);
    utils.loading(this, false);
  },

  uploadError: function(err) {
    utils.loading(this, false);
    var error = JSON.parse(err.message);
    utils.error(error.message);
  },

  getFriendlyContent: function(message) {
    if (message.items.length === 1) {
      return message.items[0].title;
    }
    var i = 1;
    var contents = message.items.map(function(item) {
      return i++ + '. ' + item.title;
    });
    return contents.join('<br />');
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