<template>
  <div>
    <el-row>
      <el-select v-model="value"
                 placeholder="请选择"
                 @change="valueChange"
      >
        <el-option
            v-for="item in options"
            :key="item.value"
            :label="item.label"
            :value="item.value"
        >
        </el-option>
      </el-select>
      <el-button type="primary" @click="addPassage">添加<i class="el-icon-plus el-icon--right"></i></el-button>
      <el-button type="info" @click="showTable">上传<i class="el-icon-upload el-icon--right"></i></el-button>
    </el-row>

    <el-table
        ref="multipleTable"
        :data="list"
        tooltip-effect="dark"
        style="width: 100%"
        :default-sort="{prop: 'time', order: 'descending'}"
        >
      <el-table-column
          label="ID"
          :show-overflow-tooltip="true"
      >
        <template slot-scope="scope">
          <span style="margin-left: 10px">{{ scope.row.passageId }}</span>
        </template>
      </el-table-column>

      <el-table-column
          label="标题"
          :show-overflow-tooltip="true"
      >
        <template slot-scope="scope">
          <span style="margin-left: 10px">{{ scope.row.title }}</span>
        </template>
      </el-table-column>

      <el-table-column
          label="内容"
          :show-overflow-tooltip="true"
      >
        <template slot-scope="scope">
          <span style="margin-left: 10px">{{ scope.row.content }}</span>
        </template>
      </el-table-column>

      <el-table-column label="操作">
        <template slot-scope="scope">
          <el-button
              v-show="value === 0"
              @click="update(scope.row.passageId)">修改
          </el-button>
          <el-button
              v-if="value === 0"

              type="danger" icon="el-icon-delete"
              @click="deleteById(scope.row.passageId)"
              >删除
          </el-button>
          <el-button
              v-else
              type="primary"
              icon="el-icon-refresh-left"
              @click="recoverById(scope.row.passageId)"
          >恢复
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
    <el-dialog title="上传阅读理解" :visible.sync="tableShow">
      <el-form>
        <el-form-item>
          <el-input v-model="title" placeholder="请输入标题"></el-input>
        </el-form-item>
        <!--        上传excel文件-->
        <el-upload
            class="upload-demo"
            ref="upload"
            accept=".xls,.xlsx"
            :file-list="fileList"
            :on-change="handleChange"
            :action=uploadURL
            :show-file-list="true"
            :on-success="onSuccess"
            :on-error="onError"
            :headers="Header"
            :name="file"
            :auto-upload="false">
          <el-button slot="trigger" size="small" type="primary">选取文件</el-button>
          <el-button style="margin-left: 10px;" size="small" type="success" @click="submitUpload">上传到服务器
          </el-button>
          <el-button @click="cancel">取 消</el-button>

        </el-upload>
      </el-form>

    </el-dialog>
  </div>
</template>
<script>


export default {
  name: 'PassageList',
  data() {
    return {
      fileList:[],
      tableShow: false,
      multipleSelection: [],
      list: [],
      page: 1,
      pageSize: 10,
      size: 10,
      currentPage: 1,
      total: 1,
      show: false,
      options: [
          {
        value: 0,
        label: '未删除'
      }, {
        value: 1,
        label: '已删除'
      }],
      value: 0,
      Header:{
        Authorization: "Bearer" + " " +  localStorage.getItem("token")
      },
      title:"",
      uploadURL:'/apis/passage/upload/',
    }
  },
  methods: {
    // 监听select中的value的变化
    valueChange() {
      this.getList();
    },

    // 更改页码
    handleCurrentChange(page) {
      this.currentPage = page;
      this.getList();
    },
    // 更改页面大小
    handleSizeChange(size) {
      this.pageSize = size;
      this.getList();
    },

    // 根据id修改内容
    update(id) {
      // 跳转到修改页面，并且开启新的页面
      let newUrl = this.$router.resolve({
        path: '/updatePassage/' + id,
        params: {
          id: id
        }
      });
      window.open(newUrl.href, "_blank");

    },
    //获取阅读理解列表
    getList() {
      this.HttpGet("/passage/admin/" + this.currentPage + "/" + this.pageSize + "/" + this.value)
          .then(res => {
            this.total = res.data.total;
            this.list = res.data.passageDTOList;
            if (this.list.length !== 0) {
              this.show = true;
            }
          })
    },
    // 删除阅读理解
    deleteById(id) {
      this.HttpPost("/passage/delete/" + id)
          .then(res => {
            if(res.success === false) {
              this.$notify({
                message: "删除失败",
                title: "系统提示",
                type: "error"
              })
            }
            else {
              this.$notify({
                message:"删除成功",
                title:"系统提示",
                type:'success'
              });
              this.getList();
            }

          })
    },
    // 恢复阅读理解
    recoverById(id) {
      this.HttpPost('/passage/recover/' + id)
          .then(res => {
            if(res.success === false) {
              this.$notify({
                message: "恢复失败",
                title: "系统提示",
                type: "error"
              })
            }
              else {
                this.$notify({
                  message:"恢复成功",
                  title:"系统提示",
                  type:'success'
                });
                this.getList();
              }

          })
    },
    // 上传阅读理解excel文件
    submitUpload() {
      if(this.title === "") {
        this.$notify({
          message:"请输入标题",
          type:"warning"
        });
      }
      else {
        this.uploadURL = this.uploadURL + this.title;
        this.$refs.upload.submit();
      }
    },
    handleChange(file, fileList) {
      if (fileList.length > 0) {
        this.fileList = [fileList[fileList.length - 1]]  // 这一步，是 展示最后一次选择的csv文件
      }
    },
    // 上传成功
    onSuccess() {
      this.$notify({
        message:'上传成功',
        type:"success"
      });
      this.getList();
      this.tableShow=false;
    },
    // 上传失败
    onError() {
      this.$notify({
        message:'上传失败',
        type:'error'
      })
    },
    // 显示dialog
    showTable() {
      this.tableShow = true;
      this.title = "";
    },
    // 取消上传
    cancel() {
      this.tableShow = false;
    },
    // 添加阅读理解
    addPassage() {
      this.$router.push("/home/passage/add");
    }
  },
  mounted() {
    this.getList();
  },
  computed: {
    timeChange() {
      return (time) => {
        var date = new Date(time);
        let Y = date.getFullYear() + '-';
        let M = (date.getMonth() + 1 < 10 ? '0' + (date.getMonth() + 1) : date.getMonth() + 1) + '-';
        let D = date.getDate() + ' ';
        return Y + M + D;
      }
    }
  }
}
</script>
<style lang="stylus" scoped>

</style>