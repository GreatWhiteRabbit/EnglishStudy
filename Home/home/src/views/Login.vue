<template>
  <div class="login" >
    <el-row class="row">
      <el-col :xl="6" :md="8" :sm="12" :xs="22" class="login-con blog-animation">
        <div class="login-img" >
          <img style="margin-left: 40%" src="~@/assets/logo.svg" />
        </div>
        <p class="login-welcome">欢迎登录！</p>
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
          <el-form-item  prop="password">
            <el-input
                style="width:100%;margin-top: 20px;"
                prefix-icon="el-icon-lock"
                type="password"
                placeholder="请输入密码"
                maxlength="20"
                minlength="8"
                v-model="formCustom.password"
                show-password
            ></el-input>
          </el-form-item>
          <div class="login-btn">
            <el-button  v-loading="loading" class="btn" @click="login('formCustom')">登录</el-button>
            <div class="a-tag user">
              <router-link to="/sign">立即注册</router-link>
              <router-link to="/forget" target="_blank">忘记密码？</router-link>

            </div>
          </div>
        </el-form>
      </el-col>
    </el-row>
  </div>
</template>
<script>
export default {
  name: "Login",
  data() {
    return {
      loading: false,
      restaurants: [],
      formCustom: {
        password: "",
        email: "",
      },
      rules: {
        password: [
          { required: true, message: "请输入密码", trigger: "blur" },
          {
            min: 8,
            max: 20,
            message: "长度在 8到 20 个字符",
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
      }
    };
  },
  methods: {
    login(formName) {
      this.$refs[formName].validate((valid) => {
        if (valid) {
          this.loading = true;
          this.HttpPost('/user/login',{
            email:this.formCustom.email,
            passWord:this.formCustom.password
          }).then(res => {
            this.loading = false;
              if(res.success === false) {
                this.$notify({
                  type:'error',
                  message:"账号或密码错误",
                  title:"登录提示"
                })
              }
              else {
                let token = res.data.token;
                let userId = res.data.id;
                // 获取头像
                this.HttpGet('/user/' + userId)
                    .then(res => {
                      let avatar = res.data.image;
                      localStorage.setItem("avatar",avatar);
                    })
                localStorage.setItem("token",token);
                localStorage.setItem("userId",userId);
                this.$router.go(-1);
                this.$notify({
                  type:'success',
                  message:"登录成功",
                  title:"登录提示"
                })
              }
              })
        }
        else {
          this.$notify({
            type:'error',
            message:"请正确填写邮箱或密码",
            title:"登录提示"
          })
        }
      });
      this.formCustom.password = "";
      this.formCustom.email = "";
    },

  },
  created(){
    localStorage.clear();
  }

};
</script>
<style lang="stylus" scoped>
@import '~@/assets/style/login.styl';

.row

  background-image url("https://bpic.588ku.com/back_pic/05/95/08/215d478e7f88271.jpg")
  min-height: 680px;
  .el-divider__text
    font-size 13px
    color #8c92a4
  .share
    .iconfont
      font-size:23px
      margin:10px
      cursor pointer
      color #ffffff
      border-radius 50%
      padding 7px
      transition all .5s
    .qq
      color rgb(27,193,250)
      border: 2px solid #efefef;
      &:hover
        color #fff
        background-color rgba(27,193,250,.3)
        border: 2px solid rgb(27,193,250);
    .gitee
      color  #fe7300
      border: 2px solid #efefef;
      &:hover
        color #fff
        background-color rgba(254,115,0,.3);
        border: 2px solid  #fe7300;
    .github
      color  #0a0203
      border: 2px solid #efefef;
      &:hover
        color #fff
        background-color rgba(10, 2, 3,.3);
        border: 2px solid  #0a0203;
  

</style>