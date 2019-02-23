var $url = '/pages/settings/administrators';
var $urlActionsLock = '/pages/settings/administrators/actions/lock';
var $urlActionsUnLock = '/pages/settings/administrators/actions/unlock';
var $urlActionsDelete = '/pages/settings/administrators/actions/delete';

var $data = {
  pageLoad: false,
  pageAlert: null,
  pageContents: null,
  count: null,
  pages: null,
  roles: null,
  departments: null,
  areas: null,
  pageOptions: null,
  isAllChecked: false,
  roleName: '',
  order: '',
  departmentId: 0,
  areaId: 0,
  keyword: '',
  searchParams: null
};

var $methods = {
  toggleChecked: function (content) {
    content.isSelected = !content.isSelected;
    if (!content.isSelected) {
      this.isAllChecked = false;
    }
  },

  selectAll: function () {
    this.isAllChecked = !this.isAllChecked;
    for (var i = 0; i < this.pageContents.length; i++) {
      this.pageContents[i].isSelected = this.isAllChecked;
    }
  },

  loadFirstPage: function () {
    if (this.page === 1) return;
    this.loadContents(1);
  },

  loadPrevPage: function () {
    if (this.page - 1 <= 0) return;
    this.loadContents(this.page - 1);
  },

  loadNextPage: function () {
    if (this.page + 1 > this.pages) return;
    this.loadContents(this.page + 1);
  },

  loadLastPage: function () {
    if (this.page + 1 > this.pages) return;
    this.loadContents(this.pages);
  },

  onPageSelect: function (option) {
    this.loadContents(option);
  },

  scrollToTop: function () {
    document.documentElement.scrollTop = document.body.scrollTop = 0;
  },

  btnSearchClick: function () {
    this.searchParams = {
      roleName: this.roleName,
      order: this.order,
      departmentId: this.departmentId,
      areaId: this.areaId,
      keyword: this.keyword
    }
    this.loadContents(1);
  },

  loadContents: function (page) {
    var $this = this;

    if ($this.pageLoad) {
      utils.loading(true);
    }

    var params = _.assign({page: page}, this.searchParams);

    $api.get($url, {
        params: params
      }).then(function (response) {
        var res = response.data;

        var pageContents = [];
        for (var i = 0; i < res.value.length; i++) {
          var content = _.assign({}, res.value[i], {
            isSelected: false
          });
          pageContents.push(content);
        }
        $this.pageContents = pageContents;

        $this.count = res.count;
        $this.pages = res.pages;
        $this.roles = res.roles;
        $this.departments = res.departments;
        $this.areas = res.areas;
        $this.page = page;
        $this.pageOptions = [];
        for (var i = 1; i <= $this.pages; i++) {
          $this.pageOptions.push(i);
        }
      }).catch(function (error) {
        $this.pageAlert = utils.getPageAlert(error);
      }).then(function () {
        if ($this.pageLoad) {
          utils.loading(false);
          $this.scrollToTop();
        } else {
          $this.pageLoad = true;
        }
      });
  },

  btnPermissionsClick: function(content) {
    utils.openLayer({
      title: '权限设置',
      url: 'modalPermissionsSet.aspx?userName=' + content.userName,
      full: true
    });
  },

  btnLockClick: function () {
    if (this.selectedContentIds.length === 0) return;
    var $this = this;

    swal2({
        title: '锁定管理员',
        text: '此操作将把所选管理员设置为锁定状态，确定吗？',
        type: 'question',
        confirmButtonText: '锁 定',
        confirmButtonClass: 'btn btn-danger',
        showCancelButton: true,
        cancelButtonText: '取 消'
      })
      .then(function (result) {
        if (result.value) {
          utils.loading(true);
          $api.post($urlActionsLock, {
            adminIds: $this.selectedContentIds.join(",")
          }).then(function (response) {
            var res = response.data;

            swal2.fire({
              title: '锁定管理员',
              text: '成功锁定所选管理员',
              type: 'success',
              confirmButtonText: '确 定',
              confirmButtonClass: 'btn btn-primary',
              showCancelButton: false
            });

            $this.loadContents(1);
          }).catch(function (error) {
            $this.pageAlert = utils.getPageAlert(error);
          }).then(function () {
            utils.loading(false);
          });
        }
      });
  },

  btnUnLockClick: function() {
    if (this.selectedContentIds.length === 0) return;
    var $this = this;

    swal2({
        title: '解锁管理员',
        text: '此操作将解锁所选管理员，确定吗？',
        type: 'question',
        confirmButtonText: '解 锁',
        confirmButtonClass: 'btn btn-danger',
        showCancelButton: true,
        cancelButtonText: '取 消'
      })
      .then(function (result) {
        if (result.value) {
          utils.loading(true);
          $api.post($urlActionsUnLock, {
            adminIds: $this.selectedContentIds.join(",")
          }).then(function (response) {
            var res = response.data;

            swal2.fire({
              title: '解锁管理员',
              text: '成功解锁所选管理员',
              type: 'success',
              confirmButtonText: '确 定',
              confirmButtonClass: 'btn btn-primary',
              showCancelButton: false
            });

            $this.loadContents(1);
          }).catch(function (error) {
            $this.pageAlert = utils.getPageAlert(error);
          }).then(function () {
            utils.loading(false);
          });
        }
      });
  },

  btnDeleteClick: function () {
    if (this.selectedContentIds.length === 0) return;
    var $this = this;

    swal2({
        title: '删除管理员',
        text: '此操作将删除所选管理员，确定吗？',
        type: 'question',
        confirmButtonText: '删 除',
        confirmButtonClass: 'btn btn-danger',
        showCancelButton: true,
        cancelButtonText: '取 消'
      })
      .then(function (result) {
        if (result.value) {
          utils.loading(true);
          $api.post($urlActionsDelete, {
            adminIds: $this.selectedContentIds.join(",")
          }).then(function (response) {
            var res = response.data;

            swal2.fire({
              title: '删除管理员',
              text: '成功删除所选管理员',
              type: 'success',
              confirmButtonText: '确 定',
              confirmButtonClass: 'btn btn-primary',
              showCancelButton: false
            });

            $this.loadContents(1);
          }).catch(function (error) {
            $this.pageAlert = utils.getPageAlert(error);
          }).then(function () {
            utils.loading(false);
          });
        }
      });
  }
};

Vue.component("multiselect", window.VueMultiselect.default);

new Vue({
  el: '#main',
  data: $data,
  methods: $methods,
  computed: {
    selectedContentIds: function () {
      var retval = [];
      if (this.pageContents) {
        for (var i = 0; i < this.pageContents.length; i++) {
          if (this.pageContents[i].isSelected) {
            retval.push(this.pageContents[i].id);
          }
        }
      }
      return retval;
    }
  },
  created: function () {
    this.loadContents(1);
  }
});
