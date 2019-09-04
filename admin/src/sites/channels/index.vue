<template>
  <div>
    <el-card class="box-card">

      <aside>栏目名称之后的蓝色标签代表栏目索引，灰色标签代表栏目组</aside>

      <el-input
        v-model="filterText"
        placeholder="输入关键字进行过滤"
      />

      <el-divider />

      <el-tree
        ref="tree"
        :data="channels"
        node-key="id"
        draggable
        show-checkbox
        highlight-current
        :allow-drop="allowDrop"
        :allow-drag="allowDrag"
        class="custom-tree"
        :filter-node-method="filterNode"
        @node-drag-start="handleDragStart"
        @node-drag-enter="handleDragEnter"
        @node-drag-leave="handleDragLeave"
        @node-drag-over="handleDragOver"
        @node-drag-end="handleDragEnd"
        @node-drop="handleDrop"
      >
        <div slot-scope="{ node, data }" class="custom-tree-node">
          <span>
            {{ data.channelName }}
          </span>
          <span>
            <el-tooltip v-if="data.indexName" content="栏目索引" placement="top" effect="light">
              <el-tag>{{ data.indexName }}</el-tag>
            </el-tooltip>
            <el-tooltip content="栏目组" placement="top" effect="light">
              <el-tag type="info">标签二</el-tag>
            </el-tooltip>
            <el-button
              type="text"
              size="mini"
              style="margin-left: 10px"
              @click.stop="btnAppendClick(data)"
            >
              添加
            </el-button>
            <el-button
              type="text"
              size="mini"
              @click.stop="remove(node, data)"
            >
              Delete
            </el-button>
          </span>
        </div>
      </el-tree>

      <el-divider />

      <el-button-group>
        <el-button @click="addChannels1">新增栏目</el-button>
        <el-button @click="addChannels2">新增栏目</el-button>
        <el-button @click="getCheckedNodes">通过 node 获取</el-button>
        <el-button @click="getCheckedKeys">通过 key 获取</el-button>
        <el-button @click="setCheckedNodes">通过 node 设置</el-button>
        <el-button @click="setCheckedKeys">通过 key 设置</el-button>
        <el-button icon="el-icon-delete" @click="resetChecked">清空</el-button>
      </el-button-group>

    </el-card>

    <right-panel v-if="appendPanel" title="添加栏目" @close="appendPanel = false">
      <append :parent="appendParent" @submit="appendChannels" @close="appendPanel = false" />
    </right-panel>
  </div>
</template>

<script>
import { getChannels, deleteChannel } from '@/api/channels'
import RightPanel from '@/components/RightPanel'
import Append from './components/Append'

export default {
  name: 'Info',
  components: {
    RightPanel,
    Append
  },
  data() {
    return {
      tableData: [],
      filterText: '',
      channels: [],
      appendPanel: false,
      appendParent: null
    }
  },
  computed: {
    siteId() {
      return this.$route.params.siteId
    }
  },
  watch: {
    filterText(val) {
      this.$refs.tree.filter(val)
    }
  },
  created() {
    this.getChannels()
  },
  methods: {
    appendChannels(channels) {
      if (!this.appendParent.children) {
        this.$set(this.appendParent, 'children', [])
      }
      this.appendParent.children.push(...channels)
      this.appendPanel = false
      this.appendParent = null
    },

    btnAppendClick(data) {
      this.appendPanel = true
      this.appendParent = data
    },

    async getChannels() {
      this.channels = await getChannels(this.siteId)
    },

    addChannels1() {
      this.show1 = true
    },

    addChannels2() {
      this.show2 = true
    },

    handleDragStart(node, ev) {
      console.log('drag start', node)
    },
    handleDragEnter(draggingNode, dropNode, ev) {
      console.log('tree drag enter: ', dropNode.channelName)
    },
    handleDragLeave(draggingNode, dropNode, ev) {
      console.log('tree drag leave: ', dropNode.channelName)
    },
    handleDragOver(draggingNode, dropNode, ev) {
      console.log('tree drag over: ', dropNode.channelName)
    },
    handleDragEnd(draggingNode, dropNode, dropType, ev) {
      console.log('tree drag end: ', dropNode && dropNode.channelName, dropType)
    },
    handleDrop(draggingNode, dropNode, dropType, ev) {
      console.log('tree drop: ', dropNode.channelName, dropType)
    },
    allowDrop(draggingNode, dropNode, type) {
      if (dropNode.data.channelName === '二级 3-1') {
        return type !== 'inner'
      } else {
        return true
      }
    },
    allowDrag(draggingNode) {
      return draggingNode.data.channelName.indexOf('三级 3-2-2') === -1
    },
    getCheckedNodes() {
      console.log(this.$refs.tree.getCheckedNodes())
    },
    getCheckedKeys() {
      console.log(this.$refs.tree.getCheckedKeys())
    },
    setCheckedNodes() {
      this.$refs.tree.setCheckedNodes([{
        id: 5,
        channelName: '二级 2-1'
      }, {
        id: 9,
        channelName: '三级 1-1-1'
      }])
    },
    setCheckedKeys() {
      this.$refs.tree.setCheckedKeys([3])
    },
    resetChecked() {
      this.$refs.tree.setCheckedKeys([])
    },
    filterNode(value, data) {
      if (!value) return true
      return data.channelName.indexOf(value) !== -1
    },

    remove(node, data) {
      this.$confirm(`此操作将永久删除栏目 ${data.channelName}, 是否继续?`, '提示', {
        confirmButtonText: '永久删除',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(async() => {
        const channel = await deleteChannel(this.siteId, data.id)

        const parent = node.parent
        const children = parent.data.children || parent.data
        const index = children.findIndex(d => d.id === channel.id)
        children.splice(index, 1)

        this.$message({
          type: 'success',
          message: '删除成功!'
        })
      })
    }
  }
}
</script>

<style>
  .custom-tree .el-tree-node__content {
    line-height: 38px;
    height: 38px;
  }
  .custom-tree .el-tree-node__content > .el-checkbox {
    margin-top: 6px;
  }
  .custom-tree .custom-tree-node {
    font-size: 16px;
    flex: 1;
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding-right: 8px;
  }
</style>
