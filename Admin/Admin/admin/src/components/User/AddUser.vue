<template>
  <div>
    <el-table
        ref="multipleTable"
        :data="userList"
        tooltip-effect="dark"
        style="width: 100%"
        >

      <el-table-column
          label="邮箱"
          :show-overflow-tooltip="true"
      >
        <template slot-scope="scope">
          <span style="margin-left: 10px" v-if="scope.row.isEdit">{{ scope.row.email }}</span>
          <el-input v-model="userList[scope.$index].email" placeholder="请输入邮箱" v-else></el-input>
        </template>
      </el-table-column>

      <el-table-column
          label="密码"
          :show-overflow-tooltip="true"
      >
        <template slot-scope="scope">
          <span style="margin-left: 10px"  v-if="scope.row.isEdit">{{ scope.row.passWord }}</span>
          <el-input v-model="userList[scope.$index].passWord" placeholder="请输入密码" v-else></el-input>
        </template>
      </el-table-column>


      <el-table-column label="操作">
        <template slot-scope="scope" >
          <el-button  v-if="scope.row.isEdit === true" type="primary" icon="el-icon-edit" @click="editRow(scope.$index)">编辑</el-button>
          <el-button v-else type="info" icon="el-icon-edit" disabled>编辑</el-button>
          <el-button type="danger" icon="el-icon-delete" @click="deleteRow(scope.$index)">删除</el-button>
          <el-button type="success" @click="savaRow(scope.$index)">保存</el-button>
        </template>
      </el-table-column>
    </el-table>
    <el-row style="background-color: #67C23A">
      <span>
        <el-button @click="submit" style="background-color: #67C23A;border: none;margin-left: 50%;font-size: 20px;color: white">提交</el-button>
      </span>
    </el-row>
    <el-row style="background-color: #409EFF" >
      <el-button @click="addRow" style="margin-left: 50%;font-size: 25px;color: white;background-color: #409EFF;border: none">+</el-button>
    </el-row>
  </div>
</template>

<script>
import qs from "qs";

export default {
  name: 'AddUser',
  data() {
    return {
      userList:[],
      edit:false, // 是否在此页面编辑过
    }
  },
  methods: {
    // 添加一行
   addRow() {
     this.edit = true;
     if(this.userList.length !== 0) {
       let edit = true; // 判断是否存在未编辑的数据
       for(let i = 0; i < this.userList.length; i++) {
         if(this.userList[i].isEdit === false) {
           edit = false;
         }
       }
       if(edit === false) {
         this.$notify({
           message:"还有未保存的数据",
           type:'warning'
         })
       }
       else {
         let data = {
           "email":"",
           "passWord":"",
           "isEdit":false
         };
         this.userList.push(data);

       }
     }
     else {
       let data = {
         "email":"",
         "passWord":"",
         "isEdit":false
       };
       this.userList.push(data);
     }

   },
    // 删除一行
    deleteRow(row) {
      this.userList.splice(row,1);
    },
    // 提交到服务器
    submit() {
      let userList = [];
      for(let i = 0; i < this.userList.length; i++) {
        let data = {};
        data.email = this.userList[i].email;
        data.passWord = this.userList[i].passWord;
        userList.push(data);
      }
      // 发送网络请求
      this.HttpPost('/user/add',JSON.stringify({userList})
    )
          .then(res => {
            if(res.data === 0) {
              this.$notify({
                message:"成功数据失败",
                title:"系统提示",
                type:"error"
              })
            }
            else {
              this.$notify({
                message:"成功上传了" + res.data +"数据",
                title:"系统提示",
                type:"success"
              });
              this.edit = false;
            }
          })

    },
    // 编辑一行
    editRow(row) {
      this.userList[row].isEdit = false;
    },
    // 保存一行的数据
    savaRow(row) {
     if(this.userList[row].email === "" || this.userList[row].passWord === "") {
       this.$notify({
         message:"邮箱或密码不能为空",
         type:'warning',
         title:'系统提示'
       })
     }
     else {
        this.userList[row].isEdit = true;
     }
    }

  },
  components: {},

  beforeRouteLeave (to, from, next) {
    if (this.edit === true) {
      setTimeout(() => {
        this.$confirm('页面内容未保存，是否退出此页面', '提示', {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning'
        }).then(() => {
          next();
        }).catch(() => {
          next(false)
        })
      }, 200);
    } else {
      next();
    }
  }
}
</script>

<style lang="stylus">

</style>


