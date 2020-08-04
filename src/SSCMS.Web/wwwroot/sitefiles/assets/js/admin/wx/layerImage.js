var $url = '/wx/layerImage';

var data = utils.init({
  siteId: utils.getQueryInt("siteId"),
  showType: 'card',
  groups: null,
  count: null,
  images: null,
  urlList: null,
  
  form: {
    siteId: utils.getQueryInt("siteId"),
    keyword: '',
    groupId: -utils.getQueryInt("siteId"),
    page: 1,
    perPage: 24
  }
});

var methods = {
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
      $this.images = res.images;
      $this.urlList = _.map($this.images, function (item) {
        return item.thumbUrl;
      });
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
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

  btnImageClick: function(image) {
    parent.$vue.runLayerImage(image);
    utils.closeLayer();
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
      $this.images = res.images;
      $this.urlList = _.map($this.images, function (item) {
        return item.url;
      });
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
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

  getFriendlyContent: function(image) {
    if (image.items.length === 1) {
      return image.items[0].title;
    }
    var i = 1;
    var contents = image.items.map(function(item) {
      return i++ + '. ' + item.title;
    });
    return contents.join('<br />');
  },

  btnCancelClick: function () {
    utils.closeLayer();
  },
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiList(1);
  }
});