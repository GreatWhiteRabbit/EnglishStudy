<template>
  <div>
    <el-row>
      <el-row style="margin-bottom: 10px;color: #7c7878;font-size: 20px;">
        每日新闻记录
      </el-row>
      <el-row v-show="record.length === 0">
        <el-empty description="暂无数据"></el-empty>
      </el-row>
      <el-row v-for="(item,index) in record" :key="item.recordId">
        <el-row >
          <el-col @click.native="goToDetail(index)" :span="4">
            <el-image
                style="width: 100px; height: 100px"
                :src="everydayImage"
                fit="fit"></el-image>
          </el-col>
          <el-col @click.native="goToDetail(index)" style="text-align: left" :span="16">
            <el-row style="font-size: 20px;">
              <el-row >
                <span>{{item.title}}</span>
              </el-row>
              <el-row style="color: #33CC99">
                <span>{{item.titleTranslations}}</span>
              </el-row>
            </el-row>
            <el-row style="margin-top: 8px;color: gray">
              <span>{{getTime(item.time)}}</span>
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
  name: "EverydayHistory",
  data() {
    return {
      total: 1, // 数据总数
      currentSize: 10, // 当前页码
      currentPage: 1, // 当前页,
      record:[], // 阅读记录
      everydayImage:require('@/assets/image/everyday.png'),
    }
  },
  methods:{

    // 获取每日英语阅读记录
    getEverydayRecord() {
      let userId = localStorage.getItem("userId");
      this.HttpGet('/everydayRecord/' + userId + '/' + this.currentPage + '/' + this.currentSize)
          .then(res => {
            // 根据时间排序
            res.data.recordList.sort((a,b) => {
              const date1 =new Date(a.time);
              const  date2 = new Date(b.time);
              return (date2 - date1)
            });
            this.record = res.data.recordList;
            this.total = res.data.total;
          })
    },

    // 调整页面尺寸
    handleSizeChange(val) {
      this.currentSize = val;
      // 重新获取数据
      this.getEverydayRecord();
    },
    // 调整页面大小
    handleCurrentChange(val) {
      this.currentPage = val;
      this.getEverydayRecord();
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
        this.HttpPost('/everydayRecord/delete/' + userId,JSON.stringify({list}))
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
      let id = this.record[index].everydayId;
      let newUrl = this.$router.resolve({
        path:'/detail/everyday/' + id,
        query:{
          id:id
        }
      });
      window.open(newUrl.href, "_blank");
    },

  },
  created() {
    this.getEverydayRecord();
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