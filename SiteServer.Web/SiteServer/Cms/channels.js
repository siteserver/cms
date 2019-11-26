var $url = "/pages/cms/channels"

var data = {
  siteId: parseInt(utils.getQueryString("siteId")),
  pageLoad: false,
  pageAlert: null,
  tableData: [],
  channels: [],
  expandedChannelIds: [],
  formInline: {
    filterText: '',
    groupName: ''
  },

  appendPanel: false,
  appendParent: null,
  appendAlert: null,
  appendForm: null,
  appendLoading: false,
  appendRules: {
    channels: [
      { required: true, message: '请输入栏目名称', trigger: 'blur' }
    ]
  },
};

var methods = {
  apiGet: function() {
    var $this = this;

    $api.get($url, {
      params: {
        siteId: this.siteId
      }
    }).then(function (response) {
      var res = response.data;

      $this.channels.push(res);
      $this.expandedChannelIds.push(res.id);
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
    });
  },

  apiAppend: function () {
    var $this = this;

    $api.post($url + '/actions/append', this.appendForm).then(function (response) {
      var res = response.data;

      var channels = res;
      if (!$this.appendParent.children) {
        $this.$set($this.appendParent, 'children', [])
      }
      for (var i = 0; i < channels.length; i++) {
        $this.appendParent.children.push(channels[i]);
      }

      if ($this.expandedChannelIds.indexOf($this.appendParent.id) ==- -1) {
        $this.expandedChannelIds.push($this.appendParent.id);
      }
      $this.appendPanel = false
      $this.appendParent = null
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    });
  },

  appendChannels: function(channels) {
    
  },

  btnAppendClick: function(data) {
    this.appendPanel = true
    this.appendParent = data
  },

  addChannels1: function() {
    this.show1 = true
  },

  addChannels2: function() {
    this.show2 = true
  },

  handleDragStart: function(node, ev) {
    console.log('drag start', node)
  },

  handleDragEnter: function(draggingNode, dropNode, ev) {
    console.log('tree drag enter: ', dropNode.channelName)
  },

  handleDragLeave: function(draggingNode, dropNode, ev) {
    console.log('tree drag leave: ', dropNode.channelName)
  },

  handleDragOver: function(draggingNode, dropNode, ev) {
    console.log('tree drag over: ', dropNode.channelName)
  },
  
  handleDragEnd: function(draggingNode, dropNode, dropType, ev) {
    console.log('tree drag end: ', dropNode && dropNode.channelName, dropType)
  },

  handleDrop: function(draggingNode, dropNode, dropType, ev) {
    console.log('tree drop: ', dropNode.channelName, dropType)
  },

  allowDrop: function(draggingNode, dropNode, type) {
    if (dropNode.data.channelName === '二级 3-1') {
      return type !== 'inner'
    } else {
      return true
    }
  },

  allowDrag: function(draggingNode) {
    return draggingNode.data.channelName.indexOf('三级 3-2-2') === -1
  },

  getCheckedNodes: function() {
    console.log(this.$refs.tree.getCheckedNodes())
  },
  
  getCheckedKeys: function() {
    console.log(this.$refs.tree.getCheckedKeys())
  },

  setCheckedNodes: function() {
    this.$refs.tree.setCheckedNodes([{
      id: 5,
      channelName: '二级 2-1'
    }, {
      id: 9,
      channelName: '三级 1-1-1'
    }])
  },

  setCheckedKeys: function() {
    this.$refs.tree.setCheckedKeys([3])
  },

  resetChecked: function() {
    this.$refs.tree.setCheckedKeys([])
  },

  filterNode: function(value, data) {
    if (!value) return true
    return data.channelName.indexOf(value) !== -1
  },

  remove: function(node, data) {
    this.$confirm(`此操作将永久删除栏目 ${data.channelName}, 是否继续?`, '提示', {
      confirmButtonText: '永久删除',
      cancelButtonText: '取消',
      type: 'warning'
    }).then(async() => {
      const channel = await deleteChannel(this.siteId, data.id)

      const parent = node.parent
      const children = parent.data.children || parent.data
      const index = children.findIndex(d => d.id === channel.id)
      children.splice(index, 1)

      this.$message({
        type: 'success',
        message: '删除成功!'
      })
    })
  },

  btnAppendClick: function(row) {
    this.appendParent = row;
    this.appendForm = {
      siteId: this.siteId,
      parentId: this.appendParent.id,
      isIndexName: false,
      channels: ''
    };
    this.appendPanel = true;
  },

  btnAppendCancelClick: function() {
    this.appendPanel = false;
    this.appendLoading = false;
    this.appendAlert = null;
  },

  btnAppendSubmitClick: function() {
    var $this = this;
    this.$refs.appendForm.validate(function(valid) {
      if (valid) {
        $this.appendLoading = true;
        $this.apiAppend();
      }
    });
  },
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    this.apiGet();
  }
});