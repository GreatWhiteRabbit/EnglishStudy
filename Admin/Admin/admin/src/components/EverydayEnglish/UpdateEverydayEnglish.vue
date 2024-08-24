<template>
  <div>
    <el-skeleton :rows="20" animated v-show="show"/>
    <div>
      <!--原文    -->
      <div class="content">
        <el-row class="container-marked">
          <el-col class="blog-content" :span="22" :offset="1">
            <div>
              <header class="article-header blog-animation">

                <div class="istitle">
                  <h1
                      class="article-title"
                  >{{ everydayEnglish.title }}</h1>

                </div>
              </header>
              <hr/>
            </div>
            <div id="write" class="blog-content blog-animation" style="padding-bottom: 20px;">
              {{ everydayEnglish.content }}
            </div>
            <div class="archive-article-date">
                    <span>
                  <i class="iconfont iconriqi icon"></i>
                      发布时间
                  {{ timeChange(everydayEnglish.time) }}
              </span>
            </div>
          </el-col>
        </el-row>
      </div>

      <!--翻译  -->

      <div class="content">
        <el-row class="container-marked">
          <el-col class="blog-content" :span="22" :offset="1">
            <div>
              <header class="article-header blog-animation">
                <div class="istitle">
                  <h1
                      class="article-title"
                  >{{ everydayEnglish.titleTranslations }}</h1>
                  <div class="archive-article-date">
                  </div>
                </div>
              </header>
              <hr/>
            </div>

            <div id="write" class="blog-content blog-animation" style="padding-bottom: 20px;">
              {{ everydayEnglish.translations }}
            </div>
          </el-col>

        </el-row>
      </div>


    </div>
  <el-row>
    <el-col :span="12">
      <el-button style="margin-left: 50%"
                 type="primary"
                 size="large"
                 @click="showTable"
      >修改
      </el-button>
    </el-col>
    <el-col :span="6">
      <el-button
          v-show="submit"
          style="margin-left: 50%"
          type="primary"
          size="large"
          @click="update"
      >提交修改
      </el-button>
    </el-col>

  </el-row>

    <el-dialog title="修改每日英语" :visible.sync="tableShow">
      <el-form :model="form">
        <el-form-item label="每日英语标题" label-width="100px">
          <el-input v-model="form.title" autocomplete="off" type="textarea"></el-input>
        </el-form-item>
        <el-form-item label="每日英语内容" label-width="100px" style="height: 60px">
          <el-input v-model="form.content" autocomplete="off" type="textarea"></el-input>
        </el-form-item>
        <el-form-item label="每日英语标题翻译" label-width="100px">
          <el-input v-model="form.titleTranslations" autocomplete="off" type="textarea"></el-input>
        </el-form-item>
        <el-form-item label="每日英语内容翻译" label-width="100px" style="height: 60px">
          <el-input v-model="form.translations" autocomplete="off" type="textarea"></el-input>
        </el-form-item>

      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="cancel">取 消</el-button>
        <el-button type="primary" @click="update">更 新</el-button>
        <el-button type="success" @click="preview">预 览</el-button>

      </div>
    </el-dialog>
  </div>
</template>

