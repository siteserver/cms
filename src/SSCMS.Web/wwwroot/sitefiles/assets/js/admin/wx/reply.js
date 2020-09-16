var $url = '/wx/reply';

var data = utils.init({
  success: false,
  errorMessage: null,
  rules: null,
  count: null,
  multipleSelection: [],
  form: {
    siteId: utils.getQueryInt('siteId'),
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

      $this.success = res.success;
      $this.errorMessage = res.errorMessage;
      $this.rules = res.rules;
      $this.count = res.count;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiDelete: function (ruleId) {
    var $this = this;

    utils.loading(this, true);
    $api.delete($url, {
      data: Object.assign({ruleId: ruleId}, this.form)
    }).then(function (response) {
      var res = response.data;

      $this.rules = res.rules;
      $this.count = res.count;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnSearchClick: function() {
    this.apiGet(1);
  },

  btnPageClick: function(val) {
    this.apiGet(val);
  },

  btnEditClick: function (rule) {
    utils.addTab('编辑回复', utils.getWxUrl('replyAdd', {
      siteId: this.form.siteId,
      ruleId: rule.id,
      tabName: utils.getTabName()
    }));
  },

  btnAddClick: function () {
    utils.addTab('添加回复', utils.getWxUrl('replyAdd', {
      siteId: this.form.siteId,
      tabName: utils.getTabName()
    }));
  },

  btnDeleteClick: function(rule) {
    var $this = this;

    utils.alertDelete({
      title: '删除关键词回复',
      text: '此操作将删除关键词回复，确定吗？',
      callback: function () {
        $this.apiDelete(rule.id);
      }
    });
  },

  getMaterialType: function (materialType) {
    if (materialType === 'Message') return '图文消息';
    if (materialType === 'Image') return '图片';
    if (materialType === 'Audio') return '音频';
    if (materialType === 'Video') return '视频';
    if (materialType === 'Text') return '文字';
    return '';
  },

  getKeywords: function (keywords) {
    if (!keywords) return '';
    return keywords.map(function (x) { return x.text; }).join(', ');
  },

  getMessages: function (messages) {
    var $this = this;
    if (!messages) return '';
    return messages.map(function (x) { return $this.getMaterialType(x.materialType); }).join(', ');
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiGet(1);
  }
});