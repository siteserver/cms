<template>
  <div>
    <app-notify />
    <loading :active.sync="loading" :is-full-page="true" loader="dots" color="#00b19d" />
    <div v-if="$nuxt.isOffline" class="alert alert-danger mt-5 mx-2">
      系统检测到网络连接已中断，正在重试...
    </div>
    <div v-if="adminInfo">
      <header id="topnav">
        <div class="topbar-main">
          <a href="https://www.siteserver.cn" target="_blank" class="logo">
            <img src="~assets/images/logo.png">
          </a>
          <template v-if="isDesktop">
            <ul id="topMenus" class="navigation-menu">
              <li v-for="topMenu in topMenus" :key="topMenu.id" class="has-submenu">
                <a :href="getHref(topMenu)" :target="getTarget(topMenu)" @click="btnTopMenuClick(topMenu)">
                  {{ topMenu.text }}
                </a>
                <ul v-if="topMenu.hasChildren" class="submenu">
                  <li v-for="parentMenu in topMenu.children" :key="parentMenu.id" :class="{'has-submenu': parentMenu.children}">
                    <a
                      :href="getHref(parentMenu)"
                      :target="getTarget(parentMenu)"
                      @click="btnTopMenuClick(parentMenu)"
                    >
                      {{ parentMenu.text }}
                    </a>
                    <ul v-if="parentMenu.hasChildren" class="submenu">
                      <li v-for="childMenu in parentMenu.children" :key="childMenu.id">
                        <a
                          :href="getHref(childMenu)"
                          :target="getTarget(childMenu)"
                          @click="btnTopMenuClick(childMenu)"
                        >
                          {{ childMenu.text }}
                        </a>
                      </li>
                    </ul>
                  </li>
                </ul>
              </li>
              <template v-if="isSuperAdmin && status.repository">
                <li class="has-submenu">
                  <a href="cloud/dashboard.cshtml" target="right">
                    云控制台
                  </a>
                  <ul class="submenu">
                    <li>
                      <a href="cloud/dashboard.cshtml" target="right">
                        控制面板
                      </a>
                    </li>
                  </ul>
                </li>
              </template>
              <template v-else-if="isSuperAdmin && status.isAvailable">
                <li>
                  <a href="cloud/connect.cshtml" target="right">
                    <i class="fa fa-flash" /> 开启云连接
                  </a>
                </li>
              </template>
            </ul>
            <div class="menu-extras">
              <ul class="navigation-menu nav navbar-nav navbar-right float-right">
                <li v-if="newVersion || updatePackages.length > 0" class="has-submenu dropdown">
                  <a href="javascript:;">
                    <i class="ion-flash text-warning" />
                    <span class="badge badge-xs badge-warning" style="right: 0;">
                      {{ updatePackages.length + (newVersion
                        ? 1 : 0) }}
                    </span>
                  </a>
                  <div class="submenu card bg-light text-dark" style="width: 19rem; z-index: 11; position: absolute; left: -120px">
                    <div v-if="newVersion" class="card-body pt-1 pb-1">
                      <h5 class="card-title text-success">
                        发现 SiteServer CMS 新版本
                      </h5>
                      <p class="card-text">
                        当前版本：{{ cmsVersion }} <br> 最新版本： {{ newVersion.version }} <br> 发布日期： {{
                          newVersion.published }} <br>
                      </p><div class="text-center">
                        <a href="upgrade.cshtml" class="btn btn-warning btn-block" target="_top">
                          立即升级
                        </a>
                      </div>
                      <hr v-if="updatePackages.length > 0">
                    </div>
                    <div v-if="updatePackages.length > 0" class="card-body pt-1 pb-1">
                      <h5 class="card-title text-success">
                        以下{{ updatePackages.length }}个插件发布了新版本
                      </h5>
                      <p class="card-text">
                        <table class="table table-sm m-0">
                          <tbody>
                            <tr v-for="updatePackage in updatePackages.slice(0, 5)" :key="updatePackage.id">
                              <td class="text-nowrap">
                                {{ updatePackage.pluginId }} <small>
                                  {{ updatePackage.cmsVersion
                                  }} -> {{ updatePackage.version }}
                                </small>
                              </td>
                            </tr>
                            <tr v-if="updatePackages.length > 5">
                              <td class="text-nowrap text-center">
                                ......
                              </td>
                            </tr>
                          </tbody>
                        </table>
                      </p>
                      <div class="text-center mt-2">
                        <a href="plugins/manage.cshtml?pageType=4" class="btn btn-warning btn-block" target="right">
                          立即升级
                        </a>
                      </div>
                    </div>
                  </div>
                </li>
                <li v-if="siteId > 0" class="dropdown">
                  <a href="javascript:;" @click="openPageCreateStatus">
                    <i class="ion-wand" />
                    <span v-if="pendingCount > 0" class="badge badge-xs badge-warning" style="right: 0;">
                      {{ pendingCount }}
                    </span>
                  </a>
                </li>
                <li class="list-inline-item dropdown notification-list has-submenu">
                  <a class="nav-link nav-user" href="javascript:;">
                    <img
                      style="height: 36px;width: 36px;"
                      :src="adminInfo.avatarUrl ? adminInfo.avatarUrl : '/assets/images/default_avatar.png'"
                      class="rounded-circle"
                    >
                  </a>
                  <template v-if="status.repository">
                    <ul class="submenu megamenu" style="left: -150px">
                      <li>
                        <ul>
                          <li>
                            <a :href="'settings/adminView.cshtml?pageType=user&userId=' + adminInfo.id" target="right">
                              {{ adminInfo.level }}：{{ adminInfo.userName }}
                            </a>
                          </li>
                          <li>
                            <a :href="'settings/adminProfile.cshtml?pageType=user&userId=' + adminInfo.id" target="right">
                              修改资料
                            </a>
                          </li>
                          <li>
                            <a :href="'settings/adminPassword.cshtml?pageType=user&userId=' + adminInfo.id" target="right">
                              更改密码
                            </a>
                          </li>
                          <li>
                            <a href="javascript:;" @click="btnLogoutClick">
                              退出系统
                            </a>
                          </li>
                        </ul>
                      </li>
                      <li>
                        <ul v-if="status.user">
                          <li>
                            <a href="cloud/dashboard.cshtml" target="right">
                              云连接账号：{{ status.user.userName }}
                            </a>
                          </li>
                          <li>
                            <a href="cloud/logout.cshtml" target="right">
                              退出云连接
                            </a>
                          </li>
                        </ul>
                        <ul v-else>
                          <li>
                            <a href="javascript:;" @click="btnCloudLoginClick">
                              云连接登录
                            </a>
                          </li>
                          <li>
                            <a href="javascript:;" @click="btnCloudLoginClick">
                              云连接注册
                            </a>
                          </li>
                        </ul>
                      </li>
                    </ul>
                  </template>
                  <template v-else>
                    <ul class="submenu" style="left: -60px">
                      <li>
                        <ul>
                          <li>
                            <a :href="'settings/adminView.cshtml?pageType=user&userId=' + adminInfo.id" target="right">
                              {{ adminInfo.level }}：{{ adminInfo.userName }}
                            </a>
                          </li>
                          <li>
                            <a :href="'settings/adminProfile.cshtml?pageType=user&userId=' + adminInfo.id" target="right">
                              修改资料
                            </a>
                          </li>
                          <li>
                            <a :href="'settings/adminPassword.cshtml?pageType=user&userId=' + adminInfo.id" target="right">
                              更改密码
                            </a>
                          </li>
                          <li>
                            <a href="javascript:;" @click="btnLogoutClick">
                              退出系统
                            </a>
                          </li>
                        </ul>
                      </li>
                    </ul>
                  </template>
                </li>
                <li>
                  <form
                    v-if="siteId > 0"
                    id="search"
                    class="navbar-left app-search float-left"
                    :action="'cms/pageContentSearch.aspx?siteId=' + siteId"
                    target="right"
                    method="get"
                  >
                    <input name="siteId" type="hidden" :value="siteId">
                    <input name="keyword" type="text" placeholder="内容搜索..." class="form-control">
                    <a href="javascript:;" onclick="$('#search').submit();">
                      <i class="ion-search" />
                    </a>
                  </form>
                </li>
              </ul>
            </div>
          </template>
          <a v-else href="javascript:;" class="position-fixed" style="right: 20px;top: 10px;" @click="btnMobileMenuClick">
            <i :class="{'ion-navicon': !isMobileMenu, 'ion-android-close': isMobileMenu}" style="font-size: 28px;color: #fff;" />
          </a>
        </div>
      </header>
      <div v-if="isDesktop || isMobileMenu" class="left side-menu" :style="{width: leftMenuWidth}">
        <div id="sidebar" class="sidebar-inner slimscrollleft">
          <div id="sidebar-menu">
            <ul>
              <li v-for="siteMenu in siteMenus" :key="siteMenu.id" class="has_sub">
                <a
                  :href="getHref(siteMenu)"
                  :target="getTarget(siteMenu)"
                  :class="{'subdrop': siteMenu === activeParentMenu}"
                  @click="btnLeftMenuClick(siteMenu)"
                >
                  <i :class="(siteMenu.iconClass ? siteMenu.iconClass : 'ion-star')" />
                  {{ siteMenu.text }}
                  <span v-if="siteMenu.hasChildren" class="menu-arrow" />
                </a>
                <ul v-if="siteMenu.hasChildren" v-show="siteMenu === activeParentMenu" class="list-unstyled">
                  <li v-for="childMenu in siteMenu.children" :key="childMenu.id" :class="{'active': childMenu === activeChildMenu}">
                    <a :href="getHref(childMenu)" :target="getTarget(childMenu)" @click="btnLeftMenuClick(childMenu)">
                      <i :class="childMenu.iconClass" />
                      {{ childMenu.text }}
                    </a>
                  </li>
                </ul>
              </li>
              <li v-for="pluginMenu in pluginMenus" :key="pluginMenu.id" class="has_sub">
                <a
                  :href="getHref(pluginMenu)"
                  :target="getTarget(pluginMenu)"
                  :class="{'subdrop': pluginMenu === activeParentMenu}"
                  @click="btnLeftMenuClick(pluginMenu)"
                >
                  <i :class="(pluginMenu.iconClass ? pluginMenu.iconClass : 'ion-star')" />
                  {{ pluginMenu.text }}
                  <span v-if="pluginMenu.hasChildren" class="menu-arrow" />
                </a>
                <ul v-if="pluginMenu.hasChildren" v-show="pluginMenu === activeParentMenu" class="list-unstyled">
                  <li v-for="childMenu in pluginMenu.children" :key="childMenu.id" :class="{'active': childMenu === activeChildMenu}">
                    <a :href="getHref(childMenu)" :target="getTarget(childMenu)" @click="btnLeftMenuClick(childMenu)">
                      <i :class="childMenu.iconClass" />
                      {{ childMenu.text }}
                    </a>
                  </li>
                </ul>
              </li>
              <template v-for="topMenu in topMenus">
                <template v-if="!isDesktop && isMobileMenu">
                  <li :key="topMenu.id" class="text-muted menu-title">
                    {{ topMenu.text }}
                  </li>
                  <li v-for="parentMenu in topMenu.children" :key="parentMenu.id" class="has_sub">
                    <a
                      :href="getHref(parentMenu)"
                      :target="getTarget(parentMenu)"
                      :class="{'subdrop': parentMenu === activeParentMenu}"
                      @click="btnLeftMenuClick(parentMenu)"
                    >
                      <i :class="(parentMenu.iconClass ? parentMenu.iconClass : 'ion-star')" />
                      {{ parentMenu.text }}
                      <span v-if="parentMenu.hasChildren" class="menu-arrow" />
                    </a>
                    <ul v-if="parentMenu.hasChildren" v-show="parentMenu === activeParentMenu" class="list-unstyled">
                      <li v-for="childMenu in parentMenu.children" :key="childMenu.id" :class="{'active': childMenu === activeChildMenu}">
                        <a :href="getHref(childMenu)" :target="getTarget(childMenu)" @click="btnLeftMenuClick(childMenu)">
                          <i :class="childMenu.iconClass" />
                          {{ childMenu.text }}
                        </a>
                      </li>
                    </ul>
                  </li>
                </template>
              </template>
            </ul>
          </div>
        </div>
      </div>
      <div :style="{marginLeft: (isDesktop ? 200 : 0) + 'px'}" style="margin-top: 62px">
        <template v-if="pageUrl">
          <div class="content-page">
            <iframe
              id="right"
              frameborder="0"
              name="right"
              :src="pageUrl"
              :style="{width: (winWidth - (isDesktop ? 200 : 0)) + 'px', height: (winHeight - 62) + 'px'}"
            />
          </div>
        </template>
        <template v-else>
          <div class="p-3">
            <nuxt />
          </div>
        </template>
      </div>
    </div>
  </div>
