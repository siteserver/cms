var $url = '/cms/create/createPage';

var data = utils.init({
  siteId: utils.getQueryInt('siteId'),
  type: utils.getQueryString('type'),
  isAllChecked: false,
  isDescendent: false,
  isChannelPage: utils.getQueryString('type') === 'Channel',
  isContentPage: utils.getQueryString('type') === 'Content',
  scope: 'all',

  channels: null,
  allChannelIds: null,
  channelTemplates: null,
  contentTemplates: null,

  expandedChannelIds: [],
  filterText: '',
  filterChannelTemplateId: 0,
  filterContentTemplateId: 0,

  defaultChannelTemplate: null,
  defaultContentTemplate: null,

  channelIds: [],
  channelTemplateId: 0,
  contentTemplateId: 0
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId,
        parentId: this.siteId
      }
    }).then(function (response) {
      var res = response.data;

      $this.channels = [res.channels];
      $this.allChannelIds = res.allChannelIds;
      $this.channelTemplates = res.channelTemplates;
      $this.contentTemplates = res.contentTemplates;

      $this.expandedChannelIds = [$this.siteId];
      $this.defaultChannelTemplate = _.find($this.channelTemplates, function(o) { return o.default; }) || {id: 0};
      $this.defaultContentTemplate = _.find($this.contentTemplates, function(o) { return o.default; }) || {id: 0};
      $this.channelTemplateId = $this.defaultChannelTemplate.id;
      $this.contentTemplateId = $this.defaultContentTemplate.id;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiCreate: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, {
      siteId: this.siteId,
      type: this.type,
      channelIdList: this.isAllChecked ? [] : this.channelIds,
      isAllChecked: this.isAllChecked,
      isDescendent: this.isDescendent,
      isChannelPage: this.isChannelPage,
      isContentPage: this.isContentPage,
      scope: this.scope
    }).then(function (response) {
      location.href = utils.getCmsUrl('createStatus', {siteId: $this.siteId});
    }).catch(function (error) {
      utils.error(error);
    });
  },

  apiCreateIndex: function () {
    var $this = this;

    $api.post($url, {
      siteId: this.siteId,
      type: this.type,
      channelIdList: [this.siteId],
      isAllChecked: false,
      isDescendent: false,
      isChannelPage: true,
      isContentPage: false,
    }).then(function (response) {
      location.href = utils.getCmsUrl('createStatus', {siteId: $this.siteId});
    }).catch(function (error) {
      utils.error(error);
    });
  },

  apiCreateAll: function () {
    var $this = this;

    $api.post($url + '/all', {
      siteId: this.siteId
    }).then(function (response) {
      location.href = utils.getCmsUrl('createStatus', {siteId: $this.siteId});
    }).catch(function (error) {
      utils.error(error);
    });
  },

  filterNode: function(value, data) {
    if (!value) return true;
    if (value.channelName && value.channelTemplateId && value.contentTemplateId) {
      return data.label.indexOf(value.channelName) !== -1 && data.channelTemplateId === value.channelTemplateId && data.contentTemplateId === value.contentTemplateId;
    } else if (value.channelName && value.channelTemplateId) {
      return data.label.indexOf(value.channelName) !== -1 && data.channelTemplateId === value.channelTemplateId;
    } else if (value.channelName && value.contentTemplateId) {
      return data.label.indexOf(value.channelName) !== -1 && data.contentTemplateId === value.contentTemplateId;
    } else if (value.channelTemplateId && value.contentTemplateId) {
      return data.channelTemplateId === value.channelTemplateId && data.contentTemplateId === value.contentTemplateId;
    } else if (value.channelName) {
      return data.label.indexOf(value.channelName) !== -1;
    } else if (value.contentTemplateId) {
      return data.contentTemplateId === value.contentTemplateId;
    } else if (value.channelTemplateId) {
      return data.channelTemplateId === value.channelTemplateId;
    }
    return true;
  },

  getChannelTemplateName: function(channelTemplateId) {
    var template = _.find(this.channelTemplates, function(o) { return o.id === channelTemplateId; });
    return (template ? template.templateName : this.defaultChannelTemplate.templateName) || '系统栏目模板';
  },

  getContentTemplateName: function(contentTemplateId) {
    var template = _.find(this.contentTemplates, function(o) { return o.id === contentTemplateId; });
    return (template ? template.templateName : this.defaultContentTemplate.templateName) || '系统内容模板';
  },

  selectAll: function (val) {
    if (val) {
      this.$refs.tree.setCheckedKeys(this.allChannelIds);
    } else {
      this.$refs.tree.setCheckedKeys([]);
    }
  },

  handleCheckChange() {
    this.channelIds = this.$refs.tree.getCheckedKeys();
  },

  btnCreateClick: function() {
    this.apiCreate();
  },

  getChannelUrl: function(data) {
    return utils.getRootUrl('redirect', {
      siteId: this.siteId,
      channelId: data.value
    });
  },
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  watch: {
    filterText: function(val) {
      this.$refs.tree.filter({
        channelName: val,
        channelTemplateId: this.filterChannelTemplateId,
        contentTemplateId: this.filterContentTemplateId
      });
    },
    filterChannelTemplateId: function(val) {
      this.$refs.tree.filter({
        channelName: this.filterText,
        channelTemplateId: val,
        contentTemplateId: this.filterContentTemplateId
      });
    },
    filterContentTemplateId: function(val) {
      this.$refs.tree.filter({
        channelName: this.filterText,
        channelTemplateId: this.filterChannelTemplateId,
        contentTemplateId: val
      });
    }
  },
  created: function () {
    if (this.type === 'Index') {
      this.apiCreateIndex();
    } else if (this.type === 'All') {
      this.apiCreateAll();
    } else {
      this.apiGet();
    }
  }
});