var $url = '/pages/cms/create';
var $urlAll = '/pages/cms/create/all';
var $siteId = parseInt(utils.getQueryString('siteId'));
var $type = utils.getQueryString('type');

var $data = {
  pageLoad: false,
  pageAlert: null,
  pageType: null,
  siteId: $siteId,
  type: $type,
  root: null,
  pageChannels: [],
  isAllChecked: false,
  isDescendent: false,
  isChannelPage: $type === 'channels',
  isContentPage: $type === 'contents',
  scope: 'all'
};

var $methods = {
  displayChildren: function (channel, event) {
    channel.isOpen ? this.hideChildren(channel) : this.showChildren(channel);
    event.stopPropagation();
  },

  toggleChecked: function (channel) {
    channel.isChecked = !channel.isChecked;
    if (!channel.isChecked) {
      this.isAllChecked = false;
    }
  },

  selectAll: function () {
    this.isAllChecked = !this.isAllChecked;
    for (var i = 0; i < this.pageChannels.length; i++) {
      this.pageChannels[i].isChecked = this.isAllChecked;
    }
    this.reload();
  },

  create: function () {
    var $this = this;

    var channelIdList = [];
    for (var i = 0; i < this.pageChannels.length; i++) {
      if (this.pageChannels[i].isChecked) {
        channelIdList.push(this.pageChannels[i].id);
      }
    }

    if (!$this.isAllChecked && channelIdList.length === 0) return;
    if (!$this.isChannelPage && !this.isContentPage) return;

    utils.loading(true);
    $api.post($url, {
      siteId: $this.siteId,
      channelIdList: $this.isAllChecked ? [] : channelIdList,
      isAllChecked: $this.isAllChecked,
      isDescendent: $this.isDescendent,
      isChannelPage: $this.isChannelPage,
      isContentPage: $this.isContentPage,
      scope: $this.scope
    }).then(function (response) {
      var res = response.data;

      location.href = 'createStatus.cshtml?siteId=' + $this.siteId;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  createIndex: function () {
    var $this = this;

    utils.loading(true);
    $api.post($url, {
      siteId: $this.siteId,
      channelIdList: [$this.siteId],
      isAllChecked: false,
      isDescendent: false,
      isChannelPage: true,
      isContentPage: false
    }).then(function (response) {
      var res = response.data;

      location.href = 'createStatus.cshtml?siteId=' + $this.siteId;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  createAll: function () {
    var $this = this;

    utils.loading(true);
    $api.post($urlAll, {
      siteId: $this.siteId
    }).then(function (response) {
      var res = response.data;

      location.href = 'createStatus.cshtml?siteId=' + $this.siteId;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  loadChannels: function () {
    var $this = this;

    $api.get($url, {
      params: {
        siteId: $siteId,
        parentId: $siteId
      }
    }).then(function (response) {
      var res = response.data;

      $this.root = _.assign({}, res.parent, {
        contentNum: res.countDict[res.parent.id],
        isOpen: true,
        isLoading: false,
        isChecked: false,
        children: []
      });

      for (var i = 0; i < res.value.length; i++) {
        var channel = _.assign({}, res.value[i], {
          contentNum: res.countDict[res.value[i].id],
          isOpen: false,
          isLoading: false,
          isChecked: false,
          children: []
        });
        $this.root.children.push(channel);
      }

      $this.reload();
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
    });
  },

  reload: function () {
    this.pageChannels = [];
    this.addPageChannels(this.root);
  },

  addPageChannels: function (channel) {
    this.pageChannels.push(channel);
    if (channel.isOpen) {
      for (var i = 0; i < channel.children.length; i++) {
        var child = channel.children[i];
        this.addPageChannels(child);
      }
    }
  },

  showChildren: function (channel) {
    var $this = this;

    channel.isOpen = true;
    if (channel.childrenCount > 0 && channel.children.length === 0) {
      channel.children = [];
      channel.isLoading = true;

      $api.get($url, {
        params: {
          siteId: $siteId,
          parentId: channel.id
        }
      }).then(function (response) {
        var res = response.data;

        for (var i = 0; i < res.value.length; i++) {
          var child = _.assign({}, res.value[i], {
            contentNum: res.countDict[res.value[i].id],
            isOpen: false,
            isLoading: false,
            isChecked: $this.isAllChecked,
            children: []
          });

          channel.children.push(child);
        }
      }).catch(function (error) {
        $this.pageAlert = utils.getPageAlert(error);
      }).then(function () {
        channel.isLoading = false;

        $this.reload();
      });
    } else {
      $this.reload();
    }
  },

  hideChildren: function (channel) {
    channel.isOpen = false;
    this.reload();
  }
};

var $vue = new Vue({
  el: '#main',
  data: $data,
  methods: $methods,
  created: function () {
    if (this.type === 'index') {
      this.createIndex();
    } else if (this.type === 'all') {
      this.createAll();
    } else {
      this.loadChannels();
    }
  }
});