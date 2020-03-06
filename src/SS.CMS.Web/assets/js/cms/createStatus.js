var $url = '/admin/cms/create/createStatus';

var data = utils.initData({
  siteId: utils.getQueryInt('siteId'),
  pageType: null,
  tasks: null,
  channelsCount: null,
  contentsCount: null,
  filesCount: null,
  specialsCount: null,
  timeoutId: null
});

var methods = {
  load: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: $this.siteId
      }
    }).then(function(response) {
      var res = response.data;

      $this.tasks = res.value.tasks;
      $this.channelsCount = res.value.channelsCount;
      $this.contentsCount = res.value.contentsCount;
      $this.filesCount = res.value.filesCount;
      $this.specialsCount = res.value.specialsCount;
    })
    .catch(function(error) {
      utils.error($this, error);
    })
    .then(function() {
      $this.timeoutId = setTimeout(function () {
        $this.load();
      }, 3000);
      utils.loading($this, false);
    });
  },

  btnRedirectClick: function (task) {
    var query = {
      siteId: task.siteId
    };

    if (task.channelId) {
      query.channelId = task.channelId;
    }
    if (task.contentId) {
      query.contentId = task.contentId;
    }
    if (task.fileTemplateId) {
      query.fileTemplateId = task.fileTemplateId;
    }
    if (task.specialId) {
      query.specialId = task.specialId;
    }
    
    window.open(utils.getRootUrl('redirect', query));
  },

  btnCancelClick: function () {
    clearTimeout(this.timeoutId);
    var $this = this;

    utils.loading(this, true);
    $api
      .post($url + '/actions/cancel', {
        siteId: this.siteId
      })
      .then(function(response) {
        var res = response.data;

        $this.load();
      })
      .catch(function(error) {
        utils.error($this, error);
      });
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.load();
  }
});