var $url = '/pages/cms/editorLayerText';

var data = {
  attributeName: utils.getQueryString('attributeName'),
  siteId: utils.getQueryInt("siteId"),
  pageLoad: false,
  pageType: 'card',

  groups: null,
  count: null,
  items: null,
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
  },

  isSubmitForm: false,
  submitForm: {
    textId: 0,
    linkText: ''
  }
};

var methods = {
  insert: function(result) {
    var html = '<el-popover trigger="click">' + result.content + '<a href="javascript:;" slot="reference">' + result.linkText + '</a></el-popover>';
    parent.insertHtml(this.attributeName, html);
  },

  apiList: function (page) {
    var $this = this;
    this.form.page = page;

    $api.post($url + '/list', this.form).then(function (response) {
      var res = response.data;

      $this.groups = res.groups;
      $this.count = res.count;
      $this.items = res.items;
      $this.urlList = _.map($this.items, function (item) {
        return item.imageUrl;
      });
    }).catch(function (error) {
      utils.notifyError($this, error);
    }).then(function () {
      $this.pageLoad = true;
      utils.loading(false);
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

  btnSelectClick: function(library) {
    this.isSubmitForm = true;
    this.submitForm.textId = library.id;
  },

  btnSelectGroupClick: function (groupId) {
    this.selectedGroupId = (this.selectedGroupId === groupId) ? 0 :groupId;
  },

  btnGroupClick: function(groupId) {
    var $this = this;

    this.form.groupId = groupId;
    this.form.page = 1;

    utils.loading(true);
    $api.post($url + '/list', this.form).then(function (response) {
      var res = response.data;

      $this.groups = res.groups;
      $this.count = res.count;
      $this.items = res.items;
      $this.urlList = _.map($this.items, function (item) {
        return item.imageUrl;
      });
    }).catch(function (error) {
      $this.$notify.error({
          title: '错误',
          message: error.message
        });
    }).then(function () {
      $this.pageLoad = true;
      utils.loading(false);
    });
  },

  btnDropdownClick: function(command) {
    this.pageType = command;
  },

  btnSubmitClick: function() {
    var $this = this;
    this.$refs.submitForm.validate(function(valid) {
      if (valid) {
        utils.loading(true);
        $api
          .get($url + '/' + $this.submitForm.textId, {
            params: {
              siteId: $this.siteId
            }
          })
          .then(function(response) {
            var res = response.data;

            $this.insert({
              linkText: $this.submitForm.linkText,
              content: res.content
            });
            $this.isSubmitForm = false;
            utils.closeLayer();
          })
          .catch(function(error) {
            utils.notifyError($this, error);
          })
          .then(function() {
            utils.loading(false);
          });
      } else {
        return false;
      }
    });
  },

  btnSearchClick() {
    utils.loading(true);
    this.apiList(1);
  },

  btnPageClick: function(val) {
    utils.loading(true);
    this.apiList(val);
  }
};

new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiList(1);
  }
});