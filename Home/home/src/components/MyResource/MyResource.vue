<template>
<div class="main">
  <el-row>
    <el-row style="margin-left: 30%;">
      <el-input
          style="width: 500px"
          placeholder="请输入内容"
          suffix-icon="el-icon-search"
          v-model="input"
      >
      </el-input>
      <el-button type="primary" @click="searchResource">搜索</el-button>
    </el-row>

  </el-row>
  <el-row v-if="total !== 0">
    <el-table
        ref="multipleTable"
        :data="list"
        tooltip-effect="dark"
        stripe
        style="width: 80%;margin-left: 10%;font-size: 20px;margin-top: 20px"
    >
      <el-table-column
          label="资源名称"
          :show-overflow-tooltip="true"
      >
        <template slot-scope="scope">
          <span style="margin-left: 10px">{{ scope.row.name }}</span>
        </template>
      </el-table-column>

      <el-table-column
          label="封面"
          :show-overflow-tooltip="true"
      >
        <template slot-scope="scope">
          <el-image style="height: 70px;width: 70px" :src="scope.row.image"></el-image>
        </template>
      </el-table-column>

      <el-table-column
          label="下载量"
      >
        <template slot-scope="scope">
          <span style="margin-left: 10px">{{ scope.row.sum }}</span>
        </template>
      </el-table-column>

      <el-table-column
          label="上传时间"
      >
        <template slot-scope="scope">
          <span style="margin-left: 10px">{{ getTime(scope.row.time) }}</span>
        </template>
      </el-table-column>
      <el-table-column
      >
        <template slot-scope="scope">
          <el-button @click="showDialog(scope.$index)" type="primary">下载<i class="el-icon-download el-icon--right"></i></el-button>
        </template>
      </el-table-column>

    </el-table>
      <el-pagination
          :hide-on-single-page="true"
          background
          layout="sizes, prev, pager, next"
          :page-size="currentSize"
          :current-page.sync="currentPage"
          @current-change="handleCurrentChange"
          @size-change="handleSizeChange"
          page-sizes="10"
          :total="total"
          style="display: flex;justify-content: center;margin-bottom:100px;margin-right: 30%;margin-left: 30%;"
      >
      </el-pagination>

  </el-row>
  <el-row v-else>
    <el-empty description="没有搜索到数据哦！ο(=•ω＜=)ρ⌒☆" :image-size="200"></el-empty>
  </el-row>
  <el-dialog
      title="请输入提取码
      悄悄告诉你，提取码在系统消息中哦n(*≧▽≦*)n"
      :visible.sync="dialogShow"
      width="50%"
      :before-close="handleClose">
    <el-input v-model="codeInput" placeholder="请输入提取码"></el-input>
    <span slot="footer" class="dialog-footer">
    <el-button @click="dialogShow = false">取 消</el-button>
    <el-button type="primary" @click="getResourceUrl">确 定</el-button>
  </span>
  </el-dialog>
</div>
</template>

<script>


export default {
  name: "MyResource",
  data() {
    return {
      list:[],
      input:'',
      total:0, // 总条数,
      currentPage:1, // 当前页
      currentSize:10, // 当前页面大小
      dialogShow:false, // 显示提取码dialog
      index:-1,
      codeInput:'',
    }
  },
  methods:{
    // 获取资源
    getResource() {
        this.HttpGet('/resource/' + this.currentPage + "/" + this.currentSize)
            .then(res => {
              this.list = res.data.list;
              this.total = res.data.total;
            })
    },

    // 搜索资源
    searchResource() {
      if(this.input !== "") {
        this.HttpGet('/resource/search/' + this.input + '/' + this.currentPage + "/" + this.currentSize)
            .then(res => {
              this.list = res.data.list;
              this.total = res.data.total;
            })
      }
      else {
        this.getResource();
      }
    },
    // 显示提取码dialog
    showDialog(index) {
      this.index = index;
      this.dialogShow = true;
    },

    // 获取资源地址并下载资源
    getResourceUrl() {
      if(this.codeInput === "") {
        this.$message.error("提取码不能为空");
        return;
      }
      let resourceId = this.list[this.index].resourceId;
      this.HttpGet('/resource/url/' + resourceId + '/' + this.codeInput)
          .then(res => {
            if(res.data === "error") {
              this.$message.error("提取码错误");
            }
            else if(res.data === "404") {
              this.$message.error("资源已经被删除了");
            }
            else {
              let url = res.data.split("/download/")[1];
              this.HttpPost('/resource/download/' + url)
                  .then(res => {
                    var blob = new Blob([res]);
                    var fileName = url //要保存的文件名称
                    var elink = document.createElement("a");
                    elink.download = fileName;
                    elink.style.display = "none";
                    elink.href = URL.createObjectURL(blob);
                    document.body.appendChild(elink);
                    elink.click();
                    URL.revokeObjectURL(elink.href); // 释放URL 对象
                    document.body.removeChild(elink);
                  })

            }
            this.dialogShow = false;
            this.codeInput = "";
            this.index = -1;
          })
    },

    // 调整页面尺寸
    handleSizeChange(val) {
      this.currentSize = val;
      // 重新获取数据
      if(this.input === "") {
        this.getResource();
      }
      else {
        this.searchResource();
      }

    },
    // 调整页面大小
    handleCurrentChange(val) {
      this.currentPage = val;
      if(this.input === "") {
        this.getResource();
      }
      else {
        this.searchResource();
      }
    },
    handleClose(done) {
      this.$confirm('确认关闭？')
          .then(_ => {
            done();
          })
          .catch(_ => {});
    }
  },
  created() {
    this.getResource();
  },
    computed:{
      // 将时间戳转换为几小时前
      getTime() {
        return (time) => {
          const now = new Date();
          const date = new Date(time);
          const diff = (now - date)/1000; // 毫秒转换为秒
          console.log(diff);
          if (diff < 60) {
            return "刚刚";
          } else if (diff < 3600) {
            return Math.floor(diff / 60) + "分钟前";
          } else if (diff < 86400) {
            return Math.floor(diff / 3600) + "小时前";
          } else if (diff < 2592000) {
            return Math.floor(diff / 86400) + "天前";
          } else if(diff < 2592000 * 12) {
            return Math.floor(diff / 2592000) + "月前";
          }
          else {
            const year = date.getFullYear();
            const month = date.getMonth() + 1;
            const day = date.getDate();
            return `${year}-${month < 10 ? '0' + month : month}-${day < 10 ? '0' + day : day}`;
          }
        }
      }

  }

}
</script>

<style lang="stylus" scoped>
.main {
  background-color white;
  min-height 90vh;
  height 100%;
  top:0;
  bottom:0;
  margin-left 20vh;
  margin-right 20vh;
}
</style>