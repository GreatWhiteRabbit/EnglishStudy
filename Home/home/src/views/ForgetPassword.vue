<template>
<el-container>
  <el-main>
    <el-row >
      <el-col style="font-size: 20px;color: gray">忘记密码</el-col>
    </el-row>
    <el-row style="margin-top: 40px;margin-bottom: 100px;">
      <el-steps :active="active" finish-status="success">
        <el-step title="人机验证、发送验证码"></el-step>
        <el-step title="修改密码" ></el-step>
        <el-step title="成功" ></el-step>
      </el-steps>
    </el-row>
    <el-row v-if="active === 0">
      <el-row>
        <el-input  prefix-icon="el-icon-message" style="width: 300px;"
                   v-model="userInfo.email" placeholder="请输入邮箱"></el-input>
        <el-button type="primary" @click="getCharCode">获取验证码</el-button>
      </el-row>
      <el-row>
        <el-col :span="3">
          <el-input minlength="4" maxlength="4" placeholder="四位图片验证码" style="width: 150px" v-model="imageCode"></el-input>
        </el-col>
        <el-col :span="3">
          <el-image  fit="cover" :src="imageUrl"></el-image>
        </el-col>
        <el-col :span="4">
          <el-button @click="refreshImage">
            <i style="font-size: 18px" class="el-icon-refresh"></i>
          </el-button>
        </el-col>
      </el-row>
    </el-row>
    <el-row v-if="active === 1">
      <el-row>
        <el-input  prefix-icon="el-icon-message" style="width: 300px;"
                   v-model="userInfo.email" placeholder="请输入邮箱"></el-input>
      </el-row>
      <el-row>
        <el-input
            style="width:300px;margin-top: 20px; margin-bottom: 10px;"
            prefix-icon="el-icon-lock"
            type="password"
            placeholder="请输入密码"
            maxlength="20"
            minlength="8"
            v-model="userInfo.password"
            show-password
        ></el-input>
      </el-row>
      <el-row>
        <el-input style="width: 300px" placeholder="请输入邮箱验证码"
        v-model="userInfo.charCode"></el-input>
      </el-row>
      <el-row>
        <el-button type="primary" @click="resetPassword">修改密码</el-button>
      </el-row>
    </el-row>
    <el-row v-if="active === 2">
      <el-result icon="success" title="修改密码成功" subTitle="请重新登录">
        <template slot="extra">
          <el-button @click="$router.push('/login')" type="primary" size="medium">登录</el-button>
        </template>
      </el-result>
    </el-row>
  </el-main>
</el-container>
</template>

<script>
export default {
  name: "ForgetPassword",
  data() {
    return {
      imageUrl:'', // 图片地址
      uuid:'',// 图片的uuid
      imageCode:'', // 图片上的文字
      active:0,
      userInfo:{
        email:'',
        charCode:'',
        password:''
      }
    }
  },
  methods:{
    // 获取图片验证码
    getImageCode() {
      this.HttpGet('/user/imageCode')
          .then(res => {
            this.imageUrl = res.data.url;
            this.uuid = res.data.uuid;
          })
    },
    // 获取邮箱验证码
    getCharCode() {
      var length = this.userInfo.email.indexOf("@");
      if(length === -1 || length === 0) {
        this.$notify({
          type:'error',
          title:'输入提示',
          message:'请输入正确的邮箱'
        });
        return;
      }
      if(this.imageCode.length !== 4) {
        this.$notify({
          type:"error",
          message:"请输入图片验证码",
          title:"系统提示"
        });
        return ;
      }
        // 发送验证码
      this.HttpPost('/user/sendCode/' + this.userInfo.email + "/" + this.imageCode + "/" + this.uuid)
          .then(res => {
            if(res.data === false) {
              this.$notify({
                type:"error",
                title:"系统提示",
                message:"邮箱或者验证码输入错误"
              });
            }else {
              this.active = 1;
            }
          })
    },
    // 重置密码
    resetPassword() {
      // 检查密码是否符合规则
      let strRegex = new RegExp(/^[a-z0-9]+$/i);
      if (!strRegex.test(this.userInfo.password)
          || this.userInfo.password.length < 8 || this.userInfo.password.length > 20) {
        this.$notify({
          type: 'error',
          title: "输入提示",
          message: "密码不符合规则，需要包含数字和字母的8-20位字符串"
        })
        return;
      }
      if(this.userInfo.email === "") {
        this.$notify({
          type:'error',
          message:"邮箱不能为空",
          title:"系统提示"
        })
        return ;
      }
      this.HttpPost('/user/forget',{
        email:this.userInfo.email,
        password: this.userInfo.password,
        chaptcha: this.userInfo.charCode
      }).then(res => {
        if(res.success === false) {
          this.$notify({
            type:'error',
            message:res.data,
            title:"系统提示"
          })
        }
        else {
          this.active = 2;
        }
      })
    },
    // 刷新图片验证码
    refreshImage() {
      this.getImageCode();
    }
  },
  created() {
    this.getImageCode();
  }
}
</script>

<style lang="stylus" scoped>
.el-container {
  width 100%;
  background-color white;
  height 100vh;
}
.el-main {
  height 80vh;
  margin-left 20vh;
  margin-right 20vh;
  margin-top 10vh;

}
</style>