var $url = '/wx/users';

var data = utils.init({
  success: false,
  errorMessage: null,
  tags: null,
  total: null,
  count: null,
  users: null,
  multipleSelection: [],
  form: {
    siteId: utils.getQueryInt('siteId'),
    init: true,
    tagId: 0,
    keyword: null,
    page: 1,
    perPage: 20
  }
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

  btnViewClick: function() {

  },

  btnSearchClick() {
    this.apiGet(1);
  },

  btnPageClick: function(val) {
    this.apiGet(val);
  },

  btnEditClick: function (type) {
    
  },

  btnAddClick: function () {

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