<template>
<el-row>
    <el-col class="text" :style="active(questionAnswer,yourAnswer,'A')">A {{question.optionsA}}</el-col>
    <el-col class="text" :style="active(questionAnswer,yourAnswer,'B')">B {{question.optionsB}}</el-col>
    <el-col class="text" :style="active(questionAnswer,yourAnswer,'C')">C {{question.optionsC}}</el-col>
    <el-col class="text" :style="active(questionAnswer,yourAnswer,'D')">D {{question.optionsD}}</el-col>
</el-row>
</template>

<script>
export default {
  name: "MySelect",
  props:{
    question:{
      type: Object
    },
    questionAnswer:{
      type:String
    },
    yourAnswer:{
      type:String
    }
  },
  computed:{
    active(){
      return(questionAnswer,yourAnswer,options) => {

        if(yourAnswer === options) {
          if(questionAnswer === yourAnswer) {
            return {'background':'#67c23a','color':'#ffffff'};
          }
          else {
            return {'background':'#f56c6c','color':'#ffffff'};
          }
        }

      }
    }
  }
}
</script>

<style lang="stylus" scoped>
.text {
  height 30px;
  width 100%;
  font-size 20px;
  background-color white;
  margin-top 10px;
  border-radius 2px;
  border solid 1px rgb(220, 223, 230);
  line-height 30px;
  color: gray;
}
</style>