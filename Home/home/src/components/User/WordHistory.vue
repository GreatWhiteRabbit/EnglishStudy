<template>
  <div>
    <el-row style="height: 40vh;width: 80vh;" v-show="option.series[0].data.length !== 0">
      <div id="box" style="height: 40vh;width: 80vh;"></div>
    </el-row>
    <el-row>
      <el-row style="margin-bottom: 10px;color: #7c7878;font-size: 20px;">
        记忆单词记录
        <el-radio-group @change="handleChange" v-model="type">
          <el-radio-button label="1">四级</el-radio-button>
          <el-radio-button label="2">六级</el-radio-button>
          <el-radio-button label="3">考研</el-radio-button>
        </el-radio-group>
        <el-popover
            placement="top-start"
            width="200"
            trigger="hover"
            content="重置单词记忆进度，重头开始记忆单词">
          <el-button
              v-show="total > 0 && wordList.length > 0"
              round @click="resetWordSchedule"
              style="margin-left: 30vh;background-color: #51cacc;color: white;font-size: 20px"
              slot="reference">重置进度
          </el-button>
        </el-popover>
      </el-row>
            <el-row v-show="total <= 0">
              <el-empty description="暂无数据"></el-empty>
            </el-row>
        <el-row v-for="item in wordList" :key="item.words">
          <el-row style="font-size: 18px;">
            <el-col style="color: #7c7878" :span="6">
              <span>{{item.words}}</span>
            </el-col>
            <el-col style="color: #f56c6c" :span="6">
              <span>/{{item.phonetic}}/</span>
            </el-col>
            <el-col style="color: #f19a9a" :span="12">
              <span>{{item.paraphrase}}</span>
            </el-col>
          </el-row>
          <el-divider></el-divider>
        </el-row>
        <el-row
            style="margin-left: 40vh;"
             v-show="total !== wordList.length && wordList.length !== 0">
          <el-col>
          <el-button @click="loadMore" style="border: none;color: #51cacc;font-size: 20px;">
            <i class="el-icon-arrow-down el-icon--left"></i>
            加载更多
            <i class="el-icon-arrow-down el-icon--right"></i>
          </el-button>
          </el-col>
        </el-row>
        <el-row
            style="margin-left: 40vh;color: #f56c6c;font-size: 20px"
            v-show="total === wordList.length">
          <el-col>
            <span>没有更多数据了</span>
          </el-col>
        </el-row>
    </el-row>
  </div>
</template>

<script>
export default {
  name: "ListeningHistory",
  data() {
    return {
      total: 0, // 单词总数
      currentSize: 20, // 当前页码
      currentPage: 1, // 当前页,
      wordList: [],// 至今记过的单词,
      type: 1,// 单词类型
      option: {
        tooltip: {},
        title: {
          text: '近10次记忆单词数'
        },
        xAxis: {
          type: 'category',
          data: []
        },
        yAxis: {
          type: 'value',
        },
        series: [
          {
            data: [],
            type: 'line'
          }
        ]
      },
      loading: false,
    }
  },
  methods: {

    // 获取记忆的所有单词
    getAllWord() {
      let userId = localStorage.getItem("userId");
      this.HttpGet('/wordRecord/history/' + userId + '/' + this.type + '/' + this.currentPage + '/' + this.currentSize)
          .then(res => {
            this.total = res.data.total;
            for(let i = 0; i < res.data.wordDTOs.length; i++) {
              this.wordList.push(res.data.wordDTOs[i]);
            }
          })
    },
    // 获取近10次记忆单词数
    getTotal() {
      let userId = localStorage.getItem("userId");
      this.HttpGet('/wordRecord/total/' + userId + '/' + this.type + '/' + 10)
          .then(res => {
            for (let i = 0; i < res.data.length; i++) {
              let x = "第" + (i + 1) + "次";
              this.option.xAxis.data.push(x);
            }
            this.option.series[0].data = res.data;
            this.setEchartsOptions1();
          })
    },

    // 监听el-radio-button的点击事件
    handleChange(val) {
      this.wordList = [];
      this.currentPage = 1;
      this.currentSize = 20;
      this.total = 0;
      this.getTotal();
      this.getAllWord();
    },
    // 重置进度
    resetWordSchedule() {
      let title = ["无", "四级", "六级", "考研"];
      this.$confirm('确定重置' + title[this.type] + "进度," + "此操作将重头开始记忆单词，是否继续执行此操作？",
          '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          }).then(() => {
        // 执行操作
        let userId = localStorage.getItem("userId");
        this.HttpPost('/wordRecord/update/' + userId + '/' + this.type)
            .then(res => {
              if (res.data === true) {
                this.$notify({
                  type: "success",
                  message: "重置进度成功",
                  title: '系统提示'
                });
                // 清除数据
                this.wordList = [];
              } else {
                this.$notify({
                  type: "error",
                  message: "服务器繁忙，请稍后",
                  title: "系统提示"
                })
              }
            })
      }).catch(() => {
        console.log("取消重置操作");
      });
    },
    // 绘制图标
    setEchartsOptions1() {
      var myChart = this.$echarts.init(document.getElementById("box"));
      myChart.setOption(this.option);
      myChart.resize(); //重绘,动态获取options时不会出现渲染异常
    },
   // 加载更多数据
  loadMore() {
    this.currentPage+=1;
    this.getAllWord();
  }

  },
  created() {
    this.getAllWord();
    this.getTotal();
  }
}
</script>

<style lang="stylus" scoped>

</style>