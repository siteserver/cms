var $urlCloud = "home/my/ticketMessages";
var $urlCloudUpload = 'home/my/ticketMessages/actions/upload';
var $urlCloudClose = 'home/my/ticketMessages/actions/close';

var data = utils.init({
  ticketNo: utils.getQueryString('ticketNo'),
  tabName: utils.getQueryString('tabName'),
  ticket: {},
  user: {},
  count: 0,
  messages: [],
  currentPage: 1,
  offset: 0,
  limit: 30,
  uploadToken: '',
  uploadUrl: '',
  form: {
    content: '',
  },
});

var methods = {
  apiGet: function() {
    if (!this.ticketNo) return;
    var $this = this;

    utils.loading(this, true);
    cloud.get($urlCloud, {
      params: {
        ticketNo: this.ticketNo,
      },
    })
    .then(function (response) {
      var res = response.data;

      $this.ticket = res.ticket;
      $this.user = res.user;
      $this.count = res.count;
      $this.messages = res.messages;
      $this.uploadToken = $cloudToken;
      $this.uploadUrl = cloud.defaults.baseURL + '/' + $urlCloudUpload;
    })
    .catch(function (error) {
      utils.error(error, {
        ignoreAuth: true,
      });
    })
    .then(function () {
      utils.loading($this, false);
    });
  },

  apiClose: function() {
    var $this = this;

    utils.loading(this, true);
    cloud.post($urlCloudClose, {
      ticketNo: this.ticket.ticketNo,
    })
    .then(function (response) {
      var res = response.data;

      utils.success('工单关闭成功！');
      $this.ticket.status = 'Closed';
    })
    .catch(function (error) {
      utils.error(error, {
        ignoreAuth: true,
      });
    })
    .then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function(messageType, storageKey, content) {
    var $this = this;

    utils.loading(this, true);
    cloud.post($urlCloud, {
      ticketNo: this.ticket.ticketNo,
      messageType: messageType,
      storageKey: storageKey,
      content: content,
    })
    .then(function (response) {
      var res = response.data;

      $this.form.content = '';
      $this.ticket.status = 'Dealing';
      $this.messages.push(res.message);
    })
    .catch(function (error) {
      utils.error(error, {
        ignoreAuth: true,
      });
    })
    .then(function () {
      utils.loading($this, false);
    });
  },

  getTicketPriority: function() {
    if (this.ticket.priority === 'High') return '加急工单';
    return '普通工单';
  },

  getTicketStatusType: function() {
    if (this.ticket.status === 'Closed') {
      return 'success';
    }
    if (this.ticket.status === 'Dealing') {
      return 'info';
    }
    return 'danger';
  },

  getTicketCategory: function() {
    if (this.ticket.category === 'Cms') return 'CMS问题';
    if (this.ticket.category === 'Cloud') return '网站云问题';
    if (this.ticket.category === 'Theme') return '模板问题';
    if (this.ticket.category === 'Extension') return '插件问题';
    return '其他';
  },

  getTicketStatus: function() {
    if (this.ticket.status === 'Waiting') return '待您确认';
    if (this.ticket.status === 'Dealing') return '正在为您处理';
    if (this.ticket.status === 'Closed') return '工单已关闭';
    return '全部';
  },

  isInputArea: function() {
    return this.ticket.status !== 'Closed';
  },

  btnCloseClick: function() {
    var $this = this;
    utils.alertDelete({
      title: '关闭工单',
      text: '此操作将关闭工单，确定吗？',
      button: '确认关闭',
      callback: function() {
        $this.apiClose();
      }
    });
  },

  btnSubmitClick: function() {
    if (this.form.content) {
      this.apiSubmit('Text', '', this.form.content);
    }
  },

  uploadBefore: function(file) {
    var isLt10M = file.size / 1024 / 1024 < 10;
    if (!isLt10M) {
      utils.error('文件大小不能超过 10MB!');
    }
    return isLt10M;
  },

  uploadProgress: function() {
    utils.loading(this, true);
  },

  uploadSuccess: function(res) {
    this.apiSubmit(res.messageType, res.storageKey, res.content);
  },

  uploadError: function(err) {
    utils.loading(this, false);
    var error = JSON.parse(err.message);
    utils.error(error.message);
  },

  getClassName: function(message) {
    return message.isReply ? 'message-item--left' : 'message-item--right';
  },

  isImage: function(message) {
    return message.messageType === 'Image';
  },

  isDocument: function(message) {
    return message.messageType === 'Document';
  },

  getStorageUrl: function(message) {
    return cloud.hostStorage + '/' + message.storageKey;
  },

  getAvatarUrl: function() {
    return this.user.avatarUrl || utils.getAssetsUrl('images/default_avatar.png');
  },

  getFileName: function(url) {
    if (url) {
      return url.substring(url.lastIndexOf('/') + 1);
    }
    return '';
  },

  btnRemoveClick: function() {
    utils.removeTab();
  },
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    utils.keyPress(this.btnSubmitClick, this.btnRemoveClick);
    var $this = this;
    cloud.checkAuth(function () {
      $this.apiGet();
    });
  },
});
