<template>
  <div class="login" style="">
    <div class="login-box">
      <el-form ref="loginForm" :model="form" :rules="rules" class="">
        <img
            style="width:60%;margin:10px 20%;"
            src="~@/assets/logo.svg"
        />
        <a href="javascript:;" style="text-align:center;display:block;margin:0px 0 30px 0">后台管理系统</a>
        <el-form-item prop="name">
          <el-input type="text" placeholder="请输入邮箱" v-model="form.email"/>
        </el-form-item>
        <el-form-item prop="password">
          <el-input type="password" placeholder="请输入密码" v-model="form.password"/>
        </el-form-item>
        <el-button type="primary" class="login-btn" style="width:100%" @click="onSubmit('loginForm')">登录</el-button>
      </el-form>
    </div>
  </div>
</template>
<script>


export default {
  name: "Login",
  components: {},
  data() {
    return {
      form: {
        type: 'name',
        email: '',
        password: ''
      },
      // 表单验证，需要在 el-form-item 元素中增加 prop 属性
      rules: {
        email: [
          {required: true, message: '账号不可为空', trigger: 'blur'},
          {min: 3, max: 20, message: '账号长度在 3 到 10个字符', trigger: 'blur'}
        ],
        password: [
          {required: true, message: '密码不可为空', trigger: 'blur'},
          {min: 8, max: 16, message: '密码长度在 3 到 20个字符', trigger: 'blur'}
        ]
      }
    }
  },
  methods: {
    onSubmit(formName) {
      this.$refs[formName].validate((valid) => {
        if (valid) {
          this.login()
        } else {
          return false;
        }
      });
    },
    login() {
      this.HttpPost("/user/adminLogin",
          {
            "email": this.form.email,
            "password": this.form.password
          }).then(res =>  {
            if(res.success === true) {
              this.$notify({
                message:'登录成功',
                type:"success",
                title:'系统提示'
              });
              localStorage.setItem("token",res.data.token);
              localStorage.setItem("userId",res.data.id);
              this.$router.push("/home/words/list");
            } else {
              this.$notify({
                message:'账号或密码错误',
                type:"error",
                title:'系统提示'
              })
            }
      })
    }
  },
  created() {
    if (sessionStorage.getItem("userId") !== null) {
      this.$router.push("/home/words/list");
    }
  },
}

</script>


<style lang="stylus" scoped>
@import '~@/assets/style/home.styl';
.login {
  background-image: linear-gradient(120deg, #84fab0 0%, #8fd3f4 100%);
  width: 100%;
  height: 100vh
  background-image: url(https://ts1.cn.mm.bing.net/th/id/R-C.91a2fdaba7d48e0045ceac895f203228?rik=us7dY%2byb8MxNDw&riu=http%3a%2f%2fbpic.588ku.com%2fback_pic%2f03%2f90%2f81%2f5457de39ec0e238.jpg!%2ffh%2f300%2fquality%2f90%2funsharp%2ftrue%2fcompress%2ftrue&ehk=qFThTtCYhN1bUBANniUGvsrf6KEDyD7CAuO2nnW0gRI%3d&risl=&pid=ImgRaw&r=0);
  background-repeat: no-repeat;
  background-position: center;
  background-size: cover
  display: flex;
  justify-content: center;
  align-items: center;
}

.login-box {
  // position:fixed
  // top: 0px;
  // left: 0px;
  // right: 0px;
  margin-left: auto;
  margin-right: auto;
  border: 1px solid rgba(255, 255, 255, .4)
  background: rgba(255, 255, 255, .4)
  backdrop-filter: blur(2px);
  width: 300px
  border-radius: 5px
  padding: 40px 20px
  box-shadow: 0 28px 50px rgba(25, 24, 40, .35);
  // box-shadow: 0 0 25px #ccc;
}

.login-title {
  text-align: center;
  margin: 0 auto 40px auto;
  color: #303133;
}

.login-btn {
  background-image: linear-gradient(270deg, var(--main-8), var(--main-6));
}

@media (max-width: 700px) {
  .login-box {
    width: 90%;
    margin: auto 5%
    box-sizing border-box
  }
}
</style>