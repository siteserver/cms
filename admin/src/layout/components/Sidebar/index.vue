<template>
  <div :class="{'has-logo':showLogo}">
    <logo v-if="showLogo" :collapse="isCollapse" />
    <el-scrollbar wrap-class="scrollbar-wrapper">
      <el-menu
        :default-active="activeMenu"
        :collapse="isCollapse"
        :background-color="variables.menuBg"
        :text-color="variables.menuText"
        :unique-opened="false"
        :active-text-color="variables.menuActiveText"
        :collapse-transition="false"
        mode="vertical"
      >
        <sidebar-item v-for="route in routes" :key="route.path" :item="route" :base-path="topMenu + '/' + route.path" />
      </el-menu>
    </el-scrollbar>
  </div>
</template>

<script>
import { mapGetters } from 'vuex'
import { getMenus } from '@/api/user'
import { parseRoutes } from '@/utils/permission'
import Logo from './Logo'
import SidebarItem from './SidebarItem'
import sitesRouters from '@/router/sites'
import pluginsRouters from '@/router/plugins'
import settingsRouters from '@/router/settings'
import variables from '@/styles/variables.scss'

export default {
  components: { SidebarItem, Logo },
  data() {
    return {
      routes: []
    }
  },
  computed: {
    ...mapGetters([
      'topMenu',
      'permission_routes',
      'sidebar'
    ]),
    activeMenu() {
      const route = this.$route
      const { meta, path } = route
      // if set path, the sidebar will highlight the path you set
      if (meta.activeMenu) {
        return meta.activeMenu
      }
      return path
    },
    showLogo() {
      return this.$store.state.settings.sidebarLogo
    },
    variables() {
      return variables
    },
    isCollapse() {
      return !this.sidebar.opened
    }
  },
  watch: {
    topMenu(val) {
      this.getRoutes(val)
    }
  },
  created() {
    this.getRoutes(this.topMenu)
  },
  methods: {
    async getRoutes(topMenu) {
      if (topMenu === '/sites') {
        this.routes = sitesRouters
      } else if (topMenu === '/plugins') {
        this.routes = pluginsRouters
      } else if (topMenu === '/settings') {
        this.routes = settingsRouters
      } else {
        const menus = await getMenus(topMenu, 0)
        this.routes = parseRoutes(menus)
      }
    }
  }
}
</script>
