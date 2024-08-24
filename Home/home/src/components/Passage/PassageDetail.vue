<template>
  <div>
    <el-skeleton :rows="30" animated v-show="loadShow"/>
    <!--  显示阅读理解内容-->
    <el-container>
      <!--  阅读理解内容-->
      <el-container>
        <el-header>
          <div class="content">
            <el-row class="container-marked">
              <el-col class="blog-content" :span="22" :offset="1">
                <div>
                  <header class="article-header blog-animation">
                    <el-row>
                      <div class="istitle">
                        <h1
                            class="article-title"
                        >{{ passage.title }}</h1>
                      </div>
                    </el-row>

                  </header>
                  <hr/>

                </div>
              </el-col>
            </el-row>
          </div>
        </el-header>
        <!--样式调到我想吐-->
        <el-main style="height: 0">
          <div class="content">
            <el-row class="container-marked">
              <el-col class="blog-content" :span="22" :offset="1">
                <div id="write" class="blog-content blog-animation" style="padding-bottom: 20px; text-align: left">
                  <el-row style="line-height: 1.5;color: gray">
                    <span>{{ passage.content }}</span>
                  </el-row>
                  <el-row v-for="(question,index) in passage.questionList" :key="question.questionId">
                    <el-row style="margin-top: 20px;" :id="setId(index)">
                      <el-col>
                        <sapn>第{{ getIndex(index) + 1 }}题 {{ question.title }}</sapn>
                      </el-col>
                    </el-row>
                    <el-row v-if="!isDone">
                      <el-radio-group v-model="radio[getIndex(index)]">
                        <el-col style="margin-top: 10px">
                          <el-radio-button label="A">A {{ question.optionsA }}</el-radio-button>
                        </el-col>
                        <el-col style="margin-top: 10px">
                          <el-radio-button label="B">B {{ question.optionsB }}</el-radio-button>
                        </el-col>
                        <el-col style="margin-top: 10px">
                          <el-radio-button label="C">C {{ question.optionsC }}</el-radio-button>
                        </el-col>
                        <el-col style="margin-top: 10px">
                          <el-radio-button label="D">D {{ question.optionsD }}</el-radio-button>
                        </el-col>
                      </el-radio-group>
                    </el-row>
                    <!--                      完成过做题的显示下面的信息-->
                    <el-row v-else>
                      <my-select :question="question" :question-answer="answer[index].answer"
                                 :your-answer="record.list.list[index].answer"/>
                      <el-row style="margin-top: 20px; text-align: center;color: #51cacc">
                        <div @click="showAnswerButton(index)">
                          <el-col >
                            点击查看答案
                            <i class="el-icon-caret-bottom el-icon-right"></i>
                          </el-col>
                        </div>

                      </el-row>
                      <el-row v-show="answerShow[index] === true">
                        <el-row >
                          <el-col style="font-size: 18px; margin-top: 20px;line-height: 1.5;">答案: {{answer[index].answer}}</el-col>
                        </el-row>
                        <el-col style="font-size: 18px; margin-top: 20px;line-height: 1.5; color: green">解释: {{answer[index].explanation}}</el-col>
                      </el-row>
                    </el-row>
                  </el-row>
                </div>

              </el-col>
            </el-row>
          </div>
        </el-main>
      </el-container>
      <!--   侧边栏 -->
      <el-aside width="400px">
        <el-row style="margin-top: 100px">
          <el-col>
            <el-button v-show="!isDone" @click="beforeSubmit" type="primary">提交</el-button>
            <el-button @click="deleteRecordById" v-show="isDone" type="danger" icon="el-icon-delete">重做</el-button>
          </el-col>
        </el-row>
        <el-row>
          <!--          没有做过题目显示下面的按钮样式-->
          <div v-if="!isDone" class="selectButton">
            <div style="width: 70px;height: 50px" v-for="item in questionCount" :key="item">
              <el-button v-if="radio[item-1]" type="success" @click="itemButton(item)">{{ item }}</el-button>
              <el-button v-else @click="itemButton(item)">{{ item }}</el-button>
            </div>
          </div>
          <!--          做过题目显示下面的按钮样式-->
          <div v-else class="selectButton">
            <div style="width: 70px;height: 50px" v-for="item in questionCount" :key="item">
              <el-button v-if="answerIsTrue(item-1)" type="success" @click="itemButton(item)">{{ item }}</el-button>
              <el-button v-else type="danger" @click="itemButton(item)">{{ item }}</el-button>
            </div>
          </div>
        </el-row>
        <el-row v-show="isDone" style="color: gray;font-size: 18px;">
          <el-col>
            <span>
              上次做题时间
              <i class="el-icon-time el-icon--right"></i>
              {{ timeChange(record.time) }}
            </span>
          </el-col>
        </el-row>
        <el-row v-show="isDone" style="color: black; font-size: 20px;margin-top: 10px;">
          <el-col>
            <span>
              准确率：
              {{ record.accuracy * 100 }}%
            </span>
          </el-col>
        </el-row>
      </el-aside>
    </el-container>
  </div>
