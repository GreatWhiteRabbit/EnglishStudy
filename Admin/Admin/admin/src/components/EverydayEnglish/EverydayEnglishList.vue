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
      <el-button v-if="value === 0"
                 type="danger"
                 icon="el-icon-delete"
                 size="large"
                 @click="removeList()">删除
      </el-button>
      <el-button v-else
                 type="primary"
                 size="large"
                 @click="recoveryList()">恢复
      </el-button>
      <el-button type="info" @click="showTable">上传<i class="el-icon-upload el-icon--right"></i></el-button>
    </el-row>

    <el-table
        ref="multipleTable"
        :data="list"
        tooltip-effect="dark"
        style="width: 100%"
        :default-sort="{prop: 'time', order: 'descending'}"
        @selection-change="handleSelectionChange">
      <el-table-column
          type="selection"
          width="55">
      </el-table-column>
      <el-table-column
          label="ID"
          :show-overflow-tooltip="true"
      >
        <template slot-scope="scope">
          <span style="margin-left: 10px">{{ scope.row.everyday_id }}</span>
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
      <el-table-column
          label="标题翻译"
          :show-overflow-tooltip="true"
      >
        <template slot-scope="scope">
          <span style="margin-left: 10px">{{ scope.row.titleTranslations }}</span>
        </template>
      </el-table-column>

      <el-table-column
          label="内容翻译"
          :show-overflow-tooltip="true"
      >
        <template slot-scope="scope">
          <span style="margin-left: 10px">{{ scope.row.translations }}</span>
        </template>
      </el-table-column>

      <el-table-column
          label="创建日期"
          prop="time"
          :show-overflow-tooltip="true"
          sortable
      >
        <template slot-scope="scope">
          <span style="margin-left: 10px">{{ timeChange(scope.row.time) }}</span>
        </template>
      </el-table-column>

      <el-table-column label="操作">
        <template slot-scope="scope">
          <el-button
              size="mini"
              @click="update(scope.row.everyday_id)">修改
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
    <el-dialog title="上传每日英语文件" :visible.sync="tableShow">
      <el-form>
        <!--        上传excel文件-->
        <el-upload
            class="upload-demo"
            ref="upload"
            accept=".xls,.xlsx"
            :file-list="fileList"
            :on-change="handleChange"
            action="/apis/everyday/upload"
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
  name: 'EverydayEnglishList',
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
      options: [{
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
    }
  },
  methods: {
    // 监听select中的value的变化
    valueChange() {
      this.getList();
    },
    // 选择框被选择时，获取id
    handleSelectionChange(val) {
      this.multipleSelection = val;
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
    // 根据id批量删除操作
    removeList() {
      // 获取multipleSelection中的id
      var list = [];
      for (let i = 0; i < this.multipleSelection.length; i++) {
        list.push(this.multipleSelection[i].everyday_id);
      }
      // 发送网络请求
      this.HttpPost('/everyday/delete', JSON.stringify({
        list
      })).then(res => {
        if (res.data === 0) {
          this.$notify({
            message: '删除失败',
            type: "error"
          })
        } else {
          this.$notify({
            message: "成功删除了" + res.data + "条数据",
            type: "success"
          })
          // 获取删除后的list
          this.getList();
        }
      })
    },
    // 根据id批量恢复数据
    recoveryList() {
      // 获取multipleSelection中的id
      var list = [];
      for (let i = 0; i < this.multipleSelection.length; i++) {
        list.push(this.multipleSelection[i].everyday_id);
      }
      // 发送网络请求
      this.HttpPost('/everyday/recover',
          JSON.stringify({list})
      )
          .then(res => {
            if (res.data === 0) {
              this.$notify({
                message: '恢复失败',
                type: "error"
              })
            } else {
              this.$notify({
                message: "成功恢复了" + res.data + "条数据",
                type: "success"
              })
              // 获取删除后的list
              this.getList();
            }
          })
    },
    // 根据id修改内容
    update(id) {
      // 跳转到修改页面，并且开启新的页面
      let newUrl = this.$router.resolve({
        path: '/updateEverydayEnglish/' + id,
        params: {
          id: id
        }
      });
      window.open(newUrl.href, "_blank");

    },
    //获取每日英语列表
    getList() {
      this.HttpGet("/everyday/admin/" + this.currentPage + "/" + this.pageSize + "/" + this.value)
          .then(res => {
            this.total = res.data.total;
            this.list = res.data.list;
            if (this.list.length !== 0) {
              this.show = true;
            }
          })
    },
    // 上传每日英语excel文件
    submitUpload() {
      this.$refs.upload.submit();
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
    },
    // 取消上传
    cancel() {
      this.tableShow = false;
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