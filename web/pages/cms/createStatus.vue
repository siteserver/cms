<i18n>
{
  "en": {
    "welcome": "Welcome to the SiteServer CMS!"
  },
  "cn": {
    "welcome": "欢迎使用 SiteServer CMS 管理后台!"
  }
}
</i18n>

<template>
  <div class="card-box">
    <div class="row app-countdown mt-0">
      <div class="col-12">
        <div>
          <div>
            <h3>剩余页面：</h3>
          </div>
          <div>
            <span :style="{color: channelsCount? '#fa0' : '#00b19d'}">{{ channelsCount }}</span>
            <span>
              <b>栏目页</b>
            </span>
          </div>
          <div>
            <span :style="{color: contentsCount? '#fa0' : '#00b19d'}">{{ contentsCount }}</span>
            <span>
              <b>内容页</b>
            </span>
          </div>
          <div>
            <span :style=" {color: filesCount? '#fa0' : '#00b19d' }">{{ filesCount }}</span>
            <span>
              <b>文件页</b>
            </span>
          </div>
          <div>
            <span :style=" {color: specialsCount? '#fa0' : '#00b19d' }">{{ specialsCount }}</span>
            <span>
              <b>专题页</b>
            </span>
          </div>
          <div v-if="channelsCount > 0 || contentsCount > 0 || filesCount > 0 || specialsCount > 0">
            <button type="button" class="btn btn-danger m-l-5" @click="btnCancelClick">
              取消生成任务
            </button>
          </div>
        </div>
      </div>
    </div>

    <hr>

    <template>
      <div v-for="task in tasks" :key="task.id" class="form-row">
        <template v-if="task.isPending">
          <label class="col-2 col-form-label-sm">
            {{ task.type }}
          </label>
          <div class="col-10">
            <div class="progress progress-md m-t-5" style="height: 20px !important">
              <div class="progress-bar bg-warning progress-bar-striped progress-bar-animated" style="width: 100%;">
                {{ task.name }}
              </div>
            </div>
          </div>
        </template>
        <template v-else-if="task.isSuccess">
          <label class="col-2 col-form-label-sm">
            {{ task.type }}
          </label>
          <div class="col-10">
            <div class="progress progress-md m-t-5" style="height: 20px !important">
              <div class="progress-bar bg-primary" style="width: 100%;">
                <a :href="getRedirectUrl(task)" target="_blank" style="color:#fff;">
                  {{ task.name }}（用时：{{ task.timeSpan }}）
                </a>
              </div>
            </div>
          </div>
        </template>
        <template v-else>
          <label class="col-2 col-form-label-sm">
            {{ task.type }}
          </label>
          <div class="col-10">
            <div class="progress progress-md m-t-5" style="height: 20px !important">
              <div class="progress-bar bg-danger" style="width: 100%;">
                {{ task.name }}（错误：{{ task.errorMessage }}）
              </div>
            </div>
          </div>
        </template>
      </div>
    </template>
  </div>
</template>

<script>
export default {
  layout: 'layer',
  data() {
    return {
      tasks: null,
      channelsCount: null,
      contentsCount: null,
      filesCount: null,
      specialsCount: null,
      timeoutId: null
    }
  },
  mounted() {
    this.apiGet()
  },
  methods: {
    async apiGet() {
      const data = await this.$api.cmsCreateStatus.get()

      this.timeoutId = setTimeout(() => {
        this.apiGet()
      }, 3000)
      if (!data || !data.value) return

      this.tasks = data.value.tasks
      this.channelsCount = data.value.channelsCount
      this.contentsCount = data.value.contentsCount
      this.filesCount = data.value.filesCount
      this.specialsCount = data.value.specialsCount
    },

    async apiCancel() {
      await this.$api.cmsCreateStatus.postAt('actions/cancel', null, {
        params: {
          siteId: this.siteId
        }
      })

      this.apiGet()
    },

    getRedirectUrl(task) {
      let url = '../pageRedirect.aspx?siteId=' + task.siteId
      if (task.channelId) {
        url += '&channelId=' + task.channelId
      }
      if (task.contentId) {
        url += '&contentId=' + task.contentId
      }
      if (task.fileTemplateId) {
        url += '&fileTemplateId=' + task.fileTemplateId
      }
      if (task.specialId) {
        url += '&specialId=' + task.specialId
      }
      return url
    },

    btnCancelClick() {
      clearTimeout(this.timeoutId)

      this.apiCancel()
    }
  }
}
</script>
