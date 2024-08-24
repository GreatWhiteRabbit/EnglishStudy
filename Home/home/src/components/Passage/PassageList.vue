<template>
<div class="main">
  <el-row v-if="list.length !== 0">
    <el-card class="box-card">
      <el-row>
        <el-col style="width: 20vh;" v-for="(item, index) in list" :key="item" :offset="index > 0 ? 1 : 1">
          <router-link target="_blank" :to="'/detail/passage/' + item.passageId">
            <el-card style="height: 30vh" :body-style="{ padding: '0px' }" shadow="hover">
              <img :src="passageImage" class="image">
              <div style="padding: 12px;height: 32px;overflow: hidden;">
                <el-popover
                    placement="bottom-start"
                    trigger="hover"
                    :content="item.title">
                  <span slot="reference" style="overflow: hidden">{{item.title}}</span>
                </el-popover>
              </div>
            </el-card>
          </router-link>
        </el-col>
      </el-row>
    </el-card>
  </el-row>
  <el-empty  v-else :image-size="300"></el-empty>
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
  name: "PassageList",
  data() {
    return {
      total: 1, // 单词总数
      currentSize: 20, // 当前页码
      currentPage: 1, // 当前页,
      list:[],
      passageImage:require('@/assets/image/passage.png')
    }
  },
  methods:{
    // 获取阅读理解数据
    getPassage() {
      this.HttpGet('/passage' + '/' + this.currentPage + "/" + this.currentSize)
          .then(res => {
            this.list = res.data.passageDTOList;
            this.total = res.data.total;
          })
    },
    // 调整页面尺寸
    handleSizeChange(val) {
      this.currentSize = val;
      // 重新获取数据
      this.getPassage();
    },
    // 调整页面大小
    handleCurrentChange(val) {
      this.currentPage = val;
      this.getPassage();
    },
  },
  created() {
    this.getPassage();
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
  height 24vh;
  width: 100%;
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