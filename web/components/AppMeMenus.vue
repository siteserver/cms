<template>
  <div class="app-sidebar sidebar-shadow d-none d-md-block">
    <div id="scrollbar-sidebar" class="scrollbar-sidebar">
      <div class="app-sidebar__inner">
        <div class="member-card text-center mt-3 mb-5">
          <div class="thumb-xl member-thumb m-b-10 center-block">
            <img :src="avatarUrl" class="rounded-circle img-thumbnail">
          </div>
          <h5 class="m-b-5">
            {{ displayName }}
          </h5>
          <p class="text-muted">
            @{{ userName }}
          </p>
          <a href="../logout/" class="btn btn-warning btn-sm mt-2">
            退出登录
          </a>
        </div>
        <ul class="vertical-nav-menu">
          <li class="app-sidebar__heading">
            个人资料
          </li>
          <li>
            <n-link exact-active-class="mm-active" to="/me">
              <i class="icon pe-7s-id" /> 我的个人资料
            </n-link>
          </li>
          <li>
            <n-link exact-active-class="mm-active" to="/me/settings">
              <i class="icon pe-7s-config" /> 账户设置
            </n-link>
          </li>
          <li>
            <n-link exact-active-class="mm-active" to="/me/password">
              <i class="icon pe-7s-config" /> 修改密码
            </n-link>
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
            VIP服务
          </li>
          <li>
            <a href="tables-regular.html">
              <i class="icon pe-7s-diamond" /> 升级VIP服务
            </a>
          </li>
        </ul>
      </div>
    </div>
  </div>
</template>

<script>
import { mapGetters } from 'vuex'

export default {
  name: 'AppMeMenus',
  props: {
    error: undefined
  },
  computed: {
    ...mapGetters(['isLoggedIn', 'userName', 'displayName', 'avatarUrl', 'isVip'])
  },
  methods: {
    getMenuUrl(name) {
      return `/${this.$route.params.userName}/${this.$route.params.repoName}/${name}`
    }
  }
}
</script>