</template>

<script>
/*
* 阅读理解试题详情
* */

import MySelect from "@/components/MySelect";

export default {

  name: "PassageDetail",
  components: {MySelect},
  data() {
    return {
      passageId: -1,
      passage: {},
      loadShow: true, // 加载,
      questionCount: 0, // 选择题的总数,
      radio: [],
      isDone: false, // 判断是否已经做过这份试题
      record: {}, // 做题记录,
      answer: [], // 答案
      answerShow: [] // 显示答案
    }
  },
  methods: {
    // 获取阅读理解试题详情
    getPassageById() {
      this.HttpGet('/passage/' + this.passageId)
          .then(res => {
            if (res.success === false) {
              this.$router.push('/404');
            } else {
              this.passage = res.data;
              // 修改页面的title
              document.title = res.data.title + "-英语学习平台";
              // 根据passageId获取题目
              this.HttpGet('question/passage/' + this.passageId)
                  .then(res => {
                    this.passage.questionList = res.data;
                    // 获取所有的试题数
                    this.questionCount = this.passage.questionList.length;
                    // 设置radio的个数
                    for (let i = 0; i < this.questionCount; i++) {
                      this.radio.push("");
                    }
                  })
            }
            this.loadShow = false;
          })
    },
    // 获取做题记录
    getPassageRecord() {
      if (localStorage.getItem("userId") === null && localStorage.getItem("token") === null) {
        console.log("用户未登录，不能获取阅读理解记录");
      } else {
        let userId = localStorage.getItem("userId");
        // 从服务器中获取数据
        this.HttpGet('/passageRecord/record/' + userId + '/' + this.passageId)
            .then(res => {
              if (res.success === false) {
                console.log("用户还未做过阅读理解");
              } else {
                this.record = res.data;
                // 获取答案
                this.HttpGet('/question/answer/' + this.passageId)
                    .then(res => {
                      this.answer = res.data;
                    })
                this.isDone = true;
              }
            })
      }
    },

    // 点击右侧的选择题题号跳转到对应的选择题位置
    itemButton(index) {
      //找到锚点id
      let selectId = "question" + index;
      let toElement = document.getElementById(selectId);
      //如果对应id存在，就跳转
      if (selectId) {
        console.log(toElement, "toElement");
        toElement.scrollIntoView({
          block: "start",//默认跳转到顶部
          behavior: "smooth"//滚动的速度
        });
      }

    },
    // 提交答题记录到服务器前的操作
    beforeSubmit() {
      // 先判断用户是否登录，如果用户没有登录，跳转到登录页面
      if (localStorage.getItem("userId") === null || localStorage.getItem("token") === null) {
        this.$router.push('/login');
      }
      // 将用户答题情况提交到服务器
      else {
        // 1.先判断是否已经做过该试题但是还没有删除，那么先删除做题记录再重新提交
        if (this.isDone === true) {
          this.$confirm('此操作将会覆盖原来的答题记录, 是否继续?', '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          }).then(() => {
            this.deleteRecordById();
            this.submit();
          }).catch(() => {
            console.log("取消覆盖原来的答题记录")
          })
        } else {
          this.submit();
        }
      }
    },
    // 提交到服务器
    submit() {
      // 先判断是否完成的所有的题目答题
      let finish = true;
      for (let i = 0; i < this.radio.length; i++) {
        if (this.radio[i] === "") {
          finish = false;
          break;
        }
      }
      if (finish === false) {
        this.$notify({
          type: 'error',
          message: "还有未完成的题目，请全部完成后提交",
          title: "系统提示"
        });
      } else {
        let userId = localStorage.getItem("userId");
        // 获取答案
        this.HttpGet('/question/answer/' + this.passageId)
            .then(res => {
              this.answer = res.data;
            })
        let list = [];
        for (let i = 0; i < this.radio.length; i++) {
          let answerDetail = {
            questionId: '',
            answer: ''
          }
          answerDetail.questionId = this.passage.questionList[i].questionId;
          answerDetail.answer = this.radio[i];
          list.push(answerDetail);
        }
        // 将答题记录提交到服务器
        this.HttpPost('/passageRecord/add/' + userId + '/' + this.passageId, JSON.stringify({list}))
            .then(res => {
              if (res.success === true) {
                let accuracy = 0;
                for (let i = 0; i < this.radio.length; i++) {
                  if (this.radio[i] === this.answer[i].answer) {
                    accuracy++;
                  }
                }
                accuracy = accuracy / this.radio.length;
                // 添加准确率到服务器
                this.HttpPost("/passageRecord/accracy/" + userId + "/" + this.passageId + '/' + accuracy)
                    .then(res => {
                      // 获取做题记录
                      this.getPassageRecord();
                      this.isDone = true;
                    })
              } else {
                this.$notify({
                  type: 'error',
                  message: "服务器繁忙，请稍后",
                  title: "系统提示"
                })
              }
            })
      }
    },
    // 删除做题记录
    deleteRecordById() {
      let userId = localStorage.getItem("userId");
      let list = [];
      list.push(this.record.recordId );
      this.HttpPost('/passageRecord/delete/' + userId,JSON.stringify({list}))
          .then(res => {
            if(res.data === 0) {
              this.$notify({
                type:'error',
                message:"服务器繁忙，请稍后",
                title:'系统提示'
              })
            }
            else {
              this.$notify({
                type:'success',
                message:"重做设置成功",
                title:"系统提示"
              });
              this.isDone = false;
            }
          })
    },
    // 显示答案
    showAnswerButton(index) {

      this.answerShow = [];
      for(let i = 0; i < this.radio.length; i++) {
        this.answerShow.push(false);
      }
      this.answerShow[index] = true;
    }

  },
  created() {
    this.passageId = this.$route.params.id;
    this.getPassageById();
    // 获取做题记录
    this.getPassageRecord();
  },
  computed: {
    getIndex() {
      return (index) => {
        return index;
      }
    },
    setId() {
      return (index1) => {
        let index = index1;
        index += 1;
        return "question" + index;
      }
    },
    timeChange() {
      return (time) => {
        var date = new Date(time);
        let Y = date.getFullYear() + '-';
        let M = (date.getMonth() + 1 < 10 ? '0' + (date.getMonth() + 1) : date.getMonth() + 1) + '-';
        let D = date.getDate() + ' ';
        let h = date.getHours() + ':';
        let m = date.getMinutes() + ':';
        let s = date.getSeconds();
        return Y + M + D + h + m + s;
      }
    },
    // 判断答案是否正确
    answerIsTrue() {
      return (index) => {
        if (this.record.list.list[index].answer === this.answer[index].answer) {
          return true;
        }
        return false;
      }
    }
  }
}
</script>

