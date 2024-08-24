<template>
  <div>
    <div class="content">
      <el-row class="container-marked">
        <el-col class="blog-content" :span="22" :offset="1">
          <div>
            <header class="article-header blog-animation">
              <div class="istitle">
                <h1
                    class="article-title"
                >{{ passageForm.title }}</h1>
              </div>
            </header>
            <hr/>
            <el-col>
              <el-row style="margin-bottom: 5px">
                <el-button @click="showPassageDialog" style="margin-left: 100%"
                           type="primary" icon="el-icon-edit">
                </el-button>
              </el-row>
              <el-row style="margin-bottom: 5px">
                <el-button
                    v-show="passageEdit"
                    @click="savaPassage" style="margin-left: 100%;font-size: 25px"
                    icon="el-icon-document-checked"
                    type="primary">
                  </el-button>
              </el-row>
              <el-row >
                <el-button @click="showAddQuestionDialog" style="margin-left: 100%;font-size: 25px"
                           type="primary">
                  +</el-button>
              </el-row>
            </el-col>

          </div>
          <div id="write" class="blog-content blog-animation" style="padding-bottom: 20px;">
            {{ passageForm.content }}
          </div>
          <div style="font-family:'Hiragino Sans GB';color: #767778">
            <div v-for="(item,index) in questionList" :key="index" style="">
              <el-row style="margin-top: 20px;border-top: groove">
                <el-col >
                  <span>{{index + 1}} {{item.title}}</span>
                </el-col>
              </el-row>
              <el-row style="margin-top: 10px">
                <el-col :span="20">
                  <el-row>
                    <el-col >
                      A: {{item.optionsA}}
                    </el-col>
                  </el-row>
                  <el-row style="margin-top: 10px">
                    <el-col >B: {{item.optionsB}}</el-col>
                  </el-row>
                  <el-row style="margin-top: 10px">
                    <el-col>C: {{item.optionsC}}</el-col>
                  </el-row>
                  <el-row style="margin-top: 10px">
                    <el-col>D: {{item.optionsD}}</el-col>
                  </el-row>
                  <el-row style="margin-top: 10px">
                    <el-col>答案: {{item.answer}}</el-col>
                  </el-row>
                  <el-row style="margin-top: 10px">
                    <el-col>详细解答: {{item.explanation}}</el-col>
                  </el-row>
                </el-col>
                <el-col :span="4">
                  <el-row>
                    <el-button @click="showQuestionDialog(index)" type="primary" icon="el-icon-edit" circle></el-button>
                  </el-row>
                  <el-row>
                    <el-button @click="updateQuestion(index)"
                               v-show="questionEdit === true && index === questionIndex"
                               type="primary" icon="el-icon-document-checked" circle></el-button>
                  </el-row>
                  <el-row style="margin-top: 10px">
                    <el-button @click="deleteQuestion(index)" type="danger" icon="el-icon-delete" circle></el-button>
                  </el-row>
                </el-col>
              </el-row>
            </div>
          </div>
        </el-col>
      </el-row>
    </div>
    <el-dialog title="添加或修改阅读理解" :visible.sync="passageDialogShow">
      <el-form :model="passageForm">
        <el-form-item label="阅读理解标题" label-width="100px">
          <el-input v-model="passageForm.title" autocomplete="off"  type="textarea"></el-input>
        </el-form-item>
        <el-form-item label="阅读理解内容" label-width="100px" >
          <el-input v-model="passageForm.content" autocomplete="off" autosize type="textarea"></el-input>
        </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="cancelPassage">取 消</el-button>

        <el-button type="success" @click="previewPassage">预 览</el-button>

      </div>
    </el-dialog>
    <el-dialog title="修改问题及选项" :visible.sync="questionDialogShow">
      <el-form :model="questionForm">
        <el-form-item label="问题" label-width="100px">
          <el-input v-model="questionForm.title" autocomplete="off" type="textarea"></el-input>
        </el-form-item>
        <el-form-item label="A选项" label-width="100px" style="height: 60px">
          <el-input v-model="questionForm.optionsA" autocomplete="off" type="textarea"></el-input>
        </el-form-item>
        <el-form-item label="B选项" label-width="100px" style="height: 60px">
          <el-input v-model="questionForm.optionsB" autocomplete="off" type="textarea"></el-input>
        </el-form-item>
        <el-form-item label="C选项" label-width="100px" style="height: 60px">
          <el-input v-model="questionForm.optionsC" autocomplete="off" type="textarea"></el-input>
        </el-form-item>
        <el-form-item label="D选项" label-width="100px" style="height: 60px">
          <el-input v-model="questionForm.optionsD" autocomplete="off" type="textarea"></el-input>
        </el-form-item>
        <el-form-item label="正确答案" label-width="100px" style="height: 60px">
          <el-input v-model="questionForm.answer" autocomplete="off" type="textarea"></el-input>
        </el-form-item>
        <el-form-item label="详细解答" label-width="100px" style="height: 100%">
          <el-input v-model="questionForm.explanation" autocomplete="off" autosize type="textarea"></el-input>
        </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="cancelQuestion">取 消</el-button>

        <el-button type="success" @click="previewQuestion">预 览</el-button>

      </div>
    </el-dialog>
    <el-dialog title="添加问题及选项" :visible.sync="addQuestionDialogShow">
      <el-form :model="addQuestionForm">
        <el-form-item label="问题" label-width="100px">
          <el-input v-model="addQuestionForm.title" autocomplete="off" type="textarea"></el-input>
        </el-form-item>
        <el-form-item label="A选项" label-width="100px" style="height: 60px">
          <el-input v-model="addQuestionForm.optionsA" autocomplete="off" type="textarea"></el-input>
        </el-form-item>
        <el-form-item label="B选项" label-width="100px" style="height: 60px">
          <el-input v-model="addQuestionForm.optionsB" autocomplete="off" type="textarea"></el-input>
        </el-form-item>
        <el-form-item label="C选项" label-width="100px" style="height: 60px">
          <el-input v-model="addQuestionForm.optionsC" autocomplete="off" type="textarea"></el-input>
        </el-form-item>
        <el-form-item label="D选项" label-width="100px" style="height: 60px">
          <el-input v-model="addQuestionForm.optionsD" autocomplete="off" type="textarea"></el-input>
        </el-form-item>
        <el-form-item label="正确答案" label-width="100px" style="height: 60px">
          <el-input v-model="addQuestionForm.answer" autocomplete="off" type="textarea"></el-input>
        </el-form-item>
        <el-form-item label="详细解答" label-width="100px" style="height: 100%">
          <el-input v-model="addQuestionForm.explanation" autocomplete="off" autosize type="textarea"></el-input>
        </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="cancelAddQuestion">取 消</el-button>
        <el-button type="success" @click="previewAddQuestion">预 览</el-button>

      </div>
    </el-dialog>
  </div>