<script>
export default {
  name: "UpdateEverydayEnglish",
  data() {
    return {
      show: true,
      tableShow: false,
      id: 0,
      submit: false,
      everydayEnglish: {
        title: '',
        content: '',
        translations: '',
        time: '',
        titleTranslations: ''
      },
      form: {
        title: '',
        content: '',
        translations: '',
        time: '',
        titleTranslations: ''
      },
    }
  },
  methods: {
    // 根据id获取每日英语
    getEveryDayById() {
      let id = this.$route.params.id;
      this.id = id;
      this.HttpGet('/everyday/' + id)
          .then(res => {
            if (res.data.everyday_id === 0) {
              // 跳转到404页面
              this.$router.push('/404')
            } else {
              this.everydayEnglish.title = res.data.title;
              this.everydayEnglish.content = res.data.content;
              this.everydayEnglish.translations = res.data.translations;
              this.everydayEnglish.time = res.data.time;
              this.everydayEnglish.titleTranslations = res.data.titleTranslations;
              this.show = false;
            }
          })
    },
    // 显示修改表单
    showTable() {
      this.tableShow = true;
      this.form = this.everydayEnglish;
    },
    //预览修改后的小高
    preview() {
      this.tableShow = false;
      this.everydayEnglish = this.form;
      this.submit = true;
    },
    // 取消修改
    cancel() {
      this.tableShow = false;
    },
    // 更新
    update() {
      this.$confirm('是否修改每日英语?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        // 发送网络请求
        this.HttpPost('/everyday/update', {
          "everyday_id":this.id,
          "title":this.form.title,
          "content":this.form.content,
          "translations":this.form.translations,
          "titleTranslations":this.form.titleTranslations,
        })
            .then(res => {
          if (res.data.success === false) {
            this.$notify({
              message: '服务器异常',
              title: '系统消息',
              type: 'warning'
            })
          } else {
            this.$notify({
              message: "修改成功",
              title: '系统消息',
              type: 'success'
            });
            this.getEveryDayById();
            this.submit=false;

          }
        })
      })
    }

  },
  created() {
    this.getEveryDayById();
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


.grid-content {
  border-radius: 4px;
  min-height: 36px;

}

.row-bg {
  padding: 10px 0;
  background-color: #f9fafc;
}

.container >>> .el-divider__text {
  background-color: #f8f8f8;
}

.content {
  padding: 60px 0 30px 0;
  font-size 20px;

  .container-marked {
    border-radius: 6px;
    background-color #fff
    margin auto;
    max-width: 1320px;
    width: 95%;

    .share {
      display: flex;
      align-items: center;

      a {
        display: inline-block;
        width: 34px;
        height: 34px;
        line-height: 34px;
        text-align: center;
        font-size: 22px;
        margin: 6px;
        border-radius: 50%;
        border: 1px solid;
      }

      .icon-qzone {
        color: #fdbe3d;
        border-color: #fdbe3d;

        &:hover {
          background-color: #fdbe3d;
          color: #fff;
        }
      }

      .icon-qq {
        color: #56b6e7;
        border-color: #56b6e7;

        &:hover {
          background-color: #56b6e7;
          color: #fff;
        }
      }

      .like {
        background-color: var(--main-5);
        color: #fff !important;
      }

      .icon-love {
        width: 50px;
        height: 50px;
        color: var(--main-5);
        border-color: var(--main-5);
        font-size: 24px;

        &:hover {
          background-color: var(--main-5);
          color: #fff;
        }

        p {
          font-size: 16px;
          line-height: 10px;
          font-family: georgia;
          font-weight: bold;
        }
      }

      .icon-weibo {
        color: #ff763b;
        border-color: #ff763b;

        &:hover {
          background-color: #ff763b;
          color: #fff;
        }
      }

      .icon-wechat {
        color: #7bc549;
        border-color: #7bc549;

        &:hover {
          background-color: #7bc549;
          color: #fff;
        }
      }
    }

    .blog-info {
      border-left: 3px solid var(--main-6);
      padding-left: 8px;
      border-radius: 4px;
      margin: 30px 0
    }

    .blog-info p {
      text-align: left;
      color: #4d4d4d;
      font-size: 14px;
      margin: 0;
    }

    .article-prev-next {
      margin: 10px 0;
      overflow hidden
    }

    .article-prev-next i {
      font-weight bold
    }

    .article-prev-next .prev {
      float left
      color #969696
    }

    .article-prev-next .next {
      float right
      color #969696
    }

    .blog-mess {
      padding-bottom: 15px;

      .next {
        text-align: center;
        color: var(--main-5);
        transition: all 0.3s;

        &:hover {
          color: var(--main-6);
        }

        span {
          cursor: pointer;

          .next-icon {
            animation: next 0.6s linear infinite alternate;
          }
        }
      }
    }
  }
}

@keyframes next {
  0% {
    transform: translateY(3px);
  }

  100% {
    transform: translateY(0px);
  }
}

@media (max-width: 992px) {
  .isimg {
    height: 240px !important;
  }

  .article-title {
    font-size: 22px !important;
  }

  .container-marked {
    width: 100% !important
  }

}


.article-header {
  border-left: 5px solid #4d4d4d;
  padding: 30px 0 15px 25px;
  position: relative;
  color: #4d4d4d;
  overflow: hidden;
  border-radius: 4px;
}

.article-title {
  margin: 10px 0;
  text-align: left;
  font-size: 30px;
  font-weight: 700;
}

.article-title:after {
  display none
}

.article-img {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  z-index: 0;
  filter: blur(6px);
}

.isimg {
  height: 350px;
  border: none;
  text-align: center;
  padding: 0;
}

.istitle {
  position: absolute;
  width: 90%;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  z-index: 2;
  color: #51cacc;

  span {
    color: #51cacc;
  }
}

.archive-article-date {
  color: #ed6d6d;
  display: flex;
  flex-wrap: wrap;
  justify-content: space-between;
  text-align: left;
  font-size 20px;
}

.archive-article-date .icon {
  margin: 5px 5px 5px 0;
}

[class*=' icon-'], [class^=icon-] {
  speak: none;
  font-style: normal;
  font-weight: 400;
  font-variant: normal;
  text-transform: none;
  line-height: 1;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
}
</style>