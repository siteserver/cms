var $url = '/wx/users';
var $urlAddTag = '/wx/users/actions/addTag';
var $urlEditTag = '/wx/users/actions/editTag';
var $urlDeleteTag = '/wx/users/actions/deleteTag';
var $urlBlock = '/wx/users/actions/block';
var $urlUnBlock = '/wx/users/actions/unBlock';
var $urlRemark = '/wx/users/actions/remark';

var data = utils.init({
  success: false,
  errorMessage: null,
  tags: null,
  total: null,
  count: null,
  users: null,
  multipleSelection: [],
  popAddTags: false,
  formAddTags: {
    tagName: null
  },
  popEditTags: false,
  formEditTags: {
    tagName: null
  },
  dialogRemark: false,
  formRemark: {
    siteId: utils.getQueryInt('siteId'),
    openId: null,
    remark: null
  },
  form: {
    siteId: utils.getQueryInt('siteId'),
    init: true,
    isBlock: false,
    tagId: 0,
    keyword: null,
    page: 1,
    perPage: 20
  },
  user: null
});

var methods = {
  apiGet: function (page) {
    this.form.page = page;
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: this.form
    }).then(function (response) {
      var res = response.data;

      $this.form.init = false;
      $this.success = res.success;
      $this.errorMessage = res.errorMessage;
      $this.tags = res.tags;
      $this.total = res.total;
      $this.count = res.count;
      $this.users = res.users;
      if ($this.form.tagId > 0) {
        var tag = res.tags.find(function(x) { return x.id === $this.form.tagId });
        $this.formEditTags.tagName = tag.name;
      }
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiAddTag: function (tagName) {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlAddTag, {
      siteId: this.form.siteId,
      tagName: tagName
    }).then(function (response) {
      var res = response.data;

      $this.popAddTags = false;
      $this.formAddTags.tagName = null;
      $this.tags = res.tags;
      utils.success('标签添加成功！');
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiEditTag: function (tagName) {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlEditTag, {
      siteId: this.form.siteId,
      tagId: this.form.tagId,
      tagName: tagName
    }).then(function (response) {
      var res = response.data;

      $this.popEditTags = false;
      $this.formEditTags.tagName = null;
      $this.tags = res.tags;
      utils.success('标签重命名成功！');
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiDeleteTag: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlDeleteTag, {
      siteId: this.form.siteId,
      tagId: this.form.tagId
    }).then(function (response) {
      var res = response.data;

      $this.form.tagId = 0;
      $this.tags = res.tags;
      utils.success('标签删除成功！');
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiBlock: function() {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlBlock, {
      siteId: this.form.siteId,
      openIds: this.openIds
    }).then(function (response) {
      var res = response.data;

      $this.apiGet(1);
      utils.success('成功将用户加入黑名单！');
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiUnBlock: function() {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlUnBlock, {
      siteId: this.form.siteId,
      openIds: this.openIds
    }).then(function (response) {
      var res = response.data;

      $this.apiGet(1);
      utils.success('成功将用户移出黑名单！');
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiRemark: function() {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlRemark, this.formRemark).then(function (response) {
      var res = response.data;

      $this.dialogRemark = false;
      $this.user.remark = $this.formRemark.remark;
      utils.success('备注修改成功！');
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  tableRowClassName: function(scope) {
    if (this.multipleSelection.indexOf(scope.row) !== -1) {
      return 'current-row';
    }
    return '';
  },

  handleSelectionChange: function(val) {
    this.multipleSelection = val;
  },

  toggleSelection: function(row) {
    this.$refs.multipleTable.toggleRowSelection(row);
  },

  getUserTitle: function(user) {
    if (user.remark) return user.remark + '(' + user.nickname + ')';
    return user.nickname;
  },

  getUserTags: function(user) {
    if (user.tagIdList && user.tagIdList.length > 0) {
      var retVal = [];
      for (var i = 0; i < user.tagIdList.length; i++) {
        var tagId = user.tagIdList[i];
        var tag = this.tags.find(function(x) {
          return x.id === tagId;
        });
        if (tag && tag.name) retVal.push(tag.name);
      }
      return retVal.join(',');
    }
    return '';
  },

  btnTabClick: function() {
    if (this.form.isBlock) {
      this.form.tagId = 0;
      this.form.keyword = null;
    }
    this.apiGet(1);
  },

  btnViewClick: function(user) {
    utils.addTab('与 ' + user.nickname + ' 的聊天记录', utils.getWxUrl('chatSend', {
      siteId: this.form.siteId,
      openId: user.openId
    }));
  },

  btnSearchClick() {
    this.apiGet(1);
  },

  btnPageClick: function(val) {
    this.apiGet(val);
  },

  btnRemarkClick: function(user) {
    this.user = user;
    this.dialogRemark = true;
    this.formRemark.openId = user.openId;
    this.formRemark.remark = user.remark;
  },

  btnAddTagClick: function () {
    var $this = this;

    this.$refs.formAddTags.validate(function(valid) {
      if (valid) {
        $this.apiAddTag($this.formAddTags.tagName);
      }
    });
  },

  btnEditTagClick: function () {
    var $this = this;

    this.$refs.formEditTags.validate(function(valid) {
      if (valid) {
        $this.apiEditTag($this.formEditTags.tagName);
      }
    });
  },

  btnRemarkSubmitClick: function () {
    var $this = this;

    this.$refs.formRemark.validate(function(valid) {
      if (valid) {
        $this.apiRemark();
      }
    });
  },

  btnDeleteTagClick: function () {
    var $this = this;

    utils.alertDelete({
      title: '删除标签',
      text: '确定删除标签 ' + this.formEditTags.tagName + ' 吗？',
      callback: function () {
        $this.apiDeleteTag();
      }
    });
  },

  btnBlockClick: function() {
    var $this = this;

    utils.alertDelete({
      title: '加入黑名单',
      text: '确定将选中用户加入黑名单吗？',
      button: '加入黑名单',
      callback: function () {
        $this.apiBlock();
      }
    });
  },

  btnUnBlockClick: function() {
    var $this = this;

    utils.alertDelete({
      title: '移出黑名单',
      text: '确定将选中用户移出黑名单吗？',
      button: '移出黑名单',
      callback: function () {
        $this.apiUnBlock();
      }
    });
  },

  formatSubscribeScene: function(subscribeScene) {
    if (subscribeScene === 'ADD_SCENE_SEARCH') return '公众号搜索';
    if (subscribeScene === 'ADD_SCENE_ACCOUNT_MIGRATION') return '公众号迁移';
    if (subscribeScene === 'ADD_SCENE_PROFILE_CARD') return '名片分享';
    if (subscribeScene === 'ADD_SCENE_QR_CODE') return '扫描二维码';
    if (subscribeScene === 'ADD_SCENEPROFILE_LINK') return '图文页内名称点击';
    if (subscribeScene === 'ADD_SCENE_PROFILE_ITEM') return '图文页右上角菜单';
    if (subscribeScene === 'ADD_SCENE_PAID') return '支付后关注';
    if (subscribeScene === 'ADD_SCENE_OTHERS') return '其他';
    return '其他';
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  computed: {
    isChecked: function() {
      return this.multipleSelection.length > 0;
    },

    openIds: function() {
      var retVal = [];
      for (var i = 0; i < this.multipleSelection.length; i++) {
        var content = this.multipleSelection[i];
        retVal.push(content.openId);
      }
      return retVal;
    },
  },
  created: function () {
    this.apiGet(1);
  }
});