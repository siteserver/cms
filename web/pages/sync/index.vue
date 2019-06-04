<template>
  <div class="mt-4 container-fluid">
    <div class="card-box">
      <h4 class="header-title m-t-0">
        SiteServer CMS 数据库升级向导
      </h4>
      <p class="text-muted m-b-25 font-13">
        欢迎来到SiteServer CMS 数据库升级向导！
      </p>
      <ul class="nav nav-pills navtab-bg nav-justified">
        <li class="nav-item">
          <a class="nav-link" :class="{'active': step === 1}" href="javascript:;">升级准备</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" :class="{'active': step === 2}" href="javascript:;">升级数据库</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" :class="{'active': step === 3}" href="javascript:;">升级完成</a>
        </li>
      </ul>

      <hr>

      <div v-if="step == 1">
        <div class="jumbotron">
          <h4 class="display-5">
            欢迎来到SiteServer CMS 数据库升级向导！
          </h4>
          <h4 class="display-5">
            升级向导将逐一检查数据库字段、将数据库结构更新至最新版本。
          </h4>
          <h4 class="display-5">
            当前版本：{{ databaseVersion }}，升级后版本：{{ productVersion }}
          </h4>
        </div>
        <hr>
        <div class="text-center">
          <button type="button" class="btn btn-primary" :disabled="!isSyncable" @click="btnUpdateClick">
            开始升级
          </button>
        </div>
      </div>

      <div v-else-if="step == 2">
        <div class="jumbotron text-center">
          <p class="lead">
            正在升级数据库，请稍后...
          </p>
        </div>
      </div>

      <div v-else-if="step == 3">
        <div class="alert alert-primary">
          <h4 class="alert-heading">
            数据库升级完成！
          </h4>
          <p>
            恭喜，您已经完成了 SiteServer CMS 数据库的升级 <a class="btn btn-primary ml-2" href="main.cshtml">进入后台</a>
          </p>
          <hr>
          <p class="mb-0">
            获取更多使用帮助请访问 <a href="https://www.siteserver.cn/docs/" target="_blank">SiteServer CMS 文档中心</a>
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  layout: 'root',
  data() {
    return {
      isSyncable: false,
      databaseVersion: null,
      productVersion: null,
      step: 1
    }
  },
  mounted() {
    this.apiGet()
  },
  methods: {
    async apiGet() {
      this.$store.commit('loading', true)
      const data = await this.$api.sync.get()
      this.isSyncable = data.value
      this.databaseVersion = data.databaseVersion
      this.productVersion = data.productVersion
      this.$store.commit('loading', false)
    },

    async btnUpdateClick(e) {
      if (this.isSyncable) {
        this.step = 2
        this.$store.commit('loading', true)
        await this.$api.sync.post()
        this.step = 3
        this.$store.commit('loading', false)
      }
    }
  }
}
</script>
