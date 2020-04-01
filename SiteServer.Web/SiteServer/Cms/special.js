var $api = new apiUtils.Api(apiUrl + '/pages/cms/special');
var $siteId = parseInt(pageUtils.getQueryStringByName('siteId'));

var data = {
  pageLoad: false,
  pageAlert: null,
  items: null,
  siteUrl: null
};

var methods = {
  getList: function () {
    var $this = this;

    $api.get({siteId: $siteId}, function (err, res) {
      if (err || !res || !res.value) return;

      $this.items = res.value;
      $this.siteUrl = res.siteUrl;
      $this.pageLoad = true;
    });
  },

  delete: function (specialId) {
    var $this = this;

    pageUtils.loading(true);
    $api.delete({
      siteId: $siteId,
      specialId: specialId
    }, function (err, res) {
      pageUtils.loading(false);
      if (err || !res || !res.value) return;

      $this.items = res.value;
    });
  },

  submit: function (item) {
    var $this = this;

    pageUtils.loading(true);
    $api.post(item, function (err, res) {
      pageUtils.loading(false);
      if (err) {
        $this.pageAlert = {
          type: 'danger',
          html: err.message
        };
        return;
      }

      $this.pageAlert = {
        type: 'success',
        html: item.id === -1 ? '专题添加成功！' : '专题修改成功！'
      };
      $this.item = null;
      $this.items = res.value;
    });
  },

  btnEditClick: function (item) {
    utils.openLayer({
      title: '编辑专题',
      url: 'specialAddLayer.cshtml?siteId=' + $siteId + '&isEditOnly=true&specialId=' + item.id,
      width: 500,
      height: 400
    });
    return false;
  },

  btnUploadClick: function (item) {
    utils.openLayer({
      title: '上传文件',
      url: 'specialAddLayer.cshtml?siteId=' + $siteId + '&isUploadOnly=true&specialId=' + item.id,
      full: true
    });
    return false;
  },

  btnDownloadClick: function(item){
    var $this = this;

    pageUtils.loading(true);
    $api.postAt('actions/download', {
      siteId: $siteId,
      specialId: item.id
    }, function (err, res) {
      pageUtils.loading(false);
      if (err || !res || !res.value) return;

      window.location.href = res.value;
    });
  },

  btnAddClick: function () {
    utils.openLayer({
      title: '新建专题',
      url: 'specialAddLayer.cshtml?siteId=' + $siteId,
      full: true
    });
    return false;
  },

  btnDeleteClick: function (item) {
    var $this = this;

    pageUtils.alertDelete({
      title: '删除专题',
      text: '此操作将删除专题 ' + item.title + '，确定吗？',
      callback: function () {
        $this.delete(item.id);
      }
    });
  }
};

new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.getList();
  }
});