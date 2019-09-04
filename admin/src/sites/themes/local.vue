<template>
  <div class="app-container">

    <el-table
      :data="tableData"
      stripe
      style="width: 100%"
    >
      <el-table-column
        prop="name"
        :label="$t('name')"
        width="220"
      />
      <el-table-column
        prop="value"
        :label="$t('value')"
      />
    </el-table>

  </div>
</template>

<script>
import { getInfo } from '@/api/cms'

export default {
  name: 'Info',
  data() {
    return {
      tableData: []
    }
  },
  created() {
    this.getInfo()
  },
  methods: {
    getInfo() {
      getInfo().then(response => {
        this.tableData.push({
          name: this.$t('tools.info.apiUrl'),
          value: response.apiUrl
        })
        this.tableData.push({
          name: this.$t('tools.info.apiServerName'),
          value: response.apiServerName
        })
        this.tableData.push({
          name: this.$t('tools.info.contentRootPath'),
          value: response.contentRootPath
        })
        this.tableData.push({
          name: this.$t('tools.info.webRootPath'),
          value: response.webRootPath
        })
        this.tableData.push({
          name: this.$t('tools.info.remoteIpAddress'),
          value: response.remoteIpAddress
        })
        this.tableData.push({
          name: this.$t('tools.info.targetFramework'),
          value: response.targetFramework
        })
        this.tableData.push({
          name: this.$t('tools.info.productVersion'),
          value: response.productVersion
        })
        this.tableData.push({
          name: this.$t('tools.info.pluginVersion'),
          value: response.pluginVersion
        })
        this.tableData.push({
          name: this.$t('tools.info.databaseType'),
          value: response.databaseType
        })
        this.tableData.push({
          name: this.$t('tools.info.database'),
          value: response.database
        })
        this.tableData.push({
          name: this.$t('tools.info.cacheType'),
          value: response.cacheType
        })
      })
    }
  }
}
</script>
