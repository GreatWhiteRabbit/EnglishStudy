<template>
<div class="main">
  <el-row>
    <el-card  class="box-card">
      <el-row v-if="everydayList.length !== 0">
        <el-col style="width: 20vh;" :span="5" v-for="(item, index) in everydayList" :key="item" :offset="index > 0 ? 1 : 1">
         <router-link :to="'/detail/everyday/' + item.everyday_id" target="_blank">
           <el-card style="height: 30vh" :body-style="{ padding: '0px' }" shadow="hover">
             <img :src="everydayImage" class="image">
             <div style="padding: 12px; height: 60px">
               <div style="overflow: hidden;height: 25px;" >
                 <el-popover
                     placement="bottom-start"
                     trigger="hover"
                     :content="item.title">
                   <span slot="reference">{{item.title}}</span>
                 </el-popover>
               </div>
               <div class="bottom clearfix">
                 <time class="time">{{ timeChange(item.time) }}</time>
               </div>
             </div>
           </el-card>
         </router-link>
        </el-col>
      </el-row>
      <el-empty  v-else :image-size="300"></el-empty>
    </el-card>
  </el-row>
  <el-row >
    <el-pagination
        background
        layout="sizes, prev, pager, next"
        :page-size="currentSize"
        :current-page.sync="currentPage"
        @current-change="handleCurrentChange"
        @size-change="handleSizeChange"
        :page-sizes="[10,20,30,50,100]"
        :total="total"
        style="display: flex;justify-content: center;margin-bottom:10px"
    >
    </el-pagination>
  </el-row>
</div>
</template>

<script>
export default {
  name: "EveryDayList",
  data() {
    return {
      everydayList:[], // 获取每日英语部分列表
      everydayImage:require('@/assets/image/everyday.png'),
      total: 1, // 单词总数
      currentSize: 10, // 当前页码
      currentPage: 1, // 当前页,
    }
  },
  methods:{
    // 获取每日英语列表
    getEveryday() {
      this.HttpGet('/everyday/' + this.currentPage + '/' + this.currentSize)
          .then(res => {
            this.everydayList = res.data.list;
            this.total = res.data.total;
          })
    },
    // 调整页面尺寸
    handleSizeChange(val) {
      this.currentSize = val;
      // 重新获取数据
      this.getEveryday();
    },
    // 调整页面大小
    handleCurrentChange(val) {
      this.currentPage = val;
      this.getEveryday();
    },
  },
  created() {
    this.getEveryday();
  },
  computed: {
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
    }
  }
}
</script>

<style lang="stylus" scoped>

.main{
  margin-left 25vh;
  margin-right 15vh;
  background-color: white;
  height: 90vh;
  box-shadow:
      inset 0 -3em 3em rgba(240, 240, 245, 0.1),
      0 0 0 2px rgb(255, 255, 255),
      0.3em 0.3em 1em rgba(234, 234, 0, 0.3);
}

// card样式
.time {
  font-size: 13px;
  color: #999;
}

.bottom {
  margin-top: 13px;
  line-height: 12px;
}

.button {
  padding: 0;
  float: right;
}

.image {
  width: 100%;
  height 23vh;
  display: block;
}

.text {
  font-size: 14px;
}

.item {
  margin-bottom: 18px;
}

.clearfix:before,
.clearfix:after {
  display: table;
  content: "";
}
.clearfix:after {
  clear: both
}

.box-card {
  width: 100%;
  flex-wrap wrap;
}
</style>