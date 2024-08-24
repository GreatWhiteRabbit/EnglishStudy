<template>
  <div  class="route" >
    <el-row>
      <el-col :md="{span:12,offset:6}" :sm="24" style="background-color:#fff;padding:30px 0">
        <el-form
            :model="userInfo" status-icon style="max-width:500px;margin-right:20px"
            ref="userInfo"
            :rules="rules"
            label-width="100px"
            class="demo-ruleForm">
          <el-form-item label="用户昵称" prop="nickName">
            <el-input v-model="userInfo.nickName"></el-input>
          </el-form-item>
          <el-form-item label="email" prop="email">
            <el-col >{{userInfo.email}}</el-col>
          </el-form-item>
          <!--                  图片上传-->
          <el-form-item label="头像" class="avatar-img">
            <el-upload
                class="avatar-uploader"
                :action=actionURL
                list-type="picture-card"
                :show-file-list="false"
                :on-success="handleAvatarSuccess"
                :before-upload="beforeAvatarUpload"
                :onError="uploadError"
                :headers="Header"
                :name="file"
            >
              <i class="el-icon-plus"></i>
            </el-upload>
            <img height="60" v-if="avatar_url" :src="avatar_url" class="avatar" />
          </el-form-item>
          <!--                  图片上传-->

          <el-form-item label="原密码" class="password">
            <el-input type="password" v-model="userInfo.passWord"></el-input>
            <el-button icon="el-icon-edit" class="password-edit" @click="update" style="background: none;
    border: none;"></el-button>
          </el-form-item>
          <el-collapse-transition>

            <div v-if="isUpdate">
              <el-form-item  prop="password" class="password" label="密码" >
                <el-input style="margin-top: 20px" type="password" v-model="password.first"  ></el-input>
              </el-form-item>
              <el-form-item  prop="password" class="password" label="确认密码"  style="margin-top: 40px">
                <el-input type="password" v-model="password.second" ></el-input>
              </el-form-item>
            </div>
          </el-collapse-transition>
          <el-row style="margin-top: 40px">
            <el-button type="primary" @click="checkPassWord">修改</el-button>
            <el-button  @click="cancel">取消</el-button>
          </el-row>
        </el-form>
      </el-col>
    </el-row>

  </div>
</template>
<script>

export default {
  // 个人中心组件
  name:'Setting',
  data(){
    return {
      actionURL:'/apis/user/upload/' + localStorage.getItem("userId") ,
      isUpdate:false,
      loading:true,
      userInfo:{
        nickName:'',
        email:'',
        passWord:''
      },
      cloneInfo:{nickName:'',
        email:'',
        passWord:''},
      rules: {
        nickName: [
          { required: true, message: '请输入密码', trigger: 'blur' },
          { min: 4, max: 20, message: '长度在 8 到 20个字符', trigger: 'blur' }
        ],
        password: [
          { required: true, message: '请输入密码', trigger: 'blur' },
          { min: 8, max: 20, message: '长度在 8 到 20个字符', trigger: 'blur' }
        ]

      },
      avatar_url: sessionStorage.getItem("avatarImg"),
      password:{
        first:"",
        second:""
      },
      Header:{
        Authorization: "Bearer" + " " +  localStorage.getItem("token")
      },
    }
  },
  methods:{
    cancel(){
      this.userInfo = this.cloneInfo;
    },
    update(){
      this.isUpdate=!this.isUpdate;
    },
    checkPassWord() {
      if(this.password.first === this.password.second ) {
        if(this.password.first === "") {
          // 没有修改密码修改其它参数
          this.submitForm();
        }
        else {
          let commit1 = false;
          let commit2 = false;
          // 不会正则表达式，直接for吧
          for(let i = 0; i < this.password.first.length; i++) {
            if(this.password.first.charAt(i) >= '0' && this.password.first.charAt(i) <= '9') {
              commit1 = true;
            }
            else if (this.password.first.charAt(i) >= 'a' && this.password.first.charAt(i) <= 'z') {
              commit2 = true;
            }
            else if (this.password.first.charAt(i) >= 'A' && this.password.first.charAt(i) <= 'Z') {
              commit2 = true;
            }
          }
          // 修改密码
          if(commit1 && commit2) {
            this.submitForm();
          }
          else {
            this.$notify({
              message:"密码格式不符合要求",
              type:'error',
              title:'系统提示'
            })
          }
        }
      } else {
        this.$notify({
          message:"密码不一致",
          type:'error',
          title:'系统提示'
        })
      }
    },
    submitForm() {
      if(this.userInfo.nickName !== this.cloneInfo.nickName) {
        this.HttpPost('/user/update/' + localStorage.getItem("userId"),{
          "nickName":this.userInfo.nickName,
          "email":this.userInfo.email,
          "image":this.avatar_url
        }).then(res => {
          if (res.success === true) {
            this.$notify({
              message: "修改成功",
              title: '系统提示',
              type: 'success'
            })
          } else {
            this.$notify({
              message: "修改失败",
              title: '系统提示',
              type: 'error'
            })
          }
        })
      }
      // 修改密码
      if(this.password.first !== "") {
        this.HttpPost('/user/password', {
          "userId": localStorage.getItem("userId"),
          "email": this.userInfo.email,
          "passWord": this.password.first,
        }).then(res => {
          if (res.success === true) {
            this.$notify({
              message: "修改成功",
              title: '系统提示',
              type: 'success'
            })
          } else {
            this.$notify({
              message: "修改失败",
              title: '系统提示',
              type: 'error'
            })
          }
        })
      }
    },
    getUserInfo(){
      // 获取用户用户信息
      let userId = localStorage.getItem("userId");
      this.HttpGet("/user/" + userId)
          .then(res => {
            this.userInfo.email = res.data.email;
            this.userInfo.nickName = res.data.nickName;
            this.avatar_url = res.data.image;
            this.userInfo.passWord = "nullnull";
            this.cloneInfo.email = this.userInfo.email;
            this.cloneInfo.nickName = this.userInfo.nickName;
            this.cloneInfo.passWord = this.userInfo.passWord;
          })
    },

    handleAvatarSuccess(res, file) {
      this.avatar_url= res.data;
      localStorage.setItem("avatar",res.data);
      this.$notify({
        message:"上传成功",
        type:'success',
        title:'系统提示'
      })
    },
    beforeAvatarUpload(file) {
      const arr = [
        "image/jpeg",
        "image/gif",
        "image/jpg",
        "image/png",
        "image/x-png",
        "image/pjpeg",
      ];
      const isJPG = arr.indexOf(file.type) >= 0;
      const isLt2M = file.size / 1024 / 1024 < 4;
      if (!isJPG) {
        this.$notify({
          message:'文件格式不正确',
          title:"系统提示",
          type:'error'
        })
      }
      if (!isLt2M) {
        this.$notify({
          message:'图片大小不能超过4MB',
          title:"系统提示",
          type:'error'
        })
      }
      return isJPG && isLt2M;
    },
    uploadError() {
      this.$notify({
        message:"上传失败",
        title:'系统提示',
        type:"error"
      })
    },
  },
  mounted(){
    this.getUserInfo()
  }

}
</script>
<style lang="stylus" scoped>
.password
  position relative
  .password-edit
    position:absolute
    right:30px
    top:0
.avatar-img {
  top: 0;
  right: 150px;
}
.avatar {
  position: absolute;
  top: 0;
  left: 160px;
  height: 148px;
  max-width: 148px;
  border-radius: 5px;
  display: block;
}
</style>