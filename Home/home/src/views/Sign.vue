<template>
  <div class="login" >
    <el-row class="row">
      <el-col :xl="6" :md="8" :sm="12" :xs="22" class="login-con blog-animation">
        <div class="login-img" >
          <img style="margin-left: 40%" src="~@/assets/logo.svg" />
        </div>
        <p class="login-welcome">欢迎注册！</p>
        <el-form :model="formCustom" status-icon :rules="rules" ref="formCustom">
          <el-form-item prop="email">
            <el-input
                style="width:100%"
                prefix-icon="el-icon-message"
                type="email"
                placeholder="邮箱地址"
                v-model="formCustom.email"
            ></el-input>
          </el-form-item>
          <el-form-item  prop="first">
            <el-input
                style="width:100%;margin-top: 10px;"
                prefix-icon="el-icon-lock"
                type="password"
                placeholder="请输入密码"
                maxlength="20"
                minlength="8"
                v-model="formCustom.first"
                show-password
            ></el-input>
          </el-form-item>
          <el-form-item  prop="second">
            <el-input
                style="width:100%;margin-top: 10px;"
                prefix-icon="el-icon-lock"
                type="password"
                placeholder="请确认密码"
                maxlength="20"
                minlength="8"
                v-model="formCustom.second"
                show-password
            ></el-input>
          </el-form-item>
          <el-form-item>
            <el-row>
                <el-input minlength="4" maxlength="4" placeholder="四位验证码" style="width: 100px" v-model="imageCode"></el-input>
                <el-image style="height: 40px; margin-top: 10px;" fit="cover" :src="imageUrl"></el-image>
                <el-button @click="refreshImage">
                  <i style="font-size: 18px" class="el-icon-refresh"></i>
                </el-button>
            </el-row>
          </el-form-item>
          <el-form-item prop="Code"  class="checkCode">
            <el-input
                style="margin-top: 10px;"
                v-model="formCustom.code"
                placeholder="请输入6位验证码"
                type="text"
                autocomplete="off"
            ></el-input>
            <el-button
                @click.stop="sendVerificationCode"
                type="primary"
                style="margin-left: 10px"
                v-if="show"
            >发送验证码</el-button
            >
            <el-button
                type="primary"
                style="margin-left: 10px"
                v-if="!show"
                disabled
            >{{ count }}秒后重发</el-button>
          </el-form-item>
          <div class="login-btn">
            <el-button v-loading="loading" class="btn" @click="login('formCustom')" >注册</el-button>
            <div class="a-tag user">
              <router-link to="/login">已有账号?</router-link>
            </div>
          </div>
        </el-form>
      </el-col>
    </el-row>
  </div>
</template>
<script>

export default {
  name: "Sign",
  data() {
    return {
      imageUrl:'', // 图片地址
      imageCode:'',// 图片验证码
      uuid:'',// 图片的uuid
      show: true,
      count: "",
      loading: false,
      restaurants: [],
      canLogin:false, // 是否允许登录
      formCustom: {
        first: "",
        second:'',
        email: "",
        code:""
      },
      rules: {
        password: [
          { required: true, message: "请输入密码", trigger: "blur" },
          {
            min: 8,
            max: 20,
            message: "长度在 8到 20 个字符，包含数字和字母",
            trigger: "change",
          },
        ],
        email: [
          { required: true, message: "请输入邮箱", trigger: "blur" },
          {
            type: "email",
            message: "请输入正确的邮箱地址",
            trigger: "change",
          },
        ],
      },
    };
  },
  methods: {
    sendVerificationCode() {
      var length = this.formCustom.email.indexOf("@");
      if(length === -1 || length === 0) {
        this.$notify({
          type:'error',
          title:'输入提示',
          message:'请输入邮箱'
        })
        return;
      }
      if(this.formCustom.first !== this.formCustom.second) {
        this.$notify({
          type:'error',
          title:"输入提示",
          message:"前后两次密码不一致"
        })
        return;
      }
      else {
        let strRegex = new RegExp(/^[a-z0-9]+$/i);
        if (!strRegex.test(this.formCustom.first)
            || this.formCustom.first.length <8 || this.formCustom.first.length > 20) {
          this.$notify({
            type:'error',
            title:"输入提示",
            message:"密码不符合规则，需要包含数字和字母的8-20位字符串"
          })
          return;
        }
      }
      if(this.imageCode.length !== 4) {
        this.$message.error("请输入图片验证码");
        return;
      }
      let TIME_COUNT = 180;
      if (!this.timer) {
        this.count = TIME_COUNT;
        this.show = false;
        this.timer = setInterval(() => {
          if (this.count > 0 && this.count <= TIME_COUNT) {
            this.count--;
          } else {
            this.show = true;
            clearInterval(this.timer);
            this.timer = null;
          }
        }, 1000);
        this.getCode();
      }
    },
    //通过用户输入的邮箱发送到指定的邮箱中取
    getCode() {
     this.HttpGet('/user/chaptcha/' + this.formCustom.email + '/' + this.imageCode + '/' + this.uuid)
         .then(res => {
           if(res.data !== "邮件发送成功") {
             this.count = 0;
           }
          this.$notify({
            type:'info',
            message:res.data,
            title:"系统提示"
          });
         })

    },
    // 注册
    login(formName) {

      this.$refs[formName].validate((valid) => {
        if (valid) {
          let that = this;
          if (!that.loading) {
            that.loading = true;
            this.HttpPost('/user/registered',{
              email:this.formCustom.email,
              passWord:this.formCustom.first,
              chaptcha:this.formCustom.code
            })
              .then(res => {
                if(res.success === true) {
                 localStorage.setItem("userId",res.data.id);
                 localStorage.setItem("token",res.data.token);
                  this.$router.go(-1);
                  this.$notify({
                    type:"success",
                    message:"注册成功",
                    title:"系统提示"
                  })
                } else {
                  this.$notify({
                    type:'error',
                    title:'错误提示',
                    message:"验证码错误或邮箱错误"
                  })
                }
                that.loading = false;
              })
          } else {
            that.$message({
              showClose: true,
              message: "正在注册中……",
              type: "success",
            });
          }
        } else {
          this.$notify.error({
            title: "错误",
            message: "按照提示正确填写信息",
          });
          return false;
        }
      });
    },
    // 获取图片验证码
    getImageCode() {
      this.HttpGet('/user/imageCode')
          .then(res => {
            this.imageUrl = res.data.url;
            this.uuid = res.data.uuid;
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

};
</script>
<style lang="stylus" scoped>
@import '~@/assets/style/login.styl';

.row {
  background-image url("https://bpic.588ku.com/back_pic/05/95/08/215d478e7f88271.jpg")
  min-height: 680px;
}
.checkCode ::v-deep .el-form-item__content {
  display: flex;
  align-items: center;
  justify-content: space-around;
}
</style>