<template>
  <div>
    <el-row>
<!-- 文件上传就删掉算了，不想要使用这个功能了 -->
<!--
      <el-button type="info" @click="showTable">上传<i class="el-icon-upload el-icon&#45;&#45;right"></i></el-button>
-->
      <el-button type="primary" @click="addPaper">添加<i class="el-icon-plus el-icon--right"></i>  </el-button>
    </el-row>
    <el-table
        ref="multipleTable"
        :data="list"
        tooltip-effect="dark"
        style="width: 100%"
        >

      <el-table-column
          label="ID"
          :show-overflow-tooltip="true"
      >
        <template slot-scope="scope">
          <span style="margin-left: 10px">{{ scope.row.listeningPaperId }}</span>
        </template>
      </el-table-column>

      <el-table-column
          label="标题"
          :show-overflow-tooltip="true"
      >
        <template slot-scope="scope">
          <span style="margin-left: 10px">{{ scope.row.paperTitle }}</span>
        </template>
      </el-table-column>

      <el-table-column
          label="题目总数"
          :show-overflow-tooltip="true"
      >
        <template slot-scope="scope">
          <span style="margin-left: 10px">{{ scope.row.partList.length }}</span>
        </template>
      </el-table-column>

      <el-table-column label="操作">
        <template slot-scope="scope">
          <el-button
              type="primary"
              @click="getDetail(scope.row.listeningPaperId)">查看详情
          </el-button>
          <el-button
              type="danger" icon="el-icon-delete"
              @click="deleteById(scope.row.listeningPaperId)">
          </el-button>
        </template>
      </el-table-column>
    </el-table>

    <el-pagination v-show="show"
                   background
                   layout="sizes, prev, pager, next"
                   :page-size="pageSize"
                   :current-page.sync="currentPage"
                   @current-change="handleCurrentChange"
                   @size-change="handleSizeChange"
                   :page-sizes="[10,20,30]"
                   :total="total"
                   style="display: flex;justify-content: center;margin-bottom:10px"
    >
    </el-pagination>
  </div>
</template>

<script>
export default {
  name: "ListeningPaperList",
  data(){
    return {
      list:[],
      page: 1,
      pageSize: 10, // 当前页面大小
      size: 10,
      currentPage: 1, // 当前页码
      total:1,// 总条数,
      show: false,
    }
  },
  methods:{
    // 获取听力列表
    getListeningPaperList() {
        this.HttpGet('/paper/list/' + this.currentPage + '/' + this.pageSize)
            .then(res => {
              this.list = res.data.list;
              this. total = res.data.total;
              this.show = true;
            })
    },

    // 更改页码
    handleCurrentChange(page) {
      this.currentPage = page;
      this.getListeningPaperList();
    },
    // 更改页面大小
    handleSizeChange(size) {
      this.pageSize = size;
      this.getListeningPaperList();
    },
    // 查看听力详情
    getDetail(id) {
      // 跳转到听力详情页，打开新的页面
      let newUrl = this.$router.resolve({
        path: '/listening/detail/' + id,
        params: {
          id: id
        }
      });
      window.open(newUrl.href, "_blank");
    },
    // 删除听力
    deleteById(id) {
      this.$confirm('是否删除该试题', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        // 删除试题操作
        this.HttpPost('/paper/delete/' + id)
            .then(res => {
              if(res.data === true) {
                this.$notify({
                  title:"系统提示",
                  message:"删除成功",
                  type:'success'
                });
                this.getListeningPaperList();
              }
              else {
                this.$notify({
                  title:"系统 提示",
                  message:"删除失败",
                  type:'error'
                })
              }
            })
      }).catch(() => {
        console.log("取消删除");
      });
    },
    // 添加听力
    addPaper() {
      // 跳转到听力详情页，打开新的页面
      let newUrl = this.$router.resolve({
        path: '/listening/add',
      });
      window.open(newUrl.href, "_blank");
    }

  },
  created() {
    this.getListeningPaperList();
  }
}
</script>

<style scoped>

</style>