<style lang="stylus" scoped>

.selectButton {
  justify-content center;
  width 390px;
  display flex;
  flex-wrap wrap;
  float left;
  margin-top 20px;
}

.el-header {
  height: auto !important;
}

.el-aside {
  background-color: #ffffff;
  color: #333;
  text-align: center;
  border-left: #7de0f7;
  border-left-style: solid;
  line-height 100%;
  height 90vh;
}

.el-main {
  color: #333;
  text-align: center;
  line-height: 100%;
  padding: 0 !important;
  height: calc(100vh - 100px);
  margin-left 2vh;
}

/*自定义el-radio-button样式*/
::v-deep .el-radio-button__inner {
  width: 800px;
  height: 40px;
  /*  border: 0 !important;*/
  font-size: 18px;
  font-weight: 400;
  color: #1d1c1c;
  line-height: 14px;
  outline: none;
  box-shadow: none;
  text-align: left;
  background-color: rgb(245, 245, 255)
}

// 修改激活后的样式
::v-deep .el-radio-button__orig-radio:checked + .el-radio-button__inner {
  background: rgb(64, 158, 255);
  border: 0 !important;
  color: white;
  line-height: 14px;
  outline: none;
  box-shadow: none;
  text-align: left;
}

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
  padding: 0 0 25px 0;
  font-size 20px;

  .container-marked {
    border-radius: 6px;
    background-color #fff
    margin auto;
    max-width: 1320px;
    width: 95%;

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