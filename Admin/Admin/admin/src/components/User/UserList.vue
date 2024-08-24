<template>
<div>
  <el-row :gutter="20">
    <el-col :span="6" >
      <el-input v-model="email" placeholder="邮箱" ></el-input>
    </el-col>
    <el-col :span="6">
      <el-input v-model="name" placeholder="昵称"></el-input>
    </el-col>
      <el-button type="primary" @click="getList">查询</el-button>
      <el-button type="info" @click="addUsers">添加</el-button>
    <el-button
               type="danger"
               icon="el-icon-delete"
               size="large"
               @click="removeList()">删除
    </el-button>
  </el-row>
  <el-table
      ref="multipleTable"
      :data="userList"
      tooltip-effect="dark"
      style="width: 100%"
      :default-sort="{prop: 'time', order: 'descending'}"
      @selection-change="handleSelectionChange">
    <el-table-column
        type="selection"
        :selectable="selectEnable"

        width="55">
    </el-table-column>
    <el-table-column
        label="ID"
        :show-overflow-tooltip="true"
    >
      <template slot-scope="scope">
        <span style="margin-left: 10px">{{ scope.row.userId }}</span>
      </template>
    </el-table-column>

    <el-table-column
        label="邮箱"
        :show-overflow-tooltip="true"
    >
      <template slot-scope="scope">
        <span style="margin-left: 10px">{{ scope.row.email }}</span>
      </template>
    </el-table-column>

    <el-table-column
        label="昵称"
        :show-overflow-tooltip="true"
    >
      <template slot-scope="scope">
        <span style="margin-left: 10px">{{ scope.row.nickName }}</span>
      </template>
    </el-table-column>
    <el-table-column
        label="密码"
        :show-overflow-tooltip="true"
    >
      <template slot-scope="scope">
        <span style="margin-left: 10px">{{ scope.row.passWord }}</span>
      </template>
    </el-table-column>
    <el-table-column
        label="角色"
        :show-overflow-tooltip="true"
    >
      <template slot-scope="scope">
        <span style="margin-left: 10px" v-if="scope.row.status === 1">管理员</span>
        <span style="margin-left: 10px" v-else>普通用户</span>
      </template>
    </el-table-column>

    <el-table-column label="修改密码">
      <template slot-scope="scope" >
        <el-button v-show="scope.row.status !== 1"
            size="mini"
            @click="update(scope.$index)">修改
        </el-button>
      </template>
    </el-table-column>
  </el-table>
  <el-pagination
                 background
                 layout="sizes, prev, pager, next"
                 :page-size="pageSize"
                 :current-page.sync="currentPage"
                 @current-change="handleCurrentChange"
                 @size-change="handleSizeChange"
                 :page-sizes="[10,20,30,40,50,100,150]"
                 :total="total"
                 style="display: flex;justify-content: center;margin-bottom:10px"
  >
  </el-pagination>
  <el-dialog title="修改密码" :visible.sync="tableShow">
    <el-form :model="form">
      <el-form-item label="修改密码" label-width="100px">
        <el-input v-model="form.newPassWord" autocomplete="off" ></el-input>
      </el-form-item>

    </el-form>
    <div slot="footer" class="dialog-footer">
      <el-button @click="cancel">取 消</el-button>
      <el-button type="primary" @click="submit">更 新</el-button>


    </div>
  </el-dialog>
</div>
</template>

<script>
export default {
  name: "UserList",
  data() {
    return {
      multipleSelection: [],
      userList:[],
      email:"",
      name:"",
      page: 1,
      pageSize: 10,
      size: 10,
      currentPage: 1,
      total: 10,
      form:{newPassWord:''},
      tableShow:false,
      index:-1
    }
  },
  methods:{
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
    getList() {
      let email = this.email;
      let name = this.name;
        if(email === "")  email = "null";
        if(name === "") name = "null";
        this.HttpGet('/user/alluser', {
          "email": email,
          "name": name,
          "page": this.currentPage,
          "size": this.pageSize
        })
            .then(res => {
              this.userList = res.data;
            })
      },
    selectEnable(row, rowIndex) {
      // 禁用选择框
      if(row.status === 1) {
        return false;
      }
      return true;
    },
    handleSelectionChange(val) {
      this.multipleSelection = val;
    },
    // 显示dialog
    update(index) {
      this.index = index;
      this.tableShow = true;
    },
    // 提交修改密码
    submit() {
      this.HttpPost('/user/password', {
        "userId": this.userList[this.index].userId,
        "email": this.userList[this.index].email,
        "passWord": this.form.newPassWord,
      }).then(res => {
        if (res.success === true) {
          this.$notify({
            message: "修改成功",
            title: '系统提示',
            type: 'success'
          });
          this.cancel();
        } else {
          this.$notify({
            message: "修改失败",
            title: '系统提示',
            type: 'error'
          })
        }
      })
    },
    // 添加用户
    addUsers() {
      this.$router.push('/home/user/add');
    },
    cancel() {
      this.index = -1;
      this.tableShow = false;
    },
    // 批量删除
    removeList(){
      let userList = [];
      for (let i = 0; i < this.multipleSelection.length; i++) {
        let data = {};
        data.userId = this.multipleSelection[i].userId;
        data.email = this.multipleSelection[i].email;
        data.password = this.multipleSelection[i].passWord;
        userList.push(data);
      }
      this.HttpPost('/user/delete',JSON.stringify({userList}))
          .then(res => {
            this.$notify({
              message:"删除了" + res.data + "条数据",
              title:"系统提示",
              type:'success'
            })
          });
      this.getList();
    }
    },
  created() {
    this.getList();
  }
}
</script>

<style scoped>

</style>