var $url = '/admin/cms/templates/templatesMatch';

var data = utils.initData({
  siteId: utils.getQueryInt("siteId"),
  channels: null,
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
  apiConfig: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId
      }
    }).then(function (response) {
      var res = response.data;

      $this.channels = [res.channels];
      $this.channelTemplates = res.channelTemplates;
      $this.contentTemplates = res.contentTemplates;

      $this.expandedChannelIds = [$this.siteId];
      $this.defaultChannelTemplate = _.find($this.channelTemplates, function(o) { return o.defaultTemplate; }) || {id: 0};
      $this.defaultContentTemplate = _.find($this.contentTemplates, function(o) { return o.defaultTemplate; }) || {id: 0};
      $this.channelTemplateId = $this.defaultChannelTemplate.id;
      $this.contentTemplateId = $this.defaultContentTemplate.id;
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function (data) {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, data).then(function (response) {
      var res = response.data;

      $this.channels = [res.value];

      $this.expandedChannelIds = $this.channelIds;
      $this.filterText = '';
      $this.filterChannelTemplateId = 0;
      $this.filterContentTemplateId = 0;
      $this.$message.success('模板匹配成功！');
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiCreate: function (data) {
    var $this = this;

    utils.loading(this, true);
    $api.post($url + '/actions/create', data).then(function (response) {
      var res = response.data;

      $this.channels = [res.channels];
      $this.channelTemplates = res.channelTemplates;
      $this.contentTemplates = res.contentTemplates;

      $this.expandedChannelIds = $this.channelIds;
      $this.filterText = '';
      $this.filterChannelTemplateId = 0;
      $this.filterContentTemplateId = 0;
      $this.$message.success('模板创建并匹配成功！');
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  getChannelUrl: function(data) {
    return utils.getRootUrl('redirect', {
      siteId: this.siteId,
      channelId: data.value
    });
  },

  getChannelTemplateName: function(channelTemplateId) {
    var template = _.find(this.channelTemplates, function(o) { return o.id === channelTemplateId; });
    return template ? template.templateName : this.defaultChannelTemplate.templateName;
  },

  getContentTemplateName: function(contentTemplateId) {
    var template = _.find(this.contentTemplates, function(o) { return o.id === contentTemplateId; });
    return template ? template.templateName : this.defaultContentTemplate.templateName;
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

  handleCheckChange() {
    this.channelIds = this.$refs.tree.getCheckedKeys();
  },

  btnCreateClick: function (command) {
    var message = '';
    var data = null;
    if (command === 'CreateChannelTemplate') {
      message = '此操作将创建空的栏目模板并匹配选中栏目, 是否继续?';
      data = {
        siteId: this.siteId,
        channelIds: this.channelIds,
        isChannelTemplate: true,
        isChildren: false,
      };
    }
    else if (command === 'CreateSubChannelTemplate') {
      message = '此操作将创建空的栏目模板并匹配选中栏目的下级栏目, 是否继续?';
      data = {
        siteId: this.siteId,
        channelIds: this.channelIds,
        isChannelTemplate: true,
        isChildren: true,
      };
    }
    else if (command === 'CreateContentTemplate') {
      message = '此操作将创建空的内容模板并匹配选中栏目, 是否继续?';
      data = {
        siteId: this.siteId,
        channelIds: this.channelIds,
        isChannelTemplate: false,
        isChildren: false,
      };
    }
    else if (command === 'CreateSubContentTemplate') {
      message = '此操作将创建空的内容模板并匹配选中栏目的下级栏目, 是否继续?';
      data = {
        siteId: this.siteId,
        channelIds: this.channelIds,
        isChannelTemplate: false,
        isChildren: true,
      };
    }

    var $this = this;
    this.$confirm(message, '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    }).then(function() {
      $this.apiCreate(data);
    });
  },

  btnChannelMatchClick: function() {
    this.apiSubmit({
      siteId: this.siteId,
      channelIds: this.channelIds,
      isChannelTemplate: true,
      templateId: this.channelTemplateId
    });
  },

  btnContentMatchClick: function() {
    this.apiSubmit({
      siteId: this.siteId,
      channelIds: this.channelIds,
      isChannelTemplate: false,
      templateId: this.contentTemplateId
    });
  }
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
    this.apiConfig();
  }
});