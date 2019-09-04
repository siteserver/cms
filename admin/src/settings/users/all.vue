<template>
  <div v-loading="loading" class="app-container">
    <el-table
      :data="tableData"
      style="width: 100%;margin-bottom: 20px;"
      row-key="id"
      border
    >
      <el-table-column prop="id" label="Id" sortable align="center" width="100" />
      <el-table-column prop="userName" label="用户名" sortable />
      <el-table-column prop="avatarUrl" label="头像" />
      <el-table-column prop="displayName" label="姓名" sortable />
      <el-table-column prop="createdDate" label="创建时间" sortable align="center" width="100" />
      <el-table-column prop="siteDir" label="站点文件夹" sortable width="180" />
      <el-table-column prop="tableName" label="默认内容表" sortable width="180" />
      <el-table-column align="center" label="操作" width="180">
        <template slot-scope="scope">
          <el-button size="mini" @click="handleEdit(scope.$index, scope.row)">Edit</el-button>
          <el-button size="mini" type="danger" @click="btnDeleteClick(scope.row)">Delete</el-button>
        </template>
      </el-table-column>
    </el-table>

    <right-panel v-if="deletePanel" title="删除站点" @close="deletePanel = false">
      <delete :site="deleteSite" @submit="handleDelete" @close="deletePanel = false" />
    </right-panel>
  </div>
</template>

<script>
import { getUsers } from '@/api/users'
import RightPanel from '@/components/RightPanel'
import Delete from './components/Delete'

export default {
  name: 'All',
  components: {
    RightPanel,
    Delete
  },
  data() {
    return {
      loading: true,
      tableData: [],
      deletePanel: false,
      deleteSite: null
    }
  },
  created() {
    getUsers().then(users => {
      this.tableData = users
      this.loading = false
    })
  },
  methods: {
    btnDeleteClick(site) {
      this.deletePanel = true
      this.deleteSite = site
    },

    handleEdit(index, row) {
      console.log(index, row)
    },

    handleDelete() {
      this.loading = true
      getUsers().then(users => {
        this.tableData = users
        this.loading = false
        this.deletePanel = false
        this.deleteSite = null

        this.$message({
          type: 'success',
          message: '删除成功!'
        })
      })
    }

    // remove(node, data) {
    //   this.$confirm(
    //     `此操作将永久删除栏目 ${data.channelName}, 是否继续?`,
    //     '提示',
    //     {
    //       confirmButtonText: '永久删除',
    //       cancelButtonText: '取消',
    //       type: 'warning'
    //     }
    //   ).then(async() => {
    //     const channel = await deleteChannel(this.siteId, data.id)

    //     const parent = node.parent
    //     const children = parent.data.children || parent.data
    //     const index = children.findIndex(d => d.id === channel.id)
    //     children.splice(index, 1)

    //     this.$message({
    //       type: 'success',
    //       message: '删除成功!'
    //     })
    //   })
    // }
  }
}
</script>
