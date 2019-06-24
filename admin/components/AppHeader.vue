<template>
  <div class="app-header app-top-bar top-bar-text-light" :class="{'bg-tempting-azure': !isLoggedIn}">
    <template v-if="!isLoggedIn">
      <div class="fiori-container">
        <div class="top-bar-left">
          <ul class="nav">
            <li class="nav-item pl-2">
              <a href="/" class="nav-link">
                a
              </a>
            </li>
          </ul>
        </div>
        <div class="top-bar-right">
          <ul class="nav">
            <li class="nav-item">
              <n-link class="nav-link" to="/login">
                <span>登 录</span>
              </n-link>
            </li>
            <li class="nav-item pr-2">
              <n-link class="nav-link" to="/register">
                <span>新用户注册</span>
              </n-link>
            </li>
          </ul>
        </div>
      </div>
    </template>
    <template v-else>
      <div class="fiori-container">
        <div class="top-bar-left">
          <ul class="nav">
            <li class="nav-item">
              <a class="nav-link" href="/" />
            </li>
            <li class="nav-item">
              <n-link class="nav-link" :to="'/' + userName">
                <i class="pe-7s-refresh-cloud" style="font-size: 20px" />
                <span class="d-none d-md-block ml-2">
                  {{ getMenuName() }}
                </span>
              </n-link>
            </li>
            <li class="nav-item">
              <n-link class="nav-link" to="/vip">
                <i class="pe-7s-call" style="font-size: 18px" />
                <span class="d-none d-md-block ml-2">
                  VIP服务
                </span>
              </n-link>
            </li>
            <li class="nav-item">
              <n-link class="nav-link" to="/developers">
                <i class="pe-7s-rocket" style="font-size: 20px" />
                <span class="d-none d-md-block ml-2">
                  开发者服务
                </span>
              </n-link>
            </li>
          </ul>
        </div>
        <div class="top-bar-right">
          <ul class="nav">
            <li class="nav-item">
              <n-link class="nav-link" to="/me">
                <img class="img-avatar" :src="avatarUrl">
              </n-link>
            </li>
            <li class="nav-item mr-2">
              <n-link class="nav-link" to="/messages">
                <i class="pe-7s-bell" style="font-size: 22px" />
                <span class="badge badge-pink noti-icon-badge">
                  4
                </span>
              </n-link>
            </li>
          </ul>
        </div>
      </div>
    </template>
  </div>
</template>

<script>
import { mapGetters } from 'vuex'

export default {
  name: 'AppHeader',
  computed: {
    ...mapGetters(['isLoggedIn', 'userName', 'displayName', 'avatarUrl', 'isVip'])
  },
  methods: {
    getMenuName() {
      if (this.$route.path === `/${this.userName}/add` || this.$route.path === `/${this.userName}`) {
        return `CMS 云连接`
      } else if (this.$route.params.userName && this.$route.params.repoName) {
        return `${this.$route.params.userName}/${this.$route.params.repoName}`
      } else if (this.$route.params.userName) {
        return this.$route.params.userName
      } else {
        return 'CMS 云连接'
      }
    }
  }
}
</script>
