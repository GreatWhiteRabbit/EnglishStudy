<template>
<div>
  <el-row>
    <el-button type="primary" @click="showAddMessageDialog">发布消息</el-button>
    <el-popover
        placement="top-start"
        width="200"
        trigger="hover"
        content="手动将消息推送给所有的用户">
      <el-button @click = pullMessage slot="reference">推送</el-button>
    </el-popover>
    <span style="margin-left: 30px">定时任务执行时间：</span>
    <el-time-picker
        @change="updateWorkTime"
        v-model="time"
        :picker-options="{
      selectableRange: '00:00:00 - 23:59:59'
    }"
        placeholder="任意时间点">
    </el-time-picker>

  </el-row>

  <el-table
      :data="messageList"
      tooltip-effect="dark"
      style="width: 100%"

  >
    <el-table-column
        label="ID"
        :show-overflow-tooltip="true"
    >
      <template slot-scope="scope">
        <span style="margin-left: 10px">{{ scope.row.messageId }}</span>
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
        label="发布时间"
        :show-overflow-tooltip="true"
    >
      <template slot-scope="scope">
        <span style="margin-left: 10px">{{ timeChange(scope.row.time) }}</span>
      </template>
    </el-table-column>

    <el-table-column label="操作">
      <template slot-scope="scope">

        <el-button
            type="danger" icon="el-icon-delete"
            @click="deleteMessage(scope.row.messageId)"
        >删除
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
  <el-dialog title="发布系统消息"
             :visible.sync="addMessageDialog"
             :before-close="handleClose">
    <el-form>
      <el-form-item>
        <el-input v-model="form.title" placeholder="请输入标题"></el-input>
      </el-form-item>
      <el-form-item>
        <el-input v-model="form.content"
                  style="height: 100%" autosize
                  autocomplete="off" type="textarea" placeholder="请输入内容"></el-input>
      </el-form-item>
    </el-form>
    <div slot="footer" class="dialog-footer">
    <el-button @click="addMessageDialog = false" >取 消</el-button>
    <el-button @click="addSystemMessage" type="primary" >确 定</el-button>
  </div>
  </el-dialog>
</div>
</template>

<script>
export default {
  name: "SystemMessageList",
  data() {
    return {
      messageList:[],
      page: 1,
      pageSize: 10,
      size: 10,
      currentPage: 1,
      total: 1,
      show: false,
      addMessageDialog:false,// 显示发布消息的窗口
      form:{
        title:'',
        content:''
      },
      time: '',// 定时任务时间
    }
  },
  methods:{
    // 更改页码
    handleCurrentChange(page) {
      this.currentPage = page;
      this.getMessageList();
    },
    // 更改页面大小
    handleSizeChange(size) {
      this.pageSize = size;
      this.getMessageList();
    },

    // 获取系统消息列表
    getMessageList() {
      this.HttpGet('/sys_msg/list/' + this.currentPage + '/' + this.pageSize)
          .then(res => {
            this.messageList = res.data;
          })
    },
    // 发布系统消息
    addSystemMessage() {
      if(this.form.title === "" || this.form.content === "") {
        this.$message({
          type:'warning',
          message:"标题或者内容不能为空"
        })
      }
      else {
        this.HttpPost('/sys_msg/publish',{
          title: this.form.title,
          content: this.form.content
        }).then(res => {
          if(res.data === false) {
            this.$notify({
              type:'error',
              message:"发布消息失败，请稍后再试",
              title:"系统提示"
            })
          }
          else {
            this.$notify({
              type:'success',
              message:"发布成功",
              title:"系统提示"
            });

            this.getMessageList();
          }
          this.addMessageDialog = false;
        })

      }
    },
    // 删除系统消息
    deleteMessage(id) {
      this.$confirm('此操作将删除该记录, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        this.HttpPost('/sys_msg/delete/' + id)
            .then(res => {
              if(res.data === true) {
                this.$notify({
                  type:'success',
                  message:"删除记录成功",
                  title:'系统提示'
                });
                this.getMessageList();
              }
            })
      }).catch(() => { });
    },
    // 显示发布系统消息的窗口
    showAddMessageDialog() {
      this.addMessageDialog = true;
    },
    // 管理员手动推送消息给所有的用户
    pullMessage() {
      this.HttpPost('/sys_msg/pull')
          .then(res => {
            this.$notify({
              type:'success',
              message:"推送消息成功",
              title:'系统提示'
            })
          })
    },
    handleClose(done) {
      this.$confirm('确认关闭？')
          .then(_ => {
          done();
          })
          .catch(_ => {});
    },
    // 获取定时任务的时间
    getWorkTime() {
        this.HttpGet('/sys_msg/time')
            .then(res => {
              let hour = res.data.split(":")[0];
              let minute = res.data.split(":")[1];
              this.time = new Date();
              this.time.setHours(Number(hour));
              this.time.setMinutes(Number(minute));
              this.time.setSeconds(0);
            })
    },
    // 修改定时任务的时间
    updateWorkTime() {
      let date = new Date(this.time);
      let hour = date.getHours();
      let minute = date.getMinutes();
      this.HttpPost('/sys_msg/time/' + hour + '/' + minute)
          .then(res => {
            if(res.data === true) {
              this.HttpPost('/quartz/stop')
                  .then(res => {
                    if(res === true) {
                      this.HttpPost('/quartz/start')
                          .then(res => {
                            if(res === true) {
                              this.$notify({
                                type:"success",
                                message:"时间修改成功",
                                title:"系统提示"
                              });
                            }
                          })
                    }
                  })
            }
          })
    }

  },
  created() {
    this.getMessageList();
    this.getWorkTime();
  },
  computed: {
    timeChange() {
      return (time) => {
        var date = new Date(time);
        let Y = date.getFullYear() + '-';
        let M = (date.getMonth() + 1 < 10 ? '0' + (date.getMonth() + 1) : date.getMonth() + 1) + '-';
        let D = date.getDate() + ' ';
        let hours = date.getHours() + ":";
        let minute = date.getMinutes() + ":";
        let seconds = date.getSeconds();
        return Y + M + D + " " + hours +  minute + seconds;
      }
    }
  }
}
</script>

<style scoped>

</style>