<template>
  <div>
  <el-row v-show="record.length !== 0">
    <div id="box" style="height: 40vh;width: 80vh;"></div>
  </el-row>
<el-row>
  <el-row style="margin-bottom: 10px;color: #7c7878;font-size: 20px;">
    阅读理解记录
  </el-row>
  <el-row v-show="record.length === 0">
    <el-empty description="暂无数据"></el-empty>
  </el-row>
  <el-row v-for="(item,index) in record" :key="item.recordId">
    <el-row >
    <el-col @click.native="goToDetail(index)" :span="4">
      <el-image
          style="width: 100px; height: 100px"
          :src="passageImage"
          fit="fit"></el-image>
    </el-col>
    <el-col @click.native="goToDetail(index)" style="text-align: left" :span="16">
      <el-row style="font-size: 20px;">
        <span>{{item.passageTitle}}</span>
      </el-row>
      <el-row style="margin-top: 8px;color: gray">
        <span>{{getTime(item.time)}}</span>
      </el-row>
     <el-row style="margin-top: 8px;color: #b0adaf">
       <span>准确率：</span>
       <span>{{item.accuracy}}</span>
     </el-row>
    </el-col>
    <el-col style="line-height: 100px" :span="4">
    <el-button @click="deleteRecordById(index)" style="background-color: #39d7da;color: white" round >
      删除
    </el-button>
    </el-col>
    </el-row>
    <el-divider></el-divider>
  </el-row>
</el-row>
    <el-row>
      <el-pagination
          background
          layout=" prev, pager, next"
          :current-page.sync="currentPage"
          @current-change="handleCurrentChange"
          @size-change="handleSizeChange"
          :total="total"
          style="display: flex;justify-content: center;margin-bottom:10px"
      >
      </el-pagination>
    </el-row>
  </div>
</template>

<script>
export default {
  name: "PassageHistory",
  data() {
    return {
      total: 1, // 单词总数
      currentSize: 20, // 当前页码
      currentPage: 1, // 当前页,
      record:[], // 做题记录
      accuracy:[], // 准确率
      passageImage:require('@/assets/image/passage.png'),
      option:{
        tooltip: {},
        title: {
          text: '近10次准确率'
        },
        xAxis: {
          type: 'category',
          data: []
        },
        yAxis: {
          type: 'value',
          data:[0,0.2,0.4,0.6,0.8,1.0]
        },
        series: [
          {
            data: [],
            type: 'line'
          }
        ]
      },
    }
  },
  methods:{
    // 获取阅读理解做题记录
    getPassageRecordList() {
      let userId = localStorage.getItem("userId");
      this.HttpGet('/passageRecord/' + userId + '/' + this.currentPage + "/" + this.currentSize)
          .then(res => {
            res.data.list.sort((a,b) => {
              const date1 =new Date(a.time);
              const  date2 = new Date(b.time);
              return (date2 - date1)
            });
            this.record = res.data.list;
            this.total = res.data.total;
          })
    },
    // 获取近10次准确率
    getAccuracy() {
      let userId = localStorage.getItem("userId");
      this.HttpGet('/passageRecord/accuracylist/' + userId + '/' + 10)
          .then(res => {
            this.accuracy = res.data;
            for(let i = 0; i < res.data.length; i++) {
              let x = "第" + (i+1) + "次";
              this.option.xAxis.data.push(x);
            }
            this.option.series[0].data = this.accuracy;
            this.setEchartsOptions1();
          })
    },
    // 调整页面尺寸
    handleSizeChange(val) {
      this.currentSize = val;
      // 重新获取数据
      this.getPassageRecordList();
    },
    // 调整页面大小
    handleCurrentChange(val) {
      this.currentPage = val;
     this.getPassageRecordList();
    },
    // 删除记录
    deleteRecordById(index) {
      this.$confirm('确定删除此记录', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
       // 执行删除操作
        let userId = localStorage.getItem("userId");
        let list = [];
        list.push(this.record[index].recordId);
        this.HttpPost('/passageRecord/delete/' + userId,JSON.stringify({list}))
            .then(res => {
              if(res.data === 0) {
                this.$notify({
                  type:'error',
                  message:"删除失败"
                })
              }
              else {
                this.$notify({
                  type:"success",
                  message:"删除成功"
                });
                this.record.splice(index,1);
              }
            })
      }).catch(() => {
        console.log("取消删除操作");
      });
    },
    // 前往详细页
    goToDetail(index) {
      let id = this.record[index].passageId;
      let newUrl = this.$router.resolve({
        path:'/detail/passage/' + id,
        query:{
          id:id
        }
      });
      window.open(newUrl.href, "_blank");

    },
    // 绘制图标
    setEchartsOptions1() {
      var myChart = this.$echarts.init(document.getElementById("box"));
      myChart.setOption(this.option);
      myChart.resize(); //重绘,动态获取options时不会出现渲染异常
    },
  },
  created() {
    this.getPassageRecordList();
    this.getAccuracy();
  },
  computed:{
    // 将时间戳转换为几小时前
    getTime() {
      return (time) => {
        const now = new Date();
        const date = new Date(time);
        const diff = (now - date)/1000; // 毫秒转换为秒
        console.log(diff);
        if (diff < 60) {
          return "刚刚";
        } else if (diff < 3600) {
          return Math.floor(diff / 60) + "分钟前";
        } else if (diff < 86400) {
          return Math.floor(diff / 3600) + "小时前";
        } else if (diff < 2592000) {
          return Math.floor(diff / 86400) + "天前";
        } else if(diff < 2592000 * 12) {
          return Math.floor(diff / 2592000) + "月前";
        }
        else {
          const year = date.getFullYear();
          const month = date.getMonth() + 1;
          const day = date.getDate();
          return `${year}-${month < 10 ? '0' + month : month}-${day < 10 ? '0' + day : day}`;
        }
      }
    }
  }
}
</script>

<style scoped>

</style>