</template>

<script>
import 'vue-loading-overlay/dist/vue-loading.css'
import 'assets/css/menu.css'
import AppNotify from '@/components/AppNotify.vue'
import Loading from 'vue-loading-overlay'
import _ from 'lodash'

export default {
  name: 'DefaultLayout',
  components: {
    AppNotify,
    Loading
  },
  data() {
    return {
      defaultPageUrl: null,
      isNightlyUpdate: null,
      pluginVersion: null,
      isSuperAdmin: null,
      packageList: null,
      packageIds: null,
      cmsVersion: null,
      apiPrefix: null,
      adminDirectory: null,
      homeDirectory: null,
      topMenus: null,
      siteMenus: null,
      activeParentMenu: null,
      activeChildMenu: null,
      pluginMenus: null,
      adminInfo: null,
      newVersion: null,
      updatePackages: [],
      status: {},
      pendingCount: 0,
      lastExecuteTime: new Date(),
      timeoutId: null,
      winHeight: 0,
      winWidth: 0,
      isDesktop: true,
      isMobileMenu: false,

      siteId: this.$route.query.siteId,
      pageUrl: ''
    }
  },
  computed: {
    leftMenuWidth() {
      if (this.isDesktop) return '200px'
      return this.isMobileMenu ? '100%' : '200px'
    },
    loading() {
      return this.$store.state.loading
    }
  },
  watch: {
    winHeight(newHeight, oldHeight) {
      this.winHeight = newHeight
    },
    winWidth(newWidth, oldWidth) {
      this.winWidth = newWidth
      this.isDesktop = this.winWidth > 992
    }
  },
  mounted() {
    this.winHeight = window.innerHeight
    this.winWidth = window.innerWidth

    this.$nextTick(() => {
      window.addEventListener('resize', () => {
        this.winHeight = window.innerHeight
        this.winWidth = window.innerWidth
      })
    })
    this.getConfig()
  },

  methods: {
    async getConfig() {
      this.$store.commit('loading', true)

      const data = await this.$api.main.get(this.$route.query)

      if (data.value) {
        this.defaultPageUrl = data.defaultPageUrl
        this.isNightlyUpdate = data.isNightlyUpdate
        this.pluginVersion = data.pluginVersion
        this.isSuperAdmin = data.isSuperAdmin
        this.packageList = data.packageList
        this.packageIds = data.packageIds
        this.cmsVersion = data.cmsVersion
        this.apiPrefix = data.apiPrefix
        this.adminDirectory = data.adminDirectory
        this.homeDirectory = data.homeDirectory
        this.topMenus = data.topMenus
        this.siteMenus = data.siteMenus
        this.pluginMenus = data.pluginMenus
        this.adminInfo = data.adminInfo
        this.activeParentMenu = this.siteMenus[0]
        this.pageLoad = true
        setTimeout(this.ready, 100)

        this.$store.commit('loading', false)
      } else {
        location.href = data.redirectUrl
      }
    },

    async create() {
      clearTimeout(this.timeoutId)

      const data = await this.$api.main.post()

      this.pendingCount = data.value
      if (this.pendingCount === 0) {
        this.timeoutId = setTimeout(this.create, 10000)
      } else {
        this.timeoutId = setTimeout(this.create, 100)
      }

      this.lastExecuteTime = new Date()
    },

    ready() {
      this.create()

      setInterval(() => {
        const dif = new Date().getTime() - this.lastExecuteTime.getTime()
        const minutes = dif / 1000 / 60
        if (minutes > 2) {
          this.create()
        }
      }, 60000)

      // $('#sidebar').slimScroll({
      //   height: 'auto',
      //   position: 'right',
      //   size: '5px',
      //   color: '#495057',
      //   wheelStep: 5
      // })

      this.connect()
    },

    compareversion(a, b) {
      const aa = a.split('.')
      const ab = b.split('.')
      let i = 0
      let la = aa.length; let lb = ab.length
      while (la > lb) {
        ab.push(0)
        ++lb
      }
      while (la < lb) {
        aa.push(0)
        ++la
      }
      while (i < la && i < lb) {
        const ai = parseInt(aa[i], 10)
        const bi = parseInt(ab[i], 10)
        if (ai > bi) {
          return 1
        } else if (ai < bi) {
          return -1
        }
        ++i
      }
      return 0
    },

    async connect() {
      const data = await this.$ss.connections.get({
        url: location.href,
        apiPrefix: this.apiPrefix,
        isNightlyUpdate: this.isNightlyUpdate,
        pluginVersion: this.pluginVersion,
        packageIds: this.isSuperAdmin ? this.packageIds.join(',') : ''
      })

      this.status = data.value

      if (data.updates && data.updates.length > 0) {
        for (let i = 0; i < data.updates.length; i++) {
          const releaseInfo = data.updates[i]
          if (!releaseInfo || !releaseInfo.version) continue
          if (releaseInfo.pluginId === 'SS.CMS') {
            this.downloadSsCms(releaseInfo)
          } else {
            const installedPackages = _.filter(this.packageList, (o) => {
              return o.id === releaseInfo.pluginId
            })
            if (installedPackages.length === 1) {
              const installedPackage = installedPackages[0]
              if (installedPackage.version) {
                if (
                  this.compareversion(
                    installedPackage.version,
                    releaseInfo.version
                  ) === -1
                ) {
                  this.updatePackages.push(
                    _.assign({}, {
                      cmsVersion: installedPackage.version
                    },
                    releaseInfo
                    )
                  )
                }
              } else {
                this.updatePackages.push(
                  _.assign({}, {
                    cmsVersion: installedPackage.version
                  },
                  releaseInfo
                  )
                )
              }
            }
          }
        }
      }
    },

    async downloadSsCms(releaseInfo) {
      // eslint-disable-next-line no-console
      console.log(this.cmsVersion)
      // eslint-disable-next-line no-console
      console.log(releaseInfo.version)
      if (this.compareversion(this.cmsVersion, releaseInfo.version) !== -1) return

      const data = await this.$api.main.postAt('/actions/download', {
        packageId: 'SS.CMS',
        version: releaseInfo.version
      })

      if (data.value) {
        this.newVersion = {
          version: releaseInfo.version,
          published: releaseInfo.published,
          releaseNotes: releaseInfo.releaseNotes
        }
      }
    },

    getHref(menu) {
      return menu.href && menu.target !== '_layer' ? menu.href : 'javascript:;'
    },

    getTarget(menu) {
      return menu.target ? menu.target : 'right'
    },

    btnCloudLoginClick() {
      this.$layer.openLayer({
        title: '云服务登录',
        url: 'cloud/layerLogin.cshtml',
        width: 550,
        height: 560
      })
    },

    btnTopMenuClick(menu) {
      if (menu.target === '_layer') {
        this.$layer.openLayer({
          title: menu.text,
          url: menu.href,
          full: true
        })
      }
    },

    btnLeftMenuClick(menu) {
      if (menu.hasChildren) {
        this.activeParentMenu = this.activeParentMenu === menu ? null : menu
      } else {
        this.activeChildMenu = menu
        this.isMobileMenu = false
        if (menu.target === '_layer') {
          this.$layer.openLayer({
            title: menu.text,
            url: menu.href,
            full: true
          })
        }
      }
    },

    btnMobileMenuClick() {
      this.isMobileMenu = !this.isMobileMenu
    },

    reload() {
      this.captcha = ''
      this.pageSubmit = false
      this.captchaUrl = this.$api.login.at('captcha?r=' + new Date().getTime())
    },

    btnLoginClick(e) {
      e.preventDefault()
      this.alert = null
      this.pageSubmit = true
      this.account && this.password && this.captcha && this.apiLogin()
    },

    btnLogoutClick() {
      this.$routers.push(`/logout/?redirectUrl=${location.href}`)
    },

    openPageCreateStatus() {
      this.$modal.openLayer({
        title: '生成进度查看',
        url: 'cms/createStatus.cshtml?siteId=' + this.siteId,
        full: true
      }, this.winHeight, this.winWidth)
      return false
    }
  }
}
</script>
