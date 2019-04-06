<template>
  <div class="app-sidebar sidebar-shadow">
    <div id="scrollbar-sidebar" class="scrollbar-sidebar">
      <div class="app-sidebar__inner">
        <ul class="vertical-nav-menu">
          <li class="app-sidebar__heading">
            管理
          </li>
          <!-- <li class="mm-active">
            <a href="#">
              <i class="icon pe-7s-global" /> www.siteserver.cn <i class="state pe-7s-angle-down caret-left" />
            </a>
            <ul class="mm-collapse mm-show">
              <li>
                <a href="components-tabs.html">
                  所有站点
                </a>
              </li>
              <li>
                <a href="components-accordions.html">
                  切换站点
                </a>
              </li>
              <li>
                <a href="components-notifications.html">
                  新增站点
                </a>
              </li>
            </ul>
          </li> -->
          <li>
            <a href="#">
              <i class="icon pe-7s-graph" /> 统计
            </a>
          </li>
          <li>
            <a href="#">
              <i class="icon pe-7s-timer" /> 活动
            </a>
          </li>
          <li>
            <a href="#">
              <i class="icon pe-7s-look" /> 查看
            </a>
          </li>
          <li class="app-sidebar__heading">
            管理
          </li>
          <li>
            <a href="#">
              <i class="icon pe-7s-id" /> 内容管理
            </a>
          </li>
          <li>
            <a href="#">
              <i class="icon pe-7s-config" /> 账户设置
            </a>
          </li>
          <li>
            <a href="tables-regular.html">
              <i class="icon pe-7s-credit" /> 管理购买
            </a>
          </li>
          <li>
            <a href="tables-regular.html">
              <i class="icon pe-7s-lock" /> 安全性
            </a>
          </li>
          <li>
            <a href="tables-regular.html">
              <i class="icon pe-7s-look" /> 隐私权
            </a>
          </li>
          <li>
            <a href="tables-regular.html">
              <i class="icon pe-7s-bell" /> 通知设置
            </a>
          </li>

          <li class="app-sidebar__heading">
            配置
          </li>
          <li>
            <a href="tables-regular.html">
              <i class="icon pe-7s-users" /> 团队 <button class="btn btn-sm btn-light mr-2">
                添加
              </button>
            </a>
          </li>
          <li>
            <n-link :to="getMenuUrl('settings/general')" :class="{'mm-active': menuName === 'settings'}">
              <i class="icon pe-7s-config" /> 设置
            </n-link>
          </li>
          <li>
            <a href="#">
              <i class="icon pe-7s-medal" /> 套餐 <span class="mr-3">
                免费
              </span>
            </a>
          </li>
        </ul>
      </div>
    </div>
  </div>
</template>

<script>
import 'perfect-scrollbar/css/perfect-scrollbar.css'
import PerfectScrollbar from 'perfect-scrollbar'

export default {
  name: 'AppRepoMenus',
  props: {
    menuName: {
      type: String,
      default: ''
    }
  },
  mounted() {
    // eslint-disable-next-line
    new PerfectScrollbar('#scrollbar-sidebar', { wheelSpeed: 2, wheelPropagation: false, minScrollbarLength: 20 })
  },
  methods: {
    getMenuUrl(name) {
      return `/${this.$route.params.owner}/${this.$route.params.name}/${name}`
    }
  }
}
</script>
