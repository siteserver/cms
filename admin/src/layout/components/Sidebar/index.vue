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
import _ from 'lodash'
import { getMenus } from '@/api/users'
import { getSites, getSite } from '@/api/sites'
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
    // call again the method if the route changes
    '$route': 'fetchData'
  },
  mounted() {
    this.fetchData()
  },
  methods: {
    async fetchData() {
      const first = this.getFirstPath()
      if (first === 'sites') {
        if (this.$route.params.siteId) {
          const site = await getSite(this.$route.params.siteId)
          const routes = _.cloneDeep(sitesRouters)
          for (const route of routes) {
            route.path = `/sites/${this.$route.params.siteId}/${route.path}`
          }
          routes.splice(0, 0, {
            link: 'open',
            path: 'http://github.com',
            name: 'SitesOpen',
            meta: { icon: 'external', title: site.siteName }
          })
          this.routes = routes
        } else {
          const sites = await getSites()
          this.routes = this.getSitesRouters(sites)
        }
      } else if (first === 'plugins') {
        const routes = _.cloneDeep(pluginsRouters)
        for (const route of routes) {
          route.path = `/plugins/${route.path}`
        }
        this.routes = routes
      } else if (first === 'settings') {
        const routes = _.cloneDeep(settingsRouters)
        for (const route of routes) {
          route.path = `/settings/${route.path}`
        }
        this.routes = routes
      } else {
        const menus = await getMenus(first, 0)
        this.routes = parseRoutes(menus)
      }
    },
    getFirstPath() {
      let firstPath = _.trim(this.$route.path, '/')

      if (firstPath) {
        if (firstPath.indexOf('/') !== -1) {
          firstPath = firstPath.substr(0, firstPath.indexOf('/'))
        }
        return firstPath
      }
    },

    getSitesRouters(sites, parent) {
      if (!sites) return null

      var routers = []
      if (parent) {
        routers.push({
          path: `/sites/${parent.id}`,
          meta: {
            title: parent.siteName,
            icon: 'site'
          }
        })
      }
      for (const site of sites) {
        var router = {
          path: `/sites/${site.id}`,
          meta: {
            title: site.siteName,
            icon: 'site'
          },
          children: this.getSitesRouters(site.children, site)
        }

        routers.push(router)
      }

      return routers
    }
  }
}
</script>
