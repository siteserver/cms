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
        <sidebar-item v-for="route in routes" :key="route.path" :item="route" :base-path="route.path" />
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
    // leftMenus() {
    //   if (!this.topMenu) {
    //     return this.permission_routes
    //   } else {
    //     return [this.permission_routes[0], this.permission_routes[1], this.permission_routes[2], this.permission_routes[3], this.permission_routes[4], this.permission_routes[5], this.permission_routes[6], this.permission_routes[7]]
    //   }
    // },
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
    topMenu: function(val) {
      this.getRoutes(val)
    }
  },
  methods: {
    async getRoutes(topMenu) {
      const menus = await getMenus(topMenu, 0)
      this.routes = parseRoutes(menus)
    }
  }
}
</script>
