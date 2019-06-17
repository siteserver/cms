<i18n>
{
  "en": {
    "welcome": "Welcome to the SiteServer CMS!"
  },
  "cn": {
    "welcome": "欢迎使用 SiteServer CMS 管理后台xxx!"
  }
}
</i18n>

<template>
  <div>
    <h4 class="page-title mb-4">
      {{ $t('welcome') }}
    </h4>
    <div class="row text-center">
      <div class="col-4">
        <div class="widget-simple-chart text-center card-box">
          <h3 class="text-primary counter m-t-10">
            {{ version }}
          </h3>
          <p class="text-muted text-nowrap m-b-10">
            当前版本
          </p>
        </div>
      </div>
      <div class="col-4">
        <div class="widget-simple-chart text-center card-box">
          <h3 class="text-primary counter m-t-10">
            {{ updateDate }}
          </h3>
          <p class="text-muted text-nowrap m-b-10">
            最近升级时间
          </p>
        </div>
      </div>
      <div class="col-4">
        <div class="widget-simple-chart text-center card-box">
          <h3 class="text-primary counter m-t-10">
            {{ lastActivityDate }}
          </h3>
          <p class="text-muted text-nowrap m-b-10">
            上次登录时间
          </p>
        </div>
      </div>
    </div>
    <div v-if="unCheckedListTotalCount > 0" class="card-box">
      <div class="header-title m-t-0">
        待审核内容
      </div>
      <p class="text-muted m-b-25 font-13">
        共有 <span style="color:#f00">
          {{ unCheckedListTotalCount }}
        </span> 篇内容待审核
      </p>
      <div class="table-responsive">
        <table class="table">
          <tbody>
            <tr v-for="item in unCheckedList" :key="item.id">
              <td>
                <a :href="item.url">
                  {{ item.siteName }} 有 <span style="color:#f00">
                    {{ item.count }}
                  </span> 篇
                </a>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script>
import '@/assets/css/menu.css'

export default {
  layout: 'admin',
  data() {
    return {
      version: null,
      lastActivityDate: null,
      updateDate: null,
      unCheckedList: null,
      unCheckedListTotalCount: 0
    }
  },
  mounted() {
    this.apiGet()
  },
  methods: {
    async apiGet() {
      this.$store.commit('loading', true)

      const data = await this.$api.index.get()

      this.version = data.value.version
      this.lastActivityDate = data.value.lastActivityDate
      this.updateDate = data.value.updateDate

      this.$store.commit('loading', false)

      this.getUnCheckedList()
    },

    async getUnCheckedList() {
      const data = await this.$api.index.getAt('unCheckedList')

      this.unCheckedList = data.value
      for (let i = 0; i < this.unCheckedList.length; i++) {
        this.unCheckedListTotalCount += this.unCheckedList[i].count
      }
    }
  }
}
</script>
