var $urlCloud = "home/my/tickets";
var $urlCloudUpload = 'home/my/tickets/actions/upload';

var data = utils.init({
  isTicket: false,
  isAdd: false,
  count: 0,
  tickets: [],
  isMessages: false,
  messagesCount: 0,
  messages: [],
  isAdd: false,
  formInline: {
    status: 'All',
    keyword: '',
    currentPage: 1,
    offset: 0,
    limit: 30,
  },
  form: {
    priority: 'Normal',
    category: 'Cms',
    content: '',
    fileUrls: [],
  },
  loading: false,
  uploadToken: '',
  uploadUrl: '',
  fileList: [],
});

var methods = {
  apiGet: function() {
    var $this = this;

    utils.loading(this, true);
    cloud
      .get($urlCloud, {
        params: this.formInline,
      })
      .then(function (response) {
        var res = response.data;

        $this.isTicket = res.isTicket;
        $this.count = res.count;
        $this.tickets = res.tickets;
        $this.uploadToken = $cloudToken;
        $this.uploadUrl = cloud.defaults.baseURL + '/' + $urlCloudUpload;
      })
      .catch(function (error) {
        utils.error(error);
      })
      .then(function () {
        utils.loading($this, false);
      });
  },

  apiSubmit: function() {
    var $this = this;

    cloud.post($urlCloud, this.form)
    .then(function (response) {
      var res = response.data;

      if (!res.value) return;
      utils.success('工单提交成功！');
      $this.apiGet();
      $this.isAdd = false;
    })
    .catch(function (error) {
      utils.error(error);
    })
    .then(function () {
      $this.loading = false;
    });
  },

  getAllTicketPriority() {
    return ['Normal', 'High'];
  },

  getAllTicketStatus: function() {
    return ['All', 'Waiting', 'Dealing', 'Closed'];
  },

  getTicketStatus: function(status) {
    if (status === 'Waiting') return '待您确认';
    if (status === 'Dealing') return '正在为您处理';
    if (status === 'Closed') return '工单已关闭';
    return '全部';
  },

  getTicketStatusType: function(ticket) {
    if (ticket.status === 'Closed') {
      return 'success';
    }
    if (ticket.status === 'Dealing') {
      return 'info';
    }
    return 'danger';
  },

  getTicketPriority: function(priority) {
    if (priority === 'High') return '加急工单';
    return '普通工单';
  },

  getTicketCategory: function(category) {
    if (category === 'Cms') return 'CMS问题';
    if (category === 'Cloud') return '网站云问题';
    if (category === 'Theme') return '模板问题';
    if (category === 'Extension') return '插件问题';
    return '其他';
  },

  btnSearchClick: function() {
    this.apiGet();
  },

  handleCurrentChange: function(val) {
    this.formInline.currentPage = val;
    this.formInline.offset = this.formInline.limit * (val - 1);

    this.btnSearchClick();
  },

  btnViewClick: function(ticket) {
    utils.addTab('工单：' + ticket.ticketNo, utils.getSettingsUrl('cloudTicketMessages', {
      ticketNo: ticket.ticketNo,
      tabName: utils.getTabName()
    }));
  },

  btnAddClick: function() {
    if (this.isTicket) {
      this.isAdd = true;
    } else {
      alert({
        title: '提交工单',
        text: '系统检测到您的云助手版本为免费版，暂无法提交工单！',
        type: 'warning',
        confirmButtonText: '关 闭',
        showConfirmButton: true,
        showCancelButton: false,
        buttonsStyling: false,
      });
    }
  },

  handleRemove: function(file) {
    this.fileList.splice(this.fileList.indexOf(file), 1);
    console.log(this.fileList);
  },

  handlePreview: function(file) {
    window.open(file.url);
  },

  uploadBefore: function(file) {
    var re = /(\.jpg|\.png)$/i;
    if (!re.exec(file.name)) {
      utils.error('截图只能是图片格式，请选择有效的文件上传!');
      return false;
    }

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
    this.fileList.push(res);
    utils.loading(this, false);
  },

  uploadError: function(err) {
    utils.loading(this, false);
    var error = JSON.parse(err.message);
    utils.error(error.message);
  },

  btnSubmitClick: function() {
    var $this = this;
    this.$refs.form.validate(function(valid) {
      if (valid) {
        $this.loading = true;
        $this.form.fileUrls = [];
        for (const file of $this.fileList) {
          $this.form.fileUrls.push(file.url);
        }
        $this.apiSubmit();
      }
    });
  },

  btnCancelClick: function() {
    this.isAdd = false;
  },
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    var $this = this;
    cloud.checkAuth(function () {
      $this.apiGet();
    });
  },
});