</template>

<script>
export default {
  name: "AddPassage",
  data() {
    return{
      questionList:[],
      passageDialogShow:true,// 显示修改文章内容或者标题窗口
      questionDialogShow:false,// 显示修改文章选项和答案窗口
      addQuestionDialogShow:false, // 显示添加选项窗口,
      passageForm:{
        title:"",
        content:"",
        questionList:[]
      },
      questionForm:{
        title:"",
        optionsA:'',
        optionsB:'',
        optionsC:'',
        optionsD:'',
        answer:'',
        explanation:""
      },
      addQuestionForm:{
        title:"",
        optionsA:'',
        optionsB:'',
        optionsC:'',
        optionsD:'',
        answer:'',
        explanation:""
      },
      passageEdit:false,// 判断passage是否更新
      questionEdit:false, // 判断question是否更新,
      addQuestionEdit:false, // 判断是否添加新的题目
      questionIndex:-1
    }
  },
  methods:{
    // 显示修改文章内容或者标题窗口
    showPassageDialog() {
      this.passageDialogShow = true;
      this.questionDialogShow = false;
      this.addQuestionDialogShow = false;
    },

    // 显示修改文章选项和答案窗口
    showQuestionDialog(index) {
        this.questionIndex = index;
        this.questionDialogShow = true;
        this.passageDialogShow = false;
        this.addQuestionDialogShow = false;
        if (this.questionEdit === false) {
          this.questionForm.title = this.questionList[index].title;
          this.questionForm.optionsB = this.questionList[index].optionsB;
          this.questionForm.optionsC = this.questionList[index].optionsC;
          this.questionForm.optionsD = this.questionList[index].optionsD;
          this.questionForm.optionsA = this.questionList[index].optionsA;
          this.questionForm.answer = this.answerList[index].answer;
          this.questionForm.explanation = this.answerList[index].explanation;
        }
    },

    // 取消更新passage
    cancelPassage() {
      this.passageDialogShow = false;

    },
    // 预览passage
    previewPassage(){
      this.passageDialogShow = false;
      this.passageEdit = true;
    },
    // 取消更新question
    cancelQuestion() {
      this.questionDialogShow = false;

    },
    // 预览选项
    previewQuestion(){
      let index = this.questionIndex;
      this.questionEdit = true;
      this.questionDialogShow = false;
      this.questionList[index].title = this.questionForm.title;
      this.questionList[index].optionsA = this.questionForm.optionsA;
      this.questionList[index].optionsB = this.questionForm.optionsB;
      this.questionList[index].optionsC = this.questionForm.optionsC;
      this.questionList[index].optionsD = this.questionForm.optionsD;
      this.answerList[index].answer = this.questionForm.answer;
      this.answerList[index].explanation = this.questionForm.explanation === "" ? "略" : this.questionForm.explanation;
    },
    // 显示添加题目dialog
    showAddQuestionDialog() {
      this.addQuestionEdit = true;
      this.addQuestionDialogShow = true;
      this.passageDialogShow = false;
      this.questionDialogShow = false;
    },
    // 取消添加问题
    cancelAddQuestion() {
      this.addQuestionDialogShow = false;
    },
    previewAddQuestion() {
      this.addQuestionDialogShow = false;
      let data = {
        title:"",
        optionsA:'',
        optionsB:'',
        optionsC:'',
        optionsD:'',
        answer:'',
        explanation:""
      };
      data.title = this.addQuestionForm.title;
      data.optionsA = this.addQuestionForm.optionsA;
      data.optionsB = this.addQuestionForm.optionsB;
      data.optionsC = this.addQuestionForm.optionsC;
      data.optionsD = this.addQuestionForm.optionsD;
      data.answer = this.addQuestionForm.answer;
      data.explanation = this.addQuestionForm.explanation;
      this.questionList.push(data);
    },
    // 保存阅读理解
    savaPassage() {
        let passageList = [];
        this.passageForm.questionList = this.questionList;
        passageList.push(this.passageForm);
        this.HttpPost('/passage/addlist',
        JSON.stringify({passageList}))
            .then(res => {
              if(res.data === 0) {
                this.$notify({
                  message:"上传失败",
                  type:"error"
                })
              }
              else {
                this.$notify({
                  message:"上传成功",
                  type:'success'
                });
                this.passageEdit = false;
                this.questionEdit = false;
                this.addQuestionEdit = false;
                this.$router.push("/home/passage/list");
              }
            })
    },
    deleteQuestion(index) {
      this.questionList.splice(index,1);
    }

  },
  created() {

  },
  beforeRouteLeave (to, from, next) {
    if (this.questionEdit === true || this.addQuestionEdit === true || this.passageEdit === true) {
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
  },
  mounted() {
    // 存一份this
    let _this = this;
    window.onbeforeunload = function(e) {
      // 那个路由页面需要，就把path的名字修改成那个，比如我当前页面的path是/vue
      if ( _this.passageEdit === true) {
        if (_this.$route.path === "/home/passage/add") {
          // 兼容IE8和Firefox 4之前的事件对象写法（不加也行，现在少有项目兼容老版本浏览器了）
          e = e || window.event;
          if (e) {
            e.returnValue = "returnValue属性值的文字不能自定义，写不写都行的";
          }
          // Chrome支持, Safari支持, Firefox 4版本以后支持, Opera 12版本以后支持 , IE 9版本以后支持
          return "returnValue属性值的文字不能自定义，写不写都行的";
        }
      }
    };
  },
  beforeDestroy() {
    // 离开页面时候再清除
    window.onbeforeunload = () => {};
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