<template>
<!--设置识别/n换行  -->
  <div id="app" style="white-space: pre-line;">
    <el-backtop :visibility-height="0"><div  style="{
        height: 100%;
        width: 100%;
        background-color: #f2f5f6;
        box-shadow: 0 0 6px rgba(0,0,0, .12);
        text-align: center;
        line-height: 40px;
        color: #1989fa;
      }" class="el-icon-top"></div></el-backtop>
  <router-view/>
  </div>
</template>

<script>


export default {
  name: 'App',
  components: {

  }
}
</script>

<style lang="stylus">

@import '~@/assets/style/reset.css';
:root {
  /* 主题色 */
  --main-1: #fff3f0;
  --main-2: #ffd4cc;
  --main-3: #ffafa3;
  --main-4: #ff887a;
  /* 网站颜色搭配 */
  --main-5: #f56c6c;
  --main-6: #8EC5FC;
  --main-7: #E0C3FC;
  --main-8: #e4e7ed;
  --main-9: #c0c4cc;
  --main-10: #606266;
}
@media(max-width: 768px ){
  .el-dialog{
    width: 95%!important;
  }
}
</style>
